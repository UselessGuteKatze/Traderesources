using FileStoreInterfaces;
using LandSource.FieldEditors;
using CommonSource;
using LandSource.Helpers;
using LandSource.Models;
using LandSource.Models.Trades;
using LandSource.QueryTables.LandObject;
using LandSource.QueryTables.Trades;
using LandSource.References.LandObject;
using LandSource.References.Trades;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TradeResourcesPlugin.Helpers;
using TradeResourcesPlugin.Modules.LandObjectsMenus.Object;
using UsersResources;
using Yoda.Interfaces;
using Yoda.Interfaces.Forms;
using Yoda.Interfaces.Forms.Components;
using Yoda.Interfaces.Helpers;
using Yoda.Interfaces.Helpers.Mrp;
using Yoda.Interfaces.Helpers.Mrp.QueryTables;
using Yoda.Interfaces.Helpers.NotWorkingDaysHelper.QueryTables;
using Yoda.YodaReferences;
using YodaApp.YodaHelpers.OrderHelpers;
using YodaHelpers.ActionMenus;
using YodaHelpers.Components;
using YodaHelpers.DateTimeHelper;
using YodaHelpers.Fields;
using YodaHelpers.OrderHelpers;
using YodaQuery;
using TradeResourcesPlugin.Modules.LandObjectsMenus.Trades;

namespace TradeResourcesPlugin.Modules.LandObjectsMenus.Trade {
    public class MnuLandObjectTradeOrderBase: MnuOrderBaseV2<LandObjectTradeModel, LandObjectTradesOrderTypes, LandObjectTradeOrderQueryArgs> {

        public MnuLandObjectTradeOrderBase(string moduleName, string menuName, OrderStandartPermissions perms)
            : base(moduleName, menuName, "Приказы по торгам", perms, () => new TbLandObjectsTradesOrderResult(), () => new TbLandObjectsTradesOrderNegotiations()) {
            AsCallback();
            Enabled(rc => {
                var xin = rc.User.GetUserXin(rc.QueryExecuter);
                if (xin == "050540004455"
                || xin == "050540000002"
                || (!rc.User.IsExternalUser() && !rc.User.IsGuest())
                || rc.User.HasRole("TRADERESOURCES-Земельные ресурсы-Создание приказов", rc.QueryExecuter)/*rc.User.HasCustomRole("landobjects", "appLandEdit", rc.QueryExecuter)*/)
                {
                    return true;
                }
                return false;
            });
        }

        public override Task<ActionItem[]> GetActions(ActionEnv<LandObjectTradeOrderQueryArgs> env) {
            var ret = base.GetActions(env).Result.ToList();

            switch (env.Args.MenuAction) {
                case Actions.ViewOrder:
                    var trade = new TbLandObjectsTradesRevisions().AddFilter(t => t.flRevisionId, env.Args.RevisionId)
                        .GetTradeModelFirst(env.QueryExecuter, null);
                    if (trade != null)
                    {
                        ret.Add(new ActionItem(env.T("Перейти к объекту"), new ActionRedirectData(ModuleName, MnuLandObjectView.MnuName, new LandObjectViewActionQueryArgs { MenuAction = "view", Id = trade.ObjectId })));
                    }

                    break;
                case Actions.CreateFrom:
                    break;
            }

            return Task.FromResult(ret.ToArray());
        }

