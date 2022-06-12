using ForestSource.QueryTables.Object;
using TradeResourcesPlugin.Helpers;
using UsersResources;
using Yoda.Interfaces.Forms.Components;
using Yoda.Interfaces.Helpers;
using Yoda.Interfaces.Menu;
using YodaApp.UiSearch;
using YodaQuery;

namespace TradeResourcesPlugin.Modules.ForestMenus.Forestries {
    public class MnuForestryOrdersSearch : FrmMenu {
        public const string MnuName = nameof(MnuForestryOrdersSearch);

        public MnuForestryOrdersSearch(string moduleName) : base(MnuName, "Приказы по лесным хозяйствам")
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
                || rc.User.HasRole("TRADERESOURCES-Лесные ресурсы-Создание объектов", rc.QueryExecuter)/*rc.User.HasCustomRole("forestobjects", "dataEdit", rc.QueryExecuter)*/)
                {
                    return true;
                }

                return false;
            });
            OnRendering(re => {

                var isInternal = (!re.User.IsExternalUser() && !re.User.IsGuest());
                var isUserRegistrator = re.User.HasRole("TRADERESOURCES-Лесные ресурсы-Создание объектов", re.QueryExecuter)/*re.User.HasCustomRole("forestobjects", "dataEdit", re.QueryExecuter)*/;

                var tbForestriesRev = new TbForestriesRevisions();
                var xin = re.User.GetUserXin(re.QueryExecuter);
                if (!isInternal)
                {
                    tbForestriesRev.AddFilter(t => t.flSellerBin, xin);
                }
                var tbForestriesOrderResult = new TbForestriesOrderResult();
                var join = tbForestriesRev
                .JoinT(tbForestriesRev.Name, tbForestriesOrderResult, tbForestriesOrderResult.Name)
                .On((t1, t2) => new Join(t1.flRevisionId, t2.flSubjectId));
                join.OrderBy = new OrderField[] { new OrderField(tbForestriesRev.flId, OrderType.Desc) };
                join
                .Search(search => search
                        .Filtering(filter => filter
                            .AddField(t => t.L.flDistrict)
                            .AddField(t => t.L.flId)
                            .AddField(t => t.L.flRevisionId)
                            .AddField(t => t.R.flStatus, customFieldName: "flOrderStatus")
                        )
                        .TablePresentation(
                            t => new FieldAlias[] {
                                t.L.flName,
                                t.L.flSellerBin,
                                t.L.flStatus,
                                t.L.flRegion,
                                t.L.flDistrict,
                                t.L.flArea,
                                t.L.flId,
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
                                        Action = nameof(MnuForestryOrder),
                                        RouteValues = new ForestryOrderQueryArgs { MenuAction = "view-order", RevisionId = r.GetVal(t => t.L.flRevisionId) },
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
                                t.Column(t => t.L.flRegion),
                                t.Column(t => t.L.flDistrict),
                                t.Column(t => t.L.flArea)
                            }
                        )
                    )
                    .Print(re.Form, re.AsFormEnv(), re.Form);
            });
        }

    }
}
