using CommonSource;
using TelecomOperatorsSource.Helpers.Trade;
using TelecomOperatorsSource.Models;
using TelecomOperatorsSource.QueryTables.Trade;
using TelecomOperatorsSource.References.Trade;
using System;
using TradeResourcesPlugin.Helpers;
using TradeResourcesPlugin.Modules.TelecomOperatorsMenus.Objects;
using UsersResources;
using Yoda.Interfaces;
using Yoda.Interfaces.Forms;
using Yoda.Interfaces.Forms.Components;
using Yoda.Interfaces.Menu;
using YodaHelpers.DateTimeHelper;
using YodaQuery;

namespace TradeResourcesPlugin.Modules.TelecomOperatorsMenus.Trades {
    public class MnuTelecomOperatorsTradeView : FrmMenu<MnuTelecomOperatorsTradeViewArgs> {
        public static string MnuName = nameof(MnuTelecomOperatorsTradeView);
        public MnuTelecomOperatorsTradeView(string moduleName) : base(MnuName, "Конкурсы")
        {
            AsCallback();
            Enabled(rc =>
            {
                var xin = rc.User.GetUserXin(rc.QueryExecuter);
                if (xin == "050540004455"
                || xin == "050540000002"
                || (!rc.User.IsExternalUser() && !rc.User.IsGuest())
                || rc.User.HasRole("TRADERESOURCES-Ресурсы связи-Для операторов связи-Создание приказов", rc.QueryExecuter)
                )
                {
                    return true;
                }
                return false;
            });
            OnRendering(re =>
            {
                var tradeActRev = TradeHelper.GetTradeModel(re.Args.tradeId, re.QueryExecuter);
                RenderRedirectButtons(re, tradeActRev);
                MnuTelecomOperatorsTradeOrder.ViewModel(re.Form, re.AsFormEnv(), tradeActRev);
            });

            void RenderRedirectButtons(FrmRenderEnvironment<MnuTelecomOperatorsTradeViewArgs> re, TelecomOperatorsTradeModel trade)
            {
                if (!re.User.HasRole("TRADERESOURCES-Ресурсы связи-Для операторов связи-Создание приказов", re.QueryExecuter) && re.User.IsExternalUser())
                {
                    return;
                }

                re.RequestContext.AddLocalTask(new Link
                {
                    Text = re.T("Перейти к объекту"),
                    Controller = moduleName,
                    Action = MnuTelecomOperatorsObjectView.MnuName,
                    RouteValues = new TelecomOperatorsObjectViewArgs { MenuAction = "view", Id = trade.flObjectId }
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
                    && trade.flStatus == RefTradesStatuses.Wait
                    && now <= ableToEditLastDate)
                {
                    if (lastRevision == trade.flRevisionId)
                    {
                        re.RequestContext.AddLocalTask(new Link
                        {
                            Text = re.T("Создать приказ на корректировку"),
                            Controller = moduleName,
                            Action = nameof(MnuTelecomOperatorsTradeOrder),
                            RouteValues = new TelecomOperatorsTradeOrderQueryArgs { Id = trade.flId, RevisionId = trade.flRevisionId, MenuAction = MnuTelecomOperatorsTradeOrder.Actions.CreateFrom, OrderType = TradesOrderTypeActions.Edit }
                        });
                        re.RequestContext.AddLocalTask(new Link
                        {
                            Text = re.T("Отменить до начала"),
                            Controller = moduleName,
                            Action = nameof(MnuTelecomOperatorsTradeOrder),
                            RouteValues = new TelecomOperatorsTradeOrderQueryArgs { Id = trade.flId, RevisionId = trade.flRevisionId, MenuAction = MnuTelecomOperatorsTradeOrder.Actions.CreateFrom, OrderType = TradesOrderTypeActions.Cancel }
                        });
                    }
                    else
                    {
                        re.RequestContext.AddLocalTask(new Link
                        {
                            Text = re.T("Открыть неисполненный приказ на корректировку"),
                            Controller = moduleName,
                            Action = nameof(MnuTelecomOperatorsTradeOrder),
                            RouteValues = new TelecomOperatorsTradeOrderQueryArgs { Id = trade.flId, RevisionId = lastRevision, MenuAction = MnuTelecomOperatorsTradeOrder.Actions.ViewOrder }
                        });
                    }
                }
                else if ((!re.User.IsExternalUser() && !re.User.IsGuest())
                        && trade.flStatus == RefTradesStatuses.Wait)
                {
                    if (lastRevision == trade.flRevisionId)
                    {
                        re.RequestContext.AddLocalTask(new Link
                        {
                            Text = re.T("Отменить до начала (Внутренний пользователь)"),
                            Controller = moduleName,
                            Action = nameof(MnuTelecomOperatorsTradeOrder),
                            RouteValues = new TelecomOperatorsTradeOrderQueryArgs { Id = trade.flId, RevisionId = trade.flRevisionId, MenuAction = MnuTelecomOperatorsTradeOrder.Actions.CreateFrom, OrderType = TradesOrderTypeActions.Cancel }
                        });
                    }
                    else
                    {
                        re.RequestContext.AddLocalTask(new Link
                        {
                            Text = re.T("Открыть неисполненный приказ на корректировку (Внутренний пользователь)"),
                            Controller = moduleName,
                            Action = nameof(MnuTelecomOperatorsTradeOrder),
                            RouteValues = new TelecomOperatorsTradeOrderQueryArgs { Id = trade.flId, RevisionId = lastRevision, MenuAction = MnuTelecomOperatorsTradeOrder.Actions.ViewOrder }
                        });
                    }
                }

                if (re.RequestContext.Project == "cabinetResourceSeller"
                    && trade.flStatus == RefTradesStatuses.Wait
                    && ableToEditLastDate < now && now < trade.flDateTime)
                {
                    if (lastRevision == trade.flRevisionId)
                    {
                        re.RequestContext.AddLocalTask(new Link
                        {
                            Text = re.T("Создать приказ на перенос"),
                            Controller = moduleName,
                            Action = nameof(MnuTelecomOperatorsTradeOrder),
                            RouteValues = new TelecomOperatorsTradeOrderQueryArgs { Id = trade.flId, RevisionId = trade.flRevisionId, MenuAction = MnuTelecomOperatorsTradeOrder.Actions.CreateFrom, OrderType = TradesOrderTypeActions.Transfer }
                        });
                    }
                    else
                    {
                        re.RequestContext.AddLocalTask(new Link
                        {
                            Text = re.T("Открыть неисполненный приказ на корректировку"),
                            Controller = moduleName,
                            Action = nameof(MnuTelecomOperatorsTradeOrder),
                            RouteValues = new TelecomOperatorsTradeOrderQueryArgs { Id = trade.flId, RevisionId = lastRevision, MenuAction = MnuTelecomOperatorsTradeOrder.Actions.ViewOrder }
                        });
                    }
                }

            }

        }
    }

    public class MnuTelecomOperatorsTradeViewArgs {
        public int tradeId { get; set; }
    }
}
