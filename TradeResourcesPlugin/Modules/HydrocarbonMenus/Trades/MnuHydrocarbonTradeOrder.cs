using HydrocarbonSource.FieldEditors.Trade;
using CommonSource;
using HydrocarbonSource.Helpers.Trade;
using HydrocarbonSource.Models;
using HydrocarbonSource.QueryTables.Object;
using HydrocarbonSource.QueryTables.Trade;
using HydrocarbonSource.References.Object;
using HydrocarbonSource.References.Trade;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TradeResourcesPlugin.Helpers;
using TradeResourcesPlugin.Modules.HydrocarbonMenus.Objects;
using TradeResourcesPlugin.Modules.LandObjectsMenus.Object;
using UsersResources;
using Yoda.Interfaces;
using Yoda.Interfaces.Forms;
using Yoda.Interfaces.Forms.Components;
using Yoda.Interfaces.Helpers;
using Yoda.Interfaces.Helpers.Mrp.QueryTables;
using Yoda.Interfaces.Helpers.NotWorkingDaysHelper.QueryTables;
using YodaApp.YodaHelpers.OrderHelpers;
using YodaHelpers.ActionMenus;
using YodaHelpers.Components;
using YodaHelpers.DateTimeHelper;
using YodaHelpers.Fields;
using YodaHelpers.OrderHelpers;
using YodaQuery;

namespace TradeResourcesPlugin.Modules.HydrocarbonMenus.Trades {
    public class MnuHydrocarbonTradeOrder : MnuOrderBaseV2<HydrocarbonTradeModel, TradeOrderTypes, HydrocarbonTradeOrderQueryArgs> {

        public MnuHydrocarbonTradeOrder(string moduleName, string menuName, OrderStandartPermissions perms)
            : base(moduleName, menuName, "Приказы по конкурсам", perms, () => new TbTradesOrderResult(), () => new TbTradesOrderNegotiations())
        {
            AsCallback();
            Enabled(rc => {
                var xin = rc.User.GetUserXin(rc.QueryExecuter);
                if (xin == "050540004455"
                || xin == "050540000002"
                || (!rc.User.IsExternalUser() && !rc.User.IsGuest())
                || rc.User.HasRole("TRADERESOURCES-Недропользование-Создание приказов", rc.QueryExecuter)/*rc.User.HasPermission(nameof(RegistersModule), RegistersModule.LocalPermissions.Landlords)*/)
                {
                    return true;
                }
                return false;
            });
        }

        public override Task<ActionItem[]> GetActions(ActionEnv<HydrocarbonTradeOrderQueryArgs> env)
        {
            var ret = base.GetActions(env).Result.ToList();

            switch (env.Args.MenuAction)
            {
                case Actions.ViewOrder:
                    var trade = new TbTradesRevisions().AddFilter(t => t.flRevisionId, env.Args.RevisionId)
                        .GetTradeModelFirst(env.QueryExecuter, null);
                    if (trade != null)
                    {
                        ret.Add(new ActionItem(env.T("Перейти к объекту"), new ActionRedirectData(ModuleName, MnuHydrocarbonObjectView.MnuName, new HydrocarbonObjectViewArgs { MenuAction = "view", Id = trade.flObjectId })));
                    }
                    break;
                case Actions.CreateFrom:
                    break;
            }

            return Task.FromResult(ret.ToArray());
        }

