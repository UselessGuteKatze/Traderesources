using System;
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
using LandSource.Helpers;
using LandSource.Models;
using LandSource.QueryTables.LandObject;
using LandSource.QueryTables.Trades;
using LandSource.References.LandObject;
using LandSource.References.Trades;
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
using LandSource.QueryTables.Common;

namespace LandTradesToAuction {

    public class LandTradesJobInput {
        public LandTradesJobInput()
        {
        }
    }

    public class PgQueue {
        public const string LandTradesToAuctionJobQueue = "LandTradesToAuctionJobQueue";
        public const string WaitingLandTradesFromAuctionJobQueue = "WaitingLandTradesFromAuctionJobQueue";
        public const string HeldLandTradesFromAuctionJobQueue = "HeldLandTradesFromAuctionJobQueue";
        public const string LandAgreementsToAuctionJobQueue = "LandAgreementsToAuctionJobQueue";
        public const string LandEgknIdToAuctionJobQueue = "LandEgknIdToAuctionJobQueue";
    }

    public static class LandTradesJobs {
        public static PgJobs<LandTradesJobInput> LandTradesToAuctionJob = new PgJobs<LandTradesJobInput>(PgQueue.LandTradesToAuctionJobQueue, NpGlobal.DbKeys.DbJobs, "Отправляет торги земельными ресурсами");
        public static PgJobs<LandTradesJobInput> WaitingLandTradesFromAuctionJob = new PgJobs<LandTradesJobInput>(PgQueue.WaitingLandTradesFromAuctionJobQueue, NpGlobal.DbKeys.DbJobs, "Пытается забрать результаты ожидающихся торгов");
        public static PgJobs<LandTradesJobInput> HeldLandTradesFromAuctionJob = new PgJobs<LandTradesJobInput>(PgQueue.HeldLandTradesFromAuctionJobQueue, NpGlobal.DbKeys.DbJobs, "Перепроверяет результаты состоявшихся торгов");
        public static PgJobs<LandTradesJobInput> LandAgreementsToAuctionJob = new PgJobs<LandTradesJobInput>(PgQueue.LandAgreementsToAuctionJobQueue, NpGlobal.DbKeys.DbJobs, "Отправляет договоры");
        public static PgJobs<LandTradesJobInput> LandEgknIdToAuctionJob = new PgJobs<LandTradesJobInput>(PgQueue.LandEgknIdToAuctionJobQueue, NpGlobal.DbKeys.DbJobs, "Отправляет идентификаторы ЕГКН в аукцион");

