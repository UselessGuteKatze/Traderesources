using HuntingSource.QueryTables.Object;
using HuntingSource.QueryTables.Trade;
using TradeResourcesPlugin.Helpers;
using UsersResources;
using Yoda.Interfaces.Forms.Components;
using Yoda.Interfaces.Helpers;
using Yoda.Interfaces.Menu;
using YodaApp.UiSearch;
using YodaQuery;

namespace TradeResourcesPlugin.Modules.HuntingMenus.Trades {
    public class MnuHuntingTradeOrdersSearch : FrmMenu {
        public MnuHuntingTradeOrdersSearch(string moduleName) : base(nameof(MnuHuntingTradeOrdersSearch), "Приказы по конкурсам")
        {
            MenuType(Yoda.Interfaces.Menu.MenuType.Normal);
            Enabled((rc) => {
                if (rc.User.IsGuest())
                {
                    return false;
                }
                var xin = rc.User.GetUserXin(rc.QueryExecuter);
                // IAC
                if (xin == "050540004455"
                || xin == "050540000002"
                || (!rc.User.IsExternalUser() && !rc.User.IsGuest())
                || rc.User.HasRole("TRADERESOURCES-Охотничьи угодья-Создание приказов", rc.QueryExecuter)/*rc.User.HasCustomRole("huntingobjects", "dataEdit", rc.QueryExecuter)*/)
                {
                    return true;
                }

                return false;
            });
            OnRendering(re => {

                var isInternal = (!re.User.IsExternalUser() && !re.User.IsGuest());
                var isUserRegistrator = re.User.HasRole("TRADERESOURCES-Охотничьи угодья-Создание приказов", re.QueryExecuter)/*re.User.HasCustomRole("huntingobjects", "dataEdit", re.QueryExecuter)*/;

                var xin = re.User.GetUserXin(re.QueryExecuter);

                var tbTradesRev = new TbTradesRevisions();
                if (!isInternal)
                {
                    tbTradesRev.AddFilter(t => t.flCompetentOrgBin, xin);
                }

                var tbObjects = new TbObjects();

                var tbTradesOrderResult = new TbTradesOrderResult();
                var join = tbTradesRev
                    .JoinT(tbTradesRev.Name, tbTradesOrderResult, tbTradesOrderResult.Name)
                    .On(new Join(tbTradesRev.flRevisionId, tbTradesOrderResult.flSubjectId))
                    .JoinT(tbTradesRev.Name, tbObjects, tbObjects.Name)
                    .On(new Join(tbTradesRev.flObjectId, tbObjects.flId));
                join.OrderBy = new OrderField[] { new OrderField(tbTradesRev.flId, OrderType.Desc) };


                join
                .Search(search => search
                    .Filtering(filter => filter
                        .AddField(t => t.L.L.flId)
                        .AddField(t => t.L.L.flRevisionId)
                        .AddField(t => t.L.L.flStatus)
                        .AddFieldDateTime(t => t.L.R.flExecDate)
                        .AddFieldDateTime(t => t.L.R.flRegDate)
                        .AddField(t => t.L.R.flStatus, customFieldName: "flOrderStatus")
                    )
                    .TablePresentation(
                        t => new FieldAlias[] {
                            t.L.L.flId,
                            t.L.L.flRevisionId,
                            t.L.R.flExecDate,
                            t.L.R.flRegDate,
                            t.L.R.flStatus.ToAlias("flOrderStatus"),
                            t.L.L.flObjectId,
                            t.R.flName,
                            t.L.L.flStatus,
                            t.L.L.flDateTime,
                            t.L.L.flNote
                        },
                        t => new[] {
                            t.Column("Действия", (env, r) =>
                                new Link
                                {
                                    Text = re.T("Открыть"),
                                    Controller = moduleName,
                                    Action = nameof(MnuHuntingTradeOrderBase),
                                    RouteValues = new HuntingTradeOrderQueryArgs { MenuAction = "view-order", RevisionId = r.GetVal(t => t.L.L.flRevisionId) },
                                    CssClass = "btn btn-secondary"
                                },
                                width: new WidthAttr(80, WidthMeasure.Px)
                            ),
                            t.Column(t =>  t.L.L.flId),
                            t.Column(t =>  t.L.L.flRevisionId),
                            t.Column(t => t.L.R.flExecDate),
                            t.Column(t => t.L.R.flRegDate),
                            t.Column("Статус приказа", (env, r) =>  {
                                var value = r.GetVal(tr => tr.L.R.flStatus, "flOrderStatus");
                                var text = t.L.R.flStatus.GetDisplayText(value.ToString(), env.RequestContext);
                                return new HtmlText(text);
                            }),
                            t.Column(t => t.R.flName),
                            t.Column(t => t.L.L.flStatus),
                            t.Column(t => t.L.L.flDateTime),
                            t.Column(t => t.L.L.flNote),
                        }
                    )
                )
                .Print(re.Form, re.AsFormEnv(), re.Form);
            });
        }
    }
}
