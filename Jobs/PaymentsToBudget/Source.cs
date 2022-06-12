using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PaymentsToBudget.DataSchema;
using PaymentsToBudget.Helpers;
using Yoda.Application.Queries;
using Yoda.Interfaces;
using YodaApp.DbQueues;
using YodaHelpers.OfflineTerminal;
using YodaHelpers.Payments;
using YodaQuery;
using static PaymentsToBudget.Helpers.PaymentHelper;

namespace PaymentsToBudget {

    public class PaymentsToBudgetJobInput {
        public PaymentsToBudgetJobInput()
        {
        }
    }

    public class PgQueue {
        public const string PaymentsToBudgetJobQueue = "PaymentsToBudgetJobQueue";
    }

    public static class PaymentsToBudgetJobs {
        public static PgJobs<PaymentsToBudgetJobInput> PaymentsToBudgetJob = new PgJobs<PaymentsToBudgetJobInput>(PgQueue.PaymentsToBudgetJobQueue, "dbJobs", "Отправляет оплату в бюджет и переплату возвращает победителю");

        public static IServiceCollection AddPaymentsToBudgetJobsConsumer(this IServiceCollection services)
        {
            return services.AddDbQueueConsumer(jobs => jobs
                .DbQueue(PaymentsToBudgetJob)
                .Consume(async (input, env) => {
                    try
                    {
                        JobSource.Work(env);

                        var now = DateTime.Now;
                        var newStartAfterDate = now.AddHours(2);

                        return JobFate.Repeat(newStartAfterDate, newStartAfterDate.AddDays(1));
                    }
                    catch (Exception ex)
                    {
                        return JobFate.Retry((env.JobContext.RetriesCount + 1) * 10, ex.ToString());
                    }
                }, batchSize: 1)
            );
        }

    }

    public class JobSource {

        public static void Work(DbJobConsumeEnvironment env)
        {
            env.Logger.LogInformation("Starting process...");

            var tbPaymentMatches = new TbPaymentMatches();
            tbPaymentMatches.AddFilter(t => t.flStatus, PaymentMatchStatus.Linked);
            tbPaymentMatches.Order(t => t.flId, OrderType.Desc);
            var paymentMatches = tbPaymentMatches.GetPaymentMatches(env.QueryExecuter).OrderByDescending(paymentMatch => paymentMatch.flId);

            foreach(var paymentMatch in paymentMatches)
            {
                DoPaymentMatch(paymentMatch, env);
            }

            env.Logger.LogInformation("End process...");
        }

