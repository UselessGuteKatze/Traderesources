﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AuctionService;
using FileStoreInterfaces;
using CommonSource;
using CommonSource.Auction;
using ForestSource.Helpers.Object;
using ForestSource.Helpers.Trade;
using ForestSource.Models;
using ForestSource.QueryTables.Common;
using ForestSource.QueryTables.Object;
using ForestSource.QueryTables.Trade;
using ForestSource.References.Object;
using ForestSource.References.Trade;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Yoda.Application.Core;
using Yoda.Interfaces;
using YodaApp.DbQueues;
using YodaQuery;
using CommonSource.QueryTables;
using CommonSource.References.Trade;
using CommonSource.References.Object;
using YodaApp.Yoda.Application.Helpers;

namespace ForestTradesToAuction {

    public class ForestTradesJobInput {
        public ForestTradesJobInput()
        {
        }
    }

    public class PgQueue {
        public const string ForestTradesToAuctionJobQueue = "ForestTradesToAuctionJobQueue";
        public const string WaitingForestTradesFromAuctionJobQueue = "WaitingForestTradesFromAuctionJobQueue";
        public const string HeldForestTradesFromAuctionJobQueue = "HeldForestTradesFromAuctionJobQueue";
    }

    public static class ForestTradesJobs {
        public static PgJobs<ForestTradesJobInput> ForestTradesToAuctionJob = new PgJobs<ForestTradesJobInput>(PgQueue.ForestTradesToAuctionJobQueue, NpGlobal.DbKeys.DbJobs, "Отправляет торги лесов");
        public static PgJobs<ForestTradesJobInput> WaitingForestTradesFromAuctionJob = new PgJobs<ForestTradesJobInput>(PgQueue.WaitingForestTradesFromAuctionJobQueue, NpGlobal.DbKeys.DbJobs, "Пытается забрать результаты ожидающихся торгов");
        public static PgJobs<ForestTradesJobInput> HeldForestTradesFromAuctionJob = new PgJobs<ForestTradesJobInput>(PgQueue.HeldForestTradesFromAuctionJobQueue, NpGlobal.DbKeys.DbJobs, "Перепроверяет результаты состоявшихся торгов");

