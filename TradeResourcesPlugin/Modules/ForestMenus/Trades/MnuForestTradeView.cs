using CommonSource;
using ForestSource.Helpers.Trade;
using ForestSource.Models;
using ForestSource.QueryTables.Trade;
using ForestSource.References.Trade;
using System;
using TradeResourcesPlugin.Helpers;
using TradeResourcesPlugin.Modules.ForestMenus.Objects;
using UsersResources;
using Yoda.Interfaces;
using Yoda.Interfaces.Forms;
using Yoda.Interfaces.Forms.Components;
using Yoda.Interfaces.Menu;
using YodaHelpers.DateTimeHelper;
using YodaQuery;

namespace TradeResourcesPlugin.Modules.ForestMenus.Trades {
    public class MnuForestTradeView : FrmMenu<MnuForestTradeViewArgs> {

        public static string MnuName = nameof(MnuForestTradeView);

        public MnuForestTradeView(string moduleName) : base(MnuName, "Конкурсы")
        {
            AsCallback();
            Enabled(rc =>
            {
                var xin = rc.User.GetUserXin(rc.QueryExecuter);
                if (xin == "050540004455"
                || xin == "050540000002"
                || (!rc.User.IsExternalUser() && !rc.User.IsGuest())
                || rc.User.HasRole("TRADERESOURCES-Лесные ресурсы-Выставление на торги", rc.QueryExecuter)/*rc.User.HasCustomRole("forestobjects", "dataView", rc.QueryExecuter)*/
                || rc.User.HasRole("TRADERESOURCES-Лесные ресурсы-Создание объектов", rc.QueryExecuter)/*rc.User.HasCustomRole("forestobjects", "dataView", rc.QueryExecuter)*/
                /*|| rc.User.HasCustomRole("forestobjects", "dataEdit", rc.QueryExecuter)*/)
                {
                    return true;
                }
                return false;
            });
            OnRendering(re =>
            {
                var tradeActRev = TradeHelper.GetTradeModel(re.Args.tradeId, re.QueryExecuter);
                RenderRedirectButtons(re, tradeActRev);
                MnuForestTradeOrder.ViewModel(re.Form, re.AsFormEnv(), tradeActRev);
            });

            void RenderRedirectButtons(FrmRenderEnvironment<MnuForestTradeViewArgs> re, ForestTradeModel trade)
            {
                if (!re.User.HasRole("TRADERESOURCES-Лесные ресурсы-Выставление на торги", re.QueryExecuter) && re.User.IsExternalUser())
                {
                    return;
                }

                re.RequestContext.AddLocalTask(new Link
                {
                    Text = re.T("Перейти к объекту"),
                    Controller = moduleName,
                    Action = MnuForestObjectView.MnuName,
                    RouteValues = new ForestObjectViewArgs { MenuAction = "view", Id = trade.flObjectId }
                });

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
                    && trade.flStatus == TradesStatuses.Wait
                    && now <= ableToEditLastDate)
                {
                    if (lastRevision == trade.flRevisionId)
                    {
                        re.RequestContext.AddLocalTask(new Link
                        {
                            Text = re.T("Создать приказ на корректировку"),
                            Controller = moduleName,
                            Action = nameof(MnuForestTradeOrder),
                            RouteValues = new ForestTradeOrderQueryArgs { Id = trade.flId, RevisionId = trade.flRevisionId, MenuAction = MnuForestTradeOrder.Actions.CreateFrom, OrderType = TradesOrderTypeActions.Edit }
                        });
                        re.RequestContext.AddLocalTask(new Link
                        {
                            Text = re.T("Отменить до начала"),
                            Controller = moduleName,
                            Action = nameof(MnuForestTradeOrder),
                            RouteValues = new ForestTradeOrderQueryArgs { Id = trade.flId, RevisionId = trade.flRevisionId, MenuAction = MnuForestTradeOrder.Actions.CreateFrom, OrderType = TradesOrderTypeActions.Cancel }
                        });
                    }
                    else
                    {
                        re.RequestContext.AddLocalTask(new Link
                        {
                            Text = re.T("Открыть неисполненный приказ на корректировку"),
                            Controller = moduleName,
                            Action = nameof(MnuForestTradeOrder),
                            RouteValues = new ForestTradeOrderQueryArgs { Id = trade.flId, RevisionId = lastRevision, MenuAction = MnuForestTradeOrder.Actions.ViewOrder }
                        });
                    }
                }
                else if ((!re.User.IsExternalUser() && !re.User.IsGuest())
                        && trade.flStatus == TradesStatuses.Wait)
                {
                    if (lastRevision == trade.flRevisionId)
                    {
                        re.RequestContext.AddLocalTask(new Link
                        {
                            Text = re.T("Отменить до начала (Внутренний пользователь)"),
                            Controller = moduleName,
                            Action = nameof(MnuForestTradeOrder),
                            RouteValues = new ForestTradeOrderQueryArgs { Id = trade.flId, RevisionId = trade.flRevisionId, MenuAction = MnuForestTradeOrder.Actions.CreateFrom, OrderType = TradesOrderTypeActions.Cancel }
                        });
                    }
                    else
                    {
                        re.RequestContext.AddLocalTask(new Link
                        {
                            Text = re.T("Открыть неисполненный приказ на корректировку (Внутренний пользователь)"),
                            Controller = moduleName,
                            Action = nameof(MnuForestTradeOrder),
                            RouteValues = new ForestTradeOrderQueryArgs { Id = trade.flId, RevisionId = lastRevision, MenuAction = MnuForestTradeOrder.Actions.ViewOrder }
                        });
                    }
                }

                if (re.RequestContext.Project == "cabinetResourceSeller"
                    && trade.flStatus == TradesStatuses.Wait
                    && ableToEditLastDate < now && now < trade.flDateTime)
                {
                    if (lastRevision == trade.flRevisionId)
                    {
                        re.RequestContext.AddLocalTask(new Link
                        {
                            Text = re.T("Создать приказ на перенос"),
                            Controller = moduleName,
                            Action = nameof(MnuForestTradeOrder),
                            RouteValues = new ForestTradeOrderQueryArgs { Id = trade.flId, RevisionId = trade.flRevisionId, MenuAction = MnuForestTradeOrder.Actions.CreateFrom, OrderType = TradesOrderTypeActions.Transfer }
                        });
                    }
                    else
                    {
                        re.RequestContext.AddLocalTask(new Link
                        {
                            Text = re.T("Открыть неисполненный приказ на корректировку"),
                            Controller = moduleName,
                            Action = nameof(MnuForestTradeOrder),
                            RouteValues = new ForestTradeOrderQueryArgs { Id = trade.flId, RevisionId = lastRevision, MenuAction = MnuForestTradeOrder.Actions.ViewOrder }
                        });
                    }
                }

            }

        }
    }

    public class MnuForestTradeViewArgs {
        public int tradeId { get; set; }
    }
}
