using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using PaymentsToBudget.DataSchema;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Yoda.Application.Queries;
using Yoda.Interfaces;
using Yoda.Interfaces.Forms;
using Yoda.Interfaces.Forms.Components;
using YodaHelpers.Fields;
using YodaHelpers.Payments;
using YodaQuery;
using PaymentModel = PaymentsToBudget.DataSchema.PaymentModel;

namespace PaymentsToBudget.Helpers {
    public static class PaymentHelper
    {
        public static PaymentMatchModel GetPaymentMatchModel(this SelectFirstResultProxy<TbPaymentMatches> paymentItemRow)
        {
            if (!paymentItemRow.IsFirstRowExists)
            {
                return null;
            }

            return new PaymentMatchModel
            {
                flId = paymentItemRow.GetVal(t => t.flId),
                flPaymentId = paymentItemRow.GetVal(t => t.flPaymentId),
                flDateTime = paymentItemRow.GetVal(t => t.flDateTime),
                flPaymentItems = paymentItemRow.GetVal(t => t.flPaymentItems),
                flStatus = paymentItemRow.GetVal(t => t.flStatus),
                flMatchResult = paymentItemRow.GetValOrNull(t => t.flMatchResult),
                flAmount = paymentItemRow.GetVal(t => t.flAmount),
                flGuaranteeAmount = paymentItemRow.GetVal(t => t.flGuaranteeAmount),
                flRealAmount = paymentItemRow.GetVal(t => t.flRealAmount),
                flHasSendAmount = paymentItemRow.GetVal(t => t.flHasSendAmount),
                flSendAmount = paymentItemRow.GetVal(t => t.flSendAmount),
                flMatchBlockResult = paymentItemRow.GetValOrNull(t => t.flMatchBlockResult),
                flRequisites = paymentItemRow.GetVal(t => t.flRequisites),
                flOverpayment = paymentItemRow.GetVal(t => t.flOverpayment),
                flOverpaymentAmount = paymentItemRow.GetValOrNull(t => t.flOverpaymentAmount),
                flSendOverpayment = paymentItemRow.GetVal(t => t.flSendOverpayment),
                flOverpaymentSendAmount = paymentItemRow.GetValOrNull(t => t.flOverpaymentSendAmount),
                flOverpaymentMatchBlockResult = paymentItemRow.GetValOrNull(t => t.flOverpaymentMatchBlockResult),
                flOverpaymentRequisites = paymentItemRow.GetValOrNull(t => t.flOverpaymentRequisites)
            };
        }

        public static PaymentMatchModel GetPaymentMatchModelFirstOrDefault(this TbPaymentMatches tbPaymentItems, IQueryExecuter queryExecuter, ITransaction transaction = null)
        {
            return tbPaymentItems.SelectFirstOrDefault(t => t.Fields.ToFieldsAliases(), queryExecuter, transaction).GetPaymentMatchModel();
        }

        public static PaymentMatchModel[] GetPaymentMatches(this TbPaymentMatches tbPaymentItems, IQueryExecuter queryExecuter, ITransaction transaction = null)
        {
            return tbPaymentItems.Select(t => t.Fields.ToFieldsAliases(), queryExecuter, transaction).Select(row => row.GetPaymentMatchModel()).OrderBy(item => item.flDateTime).ToArray();
        }

        public static PaymentModel GetPaymentModel(this SelectFirstResultProxy<TbPayments> paymentRow, IQueryExecuter queryExecuter, ITransaction transaction = null)
        {
            if (!paymentRow.IsFirstRowExists)
            {
                return null;
            }

            var paymentModel = new PaymentModel
            {
                flPaymentId = paymentRow.GetVal(t => t.flPaymentId),
                flAgreementId = paymentRow.GetVal(t => t.flAgreementId),
                flPaymentStatus = paymentRow.GetVal(t => t.flPaymentStatus),
                flPayAmount = paymentRow.GetVal(t => t.flPayAmount),
                flPaidAmount = paymentRow.GetVal(t => t.flPaidAmount),
                flPaymentMatches = new TbPaymentMatches()
                    .AddFilter(t => t.flPaymentId, paymentRow.GetVal(t => t.flPaymentId))
                    .GetPaymentMatches(queryExecuter, transaction)
            };

            return paymentModel;
        }
        public static PaymentModel GetPaymentModelFirstOrDefault(this TbPayments tbPayments, IQueryExecuter queryExecuter, ITransaction transaction = null)
        {
            return tbPayments.SelectFirstOrDefault(t => t.Fields.ToFieldsAliases(), queryExecuter, transaction).GetPaymentModel(queryExecuter, transaction);
        }

