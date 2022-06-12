using HydrocarbonSource.QueryTables.Object;
using HydrocarbonSource.QueryTables.Trade;
using TradeResourcesPlugin.Helpers;
using UsersResources;
using Yoda.Interfaces.Forms.Components;
using Yoda.Interfaces.Menu;
using YodaApp.UiSearch;
using YodaQuery;

namespace TradeResourcesPlugin.Modules.HydrocarbonMenus.Trades {
    public class MnuHydrocarbonTradesSearch : FrmMenu {

        public MnuHydrocarbonTradesSearch(string moduleName) : base(nameof(MnuHydrocarbonTradesSearch), "Конкурсы")
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
                || rc.User.HasRole("TRADERESOURCES-Недропользование-Создание приказов", rc.QueryExecuter)/*rc.User.HasCustomRole("traderesources", "view", rc.QueryExecuter)*/
                /*|| rc.User.HasPermission(nameof(RegistersModule), RegistersModule.LocalPermissions.Landlords)*/) {
                    return true;
                }

                return false;
            });
            OnRendering(re => {

                var isInternal = (!re.User.IsExternalUser() && !re.User.IsGuest());
                var isUserRegistrator = re.User.HasRole("TRADERESOURCES-Недропользование-Создание приказов", re.QueryExecuter);
                //var isUserViewer = re.User.HasCustomRole("traderesources", "view", re.QueryExecuter);

                var xin = re.User.GetUserXin(re.QueryExecuter);
                var tbTrades = new TbTrades();

                if ((/*isUserViewer ||*/ isUserRegistrator) && !(re.User.IsSuperUser || isInternal))
                {
                    tbTrades.AddFilter(t => t.flCompetentOrgBin, xin);
                }

                var tbObjects = new TbObjects();

                var join = tbTrades
                    .JoinT(tbTrades.Name, tbObjects, tbObjects.Name, JoinType.Left)
                    .On(new Join(tbTrades.flObjectId, tbObjects.flId));

                join.OrderBy = new OrderField[] { new OrderField(tbTrades.flId, OrderType.Desc) };

                join
                .Search(search => search
                    .Filtering(filter => filter
                        .AddField(t => t.L.flId)
                        .AddField(t => t.R.flNumber)
                        .AddField(t => t.L.flStatus)
                        .AddFieldDateTime(t => t.L.flDateTime)
                    )
                    .TablePresentation(
                        t => new FieldAlias[] {
                            t.L.flId,
                            t.L.flRevisionId,
                            t.L.flObjectId,
                            t.R.flNumber,
                            t.R.flName,
                            t.L.flStatus,
                            t.L.flDateTime,
                            t.L.flNote
                        },
                        t => new[] {
                            t.Column("Действия", (env, r) =>
                                new Link
                                {
                                    Text = re.T("Открыть"),
                                    Controller = moduleName,
                                    Action = nameof(MnuHydrocarbonTradeView),
                                    RouteValues = new MnuHydrocarbonTradeViewArgs { tradeId = r.GetVal(t => t.L.flId) },
                                    CssClass = "btn btn-secondary"
                                },
                                width: new WidthAttr(80, WidthMeasure.Px)
                            ),
                            t.Column(t => t.L.flId),
                            t.Column(t => t.L.flRevisionId),
                            t.Column(t => t.L.flObjectId),
                            t.Column(t => t.R.flNumber),
                            t.Column(t => t.R.flName),
                            t.Column(t => t.L.flStatus),
                            t.Column(t => t.L.flDateTime),
                            t.Column(t => t.L.flNote),
                        }
                    )
                    .ExcelPresentation(
                        t => new FieldAlias[] {
                            t.L.flId,
                            t.L.flRevisionId,
                            t.L.flObjectId,
                            t.R.flNumber,
                            t.R.flName,
                            t.L.flStatus,
                            t.L.flDateTime,
                            t.L.flNote
                        },
                        t => new[] {
                            t.ExcelColumn(t => t.L.flId),
                            t.ExcelColumn(t => t.L.flRevisionId),
                            t.ExcelColumn(t => t.L.flObjectId),
                            t.ExcelColumn(t => t.R.flNumber),
                            t.ExcelColumn(t => t.R.flName),
                            t.ExcelColumn(t => t.L.flStatus),
                            t.ExcelColumn(t => t.L.flDateTime),
                            t.ExcelColumn(t => t.L.flNote),
                        }
                    )
                )
                .Print(re.Form, re.AsFormEnv(), re.Form);
            });
        }

    }
}
