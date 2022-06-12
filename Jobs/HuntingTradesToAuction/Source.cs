using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AuctionService;
using FileStoreInterfaces;
using HuntingSource.FieldEditors.Trade;
using CommonSource;
using CommonSource.Auction;
using HuntingSource.Helpers.Object;
using HuntingSource.Helpers.Trade;
using HuntingSource.Models;
using HuntingSource.QueryTables.Common;
using HuntingSource.QueryTables.Object;
using HuntingSource.QueryTables.Trade;
using HuntingSource.References.Object;
using HuntingSource.References.Trade;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Yoda.Application.Core;
using Yoda.Interfaces;
using YodaApp.DbQueues;
using YodaQuery;
using CommonSource.QueryTables;
using CommonSource.References.Trade;
using YodaApp.Yoda.Application.Helpers;

namespace HuntingTradesToAuction {

    public class HuntingTradesJobInput {
        public HuntingTradesJobInput()
        {
        }
    }

    public class PgQueue {
        public const string HuntingTradesToAuctionJobQueue = "HuntingTradesToAuctionJobQueue";
        public const string WaitingHuntingTradesFromAuctionJobQueue = "WaitingHuntingTradesFromAuctionJobQueue";
        public const string HeldHuntingTradesFromAuctionJobQueue = "HeldHuntingTradesFromAuctionJobQueue";
        public const string HuntingAgreementsToAuctionJobQueue = "HuntingAgreementsToAuctionJobQueue";
    }

    public static class HuntingTradesJobs {
        public static PgJobs<HuntingTradesJobInput> HuntingTradesToAuctionJob = new PgJobs<HuntingTradesJobInput>(PgQueue.HuntingTradesToAuctionJobQueue, NpGlobal.DbKeys.DbJobs, "Отправляет торги охот угодий");
        public static PgJobs<HuntingTradesJobInput> WaitingHuntingTradesFromAuctionJob = new PgJobs<HuntingTradesJobInput>(PgQueue.WaitingHuntingTradesFromAuctionJobQueue, NpGlobal.DbKeys.DbJobs, "Пытается забрать результаты ожидающихся торгов");
        public static PgJobs<HuntingTradesJobInput> HeldHuntingTradesFromAuctionJob = new PgJobs<HuntingTradesJobInput>(PgQueue.HeldHuntingTradesFromAuctionJobQueue, NpGlobal.DbKeys.DbJobs, "Перепроверяет результаты состоявшихся торгов");
        public static PgJobs<HuntingTradesJobInput> HuntingAgreementsToAuctionJob = new PgJobs<HuntingTradesJobInput>(PgQueue.HuntingAgreementsToAuctionJobQueue, NpGlobal.DbKeys.DbJobs, "Отправляет договоры");