        public override IWizardFormBuilderWithStep<HydrocarbonTradeOrderQueryArgs, ModelOrderArgs<TradeOrderTypes, HydrocarbonTradeOrderQueryArgs>, HydrocarbonTradeModel>
            EditModel(IWizardFormBuilderWithFinishButton<HydrocarbonTradeOrderQueryArgs, ModelOrderArgs<TradeOrderTypes, HydrocarbonTradeOrderQueryArgs>, HydrocarbonTradeModel> wizard)
        {
            return wizard
                .Step("Отмена конкурса", step => step
                .Enabled(env => env.Args.OrderType.In(TradeOrderTypes.Cancel))
                .OnRendering(re => {
                    var tbRev = new TbTradesRevisions();
                    var trade = re.Model;

                    var flStatusRender = new Referencebox("flStatus", new RefTradeStatusesForCanceling(), tbRev.flStatus.Text, new[] { HydrocarbonTradeStatuses.CancelledBefore.ToString() });
                    re.Panel.AddComponent(flStatusRender);


                    viewModelMainData(re.Panel, re.Env, trade);

                    if (!string.IsNullOrEmpty(trade.flWinnerXin))
                    {
                        viewModelSaleData(re.Panel, re.Env, trade);
                    }
                    if (trade.flIsSaleProtocolExists == true)
                    {
                        viewModelProtocolData(re.Panel, re.Env, trade);
                    }
                })
                .OnValidating(ve => {
                    var tbRev = new TbTradesRevisions();

                    var ableToEditLastDate = ve.Model.flDateTime.AddWorkdays(-3, ve.Env.QueryExecuter);

                    if (ve.Env.QueryExecuter.GetDateTime(NpGlobal.DbKeys.DbYodaGr) >= ableToEditLastDate && !(!ve.Env.User.IsExternalUser() && !ve.Env.User.IsGuest()))
                    {
                        throw new NotImplementedException(ve.Env.T($"Отменять торги до начала можно лишь ранее чем за 3 дня!"));
                    }

                    var flStatusRender = new Referencebox("flStatus", new RefTradeStatusesForCanceling(), tbRev.flStatus.Text, new[] { HydrocarbonTradeStatuses.CancelledBefore.ToString() });
                    var values = flStatusRender.GetPostedValues<string>(ve.Env.FormCollection);

                    if (values.Length != 1) {
                        ve.Env.AddError("flStatus", ve.Env.T("Нужно выбрать только один статус для торга"));
                    }
                    else if (!(values.Contains(HydrocarbonTradeStatuses.CancelledBefore.ToString()) || values.Contains(HydrocarbonTradeStatuses.CancelledBeforeObjectSaled.ToString()))) {
                        ve.Env.AddError("flStatus", ve.Env.T("Нужно выбрать только один статус для торга"));
                    }

                })
                .OnProcessing(pe => {
                    var tbRev = new TbTradesRevisions();
                    var trade = pe.Model;

                    var ableToEditLastDate = pe.Model.flDateTime.AddWorkdays(-3, pe.Env.QueryExecuter);

                    if (pe.Env.QueryExecuter.GetDateTime(NpGlobal.DbKeys.DbYodaGr) >= ableToEditLastDate && !(!pe.Env.User.IsExternalUser() && !pe.Env.User.IsGuest()))
                    {
                        throw new NotImplementedException(pe.Env.T($"Отменять торги до начала можно лишь ранее чем за 3 дня!"));
                    }

                    var flStatusRender = new Referencebox("flStatus", new RefTradeStatusesForCanceling(), tbRev.flStatus.Text, new[] { HydrocarbonTradeStatuses.CancelledBefore.ToString() });
                    var value = flStatusRender.GetPostedValues<HydrocarbonTradeStatuses>(pe.Env.FormCollection)[0];

                    trade.flStatus = value;
                }))
                .Step("Перенос конкурса", step => step
                .Enabled(env => env.Args.OrderType.In(TradeOrderTypes.Transfer))
                .OnRendering(re => {
                    var tbRev = new TbTradesRevisions();
                    var trade = re.Model;
                    tbRev.flAppDateTime.RenderCustom(re.Panel, re.Env, trade.flAppDateTime);
                    tbRev.flDateTime.RenderCustom(re.Panel, re.Env, trade.flDateTime);
                })
                .OnValidating(ve => {
                    var tbRev = new TbTradesRevisions();
                    var trade = ve.Model;
                    tbRev.flDateTime.Validate(ve.Env);

                    if (ve.Env.IsValid)
                    {
                        var now = ve.Env.QueryExecuter.GetDateTime(NpGlobal.DbKeys.DbYodaGr);
                        var appDateMin = new DateTime(now.Year, now.Month, now.Day, 10, 0, 0).AddMonths(2).AddDays(1);

                        var appDate = tbRev.flAppDateTime.GetValOrNull(ve.Env);
                        var tradeDateMin = appDate.HasValue ? appDate.Value.AddWorkdays(10, ve.Env.QueryExecuter) : appDateMin.AddWorkdays(10, ve.Env.QueryExecuter);
                        var tradeDateMax = appDate.HasValue ? appDate.Value.AddDays(60) : appDateMin.AddDays(60);

                        var tradeDate = tbRev.flDateTime.GetValOrNull(ve.Env);
                        if (tradeDate.HasValue && tradeDate.Value < tradeDateMin) {
                            ve.Env.AddError(tbRev.flDateTime.FieldName, ve.Env.T("Минимальная дата проведения аукциона 10 рабочих дней со дня завершения приема заявок"));
                        }
                        if (tradeDate.HasValue && tradeDate.Value > tradeDateMax) {
                            ve.Env.AddError(tbRev.flDateTime.FieldName, ve.Env.T("Максимальная дата проведения аукциона 60 календарных дней со дня завершения приема заявок"));
                        }

                        var isWorkingDay = new TbNotWorkingDaysCalendar()
                        .AddFilter(t => t.flDate, tradeDate.Value)
                        .Count(ve.Env.QueryExecuter) == 0;
                        var isAcceptedDayOfWeek = tradeDate.Value.DayOfWeek.In(DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday);
                        var isAcceptedTime = new TimeSpan(10, 0, 0) <= tradeDate.Value.TimeOfDay && tradeDate.Value.TimeOfDay <= new TimeSpan(15, 0, 0);

                        if (!isWorkingDay) {
                            var workingDayMessage = ve.Env.T($"Торги могут проводиться только в рабочие дни").Text;
                            ve.Env.AddError(tbRev.flDateTime.FieldName, workingDayMessage);
                        }
                        if (!isAcceptedDayOfWeek) {
                            var workingDayMessage = ve.Env.T($"Торги могут проводиться только со вторника по пятницу").Text;
                            ve.Env.AddError(tbRev.flDateTime.FieldName, workingDayMessage);
                        }
                        if (!isAcceptedTime) {
                            var workingDayMessage = ve.Env.T($"Торги могут проводиться только с 10:00 до 15:00").Text;
                            ve.Env.AddError(tbRev.flDateTime.FieldName, workingDayMessage);
                        }
                    }
                })
                .OnProcessing(pe => {
                    var tbRev = new TbTradesRevisions();
                    var trade = pe.Model;
                    pe.Model.flDateTime = tbRev.flDateTime.GetVal(pe.Env);
                }))
                .Step("Конкурс", step => step
                .Enabled(env => env.Args.OrderType.In(TradeOrderTypes.Create, TradeOrderTypes.Edit))
                .OnRendering(re => {
                    var tbRev = new TbTradesRevisions();
                    var trade = re.Model;
                    tbRev.flCompetentOrgBin.RenderCustom(re.Panel, re.Env, trade.flCompetentOrgBin, readOnly: true);
                    tbRev.flType.RenderCustom(re.Panel, re.Env, trade.flType, readOnly: true);
                    tbRev.flStatus.RenderCustom(re.Panel, re.Env, trade.flStatus, readOnly: true);
                    tbRev.flObjectId.RenderCustom(re.Panel, re.Env, trade.flObjectId, readOnly: true);
                    tbRev.flAppDateTime.RenderCustom(re.Panel, re.Env, trade.flAppDateTime);
                    tbRev.flDateTime.RenderCustom(re.Panel, re.Env, trade.flDateTime);
                    tbRev.flStartRate.RenderCustom(re.Panel, re.Env, trade.flStartRate);
                    tbRev.flGarPay.RenderCustom(re.Panel, re.Env, trade.flGarPay, readOnly: true);
                    tbRev.flTradeOperationType.RenderCustom(re.Panel, re.Env, trade.flTradeOperationType);
                    tbRev.flExploringPeriod.RenderCustom(re.Panel, re.Env, trade.flExploringPeriod);
                    tbRev.flAdditionalConditions.RenderCustom(re.Panel, re.Env, trade.flAdditionalConditions, readOnly: true);

                    var edForPartCondJur = new RequirementsListEditor(tbRev.flParticipantConditions.FieldName + "_jur", re.Env);
                    edForPartCondJur.CaptionText = re.Env.T("Требования к участнику (юридическому лицу)");
                    var edForPartCondFiz = new RequirementsListEditor(tbRev.flParticipantConditions.FieldName + "_fiz", re.Env);
                    edForPartCondFiz.CaptionText = re.Env.T("Требования к участнику (физическому лицу)");
                    var edForPartCondFizEnt = new RequirementsListEditor(tbRev.flParticipantConditions.FieldName + "_fizEnt", re.Env);
                    edForPartCondFizEnt.CaptionText = re.Env.T("Требования к участнику (индивидуальному предпринимателю)");

                    tbRev.flParticipantConditions.RenderCustom(re.Panel, re.Env, trade.flParticipantConditionsJur, tbRev.flParticipantConditions.FieldName + "_jur",
                        customEditor: edForPartCondJur, hideLabel: true);
                    tbRev.flParticipantConditions.RenderCustom(re.Panel, re.Env, trade.flParticipantConditionsFiz, tbRev.flParticipantConditions.FieldName + "_fiz",
                        customEditor: edForPartCondFiz, hideLabel: true);
                    tbRev.flParticipantConditions.RenderCustom(re.Panel, re.Env, trade.flParticipantConditionsFizEnt, tbRev.flParticipantConditions.FieldName + "_fizEnt",
                        customEditor: edForPartCondFizEnt, hideLabel: true);

                    var editorForJur = new RequirementsListEditor(tbRev.flRequiredDocuments.FieldName + "_jur", re.Env);
                    var editorForFiz = new RequirementsListEditor(tbRev.flRequiredDocuments.FieldName + "_fiz", re.Env);
                    var editorForFizEnt = new RequirementsListEditor(tbRev.flRequiredDocuments.FieldName + "_fizEnt", re.Env);
                    editorForJur.CaptionText = re.Env.T("Список требуемых документов от юр.лица");
                    editorForFiz.CaptionText = re.Env.T("Список требуемых документов физ. лица");
                    editorForFizEnt.CaptionText = re.Env.T("Список требуемых документов от индивидуального предпринимателя");
                    tbRev.flRequiredDocuments.Text = re.Env.T("Список требуемых документов от юр.лица");
                    tbRev.flRequiredDocuments.RenderCustom(re.Panel, re.Env, trade.flRequiredDocumentsJur, fieldAlias: tbRev.flRequiredDocuments.FieldName + "_jur",
                        customEditor: editorForJur, hideLabel: true);
                    tbRev.flRequiredDocuments.Text = re.Env.T("Список требуемых документов физ.лица");
                    tbRev.flRequiredDocuments.RenderCustom(re.Panel, re.Env, trade.flRequiredDocumentsFiz, fieldAlias: tbRev.flRequiredDocuments.FieldName + "_fiz",
                        customEditor: editorForFiz, hideLabel: true);
                    tbRev.flRequiredDocuments.Text = re.Env.T("Список требуемых документов от индивидуального предпринимателя");
                    tbRev.flRequiredDocuments.RenderCustom(re.Panel, re.Env, trade.flRequiredDocumentsFizEnt, fieldAlias: tbRev.flRequiredDocuments.FieldName + "_fizEnt",
                        customEditor: editorForFizEnt, hideLabel: true);
                    tbRev.flCommissionMembers.RenderCustom(re.Panel, re.Env, trade.flCommissionMembers, hideLabel: true);
                    tbRev.flNote.RenderCustom(re.Panel, re.Env, trade.flNote);
                })
                .OnValidating(ve => {
                    var tbRev = new TbTradesRevisions();

                    tbRev.flAppDateTime.Required().Validate(ve.Env);
                    tbRev.flDateTime.Required().Validate(ve.Env);
                    tbRev.flStartRate.Required().Validate(ve.Env);
                    tbRev.flTradeOperationType.Required().Validate(ve.Env);
                    tbRev.flExploringPeriod.Required().Validate(ve.Env);

                    //tbRev.flTaxOrganBin.Required().Validate(ve.Env);
                    //tbRev.flIik.Required().Validate(ve.Env);
                    //tbRev.flBik.Required().Validate(ve.Env);
                    //tbRev.flKnp.Required().Validate(ve.Env);
                    //tbRev.flKbe.Required().Validate(ve.Env);
                    //tbRev.flKbk.Required().Validate(ve.Env);

                    if (ve.Env.IsValid) {
                        ve.Model.flRequiredDocumentsJur = JsonConvert.DeserializeObject<TextItem[]>(ve.Env.FormCollection[tbRev.flRequiredDocuments.FieldName + "_jur"] + string.Empty);
                        ve.Model.flRequiredDocumentsFiz = JsonConvert.DeserializeObject<TextItem[]>(ve.Env.FormCollection[tbRev.flRequiredDocuments.FieldName + "_fiz"] + string.Empty);
                        ve.Model.flRequiredDocumentsFizEnt = JsonConvert.DeserializeObject<TextItem[]>(ve.Env.FormCollection[tbRev.flRequiredDocuments.FieldName + "_fizEnt"] + string.Empty);
                        ve.Model.flParticipantConditionsJur = JsonConvert.DeserializeObject<TextItem[]>(ve.Env.FormCollection[tbRev.flParticipantConditions.FieldName + "_jur"] + string.Empty);
                        ve.Model.flParticipantConditionsFiz = JsonConvert.DeserializeObject<TextItem[]>(ve.Env.FormCollection[tbRev.flParticipantConditions.FieldName + "_fiz"] + string.Empty);
                        ve.Model.flParticipantConditionsFizEnt = JsonConvert.DeserializeObject<TextItem[]>(ve.Env.FormCollection[tbRev.flParticipantConditions.FieldName + "_fizEnt"] + string.Empty);
                        ve.Model.flCommissionMembers = JsonConvert.DeserializeObject<Dictionary<string, object>[]>(ve.Env.FormCollection[tbRev.flCommissionMembers.FieldName] + string.Empty);

                        var now = ve.Env.QueryExecuter.GetDateTime(NpGlobal.DbKeys.DbYodaGr);
                        var appDateMin = new DateTime(now.Year, now.Month, now.Day, 10, 0, 0).AddMonths(2).AddDays(1);

                        var appDate = tbRev.flAppDateTime.GetValOrNull(ve.Env);
                        var tradeDateMin = appDate.HasValue ? appDate.Value.AddWorkdays(10, ve.Env.QueryExecuter) : appDateMin.AddWorkdays(10, ve.Env.QueryExecuter);
                        var tradeDateMax = appDate.HasValue ? appDate.Value.AddDays(60) : appDateMin.AddDays(60);

                        var tradeDate = tbRev.flDateTime.GetValOrNull(ve.Env);
                        if (tradeDate.HasValue && tradeDate.Value < tradeDateMin) {
                            ve.Env.AddError(tbRev.flDateTime.FieldName, ve.Env.T("Минимальная дата проведения аукциона 10 рабочих дней со дня завершения приема заявок"));
                        }
                        if (tradeDate.HasValue && tradeDate.Value > tradeDateMax) {
                            ve.Env.AddError(tbRev.flDateTime.FieldName, ve.Env.T("Максимальная дата проведения аукциона 60 календарных дней со дня завершения приема заявок"));
                        }

                        var isWorkingDay = new TbNotWorkingDaysCalendar()
                        .AddFilter(t => t.flDate, tradeDate.Value)
                        .Count(ve.Env.QueryExecuter) == 0;
                        var isAcceptedDayOfWeek = tradeDate.Value.DayOfWeek.In(DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday);
                        var isAcceptedTime = new TimeSpan(10, 0, 0) <= tradeDate.Value.TimeOfDay && tradeDate.Value.TimeOfDay <= new TimeSpan(15, 0, 0);

                        if (!isWorkingDay) {
                            var workingDayMessage = ve.Env.T($"Торги могут проводиться только в рабочие дни").Text;
                            ve.Env.AddError(tbRev.flDateTime.FieldName, workingDayMessage);
                        }
                        if (!isAcceptedDayOfWeek) {
                            var workingDayMessage = ve.Env.T($"Торги могут проводиться только со вторника по пятницу").Text;
                            ve.Env.AddError(tbRev.flDateTime.FieldName, workingDayMessage);
                        }
                        if (!isAcceptedTime) {
                            var workingDayMessage = ve.Env.T($"Торги могут проводиться только с 10:00 до 15:00").Text;
                            ve.Env.AddError(tbRev.flDateTime.FieldName, workingDayMessage);
                        }

                        if (ve.Model.flCommissionMembers == null || ve.Model.flCommissionMembers.Length == 0) {
                            ve.Env.AddError(tbRev.flCommissionMembers.FieldName, ve.Env.T("Требуется указать как минимум 1 члена комиссии"));
                        }
                        if (ve.Model.flRequiredDocumentsFiz.Length == 0 && ve.Model.flRequiredDocumentsFizEnt.Length == 0 && ve.Model.flRequiredDocumentsJur.Length == 0) {
                            ve.Env.AddError(tbRev.flCommissionMembers.FieldName, ve.Env.T("Требуется указать как минимум 1 документ"));
                        }
                    }
                })
                .OnProcessing(pe => {
                    var tbRev = new TbTradesRevisions();
                    pe.Model.flAppDateTime = tbRev.flAppDateTime.GetVal(pe.Env);
                    pe.Model.flDateTime = tbRev.flDateTime.GetVal(pe.Env);
                    pe.Model.flStartRate = tbRev.flStartRate.GetVal(pe.Env);
                    pe.Model.flTradeOperationType = tbRev.flTradeOperationType.GetVal(pe.Env);
                    pe.Model.flExploringPeriod = tbRev.flExploringPeriod.GetVal(pe.Env);
                    var index = 0;
                    pe.Model.flRequiredDocumentsJur = JsonConvert.DeserializeObject<TextItem[]>(pe.Env.FormCollection[tbRev.flRequiredDocuments.FieldName + "_jur"] + string.Empty).Select(x => { x.Type = "jur"; if (string.IsNullOrEmpty(x.Id)) { x.Id = $"Hydrocarbon_{DateTime.Now:yyyy_MM_dd}_{++index}"; } return x; }).ToArray();
                    pe.Model.flRequiredDocumentsFiz = JsonConvert.DeserializeObject<TextItem[]>(pe.Env.FormCollection[tbRev.flRequiredDocuments.FieldName + "_fiz"] + string.Empty).Select(x => { x.Type = "fiz"; if (string.IsNullOrEmpty(x.Id)) { x.Id = $"Hydrocarbon_{DateTime.Now:yyyy_MM_dd}_{++index}"; } return x; }).ToArray();
                    pe.Model.flRequiredDocumentsFizEnt = JsonConvert.DeserializeObject<TextItem[]>(pe.Env.FormCollection[tbRev.flRequiredDocuments.FieldName + "_fizEnt"] + string.Empty).Select(x => { x.Type = "fizEnt"; if (string.IsNullOrEmpty(x.Id)) { x.Id = $"Hydrocarbon_{DateTime.Now:yyyy_MM_dd}_{++index}"; } return x; }).ToArray();
                    pe.Model.flParticipantConditionsJur = JsonConvert.DeserializeObject<TextItem[]>(pe.Env.FormCollection[tbRev.flParticipantConditions.FieldName + "_jur"] + string.Empty).Select(x => { x.Type = "jur"; if (string.IsNullOrEmpty(x.Id)) { x.Id = $"Hydrocarbon_{DateTime.Now:yyyy_MM_dd}_{++index}"; } return x; }).ToArray();
                    pe.Model.flParticipantConditionsFiz = JsonConvert.DeserializeObject<TextItem[]>(pe.Env.FormCollection[tbRev.flParticipantConditions.FieldName + "_fiz"] + string.Empty).Select(x => { x.Type = "fiz"; if (string.IsNullOrEmpty(x.Id)) { x.Id = $"Hydrocarbon_{DateTime.Now:yyyy_MM_dd}_{++index}"; } return x; }).ToArray();
                    pe.Model.flParticipantConditionsFizEnt = JsonConvert.DeserializeObject<TextItem[]>(pe.Env.FormCollection[tbRev.flParticipantConditions.FieldName + "_fizEnt"] + string.Empty).Select(x => { x.Type = "fizEnt"; if (string.IsNullOrEmpty(x.Id)) { x.Id = $"Hydrocarbon_{DateTime.Now:yyyy_MM_dd}_{++index}"; } return x; }).ToArray();
                    pe.Model.flCommissionMembers = JsonConvert.DeserializeObject<Dictionary<string, object>[]>(pe.Env.FormCollection[tbRev.flCommissionMembers.FieldName] + string.Empty);
                    pe.Model.flNote = tbRev.flNote.GetVal(pe.Env);

                    var allDocuments = new List<TextItem>();
                    allDocuments.AddRange(pe.Model.flRequiredDocumentsJur);
                    allDocuments.AddRange(pe.Model.flRequiredDocumentsFiz);
                    allDocuments.AddRange(pe.Model.flRequiredDocumentsFizEnt);
                    pe.Model.flRequiredDocuments = allDocuments.ToArray();
                    var allConditions = new List<TextItem>();
                    allConditions.AddRange(pe.Model.flParticipantConditionsJur);
                    allConditions.AddRange(pe.Model.flParticipantConditionsFiz);
                    allConditions.AddRange(pe.Model.flParticipantConditionsFizEnt);
                    pe.Model.flParticipantConditions = allConditions.ToArray();
                }))
                ;
        }


