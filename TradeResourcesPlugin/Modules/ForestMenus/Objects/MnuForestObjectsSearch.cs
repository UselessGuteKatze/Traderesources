using ForestSource.QueryTables.Common;
using ForestSource.QueryTables.Object;
using TradeResourcesPlugin.Helpers;
using UsersResources;
using Yoda.Interfaces;
using Yoda.Interfaces.Forms;
using Yoda.Interfaces.Forms.Components;
using Yoda.Interfaces.Helpers;
using Yoda.Interfaces.Menu;
using YodaApp.UiSearch;
using YodaHelpers;
using YodaHelpers.Fields;
using YodaQuery;
using System.Linq;

namespace TradeResourcesPlugin.Modules.ForestMenus.Objects {
    public class MnuForestObjectsSearch : FrmMenu {

        public MnuForestObjectsSearch(string moduleName) : base(nameof(MnuForestObjectsSearch), "Объекты") {
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

                var tbObjects = new TbObjects();
                if ((isUserRegistrator || isUserSeller) && !isInternal) {
                    if (hasPair) {
                        tbObjects.AddFilter(t => t.flSellerBin, ConditionOperator.In, pairsData.Select(pairData => pairData.flSellerBin).ToArray());
                    }
                    else {
                        tbObjects.AddFilter(t => t.flSellerBin, xin);
                    }
                }
                tbObjects.OrderBy = new OrderField[] { new OrderField(tbObjects.flId, OrderType.Desc) };

                tbObjects
                .Search(search => {
                    var result = search
                        .Toolbar(toolbar => toolbar.AddIf(isUserRegistrator/*&& !isAgreementSigner*/, new Link {
                            Controller = moduleName,
                            Action = nameof(MnuForestObjectOrder),
                            RouteValues = new ObjectOrderQueryArgs { RevisionId = -1, MenuAction = "create-new" },
                            Text = re.T("Добавить объекты"),
                            CssClass = "btn btn-success"
                        }))
                        .Filtering(filter => filter
                            .AddField(t => t.flId)
                            .AddField(t => t.flName)
                            .AddField(t => t.flBlock)
                        )
                        .TablePresentation(
                            t => new FieldAlias[] {
                                t.flId,
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
                                        Action = nameof(MnuForestObjectView),
                                        RouteValues = new ForestObjectViewArgs { MenuAction = "view", Id = r.GetVal(t => t.flId) },
                                        CssClass = "btn btn-secondary"
                                    },
                                    width: new WidthAttr(80, WidthMeasure.Px)
                                ),
                                t.Column(t => t.flName),
                                t.Column(t => t.flStatus),
                                t.Column(t => t.flBlock)
                            }
                        );
                    if (isUserRegistrator || isUserSeller || isInternal) {
                        result.ExcelPresentation(
                            t => new FieldAlias[] {
                                t.flId,
                                t.flName,
                                t.flStatus,
                                t.flBlock
                            },
                            t => new[] {
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