        public static AgreementsModel GetAgreementsModelFirstOrDefault(this TbAgreements tbAgreements, IQueryExecuter queryExecuter, ITransaction transaction = null)
        {
            var r = tbAgreements.SelectFirstOrDefault(t => t.Fields.ToFieldsAliases(), queryExecuter, transaction);

            if (!r.IsFirstRowExists)
            {
                return null;
            }

            return new AgreementsModel
            {
                flAgreementId = r.GetVal(t => t.flAgreementId),
                flAgreementNumber = r.GetVal(t => t.flAgreementNumber),
                flAgreementRevisionId = r.GetVal(t => t.flAgreementRevisionId),
                flAgreementType = r.GetVal(t => t.flAgreementType),
                flAgreementStatus = r.GetVal(t => t.flAgreementStatus),
                flAgreementCreateDate = r.GetVal(t => t.flAgreementCreateDate),
                flAgreementSignDate = r.GetValOrNull(t => t.flAgreementSignDate),
                flObjectId = r.GetVal(t => t.flObjectId),
                flObjectType = r.GetVal(t => t.flObjectType),
                flTradeId = r.GetValOrNull(t => t.flTradeId),
                flTradeType = r.GetVal(t => t.flTradeType),
                flAgreementCreatorBin = r.GetVal(t => t.flAgreementCreatorBin),
                flSellerBin = r.GetVal(t => t.flSellerBin),
                flWinnerXin = r.GetVal(t => t.flWinnerXin)
            };
        }
        public static AgreementsModel GetAgreementsModel(int agreementId, IQueryExecuter queryExecuter)
        {
            return new TbAgreements()
                .AddFilter(t => t.flAgreementId, agreementId)
                .GetAgreementsModelFirstOrDefault(queryExecuter, null);
        }
        
        public static void GetPurposeData(this PaymentMatchModel paymentMatchModel, IQueryExecuter queryExecuter, out int flAgreementId, out string flAgreementNumber, out int? flTradeId, out int flAuctionId, out string flWinnerFullName)
        {
            var payment = new TbPayments()
                .AddFilter(t => t.flPaymentId, paymentMatchModel.flPaymentId)
                .GetPaymentModelFirstOrDefault(queryExecuter);

            flAgreementId = new TbPayments().AddFilter(t => t.flPaymentId, paymentMatchModel.flPaymentId).SelectScalar(t => t.flAgreementId, queryExecuter).Value;
            var flAgreement = GetAgreementsModel(flAgreementId, queryExecuter);
            flAgreementNumber = flAgreement.flAgreementNumber;
            flTradeId = flAgreement.flTradeId;
            Enum.TryParse<RefAgreementTypes.Values>(new RefAgreementTypes().Search(flAgreement.flAgreementType).Parent.Value.ToString(), out var flTradeType);
            var tbTradeChanges = new TbTradeChanges(TbTradeChanges.Sources.dbLands);
            var tbTrades = new TbTrades(TbTrades.Sources.dbLands);
            switch (flTradeType)
            {
                case RefAgreementTypes.Values.Lands:
                    {
                        tbTradeChanges = new TbTradeChanges(TbTradeChanges.Sources.dbLands);
                        tbTrades = new TbTrades(TbTrades.Sources.dbLands);
                        break;
                    }
                case RefAgreementTypes.Values.Hunt:
                    {
                        tbTradeChanges = new TbTradeChanges(TbTradeChanges.Sources.dbHunting);
                        tbTrades = new TbTrades(TbTrades.Sources.dbHunting);
                        break;
                    }
                case RefAgreementTypes.Values.Fish:
                    {
                        tbTradeChanges = new TbTradeChanges(TbTradeChanges.Sources.dbFishing);
                        tbTrades = new TbTrades(TbTrades.Sources.dbFishing);
                        break;
                    }
                default:
                    {
                        throw new NotImplementedException($"Unknown trade type {flTradeType}");
                    }
            }
            flAuctionId = tbTradeChanges
                .AddFilter(t => t.flTradeId, flTradeId)
                .AddFilterNot(t => t.flAuctionId, DBNull.Value)
                .SelectScalar(t => t.flAuctionId, queryExecuter).Value;
            var flWinner = tbTrades
                .AddFilter(t => t.flId, flTradeId)
                .SelectFirst(t => new FieldAlias[] { t.flWinnerData }, queryExecuter).GetVal(t => t.flWinnerData);
            flWinnerFullName = flWinner.FullOrgXinName;
        }

        public class TraderesourcesPaymentsProvider : IacPaymentsToSystemProvider
        {
            public TraderesourcesPaymentsProvider(string userXin, string userFio, IQueryExecuterProvider qep) : base(userXin, userFio, userXin, "", qep.GetDbConnectionStringSuperUser("dbYodaPaymentsGr"))
            {
            }
        }
    }
}
