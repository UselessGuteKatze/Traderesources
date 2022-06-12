using CommonSource;
using TelecomOperatorsSource.Helpers.Trade;
using TelecomOperatorsSource.Models;
using TelecomOperatorsSource.QueryTables.Common;
using TelecomOperatorsSource.QueryTables.Object;
using TelecomOperatorsSource.QueryTables.Trade;
using TelecomOperatorsSource.References.Object;
using TelecomOperatorsSource.References.Trade;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TradeResourcesPlugin.Helpers;
using TradeResourcesPlugin.Modules.TelecomOperatorsMenus.Objects;
using TradeResourcesPlugin.Modules.LandObjectsMenus.Object;
using UsersResources;
using Yoda.Interfaces;
using Yoda.Interfaces.Forms;
using Yoda.Interfaces.Forms.Components;
using Yoda.Interfaces.Helpers.Mrp.QueryTables;
using Yoda.Interfaces.Helpers.NotWorkingDaysHelper.QueryTables;
using YodaApp.YodaHelpers.OrderHelpers;
using YodaHelpers.ActionMenus;
using YodaHelpers.Components;
using YodaHelpers.DateTimeHelper;
using YodaHelpers.Fields;
using YodaHelpers.OrderHelpers;
using YodaQuery;

namespace TradeResourcesPlugin.Modules.TelecomOperatorsMenus.Trades {
    public class MnuTelecomOperatorsTradeOrder : MnuOrderBaseV2<TelecomOperatorsTradeModel, TradeOrderTypes, TelecomOperatorsTradeOrderQueryArgs> {

        public MnuTelecomOperatorsTradeOrder(string moduleName, OrderStandartPermissions perms)
            : base(moduleName, nameof(MnuTelecomOperatorsTradeOrder), "Приказы по конкурсам", perms, () => new TbTradesOrderResult(), () => new TbTradesOrderNegotiations())
        {
            AsCallback();
            Enabled(rc => {
                var xin = rc.User.GetUserXin(rc.QueryExecuter);
                if (xin == "050540004455"
                || xin == "050540000002"
                || (!rc.User.IsExternalUser() && !rc.User.IsGuest())
                || rc.User.HasRole("TRADERESOURCES-Ресурсы связи-Для операторов связи-Создание приказов", rc.QueryExecuter))
                {
                    return true;
                }
                return false;
            });
        }

        public override Task<ActionItem[]> GetActions(ActionEnv<TelecomOperatorsTradeOrderQueryArgs> env)
        {
            var ret = base.GetActions(env).Result.ToList();

            switch (env.Args.MenuAction)
            {
                case Actions.ViewOrder:
                    var trade = new TbTradesRevisions().AddFilter(t => t.flRevisionId, env.Args.RevisionId)
                        .GetTradeModelFirst(env.QueryExecuter, null);
                    if (trade != null)
                    {
                        ret.Add(new ActionItem(env.T("Перейти к объекту"), new ActionRedirectData(ModuleName, MnuTelecomOperatorsObjectView.MnuName, new TelecomOperatorsObjectViewArgs { MenuAction = "view", Id = trade.flObjectId })));
                    }
                    break;
                case Actions.CreateFrom:
                    break;
            }

            return Task.FromResult(ret.ToArray());
        }

