using LandSource.QueryTables.LandObject;
using TradeResourcesPlugin.Helpers;
using UsersResources;
using Yoda.Interfaces.Forms.Components;
using Yoda.Interfaces.Helpers;
using Yoda.Interfaces.Helpers.Mrp.QueryTables;
using Yoda.Interfaces.Menu;
using YodaApp.UiSearch;
using YodaHelpers;
using YodaQuery;

namespace TradeResourcesPlugin.Modules.LandObjectsMenus.Object {
    public class MnuLandObjectsOrderSearch: FrmMenu {

        public MnuLandObjectsOrderSearch(string moduleName) : base(nameof(MnuLandObjectsOrderSearch), "Приказы по объектам") {
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
                || rc.User.HasRole("TRADERESOURCES-Земельные ресурсы-Создание приказов", rc.QueryExecuter)/*rc.User.HasCustomRole("landobjects", "appLandEdit", rc.QueryExecuter)*/) {
                    return true;
                }

                return false;
            });
            OnRendering(re => {

                var isInternal = (!re.User.IsExternalUser() && !re.User.IsGuest());
                var isUserRegistrator = re.User.HasRole("TRADERESOURCES-Земельные ресурсы-Создание приказов", re.QueryExecuter)/*re.User.HasCustomRole("landobjects", "appLandEdit", re.QueryExecuter)*/;

                var tbLandObjectsRev = new TbLandObjectsRevisions();
                var xin = re.User.GetUserXin(re.QueryExecuter);
                if (!isInternal)
                {
                    tbLandObjectsRev.AddFilter(t => t.flSallerBin, xin);
                }
                var tbLandObjectsOrderResult = new TbLandObjectsOrderResult();
                var join = tbLandObjectsRev
                .JoinT(tbLandObjectsRev.Name, tbLandObjectsOrderResult, tbLandObjectsOrderResult.Name)
                .On(new Join(tbLandObjectsRev.flRevisionId, tbLandObjectsOrderResult.flSubjectId));
                join.OrderBy = new OrderField[] { new OrderField(tbLandObjectsRev.flId, OrderType.Desc) };

                join
                .Search(search => search
                        .Filtering(filter => filter
                            .AddField(t => t.L.flId)
                            .AddField(t => t.L.flRevisionId)
                            .AddField(t => t.L.flName)
                            .AddField(t => t.R.flStatus, customFieldName: "flOrderStatus")
                            .AddField(t => t.L.flDistrict)
                        )
                        .TablePresentation(
                            t => new FieldAlias[] {
                                t.L.flId,
                                t.L.flName,
                                t.L.flStatus,
                                t.L.flRegion,
                                t.L.flDistrict,
                                t.L.flLandArea,
                                t.L.flRevisionId,
                                t.R.flType,
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
                                        Action = nameof(MnuLandObjectOrderBase),
                                        RouteValues = new LandObjectOrderQueryArgs { MenuAction = "view-order", RevisionId = r.GetVal(t => t.L.flRevisionId) },
                                        CssClass = "btn btn-secondary"
                                    },
                                    width: new WidthAttr(80, WidthMeasure.Px)
                                ),
                                t.Column(t => t.L.flId),
                                t.Column(t => t.R.flType),
                                t.Column(t => t.L.flRevisionId),
                                t.Column(t => t.R.flRegDate),
                                t.Column(t => t.R.flStatus),
                                t.Column(t => t.R.flExecDate),
                                t.Column("Статус приказа", (env, r) =>  {
                                    var value = r.GetVal(tr => tr.R.flStatus, "flOrderStatus");
                                    var text = t.R.flStatus.GetDisplayText(value.ToString(), env.RequestContext);
                                    return new HtmlText(text);
                                }),
                                t.Column(t => t.L.flName),
                                t.Column(t => t.L.flStatus),
                                t.Column(t => t.L.flRegion),
                                t.Column(t => t.L.flDistrict),
                                t.Column(t => t.L.flLandArea),
                            }
                        )
                    )
                    .Print(re.Form, re.AsFormEnv(), re.Form);
            });
        }

    }
}
