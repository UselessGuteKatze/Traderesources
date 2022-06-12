using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuctionService;
using CommonSource;
using CommonSource.Auction;
using HydrocarbonSource.Helpers.Object;
using HydrocarbonSource.Helpers.Trade;
using HydrocarbonSource.Models;
using HydrocarbonSource.QueryTables.Object;
using HydrocarbonSource.QueryTables.Trade;
using HydrocarbonSource.References.Object;
using HydrocarbonSource.References.Trade;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Yoda.Application.Core;
using Yoda.Interfaces;
using YodaApp.DbQueues;
using YodaQuery;
using CommonSource.QueryTables;
using CommonSource.References.Trade;

namespace HydrocarbonTradesToAuction {

    public class HydrocarbonTradesJobInput {
        public HydrocarbonTradesJobInput()
        {
        }
    }

    public class PgQueue {
        public const string HydrocarbonTradesToAuctionJobQueue = "HydrocarbonTradesToAuctionJobQueue";
        public const string WaitingHydrocarbonTradesFromAuctionJobQueue = "WaitingHydrocarbonTradesFromAuctionJobQueue";
        public const string HeldHydrocarbonTradesFromAuctionJobQueue = "HeldHydrocarbonTradesFromAuctionJobQueue";
    }

    public static class HydrocarbonTradesJobs {
        public static PgJobs<HydrocarbonTradesJobInput> HydrocarbonTradesToAuctionJob = new PgJobs<HydrocarbonTradesJobInput>(PgQueue.HydrocarbonTradesToAuctionJobQueue, NpGlobal.DbKeys.DbJobs, "Отправляет торги углеводородов");
        public static PgJobs<HydrocarbonTradesJobInput> WaitingHydrocarbonTradesFromAuctionJob = new PgJobs<HydrocarbonTradesJobInput>(PgQueue.WaitingHydrocarbonTradesFromAuctionJobQueue, NpGlobal.DbKeys.DbJobs, "Пытается забрать результаты ожидающихся торгов");
        public static PgJobs<HydrocarbonTradesJobInput> HeldHydrocarbonTradesFromAuctionJob = new PgJobs<HydrocarbonTradesJobInput>(PgQueue.HeldHydrocarbonTradesFromAuctionJobQueue, NpGlobal.DbKeys.DbJobs, "Перепроверяет результаты состоявшихся торгов");