        public override HydrocarbonTradeModel GetModel(ActionEnv<HydrocarbonTradeOrderQueryArgs> env, GetModelFrom from, TradeOrderTypes orderType, ITransaction transaction)
        {
            var now = env.QueryExecuter.GetDateTime(NpGlobal.DbKeys.DbYodaGr);
            var appDate = new DateTime(now.Year, now.Month, now.Day, 10, 0, 0).AddMonths(2).AddDays(1);
            var tradeDate = appDate.AddWorkdays(10, env.QueryExecuter);
            var tbMrp = new TbMrp();
            tbMrp.AddFilter(tbMrp.flYear, tradeDate.Year);
            var mrp = Convert.ToDecimal(tbMrp.SelectScalar(tbMrp.flMrp, env.QueryExecuter));

            var model = from switch
            {
                GetModelFrom.Empty => new HydrocarbonTradeModel()
                {
                    flCompetentOrgBin = env.Args.OwnerXin,
                    flType = HydrocarbonTradeTypes.PriceUp,
                    flGarPay = 100 * mrp,
                    flAppDateTime = appDate,
                    flDateTime = tradeDate,
                    flAdditionalConditions = TradeAdditionalConditions.SingleParticipantCannotWin,
                    flStatus = HydrocarbonTradeStatuses.Wait,
                    flCommissionMembers = new Dictionary<string, object>[] {},
                    flObjectId = env.Args.ObjectId,
                    flRequiredDocumentsFiz = RequirementsListEditor.GetRequiredDocsFixedFiz(),
                    flRequiredDocumentsFizEnt = RequirementsListEditor.GetRequiredDocsFixedFizEnt(),
                    flRequiredDocumentsJur = RequirementsListEditor.GetRequiredDocsFixedJur(),
                    flParticipantConditionsJur = RequirementsListEditor.GetTradeParticipantConditionJur(),
                    flParticipantConditionsFiz = RequirementsListEditor.GetTradeParticipantConditionFiz(),
                    flParticipantConditionsFizEnt = RequirementsListEditor.GetTradeParticipantConditionFizEnt()
                },
                GetModelFrom.Entity =>
                    new TbTrades()
                        .AddFilter(t => t.flId, env.Args.Id.Value)
                        .GetTradeModelFirst(env.QueryExecuter, transaction),
                GetModelFrom.Revision =>
                    new TbTradesRevisions()
                        .AddFilter(t => t.flRevisionId, env.Args.RevisionId.Value)
                        .GetTradeModelFirst(env.QueryExecuter, transaction),
                _ => throw new NotImplementedException()
            };

            switch (orderType)
            {
                case TradeOrderTypes.Create:
                    model.flStatus = HydrocarbonTradeStatuses.Wait;
                    break;
                case TradeOrderTypes.Transfer:
                    model.flAppDateTime = appDate;
                    model.flDateTime = tradeDate;
                    break;
                case TradeOrderTypes.Cancel:
                    model.flStatus = HydrocarbonTradeStatuses.CancelledBefore;
                    break;
                default:
                    // Status is not changing
                    break;
            }

            return model;
        }

