using CommonSource;
using LandSource.Helpers;
using LandSource.Models;
using LandSource.QueryTables.Trades;
using LandSource.References.Trades;
using System;
using TradeResourcesPlugin.Modules.LandObjectsMenus.Object;
using TradeResourcesPlugin.Modules.LandObjectsMenus.Trade;
using UsersResources;
using Yoda.Interfaces;
using Yoda.Interfaces.Forms.Components;
using Yoda.Interfaces.Menu;
using YodaHelpers.ActionMenus;
using YodaHelpers.DateTimeHelper;
using YodaQuery;

namespace TradeResourcesPlugin.Modules.LandObjectsMenus.Trades {
    public class MnuLandObjectTradeView: FrmMenu<MnuLandObjectTradeViewArgs> {
        public static string MnuName = nameof(MnuLandObjectTradeView);
        public MnuLandObjectTradeView(string moduleName) : base(MnuName, "Торги")
        {
            AsCallback();
            Enabled(rc =>
            {
                var xin = rc.User.GetUserXin(rc.QueryExecuter);
                if (xin == "050540004455"
                || xin == "050540000002"
                || (!rc.User.IsExternalUser() && !rc.User.IsGuest())
                || rc.User.HasRole("TRADERESOURCES-Земельные ресурсы-Создание приказов", rc.QueryExecuter)/*rc.User.HasCustomRole("landobjects", "appLandView", rc.QueryExecuter)*/)
                {
                    return true;
                }
                return false;
            });
            OnRendering(re =>
            {
                var tradeActRev = LandObjectTradeModelHelper.GetTradeModel(re.Args.tradeId, re.QueryExecuter);
                RenderRedirectButtons(re, tradeActRev);
                ViewModel(re, tradeActRev);
            });


            void RenderRedirectButtons(FrmRenderEnvironment<MnuLandObjectTradeViewArgs> re, LandObjectTradeModel trade)
            {
                if (!re.User.HasRole("TRADERESOURCES-Земельные ресурсы-Создание приказов", re.QueryExecuter)/*!re.User.HasCustomRole("landobjects", "appLandEdit", re.QueryExecuter)*/ && re.User.IsExternalUser())
                {
                    return;
                }

                re.RequestContext.AddLocalTask(new Link
                {
                    Text = re.T("Перейти к объекту"),
                    Controller = moduleName,
                    Action = MnuLandObjectView.MnuName,
                    RouteValues = new LandObjectViewActionQueryArgs { MenuAction = "view", Id = trade.ObjectId }
                });

                var ableToEditLastDate = trade.DateTime.Value.AddWorkdays(-3, re.QueryExecuter);

                var tradeRevisions = new TbLandObjectsTradesRevisions();
                tradeRevisions.AddFilter(t => t.flId, trade.Id);
                var revisionResults = new TbLandObjectsTradesOrderResult();
                revisionResults.AddFilterNot(t => t.flStatus, "Canceled");
                var join = tradeRevisions
                    .Join("tradeRevisions", revisionResults, "revisionResults")
                    .On(new Condition(tradeRevisions.flRevisionId, revisionResults.flSubjectId));
                join.OrderBy = new[] { new OrderField(tradeRevisions.flRevisionId, OrderType.Desc) };

                var lastRevision = Convert.ToInt32(join.SelectScalar(tradeRevisions.flRevisionId, re.QueryExecuter));

                var now = re.QueryExecuter.GetDateTime(NpGlobal.DbKeys.DbYodaGr);

                if (re.RequestContext.Project == "cabinetResourceSeller"
                    && trade.Status == RefLandObjectTradeStatuses.Wait.ToString()
                    && now <= ableToEditLastDate)
                {

                    if (lastRevision == trade.RevisionId)
                    {
                        re.RequestContext.AddLocalTask(new Link
                        {
                            Text = re.T("Создать приказ на корректировку"),
                            Controller = moduleName,
                            Action = nameof(MnuLandObjectTradeOrderBase),
                            RouteValues = new LandObjectTradeOrderQueryArgs { Id = trade.Id, RevisionId = trade.RevisionId, MenuAction = MnuLandObjectTradeOrderBase.Actions.CreateFrom, OrderType = LandObjectTradesOrderTypeActions.Edit }
                        });
                        re.RequestContext.AddLocalTask(new Link
                        {
                            Text = re.T("Отменить до начала"),
                            Controller = moduleName,
                            Action = nameof(MnuLandObjectTradeOrderBase),
                            RouteValues = new LandObjectTradeOrderQueryArgs { Id = trade.Id, RevisionId = trade.RevisionId, MenuAction = MnuLandObjectTradeOrderBase.Actions.CreateFrom, OrderType = LandObjectTradesOrderTypeActions.Cancel }
                        });
                    }
                    else
                    {
                        re.RequestContext.AddLocalTask(new Link
                        {
                            Text = re.T("Открыть неисполненный приказ на корректировку"),
                            Controller = moduleName,
                            Action = nameof(MnuLandObjectTradeOrderBase),
                            RouteValues = new LandObjectTradeOrderQueryArgs { Id = trade.Id, RevisionId = lastRevision, MenuAction = MnuLandObjectTradeOrderBase.Actions.ViewOrder }
                        });
                    }
                }
                else if ((!re.User.IsExternalUser() && !re.User.IsGuest())
                          && trade.Status == RefLandObjectTradeStatuses.Wait.ToString())
                {
                    re.RequestContext.AddLocalTask(new Link
                    {
                        Text = re.T("Отменить до начала (Внутренний пользователь)"),
                        Controller = moduleName,
                        Action = nameof(MnuLandObjectTradeOrderBase),
                        RouteValues = new LandObjectTradeOrderQueryArgs { Id = trade.Id, RevisionId = trade.RevisionId, MenuAction = MnuLandObjectTradeOrderBase.Actions.CreateFrom, OrderType = LandObjectTradesOrderTypeActions.Cancel }
                    });
                }

                if (re.RequestContext.Project == "cabinetResourceSeller"
                    && trade.Status == RefLandObjectTradeStatuses.Wait.ToString()
                    && ableToEditLastDate < now && now < trade.DateTime)
                {
                    if (lastRevision == trade.RevisionId)
                    {
                        re.RequestContext.AddLocalTask(new Link
                        {
                            Text = re.T("Создать приказ на перенос"),
                            Controller = moduleName,
                            Action = nameof(MnuLandObjectTradeOrderBase),
                            RouteValues = new LandObjectTradeOrderQueryArgs { Id = trade.Id, RevisionId = trade.RevisionId, MenuAction = MnuLandObjectTradeOrderBase.Actions.CreateFrom, OrderType = LandObjectTradesOrderTypeActions.Transfer }
                        });
                    }
                    else
                    {
                        re.RequestContext.AddLocalTask(new Link
                        {
                            Text = re.T("Открыть неисполненный приказ на корректировку"),
                            Controller = moduleName,
                            Action = nameof(MnuLandObjectTradeOrderBase),
                            RouteValues = new LandObjectTradeOrderQueryArgs { Id = trade.Id, RevisionId = lastRevision, MenuAction = MnuLandObjectTradeOrderBase.Actions.ViewOrder }
                        });
                    }
                }
            }

            void ViewModel(FrmRenderEnvironment<MnuLandObjectTradeViewArgs> re, LandObjectTradeModel trade)
            {
                MnuLandObjectTradeOrderBase.viewModelMainData(re.Form, re.AsFormEnv(), trade);
                if (trade.InitialCost > 0)
                {
                    MnuLandObjectTradeOrderBase.viewModelRequisitesData(re.Form, re.AsFormEnv(), trade);
                }
                if (trade.Method == RefLandObjectTradeForms.Values.LandTradesPriceDown)
                {
                    MnuLandObjectTradeOrderBase.viewModelPriceDownData(re.Form, re.AsFormEnv(), trade);
                }
                if (trade.Type == RefLandObjectTradeTypes.RentOwnership)
                {
                    MnuLandObjectTradeOrderBase.viewModelRentData(re.Form, re.AsFormEnv(), trade);
                }
                if (trade.Type == RefLandObjectTradeTypes.PrivateOwnership)
                {
                    MnuLandObjectTradeOrderBase.viewModelPrivateData(re.Form, re.AsFormEnv(), trade);
                }

                if (trade.Form == RefLandObjectTradeForms.Values.EAuction)
                {
                    MnuLandObjectTradeOrderBase.viewModelCalculatedData(re.Form, re.AsFormEnv(), trade);
                } else if (trade.Form == RefLandObjectTradeForms.Values.Contest)
                {
                    MnuLandObjectTradeOrderBase.viewModelContestData(re.Form, re.AsFormEnv(), trade);
                }

                if (!string.IsNullOrEmpty(trade.WinnerBin))
                {
                    MnuLandObjectTradeOrderBase.viewModelSaleData(re.Form, re.AsFormEnv(), trade);
                }
                if (trade.IsSaleProtocolExists == true)
                {
                    MnuLandObjectTradeOrderBase.viewModelProtocolData(re.Form, re.AsFormEnv(), trade);
                }
                if (trade.IsSaleAgreementExists == true)
                {
                    MnuLandObjectTradeOrderBase.viewModelAgreementData(re.Form, re.AsFormEnv(), trade);
                }

                //MnuLandObjectTradeOrderBase.viewModelPublicationsData(re.Form, re.AsFormEnv(), trade);
            }
        }
    }

    public class MnuLandObjectTradeViewArgs: ActionQueryArgsBase {
        public int tradeId { get; set; }
    }
}
