using LandSource.QueryTables.LandObject;
using LandSource.QueryTables.Trades;
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

namespace TradeResourcesPlugin.Modules.LandObjectsMenus.Object {
    public class MnuLandObjectsSearch: FrmMenu {

        public MnuLandObjectsSearch(string moduleName) : base(nameof(MnuLandObjectsSearch), "Объекты") {
            MenuType(Yoda.Interfaces.Menu.MenuType.Normal);
            Access();
            Enabled((rc) => {
                return true;
            });
            OnRendering(re => {

                var isInternal = (!re.User.IsExternalUser() && !re.User.IsGuest());
                var isUserRegistrator = re.User.HasRole("TRADERESOURCES-Земельные ресурсы-Создание приказов", re.QueryExecuter)/*re.User.HasCustomRole("landobjects", "appLandEdit", re.QueryExecuter)*/;
                //var isUserViewer = re.User.HasCustomRole("landobjects", "appLandView", re.QueryExecuter);

                //re.RequestContext.AppEnv.UserProvider.UserLogIn(re.RequestContext, "190540004205-790120300550");

                var tbLandObjects = new TbLandObjects();
                var xin = re.User.GetUserXin(re.QueryExecuter);
                if ((/*isUserViewer ||*/ isUserRegistrator) && !(re.User.IsSuperUser || isInternal || re.User.IsGuest()))
                {
                    tbLandObjects.AddFilter(t => t.flSallerBin, xin);
                }

                var tbLandObjectsTrades = new TbLandObjectsTrades();

                var joinConditions = new Join();
                joinConditions.Add(tbLandObjects.flId, tbLandObjectsTrades.flObjectId);
                joinConditions.Add(tbLandObjectsTrades.flCost, ConditionOperator.NotEqual, System.DBNull.Value);

                var join = tbLandObjects
                    .JoinT("tbLandObjects", tbLandObjectsTrades, "tbLandObjectsTrades", JoinType.Left)
                    .On(joinConditions);

                join.OrderBy = new OrderField[] { new OrderField(tbLandObjects.flId, OrderType.Desc) };

                renderInfo(re);
                join
                .Search(search => {
                    var result = search
                        .Toolbar(toolbar => toolbar.AddIf(isUserRegistrator/*&& !isAgreementSigner*/, new Link {
                            Controller = moduleName,
                            Action = nameof(MnuLandObjectOrderBase),
                            RouteValues = new LandObjectOrderQueryArgs { RevisionId = -1, MenuAction = "create-new" },
                            Text = re.T("Добавить объекты"),
                            CssClass = "btn btn-success"
                        }))
                        .Filtering(filter => filter
                            .AddField(t => t.L.flDistrict)
                            .AddField(t => t.L.flId)
                            .AddField(t => t.L.flName)
                            .AddField(t => t.L.flStatus)
                        )
                        .TablePresentation(
                            t => new FieldAlias[] {
                                t.L.flId,
                                t.L.flName,
                                t.L.flStatus,
                                t.L.flRegion,
                                t.L.flDistrict,
                                t.L.flLandArea,
                                t.R.flCost,
                            },
                            t => new[] {
                                t.Column("Действия", (env, r) =>
                                    new Link
                                    {
                                        Text = re.T("Открыть"),
                                        Controller = nameof(RegistersModule),
                                        Action = nameof(MnuLandObjectView),
                                        RouteValues = new LandObjectViewActionQueryArgs { MenuAction = "view", Id = r.GetVal(t => t.L.flId) },
                                        CssClass = "btn btn-secondary"
                                    },
                                    width: new WidthAttr(80, WidthMeasure.Px)
                                ),
                                t.Column(t => t.L.flId),
                                t.Column(t => t.L.flName),
                                t.Column(t => t.L.flStatus),
                                t.Column(t => t.L.flRegion),
                                t.Column(t => t.L.flDistrict),
                                t.Column(t => t.L.flLandArea),
                                t.Column("Цена продажи, тг.", (env, r) =>  {
                                    var value = r.GetValOrNull(tr => tr.L.flCost);
                                    var text = "";
                                    if (value != null) {
                                        text = t.R.flCost.GetDisplayText(value, env.RequestContext);
                                    }
                                    return new HtmlText(text);
                                }),
                            }
                        );
                    if (isUserRegistrator || isInternal) {
                        result.ExcelPresentation(
                            t => new FieldAlias[] {
                                t.L.flId,
                                t.L.flName,
                                t.L.flStatus,
                                t.L.flRegion,
                                t.L.flDistrict,
                                t.L.flLandArea,
                                t.R.flCost,
                            },
                            t => new[] {
                                t.ExcelColumn(t => t.L.flId),
                                t.ExcelColumn(t => t.L.flName),
                                t.ExcelColumn(t => t.L.flStatus),
                                t.ExcelColumn(t => t.L.flRegion),
                                t.ExcelColumn(t => t.L.flDistrict),
                                t.ExcelColumn(t => t.L.flLandArea),
                                t.ExcelColumn("Цена продажи, тг.", ExcelValueType.Number, null, (env, r) =>  {
                                    var value = r.GetValOrNull(tr => tr.L.flCost);
                                    return value;
                                }),
                            }
                        );
                    }
                    return result;
                })
                .Print(re.Form, re.AsFormEnv(), re.Form);
            });
        }

        private void renderInfo(FrmRenderEnvironment<EmptyQueryArgs> re) {
            re.Form.AddComponent(new HtmlText(@$"
                <p>
                    {re.T("Лицу, заинтересованному в покупке земельного участка или права аренды земельного участка (далее - Объект), необходимо по имеющимся критериям поиска выбрать Объект")}	
                </p>
                <br/>
            "));
        }

    }
}