        public static IServiceCollection AddForestTradesToAuctionJobsConsumer(this IServiceCollection services)
        {
            return services.AddDbQueueConsumer(jobs => jobs
                .DbQueue(ForestTradesToAuctionJob)
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
        public static IServiceCollection AddWaitingForestTradesFromAuctionJobsConsumer(this IServiceCollection services)
        {
            return services.AddDbQueueConsumer(jobs => jobs
                .DbQueue(WaitingForestTradesFromAuctionJob)
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
        public static IServiceCollection AddHeldForestTradesFromAuctionJobsConsumer(this IServiceCollection services)
        {
            return services.AddDbQueueConsumer(jobs => jobs
                .DbQueue(HeldForestTradesFromAuctionJob)
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

        public const string ReSource = "ForestResourcesPieces"; // СУРС
        public static string AuctionObjectType = "ForestPiece"; // ТИП ОБЪЕКТА
        public static string AuctionTradeType = "ForestTender"; // ТИП ТОРГА

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
                        if (tradeModel.flStatus != TradesStatuses.Wait)
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
            var tbTrades = new TbTrades().AddFilter(t => t.flStatus, TradesStatuses.Wait).AddFilter(t => t.flDateTime, ConditionOperator.LessOrEqual, DateTime.Now);
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

        public static void ProcessEAuction(ForestTradeModel tradeModel, DbJobConsumeEnvironment env, AuctionServiceSoapClient service, LanguageTranslatorCore langTranslator)
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
                    LandUrl = GetObjectOnMapUrl(Convert.ToInt32(objectModel.flForestryPieces[0].SearchItemId), "FORESTOBJECT")
                }
            };
            
            //Реквизиты продавца. Пихаю в метадату
            var competentOrg = getGrObject(tradeModel.flTaxAuthorityBin, env.QueryExecuter, null);
            metaData.RequisitsInfo = new RecipientRequisitsInfo
            {
                NameRu = competentOrg.GetVal(t => t.flNameRu),
                NameKz = competentOrg.GetVal(t => t.flNameKz),
                Bik = tradeModel.flBik,
                Bin = tradeModel.flTaxAuthorityBin,
                Iik = tradeModel.flIik,
                Kbe = tradeModel.flKbe.ToString(),
                Kbk = tradeModel.flKbk,
                Knp = tradeModel.flKnp.ToString(),
                SellerCode = string.Empty
            };
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
            var auctionData = getAuction(ReSource, tradeModel, auctionObjectModel, AuctionTradeType, sbPublicationsNoteRu, sbPublicationsNoteKz, metaData, getCommission(tradeModel, env.QueryExecuter, null), getRequiredFiles(tradeModel), getRequirements(tradeModel));

            SendAuction(auctionData, tradeModel, objectModel, env, service);

        }

        public static void SendAuction(AuctionData auctionData, ForestTradeModel tradeModel, ForestObjectModel objectModel, DbJobConsumeEnvironment env, AuctionServiceSoapClient service)
        {

            ArrayOfString errorMessages;
            string resultStatus;
            int? auctionId = null;
            if (tradeModel.flStatus.Equals(TradesStatuses.Wait))
            {
                env.Logger.LogInformation($"Sending trade id={tradeModel.flId}");

                var result = service.SendAuction(auctionData);
                resultStatus = result.Status;
                errorMessages = result.ErrorMessages;

                if (resultStatus == "OK")
                {
                    auctionId = result.Data.AuctionId;

                    env.Logger.LogInformation($"Successfully sended trade id={tradeModel.flId}");

                    //отправляю документы
                    env.Logger.LogInformation($"Sending docs trade id={tradeModel.flId}");
                    objectModel.flDocs.Each(p => {
                        loadDocs(p, auctionId.Value, objectModel.flId, tradeModel.flId, service, env);
                    });
                    env.Logger.LogInformation($"Successfully sended docs trade id={tradeModel.flId}");


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
            else if (tradeModel.flStatus.Equals(TradesStatuses.CanceledEarlier))
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

        private static string GenerateSHA256String(byte[] bytes) {
            SHA256 sha256 = SHA256Managed.Create();
            byte[] hash = sha256.ComputeHash(bytes);
            return GetStringFromHash(hash);
        }
        private static string GetStringFromHash(byte[] hash) {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < hash.Length; i++) {
                result.Append(hash[i].ToString("X2"));
            }
            return result.ToString();
        }
        private static void loadDocs(FileInfo fileInfo, int auctionId, int objectId, int tradeId, AuctionServiceSoapClient service, DbJobConsumeEnvironment env) {
            var SHA256Code = "SHA-256";

            var fileSrcId = new TbFilesInfo().AddFilter(t => t.flFileId, fileInfo.FileId).SelectScalar(t => t.flFileSrcId, env.QueryExecuter);
            var fileContent = new TbFiles().AddFilter(t => t.flFileSrcId, fileSrcId).SelectScalar(t => t.flContent, env.QueryExecuter);
            var fileContentHash = GenerateSHA256String(fileContent);

            var fileContentParts = getFileParts(fileContent).Select((filePartContent, filePartNumber) => {
                var filePartContentHash = GenerateSHA256String(filePartContent);

                return new FilePart() {
                    Number = filePartNumber + 1,
                    Content = filePartContent,
                    ContentHash = filePartContentHash,
                    Length = filePartContent.Length
                };

            }).ToList();

            int GuidId;

            var tbGuidIds = new TbGuidIds();
            tbGuidIds.AddFilter(tbGuidIds.flGuid, fileSrcId);
            if (tbGuidIds.Count(env.QueryExecuter) == 0) {
                using (ITransaction transaction = env.QueryExecuter.BeginTransaction(NpGlobal.DbKeys.DbFiles)) {
                    GuidId = tbGuidIds.flId.GetNextId(env.QueryExecuter, transaction);
                    tbGuidIds.Insert()
                        .Set(t => t.flId, GuidId)
                        .Set(t => t.flGuid, fileSrcId)
                        .Set(t => t.flContent, fileContent)
                        .Set(t => t.flContentHash, fileContentHash)
                        .Set(t => t.flContentHashType, SHA256Code)
                        .Set(t => t.flParts, JsonConvert.SerializeObject(fileContentParts))
                        .Execute(env.QueryExecuter, transaction);
                    transaction.Commit();
                }
            }
            else {
                GuidId = tbGuidIds.SelectScalar(t => t.flId, env.QueryExecuter).Value;
            }

            void uploadFile() {
                var auctionObjectFile = new AuctionObjectFile() {
                    AuctionDigitalFilesSource = new AuctionDigitalFilesSourceModel() {
                        FileId = GuidId,
                        Source = ReSource,
                        SourceId = GuidId
                    },
                    AuctionFileUpload = new AuctionFileUploadModel() {
                        FileHash = fileContentHash,
                        FileName = fileInfo.FileName,
                        FileSize = fileContent.Length,
                        HashAlgorithm = SHA256Code,
                        PartsCount = fileContentParts.Count
                    },
                    AuctionObjectFilesData = new AuctionObjectFilesDataModel() {
                        AuctionId = auctionId,
                        Description = fileInfo.Description,
                        FileId = GuidId,
                        FileName = fileInfo.FileName,
                        FileSize = fileContent.Length
                    }
                };

                var result = service.AddAuctionObjectFileToUpload(ReSource, tradeId, auctionObjectFile);
                if (result.Status != "OK") {
                    env.Logger.Log(LogLevel.Error, $"ERRORS: {string.Join("; ", result.ErrorMessages.ToArray())}");
                    uploadFile();
                }
                else {
                    var auctionUploadId = result.Data.AuctionFileUpload.UploadId;

                    fileContentParts.ForEach(p => {
                        void uploadFilePart() {
                            var partResult = service.UploadFilePart(ReSource, auctionUploadId,
                                new AuctionFilePartModel {
                                    FilePart = p.Content,
                                    FilePartHash = p.ContentHash,
                                    FilePartSize = p.Length,
                                    PartNumber = p.Number,
                                    PartId = p.Number
                                });
                            if (partResult.Status != "OK") {
                                env.Logger.Log(LogLevel.Error, $"ERRORS: {string.Join("; ", partResult.ErrorMessages.ToArray())}");
                                uploadFilePart();
                            }
                        }
                        uploadFilePart();
                    });
                }
            }
            uploadFile();
        }
        private static List<byte[]> getFileParts(byte[] file) {
            List<byte[]> result = new List<byte[]>();

            int lengthToSplit = 1048576; // Длина одной части (в байтах)
            int arrayLength = file.Length;

            for (int i = 0; i < arrayLength; i = i + lengthToSplit) {
                if (arrayLength < i + lengthToSplit) {
                    lengthToSplit = arrayLength - i;
                }
                byte[] val = new byte[lengthToSplit];

                Array.Copy(file, i, val, 0, lengthToSplit);
                result.Add(val);
            }

            return result;
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
                                if (tradeStatus != TradesStatuses.Held.ToString())
                                {
                                    TradeHelper.SetTradeStatus(tradeId, TradesStatuses.Held, env.QueryExecuter, transaction);
                                }
                                if (result.Data.Data != null)
                                {
                                    if (result.Data.Data.WinnerProtocolSignedDate.HasValue)
                                    {
                                        if (!new[] { ForestObjectBlocks.SaledProt, ForestObjectBlocks.SaledAgr }.Contains(objectBlock))
                                        {
                                            ObjectHelper.SetBlock(objectId, ForestObjectBlocks.SaledProt, env.QueryExecuter, transaction);
                                        }
                                        //if (tradeStatus != TradesStatuses.Held || !protocolDate.HasValue) {
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
                                    ObjectHelper.SetBlock(objectId, ForestObjectBlocks.ActiveFree, env.QueryExecuter, transaction);
                                }
                                if (!new[] { TradesStatuses.CanceledEarlier.ToString(), TradesStatuses.Cancel.ToString() }.Contains(tradeStatus))
                                {
                                    TradeHelper.SetTradeStatus(tradeId, TradesStatuses.Cancel, env.QueryExecuter, transaction);
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
                                    ObjectHelper.SetBlock(objectId, ForestObjectBlocks.ActiveFree, env.QueryExecuter, transaction);
                                }
                                if (tradeStatus != TradesStatuses.NotHeld.ToString())
                                {
                                    TradeHelper.SetTradeStatus(tradeId, TradesStatuses.NotHeld, env.QueryExecuter, transaction);
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
                                if (tradeStatus != TradesStatuses.Held.ToString()) {
                                    TradeHelper.SetTradeStatus(tradeId, TradesStatuses.Held, env.QueryExecuter, transaction);
                                }
                                if (result.Data.Data != null) {
                                    if (result.Data.Data.WinnerProtocolSignedDate.HasValue) {
                                        if (!new[] { ForestObjectBlocks.SaledProt, ForestObjectBlocks.SaledAgr }.Contains(objectBlock)) {
                                            ObjectHelper.SetBlock(objectId, ForestObjectBlocks.SaledProt, env.QueryExecuter, transaction);
                                        }
                                        //if (tradeStatus != TradesStatuses.Held || !protocolDate.HasValue) {
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
                                    ObjectHelper.SetBlock(objectId, ForestObjectBlocks.ActiveFree, env.QueryExecuter, transaction);
                                }
                                if (!new[] { TradesStatuses.CanceledEarlier.ToString(), TradesStatuses.Cancel.ToString() }.Contains(tradeStatus)) {
                                    TradeHelper.SetTradeStatus(tradeId, TradesStatuses.Cancel, env.QueryExecuter, transaction);
                                }
                                transaction.Commit();
                            }
                            break;
                        }
                    case "Failed": {
                            using (var transaction = env.QueryExecuter.BeginTransaction()) {
                                if (objectLastTradeId == tradeId) {
                                    ObjectHelper.SetBlock(objectId, ForestObjectBlocks.ActiveFree, env.QueryExecuter, transaction);
                                }
                                if (tradeStatus != TradesStatuses.NotHeld.ToString()) {
                                    TradeHelper.SetTradeStatus(tradeId, TradesStatuses.NotHeld, env.QueryExecuter, transaction);
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

        private static AuctionData getAuction(string source, ForestTradeModel trade, AuctionObjectModel auctionObject, string flAuctionType, string auctionNoteRu, string auctionNoteKz, CommonSource.Auction.MetaData metaData, AuctionCommissionModel[] commissions, AuctionRequiredFilesGroupModel[] requieredFiles,
            AuctionRequirementsToParticipantModel[] requirements)
        {
            AuctionData auctionData = new AuctionData();

            auctionData.Source = source;
            auctionData.SourceObjectId = trade.flId;
            auctionData.AuctionObject = auctionObject;
            auctionData.AuctionType = flAuctionType;
            auctionData.StartDate = trade.flDateTime;
            auctionData.StartCost = trade.flCostStart;
            auctionData.MinParticipantsCount = 2;
            auctionData.GuaranteePaymentCost = trade.flDeposit;
            auctionData.AuctionNoteRu = auctionNoteRu;
            auctionData.AuctionNoteKz = auctionNoteKz;
            auctionData.MetaData = JsonConvert.SerializeObject(metaData);
            auctionData.MetaDataType = nameof(CommonSource.Auction.MetaData);
            auctionData.LandOwnershipType = null;

            auctionData.Note = trade.flNote;
            auctionData.NoteKz = trade.flNote;

            if (commissions != null) {
                auctionData.AuctionCommissions = commissions;
            }
            if (requieredFiles != null) {
                auctionData.AuctionRequiredFilesGroups = requieredFiles;
            }
            if (requirements != null) {
                auctionData.AuctionRequirementsToParticipants = requirements;
            }

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
        private static ObjectInfo getObjectInfo(ForestObjectModel objectModel, ForestTradeModel tradeModel, IQueryExecuter queryExecuter, ITransaction transaction, ILangTranslator langTranslator)
        {
            var gid = getGlobalObjectId(objectModel.flSellerBin, queryExecuter, transaction);
            getBalansHolderInfo(out var balansHolderInfoRu, out var balansHolderInfoKz, gid, queryExecuter, langTranslator);

            Func<string, string> getMappedKato = (ar) => {
                if (ar == "1") {
                    return "118";
                }
                return AddrCodeMapHelper.GetGrKatoCodeByArRefCode(ar);
            };

            var forestryId = new TbForestryPieces()
                .AddFilter(t => t.flId, Convert.ToInt32(objectModel.flForestryPieces[0].SearchItemId))
                .GetForestryQuarterJoin(out var tbForestries, out var tbQuarters)
                .SelectScalar(t => t.L.L.flId, queryExecuter);

            var forestry = new TbForestries()
                .AddFilter(t => t.flId, forestryId)
                .GetForestryModelFirst(queryExecuter);


            var ret = new ObjectInfo {
                AdrCountry = "118",
                AdrObl = getMappedKato(forestry.flRegion),
                AdrReg = getMappedKato(forestry.flDistrict),
                AdrAdr = forestry.flLocation,
                NameRu = objectModel.flName,
                NameKz = objectModel.flName,
                BalansHolderInfoRu = balansHolderInfoRu,
                BalansHolderInfoKz = balansHolderInfoKz
            };

            var sbFullInfoRu = new StringBuilder();
            var sbFullInfoKz = new StringBuilder();

            var refRegion = new RefKato();
            var tbObjects = new TbObjects();

            sbFullInfoRu.AppendFormat("Область: {0}; ", refRegion.Search(forestry.flRegion));
            sbFullInfoKz.AppendFormat("Облыс: {0}; ", langTranslator.Translate(refRegion.Search(forestry.flRegion).Text.Text, "kz", "Reference:TraderesourcesAuctionIntegrationMonitor"));

            sbFullInfoRu.AppendFormat($"{tbObjects.flDescription.Text}: {objectModel.flDescription}; ");
            sbFullInfoKz.AppendFormat($"{langTranslator.Translate(tbObjects.flDescription.Text, "kz", "Reference:TraderesourcesAuctionIntegrationMonitor")}: {langTranslator.Translate(objectModel.flDescription, "kz", "Reference:TraderesourcesAuctionIntegrationMonitor")}; ");

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
        private static SelectFirstResultProxy<TbComissionMembers> getCommissioner(string id, string bin, IQueryExecuter queryExecuter, ITransaction transaction) {
            return new TbComissionMembers()
                .AddFilter(t => t.flId, Convert.ToInt32(id))
                .AddFilter(t => t.flCompetentOrgBin, bin)
                .AddFilter(t => t.flStatus, ComissionStatuses.Active)
                .SelectFirstOrDefault(t => t.Fields.ToFieldsAliases(), queryExecuter, transaction);
        }
        private static AuctionCommissionModel[] getCommission(ForestTradeModel trade, IQueryExecuter queryExecuter, ITransaction transaction) {

            return trade.flCommissionMembers.Select(c => {
                var comm = new AuctionCommissionModel {
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
        private static string getComissionRole(string role) {
            var roles = new Dictionary<string, string> {
                {CommissionMembersRoles.CHR.ToString(), RefAuctionCommissionRoles.Chairman},
                {CommissionMembersRoles.VCH.ToString(), RefAuctionCommissionRoles.ViceChairman},
                {CommissionMembersRoles.MEM.ToString(), RefAuctionCommissionRoles.Member},
                {CommissionMembersRoles.SCR.ToString(), RefAuctionCommissionRoles.Secretary}
            };
            return roles[role];
        }
        private static AuctionRequiredFilesGroupModel[] getRequiredFiles(ForestTradeModel trade) {
            var requiredFiles = new List<AuctionRequiredFilesGroupModel>();
            var userTypes = new List<string>() { "Corporate", "Individual", "IndividualCorporate" };
            userTypes.ForEach(userType => {
                var index = 0;
                trade.flRequiredDocuments.ToList().ForEach(d => {
                    requiredFiles.Add(new AuctionRequiredFilesGroupModel { GroupName = $"Forest_{userType}_{trade.flId}_{index}", GroupText = d["flDocument"].ToString(), ForUserType = userType });
                    index++;
                });
            });
            return requiredFiles.ToArray();
        }
        private static AuctionRequirementsToParticipantModel[] getRequirements(ForestTradeModel trade) {
            var requirements = new List<AuctionRequirementsToParticipantModel>();
            if (trade.flParticipantConditions == null) {
                return requirements.ToArray();
            }
            //var userTypes = new List<string>() { "Corporate", "Individual", "IndividualCorporate" };
            //userTypes.ForEach(userType => {
            var index = 0;
            trade.flParticipantConditions.Each(c => {
                requirements.Add(new AuctionRequirementsToParticipantModel { RequirementId = $"Forest_{trade.flId}_{index}", Text = c["flCondition"].ToString() });
                index++;
            });
            //});
            return requirements.ToArray();
        }
        private static string GetObjectOnMapUrl(int ObjectId, string ObjectType) {
            return $"https://lands.qoldau.kz/ru/lands-map/subsoils?zoomToObject={ObjectId}&objectType={ObjectType}";
        }
    }
}