        public override TradeOrderTypes GetOrderType(ActionEnv<HydrocarbonTradeOrderQueryArgs> env, GetOrderModelFrom orderModelFrom)
        {
            return orderModelFrom switch
            {
                GetOrderModelFrom.Empty => TradeOrderTypes.Create,
                _ => env.Args.OrderType switch
                {
                    TradesOrderTypeActions.Cancel => TradeOrderTypes.Cancel,
                    TradesOrderTypeActions.Edit => TradeOrderTypes.Edit,
                    TradesOrderTypeActions.Transfer => TradeOrderTypes.Transfer,
                    _ => throw new ArgumentException($"OrderType: {env.Args.OrderType}")
                }
            };
        }

        public override string GetWizardCancelRedirectUrl(ActionEnv<HydrocarbonTradeOrderQueryArgs> env, TradeOrderTypes orderType, GetModelFrom modelFrom)
        {
            var urlHelper = env.RequestContext.GetUrlHelper();
            //TODO
            return modelFrom switch
            {
                GetModelFrom.Empty => urlHelper.YodaAction(ModuleName, nameof(MnuHydrocarbonTradeOrder)),
                GetModelFrom.Entity => urlHelper.YodaAction(ModuleName, nameof(MnuHydrocarbonTradeOrder), new HydrocarbonTradeOrderQueryArgs { Id = env.Args.Id.Value, MenuAction = "view" }),
                GetModelFrom.Revision => urlHelper.YodaAction(ModuleName, MenuName, new HydrocarbonTradeOrderQueryArgs { RevisionId = env.Args.RevisionId, MenuAction = Actions.ViewOrder }),
                _ => throw new NotImplementedException()
            };
        }

