using HydrocarbonSource.QueryTables.Object;
using TradeResourcesPlugin.Helpers;
using UsersResources;
using Yoda.Interfaces.Forms.Components;
using Yoda.Interfaces.Helpers;
using Yoda.Interfaces.Menu;
using YodaApp.UiSearch;
using YodaQuery;

namespace TradeResourcesPlugin.Modules.HydrocarbonMenus.Objects {
    public class MnuHydrocarbonObjectOrdersSearch : FrmMenu {

        public MnuHydrocarbonObjectOrdersSearch(string moduleName) : base(nameof(MnuHydrocarbonObjectOrdersSearch), "Приказы по объектам")
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
                || rc.User.HasRole("TRADERESOURCES-Недропользование-Создание приказов", rc.QueryExecuter)/*rc.User.HasPermission(nameof(RegistersModule), RegistersModule.LocalPermissions.Landlords)*/)
                {
                    return true;
                }

                return false;
            });
            OnRendering(re => {

                var isInternal = (!re.User.IsExternalUser() && !re.User.IsGuest());

                var tbObjectsRev = new TbObjectsRevisions();
                var xin = re.User.GetUserXin(re.QueryExecuter);
                if (!isInternal)
                {
                    tbObjectsRev.AddFilter(t => t.flSellerBin, xin);
                }
                var tbObjectsOrderResult = new TbObjectsOrderResult();
                var join = tbObjectsRev
                .JoinT(tbObjectsRev.Name, tbObjectsOrderResult, tbObjectsOrderResult.Name)
                .On((t1, t2) => new Join(t1.flRevisionId, t2.flSubjectId));
                join.OrderBy = new OrderField[] { new OrderField(tbObjectsRev.flId, OrderType.Desc) };

                join
                .Search(search => search
                        .Filtering(filter => filter
                            .AddField(t => t.L.flId)
                            .AddField(t => t.L.flRevisionId)
                            .AddField(t => t.L.flName)
                            .AddFieldDateTime(t => t.R.flExecDate)
                            .AddFieldDateTime(t => t.R.flRegDate)
                            .AddField(t => t.R.flStatus, customFieldName: "flOrderStatus")
                        )
                        .TablePresentation(
                            t => new FieldAlias[] {
                                t.L.flId,
                                t.L.flName,
                                t.L.flSellerBin,
                                t.L.flStatus,
                                t.L.flBlock,
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
                                        Action = nameof(MnuHydrocarbonObjectOrder),
                                        RouteValues = new ObjectOrderQueryArgs { MenuAction = "view-order", RevisionId = r.GetVal(t => t.L.flRevisionId) },
                                        CssClass = "btn btn-secondary"
                                    },
                                    width: new WidthAttr(80, WidthMeasure.Px)
                                ),
                                t.Column(t => t.L.flRevisionId),
                                t.Column(t => t.R.flRegDate),
                                t.Column("Статус приказа", (env, r) =>  {
                                    var value = r.GetVal(tr => tr.R.flStatus, "flOrderStatus");
                                    var text = t.R.flStatus.GetDisplayText(value.ToString(), env.RequestContext);
                                    return new HtmlText(text);
                                }),
                                t.Column(t => t.R.flExecDate),
                                t.Column(t => t.L.flName),
                                t.Column(t => t.L.flSellerBin),
                                t.Column(t => t.L.flStatus),
                            }
                        )
                    )
                    .Print(re.Form, re.AsFormEnv(), re.Form);
            });
        }

    }
}
