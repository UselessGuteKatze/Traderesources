using CommonSource;
using FishingSource.QueryTables.Common;
using FishingSource.QueryTables.Reservoir;
using FishingSource.References.Object;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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

namespace TradeResourcesPlugin.Modules.FishingMenus.Reservoirs {
    public class MnuReservoirsSearch: FrmMenu {

        public MnuReservoirsSearch(string moduleName) : base(nameof(MnuReservoirsSearch), "Водоёмы") {
            MenuType(Yoda.Interfaces.Menu.MenuType.Normal);
            Access();
            Enabled((rc) => {
                return true;
            });
            OnRendering(re => {
                var tbObjects = new TbReservoirs();

                var isInternal = (!re.User.IsExternalUser() && !re.User.IsGuest());
                var xins = new[] { re.User.GetUserXin(re.QueryExecuter) };
                var hasPair = new TbSellerSigners().GetPair(xins[0], re.QueryExecuter, out var data);
                var isAgreementSigner = hasPair && data.flSignerBins.Contains(xins[0]);
                if (isAgreementSigner) {
                    xins = data.flSellerBins;
                }
                var isUserRegistrator = re.User.HasRole("TRADERESOURCES-Рыбохозяйственные водоёмы-Создание приказов", re.QueryExecuter);

                tbObjects.OrderBy = new OrderField[] { new OrderField(tbObjects.flId, OrderType.Desc) };

                tbObjects
                .Search(search => {
                    var result = search
                        .Toolbar(toolbar => toolbar.AddIf(isUserRegistrator/*&& !isAgreementSigner*/, new Link {
                            Controller = moduleName,
                            Action = nameof(MnuReservoirOrderBase),
                            RouteValues = new ReservoirOrderQueryArgs { RevisionId = -1, MenuAction = "create-new" },
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
                                t.flArea,
                                t.flLocation,
                            },
                            t => new[] {
                                t.Column("Действия", (env, r) =>
                                    new Link
                                    {
                                        Text = "Открыть",
                                        Controller = nameof(RegistersModule),
                                        Action = nameof(MnuReservoirView),
                                        RouteValues = new ReservoirViewActionQueryArgs { MenuAction = "view", Id = r.GetVal(t => t.flId) },
                                        CssClass = "btn btn-secondary"
                                    },
                                    width: new WidthAttr(80, WidthMeasure.Px)
                                ),
                                t.Column(t => t.flId),
                                t.Column(t => t.flName),
                                t.Column(t => t.flStatus),
                                t.Column(t => t.flRegion),
                                t.Column(t => t.flDistrict),
                                t.Column(t => t.flArea),
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
                                    t.flArea,
                                    t.flLocation,
                                },
                                t => new[] {
                                    t.ExcelColumn(t => t.flId),
                                    t.ExcelColumn(t => t.flName),
                                    t.ExcelColumn(t => t.flStatus),
                                    t.ExcelColumn(t => t.flRegion),
                                    t.ExcelColumn(t => t.flDistrict),
                                    t.ExcelColumn(t => t.flArea),
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
