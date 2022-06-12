using FishingSource.QueryTables.Common;
using FishingSource.QueryTables.Object;
using FishingSource.QueryTables.Trade;
using System.Linq;
using TradeResourcesPlugin.Helpers;
using UsersResources;
using Yoda.Interfaces.Forms.Components;
using Yoda.Interfaces.Helpers;
using Yoda.Interfaces.Menu;
using YodaApp.UiSearch;
using YodaQuery;

namespace TradeResourcesPlugin.Modules.FishingMenus.Trades {
    public class MnuFishingTradesSearch : FrmMenu {

        public MnuFishingTradesSearch(string moduleName) : base(nameof(MnuFishingTradesSearch), "Конкурсы")
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
                || rc.User.HasRole("TRADERESOURCES-Рыбохозяйственные водоёмы-Создание приказов", rc.QueryExecuter))
                {
                    return true;
                }

                return false;
            });
            OnRendering(re => {
                var isInternal = (!re.User.IsExternalUser() && !re.User.IsGuest());
                var xins = new[] { re.User.GetUserXin(re.QueryExecuter) };
                var hasPair = new TbSellerSigners().GetPair(xins[0], re.QueryExecuter, out var data);
                var isAgreementSigner = hasPair && data.flSignerBins.Contains(xins[0]);
                if (isAgreementSigner) {
                    xins = data.flSellerBins;
                }
                var isUserRegistrator = re.User.HasRole("TRADERESOURCES-Рыбохозяйственные водоёмы-Создание приказов", re.QueryExecuter);

                var tbTrades = new TbTrades();

                if ((/*isUserViewer || */isUserRegistrator) && !(re.User.IsSuperUser || isInternal))
                {
                    tbTrades.AddFilter(t => t.flCompetentOrgBin, ConditionOperator.In, xins);
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
                        .AddField(t => t.L.flStatus)
                        .AddFieldDateTime(t => t.L.flDateTime)
                    )
                    .TablePresentation(
                        t => new FieldAlias[] {
                            t.L.flId,
                            t.L.flRevisionId,
                            t.L.flObjectId,
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
                                    Action = nameof(MnuFishingTradeView),
                                    RouteValues = new MnuFishingTradeViewArgs { tradeId = r.GetVal(t => t.L.flId) },
                                    CssClass = "btn btn-secondary"
                                },
                                width: new WidthAttr(80, WidthMeasure.Px)
                            ),
                            t.Column(t => t.L.flId),
                            t.Column(t => t.L.flRevisionId),
                            t.Column(t => t.L.flObjectId),
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
                            t.R.flName,
                            t.L.flStatus,
                            t.L.flDateTime,
                            t.L.flNote
                        },
                        t => new[] {
                            t.ExcelColumn(t => t.L.flId),
                            t.ExcelColumn(t => t.L.flRevisionId),
                            t.ExcelColumn(t => t.L.flObjectId),
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
