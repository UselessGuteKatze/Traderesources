using ForestSource.QueryTables.Common;
using ForestSource.QueryTables.Object;
using TradeResourcesPlugin.Helpers;
using TradeResourcesPlugin.Modules.ForestMenus.ForestryPieces;
using UsersResources;
using Yoda.Interfaces.Forms.Components;
using Yoda.Interfaces.Menu;
using YodaApp.UiSearch;
using YodaQuery;
using System.Linq;

namespace TradeResourcesPlugin.Modules.ForestMenus.ForestryPieces {
    public class MnuForestryPiecesSearch : FrmMenu {
        public const string MnuName = nameof(MnuForestryPiecesSearch);

        public MnuForestryPiecesSearch(string moduleName) : base(MnuName, "Лесные выделы")
        {
            MenuType(Yoda.Interfaces.Menu.MenuType.Normal);
            Access();
            Enabled((rc) => {
                return true;
            });
            OnRendering(re => {

                var xin = re.User.GetUserXin(re.QueryExecuter);
                //var hasPair = new TbSellerSigners().GetPair(xin, re.QueryExecuter, out var data);
                //var isAgreementSigner = hasPair && data.flSignerBin == xin;
                //if (isAgreementSigner)
                //{
                //    xin = data.flSellerBin;
                //}
                var isInternal = (!re.User.IsExternalUser() && !re.User.IsGuest());
                var isUserRegistrator = re.User.HasRole("TRADERESOURCES-Лесные ресурсы-Создание объектов", re.QueryExecuter)/*re.User.HasCustomRole("forestobjects", "dataEdit", re.QueryExecuter)*/;
                var isUserSeller = re.User.HasRole("TRADERESOURCES-Лесные ресурсы-Выставление на торги", re.QueryExecuter)/*re.User.HasCustomRole("forestobjects", "dataEdit", re.QueryExecuter)*/;
                var hasPair = new TbSellerCreators().GetPair(xin, re.QueryExecuter, out var pairsData);
                //var isUserViewer = re.User.HasCustomRole("forestobjects", "dataView", re.QueryExecuter);

                var tbObjects = new TbForestryPieces();
                if ((isUserRegistrator || isUserSeller) && !isInternal)
                {
                    if (hasPair) {
                        tbObjects.AddFilter(t => t.flSellerBin, ConditionOperator.In, pairsData.Select(pairData => pairData.flCreatorBin).ToArray());
                    }
                    else {
                        tbObjects.AddFilter(t => t.flSellerBin, xin);
                    }
                }
                tbObjects.Order(t => t.flId, OrderType.Desc);

                var join = tbObjects
                .JoinT("tbObjects", new TbQuarters(), "TbQuarters")
                .On((t1, t2) => new Join(t1.flQuarter, t2.flId))
                .JoinT("TbQuarters", new TbForestries(), "TbForestries")
                .On((t1, t2) => new Join(t1.R.flForestry, t2.flId));

                join
                .Search(search => {
                    var result = search
                        .Filtering(filter => filter
                            .AddField(t => t.L.L.flId)
                            .AddField(t => t.L.L.flNumber)
                            .AddField(t => t.L.R.flNumber)
                            .AddField(t => t.R.flName)
                        )
                        .TablePresentation(
                            t => new FieldAlias[] {
                                t.L.L.flId,
                                t.L.L.flNumber,
                                t.L.R.flNumber.ToAlias("flQuarterNumber"),
                                t.R.flName,
                                t.L.L.flStatus,
                                t.L.L.flBlock,
                                t.L.L.flArea
                            },
                            t => new[] {
                                t.Column("Действия", (env, r) =>
                                    new Link
                                    {
                                        Text = re.T("Открыть"),
                                        Controller = nameof(RegistersModule),
                                        Action = nameof(MnuForestryPieceView),
                                        RouteValues = new ForestryPieceViewArgs { MenuAction = "view", Id = r.GetVal(t => t.L.L.flId) },
                                        CssClass = "btn btn-secondary"
                                    },
                                    width: new WidthAttr(80, WidthMeasure.Px)
                                ),
                                t.Column(t => t.L.L.flNumber),
                                t.Column(t => t.L.R.flNumber),
                                t.Column(t => t.R.flName),
                                t.Column(t => t.L.L.flStatus),
                                t.Column(t => t.L.L.flBlock),
                                t.Column(t => t.L.L.flArea)
                            }
                        );
                    if (isUserRegistrator || isUserSeller || isInternal) {
                        result.ExcelPresentation(
                            t => new FieldAlias[] {
                                t.L.L.flId,
                                t.L.L.flNumber,
                                t.L.R.flNumber.ToAlias("flQuarterNumber"),
                                t.R.flName,
                                t.L.L.flStatus,
                                t.L.L.flBlock,
                                t.L.L.flArea
                            },
                            t => new[] {
                                t.ExcelColumn(t => t.L.L.flNumber),
                                t.ExcelColumn(t => t.L.R.flNumber),
                                t.ExcelColumn(t => t.R.flName),
                                t.ExcelColumn(t => t.L.L.flStatus),
                                t.ExcelColumn(t => t.L.L.flBlock),
                                t.ExcelColumn(t => t.L.L.flArea)
                            }
                        );
                    }
                    return result;
                })
                .Print(re.Form, re.AsFormEnv(), re.Form);
            });
        }
    }
}