        public static IServiceCollection AddHuntingTradesToAuctionJobsConsumer(this IServiceCollection services)
        {
            return services.AddDbQueueConsumer(jobs => jobs
                .DbQueue(HuntingTradesToAuctionJob)
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
        public static IServiceCollection AddWaitingHuntingTradesFromAuctionJobsConsumer(this IServiceCollection services)
        {
            return services.AddDbQueueConsumer(jobs => jobs
                .DbQueue(WaitingHuntingTradesFromAuctionJob)
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
        public static IServiceCollection AddHeldHuntingTradesFromAuctionJobsConsumer(this IServiceCollection services)
        {
            return services.AddDbQueueConsumer(jobs => jobs
                .DbQueue(HeldHuntingTradesFromAuctionJob)
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
        public static IServiceCollection AddHuntingAgreementsToAuctionJobsConsumer(this IServiceCollection services)
        {
            return services.AddDbQueueConsumer(jobs => jobs
                .DbQueue(HuntingAgreementsToAuctionJob)
                .Consume(async (input, env) =>
                {
                    try
                    {
                        JobSource.SendAgreements(env);

                        var now = DateTime.Now;
                        var newStartAfterDate = now.AddHours(24);

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

        public const string ReSource = "AnimalWorldHunting"; // СУРС
        public static string AuctionObjectType = "HuntingLand"; // ТИП ОБЪЕКТА
        public static string AuctionTradeType = "CompetitionHunt"; // ТИП ТОРГА

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
                        if (tradeModel.flStatus.In(RefTradesStatuses.Cancel, RefTradesStatuses.NotHeld, RefTradesStatuses.Held))
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
            var tbTrades = new TbTrades().AddFilter(t => t.flStatus, RefTradesStatuses.Wait).AddFilter(t => t.flDateTime, ConditionOperator.LessOrEqual, DateTime.Now);
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

        public static void SendAgreements(DbJobConsumeEnvironment env)
        {
            env.Logger.LogInformation("Starting sending agreements...");

            var tbAgreements = new TbAgreements()
                .AddFilter(t => t.flObjectType, "HuntingLand")
                ;
            var dtAgreements = tbAgreements
                .Select(t => new FieldAlias[] { t.flAgreementId, t.flTradeId, t.flAgreementStatus, t.flAgreementCreateDate, t.flAgreementSignDate }, env.QueryExecuter).AsEnumerable();

            foreach (var row in dtAgreements)
            {
                try
                {
                    SendAgreement(row.GetVal(t => t.flAgreementId), row.GetVal(t => t.flTradeId), row.GetVal(t => t.flAgreementStatus), row.GetVal(t => t.flAgreementCreateDate), row.GetValOrNull(t => t.flAgreementSignDate), env);
                }
                catch (Exception e)
                {
                    env.Logger.Log(
                    LogLevel.Error,
                    @$"Error while sending agreement {row.GetVal(t => t.flAgreementId)}: 
                            {e.Message}
                            {e.StackTrace}"
                    );
                }

            }

            env.Logger.LogInformation("End sending agreements.");
        }

        public static void SendAgreement(int flAgreementId, int flTradeId, AgreementStatuses flAgreementStatus, DateTime flAgreementCreateDate, DateTime? flAgreementSignDate, DbJobConsumeEnvironment env)
        {
            var tbTradeChanges = new TbTradeChanges()
                .AddFilter(t => t.flTradeId, flTradeId)
                .AddFilterNotNull(t => t.flAuctionId)
                .AddFilter(t => t.flIsTradeLoaded, true)
            ;
            var flAuctionId = tbTradeChanges.SelectScalar(t => t.flAuctionId, env.QueryExecuter).Value;

            ConfiscateAgreementStatuses statusesMap(AgreementStatuses flAgreementStatus)
            {
                if (new[] {
                    AgreementStatuses.Saved,
                    AgreementStatuses.OnApproval,
                    AgreementStatuses.OnCorrection,
                    AgreementStatuses.Agreed,
                    AgreementStatuses.SignedWinner,
                    AgreementStatuses.SignedSeller,
                }.Contains(flAgreementStatus))
                {
                    return ConfiscateAgreementStatuses.Saved;
                }
                if (flAgreementStatus == AgreementStatuses.Signed)
                {
                    return ConfiscateAgreementStatuses.Signed;
                }
                if (flAgreementStatus == AgreementStatuses.Deleted)
                {
                    return ConfiscateAgreementStatuses.Deleted;
                }
                if (flAgreementStatus == AgreementStatuses.Canceled)
                {
                    return ConfiscateAgreementStatuses.Canceled;
                }
                throw new NotImplementedException($"unknown agreement status {flAgreementStatus}");
            }

            if (AgreementHelper.ExistsTbConfiscateAgreements(flAuctionId, flAgreementId, env.QueryExecuter))
            {
                AgreementHelper.UpdateTbConfiscateAgreementsStatus(flAuctionId, flAgreementId, statusesMap(flAgreementStatus), flAgreementSignDate, env.QueryExecuter);
            }
            else
            {
                AgreementHelper.InsertTbConfiscateAgreements(new ConfiscateAgreementsModel(flAuctionId, flAgreementId, statusesMap(flAgreementStatus), flAgreementCreateDate, flAgreementSignDate), env.QueryExecuter);
            }
        }

        public static void ProcessEAuction(HuntingTradeModel tradeModel, DbJobConsumeEnvironment env, AuctionServiceSoapClient service, LanguageTranslatorCore langTranslator)
        {
            //Собираю модельку объекта
            var objectModel = ObjectHelper.GetObjectModel(tradeModel.flObjectId, env.QueryExecuter);
            //AuctionObjectType = AuctionObjectTypes[objectModel.Type]; //Устанавливаю тип объекта !!!
            //Собираю нужную инфу объекта из модельки
            var objInfo = getObjectInfo(objectModel, tradeModel, env.QueryExecuter, null, langTranslator);


            //Метадата
            var metaData = new CommonSource.Auction.MetaData
            {
                TraderesourcesObjectInfo = new TraderesourcesObjectInfo
                {
                    LandUrl = GetObjectOnMapUrl(objectModel.flId, "HUNTINGOBJECT")
                }
            };
            ////Если аренда, то беру кол-во дней аренды, если рассрочка, то количество месяцев рассрочки. Пихаю в метадату
            //metaData.AdditionalInformations = new AdditionalInformations();
            //if (tradeModel.Type == RefLandObjectTradeTypes.PrivateOwnership && tradeModel.LandLeaseAvailable == RefLandLeaseAvailable.Yes)
            //{
            //    metaData.AdditionalInformations.LandLeaseMonth = tradeModel.LandLeaseLength.Value;
            //}
            //else if (tradeModel.Type == RefLandObjectTradeTypes.RentOwnership)
            //{
            //    metaData.AdditionalInformations.LandRentMonth = tradeModel.OwnershipMonths.Value;
            //}
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
            var sellerInfo = getSellerInfo(objectModel.flSallerBin, env.QueryExecuter, null);
            //Адрес продавца.
            var sellerAdr = getSellerAddr(objectModel.flSallerBin, env.QueryExecuter, null);

            //Из инфы продавца и метадаты собираю модельку аукциона.
            var auctionObjectModel = getAuctionObject(ReSource, objectModel.flId, AuctionObjectType,
                sellerInfo, sellerAdr, objInfo, metaData);

            //Собираю аукцион
            var auctionData = getAuction(ReSource, tradeModel, auctionObjectModel, AuctionTradeType, metaData, getCommission(tradeModel, env.QueryExecuter, null), getRequiredFiles(tradeModel), getRequirements(tradeModel));

            //Публикации
            auctionData.AuctionAdvertisments = new AuctionAdvertismentModel[] { };
            tradeModel.flPublications.Each(p =>
            {
                List<AuctionAdvertismentModel> AuctionAdvertismentsList = auctionData.AuctionAdvertisments.ToList();
                AuctionAdvertismentsList.Add(new AuctionAdvertismentModel()
                {
                    Date = Convert.ToDateTime(p["flDate"]),
                    NewspaperId = p["flNewspaperId"].ToString(),
                    NewspaperLang = p["flLanguageId"].ToString(),
                    Number = p["flNumber"].ToString(),
                    Text = p["flText"].ToString(),
                    ObjectId = objectModel.flId
                });

                auctionData.AuctionAdvertisments = AuctionAdvertismentsList.ToArray();
            });

            SendAuction(auctionData, tradeModel, objectModel, env, service);

        }

        public static void SendAuction(AuctionData auctionData, HuntingTradeModel tradeModel, HuntingObjectModel objectModel, DbJobConsumeEnvironment env, AuctionServiceSoapClient service)
        {

            ArrayOfString errorMessages;
            string resultStatus;
            int? auctionId = null;
            if (tradeModel.flStatus.Equals(RefTradesStatuses.Wait))
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
                    env.Logger.LogInformation($"Sending docs trade id={tradeModel.flId}");
                    objectModel.flDocs.Each(p =>
                    {
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
            else if (tradeModel.flStatus.Equals(RefTradesStatuses.CanceledEarlier))
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
            var agreementModel = AgreementHelper.GetAgreementModel(objectId, "HuntingLand", tradeId, "CompetitionHunt", env.QueryExecuter);
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
                                if (tradeStatus != RefTradesStatuses.Held)
                                {
                                    TradeHelper.SetTradeStatus(tradeId, RefTradesStatuses.Held, env.QueryExecuter, transaction);
                                }
                                if (result.Data.Data != null)
                                {
                                    if (result.Data.Data.WinnerProtocolSignedDate.HasValue)
                                    {
                                        if (!new[] { HuntingObjectBlocks.SaledProt.ToString(), HuntingObjectBlocks.SaledAgr.ToString() }.Contains(objectBlock))
                                        {
                                            ObjectHelper.SetBlock(objectId, HuntingObjectBlocks.SaledProt, env.QueryExecuter, transaction);
                                        }
                                        if (tradeStatus != RefTradesStatuses.Held || !protocolDate.HasValue) {
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
                                        }
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
                                    ObjectHelper.SetBlock(objectId, HuntingObjectBlocks.ActiveFree, env.QueryExecuter, transaction);
                                }
                                if (tradeStatus != RefTradesStatuses.Cancel)
                                {
                                    TradeHelper.SetTradeStatus(tradeId, RefTradesStatuses.Cancel, env.QueryExecuter, transaction);
                                }
                                if (agreementModel != null && agreementModel.flAgreementStatus != AgreementStatuses.Canceled)
                                {
                                    AgreementHelper.SetAgreementStatus(agreementModel.flAgreementId, AgreementStatuses.Canceled, env.QueryExecuter, transaction);
                                    AgreementHelper.SetAgreementModelStatus(agreementModel.flAgreementRevisionId, AgreementStatuses.Canceled, env.QueryExecuter, transaction);
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
                                    ObjectHelper.SetBlock(objectId, HuntingObjectBlocks.ActiveFree, env.QueryExecuter, transaction);
                                }
                                if (tradeStatus != RefTradesStatuses.NotHeld)
                                {
                                    TradeHelper.SetTradeStatus(tradeId, RefTradesStatuses.NotHeld, env.QueryExecuter, transaction);
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
            var agreementModel = AgreementHelper.GetAgreementModel(objectId, "HuntingLand", tradeId, "CompetitionHunt", env.QueryExecuter);
            var result = service.GetResult(ReSource, tradeId);

            if (result.Data != null)
            {
                var auctionId = result.Data.AuctionId;
                var auctionStatus = result.Data.Status;

                env.Logger.LogInformation($"Status: {auctionStatus}");

                if (auctionStatus == "Success") {
                    TradeHelper.SetTradeProtocol(tradeId, auctionId, env.QueryExecuter);
                    TradeHelper.SetTradeWinnerData(tradeId, JsonConvert.DeserializeObject<WinnerData>(JsonConvert.SerializeObject(result.Data.Data.Winner)), result.Data.Data.WinnerGuaranteePayments.Select(payment => new PaymentItemModel() {
                        flId = new YodaHelpers.Payments.PaymentId() { Id = payment.PaymentId, Source = "null" },
                        flDateTime = payment.PaymentDate,
                        flAmount = payment.PaymentAmount,
                        flPurpose = payment.Purpose,
                        flIsGuarantee = true
                    }).ToList(), env.QueryExecuter);
                }

                switch (auctionStatus)
                {
                    case "Success":
                        {
                            using (var transaction = env.QueryExecuter.BeginTransaction())
                            {
                                if (tradeStatus != RefTradesStatuses.Held)
                                {
                                    TradeHelper.SetTradeStatus(tradeId, RefTradesStatuses.Held, env.QueryExecuter, transaction);
                                }
                                if (result.Data.Data != null)
                                {
                                    if (result.Data.Data.WinnerProtocolSignedDate.HasValue)
                                    {
                                        if (!new[] { HuntingObjectBlocks.SaledProt.ToString(), HuntingObjectBlocks.SaledAgr.ToString() }.Contains(objectBlock))
                                        {
                                            ObjectHelper.SetBlock(objectId, HuntingObjectBlocks.SaledProt, env.QueryExecuter, transaction);
                                        }
                                        if (tradeStatus != RefTradesStatuses.Held || !protocolDate.HasValue) {
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
                                        }
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
                                    ObjectHelper.SetBlock(objectId, HuntingObjectBlocks.ActiveFree, env.QueryExecuter, transaction);
                                }
                                if (tradeStatus != RefTradesStatuses.Cancel)
                                {
                                    TradeHelper.SetTradeStatus(tradeId, RefTradesStatuses.Cancel, env.QueryExecuter, transaction);
                                }
                                if (agreementModel != null && agreementModel.flAgreementStatus != AgreementStatuses.Canceled)
                                {
                                    AgreementHelper.SetAgreementStatus(agreementModel.flAgreementId, AgreementStatuses.Canceled, env.QueryExecuter, transaction);
                                    AgreementHelper.SetAgreementModelStatus(agreementModel.flAgreementRevisionId, AgreementStatuses.Canceled, env.QueryExecuter, transaction);
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
                                    ObjectHelper.SetBlock(objectId, HuntingObjectBlocks.ActiveFree, env.QueryExecuter, transaction);
                                }
                                if (tradeStatus != RefTradesStatuses.NotHeld)
                                {
                                    TradeHelper.SetTradeStatus(tradeId, RefTradesStatuses.NotHeld, env.QueryExecuter, transaction);
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

        private static string GenerateSHA256String(byte[] bytes)
        {
            SHA256 sha256 = SHA256Managed.Create();
            byte[] hash = sha256.ComputeHash(bytes);
            return GetStringFromHash(hash);
        }
        private static string GetStringFromHash(byte[] hash)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }
            return result.ToString();
        }
        private static void loadDocs(FileInfo fileInfo, int auctionId, int objectId, int tradeId, AuctionServiceSoapClient service, DbJobConsumeEnvironment env)
        {
            var SHA256Code = "SHA-256";

            var fileSrcId = new TbFilesInfo().AddFilter(t => t.flFileId, fileInfo.FileId).SelectScalar(t => t.flFileSrcId, env.QueryExecuter);
            var fileContent = new TbFiles().AddFilter(t => t.flFileSrcId, fileSrcId).SelectScalar(t => t.flContent, env.QueryExecuter);
            var fileContentHash = GenerateSHA256String(fileContent);

            var fileContentParts = getFileParts(fileContent).Select((filePartContent, filePartNumber) =>
            {
                var filePartContentHash = GenerateSHA256String(filePartContent);

                return new FilePart()
                {
                    Number = filePartNumber + 1,
                    Content = filePartContent,
                    ContentHash = filePartContentHash,
                    Length = filePartContent.Length
                };

            }).ToList();

            int GuidId;

            var tbGuidIds = new TbGuidIds();
            tbGuidIds.AddFilter(tbGuidIds.flGuid, fileSrcId);
            if (tbGuidIds.Count(env.QueryExecuter) == 0)
            {
                using (ITransaction transaction = env.QueryExecuter.BeginTransaction(NpGlobal.DbKeys.DbFiles))
                {
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
            else
            {
                GuidId = tbGuidIds.SelectScalar(t => t.flId, env.QueryExecuter).Value;
            }

            void uploadFile()
            {
                var auctionObjectFile = new AuctionObjectFile()
                {
                    AuctionDigitalFilesSource = new AuctionDigitalFilesSourceModel()
                    {
                        FileId = GuidId,
                        Source = ReSource,
                        SourceId = GuidId
                    },
                    AuctionFileUpload = new AuctionFileUploadModel()
                    {
                        FileHash = fileContentHash,
                        FileName = fileInfo.FileName,
                        FileSize = fileContent.Length,
                        HashAlgorithm = SHA256Code,
                        PartsCount = fileContentParts.Count
                    },
                    AuctionObjectFilesData = new AuctionObjectFilesDataModel()
                    {
                        AuctionId = auctionId,
                        Description = fileInfo.Description,
                        FileId = GuidId,
                        FileName = fileInfo.FileName,
                        FileSize = fileContent.Length
                    }
                };

                var result = service.AddAuctionObjectFileToUpload(ReSource, tradeId, auctionObjectFile);
                if (result.Status != "OK")
                {
                    env.Logger.Log(LogLevel.Error, $"ERRORS: {string.Join("; ", result.ErrorMessages.ToArray())}");
                    uploadFile();
                }
                else
                {
                    var auctionUploadId = result.Data.AuctionFileUpload.UploadId;

                    fileContentParts.ForEach(p =>
                    {
                        void uploadFilePart()
                        {
                            var partResult = service.UploadFilePart(ReSource, auctionUploadId,
                                new AuctionFilePartModel
                                {
                                    FilePart = p.Content,
                                    FilePartHash = p.ContentHash,
                                    FilePartSize = p.Length,
                                    PartNumber = p.Number,
                                    PartId = p.Number
                                });
                            if (partResult.Status != "OK")
                            {
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
        private static void loadPhotos(FileInfo fileInfo, int auctionId, int objectId, int tradeId, AuctionServiceSoapClient service, DbJobConsumeEnvironment env)
        {
            var SHA256Code = "SHA-256";

            var fileSrcId = new TbFilesInfo().AddFilter(t => t.flFileId, fileInfo.FileId).SelectScalar(t => t.flFileSrcId, env.QueryExecuter);
            var fileContent = new TbFiles().AddFilter(t => t.flFileSrcId, fileSrcId).SelectScalar(t => t.flContent, env.QueryExecuter);
            var fileContentHash = GenerateSHA256String(fileContent);

            var fileContentParts = getFileParts(fileContent).Select((filePartContent, filePartNumber) =>
            {
                var filePartContentHash = GenerateSHA256String(filePartContent);

                return new FilePart()
                {
                    Number = filePartNumber + 1,
                    Content = filePartContent,
                    ContentHash = filePartContentHash,
                    Length = filePartContent.Length
                };

            }).ToList();

            int GuidId;

            var tbGuidIds = new TbGuidIds();
            tbGuidIds.AddFilter(tbGuidIds.flGuid, fileSrcId);
            if (tbGuidIds.Count(env.QueryExecuter) == 0)
            {
                using (ITransaction transaction = env.QueryExecuter.BeginTransaction(NpGlobal.DbKeys.DbFiles))
                {
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
            else
            {
                GuidId = tbGuidIds.SelectScalar(t => t.flId, env.QueryExecuter).Value;
            }

            void uploadFile()
            {
                var auctionObjectFile = new AuctionPhoto()
                {
                    AuctionDigitalFilesSource = new AuctionDigitalFilesSourceModel()
                    {
                        FileId = GuidId,
                        Source = ReSource,
                        SourceId = GuidId
                    },
                    AuctionFileUpload = new AuctionFileUploadModel()
                    {
                        FileHash = fileContentHash,
                        FileName = fileInfo.FileName,
                        FileSize = fileContent.Length,
                        HashAlgorithm = SHA256Code,
                        PartsCount = fileContentParts.Count
                    },
                    AuctionObjectPhoto = new AuctionObjectPhotoModel()
                    {
                        PhotoDescription = fileInfo.Description,
                        ObjectId = objectId,
                        PhotoFileId = GuidId
                    }
                };

                var result = service.AddAuctionPhotoToUpload(ReSource, tradeId, auctionObjectFile);
                if (result.Status != "OK")
                {
                    env.Logger.Log(LogLevel.Error, $"ERRORS: {string.Join("; ", result.ErrorMessages.ToArray())}");
                    uploadFile();
                }
                else
                {
                    var auctionUploadId = result.Data.AuctionFileUpload.UploadId;

                    fileContentParts.ForEach(p =>
                    {
                        void uploadFilePart()
                        {
                            var partResult = service.UploadFilePart(ReSource, auctionUploadId,
                                new AuctionFilePartModel
                                {
                                    FilePart = p.Content,
                                    FilePartHash = p.ContentHash,
                                    FilePartSize = p.Length,
                                    PartNumber = p.Number,
                                    PartId = p.Number
                                });
                            if (partResult.Status != "OK")
                            {
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
        private static List<byte[]> getFileParts(byte[] file)
        {
            List<byte[]> result = new List<byte[]>();

            int lengthToSplit = 1048576; // Длина одной части (в байтах)
            int arrayLength = file.Length;

            for (int i = 0; i < arrayLength; i = i + lengthToSplit)
            {
                if (arrayLength < i + lengthToSplit)
                {
                    lengthToSplit = arrayLength - i;
                }
                byte[] val = new byte[lengthToSplit];

                Array.Copy(file, i, val, 0, lengthToSplit);
                result.Add(val);
            }

            return result;
        }
        private static AuctionData getAuction(string source, HuntingTradeModel trade, AuctionObjectModel auctionObject, string AuctionType, CommonSource.Auction.MetaData metaData, AuctionCommissionModel[] commissions = null, AuctionRequiredFilesGroupModel[] requieredFiles = null,
            AuctionRequirementsToParticipantModel[] requirements = null)
        {
            AuctionData auctionData = new AuctionData();

            auctionData.Source = source;
            auctionData.SourceObjectId = trade.flId;
            auctionData.AuctionObject = auctionObject;
            auctionData.AuctionType = AuctionType;
            auctionData.StartDate = trade.flDateTime;
            //auctionData.AcceptAppsEndDate = trade.flDateTime;
            auctionData.StartCost = trade.flCostStart;
            //auctionData.MinCost = trade.CostMin;
            auctionData.MinParticipantsCount = trade.flAdditionalCondition == RefConditionParticipants.Values.Allowed ? 1 : 2;
            auctionData.GuaranteePaymentCost = trade.flDeposit;
            auctionData.MetaData = JsonConvert.SerializeObject(metaData);
            auctionData.MetaDataType = nameof(CommonSource.Auction.MetaData);
            //auctionData.LandOwnershipType = trade.flType;

            if (!string.IsNullOrEmpty(trade.flNote))
            {
                auctionData.Note = trade.flNote;
                auctionData.NoteKz = trade.flNote;
            }

            if (commissions != null)
            {
                auctionData.AuctionCommissions = commissions;
            }
            if (requieredFiles != null)
            {
                auctionData.AuctionRequiredFilesGroups = requieredFiles;
            }
            if (requirements != null)
            {
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
        private static ObjectInfo getObjectInfo(HuntingObjectModel objectModel, HuntingTradeModel tradeModel, IQueryExecuter queryExecuter, ITransaction transaction, ILangTranslator langTranslator)
        {
            var TbObjects = new TbObjects();
            var gid = getGlobalObjectId(objectModel.flSallerBin, queryExecuter, transaction);
            string balansHolderInfoRu, balansHolderInfoKz;
            getBalansHolderInfo(out balansHolderInfoRu, out balansHolderInfoKz, gid, queryExecuter, langTranslator);

            Func<string, string> getMappedKato = (ar) => {
                if (ar == "1") {
                    return "118";
                }
                return AddrCodeMapHelper.GetGrKatoCodeByArRefCode(ar);
            };

            var ret = new ObjectInfo
            {
                AdrCountry = getMappedKato(objectModel.flCountry),
                AdrObl = getMappedKato(objectModel.flRegion),
                AdrReg = getMappedKato(objectModel.flDistrict),
                AdrAdr = objectModel.flLocation,
                NameRu = $"Участок: {objectModel.flName}",
                NameKz = $"Жер телімі: {langTranslator.Translate(objectModel.flName, "kz", "Reference:TraderesourcesAuctionIntegrationMonitor")}",
                BalansHolderInfoRu = balansHolderInfoRu,
                BalansHolderInfoKz = balansHolderInfoKz
            };

            var sbFullInfoRu = new StringBuilder();
            var sbFullInfoKz = new StringBuilder();

            sbFullInfoRu.AppendFormat($"{TbObjects.flDescription.Text}: {objectModel.flDescription}; ");
            sbFullInfoKz.AppendFormat($"{langTranslator.Translate(TbObjects.flDescription.Text, "kz", "Reference:TraderesourcesAuctionIntegrationMonitor")}: {langTranslator.Translate(objectModel.flDescription, "kz", "Reference:TraderesourcesAuctionIntegrationMonitor")}; ");

            sbFullInfoRu.AppendFormat($"{TbObjects.flHuntArea.Text}: {objectModel.flHuntArea}; ");
            sbFullInfoKz.AppendFormat($"{langTranslator.Translate(TbObjects.flHuntArea.Text, "kz", "Reference:TraderesourcesAuctionIntegrationMonitor")}: {objectModel.flHuntArea}; ");

            sbFullInfoRu.AppendFormat($"{TbObjects.flForestArea.Text}: {objectModel.flForestArea}; ");
            sbFullInfoKz.AppendFormat($"{langTranslator.Translate(TbObjects.flForestArea.Text, "kz", "Reference:TraderesourcesAuctionIntegrationMonitor")}: {objectModel.flForestArea}; ");

            sbFullInfoRu.AppendFormat($"{TbObjects.flAgriArea.Text}: {objectModel.flAgriArea}; ");
            sbFullInfoKz.AppendFormat($"{langTranslator.Translate(TbObjects.flAgriArea.Text, "kz", "Reference:TraderesourcesAuctionIntegrationMonitor")}: {objectModel.flAgriArea}; ");

            sbFullInfoRu.AppendFormat($"{TbObjects.flWaterArea.Text}: {objectModel.flWaterArea}; ");
            sbFullInfoKz.AppendFormat($"{langTranslator.Translate(TbObjects.flWaterArea.Text, "kz", "Reference:TraderesourcesAuctionIntegrationMonitor")}: {objectModel.flWaterArea}; ");

            sbFullInfoRu.AppendFormat($"{TbObjects.flLandReserveArea.Text}: {objectModel.flLandReserveArea}; ");
            sbFullInfoKz.AppendFormat($"{langTranslator.Translate(TbObjects.flLandReserveArea.Text, "kz", "Reference:TraderesourcesAuctionIntegrationMonitor")}: {objectModel.flLandReserveArea}; ");

            sbFullInfoRu.AppendFormat($"{TbObjects.flOtherArea.Text}: {objectModel.flOtherArea}; ");
            sbFullInfoKz.AppendFormat($"{langTranslator.Translate(TbObjects.flOtherArea.Text, "kz", "Reference:TraderesourcesAuctionIntegrationMonitor")}: {objectModel.flOtherArea}; ");

            sbFullInfoRu.AppendFormat($"{TbObjects.flRangerSites.Text}: {objectModel.flRangerSites}; ");
            sbFullInfoKz.AppendFormat($"{langTranslator.Translate(TbObjects.flRangerSites.Text, "kz", "Reference:TraderesourcesAuctionIntegrationMonitor")}: {objectModel.flRangerSites}; ");


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
        private static AuctionCommissionModel[] getCommission(HuntingTradeModel trade, IQueryExecuter queryExecuter, ITransaction transaction)
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
        private static AuctionRequiredFilesGroupModel[] getRequiredFiles(HuntingTradeModel trade)
        {
            var requiredFiles = new List<AuctionRequiredFilesGroupModel>();
            var userTypes = new List<string>() { "Corporate", "Individual", "IndividualCorporate" };
            userTypes.ForEach(userType =>
            {
                var index = 0;
                trade.flRequiredDocuments.ToList().ForEach(d =>
                {
                    requiredFiles.Add(new AuctionRequiredFilesGroupModel { GroupName = $"Hunting_{userType}_{trade.flId}_{index}", GroupText = d["flDocument"].ToString(), ForUserType = userType });
                    index++;
                });
            });
            return requiredFiles.ToArray();
        }
        private static AuctionRequirementsToParticipantModel[] getRequirements(HuntingTradeModel trade)
        {
            var requirements = new List<AuctionRequirementsToParticipantModel>();
            if (trade.flParticipantConditions == null)
            {
                return requirements.ToArray();
            }
            //var userTypes = new List<string>() { "Corporate", "Individual", "IndividualCorporate" };
            //userTypes.ForEach(userType => {
            var index = 0;
            trade.flParticipantConditions.Each(c =>
            {
                requirements.Add(new AuctionRequirementsToParticipantModel { RequirementId = $"Hunting_{trade.flId}_{index}", Text = c["flCondition"].ToString() });
                index++;
            });
            //});
            return requirements.ToArray();
        }
        private static string GetObjectOnMapUrl(int ObjectId, string ObjectType)
        {
            return $"https://lands.qoldau.kz/ru/lands-map/subsoils?zoomToObject={ObjectId}&objectType={ObjectType}";
        }
    }
}