        public override ValidationResultBase IsCreateFromActionValid(ActionEnv<HydrocarbonTradeOrderQueryArgs> env) {
            if (env.Args.Id == null) {
                return new RedirectResult(new ArgumentException("Id"));
            }

            if (env.Args.OrderType == null) {
                return new RedirectResult(new ArgumentException("OrderType"));
            }

            if (!CanCreateOrder(env.Args.Id.Value, GetOrderType(env, GetOrderModelFrom.Entity), out var error, env)) {
                if (env.Args.Id.HasValue) {
                    return new RedirectResult(ModuleName, MnuHydrocarbonTradeView.MnuName, new MnuHydrocarbonTradeViewArgs() { tradeId = env.Args.Id.Value }, error);
                }
                else {
                    return new RedirectResult(ModuleName, "ActionError", null, error);
                }
            }

            return new OkResult();
        }

        public static bool TryGetCurrentInWorkOrderRevId(int id, out int revId, IQueryExecuter queryExecuter)
        {
            var ret = new TbTradesOrderResult()
                .AddFilterNotFinishedOrders()
                .JoinT("tbOrders", new TbTradesRevisions().AddFilter(t => t.flId, id), "tbRevs")
                .On((t1, t2) => new Join(t1.flSubjectId, t2.flRevisionId))
                .SelectScalar(t => t.L.flSubjectId, queryExecuter);

            if (ret == null)
            {
                revId = 0;
                return false;
            }

            revId = ret.Value;
            return true;
        }