        public override IWizardFormBuilderWithStep<TelecomOperatorsTradeOrderQueryArgs, ModelOrderArgs<TradeOrderTypes, TelecomOperatorsTradeOrderQueryArgs>, TelecomOperatorsTradeModel>
            EditModel(IWizardFormBuilderWithFinishButton<TelecomOperatorsTradeOrderQueryArgs, ModelOrderArgs<TradeOrderTypes, TelecomOperatorsTradeOrderQueryArgs>, TelecomOperatorsTradeModel> wizard)
        {
            return wizard
                .Step("Отмена конкурса", step => step
                .Enabled(env => env.Args.OrderType.In(TradeOrderTypes.Cancel))
                .OnRendering(re => {
                    var trade = re.Model;

                    viewModelMainData(re.Panel, re.Env, trade);
                    viewModelRequisitesData(re.Panel, re.Env, trade);

                    if (!string.IsNullOrEmpty(trade.flWinnerBin))
                    {
                        viewModelSaleData(re.Panel, re.Env, trade);
                    }
                    if (trade.flIsSaleProtocolExists == true)
                    {
                        viewModelProtocolData(re.Panel, re.Env, trade);
                    }
                    if (trade.flIsSaleAgreementExists == true)
                    {
                        viewModelAgreementData(re.Panel, re.Env, trade);
                    }
                })
                .OnValidating(ve => {

                    var ableToEditLastDate = ve.Model.flDateTime.AddWorkdays(-3, ve.Env.QueryExecuter);

                    if (ve.Env.QueryExecuter.GetDateTime(NpGlobal.DbKeys.DbYodaGr) >= ableToEditLastDate && !(!ve.Env.User.IsExternalUser() && !ve.Env.User.IsGuest()))
                    {
                        throw new NotImplementedException(ve.Env.T($"Отменять торги до начала можно лишь ранее чем за 3 дня!"));
                    }
                })
                .OnProcessing(pe => {

                    var ableToEditLastDate = pe.Model.flDateTime.AddWorkdays(-3, pe.Env.QueryExecuter);

                    if (pe.Env.QueryExecuter.GetDateTime(NpGlobal.DbKeys.DbYodaGr) >= ableToEditLastDate && !(!pe.Env.User.IsExternalUser() && !pe.Env.User.IsGuest()))
                    {
                        throw new NotImplementedException(pe.Env.T($"Отменять торги до начала можно лишь ранее чем за 3 дня!"));
                    }
                }))
                .Step("Перенос конкурса", step => step
                .Enabled(env => env.Args.OrderType.In(TradeOrderTypes.Transfer))
                .OnRendering(re => {
                    var tbRev = new TbTradesRevisions();
                    var trade = re.Model;
                    var now = re.Env.QueryExecuter.GetDateTime(NpGlobal.DbKeys.DbYodaGr);
                    var minimumDays = 15;
                    trade.flDateTime = new DateTime(now.Year, now.Month, now.Day, 10, 0, 0).AddDays(minimumDays);
                    tbRev.flDateTime.RenderCustom(re.Panel, re.Env, trade.flDateTime);
                })
                .OnValidating(ve => {
                    var tbRev = new TbTradesRevisions();
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
                    var tbRev = new TbTradesRevisions();
                    var trade = pe.Model;
                    pe.Model.flDateTime = tbRev.flDateTime.GetVal(pe.Env);
                }))
                .Step("Конкурс", step => step
                .Enabled(env => env.Args.OrderType.In(TradeOrderTypes.Create, TradeOrderTypes.Edit))
                .OnRendering(re => {
                    var tbRev = new TbTradesRevisions();
                    var trade = re.Model;

                    tbRev.flObjectId.RenderCustom(re.Panel, re.Env, trade.flObjectId, readOnly: true);
                    tbRev.flCompetentOrgBin.RenderCustom(re.Panel, re.Env, trade.flCompetentOrgBin, readOnly: true);
                    tbRev.flStatus.RenderCustom(re.Panel, re.Env, trade.flStatus, readOnly: true);


                })
                .OnValidating(ve => {
                })
                .OnProcessing(pe => {
                }))
                .Step("Данные", step => step
                .Enabled(env => env.Args.OrderType.In(TradeOrderTypes.Create, TradeOrderTypes.Edit))
                .OnRendering(re => {
                    var tbRev = new TbTradesRevisions();
                    var trade = re.Model;

                    tbRev.flCostStart.RenderCustom(re.Panel, re.Env, trade.flCostStart);
                    //tbRev.flDeposit.RenderCustom(re.Panel, re.Env, trade.flDeposit);
                    tbRev.flDateTime.RenderCustom(re.Panel, re.Env, trade.flDateTime);
                    tbRev.flNote.RenderCustom(re.Panel, re.Env, trade.flNote);

                })
                .OnValidating(ve =>
                {
                    var tbRev = new TbTradesRevisions();
                    var trade = ve.Model;

                    var checkFields = new Field[] {
                            tbRev.flCostStart,
                            //tbRev.flDeposit,
                            tbRev.flDateTime,
                            tbRev.flNote
                        };
                    checkFields.Each(f => f.Validate(ve.Env));

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
                    var tbRev = new TbTradesRevisions();
                    var trade = pe.Model;

                    pe.Model.flCostStart = tbRev.flCostStart.GetVal(pe.Env);
                    pe.Model.flDateTime = tbRev.flDateTime.GetVal(pe.Env);
                    pe.Model.flNote = tbRev.flNote.GetVal(pe.Env);

                    var tbMrp = new TbMrp();
                    tbMrp.AddFilter(tbMrp.flYear, trade.flDateTime.Year);
                    var hasMrp = tbMrp.Count(pe.Env.QueryExecuter) > 0;

                    if (!hasMrp)
                    {
                        var yearMrpMessage = pe.Env.T("В базе не найдено значение мрп на @@@year год").Text;
                        yearMrpMessage = yearMrpMessage.Replace("@@@year", tbRev.flDateTime.GetVal(pe.Env).Year.ToString());
                        pe.Env.AddError(tbRev.flDateTime.FieldName, yearMrpMessage);
                    }
                    else
                    {
                        var mrp = Convert.ToDecimal(tbMrp.SelectScalar(tbMrp.flMrp, pe.Env.QueryExecuter));
                        var mrp30000 = 30000 * mrp;

                        pe.Model.flDeposit = ((trade.flCostStart / 100) * 15 > mrp30000 ? mrp30000 : (trade.flCostStart / 100) * 15);
                    }

                }))
                .Step("Гарантийный взнос", step => step
                .Enabled(env => env.Args.OrderType.In(TradeOrderTypes.Create, TradeOrderTypes.Edit))
                .OnRendering(re => {
                    var tbRev = new TbTradesRevisions();
                    var trade = re.Model;

                    var tbMrp = new TbMrp();
                    tbMrp.AddFilter(tbMrp.flYear, trade.flDateTime.Year);
                    var mrp = Convert.ToDecimal(tbMrp.SelectScalar(tbMrp.flMrp, re.Env.QueryExecuter));
                    var mrp30000 = 30000 * mrp;

                    tbRev.flDeposit.RenderCustom(re.Panel, re.Env, trade.flDeposit, readOnly: true,
                        description: new Label($"({re.Env.T("Начальная цена, тг.")} * 15% ({re.Env.T("максимум")} 30000 {re.Env.T("мрп")} {mrp30000} {re.Env.T("тг.")}, 1 {re.Env.T("мрп")} = {mrp} {re.Env.T("тг.")}))")
                    );
                })
                .OnValidating(ve =>
                {

                })
                .OnProcessing(pe => {
                    
                }))
            #region Конкурс

                //.Step("Условия конкурса", step => step
                //.Enabled(env => env.Args.OrderType.In(TradeOrderTypes.Create, TradeOrderTypes.Edit))
                //.OnRendering(re => {
                //    var tbRev = new TbTradesRevisions();
                //    var trade = re.Model;

                //    tbRev.flParticipantConditions.RenderCustom(re.Panel, re.Env, trade.flParticipantConditions);
                //    tbRev.flRequiredDocuments.RenderCustomT(re.Panel, re.Env, trade.flRequiredDocuments);
                //    //tbRev.flAdditionalCondition.RenderCustomT(re.Panel, re.Env, trade.flAdditionalCondition);
                //    tbRev.flCommissionMembers.RenderCustom(re.Panel, re.Env, trade.flCommissionMembers);

                //})
                //.OnValidating(ve => {
                //    var tbRev = new TbTradesRevisions();
                //    var trade = ve.Model;
                //    var checkFields = new Field[] {
                //            tbRev.flParticipantConditions,
                //            tbRev.flRequiredDocuments,
                //            //tbRev.flAdditionalCondition,
                //            tbRev.flCommissionMembers
                //        };
                //    checkFields.Each(f => f.Validate(ve.Env));

                //    if (ve.Env.IsValid)
                //    {
                //        var flCommissionMembers = (Dictionary<string, object>[])tbRev.flCommissionMembers.GetValue(ve.Env);

                //        if (flCommissionMembers.Length < 5)
                //        {
                //            ve.Env.AddError(tbRev.flCommissionMembers.FieldName, ve.Env.T("Должно быть как минимум 5 членов комиссии"));
                //        }

                //        if (flCommissionMembers.Length > flCommissionMembers.Select(x => x[CommissionMembersEditor.Fls.flUserId.ToString()].ToString()).Distinct().Count())
                //        {
                //            ve.Env.AddError(tbRev.flCommissionMembers.FieldName, ve.Env.T("Один человек не может исполнять несколько ролей"));
                //        }

                //        if (!flCommissionMembers.Any(x =>
                //        {
                //            return x[CommissionMembersEditor.Fls.flRole.ToString()].ToString() == CommissionMembersRoles.SCR.ToString();
                //        }))
                //        {
                //            ve.Env.AddError(tbRev.flCommissionMembers.FieldName, ve.Env.T("В комисси должен быть секретарь"));
                //        }
                //    }

                //})
                //.OnProcessing(pe => {
                //    var tbRev = new TbTradesRevisions();

                //    var flParticipantConditions = (Dictionary<string, object>[])tbRev.flParticipantConditions.GetValue(pe.Env);
                //    var flRequiredDocuments = (Dictionary<string, object>[])tbRev.flRequiredDocuments.GetValue(pe.Env);
                //    var flCommissionMembers = (Dictionary<string, object>[])tbRev.flCommissionMembers.GetValue(pe.Env);

                //    pe.Model.flParticipantConditions = flParticipantConditions;
                //    pe.Model.flRequiredDocuments = flRequiredDocuments;
                //    //pe.Model.flAdditionalCondition = tbRev.flAdditionalCondition.GetValue(pe.Env).ToString();
                //    pe.Model.flCommissionMembers = flCommissionMembers;

                //}))

            #endregion

                .Step("Реквизиты", step => step
                .Enabled(env => env.Args.OrderType.In(TradeOrderTypes.Create, TradeOrderTypes.Edit))
                .OnRendering(re => {
                    var tbRev = new TbTradesRevisions();
                    var trade = re.Model;

                    tbRev.flTaxAuthorityBin.RenderCustom(re.Panel, re.Env, trade.flTaxAuthorityBin);
                    tbRev.flBik.RenderCustom(re.Panel, re.Env, "KKMFKZ2A", readOnly: true);
                    tbRev.flIik.RenderCustom(re.Panel, re.Env, "KZ24070105KSN0000000", readOnly: true);
                    tbRev.flKnp.RenderCustom(re.Panel, re.Env, trade.flKnp);
                    tbRev.flKbk.RenderCustom(re.Panel, re.Env, trade.flKbk);
                    tbRev.flKbe.RenderCustom(re.Panel, re.Env, trade.flKbe);

                })
                .OnValidating(ve => {
                    var tbRev = new TbTradesRevisions();
                    var checkFields = new Field[] {
                            tbRev.flTaxAuthorityBin,
                            tbRev.flKnp,
                            tbRev.flKbk,
                            tbRev.flKbe,
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
                    var tbRev = new TbTradesRevisions();
                    pe.Model.flTaxAuthorityBin = tbRev.flTaxAuthorityBin.GetVal(pe.Env);
                    pe.Model.flBik = "KKMFKZ2A";
                    pe.Model.flIik = "KZ24070105KSN0000000";
                    pe.Model.flKnp = tbRev.flKnp.GetVal(pe.Env);
                    pe.Model.flKbk = tbRev.flKbk.GetVal(pe.Env);
                    pe.Model.flKbe = tbRev.flKbe.GetVal(pe.Env);
                }))
                ;
        }


        public override TelecomOperatorsTradeModel GetModel(ActionEnv<TelecomOperatorsTradeOrderQueryArgs> env, GetModelFrom from, TradeOrderTypes orderType, ITransaction transaction)
        {
            var now = env.QueryExecuter.GetDateTime(NpGlobal.DbKeys.DbYodaGr);
            var tradeDate = new DateTime(now.Year, now.Month, now.Day, 10, 0, 0).AddWorkdays(15, env.QueryExecuter);

            var model = from switch
            {
                GetModelFrom.Empty => new TelecomOperatorsTradeModel()
                {
                    flCompetentOrgBin = env.Args.OwnerXin,
                    flDateTime = tradeDate,
                    flStatus = RefTradesStatuses.Wait.ToString(),
                    flObjectId = env.Args.ObjectId
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
                    model.flStatus = RefTradesStatuses.Wait.ToString();
                    break;
                case TradeOrderTypes.Cancel:
                    model.flStatus = RefTradesStatuses.CanceledEarlier.ToString();
                    break;
                default:
                    // Status is not changing
                    break;
            }

            return model;
        }

        public override TradeOrderTypes GetOrderType(ActionEnv<TelecomOperatorsTradeOrderQueryArgs> env, GetOrderModelFrom orderModelFrom)
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

        public override string GetWizardCancelRedirectUrl(ActionEnv<TelecomOperatorsTradeOrderQueryArgs> env, TradeOrderTypes orderType, GetModelFrom modelFrom)
        {
            var urlHelper = env.RequestContext.GetUrlHelper();
            //TODO
            return modelFrom switch
            {
                GetModelFrom.Empty => urlHelper.YodaAction(ModuleName, nameof(MnuTelecomOperatorsTradeOrder)),
                GetModelFrom.Entity => urlHelper.YodaAction(ModuleName, nameof(MnuTelecomOperatorsTradeOrder), new TelecomOperatorsTradeOrderQueryArgs { Id = env.Args.Id.Value, MenuAction = "view" }),
                GetModelFrom.Revision => urlHelper.YodaAction(ModuleName, MenuName, new TelecomOperatorsTradeOrderQueryArgs { RevisionId = env.Args.RevisionId, MenuAction = Actions.ViewOrder }),
                _ => throw new NotImplementedException()
            };
        }

        public override ValidationResultBase IsCreateFromActionValid(ActionEnv<TelecomOperatorsTradeOrderQueryArgs> env) {
            if (env.Args.Id == null) {
                return new RedirectResult(new ArgumentException("Id"));
            }

            if (env.Args.OrderType == null) {
                return new RedirectResult(new ArgumentException("OrderType"));
            }

            if (!CanCreateOrder(env.Args.Id.Value, GetOrderType(env, GetOrderModelFrom.Entity), out var error, env)) {
                if (env.Args.Id.HasValue) {
                    return new RedirectResult(ModuleName, MnuTelecomOperatorsTradeView.MnuName, new MnuTelecomOperatorsTradeViewArgs() { tradeId = env.Args.Id.Value }, error);
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

        public static bool CanCreateOrder(int id, TradeOrderTypes orderType, out string error, ActionEnv<TelecomOperatorsTradeOrderQueryArgs> env)
        {
            if (TryGetCurrentInWorkOrderRevId(id, out _, env.QueryExecuter))
            {
                error = env.T("Для данного объекта имеется не исполненый приказ");
                return false;
            }

            var tradeResourceObject = TradeHelper.GetTradeModel(id, env.QueryExecuter);


            bool checkTradeWaiting(out string err)
            {
                if (tradeResourceObject.flStatus != RefTradesStatuses.Wait.ToString())
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

        public override void SaveModel(int revisionId, TelecomOperatorsTradeModel model, TradeOrderTypes orderType, ActionEnv<TelecomOperatorsTradeOrderQueryArgs> env, ITransaction transaction, GetModelFrom modelFrom)
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
        private void setObjectStatus(TradeOrderTypes orderType, TelecomOperatorsTradeModel model, ActionEnv<TelecomOperatorsTradeOrderQueryArgs> env, ITransaction transaction)
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

            var objectBloc = TelecomOperatorsObjectBlocks.ActiveFree.ToString();
            if (orderType == TradeOrderTypes.Create)
            {
                objectBloc = TelecomOperatorsObjectBlocks.ActiveToBeTraded.ToString();
            }
            if (orderType == TradeOrderTypes.Cancel)
            {
                objectBloc = TelecomOperatorsObjectBlocks.ActiveFree.ToString();
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

        private void setModel<TTradesTable>(DataModifingQueryProxy<TTradesTable> updateOrInsert, TelecomOperatorsTradeModel model) where TTradesTable : TbTradesBase
        {
            updateOrInsert
                .SetT(t => t.flId, model.flId)
                .SetT(t => t.flRevisionId, model.flRevisionId)
                .SetT(t => t.flObjectId, model.flObjectId)
                .Set(t => t.flStatus, model.flStatus)

                .SetT(t => t.flCompetentOrgBin, model.flCompetentOrgBin)
                .SetT(t => t.flDeposit, model.flDeposit)
                .SetT(t => t.flCostStart, model.flCostStart)
                .SetT(t => t.flDateTime, model.flDateTime)
                .SetT(t => t.flNote, model.flNote)

                //.SetT(t => t.flWinnerId, model.flWinnerId)
                //.SetT(t => t.flWinnerData, model.flWinnerData)
                //.SetT(t => t.flWinnerBin, model.flWinnerBin)
                //.SetT(t => t.flSaleCondition, model.flSaleCondition)
                //.SetT(t => t.flCost, model.flCost)
                //.Set(t => t.flPostControl, model.flPostControl)

                //.SetT(t => t.flIsSaleAgreementExists, model.flIsSaleAgreementExists)
                //.SetT(t => t.flSaleAgreementDate, model.flSaleAgreementDate)
                //.SetT(t => t.flSaleAgreementNumber, model.flSaleAgreementNumber)
                //.SetT(t => t.flPrepayment, model.flPrepayment)
                //.SetT(t => t.flRemainingSum, model.flRemainingSum)
                //.SetT(t => t.flRemainingSumDate, model.flRemainingSumDate)

                //.SetT(t => t.flIsSaleProtocolExists, model.flIsSaleProtocolExists)
                //.SetT(t => t.flSaleProtocolDate, model.flSaleProtocolDate)
                //.Set(t => t.flAdditionalCondition, "NotAllowed")
                //.SetT(t => t.flSaleProtocolNumber, model.flSaleProtocolNumber)


                //.SetT(t => t.flPublications, model.flPublications)
                //.SetT(t => t.flRequiredDocuments, model.flRequiredDocuments)
                //.SetT(t => t.flParticipantConditions, model.flParticipantConditions)
                //.SetT(t => t.flCommissionMembers, model.flCommissionMembers)

                .SetT(t => t.flTaxAuthorityBin, model.flTaxAuthorityBin)
                .SetT(t => t.flBik, model.flBik)
                .SetT(t => t.flIik, model.flIik)
                .SetT(t => t.flKnp, model.flKnp)
                .Set(t => t.flKbk, model.flKbk)
                .SetT(t => t.flKbe, model.flKbe)

                .SetT(t => t.flId, model.flId);
        }

        public override bool TryAcceptModel(int revisionId, TelecomOperatorsTradeModel model, TradeOrderTypes orderType, ActionEnv<TelecomOperatorsTradeOrderQueryArgs> env, ITransaction transaction, out string error)
        {
            var result = true;
            var errorsList = new List<string>() { };

            if (orderType == TradeOrderTypes.Create)
            {
                if (new TbTrades().AddFilter(t => t.flObjectId, model.flObjectId).AddFilter(t => t.flStatus, RefTradesStatuses.Wait).Count(env.QueryExecuter) > 0)
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

        public override bool TryValidateModel(int revisionId, TelecomOperatorsTradeModel model, TradeOrderTypes orderType, ActionEnv<TelecomOperatorsTradeOrderQueryArgs> env, out string[] errors)
        {
            var result = true;
            var errorsList = new List<string>() { };

            if (orderType == TradeOrderTypes.Create)
            {
                if (new TbTrades().AddFilter(t => t.flObjectId, model.flObjectId).AddFilter(t => t.flStatus, RefTradesStatuses.Wait).Count(env.QueryExecuter) > 0)
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

        private void setTradeChange(TradeOrderTypes orderType, TelecomOperatorsTradeModel model, ActionEnv<TelecomOperatorsTradeOrderQueryArgs> env, ITransaction transaction)
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

        public override void ViewModel(RenderActionEnv<TelecomOperatorsTradeOrderQueryArgs> env, TelecomOperatorsTradeModel trade, TradeOrderTypes orderType, OrderStatus orderStatus)
        {
            ViewModel(env.Form, env, trade);
        }

        public static void ViewModel(WidgetBase widget, FormEnvironment env, TelecomOperatorsTradeModel trade)
        {
            viewModelMainData(widget, env, trade);

            viewModelRequisitesData(widget, env, trade);

            if (!string.IsNullOrEmpty(trade.flWinnerBin))
            {
                viewModelSaleData(widget, env, trade);
            }
            if (trade.flIsSaleProtocolExists == true)
            {
                viewModelProtocolData(widget, env, trade);
            }
            if (trade.flIsSaleAgreementExists == true)
            {
                viewModelAgreementData(widget, env, trade);
            }

        }

        public static void viewModelMainData(WidgetBase widget, FormEnvironment env, TelecomOperatorsTradeModel trade, bool collapsed = false)
        {
            var tbRevs = new TbTradesRevisions();
            var groupbox = new Accordion(env.T("Данные по конкурсу"), collapsed);
            widget.AddComponent(groupbox);

            tbRevs.flId.RenderCustom(groupbox, env, trade.flId.ToString() ?? env.T("*Генерируется автоматически при исполнении приказа"), readOnly: true);
            tbRevs.flRevisionId.RenderCustom(groupbox, env, trade.flRevisionId, readOnly: true);

            tbRevs.flObjectId.RenderCustom(groupbox, env, trade.flObjectId, readOnly: true);
            tbRevs.flCompetentOrgBin.RenderCustom(groupbox, env, trade.flCompetentOrgBin, readOnly: true);
            tbRevs.flStatus.RenderCustom(groupbox, env, trade.flStatus, readOnly: true);

            tbRevs.flCostStart.RenderCustom(groupbox, env, trade.flCostStart, readOnly: true);


            var tbMrp = new TbMrp();
            tbMrp.AddFilter(tbMrp.flYear, trade.flDateTime.Year);
            var mrp = Convert.ToDecimal(tbMrp.SelectScalar(tbMrp.flMrp, env.QueryExecuter));
            var mrp30000 = 30000 * mrp;
            tbRevs.flDeposit.RenderCustom(groupbox, env, trade.flDeposit, readOnly: true,
                description: new Label($"({env.T("Начальная цена, тг.")} * 15% ({env.T("максимум")} 30000 {env.T("мрп")} {mrp30000} {env.T("тг.")}, 1 {env.T("мрп")} = {mrp} {env.T("тг.")}))")
            );


            tbRevs.flDateTime.RenderCustom(groupbox, env, trade.flDateTime, readOnly: true);
            tbRevs.flNote.RenderCustom(groupbox, env, trade.flNote, readOnly: true);

        }

        public static void viewModelRequisitesData(WidgetBase widget, FormEnvironment env, TelecomOperatorsTradeModel trade, bool collapsed = false)
        {
            var tbRevs = new TbTradesRevisions();
            var groupbox = new Accordion(env.T("Реквизиты"), collapsed);
            widget.AddComponent(groupbox);

            tbRevs.flTaxAuthorityBin.RenderCustom(groupbox, env, trade.flTaxAuthorityBin, readOnly: true);
            tbRevs.flBik.RenderCustom(groupbox, env, trade.flBik, readOnly: true);
            tbRevs.flIik.RenderCustom(groupbox, env, trade.flIik, readOnly: true);
            tbRevs.flKnp.RenderCustom(groupbox, env, trade.flKnp, readOnly: true);
            tbRevs.flKbk.RenderCustom(groupbox, env, trade.flKbk, readOnly: true);
            tbRevs.flKbe.RenderCustom(groupbox, env, trade.flKbe, readOnly: true);

        }

        public static void viewModelAgreementData(WidgetBase widget, FormEnvironment env, TelecomOperatorsTradeModel trade, bool collapsed = false)
        {
            var tbRevs = new TbTradesRevisions();
            var groupbox = new Accordion(env.T("Договор"), collapsed);
            widget.AddComponent(groupbox);

            tbRevs.flSaleAgreementDate.RenderCustom(groupbox, env, trade.flSaleAgreementDate, readOnly: true);
            tbRevs.flSaleAgreementNumber.RenderCustom(groupbox, env, trade.flSaleAgreementNumber, readOnly: true);
            //tbRevs.flPrepayment.RenderCustom(groupbox, env, trade.flPrepayment, readOnly: true);
            //tbRevs.flRemainingSum.RenderCustom(groupbox, env, trade.flRemainingSum, readOnly: true);
            //tbRevs.flRemainingSumDate.RenderCustom(groupbox, env, trade.flRemainingSumDate, readOnly: true);

        }

        public static void viewModelSaleData(WidgetBase widget, FormEnvironment env, TelecomOperatorsTradeModel trade, bool collapsed = false)
        {
            var tbRevs = new TbTradesRevisions();
            var groupbox = new Accordion(env.T("Информация о продаже"), collapsed);
            widget.AddComponent(groupbox);

            //tbRevs.flWinnerId.RenderCustom(groupbox, env, trade.flWinnerId, readOnly: true);
            tbRevs.flWinnerBin.RenderCustom(groupbox, env, trade.flWinnerBin, readOnly: true);
            //tbRevs.flSaleCondition.RenderCustom(groupbox, env, trade.flSaleCondition, readOnly: true);
            tbRevs.flCost.RenderCustom(groupbox, env, trade.flCost, readOnly: true);
            //tbRevs.flPostControl.RenderCustom(groupbox, env, trade.flPostControl, readOnly: true);

        }

        public static void viewModelProtocolData(WidgetBase widget, FormEnvironment env, TelecomOperatorsTradeModel trade, bool collapsed = false)
        {
            var tbRevs = new TbTradesRevisions();
            var groupbox = new Accordion(env.T("Протокол"), collapsed);
            widget.AddComponent(groupbox);

            tbRevs.flSaleProtocolDate.RenderCustom(groupbox, env, trade.flSaleProtocolDate, readOnly: true);
            //tbRevs.flSaleProtocolNumber.RenderCustom(groupbox, env, trade.flSaleProtocolNumber, readOnly: true);

        }

        protected override bool HasAccessToOrder(int revisionId, ActionEnv<TelecomOperatorsTradeOrderQueryArgs> env)
        {
            if ((!env.User.IsExternalUser() && !env.User.IsGuest()))
            {
                return true;
            }
            if (env.Args.MenuAction != "view-order" && !env.User.HasRole("TRADERESOURCES-Лесные ресурсы-Выставление на торги", env.QueryExecuter)/*!env.User.HasCustomRole("TelecomOperatorsobjects", "dataEdit", env.QueryExecuter)*/)
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

    
    public class TelecomOperatorsTradeOrderQueryArgs : OrderQueryArgs {
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
