using ForestSource.QueryTables.Common;
using ForestSource.QueryTables.Object;
using TradeResourcesPlugin.Helpers;
using TradeResourcesPlugin.Modules.ForestMenus.Forestries;
using UsersResources;
using Yoda.Interfaces.Forms.Components;
using Yoda.Interfaces.Menu;
using YodaApp.UiSearch;
using YodaQuery;
using System.Linq;

namespace TradeResourcesPlugin.Modules.ForestMenus.Forestries {
    public class MnuForestriesSearch : FrmMenu {
        public const string MnuName = nameof(MnuForestriesSearch);

        public MnuForestriesSearch(string moduleName) : base(MnuName, "Лесные хозяйства")
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

                var tbObjects = new TbForestries();
                if ((isUserRegistrator || isUserSeller) && !isInternal) {
                    if (hasPair) {
                        tbObjects.AddFilter(t => t.flSellerBin, ConditionOperator.In, pairsData.Select(pairData => pairData.flCreatorBin).ToArray());
                    }
                    else {
                        tbObjects.AddFilter(t => t.flSellerBin, xin);
                    }
                }
                tbObjects.Order(t => t.flId, OrderType.Desc);

                tbObjects
            .Search(search => {
                var result = search
                    .Toolbar(toolbar => toolbar.AddIf(isUserRegistrator/*&& !isAgreementSigner*/, new Link {
                        Controller = moduleName,
                        Action = nameof(MnuForestryOrder),
                        RouteValues = new ForestryOrderQueryArgs { RevisionId = -1, MenuAction = "create-new" },
                        Text = re.T("Добавить объект"),
                        CssClass = "btn btn-success"
                    }))
                    .Filtering(filter => filter
                        .AddField(t => t.flDistrict)
                        .AddField(t => t.flId)
                        .AddField(t => t.flName)
                    )
                    .TablePresentation(
                        t => new FieldAlias[] {
                            t.flId,
                            t.flName,
                            t.flStatus,
                            t.flRegion,
                            t.flDistrict,
                            t.flArea
                        },
                        t => new[] {
                            t.Column("Действия", (env, r) =>
                                new Link
                                {
                                    Text = re.T("Открыть"),
                                    Controller = nameof(RegistersModule),
                                    Action = nameof(MnuForestryView),
                                    RouteValues = new ForestryViewArgs { MenuAction = "view", Id = r.GetVal(t => t.flId) },
                                    CssClass = "btn btn-secondary"
                                },
                                width: new WidthAttr(80, WidthMeasure.Px)
                            ),
                            t.Column(t => t.flName),
                            t.Column(t => t.flStatus),
                            t.Column(t => t.flRegion),
                            t.Column(t => t.flDistrict),
                            t.Column(t => t.flArea)
                        }
                    );
                if (isUserRegistrator || isUserSeller || isInternal) {
                    result.ExcelPresentation(
                        t => new FieldAlias[] {
                            t.flId,
                            t.flName,
                            t.flStatus,
                            t.flRegion,
                            t.flDistrict,
                            t.flArea
                        },
                        t => new[] {
                            t.ExcelColumn(t => t.flName),
                            t.ExcelColumn(t => t.flStatus),
                            t.ExcelColumn(t => t.flRegion),
                            t.ExcelColumn(t => t.flDistrict),
                            t.ExcelColumn(t => t.flArea)
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