        public static IServiceCollection AddLandTradesToAuctionJobsConsumer(this IServiceCollection services)
        {
            return services.AddDbQueueConsumer(jobs => jobs
                .DbQueue(LandTradesToAuctionJob)
                .Consume(async (input, env) =>
                {
                    try
                    {
                        JobSource.UpLoadTrades(env);

                        var now = DateTime.Now;
                        var newStartAfterDate = now.AddHours(1);

                        return JobFate.Repeat(newStartAfterDate, newStartAfterDate.AddDays(1));
                    }
                    catch (Exception ex)
                    {
                        return JobFate.Retry((env.JobContext.RetriesCount + 1) * 10, ex.ToString());
                    }
                }, batchSize: 1)
            );
        }
        public static IServiceCollection AddWaitingLandTradesFromAuctionJobsConsumer(this IServiceCollection services)
        {
            return services.AddDbQueueConsumer(jobs => jobs
                .DbQueue(WaitingLandTradesFromAuctionJob)
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
        public static IServiceCollection AddHeldLandTradesFromAuctionJobsConsumer(this IServiceCollection services)
        {
            return services.AddDbQueueConsumer(jobs => jobs
                .DbQueue(HeldLandTradesFromAuctionJob)
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
        public static IServiceCollection AddLandAgreementsToAuctionJobsConsumer(this IServiceCollection services)
        {
            return services.AddDbQueueConsumer(jobs => jobs
                .DbQueue(LandAgreementsToAuctionJob)
                .Consume(async (input, env) =>
                {
                    try
                    {
                        JobSource.SendAgreements(env);

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
        public static IServiceCollection AddLandEgknIdToAuctionJobsConsumer(this IServiceCollection services)
        {
            return services.AddDbQueueConsumer(jobs => jobs
                .DbQueue(LandEgknIdToAuctionJob)
                .Consume(async (input, env) =>
                {
                    try
                    {
                        JobSource.SendEgknId(env);

                        var now = DateTime.Now;
                        var newStartAfterDate = now.AddHours(1);

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

        public const string ReSource = "LandResources"; // СУРС
        public static string AuctionObjectType = ""; // ТИП ОБЪЕКТА
        public static string AuctionTradeType = ""; // ТИП ТОРГА

        public static Dictionary<string, string> AuctionObjectTypes = new Dictionary<string, string>() {
            {nameof(LandObjectTypes.Agricultural), "LandAgricultural" },
            {nameof(LandObjectTypes.Сommercial), "Land" },
        };

        public static Dictionary<string, string> AuctionTradeTypes = new Dictionary<string, string>() {
            {RefLandObjectTradeForms.Values.LandTradesPriceUp, "PriceUpLand" },
            {RefLandObjectTradeForms.Values.LandTradesPriceDown, "PriceDownLand" },
            {RefLandObjectTradeForms.Values.ContestRent, "CompetitionAgricultureLand" },
        };

        public static void UpLoadTrades(DbJobConsumeEnvironment env)
        {
            env.Logger.LogInformation("Starting upload process...");

            var langTranslator = new LanguageTranslatorCore(env.QueryExecuter, false);

            var conf = new AuctionServiceSoapClient.EndpointConfiguration();
            var service = new AuctionServiceSoapClient(conf, env.Configuration["AuctionIntegrationServiceUrl"]);

            var tbTradeChanges = new TbLandObjectTradeChanges().AddFilter(t => t.flIsTradeLoaded, false);
            tbTradeChanges.OrderBy = new[] { new OrderField(tbTradeChanges.flDateTime, OrderType.Asc) };
            var dtTradeChanges = tbTradeChanges.Select(new FieldAlias[] { tbTradeChanges.flTradeId, tbTradeChanges.flRevisionId }, env.QueryExecuter);

            if (dtTradeChanges.Rows.Count > 0)
            {
                foreach (DataRow row in dtTradeChanges.Rows)
                {
                    try
                    {
                        //Собираю модельку торга
                        var tradeModel = LandObjectTradeModelHelper.GetTradeRevisionModel(tbTradeChanges.flTradeId.GetRowVal(row), tbTradeChanges.flRevisionId.GetRowVal(row), env.QueryExecuter);
                        AuctionTradeType = AuctionTradeTypes[tradeModel.Method]; //Устанавливаю тип торгов !!!


                        //Если торг завершён, кидаю ошибку
                        if (tradeModel.Status.In(RefLandObjectTradeStatuses.Cancel, RefLandObjectTradeStatuses.NotHeld, RefLandObjectTradeStatuses.Held))
                        {
                            env.Logger.Log(LogLevel.Error, $"Error flag trade id={tradeModel.Id}");
                            continue;
                        }
                        env.Logger.LogInformation($"Preparing trade id={tradeModel.Id}");


                        if (tradeModel.Form == RefLandObjectTradeForms.Values.Contest)
                        {
                            //Если конкурс
                            //env.Logger.LogInformation($"Is contest trade id={tradeModel.Id}");
                            ProcessContest(tradeModel, env, service, langTranslator);
                            //continue;
                        }
                        else if (tradeModel.Form == RefLandObjectTradeForms.Values.EAuction)
                        {
                            //Если аукцион
                            ProcessEAuction(tradeModel, env, service, langTranslator);
                        }
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

            var tbTradeChanges = new TbLandObjectTradeChanges().AddFilter(t => t.flIsTradeLoaded, true);
            var tbTrades = new TbLandObjectsTrades().AddFilter(t => t.flStatus, RefLandObjectTradeStatuses.Wait).AddFilter(t => t.flDateTime, ConditionOperator.LessOrEqual, DateTime.Now);
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

            var tbTradeChanges = new TbLandObjectTradeChanges().AddFilter(t => t.flIsTradeLoaded, true);
            var tbTrades = new TbLandObjectsTrades();//.AddFilter(t => t.flDateTime, ConditionOperator.GreateOrEqual, DateTime.Now.AddMonths(-6));
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
                .AddFilter(t => t.flObjectType, ConditionOperator.In, new[] { "LandAgricultural", "Land" })
                ;
            var dtAgreements = tbAgreements
                .Select(t => new FieldAlias[] { t.flAgreementId, t.flTradeId, t.flAgreementType, t.flAgreementStatus, t.flAgreementCreateDate, t.flAgreementSignDate }, env.QueryExecuter).AsEnumerable();

            foreach (var row in dtAgreements)
            {
                try
                {
                    SendAgreement(row.GetVal(t => t.flAgreementId), row.GetVal(t => t.flTradeId), row.GetVal(t => t.flAgreementType), row.GetVal(t => t.flAgreementStatus), row.GetVal(t => t.flAgreementCreateDate), row.GetValOrNull(t => t.flAgreementSignDate), env);
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

        public static void SendAgreement(int flAgreementId, int flTradeId, string flAgreementType, AgreementStatuses flAgreementStatus, DateTime flAgreementCreateDate, DateTime? flAgreementSignDate, DbJobConsumeEnvironment env)
        {
            var tbTradeChanges = new TbLandObjectTradeChanges()
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
            AgreementTypes typesMap(string flAgreementType)
            {
                if (flAgreementType == "ДоговорКпЗемельногоУчастка")
                {
                    return AgreementTypes.LandPrivate;
                }
                if (flAgreementType == "ДоговорКпПраваАрендыЗемельногоУчастка")
                {
                    return AgreementTypes.LandRentRights;
                }
                throw new NotImplementedException($"unknown agreement type {flAgreementType}");
            }

            if (AgreementHelper.ExistsTbConfiscateAgreements(flAuctionId, flAgreementId, typesMap(flAgreementType), env.QueryExecuter))
            {
                AgreementHelper.UpdateTbConfiscateAgreementsStatus(flAuctionId, flAgreementId, typesMap(flAgreementType), statusesMap(flAgreementStatus), flAgreementSignDate, env.QueryExecuter);
            }
            else
            {
                AgreementHelper.InsertTbConfiscateAgreements(new ConfiscateAgreementsModel(flAuctionId, flAgreementId, typesMap(flAgreementType), statusesMap(flAgreementStatus), flAgreementCreateDate, flAgreementSignDate), env.QueryExecuter);
            }
        }

        public static void SendEgknId(DbJobConsumeEnvironment env) {
            env.Logger.LogInformation("Starting sending EgknId...");

            var tbObjects = new TbLandObjects()
                .AddFilter(t => t.flEgknId, ConditionOperator.NotEqual, DBNull.Value)
                ;
            var dtObjects = tbObjects
                .Select(t => new FieldAlias[] { t.flId, t.flEgknId }, env.QueryExecuter).AsEnumerable();

            foreach (var row in dtObjects) {
                try {
                    if (!string.IsNullOrEmpty(row.GetVal(t => t.flEgknId)) && row.GetVal(t => t.flEgknId).Length == 20) {
                        var tradeId = new TbLandObjectsTrades().AddFilter(t => t.flObjectId, row.GetVal(t => t.flId)).Order(t => t.flDateTime, OrderType.Desc).SelectScalar(t => t.flId, env.QueryExecuter);
                        var auctionIdRow = new TbLandObjectTradeChanges().AddFilter(t => t.flTradeId, tradeId).AddFilter(t => t.flAuctionId, ConditionOperator.NotEqual, DBNull.Value).Order(t => t.flDateTime).SelectFirstOrDefault(t => new FieldAlias[] { t.flAuctionId }, env.QueryExecuter);
                        if (auctionIdRow.IsFirstRowExists) {
                            SendEgknId(auctionIdRow.GetVal(t => t.flAuctionId), row.GetVal(t => t.flEgknId), env);
                        }
                    }
                }
                catch (Exception e) {
                    env.Logger.Log(
                    LogLevel.Error,
                    @$"Error while sending EgknId {row.GetVal(t => t.flEgknId)}: 
                            {e.Message}
                            {e.StackTrace}"
                    );
                }

            }

            env.Logger.LogInformation("End sending EgknId.");
        }

        public static void SendEgknId(int flAuctionId, string flEgknId, DbJobConsumeEnvironment env) {

            bool existsTbAuctionEgknIds(int flAuctionId, IQueryExecuter qe) {
                return new TbAuctionEgknIds().AddFilter(t => t.flAuctionId, flAuctionId).Count(qe) > 0;
            }
            void updateTbAuctionEgknIds(int flAuctionId, string flEgknId, IQueryExecuter qe) {
                new TbAuctionEgknIds().AddFilter(t => t.flAuctionId, flAuctionId).Update().SetT(t => t.flLandId, flEgknId).Execute(qe);
            }
            void insertTbAuctionEgknIds(int flAuctionId, string flEgknId, IQueryExecuter qe) {
                new TbAuctionEgknIds().Insert().SetT(t => t.flAuctionId, flAuctionId).SetT(t => t.flLandId, flEgknId).Execute(qe);
            }

            if (existsTbAuctionEgknIds(flAuctionId, env.QueryExecuter)) {
                updateTbAuctionEgknIds(flAuctionId, flEgknId, env.QueryExecuter);
            }
            else {
                insertTbAuctionEgknIds(flAuctionId, flEgknId, env.QueryExecuter);
            }
        }

        public static void ProcessEAuction(LandObjectTradeModel tradeModel, DbJobConsumeEnvironment env, AuctionServiceSoapClient service, LanguageTranslatorCore langTranslator)
        {
            //Собираю модельку объекта
            var objectModel = LandObjectModelHelper.GetObjectModel(tradeModel.ObjectId, env.QueryExecuter);
            AuctionObjectType = AuctionObjectTypes[objectModel.Type]; //Устанавливаю тип объекта !!!
                                                                      //Собираю нужную инфу объекта из модельки
            var objInfo = getObjectInfo(objectModel, tradeModel, env.QueryExecuter, null, langTranslator);


            //Метадата
            var metaData = new CommonSource.Auction.MetaData
            {
                TraderesourcesObjectInfo = new TraderesourcesObjectInfo
                {
                    LandUrl = GetObjectOnMapUrl(objectModel.Id, "LANDOBJECT")
                }
            };
            //Если аренда, то беру кол-во дней аренды, если рассрочка, то количество месяцев рассрочки. Пихаю в метадату
            metaData.AdditionalInformations = new AdditionalInformations();
            if (tradeModel.Type == RefLandObjectTradeTypes.PrivateOwnership && tradeModel.LandLeaseAvailable == RefLandLeaseAvailable.Yes)
            {
                metaData.AdditionalInformations.LandLeaseMonth = tradeModel.LandLeaseLength.Value;
            }
            else if (tradeModel.Type == RefLandObjectTradeTypes.RentOwnership)
            {
                metaData.AdditionalInformations.LandRentMonth = tradeModel.OwnershipMonths.Value;
            }
            //Реквизиты продавца. Пихаю в метадату
            var competentOrg = getGrObject(tradeModel.TaxAuthorityBin, env.QueryExecuter, null);
            metaData.RequisitsInfo = new RecipientRequisitsInfo
            {
                NameRu = competentOrg.GetVal(t => t.flNameRu),
                NameKz = competentOrg.GetVal(t => t.flNameKz),
                Bik = tradeModel.Bik,
                Bin = tradeModel.TaxAuthorityBin,
                Iik = tradeModel.Iik,
                Kbe = tradeModel.Kbe.ToString(),
                Kbk = tradeModel.Kbk,
                Knp = tradeModel.Knp.ToString(),
                SellerCode = string.Empty
            };
            //Наименование и БИН продавца.
            var sellerInfo = getSellerInfo(objectModel.SallerBin, env.QueryExecuter, null);
            //Адрес продавца.
            var sellerAdr = getSellerAddr(objectModel.SallerBin, env.QueryExecuter, null);

            //Из инфы продавца и метадаты собираю модельку аукциона.
            var auctionObjectModel = getAuctionObject(ReSource, objectModel.Id, AuctionObjectType,
                sellerInfo, sellerAdr, objInfo, metaData);


            //Собираю аукцион
            var auctionData = getAuction(ReSource, tradeModel, auctionObjectModel, AuctionTradeType, metaData);


            //Публикации
            //var refNewspapers = RefNewspapers.GetReference(env.QueryExecuter);
            //var sbPublicationsNoteRu = $"на русском языке - {String.Join("; ", tradeModel.Publications.Where(p => p.NewspaperLang == "100000").Select(p => $"{refNewspapers.Search(p.NewspaperId).Text} - {p.Date.Value.ToShortDateString()} № {p.Number}" )) }";
            //var sbPublicationsNoteKz = $"на государственном языке - {langTranslator.Translate(String.Join("; ", tradeModel.Publications.Where(p => p.NewspaperLang == "110000").Select(p => $"{refNewspapers.Search(p.NewspaperId).Text} - {p.Date.Value.ToShortDateString()}  № {p.Number}")), "kz", "Module: TraderesourcesAuctionIntegrationMonitor") }";
            auctionData.AuctionAdvertisments = new AuctionAdvertismentModel[] { };
            //tradeModel.Publications.Each(p =>
            //{
            //    List<AuctionAdvertismentModel> AuctionAdvertismentsList = auctionData.AuctionAdvertisments.ToList();
            //    AuctionAdvertismentsList.Add(new AuctionAdvertismentModel()
            //    {
            //        Date = p.Date.Value,
            //        NewspaperId = p.NewspaperId,
            //        NewspaperLang = p.NewspaperLang,
            //        Number = p.Number,
            //        Text = p.Text,
            //        ObjectId = objectModel.Id
            //    });

            //    auctionData.AuctionAdvertisments = AuctionAdvertismentsList.ToArray();
            //});

            SendAuction(auctionData, tradeModel, objectModel, env, service);

        }

        public static void ProcessContest(LandObjectTradeModel tradeModel, DbJobConsumeEnvironment env, AuctionServiceSoapClient service, LanguageTranslatorCore langTranslator)
        {
            //Собираю модельку объекта
            var objectModel = LandObjectModelHelper.GetObjectModel(tradeModel.ObjectId, env.QueryExecuter);
            AuctionObjectType = AuctionObjectTypes[objectModel.Type]; //Устанавливаю тип объекта !!!
            //Собираю нужную инфу объекта из модельки
            var objInfo = getObjectInfo(objectModel, tradeModel, env.QueryExecuter, null, langTranslator);


            //Метадата
            var metaData = new CommonSource.Auction.MetaData
            {
                TraderesourcesObjectInfo = new TraderesourcesObjectInfo
                {
                    LandUrl = GetObjectOnMapUrl(objectModel.Id, "LANDOBJECT")
                }
            };
            //Наименование и БИН продавца.
            var sellerInfo = getSellerInfo(objectModel.SallerBin, env.QueryExecuter, null);
            //Адрес продавца.
            var sellerAdr = getSellerAddr(objectModel.SallerBin, env.QueryExecuter, null);

            //Из инфы продавца и метадаты собираю модельку конкурса.
            var contestObjectModel = getAuctionObject(ReSource, objectModel.Id, AuctionObjectType,
                sellerInfo, sellerAdr, objInfo, metaData);


            //Собираю конкурс
            var contestData = getAuction(ReSource, tradeModel, contestObjectModel, AuctionTradeType, metaData, getCommission(tradeModel, env.QueryExecuter, null), getRequiredFiles(tradeModel), getRequirements(tradeModel));


            //Публикации
            contestData.AuctionAdvertisments = new AuctionAdvertismentModel[] { };
            //tradeModel.Publications.Each(p =>
            //{
            //    List<AuctionAdvertismentModel> AuctionAdvertismentsList = contestData.AuctionAdvertisments.ToList();
            //    AuctionAdvertismentsList.Add(new AuctionAdvertismentModel()
            //    {
            //        Date = p.Date.Value,
            //        NewspaperId = p.NewspaperId,
            //        NewspaperLang = p.NewspaperLang,
            //        Number = p.Number,
            //        Text = p.Text,
            //        ObjectId = objectModel.Id
            //    });

            //    contestData.AuctionAdvertisments = AuctionAdvertismentsList.ToArray();
            //});

            SendAuction(contestData, tradeModel, objectModel, env, service);

        }

        public static void SendAuction(AuctionData auctionData, LandObjectTradeModel tradeModel, LandObjectModel objectModel, DbJobConsumeEnvironment env, AuctionServiceSoapClient service)
        {

            ArrayOfString errorMessages;
            string resultStatus;
            int? auctionId = null;
            if (tradeModel.Status.Equals(RefLandObjectTradeStatuses.Wait))
            {
                env.Logger.LogInformation($"Sending trade id={tradeModel.Id}");

                var result = service.SendAuction(auctionData);
                resultStatus = result.Status;
                errorMessages = result.ErrorMessages;

                if (resultStatus == "OK")
                {
                    auctionId = result.Data.AuctionId;

                    env.Logger.LogInformation($"Successfully sended trade id={tradeModel.Id}");
                    // отправляю документы
                    env.Logger.LogInformation($"Sending docs trade id={tradeModel.Id}");
                    objectModel.Docs.Each(p =>
                    {
                        loadDocs(p, auctionId.Value, objectModel.Id, tradeModel.Id, service, env);
                    });
                    env.Logger.LogInformation($"Successfully sended docs trade id={tradeModel.Id}");


                    // отправляю фото
                    env.Logger.LogInformation($"Sending photos trade id={tradeModel.Id}");
                    objectModel.Photos.Each(p =>
                    {
                        loadPhotos(p, auctionId.Value, objectModel.Id, tradeModel.Id, service, env);
                    });
                    env.Logger.LogInformation($"Successfully sended photos trade id={tradeModel.Id}");

                }
                else
                {
                    env.Logger.Log(LogLevel.Error, $"TRADE {tradeModel.Id} ERRORS: {string.Join("; ", errorMessages.ToArray())}");
                }
            }
            else if (tradeModel.Status.Equals(RefLandObjectTradeStatuses.CanceledEarlier))
            {
                env.Logger.LogInformation($"Sending cancel trade id={tradeModel.Id}");
                var result = service.CancelBeforeBegin(ReSource, tradeModel.Id);
                resultStatus = result.Status;
                errorMessages = result.ErrorMessages;
                if (resultStatus == "OK")
                {
                    auctionId = new TbLandObjectTradeChanges().AddFilter(t => t.flTradeId, tradeModel.Id).AddFilterNotNull(t => t.flAuctionId).SelectScalar(t => t.flAuctionId, env.QueryExecuter);
                }
                else
                {
                    env.Logger.Log(LogLevel.Error, $"TRADE {tradeModel.Id} ERRORS: {string.Join("; ", errorMessages.ToArray())}");
                }
            }
            else
            {
                throw new NotImplementedException($"{tradeModel.Status} is not implemented");
            }

            if (errorMessages.Count == 0 && resultStatus == "OK" && auctionId.HasValue)
            {
                new TbLandObjectTradeChanges().AddFilter(t => t.flTradeId, tradeModel.Id).AddFilter(t => t.flRevisionId, tradeModel.RevisionId)
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
                    new TbLandObjectTradeChanges().AddFilter(t => t.flTradeId, tradeModel.Id).AddFilter(t => t.flRevisionId, tradeModel.RevisionId)
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

            var tradeStatus = LandObjectTradeModelHelper.GetTradeStatus(tradeId, env.QueryExecuter);
            var tradeMethod = LandObjectTradeModelHelper.GetTradeMethod(tradeId, env.QueryExecuter);
            var protocolDate = LandObjectTradeModelHelper.GetTradeProtocolDate(tradeId, env.QueryExecuter);
            var objectId = LandObjectTradeModelHelper.GetTradeObjectId(tradeId, env.QueryExecuter);
            var objectBlock = LandObjectModelHelper.GetBlock(objectId, env.QueryExecuter);
            var objectType = LandObjectModelHelper.GetType(objectId, env.QueryExecuter);
            var objectLastTradeId = LandObjectModelHelper.GetObjectLastTradeId(objectId, env.QueryExecuter);

            Dictionary<string, string> AuctionTradeMethods = new Dictionary<string, string>() {
                            {RefLandObjectTradeForms.Values.LandTradesPriceUp, "PriceUpLand" },
                            {RefLandObjectTradeForms.Values.LandTradesPriceDown, "PriceDownLand" },
                            {RefLandObjectTradeForms.Values.ContestRent, "CompetitionAgricultureLand" },
                        };
            Dictionary<string, string> AuctionObjectTypes = new Dictionary<string, string>() {
                            {nameof(LandObjectTypes.Agricultural), "LandAgricultural" },
                            {nameof(LandObjectTypes.Сommercial), "Land" },
                        };

            var agreementModel = AgreementHelper.GetAgreementModel(objectId, AuctionObjectTypes[objectType], tradeId, AuctionTradeMethods[tradeMethod], env.QueryExecuter);

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
                                if (tradeStatus != RefLandObjectTradeStatuses.Held)
                                {
                                    LandObjectTradeModelHelper.SetTradeStatus(tradeId, RefLandObjectTradeStatuses.Held, env.QueryExecuter, transaction);
                                }
                                if (result.Data.Data != null)
                                {
                                    if (result.Data.Data.WinnerProtocolSignedDate.HasValue)
                                    {
                                        if (!new[] { LandObjectBlocks.SaledProt.ToString(), LandObjectBlocks.SaledAgr.ToString() }.Contains(objectBlock))
                                        {
                                            LandObjectModelHelper.SetBlock(objectId, LandObjectBlocks.SaledProt, env.QueryExecuter, transaction);
                                        }
                                        if (tradeStatus != RefLandObjectTradeStatuses.Held || !protocolDate.HasValue) {
                                            LandObjectTradeModelHelper.SetTradeProtocol(tradeId, auctionId, env.QueryExecuter, transaction);
                                            LandObjectTradeModelHelper.SetTradeProtocolDate(tradeId, result.Data.Data.WinnerProtocolSignedDate.Value, env.QueryExecuter, transaction);
                                            LandObjectTradeModelHelper.SetTradeSaleCost(tradeId, result.Data.Data.Price.Value, env.QueryExecuter, transaction);
                                            LandObjectTradeModelHelper.SetTradeWinnerData(tradeId, JsonConvert.DeserializeObject<WinnerData>(JsonConvert.SerializeObject(result.Data.Data.Winner)), result.Data.Data.WinnerGuaranteePayments.Select(payment => new PaymentItemModel() {
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
                                    LandObjectModelHelper.SetBlock(objectId, LandObjectBlocks.ActiveFree, env.QueryExecuter, transaction);
                                }
                                if (tradeStatus != RefLandObjectTradeStatuses.Cancel)
                                {
                                    LandObjectTradeModelHelper.SetTradeStatus(tradeId, RefLandObjectTradeStatuses.Cancel, env.QueryExecuter, transaction);
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
                                    LandObjectModelHelper.SetBlock(objectId, LandObjectBlocks.ActiveFree, env.QueryExecuter, transaction);
                                }
                                if (tradeStatus != RefLandObjectTradeStatuses.NotHeld)
                                {
                                    LandObjectTradeModelHelper.SetTradeStatus(tradeId, RefLandObjectTradeStatuses.NotHeld, env.QueryExecuter, transaction);
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

            var tradeStatus = LandObjectTradeModelHelper.GetTradeStatus(tradeId, env.QueryExecuter);
            var tradeMethod = LandObjectTradeModelHelper.GetTradeMethod(tradeId, env.QueryExecuter);
            var protocolDate = LandObjectTradeModelHelper.GetTradeProtocolDate(tradeId, env.QueryExecuter);
            var objectId = LandObjectTradeModelHelper.GetTradeObjectId(tradeId, env.QueryExecuter);
            var objectBlock = LandObjectModelHelper.GetBlock(objectId, env.QueryExecuter);
            var objectType = LandObjectModelHelper.GetType(objectId, env.QueryExecuter);
            var objectLastTradeId = LandObjectModelHelper.GetObjectLastTradeId(objectId, env.QueryExecuter);

            Dictionary<string, string> AuctionTradeMethods = new Dictionary<string, string>() {
                            {RefLandObjectTradeForms.Values.LandTradesPriceUp, "PriceUpLand" },
                            {RefLandObjectTradeForms.Values.LandTradesPriceDown, "PriceDownLand" },
                            {RefLandObjectTradeForms.Values.ContestRent, "CompetitionAgricultureLand" },
                        };
            Dictionary<string, string> AuctionObjectTypes = new Dictionary<string, string>() {
                            {nameof(LandObjectTypes.Agricultural), "LandAgricultural" },
                            {nameof(LandObjectTypes.Сommercial), "Land" },
                        };

            var agreementModel = AgreementHelper.GetAgreementModel(objectId, AuctionObjectTypes[objectType], tradeId, AuctionTradeMethods[tradeMethod], env.QueryExecuter);

            var result = service.GetResult(ReSource, tradeId);

            if (result.Data != null)
            {
                var auctionId = result.Data.AuctionId;
                var auctionStatus = result.Data.Status;

                env.Logger.LogInformation($"Status: {auctionStatus}");

                if (auctionStatus == "Success") {
                    LandObjectTradeModelHelper.SetTradeProtocol(tradeId, auctionId, env.QueryExecuter);
                    LandObjectTradeModelHelper.SetTradeWinnerData(tradeId, JsonConvert.DeserializeObject<WinnerData>(JsonConvert.SerializeObject(result.Data.Data.Winner)), result.Data.Data.WinnerGuaranteePayments.Select(payment => new PaymentItemModel() {
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
                                if (tradeStatus != RefLandObjectTradeStatuses.Held)
                                {
                                    LandObjectTradeModelHelper.SetTradeStatus(tradeId, RefLandObjectTradeStatuses.Held, env.QueryExecuter, transaction);
                                }
                                if (result.Data.Data != null)
                                {
                                    if (result.Data.Data.WinnerProtocolSignedDate.HasValue)
                                    {
                                        if (!new[] { LandObjectBlocks.SaledProt.ToString(), LandObjectBlocks.SaledAgr.ToString() }.Contains(objectBlock))
                                        {
                                            LandObjectModelHelper.SetBlock(objectId, LandObjectBlocks.SaledProt, env.QueryExecuter, transaction);
                                        }
                                        if (tradeStatus != RefLandObjectTradeStatuses.Held || !protocolDate.HasValue) {
                                            LandObjectTradeModelHelper.SetTradeProtocol(tradeId, auctionId, env.QueryExecuter, transaction);
                                            LandObjectTradeModelHelper.SetTradeProtocolDate(tradeId, result.Data.Data.WinnerProtocolSignedDate.Value, env.QueryExecuter, transaction);
                                            LandObjectTradeModelHelper.SetTradeSaleCost(tradeId, result.Data.Data.Price.Value, env.QueryExecuter, transaction);
                                            LandObjectTradeModelHelper.SetTradeWinnerData(tradeId, JsonConvert.DeserializeObject<WinnerData>(JsonConvert.SerializeObject(result.Data.Data.Winner)), result.Data.Data.WinnerGuaranteePayments.Select(payment => new PaymentItemModel() {
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
                                    LandObjectModelHelper.SetBlock(objectId, LandObjectBlocks.ActiveFree, env.QueryExecuter, transaction);
                                }
                                if (tradeStatus != RefLandObjectTradeStatuses.Cancel)
                                {
                                    LandObjectTradeModelHelper.SetTradeStatus(tradeId, RefLandObjectTradeStatuses.Cancel, env.QueryExecuter, transaction);
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
                                    LandObjectModelHelper.SetBlock(objectId, LandObjectBlocks.ActiveFree, env.QueryExecuter, transaction);
                                }
                                if (tradeStatus != RefLandObjectTradeStatuses.NotHeld)
                                {
                                    LandObjectTradeModelHelper.SetTradeStatus(tradeId, RefLandObjectTradeStatuses.NotHeld, env.QueryExecuter, transaction);
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
        private static AuctionData getAuction(string source, LandObjectTradeModel trade, AuctionObjectModel auctionObject, string AuctionType, CommonSource.Auction.MetaData metaData, AuctionCommissionModel[] commissions = null, AuctionRequiredFilesGroupModel[] requieredFiles = null,
            AuctionRequirementsToParticipantModel[] requirements = null)
        {
            AuctionData auctionData = new AuctionData();

            auctionData.Source = source;
            auctionData.SourceObjectId = trade.Id;
            auctionData.AuctionObject = auctionObject;
            auctionData.AuctionType = AuctionType;
            auctionData.StartDate = trade.DateTime;
            //auctionData.AcceptAppsEndDate = trade.DateTime;
            auctionData.StartCost = trade.CostStart;
            auctionData.MinCost = trade.CostMin;
            auctionData.MinParticipantsCount = trade.AdditionalCondition == RefConditionParticipants.Values.Allowed ? 1 : 2;
            auctionData.GuaranteePaymentCost = trade.Deposit;
            auctionData.MetaData = JsonConvert.SerializeObject(metaData);
            auctionData.MetaDataType = nameof(CommonSource.Auction.MetaData);
            auctionData.LandOwnershipType = trade.Type;

            if (!string.IsNullOrEmpty(trade.Note))
            {
                auctionData.Note = trade.Note;
                auctionData.NoteKz = trade.Note;
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
        private static ObjectInfo getObjectInfo(LandObjectModel objectModel, LandObjectTradeModel tradeModel, IQueryExecuter queryExecuter, ITransaction transaction, ILangTranslator langTranslator)
        {
            var tbLandObjects = new TbLandObjects();
            var gid = getGlobalObjectId(objectModel.SallerBin, queryExecuter, transaction);
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
                AdrCountry = getMappedKato(objectModel.Country),
                AdrObl = getMappedKato(objectModel.Region),
                AdrReg = getMappedKato(objectModel.District),
                AdrAdr = objectModel.Address,
                NameRu = $"Участок: {objectModel.Name}",
                NameKz = $"Жер телімі: {langTranslator.Translate(objectModel.Name, "kz", "Reference:TraderesourcesAuctionIntegrationMonitor")}",
                BalansHolderInfoRu = balansHolderInfoRu,
                BalansHolderInfoKz = balansHolderInfoKz
            };

            var sbFullInfoRu = new StringBuilder();
            var sbFullInfoKz = new StringBuilder();

            var refFuncPurpose = RefUpgsLandObjectsFuncPurpose.GetReference(queryExecuter);
            var refUsageAim = RefUpgsLandObjectsUsageAim.GetReference(queryExecuter);
            var refIsDivisible = RefUpgsLandObjectsIsDivisible.GetReference(queryExecuter);

            sbFullInfoRu.AppendFormat($"{tbLandObjects.flDescription.Text}: {objectModel.Description}; ");
            sbFullInfoKz.AppendFormat($"{langTranslator.Translate(tbLandObjects.flDescription.Text, "kz", "Reference:TraderesourcesAuctionIntegrationMonitor")}: {langTranslator.Translate(objectModel.Description, "kz", "Reference:TraderesourcesAuctionIntegrationMonitor")}; ");

            sbFullInfoRu.AppendFormat($"{tbLandObjects.flKadNumber.Text}: {objectModel.KadNumber}; ");
            sbFullInfoKz.AppendFormat($"{langTranslator.Translate(tbLandObjects.flKadNumber.Text, "kz", "Reference:TraderesourcesAuctionIntegrationMonitor")}: {langTranslator.Translate(objectModel.KadNumber, "kz", "Reference:TraderesourcesAuctionIntegrationMonitor")}; ");

            sbFullInfoRu.AppendFormat($"{tbLandObjects.flLandArea.Text}: {objectModel.LandArea}; ");
            sbFullInfoKz.AppendFormat($"{langTranslator.Translate(tbLandObjects.flLandArea.Text, "kz", "Reference:TraderesourcesAuctionIntegrationMonitor")}: {objectModel.LandArea}; ");


            sbFullInfoRu.AppendFormat($"{tbLandObjects.flFuncPurpose2.Text}: {refFuncPurpose.Search(objectModel.FuncPurpose2).Text}; ");
            sbFullInfoKz.AppendFormat($"{langTranslator.Translate(tbLandObjects.flFuncPurpose2.Text, "kz", "Reference:TraderesourcesAuctionIntegrationMonitor")}: {langTranslator.Translate(refFuncPurpose.Search(objectModel.FuncPurpose2).Text, "kz", "Reference:TraderesourcesAuctionIntegrationMonitor")}; ");

            sbFullInfoRu.AppendFormat($"{tbLandObjects.flFuncPurpose3.Text}: {refFuncPurpose.Search(objectModel.FuncPurpose3).Text}; ");
            sbFullInfoKz.AppendFormat($"{langTranslator.Translate(tbLandObjects.flFuncPurpose3.Text, "kz", "Reference:TraderesourcesAuctionIntegrationMonitor")}: {langTranslator.Translate(refFuncPurpose.Search(objectModel.FuncPurpose3).Text, "kz", "Reference:TraderesourcesAuctionIntegrationMonitor")}; ");

            sbFullInfoRu.AppendFormat($"{tbLandObjects.flFuncPurpose4.Text}: {refFuncPurpose.Search(objectModel.FuncPurpose4).Text}; ");
            sbFullInfoKz.AppendFormat($"{langTranslator.Translate(tbLandObjects.flFuncPurpose4.Text, "kz", "Reference:TraderesourcesAuctionIntegrationMonitor")}: {langTranslator.Translate(refFuncPurpose.Search(objectModel.FuncPurpose4).Text, "kz", "Reference:TraderesourcesAuctionIntegrationMonitor")}; ");


            sbFullInfoRu.AppendFormat($"{tbLandObjects.flUsageAim.Text}: {refUsageAim.Search(objectModel.UsageAim).Text}; ");
            sbFullInfoKz.AppendFormat($"{langTranslator.Translate(tbLandObjects.flUsageAim.Text, "kz", "Reference:TraderesourcesAuctionIntegrationMonitor")}: {langTranslator.Translate(refUsageAim.Search(objectModel.UsageAim).Text, "kz", "Reference:TraderesourcesAuctionIntegrationMonitor")}; ");
            
            sbFullInfoRu.AppendFormat($"{tbLandObjects.flPurpose.Text}: {objectModel.Purpose}; ");
            sbFullInfoKz.AppendFormat($"{langTranslator.Translate(tbLandObjects.flPurpose.Text, "kz", "Reference:TraderesourcesAuctionIntegrationMonitor")}: {objectModel.Purpose}; ");

            sbFullInfoRu.AppendFormat($"{tbLandObjects.flIsDivisible.Text}: {refIsDivisible.Search(objectModel.IsDivisible).Text}; ");
            sbFullInfoKz.AppendFormat($"{langTranslator.Translate(tbLandObjects.flIsDivisible.Text, "kz", "Reference:TraderesourcesAuctionIntegrationMonitor")}: {langTranslator.Translate(refIsDivisible.Search(objectModel.IsDivisible).Text, "kz", "Reference:TraderesourcesAuctionIntegrationMonitor")}; ");

            if (objectModel.ChargePresence == RefBoolChargePresence.Values.HasChargeArest)
            {
                if (!string.IsNullOrEmpty(objectModel.Charge))
                {
                    sbFullInfoRu.AppendFormat($"{tbLandObjects.flCharge.Text}: {objectModel.Charge}; ");
                    sbFullInfoKz.AppendFormat($"{langTranslator.Translate(tbLandObjects.flCharge.Text, "kz", "Reference:TraderesourcesAuctionIntegrationMonitor")}: {langTranslator.Translate(objectModel.Charge, "kz", "Reference:TraderesourcesAuctionIntegrationMonitor")}; ");
                }
                if (!string.IsNullOrEmpty(objectModel.Restriction))
                {
                    sbFullInfoRu.AppendFormat($"{tbLandObjects.flRestriction.Text}: {objectModel.Restriction}; ");
                    sbFullInfoKz.AppendFormat($"{langTranslator.Translate(tbLandObjects.flRestriction.Text, "kz", "Reference:TraderesourcesAuctionIntegrationMonitor")}: {langTranslator.Translate(objectModel.Restriction, "kz", "Reference:TraderesourcesAuctionIntegrationMonitor")}; ");
                }
                if (!string.IsNullOrEmpty(objectModel.Arest))
                {
                    sbFullInfoRu.AppendFormat($"{tbLandObjects.flArest.Text}: {objectModel.Arest}; ");
                    sbFullInfoKz.AppendFormat($"{langTranslator.Translate(tbLandObjects.flArest.Text, "kz", "Reference:TraderesourcesAuctionIntegrationMonitor")}: {langTranslator.Translate(objectModel.Arest, "kz", "Reference:TraderesourcesAuctionIntegrationMonitor")}; ");
                }
            }

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
        private static AuctionCommissionModel[] getCommission(LandObjectTradeModel trade, IQueryExecuter queryExecuter, ITransaction transaction)
        {

            return trade.CommissionMembers.Select(c =>
            {
                var comm = new AuctionCommissionModel
                {
                    Role = getComissionRole(c.Role)
                };
                var commission = getCommissioner(c.MemberId, trade.CompetentOrgBin, queryExecuter, transaction);
                comm.IsChairman = getComissionRole(c.Role).Equals(RefAuctionCommissionRoles.Chairman);
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
        private static AuctionRequiredFilesGroupModel[] getRequiredFiles(LandObjectTradeModel trade)
        {
            var requiredFiles = new List<AuctionRequiredFilesGroupModel>();
            var userTypes = new List<string>() { "Corporate", "Individual", "IndividualCorporate" };
            userTypes.ForEach(userType =>
            {
                requiredFiles.AddRange(trade.RequiredDocuments.Select(d => new AuctionRequiredFilesGroupModel { GroupName = $"{userType}_{d.Id}", GroupText = d.Text, ForUserType = userType }));
            });
            return requiredFiles.ToArray();
        }
        private static AuctionRequirementsToParticipantModel[] getRequirements(LandObjectTradeModel trade)
        {
            var requirements = new List<AuctionRequirementsToParticipantModel>();
            if (trade.ParticipantConditions == null)
            {
                return requirements.ToArray();
            }
            //var userTypes = new List<string>() { "Corporate", "Individual", "IndividualCorporate" };
            //userTypes.ForEach(userType => {
            requirements.AddRange(trade.ParticipantConditions.Select(c => new AuctionRequirementsToParticipantModel { RequirementId = c.Id, Text = c.Text }));
            //});
            return requirements.ToArray();
        }
        private static string GetObjectOnMapUrl(int ObjectId, string ObjectType)
        {
            return $"https://lands.qoldau.kz/ru/lands-map/subsoils?zoomToObject={ObjectId}&objectType={ObjectType}";
        }
    }
}