        public static IServiceCollection AddHydrocarbonTradesToAuctionJobsConsumer(this IServiceCollection services)
        {
            return services.AddDbQueueConsumer(jobs => jobs
                .DbQueue(HydrocarbonTradesToAuctionJob)
                .Consume(async (input, env) =>
                {
                    try
                    {
                        JobSource.UpLoadTrades(env);

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
        public static IServiceCollection AddWaitingHydrocarbonTradesFromAuctionJobsConsumer(this IServiceCollection services)
        {
            return services.AddDbQueueConsumer(jobs => jobs
                .DbQueue(WaitingHydrocarbonTradesFromAuctionJob)
                .Consume(async (input, env) =>
                {
                    try
                    {
                        JobSource.CheckWaitingTrades(env);

                        var now = DateTime.Now;
                        var newStartAfterDate = now.AddHours(4);

                        return JobFate.Repeat(newStartAfterDate, newStartAfterDate.AddDays(1));
                    }
                    catch (Exception ex)
                    {
                        return JobFate.Retry((env.JobContext.RetriesCount + 1) * 10, ex.ToString());
                    }
                }, batchSize: 1)
            );
        }
        public static IServiceCollection AddHeldHydrocarbonTradesFromAuctionJobsConsumer(this IServiceCollection services)
        {
            return services.AddDbQueueConsumer(jobs => jobs
                .DbQueue(HeldHydrocarbonTradesFromAuctionJob)
                .Consume((input, env) =>
                {
                    try
                    {
                        JobSource.CheckHeldTrades(env);

                        var now = DateTime.Now;
                        var newStartAfterDate = now.AddHours(8);

                        return Task.FromResult(JobFate.Repeat(newStartAfterDate, newStartAfterDate.AddDays(1)));
                    }
                    catch (Exception ex)
                    {
                        return Task.FromResult(JobFate.Retry((env.JobContext.RetriesCount + 1) * 10, ex.ToString()));
                    }
                }, batchSize: 1)
            );
        }
        
    }

    public class JobSource {

        public const string ReSource = "Resources"; // СУРС
        public static string AuctionObjectType = "Hydrocarbon"; // ТИП ОБЪЕКТА
        public static string AuctionTradeType = "AuctionSubsoils"; // ТИП ТОРГА

        public static void UpLoadTrades(DbJobConsumeEnvironment env)
        {

            env.Logger.LogInformation("Starting upload process...");

            var langTranslator = new LanguageTranslatorCore(env.QueryExecuter, false);

            var conf = new AuctionServiceSoapClient.EndpointConfiguration();
            var service = new AuctionServiceSoapClient(conf, env.Configuration["AuctionIntegrationServiceUrl"]);

            var tbTradeChanges = new TbTradeChanges().AddFilter(t => t.flIsTradeLoaded, false);
            tbTradeChanges.OrderBy = new[] { new OrderField(tbTradeChanges.flDateTime, OrderType.Asc) };
            var dtTradeChanges = tbTradeChanges.Select(new FieldAlias[] { tbTradeChanges.flTradeId, tbTradeChanges.flRevisionId }, env.QueryExecuter);

            if (dtTradeChanges.Rows.Count > 0)
            {
                foreach (DataRow row in dtTradeChanges.Rows)
                {
                    try
                    {
                        //Собираю модельку торга
                        var tradeModel = TradeHelper.GetTradeRevisionModel(tbTradeChanges.flTradeId.GetRowVal(row), tbTradeChanges.flRevisionId.GetRowVal(row), env.QueryExecuter);
                        //AuctionTradeType = AuctionTradeTypes[tradeModel.Method]; //Устанавливаю тип торгов !!!


                        //Если торг завершён, кидаю ошибку
                        if (tradeModel.flStatus != HydrocarbonTradeStatuses.Wait)
                        {
                            env.Logger.Log(LogLevel.Error, $"Error flag trade id={tradeModel.flId}");
                            continue;
                        }
                        env.Logger.LogInformation($"Preparing trade id={tradeModel.flId}");


                        ProcessEAuction(tradeModel, env, service, langTranslator);

                    }
                    catch (Exception e)
                    {
                        env.Logger.Log(
                        LogLevel.Error,
                        @$"Error while uploading trade {tbTradeChanges.flTradeId.GetRowVal(row)}: 
                            {e.Message}
                            {e.StackTrace}"
                        );
                    }

                }
            }
            else
            {
                env.Logger.LogInformation("Nothing to do.");
            }


            env.Logger.LogInformation("End upload process...");
        }

        public static void CheckWaitingTrades(DbJobConsumeEnvironment env)
        {
            env.Logger.LogInformation("Starting check waiting trades process...");

            var conf = new AuctionServiceSoapClient.EndpointConfiguration();
            var service = new AuctionServiceSoapClient(conf, env.Configuration["AuctionIntegrationServiceUrl"]);

            var tbTradeChanges = new TbTradeChanges().AddFilter(t => t.flIsTradeLoaded, true);
            var tbTrades = new TbTrades().AddFilter(t => t.flStatus, HydrocarbonTradeStatuses.Wait).AddFilter(t => t.flDateTime, ConditionOperator.LessOrEqual, DateTime.Now);
            var dtTradeChanges = tbTradeChanges
                .Join(tbTradeChanges.Name, tbTrades, tbTrades.Name)
                .On(new Condition(tbTradeChanges.flTradeId, tbTrades.flId))
                .Select(new FieldAlias[] { tbTradeChanges.flTradeId }, env.QueryExecuter);

            if (dtTradeChanges.Rows.Count > 0)
            {
                foreach (DataRow row in dtTradeChanges.Rows)
                {
                    try
                    {
                        ProcessCheckWaitings(tbTradeChanges.flTradeId.GetRowVal(row), env, service);
                    }
                    catch (Exception e)
                    {
                        env.Logger.Log(
                        LogLevel.Error,
                        @$"Error while checking waiting trade {tbTradeChanges.flTradeId.GetRowVal(row)}: 
                            {e.Message}
                            {e.StackTrace}"
                        );
                    }

                }
            }
            else
            {
                env.Logger.LogInformation("Nothing to do.");
            }
            env.Logger.LogInformation("End check waiting trades process...");
        }

        public static void CheckHeldTrades(DbJobConsumeEnvironment env)
        {
            env.Logger.LogInformation("Starting check held trades process...");

            var conf = new AuctionServiceSoapClient.EndpointConfiguration();
            var service = new AuctionServiceSoapClient(conf, env.Configuration["AuctionIntegrationServiceUrl"]);

            var tbTradeChanges = new TbTradeChanges().AddFilter(t => t.flIsTradeLoaded, true);
            var tbTrades = new TbTrades();//.AddFilter(t => t.flDateTime, ConditionOperator.GreateOrEqual, DateTime.Now.AddMonths(-6));
            var join = tbTradeChanges
                .Join(tbTradeChanges.Name, tbTrades, tbTrades.Name)
                .On(new Condition(tbTradeChanges.flTradeId, tbTrades.flId));
            join.OrderBy = new[] { new OrderField(tbTrades.flDateTime, OrderType.Asc) };
            var tradeIds = join.Select(new FieldAlias[] { tbTradeChanges.flTradeId }, env.QueryExecuter).AsEnumerable().Select(row => tbTradeChanges.flTradeId.GetRowVal(row)).Distinct();

            if (tradeIds.Count() > 0)
            {
                foreach (int tradeId in tradeIds)
                {
                    try
                    {
                        ProcessCheckHelds(tradeId, env, service);
                    }
                    catch (Exception e)
                    {
                        env.Logger.Log(
                        LogLevel.Error,
                        @$"Error while checking held trade {tradeId}: 
                            {e.Message}
                            {e.StackTrace}"
                        );
                    }

                }
            }
            else
            {
                env.Logger.LogInformation("Nothing to do.");
            }
            env.Logger.LogInformation("End check held trades process...");
        }

        public static void ProcessEAuction(HydrocarbonTradeModel tradeModel, DbJobConsumeEnvironment env, AuctionServiceSoapClient service, LanguageTranslatorCore langTranslator)
        {
            //Собираю модельку объекта
            var objectModel = new TbObjects().AddFilter(t => t.flId, tradeModel.flObjectId).GetObjectModelFirst(env.QueryExecuter);
            //AuctionObjectType = AuctionObjectTypes[objectModel.Type]; //Устанавливаю тип объекта !!!
            //Собираю нужную инфу объекта из модельки
            var objInfo = getObjectInfo(objectModel, tradeModel, env.QueryExecuter, null, langTranslator);


            //Метадата
            var metaData = new CommonSource.Auction.MetaData
            {
                TraderesourcesObjectInfo = new TraderesourcesObjectInfo
                {
                    LandUrl = GetObjectOnMapUrl(objectModel.flId, "HYDROCARBON")
                }
            };
            
            //Реквизиты продавца. Пихаю в метадату
            //var competentOrg = getGrObject(tradeModel.flTaxAuthorityBin, env.QueryExecuter, null);
            //metaData.RequisitsInfo = new RecipientRequisitsInfo
            //{
            //    NameRu = competentOrg.GetVal(t => t.flNameRu),
            //    NameKz = competentOrg.GetVal(t => t.flNameKz),
            //    Bik = tradeModel.flBik,
            //    Bin = tradeModel.flTaxAuthorityBin,
            //    Iik = tradeModel.flIik,
            //    Kbe = tradeModel.flKbe.ToString(),
            //    Kbk = tradeModel.flKbk,
            //    Knp = tradeModel.flKnp.ToString(),
            //    SellerCode = string.Empty
            //};

            //Наименование и БИН продавца.
            var sellerInfo = getSellerInfo(objectModel.flSellerBin, env.QueryExecuter, null);
            //Адрес продавца.
            var sellerAdr = getSellerAddr(objectModel.flSellerBin, env.QueryExecuter, null);

            //Из инфы продавца и метадаты собираю модельку аукциона.
            var auctionObjectModel = getAuctionObject(ReSource, objectModel.flId, AuctionObjectType,
                sellerInfo, sellerAdr, objInfo, metaData);

            var defaultPublishDate = env.QueryExecuter.GetDateTime(NpGlobal.DbKeys.DbTradeResources);
            var sbPublicationsNoteRu = string.Format("Извещение о продаже опубликовано: на веб-портале Реестра государственного имущества - {0};", defaultPublishDate.ToShortDateString());
            var kzVersion = langTranslator.Translate("Извещение о продаже опубликовано: на веб-портале Реестра государственного имущества - {0};", "kz", "Module: TraderesourcesAuctionIntegrationMonitor");
            var sbPublicationsNoteKz = string.Format(kzVersion, defaultPublishDate.ToShortDateString());

            //Собираю аукцион
            var auctionData = getAuction(ReSource, tradeModel, auctionObjectModel, AuctionTradeType, sbPublicationsNoteRu, sbPublicationsNoteKz, metaData, getCommission(tradeModel, env.QueryExecuter, null), getRequiredFiles(tradeModel), getRequirements(tradeModel), getParticipantRequirements(tradeModel));

            SendAuction(auctionData, tradeModel, objectModel, env, service);

        }

        public static void SendAuction(AuctionData auctionData, HydrocarbonTradeModel tradeModel, HydrocarbonObjectModel objectModel, DbJobConsumeEnvironment env, AuctionServiceSoapClient service)
        {

            ArrayOfString errorMessages;
            string resultStatus;
            int? auctionId = null;
            if (tradeModel.flStatus.Equals(HydrocarbonTradeStatuses.Wait))
            {
                env.Logger.LogInformation($"Sending trade id={tradeModel.flId}");

                var result = service.SendAuction(auctionData);
                resultStatus = result.Status;
                errorMessages = result.ErrorMessages;

                if (resultStatus == "OK")
                {
                    auctionId = result.Data.AuctionId;

                    env.Logger.LogInformation($"Successfully sended trade id={tradeModel.flId}");

                    // отправляю документы
                    //env.Logger.LogInformation($"Sending docs trade id={tradeModel.flId}");
                    //objectModel.flDocs.Each(p =>
                    //{
                    //    loadDocs(p, auctionId.Value, objectModel.flId, tradeModel.flId, service, env);
                    //});
                    //env.Logger.LogInformation($"Successfully sended docs trade id={tradeModel.flId}");


                    //// отправляю фото
                    //env.Logger.LogInformation($"Sending photos trade id={tradeModel.flId}");
                    //objectModel.flPhotos.Each(p =>
                    //{
                    //    loadPhotos(p, auctionId.Value, objectModel.flId, tradeModel.flId, service, env);
                    //});
                    //env.Logger.LogInformation($"Successfully sended photos trade id={tradeModel.flId}");

                }
                else
                {
                    env.Logger.Log(LogLevel.Error, $"TRADE {tradeModel.flId} ERRORS: {string.Join("; ", errorMessages.ToArray())}");
                }
            }
            else if (tradeModel.flStatus.Equals(HydrocarbonTradeStatuses.CancelledBefore))
            {
                env.Logger.LogInformation($"Sending cancel trade id={tradeModel.flId}");
                var result = service.CancelBeforeBegin(ReSource, tradeModel.flId);
                resultStatus = result.Status;
                errorMessages = result.ErrorMessages;
                if (resultStatus == "OK")
                {
                    auctionId = new TbTradeChanges().AddFilter(t => t.flTradeId, tradeModel.flId).AddFilterNotNull(t => t.flAuctionId).SelectScalar(t => t.flAuctionId, env.QueryExecuter);
                }
                else
                {
                    env.Logger.Log(LogLevel.Error, $"TRADE {tradeModel.flId} ERRORS: {string.Join("; ", errorMessages.ToArray())}");
                }
            }
            else
            {
                throw new NotImplementedException($"{tradeModel.flStatus} is not implemented");
            }

            if (errorMessages.Count == 0 && resultStatus == "OK" && auctionId.HasValue)
            {
                new TbTradeChanges().AddFilter(t => t.flTradeId, tradeModel.flId).AddFilter(t => t.flRevisionId, tradeModel.flRevisionId)
                    .Update()
                    .SetT(t => t.flIsTradeLoaded, true)
                    .SetT(t => t.flDateTime, env.QueryExecuter.GetDateTime(NpGlobal.DbKeys.DbTradeResources))
                    .SetT(t => t.flAuctionId, auctionId.Value)
                    .SetT(t => t.flMessage, string.Empty)
                    .Execute(env.QueryExecuter);
            }
            else
            {
                env.Logger.Log(LogLevel.Error, "Can't proccess trade.");

                if (errorMessages.Count >= 0)
                {
                    new TbTradeChanges().AddFilter(t => t.flTradeId, tradeModel.flId).AddFilter(t => t.flRevisionId, tradeModel.flRevisionId)
                    .Update()
                    .SetT(t => t.flDateTime, env.QueryExecuter.GetDateTime(NpGlobal.DbKeys.DbTradeResources))
                    .SetT(t => t.flMessage, string.Join("; ", errorMessages.ToArray()))
                    .Execute(env.QueryExecuter);
                }
            }
        }

        public static void ProcessCheckWaitings(int tradeId, DbJobConsumeEnvironment env, AuctionServiceSoapClient service)
        {
            env.Logger.LogInformation($"Checking waiting trade id={tradeId}");

            var tradeStatus = TradeHelper.GetTradeStatus(tradeId, env.QueryExecuter);
            var protocolDate = TradeHelper.GetTradeProtocolDate(tradeId, env.QueryExecuter);
            var objectId = TradeHelper.GetTradeObjectId(tradeId, env.QueryExecuter);
            var objectBlock = ObjectHelper.GetBlock(objectId, env.QueryExecuter);
            var objectLastTradeId = ObjectHelper.GetObjectLastTradeId(objectId, env.QueryExecuter);
            var result = service.GetResult(ReSource, tradeId);

            if (result.Data != null)
            {
                var auctionId = result.Data.AuctionId;
                var auctionStatus = result.Data.Status;

                env.Logger.LogInformation($"Status: {auctionStatus}");

                switch (auctionStatus)
                {
                    case "Success":
                        {
                            using (var transaction = env.QueryExecuter.BeginTransaction())
                            {
                                if (tradeStatus != HydrocarbonTradeStatuses.Held.ToString())
                                {
                                    TradeHelper.SetTradeStatus(tradeId, HydrocarbonTradeStatuses.Held, env.QueryExecuter, transaction);
                                }
                                if (result.Data.Data != null)
                                {
                                    if (result.Data.Data.WinnerProtocolSignedDate.HasValue)
                                    {
                                        if (!new[] { HydrocarbonObjectBlocks.SaledProt, HydrocarbonObjectBlocks.SaledAgr }.Contains(objectBlock))
                                        {
                                            ObjectHelper.SetBlock(objectId, HydrocarbonObjectBlocks.SaledProt, env.QueryExecuter, transaction);
                                        }
                                        //if (tradeStatus != HydrocarbonTradeStatuses.Held || !protocolDate.HasValue) {
                                            TradeHelper.SetTradeProtocol(tradeId, auctionId, env.QueryExecuter, transaction);
                                            TradeHelper.SetTradeProtocolDate(tradeId, result.Data.Data.WinnerProtocolSignedDate.Value, env.QueryExecuter, transaction);
                                            TradeHelper.SetTradeSaleCost(tradeId, result.Data.Data.Price.Value, env.QueryExecuter, transaction);
                                            TradeHelper.SetTradeWinnerData(tradeId, JsonConvert.DeserializeObject<WinnerData>(JsonConvert.SerializeObject(result.Data.Data.Winner)), result.Data.Data.WinnerGuaranteePayments.Select(payment => new PaymentItemModel() {
                                                flId = new YodaHelpers.Payments.PaymentId() { Id = payment.PaymentId, Source = "null" },
                                                flDateTime = payment.PaymentDate,
                                                flAmount = payment.PaymentAmount,
                                                flPurpose = payment.Purpose,
                                                flIsGuarantee = true
                                            }).ToList(), env.QueryExecuter, transaction);
                                        //}
                                    }
                                }
                                transaction.Commit();
                            }
                            break;
                        }
                    case "Canceled":
                        {
                            using (var transaction = env.QueryExecuter.BeginTransaction())
                            {
                                if (objectLastTradeId == tradeId)
                                {
                                    ObjectHelper.SetBlock(objectId, HydrocarbonObjectBlocks.ActiveFree, env.QueryExecuter, transaction);
                                }
                                if (!new[] { HydrocarbonTradeStatuses.CancelledBefore.ToString(), HydrocarbonTradeStatuses.CancelledBeforeObjectSaled.ToString(), HydrocarbonTradeStatuses.CancelledAfter.ToString() }.Contains(tradeStatus))
                                {
                                    TradeHelper.SetTradeStatus(tradeId, HydrocarbonTradeStatuses.CancelledAfter, env.QueryExecuter, transaction);
                                }
                                transaction.Commit();
                            }
                            break;
                        }
                    case "Failed":
                        {
                            using (var transaction = env.QueryExecuter.BeginTransaction())
                            {
                                if (objectLastTradeId == tradeId)
                                {
                                    ObjectHelper.SetBlock(objectId, HydrocarbonObjectBlocks.ActiveFree, env.QueryExecuter, transaction);
                                }
                                if (tradeStatus != HydrocarbonTradeStatuses.NotHeld.ToString())
                                {
                                    TradeHelper.SetTradeStatus(tradeId, HydrocarbonTradeStatuses.NotHeld, env.QueryExecuter, transaction);
                                }
                                transaction.Commit();
                            }
                            break;
                        }
                    default:
                        {
                            env.Logger.Log(LogLevel.Error, $"Unknown auction status - '{auctionStatus}'");
                            break;
                        }
                }
            }
        }

        public static void ProcessCheckHelds(int tradeId, DbJobConsumeEnvironment env, AuctionServiceSoapClient service)
        {
            env.Logger.LogInformation($"Checking held trade id={tradeId}");

            var tradeStatus = TradeHelper.GetTradeStatus(tradeId, env.QueryExecuter);
            var protocolDate = TradeHelper.GetTradeProtocolDate(tradeId, env.QueryExecuter);
            var objectId = TradeHelper.GetTradeObjectId(tradeId, env.QueryExecuter);
            var objectBlock = ObjectHelper.GetBlock(objectId, env.QueryExecuter);
            var objectLastTradeId = ObjectHelper.GetObjectLastTradeId(objectId, env.QueryExecuter);
            var result = service.GetResult(ReSource, tradeId);

            if (result.Data != null) {
                var auctionId = result.Data.AuctionId;
                var auctionStatus = result.Data.Status;

                env.Logger.LogInformation($"Status: {auctionStatus}");

                switch (auctionStatus) {
                    case "Success": {
                            using (var transaction = env.QueryExecuter.BeginTransaction()) {
                                if (tradeStatus != HydrocarbonTradeStatuses.Held.ToString()) {
                                    TradeHelper.SetTradeStatus(tradeId, HydrocarbonTradeStatuses.Held, env.QueryExecuter, transaction);
                                }
                                if (result.Data.Data != null) {
                                    if (result.Data.Data.WinnerProtocolSignedDate.HasValue) {
                                        if (!new[] { HydrocarbonObjectBlocks.SaledProt, HydrocarbonObjectBlocks.SaledAgr }.Contains(objectBlock)) {
                                            ObjectHelper.SetBlock(objectId, HydrocarbonObjectBlocks.SaledProt, env.QueryExecuter, transaction);
                                        }
                                        //if (tradeStatus != HydrocarbonTradeStatuses.Held || !protocolDate.HasValue) {
                                        TradeHelper.SetTradeProtocol(tradeId, auctionId, env.QueryExecuter, transaction);
                                        TradeHelper.SetTradeProtocolDate(tradeId, result.Data.Data.WinnerProtocolSignedDate.Value, env.QueryExecuter, transaction);
                                        TradeHelper.SetTradeSaleCost(tradeId, result.Data.Data.Price.Value, env.QueryExecuter, transaction);
                                        TradeHelper.SetTradeWinnerData(tradeId, JsonConvert.DeserializeObject<WinnerData>(JsonConvert.SerializeObject(result.Data.Data.Winner)), result.Data.Data.WinnerGuaranteePayments.Select(payment => new PaymentItemModel() {
                                            flId = new YodaHelpers.Payments.PaymentId() { Id = payment.PaymentId, Source = "null" },
                                            flDateTime = payment.PaymentDate,
                                            flAmount = payment.PaymentAmount,
                                            flPurpose = payment.Purpose,
                                            flIsGuarantee = true
                                        }).ToList(), env.QueryExecuter, transaction);
                                        //}
                                    }
                                }
                                transaction.Commit();
                            }
                            break;
                        }
                    case "Canceled": {
                            using (var transaction = env.QueryExecuter.BeginTransaction()) {
                                if (objectLastTradeId == tradeId) {
                                    ObjectHelper.SetBlock(objectId, HydrocarbonObjectBlocks.ActiveFree, env.QueryExecuter, transaction);
                                }
                                if (!new[] { HydrocarbonTradeStatuses.CancelledBefore.ToString(), HydrocarbonTradeStatuses.CancelledBeforeObjectSaled.ToString(), HydrocarbonTradeStatuses.CancelledAfter.ToString() }.Contains(tradeStatus)) {
                                    TradeHelper.SetTradeStatus(tradeId, HydrocarbonTradeStatuses.CancelledAfter, env.QueryExecuter, transaction);
                                }
                                transaction.Commit();
                            }
                            break;
                        }
                    case "Failed": {
                            using (var transaction = env.QueryExecuter.BeginTransaction()) {
                                if (objectLastTradeId == tradeId) {
                                    ObjectHelper.SetBlock(objectId, HydrocarbonObjectBlocks.ActiveFree, env.QueryExecuter, transaction);
                                }
                                if (tradeStatus != HydrocarbonTradeStatuses.NotHeld.ToString()) {
                                    TradeHelper.SetTradeStatus(tradeId, HydrocarbonTradeStatuses.NotHeld, env.QueryExecuter, transaction);
                                }
                                transaction.Commit();
                            }
                            break;
                        }
                    default: {
                            env.Logger.Log(LogLevel.Error, $"Unknown auction status - '{auctionStatus}'");
                            break;
                        }
                }
            }
        }

        private static AuctionData getAuction(string source, HydrocarbonTradeModel trade, AuctionObjectModel auctionObject, string flAuctionType, string auctionNoteRu, string auctionNoteKz, CommonSource.Auction.MetaData metaData, AuctionCommissionModel[] commissions, AuctionRequiredFilesGroupModel[] requieredFiles,
            AuctionRequirementsToParticipantModel[] requirements, AuctionRequirementsToParticipantByUserTypesModel[] participantRequirements)
        {
            AuctionData auctionData = new AuctionData();

            auctionData.Source = source;
            auctionData.SourceObjectId = trade.flId;
            auctionData.AuctionObject = auctionObject;
            auctionData.AuctionType = flAuctionType;
            auctionData.StartDate = trade.flDateTime;
            auctionData.AcceptAppsEndDate = trade.flAppDateTime;
            auctionData.StartCost = trade.flStartRate;
            auctionData.MinParticipantsCount = 2;
            auctionData.GuaranteePaymentCost = trade.flGarPay;
            auctionData.AuctionNoteRu = auctionNoteRu;
            auctionData.AuctionNoteKz = auctionNoteKz;
            auctionData.MetaData = JsonConvert.SerializeObject(metaData);
            auctionData.MetaDataType = nameof(CommonSource.Auction.MetaData);
            auctionData.LandOwnershipType = null;

            auctionData.Note = trade.flNote;
            auctionData.NoteKz = trade.flNote;

            auctionData.AuctionCommissions = commissions;
            auctionData.AuctionRequiredFilesGroups = requieredFiles;
            auctionData.AuctionRequirementsToParticipants = requirements;
            auctionData.AuctionRequirementsToParticipantsByUserTypes = participantRequirements;

            return auctionData;
        }
        private static AuctionObjectModel getAuctionObject(string flSource, int flSourceId, string objectType, SellerInfo sellerInfo, SellerAddress sellerAdr, ObjectInfo objectInfo, CommonSource.Auction.MetaData metaData)
        {
            var auctionObject = new AuctionObjectModel();

            auctionObject.Source = flSource;
            auctionObject.SourceObjectId = flSourceId;

            auctionObject.ObjectType = objectType;
            auctionObject.NameRu = objectInfo.NameRu;
            auctionObject.NameKz = objectInfo.NameKz;
            auctionObject.DescriptionRu = objectInfo.FullDescriptionRu;
            auctionObject.DescriptionKz = objectInfo.FullDescriptionKz;
            auctionObject.BalansHolderInfoRu = objectInfo.BalansHolderInfoRu;
            auctionObject.BalansHolderInfoKz = objectInfo.BalansHolderInfoKz;
            auctionObject.ObjectAdrCountry = objectInfo.AdrCountry;
            auctionObject.ObjectAdrObl = objectInfo.AdrObl;
            auctionObject.ObjectAdrReg = objectInfo.AdrReg;
            auctionObject.ObjectAdrAdr = objectInfo.AdrAdr;

            auctionObject.SellerInfoRu = sellerInfo.NameRu;
            auctionObject.SellerInfoKz = sellerInfo.NameKz;
            auctionObject.SellerBin = sellerInfo.Bin;

            auctionObject.SellerPhoneRu = sellerAdr.ContactsRu;
            auctionObject.SellerPhoneKz = sellerAdr.ContactsKz;
            auctionObject.SellerAdrCountry = sellerAdr.AdrCountry;
            auctionObject.SellerAdrIndex = sellerAdr.AdrIndex;
            auctionObject.SellerAdrObl = sellerAdr.AdrObl;
            auctionObject.SellerAdrReg = sellerAdr.AdrReg;
            auctionObject.SellerAdrAdr = sellerAdr.AdrAdr;
            auctionObject.MetaData = JsonConvert.SerializeObject(metaData);
            auctionObject.MetaDataType = typeof(CommonSource.Auction.MetaData).Name;

            auctionObject.Note = string.Empty;

            return auctionObject;
        }
        private static ObjectInfo getObjectInfo(HydrocarbonObjectModel objectModel, HydrocarbonTradeModel tradeModel, IQueryExecuter queryExecuter, ITransaction transaction, ILangTranslator langTranslator)
        {
            var gid = getGlobalObjectId(objectModel.flSellerBin, queryExecuter, transaction);
            string balansHolderInfoRu, balansHolderInfoKz;
            getBalansHolderInfo(out balansHolderInfoRu, out balansHolderInfoKz, gid, queryExecuter, langTranslator);

            var ret = new ObjectInfo {
                AdrCountry = "118",
                AdrObl = "118",
                AdrReg = "118",
                AdrAdr = objectModel.flRegion,
                NameRu = $"Участок: {objectModel.flName}",
                NameKz = $"Жер телімі: {objectModel.flName}",
                BalansHolderInfoRu = balansHolderInfoRu,
                BalansHolderInfoKz = balansHolderInfoKz
            };

            var sbFullInfoRu = new StringBuilder();
            var sbFullInfoKz = new StringBuilder();

            sbFullInfoRu.AppendFormat("Участок: {0}; ", objectModel.flName);
            sbFullInfoKz.AppendFormat("Жер телімі: {0}; ", objectModel.flName);

            sbFullInfoRu.AppendFormat("Тип: {0}; ", "Недропользование");
            sbFullInfoKz.AppendFormat("Түрі: {0}; ", "Жер қойнауын пайдалану");

            sbFullInfoRu.AppendFormat("Тип 2: {0}; ", "Углеводороды");
            sbFullInfoKz.AppendFormat("Түрі 2: {0}; ", "Көмірсутектер");

            sbFullInfoRu.AppendFormat("Область: {0}; ", objectModel.flRegion);
            sbFullInfoKz.AppendFormat("Облыс: {0}; ", objectModel.flRegion);

            sbFullInfoRu.AppendFormat("Номер участка: {0}; ", objectModel.flNumber);
            sbFullInfoKz.AppendFormat("Жер телімі нөмірі: {0}; ", objectModel.flNumber);

            sbFullInfoRu.AppendFormat("Площадь, кв. км.: {0}; ", objectModel.flArea.HasValue ? objectModel.flArea.Value.ToString("#,#0.00") : "-");
            sbFullInfoKz.AppendFormat("Ауданы, ш. км.: {0}; ", objectModel.flArea.HasValue ? objectModel.flArea.Value.ToString("#,#0.00") : "-");

            sbFullInfoRu.AppendFormat("Количество блоков: {0}; ", objectModel.flBlocksCount);
            sbFullInfoKz.AppendFormat("блоктар саны: {0}; ", objectModel.flBlocksCount);

            sbFullInfoRu.AppendFormat("Период разведки: {0}; ", tradeModel.flExploringPeriod);
            sbFullInfoKz.AppendFormat("Барлау периоды: {0}; ", tradeModel.flExploringPeriod);

            ret.FullDescriptionRu = sbFullInfoRu.ToString();
            ret.FullDescriptionKz = sbFullInfoKz.ToString();
            return ret;
        }
        private static int getGlobalObjectId(string bin, IQueryExecuter queryExecuter, ITransaction t)
        {
            var r = new TbGrObjectsActiveRevisions().AddFilter(t => t.flBin, bin).SelectFirstOrDefault(t => t.Fields.ToFieldsAliases(), queryExecuter, t);
            if (!r.IsFirstRowExists)
            {
                return -1;
            }
            return r.GetVal(t => t.flGlobalObjectId);
        }
        private static SelectFirstResultProxy<TbGrObjectsActiveRevisions> getGrObject(string bin, IQueryExecuter queryExecuter, ITransaction transaction)
        {
            return new TbGrObjectsActiveRevisions().AddFilter(t => t.flBin, bin).SelectFirstOrDefault(t => t.Fields.ToFieldsAliases(), queryExecuter, transaction);
        }
        private static void getBalansHolderInfo(out string infoRu, out string infoKz, int gid, IQueryExecuter qe, ILangTranslator langTranslator)
        {
            var sbRu = new StringBuilder();
            var sbKz = new StringBuilder();

            var tbGrObjects = new TbGrObjectsActiveRevisions();
            tbGrObjects.AddFilter(tbGrObjects.flGlobalObjectId, gid);
            var grData = tbGrObjects.SelectFirst(new FieldAlias[] { tbGrObjects.flNameRu, tbGrObjects.flNameKz, tbGrObjects.flBin }, qe);

            var tbContacts = new TbContactsActiveRevisions();
            tbContacts.AddFilter(tbContacts.flGlobalObjectId, gid);
            var adrData = tbContacts.SelectFirst(tbContacts.Fields.ToFieldsAliases(), qe);

            string adrInfoRu, adrInfoKz;
            getAdrInfo(out adrInfoRu, out adrInfoKz, langTranslator, (string)adrData[tbContacts.flAdrCountry.FieldName], (string)adrData[tbContacts.flAdrObl.FieldName], (string)adrData[tbContacts.flAdrReg.FieldName], (string)adrData[tbContacts.flAdrAdr.FieldName], qe);

            adrInfoRu = adrInfoRu.Trim(' ', ';') + "; ";
            adrInfoKz = adrInfoKz.Trim(' ', ';') + "; ";

            var sbAdrInfoRu = new StringBuilder(adrInfoRu);
            var sbAdrInfoKz = new StringBuilder(adrInfoKz);

            Action<string, object> addAdrInfoNotEmpty = (title, value) =>
            {
                if (Convert.IsDBNull(value) || value == null || string.IsNullOrWhiteSpace((string)value))
                {
                    return;
                }
                sbAdrInfoRu.Append(title + ": " + value + "; ");
                sbAdrInfoKz.Append(langTranslator.Translate(title, "kz", "Reference:TraderesourcesAuctionIntegrationMonitor") + ": " + value + "; ");
            };
            addAdrInfoNotEmpty(tbContacts.flAdrFax.Text, adrData[tbContacts.flAdrFax.FieldName]);
            addAdrInfoNotEmpty(tbContacts.flAdrPhone.Text, adrData[tbContacts.flAdrPhone.FieldName]);
            addAdrInfoNotEmpty(tbContacts.flAdrMobile.Text, adrData[tbContacts.flAdrMobile.FieldName]);
            addAdrInfoNotEmpty(tbContacts.flAdrMail.Text, adrData[tbContacts.flAdrMail.FieldName]);



            sbRu.AppendFormat("{0}; {1}: {2}; {3}: {4}", (grData[tbGrObjects.flNameRu.FieldName] + string.Empty).Trim(), "БИН", (grData[tbGrObjects.flBin.FieldName] + string.Empty).Trim(), "Адрес", adrInfoRu);
            sbKz.AppendFormat("{0}; {1}: {2}; {3}: {4}", (grData[tbGrObjects.flNameKz.FieldName] + string.Empty).Trim(), langTranslator.Translate("БИН", "kz", "Reference:TraderesourcesAuctionIntegrationMonitor"), (grData[tbGrObjects.flBin.FieldName] + string.Empty).Trim(), langTranslator.Translate("Адрес", "kz", "Reference:TraderesourcesAuctionIntegrationMonitor"), adrInfoKz);

            var strPhones = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(adrData[tbContacts.flAdrPhone.FieldName] + string.Empty))
            {
                strPhones.Append(adrData[tbContacts.flAdrPhone.FieldName] + string.Empty + "; ");
            }
            if (!string.IsNullOrWhiteSpace(adrData[tbContacts.flAdrMobile.FieldName] + string.Empty))
            {
                strPhones.Append(adrData[tbContacts.flAdrMobile.FieldName] + string.Empty + "; ");
            }

            if (strPhones.Length > 0)
            {
                sbRu.AppendFormat("Телефоны: {0}", strPhones);
                sbKz.AppendFormat(langTranslator.Translate("Телефоны: {0}", "kz", "Reference:TraderesourcesAuctionIntegrationMonitor").ToString(), strPhones);
            }

            infoRu = sbRu.ToString();
            infoKz = sbKz.ToString();
        }
        private static void getAdrInfo(out string adrRuOut, out string adrKzOut, ILangTranslator langTranslator, string adrCountry, string adrObl, string adrReg, string adrAdr, IQueryExecuter queryExecuter)
        {
            var retRu = new StringBuilder();
            var retKz = new StringBuilder();
            Action<string> addKatoInfo = (katoCode) =>
            {
                var tbRefKato = new QueryTable("tbRfcKato", QueryTableType.View)
                {
                    Fields = new Field[] {
                        new TextField("flId", "Id"),
                        new TextField("flRu", "flRu"),
                    },
                    DbKey = NpGlobal.DbKeys.DbYodaReferencesGr
                };
                var katoRu = tbRefKato.AddFilter("flId", katoCode).SelectScalar("flRu", queryExecuter);
                retRu.Append(katoRu + "; ");
                retKz.Append(langTranslator.Translate((string)katoRu, "kz", "Reference:TraderesourcesAuctionIntegrationMonitor") + "; ");
            };
            if (!adrCountry.EqualsIgnoreCase("118"))
            {
                addKatoInfo(adrCountry);
            }
            addKatoInfo(adrObl);
            addKatoInfo(adrReg);

            if (!string.IsNullOrWhiteSpace(adrAdr))
            {
                retRu.Append(adrAdr);
                retKz.Append(adrAdr);
            }
            adrRuOut = retRu.ToString();
            adrKzOut = retKz.ToString();
        }
        private static SellerInfo getSellerInfo(string bin, IQueryExecuter queryExecuter, ITransaction transaction)
        {
            var flGlobalObjectId = getGlobalObjectId(bin, queryExecuter, transaction);
            var grObject = new TbGrObjectsActiveRevisions().AddFilter(t => t.flGlobalObjectId, flGlobalObjectId).SelectFirstOrDefault(t => t.Fields.ToFieldsAliases(), queryExecuter, transaction);
            return new SellerInfo
            {
                Bin = grObject.GetVal(t => t.flBin),
                NameRu = grObject.GetVal(t => t.flNameRu),
                NameKz = grObject.GetVal(t => t.flNameKz)
            };
        }
        private static SellerAddress getSellerAddr(string bin, IQueryExecuter queryExecuter, ITransaction transaction)
        {
            var flGlobalObjectId = getGlobalObjectId(bin, queryExecuter, transaction);
            var row = new TbContactsActiveRevisions().AddFilter(t => t.flGlobalObjectId, flGlobalObjectId).SelectFirstOrDefault(t => t.Fields.ToFieldsAliases(), queryExecuter, transaction);
            return new SellerAddress
            {
                AdrCountry = row.GetVal(r => r.flAdrCountry),
                AdrObl = row.GetVal(r => r.flAdrObl),
                AdrReg = row.GetVal(r => r.flAdrReg),
                AdrAdr = row.GetVal(r => r.flAdrAdr),
                AdrIndex = row.GetVal(r => r.flAdrIndex),
                ContactsRu = row.GetVal(r => r.flAdrPhone),
                ContactsKz = row.GetVal(r => r.flAdrPhone)
            };
        }
        private static SelectFirstResultProxy<TbComissionMembers> getCommissioner(string id, string bin, IQueryExecuter queryExecuter, ITransaction transaction)
        {
            return new TbComissionMembers()
                .AddFilter(t => t.flId, Convert.ToInt32(id))
                .AddFilter(t => t.flCompetentOrgBin, bin)
                .AddFilter(t => t.flStatus, ComissionStatuses.Active)
                .SelectFirstOrDefault(t => t.Fields.ToFieldsAliases(), queryExecuter, transaction);
        }
        private static AuctionCommissionModel[] getCommission(HydrocarbonTradeModel trade, IQueryExecuter queryExecuter, ITransaction transaction)
        {

            return trade.flCommissionMembers.Select(c =>
            {
                var comm = new AuctionCommissionModel
                {
                    Role = getComissionRole(c["flRole"].ToString())
                };
                var commission = getCommissioner(c["flUserId"].ToString(), trade.flCompetentOrgBin, queryExecuter, transaction);
                comm.IsChairman = getComissionRole(c["flRole"].ToString()).Equals(RefAuctionCommissionRoles.Chairman);
                comm.Position = commission.GetVal(t => t.flInfo);
                comm.CommissionerXin = commission.GetVal(t => t.flIin);
                comm.CommissionerFio = commission.GetVal(t => t.flFio);
                return comm;
            }).ToArray();
        }
        private static string getComissionRole(string role)
        {
            var roles = new Dictionary<string, string> {
                {CommissionMembersRoles.CHR.ToString(), RefAuctionCommissionRoles.Chairman},
                {CommissionMembersRoles.VCH.ToString(), RefAuctionCommissionRoles.ViceChairman},
                {CommissionMembersRoles.MEM.ToString(), RefAuctionCommissionRoles.Member},
                {CommissionMembersRoles.SCR.ToString(), RefAuctionCommissionRoles.Secretary}
            };
            return roles[role];
        }
        private static AuctionRequirementsToParticipantModel[] getRequirements(HydrocarbonTradeModel trade) {
            if (trade.flParticipantConditions == null) {
                return new AuctionRequirementsToParticipantModel[] { };
            }
            return trade.flParticipantConditions.Select(c => new AuctionRequirementsToParticipantModel { RequirementId = $"{c.Id}_{trade.flId}_{trade.flObjectId}", Text = c.Text }).ToArray();
        }
        private static AuctionRequirementsToParticipantByUserTypesModel[] getParticipantRequirements(HydrocarbonTradeModel trade) {
            var requirements = new List<AuctionRequirementsToParticipantByUserTypesModel>();
            requirements.AddRange(trade.flParticipantConditionsFiz.Select(c => new AuctionRequirementsToParticipantByUserTypesModel { RequirementId = $"{c.Id}_{trade.flId}_{trade.flObjectId}", Text = c.Text, ForUserType = "Individual" }).ToArray());
            requirements.AddRange(trade.flParticipantConditionsFizEnt.Select(c => new AuctionRequirementsToParticipantByUserTypesModel { RequirementId = $"{c.Id}_{trade.flId}_{trade.flObjectId}", Text = c.Text, ForUserType = "IndividualCorporate" }).ToArray());
            requirements.AddRange(trade.flParticipantConditionsJur.Select(c => new AuctionRequirementsToParticipantByUserTypesModel { RequirementId = $"{c.Id}_{trade.flId}_{trade.flObjectId}", Text = c.Text, ForUserType = "Corporate" }).ToArray());
            return requirements.ToArray();
        }
        private static AuctionRequiredFilesGroupModel[] getRequiredFiles(HydrocarbonTradeModel trade) {
            var requiredFiles = new List<AuctionRequiredFilesGroupModel>();
            requiredFiles.AddRange(trade.flRequiredDocumentsJur.Select(d => new AuctionRequiredFilesGroupModel { GroupName = $"{d.Id}_{trade.flId}_{trade.flObjectId}", GroupText = d.Text, ForUserType = "Corporate" }).ToArray());
            requiredFiles.AddRange(trade.flRequiredDocumentsFiz.Select(d => new AuctionRequiredFilesGroupModel { GroupName = $"{d.Id}_{trade.flId}_{trade.flObjectId}", GroupText = d.Text, ForUserType = "Individual" }).ToArray());
            requiredFiles.AddRange(trade.flRequiredDocumentsFizEnt.Select(d => new AuctionRequiredFilesGroupModel { GroupName = $"{d.Id}_{trade.flId}_{trade.flObjectId}", GroupText = d.Text, ForUserType = "IndividualCorporate" }).ToArray());
            return requiredFiles.ToArray();
        }
        private static string GetObjectOnMapUrl(int ObjectId, string ObjectType)
        {
            return $"https://lands.qoldau.kz/ru/lands-map/subsoils?zoomToObject={ObjectId}&objectType={ObjectType}";
        }
    }
}