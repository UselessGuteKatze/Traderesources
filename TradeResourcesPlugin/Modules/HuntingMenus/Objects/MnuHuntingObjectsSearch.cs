using HuntingSource.QueryTables.Common;
using HuntingSource.QueryTables.Object;
using TradeResourcesPlugin.Helpers;
using UsersResources;
using Yoda.Interfaces;
using Yoda.Interfaces.Forms;
using Yoda.Interfaces.Forms.Components;
using Yoda.Interfaces.Helpers;
using Yoda.Interfaces.Menu;
using YodaApp.UiSearch;
using YodaHelpers;
using YodaQuery;

namespace TradeResourcesPlugin.Modules.HuntingMenus.Objects {
    public class MnuHuntingObjectsSearch : FrmMenu {

        public MnuHuntingObjectsSearch(string moduleName) : base(nameof(MnuHuntingObjectsSearch), "Объекты")
        {
            MenuType(Yoda.Interfaces.Menu.MenuType.Normal);
            Access();
            Enabled((rc) => {
                return true;
            });
            OnRendering(re => {

                var xin = re.User.GetUserXin(re.QueryExecuter);
                var hasPair = new TbSellerSigners().GetPair(xin, re.QueryExecuter, out var data);
                var isAgreementSigner = hasPair && data.flSignerBin == xin;
                if (isAgreementSigner)
                {
                    xin = data.flSellerBin;
                }
                var isInternal = (!re.User.IsExternalUser() && !re.User.IsGuest());
                var isUserRegistrator = re.User.HasRole("TRADERESOURCES-Охотничьи угодья-Создание приказов", re.QueryExecuter);

                var tbObjects = new TbObjects();
                if ((isUserRegistrator || isAgreementSigner) && !(re.User.IsSuperUser || isInternal || re.User.IsGuest()))
                {
                    tbObjects.AddFilter(t => t.flSallerBin, xin);
                }
                tbObjects.OrderBy = new OrderField[] { new OrderField(tbObjects.flId, OrderType.Desc) };

                tbObjects
                .Search(search => {
                    var result = search
                        .Toolbar(toolbar => toolbar.AddIf(isUserRegistrator/*&& !isAgreementSigner*/, new Link {
                            Controller = moduleName,
                            Action = nameof(MnuHuntingObjectOrderBase),
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
                                t.flLocation,
                            },
                            t => new[] {
                                t.Column("Действия", (env, r) =>
                                    new Link {
                                        Text = re.T("Открыть"),
                                        Controller = nameof(RegistersModule),
                                        Action = nameof(MnuHuntingObjectView),
                                        RouteValues = new HuntingObjectViewArgs { MenuAction = "view", Id = r.GetVal(t => t.flId) },
                                        CssClass = "btn btn-secondary"
                                    },
                                    width: new WidthAttr(80, WidthMeasure.Px)
                                ),
                                t.Column(t => t.flId),
                                t.Column(t => t.flName),
                                t.Column(t => t.flStatus),
                                t.Column(t => t.flRegion),
                                t.Column(t => t.flDistrict),
                                t.Column(t => t.flLocation),
                            }
                        );
                    if (isUserRegistrator || isAgreementSigner || isInternal) {
                        result.ExcelPresentation(
                            t => new FieldAlias[] {
                                t.flId,
                                t.flName,
                                t.flStatus,
                                t.flRegion,
                                t.flDistrict,
                                t.flLocation,
                            },
                            t => new[] {
                                t.ExcelColumn(t => t.flId),
                                t.ExcelColumn(t => t.flName),
                                t.ExcelColumn(t => t.flStatus),
                                t.ExcelColumn(t => t.flRegion),
                                t.ExcelColumn(t => t.flDistrict),
                                t.ExcelColumn(t => t.flLocation),
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