        public override IWizardFormBuilderWithStep<LandObjectTradeOrderQueryArgs, ModelOrderArgs<LandObjectTradesOrderTypes, LandObjectTradeOrderQueryArgs>, LandObjectTradeModel>
            EditModel(IWizardFormBuilderWithFinishButton<LandObjectTradeOrderQueryArgs, ModelOrderArgs<LandObjectTradesOrderTypes, LandObjectTradeOrderQueryArgs>, LandObjectTradeModel> wizard) {
            return wizard
                .Step("Отмена торгов", step => step
                .Enabled(env => env.Args.OrderType.In(LandObjectTradesOrderTypes.Cancel))
                .OnRendering(re => {
                    var trade = re.Model;

                    viewModelMainData(re.Panel, re.Env, trade);
                    if (trade.InitialCost > 0)
                    {
                        viewModelRequisitesData(re.Panel, re.Env, trade);
                    }
                    if (trade.Method == RefLandObjectTradeForms.Values.LandTradesPriceDown)
                    {
                        viewModelPriceDownData(re.Panel, re.Env, trade);
                    }
                    if (trade.Type == RefLandObjectTradeTypes.RentOwnership)
                    {
                        viewModelRentData(re.Panel, re.Env, trade);
                    }
                    if (trade.Type == RefLandObjectTradeTypes.PrivateOwnership)
                    {
                        viewModelPrivateData(re.Panel, re.Env, trade);
                    }

                    if (trade.Form == RefLandObjectTradeForms.Values.EAuction)
                    {
                        viewModelCalculatedData(re.Panel, re.Env, trade);
                    } else if (trade.Form == RefLandObjectTradeForms.Values.Contest)
                    {
                        viewModelContestData(re.Panel, re.Env, trade);
                    }

                    if (!string.IsNullOrEmpty(trade.WinnerBin))
                    {
                        viewModelSaleData(re.Panel, re.Env, trade);
                    }
                    if (trade.IsSaleProtocolExists == true)
                    {
                        viewModelProtocolData(re.Panel, re.Env, trade);
                    }
                    if (trade.IsSaleAgreementExists == true)
                    {
                        viewModelAgreementData(re.Panel, re.Env, trade);
                    }

                    //viewModelPublicationsData(re.Panel, re.Env, trade);
                })
                .OnValidating(ve => {

                    //var ableToEditLastDate = ve.Model.DateTime.Value.AddWorkdays(-3, ve.Env.QueryExecuter);
                    var ableToEditLastDate = ve.Model.DateTime.Value;

                    if ((ve.Env.QueryExecuter.GetDateTime(NpGlobal.DbKeys.DbYodaGr) >= ableToEditLastDate) && !(!ve.Env.User.IsExternalUser() && !ve.Env.User.IsGuest()))
                    {
                        throw new NotImplementedException(ve.Env.T("Отменять торги до начала можно лишь ранее чем за 3 дня!"));
                    }
                })
                .OnProcessing(pe => {

                    //var ableToEditLastDate = pe.Model.DateTime.Value.AddWorkdays(-3, pe.Env.QueryExecuter);
                    var ableToEditLastDate = pe.Model.DateTime.Value;

                    if ((pe.Env.QueryExecuter.GetDateTime(NpGlobal.DbKeys.DbYodaGr) >= ableToEditLastDate) && !(!pe.Env.User.IsExternalUser() && !pe.Env.User.IsGuest()))
                    {
                        throw new NotImplementedException(pe.Env.T("Отменять торги до начала можно лишь ранее чем за 3 дня!"));
                    }
                }))
                .Step("Перенос торгов", step => step
                .Enabled(env => env.Args.OrderType.In(LandObjectTradesOrderTypes.Transfer))
                .OnRendering(re => {
                    var tbRev = new TbLandObjectsTradesRevisions();
                    var trade = re.Model;
                    var now = re.Env.QueryExecuter.GetDateTime(NpGlobal.DbKeys.DbYodaGr);
                    var minimumDays = 15;
                    trade.DateTime = new DateTime(now.Year, now.Month, now.Day, 10, 0, 0).AddDays(minimumDays);
                    tbRev.flDateTime.RenderCustom(re.Panel, re.Env, trade.DateTime);
                })
                .OnValidating(ve => {
                    var tbRev = new TbLandObjectsTradesRevisions();
                    var trade = ve.Model;
                    tbRev.flDateTime.Validate(ve.Env);

                    if (ve.Env.IsValid)
                    {
                        var flDateTime = tbRev.flDateTime.GetVal(ve.Env);

                        var now = ve.Env.QueryExecuter.GetDateTime(NpGlobal.DbKeys.DbYodaGr);
                        var minimumDays = 15;
                        var minimumTradeDate = new DateTime(now.Year, now.Month, now.Day, 10, 0, 0).AddDays(minimumDays);

                        if ((flDateTime - minimumTradeDate).TotalDays < 0)
                        {
                            var dateTimeMessage = ve.Env.T($"{tbRev.flDateTime.Text} не может быть ранее чем через @@@minimumDays дней после создания приказа").Text;
                            dateTimeMessage = dateTimeMessage.Replace("@@@minimumDays", minimumDays.ToString());
                            ve.Env.AddError(tbRev.flDateTime.FieldName, dateTimeMessage);
                        }

                        var isWorkingDay = new TbNotWorkingDaysCalendar()
                        .AddFilter(t => t.flDate, flDateTime)
                        .Count(ve.Env.QueryExecuter) == 0;
                        var isAcceptedDayOfWeek = flDateTime.DayOfWeek.In(DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday);
                        var isAcceptedTime = new TimeSpan(10, 0, 0) <= flDateTime.TimeOfDay && flDateTime.TimeOfDay <= new TimeSpan(15, 0, 0);

                        if (!isWorkingDay)
                        {
                            var workingDayMessage = ve.Env.T($"Торги могут проводиться только в рабочие дни").Text;
                            ve.Env.AddError(tbRev.flDateTime.FieldName, workingDayMessage);
                        }
                        if (!isAcceptedDayOfWeek)
                        {
                            var workingDayMessage = ve.Env.T($"Торги могут проводиться только со вторника по пятницу").Text;
                            ve.Env.AddError(tbRev.flDateTime.FieldName, workingDayMessage);
                        }
                        if (!isAcceptedTime)
                        {
                            var workingDayMessage = ve.Env.T($"Торги могут проводиться только с 10:00 до 15:00").Text;
                            ve.Env.AddError(tbRev.flDateTime.FieldName, workingDayMessage);
                        }
                    }
                })
                .OnProcessing(pe => {
                    var tbRev = new TbLandObjectsTradesRevisions();
                    var trade = pe.Model;
                    pe.Model.DateTime = tbRev.flDateTime.GetVal(pe.Env);
                }))
                .Step("Метод торгов", step => step
                .Enabled(env => env.Args.OrderType.In(LandObjectTradesOrderTypes.Create, LandObjectTradesOrderTypes.Edit))
                .OnRendering(re => {
                    var tbRev = new TbLandObjectsTradesRevisions();
                    var trade = re.Model;

                    tbRev.flObjectId.RenderCustom(re.Panel, re.Env, trade.ObjectId, readOnly: true);
                    tbRev.flCompetentOrgBin.RenderCustom(re.Panel, re.Env, trade.CompetentOrgBin, readOnly: true);
                    tbRev.flStatus.RenderCustom(re.Panel, re.Env, trade.Status, readOnly: true);

                    tbRev.flForm.RenderCustom(re.Panel, re.Env, trade.Form);

                    var landObjectTrades = new TbLandObjectsTrades()
                        .AddFilter(t => t.flObjectId, trade.ObjectId)
                        .AddFilterNot(t => t.flId, trade.Id);
                    landObjectTrades.OrderBy = new[] { new OrderField(landObjectTrades.flDateTime, OrderType.Desc) };
                    var objectTrades = landObjectTrades.Select(re.Env.QueryExecuter).Select().AsEnumerable().ToList();

                    var ObjectType = new TbLandObjects().AddFilter(t => t.flId, re.Model.ObjectId).SelectScalar(t => t.flType, re.Env.QueryExecuter);

                    if (objectTrades.Count == 0 && ObjectType == nameof(LandObjectTypes.Сommercial))
                    {/*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/
                        re.Model.Method = RefLandObjectTradeForms.Values.LandTradesPriceUp;
                        tbRev.flMethod.RenderCustom(re.Panel, re.Env, re.Model.Method, readOnly: true);
                    }
                    else
                    {
                        tbRev.flMethod.RenderCustom(re.Panel, re.Env, trade.Method);
                    }

                })
                .OnValidating(ve =>
                {
                    var tbRev = new TbLandObjectsTradesRevisions();
                    var trade = ve.Model;
                    var obj = new TbLandObjects().AddFilter(t => t.flId, trade.ObjectId)
                       .GetObjectModelFirst(ve.Env.QueryExecuter, null);

                    tbRev.flForm.Validate(ve.Env);

                    var landObjectTrades = new TbLandObjectsTrades()
                        .AddFilter(t => t.flObjectId, trade.ObjectId)
                        .AddFilterNot(t => t.flId, trade.Id);
                    landObjectTrades.OrderBy = new[] { new OrderField(landObjectTrades.flDateTime, OrderType.Desc) };
                    var objectTrades = landObjectTrades.Select(ve.Env.QueryExecuter).Select().AsEnumerable().ToList();

                    var ObjectType = new TbLandObjects().AddFilter(t => t.flId, ve.Model.ObjectId).SelectScalar(t => t.flType, ve.Env.QueryExecuter);

                    if (!(objectTrades.Count == 0 && ObjectType == nameof(LandObjectTypes.Сommercial)))
                    {
                        tbRev.flMethod.Validate(ve.Env);
                    }

                    if (ve.Env.IsValid)
                    {
                        if (!(objectTrades.Count == 0 && ObjectType == nameof(LandObjectTypes.Сommercial)))
                        {

                            if ((ObjectType != nameof(LandObjectTypes.Agricultural) && tbRev.flMethod.GetVal(ve.Env) == RefLandObjectTradeForms.Values.ContestRent) || (ObjectType == nameof(LandObjectTypes.Agricultural) && tbRev.flMethod.GetVal(ve.Env) != RefLandObjectTradeForms.Values.ContestRent))
                            {
                                ve.Env.AddError(tbRev.flMethod.FieldName, ve.Env.T("Конкурсы проводятся только с объектами типа \"Земельные участки сель хоз назначения\""));
                            }
                        }

                    }

                })
                .OnProcessing(pe => {
                    var tbRev = new TbLandObjectsTradesRevisions();
                    var trade = pe.Model;
                    pe.Model.Form = tbRev.flForm.GetVal(pe.Env);

                    var landObjectTrades = new TbLandObjectsTrades()
                        .AddFilter(t => t.flObjectId, trade.ObjectId)
                        .AddFilterNot(t => t.flId, trade.Id);
                    landObjectTrades.OrderBy = new[] { new OrderField(landObjectTrades.flDateTime, OrderType.Desc) };
                    var objectTrades = landObjectTrades.Select(pe.Env.QueryExecuter).Select().AsEnumerable().ToList();

                    if (objectTrades.Count > 0)
                    {
                        pe.Model.Method = tbRev.flMethod.GetVal(pe.Env);
                    }

                    if (pe.Model.Method == RefLandObjectTradeForms.Values.ContestRent)
                    {
                        pe.Model.Type = RefLandObjectTradeTypes.RentOwnership;
                    }

                }))
                .Step("Данные", step => step
                .Enabled(env => env.Args.OrderType.In(LandObjectTradesOrderTypes.Create, LandObjectTradesOrderTypes.Edit))
                .OnRendering(re => {
                    var tbRev = new TbLandObjectsTradesRevisions();
                    var trade = re.Model;

                    tbRev.flType.RenderCustom(re.Panel, re.Env, trade.Type, readOnly: trade.Method == RefLandObjectTradeForms.Values.ContestRent);
                    tbRev.flDateTime.RenderCustom(re.Panel, re.Env, trade.DateTime);
                    tbRev.flNote.RenderCustom(re.Panel, re.Env, trade.Note);

                })
                .OnValidating(ve =>
                {
                    var tbRev = new TbLandObjectsTradesRevisions();
                    var trade = ve.Model;
                    var obj = new TbLandObjects().AddFilter(t => t.flId, trade.ObjectId)
                       .GetObjectModelFirst(ve.Env.QueryExecuter, null);
                    var checkFields = new Field[] {
                            tbRev.flDateTime,
                            tbRev.flNote
                        };
                    checkFields.Each(f => f.Validate(ve.Env));

                    if (ve.Env.IsValid)
                    {
                        var flDateTime = tbRev.flDateTime.GetVal(ve.Env);

                        var now = ve.Env.QueryExecuter.GetDateTime(NpGlobal.DbKeys.DbYodaGr);
                        var minimumDays = GetMinimumTradeDays(trade.Id, trade.Method, trade.ObjectId, ve.Env.QueryExecuter);
                        var minimumTradeDate = new DateTime(now.Year, now.Month, now.Day, 10, 0, 0).AddDays(minimumDays);


                        if ((flDateTime - minimumTradeDate).TotalDays < 0)
                        {
                            var dateTimeMessage = ve.Env.T($"{tbRev.flDateTime.Text} не может быть ранее чем через @@@minimumDays дней после создания приказа").Text;
                            dateTimeMessage = dateTimeMessage.Replace("@@@minimumDays", minimumDays.ToString());
                            ve.Env.AddError(tbRev.flDateTime.FieldName, dateTimeMessage);
                        }

                        var isWorkingDay = new TbNotWorkingDaysCalendar()
                        .AddFilter(t => t.flDate, flDateTime)
                        .Count(ve.Env.QueryExecuter) == 0;
                        var isAcceptedDayOfWeek = flDateTime.DayOfWeek.In(DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday);
                        var isAcceptedTime = new TimeSpan(10, 0, 0) <= flDateTime.TimeOfDay && flDateTime.TimeOfDay <= new TimeSpan(15, 0, 0);

                        if (!isWorkingDay)
                        {
                            var workingDayMessage = ve.Env.T($"Торги могут проводиться только в рабочие дни").Text;
                            ve.Env.AddError(tbRev.flDateTime.FieldName, workingDayMessage);
                        }
                        if (!isAcceptedDayOfWeek)
                        {
                            var workingDayMessage = ve.Env.T($"Торги могут проводиться только со вторника по пятницу").Text;
                            ve.Env.AddError(tbRev.flDateTime.FieldName, workingDayMessage);
                        }
                        if (!isAcceptedTime)
                        {
                            var workingDayMessage = ve.Env.T($"Торги могут проводиться только с 10:00 до 15:00").Text;
                            ve.Env.AddError(tbRev.flDateTime.FieldName, workingDayMessage);
                        }

                        var tbMrp = new TbMrp();
                        tbMrp.AddFilter(tbMrp.flYear, tbRev.flDateTime.GetVal(ve.Env).Year);
                        var hasMrp = tbMrp.Count(ve.Env.QueryExecuter) > 0;

                        if (!hasMrp)
                        {
                            var yearMrpMessage = ve.Env.T("В базе не найдено значение мрп на @@@year год").Text;
                            yearMrpMessage = yearMrpMessage.Replace("@@@year", tbRev.flDateTime.GetVal(ve.Env).Year.ToString());
                            ve.Env.AddError(tbRev.flDateTime.FieldName, yearMrpMessage);
                        }

                        if (trade.Method != RefLandObjectTradeForms.Values.ContestRent)
                        {
                            tbRev.flType.Validate(ve.Env);
                        }

                    }
                })
                .OnProcessing(pe => {
                    var tbRev = new TbLandObjectsTradesRevisions();
                    var trade = pe.Model;
                    if (trade.Method != RefLandObjectTradeForms.Values.ContestRent)
                    {
                        pe.Model.Type = tbRev.flType.GetVal(pe.Env);
                    }
                    pe.Model.DateTime = tbRev.flDateTime.GetVal(pe.Env);
                    pe.Model.Note = tbRev.flNote.GetVal(pe.Env);

                    var tbLandObjects = new TbLandObjects();
                    var ObjectCost = tbLandObjects.AddFilter(t => t.flId, pe.Model.ObjectId).SelectScalar(t => t.flCost, pe.Env.QueryExecuter).Value;
                    pe.Model.InitialCost = ObjectCost;
                }))
                //.Step("Публикации в СМИ", step => step
                //.Enabled(env => env.Args.OrderType.In(LandObjectTradesOrderTypes.Create, LandObjectTradesOrderTypes.Edit))
                //.OnRendering(re => {
                //    var trade = re.Model;
                //    var tbPublications = new TbLandObjectsTradesPublications();

                //    var publKz = new LandObjectTradePublicationsModel();
                //    var publRu = new LandObjectTradePublicationsModel();
                //    if (trade.Publications.Count > 0)
                //    {
                //        publKz = trade.Publications.Where(x => x.NewspaperLang == "110000").First();
                //        publRu = trade.Publications.Where(x => x.NewspaperLang == "100000").First();
                //    }

                //    var groupboxKz = new Accordion(re.Env.T("Публикация на казахском языке"));
                //    re.Panel.AddComponent(groupboxKz);
                //    groupboxKz.AddComponent(new Referencebox($"{tbPublications.flNewspaperId.FieldName}Kz", RefNewspapers.GetReference(re.Env.QueryExecuter, "kz"), re.Env.T("Газета"), (publKz.NewspaperId != null ? new[] { publKz.NewspaperId } : null)) { CssClass = "form-control mb-3" });
                //    tbPublications.flNumber.RenderCustom(groupboxKz, re.Env, publKz.Number, fieldAlias: $"{tbPublications.flNumber.FieldName}Kz");
                //    tbPublications.flDate.RenderCustom(groupboxKz, re.Env, (publKz.Date != null ? publKz.Date : DateTime.Now), fieldAlias: $"{tbPublications.flDate.FieldName}Kz");
                //    tbPublications.flText.RenderCustom(groupboxKz, re.Env, publKz.Text, fieldAlias: $"{tbPublications.flText.FieldName}Kz");

                //    var groupboxRu = new Accordion(re.Env.T("Публикация на русском языке"));
                //    re.Panel.AddComponent(groupboxRu);
                //    groupboxRu.AddComponent(new Referencebox($"{tbPublications.flNewspaperId.FieldName}Ru", RefNewspapers.GetReference(re.Env.QueryExecuter, "ru"), re.Env.T("Газета"), (publRu.NewspaperId != null ? new[] { publRu.NewspaperId } : null)) { CssClass = "form-control mb-3" });
                //    tbPublications.flNumber.RenderCustom(groupboxRu, re.Env, publRu.Number, fieldAlias: $"{tbPublications.flNumber.FieldName}Ru");
                //    tbPublications.flDate.RenderCustom(groupboxRu, re.Env, (publRu.Date != null ? publRu.Date : DateTime.Now), fieldAlias: $"{tbPublications.flDate.FieldName}Ru");
                //    tbPublications.flText.RenderCustom(groupboxRu, re.Env, publRu.Text, fieldAlias: $"{tbPublications.flText.FieldName}Ru");
                //})
                //.OnValidating(ve => {
                //    var trade = ve.Model;
                //    var tbPublications = new TbLandObjectsTradesPublications();

                //    var checkFields = new Field[] {
                //        tbPublications.flNewspaperId,
                //        tbPublications.flNumber,
                //        tbPublications.flDate,
                //        tbPublications.flText
                //    };
                //    checkFields.Each(f => f.Validate(ve.Env, f.FieldName + "Ru"));
                //    checkFields.Each(f => f.Validate(ve.Env, f.FieldName + "Kz"));
                //})
                //.OnProcessing(pe => {
                //    var trade = pe.Model;
                //    trade.Publications.Clear();
                //    var tbPublications = new TbLandObjectsTradesPublications();

                //    var langs = new[] { "Kz", "Ru" };

                //    langs.Each(lang => {
                //        var publ = new LandObjectTradePublicationsModel();
                //        publ.NewspaperLang = (lang == "Kz" ? "110000" : "100000");
                //        publ.NewspaperId = tbPublications.flNewspaperId.GetVal(pe.Env, tbPublications.flNewspaperId.FieldName + lang);
                //        publ.Number = tbPublications.flNumber.GetVal(pe.Env, tbPublications.flNumber.FieldName + lang);
                //        publ.Date = tbPublications.flDate.GetVal(pe.Env, tbPublications.flDate.FieldName + lang);
                //        publ.Text = tbPublications.flText.GetVal(pe.Env, tbPublications.flText.FieldName + lang);
                //        trade.Publications.Add(publ);
                //    });

                //}))

            #region Электронный аукцион

                .Step("Понижение цены", step => step
                .Enabled(env => env.Args.OrderType.In(LandObjectTradesOrderTypes.Create, LandObjectTradesOrderTypes.Edit) && env.Model.Method == RefLandObjectTradeForms.Values.LandTradesPriceDown)
                .OnRendering(re => {
                    var tbRev = new TbLandObjectsTradesRevisions();
                    var trade = re.Model;

                    trade.AdditionalCondition = RefConditionParticipants.Values.NotAllowed;
                    var canChageAdditionalCondition = false;

                    var landObjectTrades = new TbLandObjectsTrades()
                        .AddFilter(t => t.flObjectId, trade.ObjectId)
                        .AddFilterNot(t => t.flId, trade.Id);
                    landObjectTrades.OrderBy = new[] { new OrderField(landObjectTrades.flDateTime, OrderType.Desc) };
                    var objectTrades = landObjectTrades.Select(re.Env.QueryExecuter).Select().AsEnumerable().ToList();
                    var canceledTrades = new string[] { RefLandObjectTradeStatuses.NotHeld, RefLandObjectTradeStatuses.Cancel, RefLandObjectTradeStatuses.CanceledEarlier };
                    if (objectTrades.Count >= 2)
                    {
                        if (canceledTrades.Contains(landObjectTrades.flStatus.GetVal(objectTrades[0])) && canceledTrades.Contains(landObjectTrades.flStatus.GetVal(objectTrades[1])))
                        {
                            canChageAdditionalCondition = true;
                        }
                    }
                    trade.CanChageAdditionalCondition = canChageAdditionalCondition;

                    tbRev.flAdditionalCondition.RenderCustom(re.Panel, re.Env, trade.AdditionalCondition, readOnly: !canChageAdditionalCondition);
                    tbRev.flUpFactor.RenderCustom(re.Panel, re.Env, trade.UpFactor);

                })
                .OnValidating(ve => {
                    var tbRev = new TbLandObjectsTradesRevisions();
                    var trade = ve.Model;
                    var obj = new TbLandObjects().AddFilter(t => t.flId, trade.ObjectId)
                       .GetObjectModelFirst(ve.Env.QueryExecuter, null);

                    tbRev.flUpFactor.Validate(ve.Env);

                    if (trade.CanChageAdditionalCondition)
                    {
                        tbRev.flAdditionalCondition.Validate(ve.Env);
                    }

                    if (ve.Env.IsValid)
                    {
                        var upFactor = tbRev.flUpFactor.GetValOrNull(ve.Env);

                        if (upFactor.HasValue && upFactor.Value < 100)
                        {
                            ve.Env.AddError(tbRev.flUpFactor.FieldName, ve.Env.T($"{tbRev.flUpFactor.Text} не может быть ниже 100"));
                        }
                    }
                })
                .OnProcessing(pe => {
                    var tbRev = new TbLandObjectsTradesRevisions();
                    var trade = pe.Model;
                    if (trade.CanChageAdditionalCondition)
                    {
                        pe.Model.AdditionalCondition = tbRev.flAdditionalCondition.GetVal(pe.Env);
                    }
                    pe.Model.UpFactor = tbRev.flUpFactor.GetVal(pe.Env);
                }))
                .Step("Продажа", step => step
                .Enabled(env => env.Args.OrderType.In(LandObjectTradesOrderTypes.Create, LandObjectTradesOrderTypes.Edit) && env.Model.Type == RefLandObjectTradeTypes.PrivateOwnership)
                .OnRendering(re => {
                    var tbRev = new TbLandObjectsTradesRevisions();
                    var trade = re.Model;

                    tbRev.flLandLeaseAvailable.RenderCustom(re.Panel, re.Env, trade.LandLeaseAvailable);
                    tbRev.flLandLeaseLength.RenderCustom(re.Panel, re.Env, trade.LandLeaseLength);

                    var tbLandObjects = new TbLandObjects();
                    var ObjectCost = tbLandObjects.AddFilter(t => t.flId, re.Model.ObjectId).SelectScalar(t => t.flCost, re.Env.QueryExecuter).Value;
                    tbLandObjects.flCost.RenderCustom(re.Panel, re.Env, ObjectCost, readOnly: true);
                    tbRev.flInitialCost.RenderCustom(re.Panel, re.Env, trade.InitialCost);


                    if (trade.Method == RefLandObjectTradeForms.Values.LandTradesPriceDown)
                    {
                        tbRev.flCostMin.RenderCustom(re.Panel, re.Env, trade.CostMin);
                    }

                })
                .OnValidating(ve => {
                    var tbRev = new TbLandObjectsTradesRevisions();
                    var tbLandObjects = new TbLandObjects();
                    var checkFields = new Field[] {
                        tbRev.flInitialCost,
                        tbRev.flLandLeaseAvailable
                    };
                    checkFields.Each(f => f.Validate(ve.Env));

                    if (ve.Env.IsValid)
                    {
                        if (tbRev.flLandLeaseAvailable.GetVal(ve.Env) == RefLandLeaseAvailable.Yes)
                        {
                            tbRev.flLandLeaseLength.Required().Validate(ve.Env);
                        }

                        var ObjectCost = new TbLandObjects().AddFilter(t => t.flId, ve.Model.ObjectId).SelectScalar(t => t.flCost, ve.Env.QueryExecuter).Value;

                        if (ve.Model.Method == RefLandObjectTradeForms.Values.LandTradesPriceDown)
                        {
                            tbRev.flCostMin.Validate(ve.Env);


                            if (tbRev.flCostMin.GetValOrNull(ve.Env).HasValue)
                            {
                                if (tbRev.flCostMin.GetVal(ve.Env) < ObjectCost)
                                {
                                    ve.Env.AddError(tbRev.flCostMin.FieldName, ve.Env.T($"{tbRev.flCostMin.Text} не может быть ниже оценочной стоимости"));
                                }
                            }
                        }

                        if (tbRev.flInitialCost.GetValOrNull(ve.Env).HasValue)
                        {
                            if (tbRev.flInitialCost.GetVal(ve.Env) < ObjectCost)
                            {
                                ve.Env.AddError(tbRev.flInitialCost.FieldName, ve.Env.T($"{tbRev.flInitialCost.Text} не может быть ниже оценочной стоимости"));
                            }
                        }
                    }
                })
                .OnProcessing(pe => {
                    var tbRev = new TbLandObjectsTradesRevisions();

                    //var ObjectCost = new TbLandObjects().AddFilter(t => t.flId, pe.Model.ObjectId).SelectScalar(t => t.flCost, pe.Env.QueryExecuter).Value;
                    //pe.Model.InitialCost = ObjectCost;
                    pe.Model.LandLeaseAvailable = tbRev.flLandLeaseAvailable.GetVal(pe.Env);
                    pe.Model.LandLeaseLength = tbRev.flLandLeaseLength.GetValOrNull(pe.Env);
                    pe.Model.InitialCost = tbRev.flInitialCost.GetVal(pe.Env);
                    if (pe.Model.Method == RefLandObjectTradeForms.Values.LandTradesPriceDown)
                    {
                        pe.Model.CostMin = tbRev.flCostMin.GetVal(pe.Env);
                    }
                }))
                .Step("Аренда", step => step
                .Enabled(env => env.Args.OrderType.In(LandObjectTradesOrderTypes.Create, LandObjectTradesOrderTypes.Edit) && env.Model.Type == RefLandObjectTradeTypes.RentOwnership)
                .OnRendering(re => {
                    var tbRev = new TbLandObjectsTradesRevisions();
                    var trade = re.Model;

                    tbRev.flOwnershipMonths.RenderCustom(re.Panel, re.Env, trade.OwnershipMonths);

                })
                .OnValidating(ve => {
                    var tbRev = new TbLandObjectsTradesRevisions();
                    var checkFields = new Field[] {
                            tbRev.flOwnershipMonths.Required()
                        };
                    checkFields.Each(f => f.Validate(ve.Env));

                    if (ve.Env.IsValid)
                    {
                        if (tbRev.flOwnershipMonths.GetVal(ve.Env) == 0)
                        {
                            ve.Env.AddError(tbRev.flOwnershipMonths.FieldName, ve.Env.T("Срок не может быть равен 0"));
                        }

                        if (tbRev.flOwnershipMonths.GetVal(ve.Env) == 12 * 49)
                        {
                            ve.Env.AddError(tbRev.flOwnershipMonths.FieldName, ve.Env.T("Срок не может превышать 49 лет"));
                        }

                        var ObjectType = new TbLandObjects().AddFilter(t => t.flId, ve.Model.ObjectId).SelectScalar(t => t.flType, ve.Env.QueryExecuter);
                        if (ObjectType == nameof(LandObjectTypes.Сommercial) && tbRev.flOwnershipMonths.GetVal(ve.Env) % 12.0m > 0)
                        {
                            ve.Env.AddError(tbRev.flOwnershipMonths.FieldName, ve.Env.T("Срок должен быть кратен 12 месяцам"));
                        }
                    }
                })
                .OnProcessing(pe => {
                    var tbRev = new TbLandObjectsTradesRevisions();
                    pe.Model.OwnershipMonths = tbRev.flOwnershipMonths.GetVal(pe.Env);
                }))

                .Step("Калькулируемые данные", step => step
                .Enabled(env => env.Args.OrderType.In(LandObjectTradesOrderTypes.Create, LandObjectTradesOrderTypes.Edit) && env.Model.Form == RefLandObjectTradeForms.Values.EAuction)
                .OnRendering(re => {

                    var trade = re.Model;
                    var tbRev = new TbLandObjectsTradesRevisions();
                    var tbLandObjects = new TbLandObjects();

                    var tbMrp = new TbMrp();
                    tbMrp.AddFilter(tbMrp.flYear, trade.DateTime.Value.Year);
                    var mrp = Convert.ToDecimal(tbMrp.SelectScalar(tbMrp.flMrp, re.Env.QueryExecuter));
                    var mrp50 = 50 * mrp;
                    var mrp30000 = 30000 * mrp;

                    var ObjectCost = tbLandObjects.AddFilter(t => t.flId, re.Model.ObjectId).SelectScalar(t => t.flCost, re.Env.QueryExecuter).Value;
                    tbLandObjects.flCost.RenderCustom(re.Panel, re.Env, ObjectCost, readOnly: true);

                    //trade.InitialCost = ObjectCost;

                    if (trade.Type == RefLandObjectTradeTypes.PrivateOwnership)
                    {
                        if (trade.Method == RefLandObjectTradeForms.Values.LandTradesPriceUp)
                        {

                            trade.CostStart = trade.InitialCost;
                            trade.CostMin = trade.CostStart;
                            trade.Deposit = (trade.InitialCost / 100 * 15 < mrp50 ? mrp50 : trade.InitialCost / 100 * 15 > mrp30000 ? mrp30000 : trade.InitialCost / 100 * 15);
                            trade.CostMinCondition = RefCostMinCondition.Values.WithOutCostMin;

                            tbRev.flInitialCost.RenderCustom(re.Panel, re.Env, trade.InitialCost, readOnly: true, description: new Label(re.Env.T("(Равна стартовой цене)")));

                            tbRev.flCostStart.RenderCustom(re.Panel, re.Env, trade.CostStart, readOnly: true);

                            tbRev.flDeposit.RenderCustom(re.Panel, re.Env, trade.Deposit, readOnly: true,
                                description: new Label($"({re.Env.T("Начальная цена (оценочная стоимость), тг.")} * 15% ({re.Env.T("минимум")} 50 {re.Env.T("мрп")} {mrp50} {re.Env.T("тг.")}), {re.Env.T("максимум")} 30000 {re.Env.T("мрп")} {mrp30000} {re.Env.T("тг.")}), 1 {re.Env.T("мрп")} = {mrp} {re.Env.T("тг.")}))")
                            );
                        } else
                        {
                            trade.CostStart = trade.InitialCost * trade.UpFactor;
                            trade.Deposit = (trade.InitialCost / 100 * 15 < mrp50 ? mrp50 : trade.InitialCost / 100 * 15 > mrp30000 ? mrp30000 : trade.InitialCost / 100 * 15);
                            trade.CostMinCondition = RefCostMinCondition.Values.WithCostMin;

                            trade.InitialCost = trade.CostStart;
                            tbRev.flInitialCost.RenderCustom(re.Panel, re.Env, trade.InitialCost, readOnly: true, description: new Label(re.Env.T("(Равна стартовой цене)")));

                            tbRev.flCostStart.RenderCustom(re.Panel, re.Env, trade.CostStart, readOnly: true,
                                description: new Label($"({tbRev.flInitialCost.Text} * {tbRev.flUpFactor.Text} ({trade.UpFactor}))")
                            );

                            tbRev.flDeposit.RenderCustom(re.Panel, re.Env, trade.Deposit, readOnly: true,
                                description: new Label($"({re.Env.T("Минимальная цена (оценочная стоимость), тг.")} * 15% ({re.Env.T("минимум")} 50 {re.Env.T("мрп")} {mrp50} {re.Env.T("тг.")}), {re.Env.T("максимум")} 30000 {re.Env.T("мрп")} {mrp30000} {re.Env.T("тг.")}), 1 {re.Env.T("мрп")} = {mrp} {re.Env.T("тг.")}))")
                            );

                            tbRev.flCostMinCondition.RenderCustom(re.Panel, re.Env, trade.CostMinCondition, readOnly: true);
                            tbRev.flCostMin.RenderCustom(re.Panel, re.Env, trade.CostMin, readOnly: true);
                        }
                    } else
                    {

                        if (trade.Method == RefLandObjectTradeForms.Values.LandTradesPriceUp)
                        {

                            var costStart = 0m;
                            var costStartText = string.Empty;
                            var years = trade.OwnershipMonths.Value / 12;

                            var yearsText = years.ToString();

                            if (years <= 10) {
                                costStart = (trade.InitialCost.Value / 100 * 2) * years;
                                costStartText = $"({tbLandObjects.flCost.Text} * 2% * {yearsText}(срок аренды в годах))";
                            }
                            else if (years <= 30) {
                                var yearDifference = years - 10;
                                costStart = (trade.InitialCost.Value / 100 * 20) + (trade.InitialCost.Value / 100 * 1.5m * yearDifference);
                                costStartText = $"({tbLandObjects.flCost.Text} * 20% + 1,5% {re.Env.T("за каждый год свыше")} 10 {re.Env.T("лет")} ({re.Env.T("всего")} {yearsText}))";
                            }
                            else if (years <= 49) {
                                var yearDifference = years - 30;
                                costStart = (trade.InitialCost.Value / 100 * 50) + (trade.InitialCost.Value / 100 * 1 * yearDifference);
                                costStartText = $"({tbLandObjects.flCost.Text} * 50% + 1% {re.Env.T("за каждый год свыше")} 30 {re.Env.T("лет")} ({re.Env.T("всего")} {yearsText}))";
                            }
                            trade.CostStart = costStart;


                            trade.CostMin = trade.CostStart;
                            trade.InitialCost = trade.CostStart;

                            trade.Deposit = (trade.InitialCost / 100 * 15 < mrp50 ? mrp50 : trade.InitialCost / 100 * 15 > mrp30000 ? mrp30000 : trade.InitialCost / 100 * 15);

                            trade.CostMinCondition = RefCostMinCondition.Values.WithOutCostMin;

                            tbRev.flInitialCost.RenderCustom(re.Panel, re.Env, trade.InitialCost, readOnly: true, description: new Label("(Равна стартовой цене)"));

                            tbRev.flCostStart.RenderCustom(re.Panel, re.Env, trade.CostStart, readOnly: true,
                                description: new Label(costStartText)
                            );

                            tbRev.flDeposit.RenderCustom(re.Panel, re.Env, trade.Deposit, readOnly: true,
                                description: new Label($"({re.Env.T("Начальная цена (оценочная стоимость), тг.")} * 15% ({re.Env.T("минимум")} 50 {re.Env.T("мрп")} {mrp50} {re.Env.T("тг.")}), {re.Env.T("максимум")} 30000 {re.Env.T("мрп")} {mrp30000} {re.Env.T("тг.")}), 1 {re.Env.T("мрп")} = {mrp} {re.Env.T("тг.")}))")
                            );
                        } else
                        {
                            var costMin = 0m;
                            var costMinText = string.Empty;
                            var years = trade.OwnershipMonths.Value / 12;

                            var yearsText = years.ToString();

                            if (years <= 10)
                            {
                                costMin = (trade.InitialCost.Value / 100 * 2) * years;
                                costMinText = $"({tbLandObjects.flCost.Text} * 2% * {yearsText}(срок аренды в годах))";
                            } else if (years <= 30)
                            {
                                var yearDifference = years - 10;
                                costMin = (trade.InitialCost.Value / 100 * 20) + (trade.InitialCost.Value / 100 * 1.5m * yearDifference);
                                costMinText = $"({tbLandObjects.flCost.Text} * 20% + 1,5% {re.Env.T("за каждый год свыше")} 10 {re.Env.T("лет")} ({re.Env.T("всего")} {yearsText}))";
                            } else if (years <= 49)
                            {
                                var yearDifference = years - 30;
                                costMin = (trade.InitialCost.Value / 100 * 50) + (trade.InitialCost.Value / 100 * 1 * yearDifference);
                                costMinText = $"({tbLandObjects.flCost.Text} * 50% + 1% {re.Env.T("за каждый год свыше")} 30 {re.Env.T("лет")} ({re.Env.T("всего")} {yearsText}))";
                            }

                            var costStart = costMin * trade.UpFactor;

                            trade.InitialCost = costStart;
                            trade.CostStart = costStart;
                            trade.Deposit = (costMin / 100 * 15 < mrp50 ? mrp50 : costMin / 100 * 15 > mrp30000 ? mrp30000 : costMin / 100 * 15);
                            trade.CostMinCondition = RefCostMinCondition.Values.WithCostMin;
                            trade.CostMin = costMin;

                            tbRev.flInitialCost.RenderCustom(re.Panel, re.Env, trade.InitialCost, readOnly: true, description: new Label(re.Env.T("(Равна стартовой цене)")));

                            tbRev.flCostStart.RenderCustom(re.Panel, re.Env, trade.CostStart, readOnly: true,
                                description: new Label($"({tbRev.flCostMin.Text} * {tbRev.flUpFactor.Text} ({trade.UpFactor}))")
                            );

                            tbRev.flDeposit.RenderCustom(re.Panel, re.Env, trade.Deposit, readOnly: true,
                                description: new Label($"({re.Env.T("Минимальная цена (оценочная стоимость), тг.")} * 15% ({re.Env.T("минимум")} 50 {re.Env.T("мрп")} {mrp50} {re.Env.T("тг.")}), {re.Env.T("максимум")} 30000 {re.Env.T("мрп")} {mrp30000} {re.Env.T("тг.")}), 1 {re.Env.T("мрп")} = {mrp} {re.Env.T("тг.")}))")
                            );

                            tbRev.flCostMinCondition.RenderCustom(re.Panel, re.Env, trade.CostMinCondition, readOnly: true);
                            tbRev.flCostMin.RenderCustom(re.Panel, re.Env, trade.CostMin, readOnly: true,
                                description: new Label(costMinText)
                            );
                        }
                    }

                })
                .OnValidating(ve => {

                })
                .OnProcessing(pe => {

                }))

            #endregion

            #region Конкурс

                .Step("Условия торгов", step => step
                .Enabled(env => env.Args.OrderType.In(LandObjectTradesOrderTypes.Create, LandObjectTradesOrderTypes.Edit) && env.Model.Form == RefLandObjectTradeForms.Values.Contest)
                .OnRendering(re => {
                    var tbRev = new TbLandObjectsTradesRevisions();
                    var trade = re.Model;

                    trade.AdditionalCondition = RefConditionParticipants.Values.NotAllowed;
                    var canChageAdditionalCondition = false;

                    var landObjectTrades = new TbLandObjectsTrades()
                        .AddFilter(t => t.flObjectId, trade.ObjectId)
                        .AddFilterNot(t => t.flId, trade.Id);
                    landObjectTrades.OrderBy = new[] { new OrderField(landObjectTrades.flDateTime, OrderType.Desc) };
                    var objectTrades = landObjectTrades.Select(re.Env.QueryExecuter).Select().AsEnumerable().ToList();
                    var canceledTrades = new string[] { RefLandObjectTradeStatuses.NotHeld, RefLandObjectTradeStatuses.Cancel, RefLandObjectTradeStatuses.CanceledEarlier };
                    if (objectTrades.Count >= 2)
                    {
                        if (canceledTrades.Contains(landObjectTrades.flStatus.GetVal(objectTrades[0])) && canceledTrades.Contains(landObjectTrades.flStatus.GetVal(objectTrades[1])))
                        {
                            canChageAdditionalCondition = true;
                        }
                    }
                    trade.CanChageAdditionalCondition = canChageAdditionalCondition;

                    if (trade.RequiredDocuments.Count == 0)
                    {
                        trade.RequiredDocuments = new TradeRequiredDocuments[] {
                            new TradeRequiredDocuments("docs_business_plan", re.Env.T("1) бизнес-план")),
                            new TradeRequiredDocuments("docs_preparation_and_development_obligation", re.Env.T("2) письменное обязательство по составлению и освоению проекта внутрихозяйственного землеустройства, выполнению мероприятий по развитию сельскохозяйственной инфраструктуры")),
                            new TradeRequiredDocuments("docs_fulfill_requirements_obligation", re.Env.T("3) письменное обязательство по исполнению требований земельного законодательства Республики Казахстан, включая правила рационального использования земель сельскохозяйственного назначения, выполнению индикативных показателей по диверсификации структуры посевных площадей сельскохозяйственных культур в соответствии со специализацией региона, соблюдению научно обоснованных агротехнологий, фитосанитарных и карантинных требований")),
                        }.ToList();
                    }

                    tbRev.flAdditionalCondition.RenderCustom(re.Panel, re.Env, trade.AdditionalCondition, readOnly: !canChageAdditionalCondition);
                    tbRev.flOwnershipMonths.RenderCustom(re.Panel, re.Env, trade.OwnershipMonths);

                    tbRev.flParticipantConditions.RenderCustom(re.Panel, re.Env, trade.ParticipantConditions);
                    tbRev.flRequiredDocuments.RenderCustomT(re.Panel, re.Env, trade.RequiredDocuments, customEditor: new TextListEditor(tbRev.flRequiredDocuments.FieldName, re.Env) { CaptionText = re.Env.T("Требуемые документы") });
                    tbRev.flCommissionMembers.RenderCustom(re.Panel, re.Env, trade.CommissionMembers);

                })
                .OnValidating(ve => {
                    var tbRev = new TbLandObjectsTradesRevisions();
                    var trade = ve.Model;
                    var checkFields = new Field[] {
                            tbRev.flOwnershipMonths
                        };
                    checkFields.Each(f => f.Validate(ve.Env));

                    if (trade.CanChageAdditionalCondition)
                    {
                        tbRev.flAdditionalCondition.Validate(ve.Env);
                    }

                    if (tbRev.flOwnershipMonths.GetValOrNull(ve.Env) != null && tbRev.flOwnershipMonths.GetVal(ve.Env) == 0)
                    {
                        ve.Env.AddError(tbRev.flOwnershipMonths.FieldName, ve.Env.T("Срок не может быть равен 0"));
                    }

                    var ParticipantConditions = JsonConvert.DeserializeObject<List<TradeParticipantConditions>>(ve.Env.FormCollection[tbRev.flParticipantConditions.FieldName] + string.Empty);
                    var RequiredDocuments = JsonConvert.DeserializeObject<List<TradeRequiredDocuments>>(ve.Env.FormCollection[tbRev.flRequiredDocuments.FieldName] + string.Empty);
                    var CommissionMembers = JsonConvert.DeserializeObject<List<TradeComissionMembers>>(ve.Env.FormCollection[tbRev.flCommissionMembers.FieldName] + string.Empty);

                    if (CommissionMembers == null || CommissionMembers.Count < 9)
                    {
                        ve.Env.AddError(tbRev.flCommissionMembers.FieldName, ve.Env.T("Требуется указать как минимум 9 членов комиссии"));
                    }
                    if (RequiredDocuments == null || RequiredDocuments.Count < 3)
                    {
                        ve.Env.AddError(tbRev.flRequiredDocuments.FieldName, ve.Env.T("Требуется указать как минимум 3 документа (предложение и 2 обязательства)"));
                    }

                })
                .OnProcessing(pe => {
                    var tbRev = new TbLandObjectsTradesRevisions();
                    pe.Model.OwnershipMonths = tbRev.flOwnershipMonths.GetVal(pe.Env);

                    if (pe.Model.CanChageAdditionalCondition)
                    {
                        pe.Model.AdditionalCondition = tbRev.flAdditionalCondition.GetVal(pe.Env);
                    }

                    pe.Model.ParticipantConditions = JsonConvert.DeserializeObject<List<TradeParticipantConditions>>(pe.Env.FormCollection[tbRev.flParticipantConditions.FieldName] + string.Empty);
                    pe.Model.ParticipantConditions.ForEach(partCond => {
                        if (string.IsNullOrEmpty(partCond.Id))
                        {
                            partCond.Id = Guid.NewGuid().ToString("N");
                        }
                    });

                    pe.Model.RequiredDocuments = JsonConvert.DeserializeObject<List<TradeRequiredDocuments>>(pe.Env.FormCollection[tbRev.flRequiredDocuments.FieldName] + string.Empty);
                    pe.Model.RequiredDocuments.ForEach(reqDoc => {
                        if (string.IsNullOrEmpty(reqDoc.Id))
                        {
                            reqDoc.Id = Guid.NewGuid().ToString("N");
                        }
                    });

                    pe.Model.CommissionMembers = JsonConvert.DeserializeObject<List<TradeComissionMembers>>(pe.Env.FormCollection[tbRev.flCommissionMembers.FieldName] + string.Empty);
                    pe.Model.CommissionMembers.ForEach(commMem => {
                        if (string.IsNullOrEmpty(commMem.MemberId))
                        {
                            commMem.MemberId = Guid.NewGuid().ToString("N");
                        }
                    });

                }))

                #endregion

                .Step("Реквизиты", step => step
                .Enabled(env => env.Args.OrderType.In(LandObjectTradesOrderTypes.Create, LandObjectTradesOrderTypes.Edit) && env.Model.InitialCost > 0)
                .OnRendering(re => {
                    var tbRev = new TbLandObjectsTradesRevisions();
                    var trade = re.Model;

                    tbRev.flTaxAuthorityBin.RenderCustom(re.Panel, re.Env, trade.TaxAuthorityBin);
                    tbRev.flBik.RenderCustom(re.Panel, re.Env, "KKMFKZ2A", readOnly: true);
                    tbRev.flIik.RenderCustom(re.Panel, re.Env, "KZ24070105KSN0000000", readOnly: true);
                    tbRev.flKnp.RenderCustom(re.Panel, re.Env, trade.Knp);
                    tbRev.flKbk.RenderCustom(re.Panel, re.Env, trade.Kbk);
                    tbRev.flKbe.RenderCustom(re.Panel, re.Env, trade.Kbe);

                })
                .OnValidating(ve => {
                    var tbRev = new TbLandObjectsTradesRevisions();
                    var checkFields = new Field[] {
                            tbRev.flTaxAuthorityBin.Required().NotNull(),
                            tbRev.flKnp.Required().NotNull(),
                            tbRev.flKbk.Required().NotNull(),
                            tbRev.flKbe.Required().NotNull(),
                        };
                    checkFields.Each(f => f.Validate(ve.Env));

                    if (ve.Env.IsValid)
                    {
                        if (tbRev.flTaxAuthorityBin.GetVal(ve.Env).Length != 12)
                        {
                            ve.Env.AddError(tbRev.flTaxAuthorityBin.FieldName, ve.Env.T("Бин состоит из 12 чисел"));
                        }
                    }
                })
                .OnProcessing(pe => {
                    var tbRev = new TbLandObjectsTradesRevisions();
                    pe.Model.TaxAuthorityBin = tbRev.flTaxAuthorityBin.GetVal(pe.Env);
                    pe.Model.Bik = "KKMFKZ2A";
                    pe.Model.Iik = "KZ24070105KSN0000000";
                    pe.Model.Knp = tbRev.flKnp.GetVal(pe.Env);
                    pe.Model.Kbk = tbRev.flKbk.GetVal(pe.Env);
                    pe.Model.Kbe = tbRev.flKbe.GetVal(pe.Env);
                }))
                ;
        }


        public override LandObjectTradeModel GetModel(ActionEnv<LandObjectTradeOrderQueryArgs> env, GetModelFrom from, LandObjectTradesOrderTypes orderType, ITransaction transaction) {
            var now = env.QueryExecuter.GetDateTime(NpGlobal.DbKeys.DbYodaGr);
            var tradeDate = new DateTime(now.Year, now.Month, now.Day, 10, 0, 0).AddWorkdays(15, env.QueryExecuter);

            var model = from switch
            {
                GetModelFrom.Empty => new LandObjectTradeModel() {
                    CompetentOrgBin = env.Args.OwnerXin,
                    DateTime = tradeDate,
                    Status = RefLandObjectTradeStatuses.Wait.ToString(),
                    ObjectId = env.Args.ObjectId
                },
                GetModelFrom.Entity =>
                    new TbLandObjectsTrades()
                        .AddFilter(t => t.flId, env.Args.Id.Value)
                        .GetTradeModelFirst(env.QueryExecuter, transaction),
                GetModelFrom.Revision =>
                    new TbLandObjectsTradesRevisions()
                        .AddFilter(t => t.flRevisionId, env.Args.RevisionId.Value)
                        .GetTradeModelFirst(env.QueryExecuter, transaction),
                _ => throw new NotImplementedException()
            };

            switch (orderType) {
                case LandObjectTradesOrderTypes.Create:
                    model.Status = RefLandObjectTradeStatuses.Wait.ToString();
                    break;
                case LandObjectTradesOrderTypes.Cancel:
                    model.Status = RefLandObjectTradeStatuses.CanceledEarlier.ToString();
                    break;
                default:
                    // Status is not changing
                    break;
            }

            return model;
        }

        public override LandObjectTradesOrderTypes GetOrderType(ActionEnv<LandObjectTradeOrderQueryArgs> env, GetOrderModelFrom orderModelFrom) {
            return orderModelFrom switch
            {
                GetOrderModelFrom.Empty => LandObjectTradesOrderTypes.Create,
                _ => env.Args.OrderType switch
                {
                    LandObjectTradesOrderTypeActions.Cancel => LandObjectTradesOrderTypes.Cancel,
                    LandObjectTradesOrderTypeActions.Edit => LandObjectTradesOrderTypes.Edit,
                    LandObjectTradesOrderTypeActions.Transfer => LandObjectTradesOrderTypes.Transfer,
                    _ => throw new ArgumentException($"OrderType: {env.Args.OrderType}")
                }
            };
        }

        public override string GetWizardCancelRedirectUrl(ActionEnv<LandObjectTradeOrderQueryArgs> env, LandObjectTradesOrderTypes orderType, GetModelFrom modelFrom) {
            var urlHelper = env.RequestContext.GetUrlHelper();
            //TODO
            return modelFrom switch
            {
                GetModelFrom.Empty => urlHelper.YodaAction(ModuleName, nameof(MnuLandObjectTradeOrderBase)),
                GetModelFrom.Entity => urlHelper.YodaAction(ModuleName, nameof(MnuLandObjectTradeOrderBase), new LandObjectTradeOrderQueryArgs { Id = env.Args.Id.Value, MenuAction = "view" }),
                GetModelFrom.Revision => urlHelper.YodaAction(ModuleName, MenuName, new LandObjectTradeOrderQueryArgs { RevisionId = env.Args.RevisionId, MenuAction = Actions.ViewOrder }),
                _ => throw new NotImplementedException()
            };
        }

        public override ValidationResultBase IsCreateFromActionValid(ActionEnv<LandObjectTradeOrderQueryArgs> env) {
            if (env.Args.Id == null) {
                return new RedirectResult(new ArgumentException("Id"));
            }

            if (env.Args.OrderType == null) {
                return new RedirectResult(new ArgumentException("OrderType"));
            }

            if (!CanCreateOrder(env.Args.Id.Value, GetOrderType(env, GetOrderModelFrom.Entity), out var error, env)) {
                if (env.Args.Id.HasValue) {
                    return new RedirectResult(ModuleName, MnuLandObjectTradeView.MnuName, new MnuLandObjectTradeViewArgs() { tradeId = env.Args.Id.Value }, error);
                }
                else {
                    return new RedirectResult(ModuleName, "ActionError", null, error);
                }
            }

            return new OkResult();
        }

        public static bool TryGetCurrentInWorkOrderRevId(int id, out int revId, IQueryExecuter queryExecuter) {
            var ret = new TbLandObjectsTradesOrderResult()
                .AddFilterNotFinishedOrders()
                .JoinT("tbOrders", new TbLandObjectsTradesRevisions().AddFilter(t => t.flId, id), "tbRevs")
                .On((t1, t2) => new Join(t1.flSubjectId, t2.flRevisionId))
                .SelectScalar(t => t.L.flSubjectId, queryExecuter);

            if (ret == null) {
                revId = 0;
                return false;
            }

            revId = ret.Value;
            return true;
        }

        public static bool CanCreateOrder(int id, LandObjectTradesOrderTypes orderType, out string error, ActionEnv<LandObjectTradeOrderQueryArgs> env) {
            if (TryGetCurrentInWorkOrderRevId(id, out _, env.QueryExecuter)) {
                error = env.T("Для данного объекта имеется не исполненый приказ");
                return false;
            }

            var tradeResourceObject = LandObjectTradeModelHelper.GetTradeModel(id, env.QueryExecuter);


            bool checkTradeWaiting(out string err) {
                if (tradeResourceObject.Status != RefLandObjectTradeStatuses.Wait.ToString()) {
                    err = env.T("Операция невозможна, так как торг завершен");
                    return false;
                }
                err = null;
                return true;
            }

            switch (orderType) {
                case LandObjectTradesOrderTypes.Create:
                    error = env.T("Недопустимый тип приказа");
                    return false;
                case LandObjectTradesOrderTypes.Edit:
                    return checkTradeWaiting(out error);
                case LandObjectTradesOrderTypes.Cancel:
                    if (!checkTradeWaiting(out error)) {
                        return false;
                    }
                    error = null;
                    return true;
                case LandObjectTradesOrderTypes.Transfer:
                    if (!checkTradeWaiting(out error))
                    {
                        return false;
                    }
                    error = null;
                    return true;
                default:
                    throw new NotImplementedException($"TradesOrderTypes: {orderType}");
            }
        }

        public override void SaveModel(int revisionId, LandObjectTradeModel model, LandObjectTradesOrderTypes orderType, ActionEnv<LandObjectTradeOrderQueryArgs> env, ITransaction transaction, GetModelFrom modelFrom) {
            var tbTradeRevs = new TbLandObjectsTradesRevisions();

            if (modelFrom == GetModelFrom.Empty) {
                model.Id = new TbLandObjectsTrades().flId.GetNextId(env.QueryExecuter, transaction);
            }

            var updateOrInsert = modelFrom switch
            {
                GetModelFrom.Revision =>
                    tbTradeRevs
                        .AddFilter(t => t.flRevisionId, revisionId)
                        .Update(),

                _ => tbTradeRevs
                    .Insert()
            };
            model.RevisionId = revisionId;

            setModel(updateOrInsert, model);

            updateOrInsert
                .Execute(env.QueryExecuter, transaction);



            //var tbPublications = new TbLandObjectsTradesPublications();
            //tbPublications.AddFilter(tbPublications.flLandObjectTradeId, model.Id);
            //tbPublications.AddFilter(tbPublications.flLandObjectTradeRevisionId, model.RevisionId);
            //var removePubls = tbPublications.Remove();
            //removePubls.Execute(env.QueryExecuter, transaction);

            //model.Publications.Each(publ =>
            //{
            //    var insertPubls = tbPublications.Insert();
            //    var Id = new TbLandObjectsTradesPublications().flId.GetNextId(env.QueryExecuter, transaction);
            //    insertPubls.Set(t => t.flId, Id);
            //    insertPubls.Set(t => t.flLandObjectTradeId, model.Id);
            //    insertPubls.Set(t => t.flLandObjectTradeRevisionId, model.RevisionId);
            //    insertPubls.Set(t => t.flNewspaperLang, publ.NewspaperLang);
            //    insertPubls.Set(t => t.flNewspaperId, publ.NewspaperId);
            //    insertPubls.Set(t => t.flNumber, publ.Number);
            //    insertPubls.Set(t => t.flDate, publ.Date);
            //    insertPubls.Set(t => t.flText, publ.Text);
            //    insertPubls.Execute(env.QueryExecuter, transaction);
            //});
        }
        //private void checkIdGeneration(LandObjectTradeModel model) {
        //    //foreach (var condition in model.Conditions) {
        //    //    if (string.IsNullOrEmpty(condition.Id)) {
        //    //        condition.Id = Guid.NewGuid().ToString("N");
        //    //    }
        //    //}
        //    foreach (var commMem in model.CommissionMembers) {
        //        if (string.IsNullOrEmpty(commMem.MemberId)) {
        //            commMem.MemberId = Guid.NewGuid().ToString("N");
        //        }
        //    }
        //}

        private void setObjectStatus(LandObjectTradesOrderTypes orderType, LandObjectTradeModel model, ActionEnv<LandObjectTradeOrderQueryArgs> env, ITransaction transaction) {
            switch (orderType) {
                case LandObjectTradesOrderTypes.Edit:
                case LandObjectTradesOrderTypes.Transfer:
                    return;
            }
            switch (env.CurAction) {
                case Actions.Blank:
                case Actions.Cancel:
                case Actions.CreateFrom:
                case Actions.CreateNew:
                case Actions.Edit:
                case Actions.ReturnToImprove:
                case Actions.SendToExec:
                case Actions.ViewOrder:
                case Actions.Withdraw:
                    return;
            }
            var tbObjects = new TbLandObjects();
            tbObjects.AddFilter(t => t.flId, model.ObjectId);

            var rObject = tbObjects.SelectFirst(t => t.Fields.ToFieldsAliases(), env.QueryExecuter, transaction);

            var tbObjRev = new TbLandObjectsRevisions();
            var revObj = tbObjRev.AddFilter(t => t.flId, model.ObjectId).AddFilter(t => t.flRevisionId, rObject.GetVal(a => a.flRevisionId)).SelectFirst(t => t.Fields.ToFieldsAliases(), env.QueryExecuter, transaction);

            var objectBloc = LandObjectBlocks.ActiveFree.ToString();
            if (orderType == LandObjectTradesOrderTypes.Create) {
                objectBloc = LandObjectBlocks.ActiveToBeTraded.ToString();
            }
            if (orderType == LandObjectTradesOrderTypes.Cancel) {
                objectBloc = LandObjectBlocks.ActiveFree.ToString();
            }

            var qu = new QueryUpdate(tbObjects);
            qu
                .Set(tbObjects.flBlock, objectBloc)
                .Execute(env.QueryExecuter, transaction);

            var quRev = new QueryUpdate(tbObjRev);
            quRev
                .Set(tbObjRev.flBlock, objectBloc)
                .Execute(env.QueryExecuter, transaction);
        }

        private void setModel<TTradesTable>(DataModifingQueryProxy<TTradesTable> updateOrInsert, LandObjectTradeModel model) where TTradesTable : TbLandObjectsTradesBase {
            updateOrInsert
                .SetT(t => t.flId, model.Id)
                .SetT(t => t.flRevisionId, model.RevisionId)
                .SetT(t => t.flObjectId, model.ObjectId)
                .Set(t => t.flForm, model.Form)
                .Set(t => t.flMethod, model.Method)
                .Set(t => t.flType, model.Type)
                .Set(t => t.flStatus, model.Status)

                .SetT(t => t.flCompetentOrgBin, model.CompetentOrgBin)
                .SetT(t => t.flDateTime, model.DateTime)
                .SetT(t => t.flInitialCost, model.InitialCost)
                .SetT(t => t.flDeposit, model.Deposit)
                .SetT(t => t.flCostStart, model.CostStart)
                .SetT(t => t.flNote, model.Note)

                .Set(t => t.flLandLeaseAvailable, model.LandLeaseAvailable)
                .SetT(t => t.flLandLeaseLength, model.LandLeaseLength)
                .Set(t => t.flAdditionalCondition, model.AdditionalCondition)
                .SetT(t => t.flUpFactor, model.UpFactor)
                .Set(t => t.flCostMinCondition, model.CostMinCondition)
                .SetT(t => t.flCostMin, model.CostMin)

                .SetT(t => t.flOwnershipMonths, model.OwnershipMonths)

                .SetT(t => t.flWinnerId, model.WinnerId)
                .SetT(t => t.flWinnerBin, model.WinnerBin)
                .SetT(t => t.flSaleCondition, model.SaleCondition)
                .SetT(t => t.flCost, model.Cost)
                //.Set(t => t.flPostControl, model.PostControl)

                .SetT(t => t.flIsSaleAgreementExists, model.IsSaleAgreementExists)
                .SetT(t => t.flSaleAgreementDate, model.SaleAgreementDate)
                .SetT(t => t.flSaleAgreementNumber, model.SaleAgreementNumber)
                .SetT(t => t.flPrepayment, model.Prepayment)
                .SetT(t => t.flRemainingSum, model.RemainingSum)
                .SetT(t => t.flRemainingSumDate, model.RemainingSumDate)

                .SetT(t => t.flIsSaleProtocolExists, model.IsSaleProtocolExists)
                .SetT(t => t.flSaleProtocolDate, model.SaleProtocolDate)
                .SetT(t => t.flSaleProtocolNumber, model.SaleProtocolNumber)


                .SetT(t => t.flRequiredDocuments, model.RequiredDocuments)
                .SetT(t => t.flParticipantConditions, model.ParticipantConditions)
                .SetT(t => t.flCommissionMembers, model.CommissionMembers)


                //.SetT(t => t.flIsActExists, model.IsActExists)
                //.SetT(t => t.flSaleActDate, model.SaleActDate)
                //.SetT(t => t.flSaleActNumber, model.SaleActNumber)

                .SetT(t => t.flControlDateStart, model.ControlDateStart)
                .SetT(t => t.flControlDateEnd, model.ControlDateEnd)
                .Set(t => t.flControlFrequency, model.ControlFrequency)
                .SetT(t => t.flControlCondition, model.ControlCondition)

                .SetT(t => t.flTaxAuthorityBin, model.TaxAuthorityBin)
                .SetT(t => t.flBik, model.Bik)
                .SetT(t => t.flIik, model.Iik)
                .SetT(t => t.flKnp, model.Knp)
                .Set(t => t.flKbk, model.Kbk)
                .SetT(t => t.flKbe, model.Kbe)

                .SetT(t => t.flId, model.Id);
        }

        public override bool TryAcceptModel(int revisionId, LandObjectTradeModel model, LandObjectTradesOrderTypes orderType, ActionEnv<LandObjectTradeOrderQueryArgs> env, ITransaction transaction, out string error) {

            var result = true;
            var errorsList = new List<string>() { };

            if ((orderType == LandObjectTradesOrderTypes.Create || orderType == LandObjectTradesOrderTypes.Edit) && env.User.IsExternalUser())
            {
                var minimumDays = GetMinimumTradeDays(model.Id, model.Method, model.ObjectId, env.QueryExecuter);
                if (!IsValidDateTime(model.DateTime.Value, new TbLandObjectsTrades().flDateTime, minimumDays, env))
                {
                    result = false;
                    errorsList.Add(env.T("Не пройдена валидация, отредактируйте объект!"));
                };
            }

            if (orderType == LandObjectTradesOrderTypes.Create)
            {
                if (new TbLandObjectsTrades().AddFilter(t => t.flObjectId, model.ObjectId).AddFilter(t => t.flStatus, RefLandObjectTradeStatuses.Wait).Count(env.QueryExecuter) > 0)
                {
                    result = false;
                    errorsList.Add(env.T("У объекта уже есть ожидающиеся торги!"));
                }
            }

            if (orderType == LandObjectTradesOrderTypes.Cancel)
            {
                var now = env.QueryExecuter.GetDateTime(NpGlobal.DbKeys.DbYodaGr);
                //var ableToEditLastDate = model.DateTime.Value.AddWorkdays(-3, env.QueryExecuter);
                var ableToEditLastDate = model.DateTime.Value;

                if ((now >= ableToEditLastDate) && env.User.IsExternalUser())
                {
                    result = false;
                    errorsList.Add(env.T("Отменять до начала торги можно лишь ранее чем за 3 дня!"));
                }
            }

            if (env.IsValid && errorsList.Count == 0 && result == true)
            {
                var updateOrInsert = orderType switch
                {
                    LandObjectTradesOrderTypes.Create =>
                        new TbLandObjectsTrades()
                            .Insert(),
                    _ =>
                        new TbLandObjectsTrades()
                            .AddFilter(t => t.flId, model.Id)
                            .Update()
                };

                if (env.Args.MenuAction == MnuOrderV2Actions.Execute)
                {
                    setModel(updateOrInsert, model);
                    setObjectStatus(orderType, model, env, transaction);
                    setTradeChange(orderType, model, env, transaction);
                    updateOrInsert.Execute(env.QueryExecuter, transaction);
                }

            }
            error = string.Join(" ", errorsList);
            return result;
        }
        public override bool TryValidateModel(int revisionId, LandObjectTradeModel model, LandObjectTradesOrderTypes orderType, ActionEnv<LandObjectTradeOrderQueryArgs> env, out string[] errors) {

            var result = true;
            var errorsList = new List<string>() { };

            if ((orderType == LandObjectTradesOrderTypes.Create || orderType == LandObjectTradesOrderTypes.Edit) && env.User.IsExternalUser())
            {
                var minimumDays = GetMinimumTradeDays(model.Id, model.Method, model.ObjectId, env.QueryExecuter);
                if (!IsValidDateTime(model.DateTime.Value, new TbLandObjectsTrades().flDateTime, minimumDays, env))
                {
                    result = false;
                    errorsList.Add(env.T("Не пройдена валидация, отредактируйте объект!"));
                };
            }

            if (orderType == LandObjectTradesOrderTypes.Create)
            {
                if (new TbLandObjectsTrades().AddFilter(t => t.flObjectId, model.ObjectId).AddFilter(t => t.flStatus, RefLandObjectTradeStatuses.Wait).Count(env.QueryExecuter) > 0)
                {
                    result = false;
                    errorsList.Add(env.T("У объекта уже есть ожидающиеся торги!"));
                }
            }
            
            if (orderType == LandObjectTradesOrderTypes.Cancel)
            {
                var now = env.QueryExecuter.GetDateTime(NpGlobal.DbKeys.DbYodaGr);
                //var ableToEditLastDate = model.DateTime.Value.AddWorkdays(-3, env.QueryExecuter);
                var ableToEditLastDate = model.DateTime.Value;

                if ((now >= ableToEditLastDate) && env.User.IsExternalUser())
                {
                    result = false;
                    errorsList.Add(env.T("Отменять до начала торги можно лишь ранее чем за 3 дня!"));
                }
            }

            errors = errorsList.ToArray();
            return result;
        }

        public bool IsValidDateTime(DateTime tradeDate, DateTimeField tradeDateField, int minimumDays, ActionEnv<LandObjectTradeOrderQueryArgs> env)
        {
            var isValid = true;
            var now = env.QueryExecuter.GetDateTime(NpGlobal.DbKeys.DbYodaGr);
            var minimumTradeDate = new DateTime(now.Year, now.Month, now.Day, 10, 0, 0).AddDays(minimumDays);

            if ((tradeDate - minimumTradeDate).TotalDays < 0)
            {
                var dateTimeMessage = env.T($"{tradeDateField.Text} не может быть ранее чем через @@@minimumDays дней после исполнения приказа").Text;
                dateTimeMessage = dateTimeMessage.Replace("@@@minimumDays", minimumDays.ToString());
                env.AddError(tradeDateField.FieldName, dateTimeMessage);
                isValid = false;
            }

            var isWorkingDay = new TbNotWorkingDaysCalendar()
            .AddFilter(t => t.flDate, tradeDate)
            .Count(env.QueryExecuter) == 0;
            var isAcceptedDayOfWeek = tradeDate.DayOfWeek.In(DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday);
            var isAcceptedTime = new TimeSpan(10, 0, 0) <= tradeDate.TimeOfDay && tradeDate.TimeOfDay <= new TimeSpan(15, 0, 0);

            if (!isWorkingDay)
            {
                var workingDayMessage = env.T($"Торги могут проводиться только в рабочие дни").Text;
                env.AddError(tradeDateField.FieldName, workingDayMessage);
                isValid = false;
            }
            if (!isAcceptedDayOfWeek)
            {
                var workingDayMessage = env.T($"Торги могут проводиться только со вторника по пятницу").Text;
                env.AddError(tradeDateField.FieldName, workingDayMessage);
                isValid = false;
            }
            if (!isAcceptedTime)
            {
                var workingDayMessage = env.T($"Торги могут проводиться только с 10:00 до 15:00").Text;
                env.AddError(tradeDateField.FieldName, workingDayMessage);
                isValid = false;
            }
            return isValid;
        }
        
        public int GetMinimumTradeDays(int landTradeId, string landTradeMethod, int landObjectId, IQueryExecuter queryExecuter)
        {
            var minimumDays = 15;
            if (landTradeMethod == RefLandObjectTradeForms.Values.ContestRent)
            {
                var landObjectTrades = new TbLandObjectsTrades()
                .AddFilter(t => t.flObjectId, landObjectId)
                .AddFilter(t => t.flMethod, RefLandObjectTradeForms.Values.ContestRent)
                .AddFilterNot(t => t.flId, landTradeId);
                landObjectTrades.OrderBy = new[] { new OrderField(landObjectTrades.flDateTime, OrderType.Desc) };
                var objectTrades = landObjectTrades.Select(queryExecuter).Select().AsEnumerable().ToList();
                var canceledTrades = new string[] { RefLandObjectTradeStatuses.NotHeld, RefLandObjectTradeStatuses.Cancel, RefLandObjectTradeStatuses.CanceledEarlier };
                if (objectTrades.Count >= 1)
                {
                    minimumDays = 10;
                }
            }
            return minimumDays;
        }

        private void setTradeChange(LandObjectTradesOrderTypes orderType, LandObjectTradeModel model, ActionEnv<LandObjectTradeOrderQueryArgs> env, ITransaction transaction)
        {
            if (env.CurAction == Actions.Execute)
            {
                switch (orderType) {
                    case LandObjectTradesOrderTypes.Cancel:
                        {

                            var auctionId = new TbLandObjectTradeChanges().AddFilter(t => t.flTradeId, model.Id).AddFilterNotNull(t => t.flAuctionId).SelectScalar(t => t.flAuctionId, env.QueryExecuter, transaction);
                            
                            if (auctionId.HasValue)
                            {
                                var id = env.QueryExecuter.GetNextId(new TbLandObjectTradeChanges(), new TbLandObjectTradeChanges().flId.FieldName, transaction);
                                var tbChanges = new TbLandObjectTradeChanges()
                                    .Insert()
                                    .SetT(t => t.flTradeId, model.Id)
                                    .SetT(t => t.flAuctionId, auctionId.Value)
                                    .SetT(t => t.flRevisionId, model.RevisionId)
                                    .SetT(t => t.flIsTradeLoaded, false)
                                    .SetT(t => t.flDateTime, DateTime.Now)
                                    .SetT(t => t.flId, id)
                                    .Execute(env.QueryExecuter, transaction);
                            } else
                            {
                                throw new NotImplementedException(env.T("Торги ещё не заливались на ЭТП"));
                            }
                            break;
                        }
                    default:
                        {
                            var id = env.QueryExecuter.GetNextId(new TbLandObjectTradeChanges(), new TbLandObjectTradeChanges().flId.FieldName, transaction);
                            var tbChanges = new TbLandObjectTradeChanges()
                                .Insert()
                                .SetT(t => t.flTradeId, model.Id)
                                .SetT(t => t.flRevisionId, model.RevisionId)
                                .SetT(t => t.flIsTradeLoaded, false)
                                .SetT(t => t.flDateTime, DateTime.Now)
                                .SetT(t => t.flId, id)
                                .Execute(env.QueryExecuter, transaction);
                            break;
                        }
                }
            } else
            {
                return;
            }
        }

        public override void ViewModel(RenderActionEnv<LandObjectTradeOrderQueryArgs> env, LandObjectTradeModel trade, LandObjectTradesOrderTypes orderType, OrderStatus orderStatus) {
            viewModelMainData(env.Form, env, trade);
            if (trade.InitialCost > 0)
            {
                viewModelRequisitesData(env.Form, env, trade);
            }
            if (trade.Method == RefLandObjectTradeForms.Values.LandTradesPriceDown)
            {
                viewModelPriceDownData(env.Form, env, trade);
            }
            if (trade.Type == RefLandObjectTradeTypes.RentOwnership)
            {
                viewModelRentData(env.Form, env, trade);
            }
            if (trade.Type == RefLandObjectTradeTypes.PrivateOwnership)
            {
                viewModelPrivateData(env.Form, env, trade);
            }

            if (trade.Form == RefLandObjectTradeForms.Values.EAuction)
            {
                viewModelCalculatedData(env.Form, env, trade);
            } else if (trade.Form == RefLandObjectTradeForms.Values.Contest)
            {
                viewModelContestData(env.Form, env, trade);
            }

            if (!string.IsNullOrEmpty(trade.WinnerBin))
            {
                viewModelSaleData(env.Form, env, trade);
            }
            if (trade.IsSaleProtocolExists == true)
            {
                viewModelProtocolData(env.Form, env, trade);
            }
            if (trade.IsSaleAgreementExists == true) {
                viewModelAgreementData(env.Form, env, trade);
            }

            //viewModelPublicationsData(env.Form, env, trade);
        }

        public static void viewModelMainData(WidgetBase widget, FormEnvironment env, LandObjectTradeModel trade, bool collapsed = false) {
            var tbRevs = new TbLandObjectsTradesRevisions();
            var groupbox = new Accordion(env.T("Данные по торгам"), collapsed);
            widget.AddComponent(groupbox);

            tbRevs.flId.RenderCustom(groupbox, env, trade.Id.ToString() ?? env.T("*Генерируется автоматически при исполнении приказа"), readOnly: true);
            tbRevs.flRevisionId.RenderCustom(groupbox, env, trade.RevisionId, readOnly: true);

            tbRevs.flObjectId.RenderCustom(groupbox, env, trade.ObjectId, readOnly: true);
            tbRevs.flCompetentOrgBin.RenderCustom(groupbox, env, trade.CompetentOrgBin, readOnly: true);
            tbRevs.flStatus.RenderCustom(groupbox, env, trade.Status, readOnly: true);

            tbRevs.flForm.RenderCustom(groupbox, env, trade.Form, readOnly: true);
            tbRevs.flMethod.RenderCustom(groupbox, env, trade.Method, readOnly: true);
            tbRevs.flType.RenderCustom(groupbox, env, trade.Type, readOnly: true);
            tbRevs.flDateTime.RenderCustom(groupbox, env, trade.DateTime, readOnly: true);
            tbRevs.flNote.RenderCustom(groupbox, env, trade.Note, readOnly: true);

        }

        public static void viewModelContestData(WidgetBase widget, FormEnvironment env, LandObjectTradeModel trade, bool collapsed = false) {
            var tbRevs = new TbLandObjectsTradesRevisions();
            var groupbox = new Accordion(env.T("Условия торгов"), collapsed);
            widget.AddComponent(groupbox);

            tbRevs.flAdditionalCondition.RenderCustom(groupbox, env, trade.AdditionalCondition, readOnly: true);
            tbRevs.flOwnershipMonths.RenderCustom(groupbox, env, trade.OwnershipMonths, readOnly: true);

            tbRevs.flParticipantConditions.RenderCustom(groupbox, env, trade.ParticipantConditions, readOnly: true);
            tbRevs.flRequiredDocuments.RenderCustom(groupbox, env, trade.RequiredDocuments, readOnly: true);
            tbRevs.flCommissionMembers.RenderCustom(groupbox, env, trade.CommissionMembers, readOnly: true);

        }
        //public static void viewModelPublicationsData(WidgetBase widget, FormEnvironment env, LandObjectTradeModel trade, bool collapsed = false) {
        //    var tbRevs = new TbLandObjectsTradesRevisions();
        //    var groupbox = new Accordion(env.T("Публикации в СМИ"), collapsed);
        //    widget.AddComponent(groupbox);

        //    var tbPublications = new TbLandObjectsTradesPublications();

        //    var publKz = new LandObjectTradePublicationsModel();
        //    var publRu = new LandObjectTradePublicationsModel();
        //    if (trade.Publications.Count > 0)
        //    {
        //        publKz = trade.Publications.Where(x => x.NewspaperLang == "110000").First();
        //        publRu = trade.Publications.Where(x => x.NewspaperLang == "100000").First();
        //    }

        //    var groupboxKz = new Accordion(env.T("Публикация на казахском языке"));
        //    groupbox.AddComponent(groupboxKz);
        //    tbPublications.flNewspaperId.RenderCustom(groupboxKz, env, publKz.NewspaperId, fieldAlias: $"{tbPublications.flNewspaperId.FieldName}Kz", readOnly: true);
        //    tbPublications.flNumber.RenderCustom(groupboxKz, env, publKz.Number, fieldAlias: $"{tbPublications.flNumber.FieldName}Kz", readOnly: true);
        //    tbPublications.flDate.RenderCustom(groupboxKz, env, (publKz.Date != null ? publKz.Date : DateTime.Now), fieldAlias: $"{tbPublications.flDate.FieldName}Kz", readOnly: true);
        //    tbPublications.flText.RenderCustom(groupboxKz, env, publKz.Text, fieldAlias: $"{tbPublications.flText.FieldName}Kz", readOnly: true);

        //    var groupboxRu = new Accordion(env.T("Публикация на русском языке"));
        //    groupbox.AddComponent(groupboxRu);
        //    tbPublications.flNewspaperId.RenderCustom(groupboxRu, env, publRu.NewspaperId, fieldAlias: $"{tbPublications.flNewspaperId.FieldName}Ru", readOnly: true);
        //    tbPublications.flNumber.RenderCustom(groupboxRu, env, publRu.Number, fieldAlias: $"{tbPublications.flNumber.FieldName}Ru", readOnly: true);
        //    tbPublications.flDate.RenderCustom(groupboxRu, env, publRu.Date, fieldAlias: $"{tbPublications.flDate.FieldName}Ru", readOnly: true);
        //    tbPublications.flText.RenderCustom(groupboxRu, env, publRu.Text, fieldAlias: $"{tbPublications.flText.FieldName}Ru", readOnly: true);

        //}

        public static void viewModelPriceDownData(WidgetBase widget, FormEnvironment env, LandObjectTradeModel trade, bool collapsed = false) {
            var tbRevs = new TbLandObjectsTradesRevisions();
            var groupbox = new Accordion(env.T("Понижение цены"), collapsed);
            widget.AddComponent(groupbox);

            tbRevs.flAdditionalCondition.RenderCustom(groupbox, env, trade.AdditionalCondition, readOnly: true);
            tbRevs.flUpFactor.RenderCustom(groupbox, env, trade.UpFactor, readOnly: true);

        }

        public static void viewModelRentData(WidgetBase widget, FormEnvironment env, LandObjectTradeModel trade, bool collapsed = false) {
            var tbRevs = new TbLandObjectsTradesRevisions();
            var groupbox = new Accordion(env.T("Аренда"), collapsed);
            widget.AddComponent(groupbox);

            tbRevs.flOwnershipMonths.RenderCustom(groupbox, env, trade.OwnershipMonths, readOnly: true);

        }

        public static void viewModelPrivateData(WidgetBase widget, FormEnvironment env, LandObjectTradeModel trade, bool collapsed = false) {
            var tbRevs = new TbLandObjectsTradesRevisions();
            var groupbox = new Accordion(env.T("Продажа"), collapsed);
            widget.AddComponent(groupbox);

            tbRevs.flLandLeaseAvailable.RenderCustom(groupbox, env, trade.LandLeaseAvailable, readOnly: true);
            tbRevs.flLandLeaseLength.RenderCustom(groupbox, env, trade.LandLeaseLength, readOnly: true);

            if (trade.Method == RefLandObjectTradeForms.Values.LandTradesPriceDown)
            {
                tbRevs.flCostMin.RenderCustom(groupbox, env, trade.CostMin, readOnly: true);
            }

        }


        public static void viewModelCalculatedData(WidgetBase widget, FormEnvironment env, LandObjectTradeModel trade, bool collapsed = false)
        {
            var tbRevs = new TbLandObjectsTradesRevisions();
            var groupbox = new Accordion(env.T("Калькулируемые данные"), collapsed);
            widget.AddComponent(groupbox);

            var tbMrp = new TbMrp();
            tbMrp.AddFilter(tbMrp.flYear, trade.DateTime.Value.Year);
            var mrp = Convert.ToDecimal(tbMrp.SelectScalar(tbMrp.flMrp, env.QueryExecuter));
            var mrp50 = 50 * mrp;
            var mrp30000 = 30000 * mrp;


            var tbLandObjects = new TbLandObjects();
            var ObjectCost = tbLandObjects.AddFilter(t => t.flId, trade.ObjectId).SelectScalar(t => t.flCost, env.QueryExecuter).Value;
            tbLandObjects.flCost.RenderCustom(groupbox, env, ObjectCost, readOnly: true);

            var calcText = string.Empty;
            var years = 0;
            var yearsText = "";
            if (trade.Type == RefLandObjectTradeTypes.RentOwnership) {
                years = trade.OwnershipMonths.Value / 12;
                yearsText = years.ToString();
            }

            if (years <= 10) {
                calcText = $"({tbLandObjects.flCost.Text} * 2% * {yearsText}(срок аренды в годах))";
            }
            else if (years <= 30) {
                calcText = $"({tbLandObjects.flCost.Text} * 20% + 1,5% {env.T("за каждый год свыше")} 10 {env.T("лет")} ({env.T("всего")} {yearsText}))";
            }
            else if (years <= 49) {
                calcText = $"({tbLandObjects.flCost.Text} * 50% + 1% {env.T("за каждый год свыше")} 30 {env.T("лет")} ({env.T("всего")} {yearsText}))";
            }

            if (trade.Method == RefLandObjectTradeForms.Values.LandTradesPriceUp)
            {
                tbRevs.flInitialCost.RenderCustom(groupbox, env, trade.InitialCost, readOnly: true, description: new Label(env.T("(Равна стартовой цене)")));

                if (trade.Type == RefLandObjectTradeTypes.PrivateOwnership) {
                    tbRevs.flCostStart.RenderCustom(groupbox, env, trade.CostStart, readOnly: true);
                } else {
                    tbRevs.flCostStart.RenderCustom(groupbox, env, trade.CostStart, readOnly: true,
                        description: new Label(calcText)
                    );
                }
                
                tbRevs.flDeposit.RenderCustom(groupbox, env, trade.Deposit, readOnly: true,
                    description: new Label($"({env.T("Начальная цена (оценочная стоимость), тг.")} * 15% ({env.T("минимум")} 50 {env.T("мрп")} {mrp50} {env.T("тг.")}), {env.T("максимум")} 30000 {env.T("мрп")} {mrp30000} {env.T("тг.")}), 1 {env.T("мрп")} = {mrp} {env.T("тг.")}))")
                );
            } else
            {
                tbRevs.flInitialCost.RenderCustom(groupbox, env, trade.InitialCost, readOnly: true, description: new Label("(Равна стартовой цене)"));

                tbRevs.flCostStart.RenderCustom(groupbox, env, trade.CostStart, readOnly: true,
                    description: new Label($"({tbRevs.flCostMin.Text} * {tbRevs.flUpFactor.Text} ({trade.UpFactor}))")
                );

                tbRevs.flDeposit.RenderCustom(groupbox, env, trade.Deposit, readOnly: true,
                    description: new Label($"({env.T("Минимальная цена (оценочная стоимость), тг.")} * 15% ({env.T("минимум")} 50 {env.T("мрп")} {mrp50} {env.T("тг.")}), {env.T("максимум")} 30000 {env.T("мрп")} {mrp30000} {env.T("тг.")}), 1 {env.T("мрп")} = {mrp} {env.T("тг.")}))")
                );

                tbRevs.flCostMinCondition.RenderCustom(groupbox, env, trade.CostMinCondition, readOnly: true);


                if (trade.Type == RefLandObjectTradeTypes.PrivateOwnership)
                {
                    tbRevs.flCostMin.RenderCustom(groupbox, env, trade.CostMin);
                } else
                {
                    tbRevs.flCostMin.RenderCustom(groupbox, env, trade.CostMin, readOnly: true,
                        description: new Label(calcText)
                    );

                }
            }
            

        }

        public static void viewModelRequisitesData(WidgetBase widget, FormEnvironment env, LandObjectTradeModel trade, bool collapsed = false) {
            var tbRevs = new TbLandObjectsTradesRevisions();
            var groupbox = new Accordion(env.T("Реквизиты"), collapsed);
            widget.AddComponent(groupbox);

            tbRevs.flTaxAuthorityBin.RenderCustom(groupbox, env, trade.TaxAuthorityBin, readOnly: true);
            tbRevs.flBik.RenderCustom(groupbox, env, trade.Bik, readOnly: true);
            tbRevs.flIik.RenderCustom(groupbox, env, trade.Iik, readOnly: true);
            tbRevs.flKnp.RenderCustom(groupbox, env, trade.Knp, readOnly: true);
            tbRevs.flKbk.RenderCustom(groupbox, env, trade.Kbk, readOnly: true);
            tbRevs.flKbe.RenderCustom(groupbox, env, trade.Kbe, readOnly: true);

        }

        public static void viewModelAgreementData(WidgetBase widget, FormEnvironment env, LandObjectTradeModel trade, bool collapsed = false) {
            var tbRevs = new TbLandObjectsTradesRevisions();
            var groupbox = new Accordion(env.T("Договор"), collapsed);
            widget.AddComponent(groupbox);

            tbRevs.flSaleAgreementDate.RenderCustom(groupbox, env, trade.SaleAgreementDate, readOnly: true);
            tbRevs.flSaleAgreementNumber.RenderCustom(groupbox, env, trade.SaleAgreementNumber, readOnly: true);
            tbRevs.flPrepayment.RenderCustom(groupbox, env, trade.Prepayment, readOnly: true);
            tbRevs.flRemainingSum.RenderCustom(groupbox, env, trade.RemainingSum, readOnly: true);
            tbRevs.flRemainingSumDate.RenderCustom(groupbox, env, trade.RemainingSumDate, readOnly: true);

        }

        public static void viewModelSaleData(WidgetBase widget, FormEnvironment env, LandObjectTradeModel trade, bool collapsed = false) {
            var tbRevs = new TbLandObjectsTradesRevisions();
            var groupbox = new Accordion(env.T("Информация о продаже"), collapsed);
            widget.AddComponent(groupbox);

            tbRevs.flWinnerId.RenderCustom(groupbox, env, trade.WinnerId, readOnly: true);
            tbRevs.flWinnerBin.RenderCustom(groupbox, env, trade.WinnerBin, readOnly: true);
            tbRevs.flSaleCondition.RenderCustom(groupbox, env, trade.SaleCondition, readOnly: true);
            tbRevs.flCost.RenderCustom(groupbox, env, trade.Cost, readOnly: true);
            //tbRevs.flPostControl.RenderCustom(groupbox, env, trade.PostControl, readOnly: true);

        }

        public static void viewModelProtocolData(WidgetBase widget, FormEnvironment env, LandObjectTradeModel trade, bool collapsed = false) {
            var tbRevs = new TbLandObjectsTradesRevisions();
            var groupbox = new Accordion(env.T("Протокол"), collapsed);
            widget.AddComponent(groupbox);

            tbRevs.flSaleProtocolDate.RenderCustom(groupbox, env, trade.SaleProtocolDate, readOnly: true);
            tbRevs.flSaleProtocolNumber.RenderCustom(groupbox, env, trade.SaleProtocolNumber, readOnly: true);

        }

        protected override bool HasAccessToOrder(int revisionId, ActionEnv<LandObjectTradeOrderQueryArgs> env) {
            if ((!env.User.IsExternalUser() && !env.User.IsGuest())) {
                return true;
            }
            if (env.Args.MenuAction != "view-order" && !env.User.HasRole("TRADERESOURCES-Земельные ресурсы-Создание приказов", env.QueryExecuter)/*!env.User.HasCustomRole("landobjects", "appLandEdit", env.QueryExecuter)*/)
            {
                throw new AccessDeniedException();
            }
            var hasRecords = new TbLandObjectsTradesRevisions()
                .AddFilter(t => t.flRevisionId, revisionId)
                .AddFilter(t => t.flCompetentOrgBin, env.User.GetUserBin(env.QueryExecuter))
                .Count(env.QueryExecuter) > 0;

            return hasRecords;
        }
    }

    public class LandObjectTradeOrderQueryArgs: OrderQueryArgs {
        public int? Id { get; set; }
        public int ObjectId { get; set; }
        public string OwnerXin { get; set; }
        public LandObjectTradesOrderTypeActions? OrderType { get; set; }
    }
    public enum LandObjectTradesOrderTypeActions {
        Create,
        Edit,
        Cancel,
        Transfer
    }
}
