using CommonSource;
using FishingSource.Helpers.Trade;
using FishingSource.Models;
using FishingSource.QueryTables.Trade;
using FishingSource.References.Trade;
using System;
using TradeResourcesPlugin.Helpers;
using TradeResourcesPlugin.Modules.FishingMenus.Objects;
using UsersResources;
using Yoda.Interfaces;
using Yoda.Interfaces.Forms.Components;
using Yoda.Interfaces.Menu;
using YodaHelpers.DateTimeHelper;
using YodaQuery;

namespace TradeResourcesPlugin.Modules.FishingMenus.Trades {
    public class MnuFishingTradeView : FrmMenu<MnuFishingTradeViewArgs> {
        public static string MnuName = nameof(MnuFishingTradeView);
        public MnuFishingTradeView(string moduleName) : base(MnuName, "Конкурсы")
        {
            AsCallback();
            Enabled(rc =>
            {
                var xin = rc.User.GetUserXin(rc.QueryExecuter);
                if (xin == "050540004455"
                || xin == "050540000002"
                || (!rc.User.IsExternalUser() && !rc.User.IsGuest())
                || rc.User.HasRole("TRADERESOURCES-Рыбохозяйственные водоёмы-Создание приказов", rc.QueryExecuter)/*rc.User.HasCustomRole("fishingobjects", "dataView", rc.QueryExecuter)*/
                /*|| rc.User.HasCustomRole("fishingobjects", "dataEdit", rc.QueryExecuter)*/)
                {
                    return true;
                }
                return false;
            });
            OnRendering(re =>
            {
                var tradeActRev = TradeHelper.GetTradeModel(re.Args.tradeId, re.QueryExecuter);
                RenderRedirectButtons(re, tradeActRev);
                MnuFishingTradeOrderBase.ViewModel(re.Form, re.AsFormEnv(), tradeActRev);
            });


            void RenderRedirectButtons(FrmRenderEnvironment<MnuFishingTradeViewArgs> re, FishingTradeModel trade)
            {
                if (!re.User.HasRole("TRADERESOURCES-Рыбохозяйственные водоёмы-Создание приказов", re.QueryExecuter)/*!re.User.HasCustomRole("fishingobjects", "dataEdit", re.QueryExecuter)*/ && re.User.IsExternalUser())
                {
                    return;
                }

                re.RequestContext.AddLocalTask(new Link
                {
                    Text = re.T("Перейти к объекту"),
                    Controller = moduleName,
                    Action = MnuFishingObjectView.MnuName,
                    RouteValues = new FishingObjectViewArgs { MenuAction = "view", Id = trade.flObjectId }
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
                            Action = nameof(MnuFishingTradeOrderBase),
                            RouteValues = new FishingTradeOrderQueryArgs { Id = trade.flId, RevisionId = trade.flRevisionId, MenuAction = MnuFishingTradeOrderBase.Actions.CreateFrom, OrderType = TradesOrderTypeActions.Edit }
                        });
                        re.RequestContext.AddLocalTask(new Link
                        {
                            Text = re.T("Отменить до начала"),
                            Controller = moduleName,
                            Action = nameof(MnuFishingTradeOrderBase),
                            RouteValues = new FishingTradeOrderQueryArgs { Id = trade.flId, RevisionId = trade.flRevisionId, MenuAction = MnuFishingTradeOrderBase.Actions.CreateFrom, OrderType = TradesOrderTypeActions.Cancel }
                        });
                    }
                    else
                    {
                        re.RequestContext.AddLocalTask(new Link
                        {
                            Text = re.T("Открыть неисполненный приказ на корректировку"),
                            Controller = moduleName,
                            Action = nameof(MnuFishingTradeOrderBase),
                            RouteValues = new FishingTradeOrderQueryArgs { Id = trade.flId, RevisionId = lastRevision, MenuAction = MnuFishingTradeOrderBase.Actions.ViewOrder }
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
                            Action = nameof(MnuFishingTradeOrderBase),
                            RouteValues = new FishingTradeOrderQueryArgs { Id = trade.flId, RevisionId = trade.flRevisionId, MenuAction = MnuFishingTradeOrderBase.Actions.CreateFrom, OrderType = TradesOrderTypeActions.Cancel }
                        });
                    } else
                    {
                        re.RequestContext.AddLocalTask(new Link
                        {
                            Text = re.T("Открыть неисполненный приказ на корректировку (Внутренний пользователь)"),
                            Controller = moduleName,
                            Action = nameof(MnuFishingTradeOrderBase),
                            RouteValues = new FishingTradeOrderQueryArgs { Id = trade.flId, RevisionId = lastRevision, MenuAction = MnuFishingTradeOrderBase.Actions.ViewOrder }
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
                            Action = nameof(MnuFishingTradeOrderBase),
                            RouteValues = new FishingTradeOrderQueryArgs { Id = trade.flId, RevisionId = trade.flRevisionId, MenuAction = MnuFishingTradeOrderBase.Actions.CreateFrom, OrderType = TradesOrderTypeActions.Transfer }
                        });
                    }
                    else
                    {
                        re.RequestContext.AddLocalTask(new Link
                        {
                            Text = re.T("Открыть неисполненный приказ на корректировку"),
                            Controller = moduleName,
                            Action = nameof(MnuFishingTradeOrderBase),
                            RouteValues = new FishingTradeOrderQueryArgs { Id = trade.flId, RevisionId = lastRevision, MenuAction = MnuFishingTradeOrderBase.Actions.ViewOrder }
                        });
                    }
                }

            }
        }
    }

    public class MnuFishingTradeViewArgs {
        public int tradeId { get; set; }
    }
}
