using HydrocarbonSource.QueryTables.Object;
using TradeResourcesPlugin.Helpers;
using UsersResources;
using Yoda.Interfaces.Forms.Components;
using Yoda.Interfaces.Menu;
using YodaApp.UiSearch;
using YodaHelpers.Fields;
using YodaQuery;

namespace TradeResourcesPlugin.Modules.HydrocarbonMenus.Objects {
    public class MnuHydrocarbonObjectsSearch : FrmMenu {

        public MnuHydrocarbonObjectsSearch(string moduleName) : base(nameof(MnuHydrocarbonObjectsSearch), "Объекты") {
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
                var isUserRegistrator = re.User.HasRole("TRADERESOURCES-Недропользование-Создание приказов", re.QueryExecuter)/*re.User.HasPermission(nameof(RegistersModule), RegistersModule.LocalPermissions.Landlords)*/;
                //var isUserViewer = re.User.HasCustomRole("traderesources", "view", re.QueryExecuter);

                var tbObjects = new TbObjects();
                if ((/*isUserViewer || */isUserRegistrator /*|| isAgreementSigner*/) && !(re.User.IsSuperUser || isInternal || re.User.IsGuest())) {
                    tbObjects.AddFilter(t => t.flSellerBin, xin);
                }
                tbObjects.OrderBy = new OrderField[] { new OrderField(tbObjects.flId, OrderType.Desc) };

                tbObjects
                .Search(search => {
                    var result = search
                        .Toolbar(toolbar => toolbar.AddIf(isUserRegistrator/*&& !isAgreementSigner*/, new Link {
                            Controller = moduleName,
                            Action = nameof(MnuHydrocarbonObjectOrder),
                            RouteValues = new ObjectOrderQueryArgs { RevisionId = -1, MenuAction = "create-new" },
                            Text = re.T("Добавить объекты"),
                            CssClass = "btn btn-success"
                        }))
                        .Filtering(filter => filter
                            .AddField(t => t.flId)
                            .AddField(t => t.flNumber)
                            .AddField(t => t.flName)
                            .AddField(t => t.flBlock)
                        )
                        .TablePresentation(
                            t => new FieldAlias[] {
                                t.flId,
                                t.flNumber,
                                t.flName,
                                t.flStatus,
                                t.flBlock
                            },
                            t => new[] {
                                t.Column("Действия", (env, r) =>
                                    new Link
                                    {
                                        Text = re.T("Открыть"),
                                        Controller = nameof(RegistersModule),
                                        Action = nameof(MnuHydrocarbonObjectView),
                                        RouteValues = new HydrocarbonObjectViewArgs { MenuAction = "view", Id = r.GetVal(t => t.flId) },
                                        CssClass = "btn btn-secondary"
                                    },
                                    width: new WidthAttr(80, WidthMeasure.Px)
                                ),
                                t.Column(t => t.flNumber),
                                t.Column(t => t.flName),
                                t.Column(t => t.flStatus),
                                t.Column(t => t.flBlock)
                            }
                        );
                    if (isUserRegistrator || isInternal) {
                        result.ExcelPresentation(
                            t => new FieldAlias[] {
                                t.flId,
                                t.flName,
                                t.flStatus,
                                t.flBlock
                            },
                            t => new[] {
                                t.ExcelColumn(t => t.flNumber),
                                t.ExcelColumn(t => t.flName),
                                t.ExcelColumn(t => t.flStatus),
                                t.ExcelColumn(t => t.flBlock)
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