using CommonSource;
using HydrocarbonSource.Helpers.Trade;
using HydrocarbonSource.Models;
using HydrocarbonSource.QueryTables.Trade;
using HydrocarbonSource.References.Trade;
using System;
using TradeResourcesPlugin.Helpers;
using TradeResourcesPlugin.Modules.HydrocarbonMenus.Objects;
using UsersResources;
using Yoda.Interfaces;
using Yoda.Interfaces.Forms;
using Yoda.Interfaces.Forms.Components;
using Yoda.Interfaces.Menu;
using YodaHelpers.DateTimeHelper;
using YodaQuery;

namespace TradeResourcesPlugin.Modules.HydrocarbonMenus.Trades {
    public class MnuHydrocarbonTradeView : FrmMenu<MnuHydrocarbonTradeViewArgs> {
        public static string MnuName = nameof(MnuHydrocarbonTradeView);
        public MnuHydrocarbonTradeView(string moduleName) : base(MnuName, "Конкурсы")
        {
            AsCallback();
            Enabled(rc =>
            {
                var xin = rc.User.GetUserXin(rc.QueryExecuter);
                if (xin == "050540004455"
                || xin == "050540000002"
                || (!rc.User.IsExternalUser() && !rc.User.IsGuest())
                || rc.User.HasRole("TRADERESOURCES-Недропользование-Создание приказов", rc.QueryExecuter)/*rc.User.HasCustomRole("traderesources", "view", rc.QueryExecuter)*/
                /*|| rc.User.HasPermission(nameof(RegistersModule), RegistersModule.LocalPermissions.Landlords)*/)
                {
                    return true;
                }
                return false;
            });
            OnRendering(re =>
            {
                var tradeActRev = TradeHelper.GetTradeModel(re.Args.tradeId, re.QueryExecuter);
                RenderRedirectButtons(re, tradeActRev);
                MnuHydrocarbonTradeOrder.ViewModel(re.Form, re.AsFormEnv(), tradeActRev);
            });

            void RenderRedirectButtons(FrmRenderEnvironment<MnuHydrocarbonTradeViewArgs> re, HydrocarbonTradeModel trade) {

                re.RequestContext.AddLocalTask(new Link {
                    Text = re.T("Перейти к объекту"),
                    Controller = moduleName,
                    Action = MnuHydrocarbonObjectView.MnuName,
                    RouteValues = new HydrocarbonObjectViewArgs { MenuAction = "view", Id = trade.flObjectId }
                });

                if (!re.User.HasRole("TRADERESOURCES-Недропользование-Создание приказов", re.QueryExecuter))
                {
                    return;
                }

                var ableToEditLastDate = trade.flDateTime.AddWorkdays(-3, re.QueryExecuter);

                var tradeRevisions = new TbTradesRevisions();
                tradeRevisions.AddFilter(t => t.flId, trade.flId);
                var revisionResults = new TbTradesOrderResult();
                revisionResults.AddFilterNot(t => t.flStatus, "Canceled");
                var join = tradeRevisions
                    .Join("tradeRevisions", revisionResults, "revisionResults")
                    .On(new Condition(tradeRevisions.flRevisionId, revisionResults.flSubjectId));
                join.OrderBy = new[] { new OrderField(tradeRevisions.flRevisionId, OrderType.Desc) };

                var lastRevision = Convert.ToInt32(join.SelectScalar(tradeRevisions.flRevisionId, re.QueryExecuter));

                var now = re.QueryExecuter.GetDateTime(NpGlobal.DbKeys.DbYodaGr);

                if (re.RequestContext.Project == "cabinetResourceSeller"
                    && trade.flStatus == HydrocarbonTradeStatuses.Wait
                    && now <= ableToEditLastDate)
                {
                    if (lastRevision == trade.flRevisionId)
                    {
                        re.RequestContext.AddLocalTask(new Link
                        {
                            Text = re.T("Создать приказ на корректировку"),
                            Controller = moduleName,
                            Action = nameof(MnuHydrocarbonTradeOrder),
                            RouteValues = new HydrocarbonTradeOrderQueryArgs { Id = trade.flId, RevisionId = trade.flRevisionId, MenuAction = MnuHydrocarbonTradeOrder.Actions.CreateFrom, OrderType = TradesOrderTypeActions.Edit }
                        });
                        re.RequestContext.AddLocalTask(new Link
                        {
                            Text = re.T("Отменить до начала"),
                            Controller = moduleName,
                            Action = nameof(MnuHydrocarbonTradeOrder),
                            RouteValues = new HydrocarbonTradeOrderQueryArgs { Id = trade.flId, RevisionId = trade.flRevisionId, MenuAction = MnuHydrocarbonTradeOrder.Actions.CreateFrom, OrderType = TradesOrderTypeActions.Cancel }
                        });
                    }
                    else
                    {
                        re.RequestContext.AddLocalTask(new Link
                        {
                            Text = re.T("Открыть неисполненный приказ на корректировку"),
                            Controller = moduleName,
                            Action = nameof(MnuHydrocarbonTradeOrder),
                            RouteValues = new HydrocarbonTradeOrderQueryArgs { Id = trade.flId, RevisionId = lastRevision, MenuAction = MnuHydrocarbonTradeOrder.Actions.ViewOrder }
                        });
                    }
                }
                else if ((!re.User.IsExternalUser() && !re.User.IsGuest())
                        && trade.flStatus == HydrocarbonTradeStatuses.Wait)
                {
                    if (lastRevision == trade.flRevisionId)
                    {
                        re.RequestContext.AddLocalTask(new Link
                        {
                            Text = re.T("Отменить до начала (Внутренний пользователь)"),
                            Controller = moduleName,
                            Action = nameof(MnuHydrocarbonTradeOrder),
                            RouteValues = new HydrocarbonTradeOrderQueryArgs { Id = trade.flId, RevisionId = trade.flRevisionId, MenuAction = MnuHydrocarbonTradeOrder.Actions.CreateFrom, OrderType = TradesOrderTypeActions.Cancel }
                        });
                    }
                    else
                    {
                        re.RequestContext.AddLocalTask(new Link
                        {
                            Text = re.T("Открыть неисполненный приказ на корректировку (Внутренний пользователь)"),
                            Controller = moduleName,
                            Action = nameof(MnuHydrocarbonTradeOrder),
                            RouteValues = new HydrocarbonTradeOrderQueryArgs { Id = trade.flId, RevisionId = lastRevision, MenuAction = MnuHydrocarbonTradeOrder.Actions.ViewOrder }
                        });
                    }
                }

                if (re.RequestContext.Project == "cabinetResourceSeller"
                    && trade.flStatus == HydrocarbonTradeStatuses.Wait
                    && ableToEditLastDate < now && now < trade.flDateTime)
                {
                    if (lastRevision == trade.flRevisionId)
                    {
                        re.RequestContext.AddLocalTask(new Link
                        {
                            Text = re.T("Создать приказ на перенос"),
                            Controller = moduleName,
                            Action = nameof(MnuHydrocarbonTradeOrder),
                            RouteValues = new HydrocarbonTradeOrderQueryArgs { Id = trade.flId, RevisionId = trade.flRevisionId, MenuAction = MnuHydrocarbonTradeOrder.Actions.CreateFrom, OrderType = TradesOrderTypeActions.Transfer }
                        });
                    }
                    else
                    {
                        re.RequestContext.AddLocalTask(new Link
                        {
                            Text = re.T("Открыть неисполненный приказ на корректировку"),
                            Controller = moduleName,
                            Action = nameof(MnuHydrocarbonTradeOrder),
                            RouteValues = new HydrocarbonTradeOrderQueryArgs { Id = trade.flId, RevisionId = lastRevision, MenuAction = MnuHydrocarbonTradeOrder.Actions.ViewOrder }
                        });
                    }
                }

            }

        }
    }

    public class MnuHydrocarbonTradeViewArgs {
        public int tradeId { get; set; }
    }
}