        public static void DoPaymentMatch(PaymentMatchModel paymentMatch, DbJobConsumeEnvironment env)
        {
            try
            {
                env.Logger.LogInformation($"{paymentMatch.flId} ({paymentMatch.flDateTime}) working...");

                paymentMatch.GetPurposeData(env.QueryExecuter, out var flAgreementId, out var flAgreementNumber, out var flTradeId, out var flAuctionId, out string flWinnerFullName);

                var tbPaymentMatchesUpdate = new TbPaymentMatches()
                    .AddFilter(t => t.flId, paymentMatch.flId)
                    .Update()
                    ;

                if (paymentMatch.flPaymentItems.Any(x => !x.flIsGuarantee))
                {

                    var paymentsProvider = new TraderesourcesPaymentsProvider(paymentMatch.flRequisites.flXin, paymentMatch.flRequisites.flName, env.ServiceProvider.GetRequiredService<IQueryExecuterProvider>());

                    if (paymentMatch.flHasSendAmount)
                    {
                        var matchBlockResult = paymentMatch.flMatchBlockResult;

                        if (matchBlockResult == null)
                        {
                            matchBlockResult = paymentsProvider.BlockMatch(paymentMatch.flMatchResult, paymentMatch.flSendAmount, $"{paymentMatch.flId}-traderesources-block");
                            env.Logger.LogInformation($"{paymentMatch.flId} match blocked...");

                            tbPaymentMatchesUpdate = new TbPaymentMatches()
                                .AddFilter(t => t.flId, paymentMatch.flId)
                                .Update()
                                ;
                            tbPaymentMatchesUpdate.SetT(t => t.flMatchBlockResult, matchBlockResult);
                            tbPaymentMatchesUpdate.Execute(env.QueryExecuter);

                            paymentsProvider.SettleBlock(matchBlockResult);
                            env.Logger.LogInformation($"{paymentMatch.flId} match block settled...");
                        }
                        else
                        {
                            env.Logger.LogInformation($"{paymentMatch.flId} match already has settled block...");
                        }

                        var order = new Order(
                            $"{paymentMatch.flId}-traderesources-order",
                            paymentMatch.flMatchResult.MatchId,
                            new[] { matchBlockResult },
                            RefStatementTransactionCommand.Values.TransferPayment,
                            paymentMatch.flRequisites.flXin,
                            paymentMatch.flRequisites.flName,
                            paymentMatch.flRequisites.flBik,
                            paymentMatch.flRequisites.flIban,
                            paymentMatch.flRequisites.flKbe.ToString(),
                            paymentMatch.flRequisites.flKnp.ToString(),
                            paymentMatch.flRequisites.flKbk,
                            $"Оплата победителя торгов №{flTradeId} (в ЭТП №{flAuctionId}) {flWinnerFullName} согласно дог. №{flAgreementNumber} по закреп. земель, охот. уг. или рыб. водоемов",
                            paymentMatch.flSendAmount,
                            false
                        );
                        var orderResult = paymentsProvider.SendOrder(order);
                        env.Logger.LogInformation($"{paymentMatch.flId} order sent...");
                    }

                    if (paymentMatch.flSendOverpayment)
                    {
                        var overpaymentMatchBlockResult = paymentMatch.flOverpaymentMatchBlockResult;

                        if (overpaymentMatchBlockResult == null)
                        {
                            overpaymentMatchBlockResult = paymentsProvider.BlockMatch(paymentMatch.flMatchResult, paymentMatch.flOverpaymentSendAmount.Value, $"{paymentMatch.flId}-traderesources-block-overpayment");
                            env.Logger.LogInformation($"{paymentMatch.flId} overpayment match blocked...");

                            tbPaymentMatchesUpdate = new TbPaymentMatches()
                                .AddFilter(t => t.flId, paymentMatch.flId)
                                .Update()
                                ;
                            tbPaymentMatchesUpdate.SetT(t => t.flOverpaymentMatchBlockResult, overpaymentMatchBlockResult);
                            tbPaymentMatchesUpdate.Execute(env.QueryExecuter);

                            paymentsProvider.SettleBlock(overpaymentMatchBlockResult);
                            env.Logger.LogInformation($"{paymentMatch.flId} overpayment match block settled...");
                        }
                        else
                        {
                            env.Logger.LogInformation($"{paymentMatch.flId} overpayment match already has settled block...");
                        }

                        var overpaymentOrder = new Order(
                            $"{paymentMatch.flId}-traderesources-order-overpayment",
                            paymentMatch.flMatchResult.MatchId,
                            new[] { overpaymentMatchBlockResult },
                            RefStatementTransactionCommand.Values.RefundPart,
                            paymentMatch.flOverpaymentRequisites.flXin,
                            paymentMatch.flOverpaymentRequisites.flName,
                            paymentMatch.flOverpaymentRequisites.flBik,
                            paymentMatch.flOverpaymentRequisites.flIban,
                            paymentMatch.flOverpaymentRequisites.flKbe.ToString(),
                            paymentMatch.flOverpaymentRequisites.flKnp.ToString(),
                            "",
                            $"Возврат переплаты победителя торгов №{flTradeId} (в ЭТП №{flAuctionId}) {flWinnerFullName} согласно дог. №{flAgreementNumber} по закреп. земель, охот. уг. или рыб. водоемов",
                            paymentMatch.flOverpaymentSendAmount.Value,
                            false
                        );
                        var overpaymentOrderResult = paymentsProvider.SendOrder(overpaymentOrder);
                        env.Logger.LogInformation($"{paymentMatch.flId} overpayment order sent...");
                    }
                }

                tbPaymentMatchesUpdate = new TbPaymentMatches()
                   .AddFilter(t => t.flId, paymentMatch.flId)
                   .Update()
                   ;
                tbPaymentMatchesUpdate.SetT(t => t.flStatus, PaymentMatchStatus.LinkedAndSent);
                tbPaymentMatchesUpdate.Execute(env.QueryExecuter);

                env.Logger.LogInformation($"{paymentMatch.flId} end");

            }
            catch (Exception ex)
            {
                env.Logger.LogInformation(ex.Message);
                if (!ex.Message.Contains("Сумма блокировки превышает")) {
                    env.Logger.LogInformation(ex.StackTrace);
                }
            }
        }

    }

}