        public static bool CanCreateOrder(int id, TradeOrderTypes orderType, out string error, ActionEnv<HydrocarbonTradeOrderQueryArgs> env)
        {
            if (TryGetCurrentInWorkOrderRevId(id, out _, env.QueryExecuter))
            {
                error = env.T("Для данного объекта имеется не исполненый приказ");
                return false;
            }

            var tradeResourceObject = TradeHelper.GetTradeModel(id, env.QueryExecuter);


            bool checkTradeWaiting(out string err)
            {
                if (tradeResourceObject.flStatus != HydrocarbonTradeStatuses.Wait)
                {
                    err = env.T("Операция невозможна, так как конкурс завершен");
                    return false;
                }
                err = null;
                return true;
            }

            switch (orderType)
            {
                case TradeOrderTypes.Create:
                    error = env.T("Недопустимый тип приказа");
                    return false;
                case TradeOrderTypes.Edit:
                    return checkTradeWaiting(out error);
                case TradeOrderTypes.Cancel:
                    if (!checkTradeWaiting(out error))
                    {
                        return false;
                    }
                    error = null;
                    return true;
                case TradeOrderTypes.Transfer:
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

        public override void SaveModel(int revisionId, HydrocarbonTradeModel model, TradeOrderTypes orderType, ActionEnv<HydrocarbonTradeOrderQueryArgs> env, ITransaction transaction, GetModelFrom modelFrom)
        {
            var tbTradeRevs = new TbTradesRevisions();

            if (modelFrom == GetModelFrom.Empty)
            {
                model.flId = new TbTrades().flId.GetNextId(env.QueryExecuter, transaction);
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
            model.flRevisionId = revisionId;

            setModel(updateOrInsert, model);

            updateOrInsert
                .Execute(env.QueryExecuter, transaction);


        }
        private void setObjectStatus(TradeOrderTypes orderType, HydrocarbonTradeModel model, ActionEnv<HydrocarbonTradeOrderQueryArgs> env, ITransaction transaction)
        {
            switch (orderType)
            {
                case TradeOrderTypes.Edit:
                case TradeOrderTypes.Transfer:
                    return;
            }
            switch (env.CurAction)
            {
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
            var tbObjects = new TbObjects();
            tbObjects.AddFilter(t => t.flId, model.flObjectId);

            var rObject = tbObjects.SelectFirst(t => t.Fields.ToFieldsAliases(), env.QueryExecuter, transaction);

            var tbObjRev = new TbObjectsRevisions();
            var revObj = tbObjRev.AddFilter(t => t.flId, model.flObjectId).AddFilter(t => t.flRevisionId, rObject.GetVal(a => a.flRevisionId)).SelectFirst(t => t.Fields.ToFieldsAliases(), env.QueryExecuter, transaction);

            var objectBloc = HydrocarbonObjectBlocks.ActiveFree.ToString();
            if (orderType == TradeOrderTypes.Create)
            {
                objectBloc = HydrocarbonObjectBlocks.ActiveToBeTraded.ToString();
            }
            if (orderType == TradeOrderTypes.Cancel)
            {
                objectBloc = HydrocarbonObjectBlocks.ActiveFree.ToString();
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

        private void setModel<TTradesTable>(DataModifingQueryProxy<TTradesTable> updateOrInsert, HydrocarbonTradeModel model) where TTradesTable : TbTradesBase
        {
            updateOrInsert
                .SetT(t => t.flId, model.flId)
                .SetT(t => t.flRevisionId, model.flRevisionId)
                .SetT(t => t.flObjectId, model.flObjectId)
                .SetT(t => t.flStatus, model.flStatus)
                .SetT(t => t.flType, model.flType)
                .SetT(t => t.flCompetentOrgBin, model.flCompetentOrgBin)

                .SetT(t => t.flAppDateTime, model.flAppDateTime)
                .SetT(t => t.flDateTime, model.flDateTime)
                .SetT(t => t.flStartRate, model.flStartRate)
                .SetT(t => t.flGarPay, model.flGarPay)
                .SetT(t => t.flTradeOperationType, model.flTradeOperationType)
                .SetT(t => t.flExploringPeriod, model.flExploringPeriod)

                .SetT(t => t.flAdditionalConditions, model.flAdditionalConditions)
                .SetT(t => t.flParticipantConditions, model.flParticipantConditions)
                .SetT(t => t.flRequiredDocuments, model.flRequiredDocuments)
                .SetT(t => t.flCommissionMembers, model.flCommissionMembers)

                .SetT(t => t.flNote, model.flNote);

        }

        public override bool TryAcceptModel(int revisionId, HydrocarbonTradeModel model, TradeOrderTypes orderType, ActionEnv<HydrocarbonTradeOrderQueryArgs> env, ITransaction transaction, out string error)
        {
            var result = true;
            var errorsList = new List<string>() { };

            if (orderType == TradeOrderTypes.Create)
            {
                if (new TbTrades().AddFilter(t => t.flObjectId, model.flObjectId).AddFilter(t => t.flStatus, HydrocarbonTradeStatuses.Wait).Count(env.QueryExecuter) > 0)
                {
                    result = false;
                    errorsList.Add(env.T("У объекта уже есть ожидающийся конкурс!"));
                }
            }
            else if (orderType == TradeOrderTypes.Cancel || orderType == TradeOrderTypes.Edit)
            {
                var now = env.QueryExecuter.GetDateTime(NpGlobal.DbKeys.DbYodaGr);
                var ableToEditLastDate = model.flDateTime.AddWorkdays(-3, env.QueryExecuter);

                if (now >= ableToEditLastDate && env.User.IsExternalUser())
                {
                    result = false;
                    errorsList.Add(env.T("Отменять до начала или изменять конкурс можно лишь ранее чем за 3 дня!"));
                }
            }

            if (env.IsValid && errorsList.Count == 0 && result == true)
            {
                var updateOrInsert = orderType switch
                {
                    TradeOrderTypes.Create =>
                        new TbTrades()
                            .Insert(),
                    _ =>
                        new TbTrades()
                            .AddFilter(t => t.flId, model.flId)
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

        public override bool TryValidateModel(int revisionId, HydrocarbonTradeModel model, TradeOrderTypes orderType, ActionEnv<HydrocarbonTradeOrderQueryArgs> env, out string[] errors)
        {
            var result = true;
            var errorsList = new List<string>() { };

            if (orderType == TradeOrderTypes.Create)
            {
                if (new TbTrades().AddFilter(t => t.flObjectId, model.flObjectId).AddFilter(t => t.flStatus, HydrocarbonTradeStatuses.Wait).Count(env.QueryExecuter) > 0)
                {
                    result = false;
                    errorsList.Add(env.T("У объекта уже есть ожидающийся конкурс!"));
                }
            }
            else if (orderType == TradeOrderTypes.Cancel || orderType == TradeOrderTypes.Edit)
            {
                var now = env.QueryExecuter.GetDateTime(NpGlobal.DbKeys.DbYodaGr);
                var ableToEditLastDate = model.flDateTime.AddWorkdays(-3, env.QueryExecuter);

                if (now >= ableToEditLastDate && env.User.IsExternalUser())
                {
                    result = false;
                    errorsList.Add(env.T("Отменять до начала или изменять конкурс можно лишь ранее чем за 3 дня!"));
                }
            }

            errors = errorsList.ToArray();
            return result;
        }

        private void setTradeChange(TradeOrderTypes orderType, HydrocarbonTradeModel model, ActionEnv<HydrocarbonTradeOrderQueryArgs> env, ITransaction transaction)
        {
            if (env.CurAction == Actions.Execute)
            {
                switch (orderType)
                {
                    case TradeOrderTypes.Cancel:
                        {

                            var auctionId = new TbTradeChanges().AddFilter(t => t.flTradeId, model.flId).AddFilterNotNull(t => t.flAuctionId).SelectScalar(t => t.flAuctionId, env.QueryExecuter, transaction);

                            if (auctionId.HasValue)
                            {
                                var id = env.QueryExecuter.GetNextId(new TbTradeChanges(), new TbTradeChanges().flId.FieldName, transaction);
                                var tbChanges = new TbTradeChanges()
                                    .Insert()
                                    .SetT(t => t.flTradeId, model.flId)
                                    .SetT(t => t.flAuctionId, auctionId.Value)
                                    .SetT(t => t.flRevisionId, model.flRevisionId)
                                    .SetT(t => t.flIsTradeLoaded, false)
                                    .SetT(t => t.flDateTime, DateTime.Now)
                                    .SetT(t => t.flId, id)
                                    .Execute(env.QueryExecuter, transaction);
                            }
                            else
                            {
                                throw new NotImplementedException(env.T("Конкурс ещё не заливался на ЭТП"));
                            }
                            break;
                        }
                    default:
                        {
                            var id = env.QueryExecuter.GetNextId(new TbTradeChanges(), new TbTradeChanges().flId.FieldName, transaction);
                            var tbChanges = new TbTradeChanges()
                                .Insert()
                                .SetT(t => t.flTradeId, model.flId)
                                .SetT(t => t.flRevisionId, model.flRevisionId)
                                .SetT(t => t.flIsTradeLoaded, false)
                                .SetT(t => t.flDateTime, DateTime.Now)
                                .SetT(t => t.flId, id)
                                .Execute(env.QueryExecuter, transaction);
                            break;
                        }
                }
            }
            else
            {
                return;
            }
        }

        public override void ViewModel(RenderActionEnv<HydrocarbonTradeOrderQueryArgs> env, HydrocarbonTradeModel trade, TradeOrderTypes orderType, OrderStatus orderStatus)
        {
            ViewModel(env.Form, env, trade);
        }

        public static void ViewModel(WidgetBase widget, FormEnvironment env, HydrocarbonTradeModel trade)
        {
            viewModelMainData(widget, env, trade);

            if (!string.IsNullOrEmpty(trade.flWinnerXin))
            {
                viewModelSaleData(widget, env, trade);
            }
            if (trade.flIsSaleProtocolExists == true)
            {
                viewModelProtocolData(widget, env, trade);
            }

        }

        public static void viewModelMainData(WidgetBase widget, FormEnvironment env, HydrocarbonTradeModel trade, bool collapsed = false)
        {
            var tbRevs = new TbTradesRevisions();
            var groupbox = new Accordion(env.T("Данные по конкурсу"), collapsed);
            widget.AddComponent(groupbox);

            tbRevs.flId.RenderCustom(groupbox, env, trade.flId.ToString() ?? env.T("*Генерируется автоматически при исполнении приказа"), readOnly: true);
            tbRevs.flRevisionId.RenderCustom(groupbox, env, trade.flRevisionId, readOnly: true);
            tbRevs.flObjectId.RenderCustom(groupbox, env, trade.flObjectId, readOnly: true);
            tbRevs.flStatus.RenderCustom(groupbox, env, trade.flStatus, readOnly: true);
            tbRevs.flType.RenderCustom(groupbox, env, trade.flType, readOnly: true);
            tbRevs.flCompetentOrgBin.RenderCustom(groupbox, env, trade.flCompetentOrgBin, readOnly: true);
            tbRevs.flAppDateTime.RenderCustom(groupbox, env, trade.flAppDateTime, readOnly: true);
            tbRevs.flDateTime.RenderCustom(groupbox, env, trade.flDateTime, readOnly: true);
            tbRevs.flStartRate.RenderCustom(groupbox, env, trade.flStartRate, readOnly: true);
            tbRevs.flGarPay.RenderCustom(groupbox, env, trade.flGarPay, readOnly: true);
            tbRevs.flTradeOperationType.RenderCustom(groupbox, env, trade.flTradeOperationType, readOnly: true);
            tbRevs.flExploringPeriod.RenderCustom(groupbox, env, trade.flExploringPeriod, readOnly: true);
            tbRevs.flAdditionalConditions.RenderCustom(groupbox, env, trade.flAdditionalConditions, readOnly: true);
            tbRevs.flParticipantConditions.RenderCustom(groupbox, env, trade.flParticipantConditions, customEditor: new RequirementsListEditor(tbRevs.flParticipantConditions.FieldName, env), readOnly: true);
            tbRevs.flRequiredDocuments.RenderCustom(groupbox, env, trade.flRequiredDocuments, customEditor: new RequirementsListEditor(tbRevs.flRequiredDocuments.FieldName, env), readOnly: true);
            tbRevs.flCommissionMembers.RenderCustom(groupbox, env, trade.flCommissionMembers, readOnly: true);
            tbRevs.flNote.RenderCustom(groupbox, env, trade.flNote, readOnly: true);

        }

        public static void viewModelSaleData(WidgetBase widget, FormEnvironment env, HydrocarbonTradeModel trade, bool collapsed = false)
        {
            var tbRevs = new TbTradesRevisions();
            var groupbox = new Accordion(env.T("Информация о продаже"), collapsed);
            widget.AddComponent(groupbox);

            //tbRevs.flWinnerId.RenderCustom(groupbox, env, trade.flWinnerId, readOnly: true);
            tbRevs.flWinnerXin.RenderCustom(groupbox, env, trade.flWinnerXin, readOnly: true);
            //tbRevs.flSaleCondition.RenderCustom(groupbox, env, trade.flSaleCondition, readOnly: true);
            tbRevs.flCost.RenderCustom(groupbox, env, trade.flCost, readOnly: true);
            //tbRevs.flPostControl.RenderCustom(groupbox, env, trade.flPostControl, readOnly: true);

        }

        public static void viewModelProtocolData(WidgetBase widget, FormEnvironment env, HydrocarbonTradeModel trade, bool collapsed = false)
        {
            var tbRevs = new TbTradesRevisions();
            var groupbox = new Accordion(env.T("Протокол"), collapsed);
            widget.AddComponent(groupbox);

            tbRevs.flSaleProtocolDate.RenderCustom(groupbox, env, trade.flSaleProtocolDate, readOnly: true);
            //tbRevs.flSaleProtocolNumber.RenderCustom(groupbox, env, trade.flSaleProtocolNumber, readOnly: true);

        }

        protected override bool HasAccessToOrder(int revisionId, ActionEnv<HydrocarbonTradeOrderQueryArgs> env)
        {
            if ((!env.User.IsExternalUser() && !env.User.IsGuest()))
            {
                return true;
            }
            if (env.Args.MenuAction != "view-order" && !env.User.HasRole("TRADERESOURCES-Недропользование-Создание договора", env.QueryExecuter)/*!env.User.HasPermission(nameof(RegistersModule), RegistersModule.LocalPermissions.Landlords)*/)
            {
                throw new AccessDeniedException();
            }
            var hasRecords = new TbTradesRevisions()
                .AddFilter(t => t.flRevisionId, revisionId)
                .AddFilter(t => t.flCompetentOrgBin, env.User.GetUserBin(env.QueryExecuter))
                .Count(env.QueryExecuter) > 0;

            return hasRecords;
        }
    }

    
    public class HydrocarbonTradeOrderQueryArgs : OrderQueryArgs {
        public int? Id { get; set; }
        public int ObjectId { get; set; }
        public string OwnerXin { get; set; }
        public TradesOrderTypeActions? OrderType { get; set; }
    }
    public enum TradesOrderTypeActions {
        Create,
        Edit,
        Cancel,
        Transfer
    }
}
