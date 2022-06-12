using TelecomOperatorsSource.QueryTables.Object;
using TradeResourcesPlugin.Helpers;
using TradeResourcesPlugin.Modules.TelecomOperatorsMenus.Objects;
using UsersResources;
using Yoda.Interfaces;
using Yoda.Interfaces.Forms;
using Yoda.Interfaces.Forms.Components;
using Yoda.Interfaces.Helpers;
using Yoda.Interfaces.Menu;
using YodaApp.UiSearch;
using YodaHelpers;
using YodaQuery;

namespace TradeResourcesPlugin.Modules.TelecomOperatorsMenus.Object {
    public class MnuTelecomOperatorsObjectsSearch : FrmMenu {

        public MnuTelecomOperatorsObjectsSearch(string moduleName) : base(nameof(MnuTelecomOperatorsObjectsSearch), "Объекты") {
            MenuType(Yoda.Interfaces.Menu.MenuType.Normal);
            Access();
            Enabled((rc) => {
                return true;
            });
            OnRendering(re => {

                var isInternal = (!re.User.IsExternalUser() && !re.User.IsGuest());
                var isUserRegistrator = re.User.HasRole("TRADERESOURCES-Ресурсы связи-Для операторов связи-Создание приказов", re.QueryExecuter);

                var tbObjects = new TbObjects();
                var xin = re.User.GetUserXin(re.QueryExecuter);
                if (isUserRegistrator && !(re.User.IsSuperUser || isInternal || re.User.IsGuest()))
                {
                    tbObjects.AddFilter(t => t.flSellerBin, xin);
                }

                tbObjects.OrderBy = new OrderField[] { new OrderField(tbObjects.flId, OrderType.Desc) };

                tbObjects
                .Search(search => {
                    var result = search
                        .Toolbar(toolbar => toolbar.AddIf(isUserRegistrator/*&& !isAgreementSigner*/, new Link {
                            Controller = moduleName,
                            Action = nameof(MnuTelecomOperatorsObjectOrder),
                            RouteValues = new ObjectOrderQueryArgs { RevisionId = -1, MenuAction = "create-new" },
                            Text = re.T("Добавить объекты"),
                            CssClass = "btn btn-success"
                        }))
                        .Filtering(filter => filter
                            .AddField(t => t.flDistrict)
                            .AddField(t => t.flId)
                            .AddField(t => t.flName)
                            .AddField(t => t.flStatus)
                        )
                        .TablePresentation(
                            t => new FieldAlias[] {
                                t.flId,
                                t.flName,
                                t.flStatus,
                                t.flRegion,
                                t.flDistrict,
                            },
                            t => new[] {
                                t.Column("Действия", (env, r) =>
                                    new Link
                                    {
                                        Text = re.T("Открыть"),
                                        Controller = nameof(RegistersModule),
                                        Action = nameof(MnuTelecomOperatorsObjectView),
                                        RouteValues = new TelecomOperatorsObjectViewArgs { MenuAction = "view", Id = r.GetVal(t => t.flId) },
                                        CssClass = "btn btn-secondary"
                                    },
                                    width: new WidthAttr(80, WidthMeasure.Px)
                                ),
                                t.Column(t => t.flDistrict),
                                t.Column(t => t.flId),
                                t.Column(t => t.flName),
                                t.Column(t => t.flStatus),
                            }
                        );
                    if (isUserRegistrator || isInternal) {
                        result.ExcelPresentation(
                            t => new FieldAlias[] {
                                t.flId,
                                t.flName,
                                t.flStatus,
                                t.flRegion,
                                t.flDistrict,
                            },
                            t => new[] {
                                t.ExcelColumn(t => t.flDistrict),
                                t.ExcelColumn(t => t.flId),
                                t.ExcelColumn(t => t.flName),
                                t.ExcelColumn(t => t.flStatus),
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
