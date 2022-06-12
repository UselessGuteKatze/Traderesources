using HuntingSource.QueryTables.Object;
using TradeResourcesPlugin.Helpers;
using UsersResources;
using Yoda.Interfaces.Forms.Components;
using Yoda.Interfaces.Helpers;
using Yoda.Interfaces.Menu;
using YodaApp.UiSearch;
using YodaQuery;

namespace TradeResourcesPlugin.Modules.HuntingMenus.Objects {
    public class MnuHuntingObjectOrdersSearch : FrmMenu {

        public MnuHuntingObjectOrdersSearch(string moduleName) : base(nameof(MnuHuntingObjectOrdersSearch), "Приказы по объектам")
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

                var tbObjectsRev = new TbObjectsRevisions();
                var xin = re.User.GetUserXin(re.QueryExecuter);
                if (!isInternal)
                {
                    tbObjectsRev.AddFilter(t => t.flSallerBin, xin);
                }
                var tbObjectsOrderResult = new TbObjectsOrderResult();
                var join = tbObjectsRev
                .JoinT(tbObjectsRev.Name, tbObjectsOrderResult, tbObjectsOrderResult.Name)
                .On(new Join(tbObjectsRev.flRevisionId, tbObjectsOrderResult.flSubjectId));
                join.OrderBy = new OrderField[] { new OrderField(tbObjectsRev.flId, OrderType.Desc) };

                join
                .Search(search => search
                        .Filtering(filter => filter
                            .AddField(t => t.L.flDistrict)
                            .AddField(t => t.L.flId)
                            .AddField(t => t.L.flRevisionId)
                            .AddField(t => t.R.flStatus, customFieldName: "flOrderStatus")
                            .AddField(t => t.L.flName)
                        )
                        .TablePresentation(
                            t => new FieldAlias[] {
                                t.L.flId,
                                t.L.flName,
                                t.L.flStatus,
                                t.L.flRegion,
                                t.L.flDistrict,
                                t.L.flLocation,
                                t.L.flRevisionId,
                                t.R.flExecDate,
                                t.R.flRegDate,
                                t.R.flStatus.ToAlias("flOrderStatus"),
                            },
                            t => new[] {
                                t.Column("Действия", (env, r) =>
                                    new Link
                                    {
                                        Text = re.T("Открыть"),
                                        Controller = moduleName,
                                        Action = nameof(MnuHuntingObjectOrderBase),
                                        RouteValues = new ObjectOrderQueryArgs { MenuAction = "view-order", RevisionId = r.GetVal(t => t.L.flRevisionId) },
                                        CssClass = "btn btn-secondary"
                                    },
                                    width: new WidthAttr(80, WidthMeasure.Px)
                                ),
                                t.Column(t => t.L.flRevisionId),
                                t.Column(t => t.R.flRegDate),
                                t.Column(t => t.R.flStatus),
                                t.Column(t => t.R.flExecDate),
                                t.Column(re.T("Статус приказа"), (env, r) =>  {
                                    var value = r.GetVal(tr => tr.R.flStatus, "flOrderStatus");
                                    var text = t.R.flStatus.GetDisplayText(value.ToString(), env.RequestContext);
                                    return new HtmlText(text);
                                }),
                                t.Column(t => t.L.flId),
                                t.Column(t => t.L.flName),
                                t.Column(t => t.L.flStatus),
                                t.Column(t => t.L.flRegion),
                                t.Column(t => t.L.flDistrict),
                                t.Column(t => t.L.flLocation),
                            }
                        )
                    )
                    .Print(re.Form, re.AsFormEnv(), re.Form);
            });
        }

    }
}
