using FishingSource.FieldEditors.Object;
using ForestSource.FieldEditors.Object;
using ForestSource.Helpers.Object;
using ForestSource.Models;
using ForestSource.QueryTables.Object;
using ForestSource.References.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using TradeResourcesPlugin.Helpers;
using TradeResourcesPlugin.Modules.Components;
using TradeResourcesPlugin.Modules.ForestMenus.ForestryPieces;
using TradeResourcesPlugin.Modules.ForestMenus.Quarters;
using UsersResources;
using Yoda.Interfaces;
using Yoda.Interfaces.Forms;
using Yoda.Interfaces.Forms.Components;
using YodaApp.Yoda.Interfaces.Forms.Components;
using YodaHelpers.ActionMenus;
using YodaHelpers.Fields;
using YodaQuery;

namespace TradeResourcesPlugin.Modules.ForestMenus.Forestries {
    public class MnuForestryView : MnuActionsExt<ForestryViewArgs> {
        public const string MnuName = nameof(MnuForestryView);

        public MnuForestryView(string moduleName) : base(moduleName, MnuName, "Лесное хозяйство")
        {
            AsCallback();
            //Enabled(rc => rc.User.IsAuthentificated);
        }

        public class Actions {
            public const string View = "view";
        }

        public const string ActionCssClass = "btn btn-sm btn-dark font-weight-normal text-wrap mr-2 mb-2";

        public override void Configure(ActionConfig<ForestryViewArgs> config)
        {
            config
                .OnAction(Actions.View, action => action
                    .IsValid(env => new OkResult())
                    .OnRendering(re => {
                        renderObjectView(re.Form, re);
                    }));
        }

        private void renderObjectView(WidgetBase widget, ActionEnv<ForestryViewArgs> env)
        {
            var objectModel = new TbForestries()
                .AddFilter(t => t.flId, env.Args.Id)
                .GetForestryModelFirst(env.QueryExecuter);

            var objectSeller = objectModel.flSellerBin;
            var currentUser = env.User.GetUserXin(env.QueryExecuter);
            var isObjectSeller = currentUser == objectSeller && env.User.HasRole("TRADERESOURCES-Лесные ресурсы-Создание объектов", env.QueryExecuter)/*env.User.HasCustomRole("forestobjects", "dataEdit", env.QueryExecuter)*/;
            //var isAgreementSigner = currentUser == objectSeller && env.User.HasCustomRole("forestobjects", "dataEdit", env.QueryExecuter);

            var quarterModels = new TbQuarters()
                .AddFilter(t => t.flStatus, ForestryQuarterStatuses.Active)
                .AddFilter(t => t.flForestry, objectModel.flId)
                .GetQuarterModels(env.QueryExecuter);

            var tbForestryPiecesJoin = new TbForestryPieces()
                .AddFilterNot(t => t.flStatus, ForestObjectStatuses.Deleted.ToString())
                .GetForestryQuarterJoin(out var tbForestries, out var tbQuarters);
            tbForestries.AddFilter(t => t.flId, objectModel.flId);
            var forestryPiecesModels = tbForestryPiecesJoin.GetForestryPieceModels(env.QueryExecuter);

            var mainRow = new GridRow().AppendTo(widget);
            new GridCol("col-md-9")
                .Append(renderObjectData(objectModel, quarterModels, isObjectSeller, env))
                .Append(renderFellingValue(forestryPiecesModels, env))
                .Append(renderObjectQuarters(objectModel, quarterModels, forestryPiecesModels, isObjectSeller, env))
                .AppendTo(mainRow);
            new GridCol("col-md-3")
                .Append(renderObjectGeometry(objectModel, quarterModels, forestryPiecesModels, env))
                .AppendTo(mainRow);
        }

        private List<LinkBase> objectActions(ForestryModel objectModel, QuarterModel[] quarterModels, bool isObjectSeller, ActionEnv<ForestryViewArgs> env) {
            var actions = new List<LinkBase>();

            if (!isObjectSeller) {
                return actions;
            }

            if (objectModel.flStatus == ForestryQuarterStatuses.Active) {
                actions.Add(new Link {
                    CssClass = ActionCssClass,
                    Text = env.T("Редактировать объект"),
                    Controller = ModuleName,
                    Action = nameof(MnuForestryOrder),
                    RouteValues = new ForestryOrderQueryArgs { Id = objectModel.flId, RevisionId = objectModel.flRevisionId, OrderType = ForestriesOrderTypeActions.Edit, MenuAction = "create-from" }
                });

                if (quarterModels.Length == 0) {
                    actions.Add(new Link {
                        CssClass = ActionCssClass,
                        Text = env.T("Удалить объект"),
                        Controller = ModuleName,
                        Action = nameof(MnuForestryOrder),
                        RouteValues = new ForestryOrderQueryArgs { Id = objectModel.flId, RevisionId = objectModel.flRevisionId, OrderType = ForestriesOrderTypeActions.Remove, MenuAction = "create-from" }
                    });
                }
            }
            return actions;
        }

        private Card renderObjectData(ForestryModel objectModel, QuarterModel[] quarterModels, bool isObjectSeller, ActionEnv<ForestryViewArgs> env)
        {
            var objectData = new Card(objectModel.flName, objectModel.flStatus == ForestryQuarterStatuses.Deleted ? "text-decoration-line-through" : "");
            var objectDataActions = objectActions(objectModel, quarterModels, isObjectSeller, env);
            if (objectDataActions.Count > 0)
            {
                new Panel("bg-white bordered border-top form-wizard-header mt-1 pt-2 pl-2").AppendRange(objectDataActions).AppendTo(objectData);
            }
            var tabPanel = new HyperTabs().AppendTo(objectData);

            var tbObject = new TbForestries();

            var objectShortData = new Panel();
            tbObject.flId.RenderCustom(objectShortData, env, objectModel.flId, readOnly: true);
            tbObject.flStatus.RenderCustom(objectShortData, env, objectModel.flStatus, readOnly: true);
            tabPanel.AddNewTab("Краткая информация", objectShortData);

            var objectFullData = new Panel();
            MnuForestryOrder.viewModelMainData(objectFullData, env, objectModel, true);
            MnuForestryOrder.viewModelGeoPositionData(objectFullData, env, objectModel, true);
            tabPanel.AddNewTab("Подробная информация", objectFullData);

            return objectData;
        }
        private Card renderObjectQuarters(ForestryModel objectModel, QuarterModel[] quarterModels, ForestryPieceModel[] forestryPieceModels, bool isObjectSeller, ActionEnv<ForestryViewArgs> env)
        {
            var quartersShortData = new Card(env.T("Кварталы"));
            var quartersShortDataActions = quartersActions(objectModel, isObjectSeller, env);
            if (quartersShortDataActions.Count > 0) {
                new Panel("bg-white bordered border-top form-wizard-header mt-1 pt-2 pl-2").AppendRange(quartersShortDataActions).AppendTo(quartersShortData);
            }

            quarterModels.Each(quarterModel => {
                var forestryPiecesRows = new Accordion("Лесные выделы:", collapsed: true);
                forestryPiecesRows.CssClass = "mt-3 d-none";

                forestryPieceModels.Where(forestryPieceModel => forestryPieceModel.flQuarter == quarterModel.flId).Each(forestryPieceModel => {
                    forestryPiecesRows.CssClass = "mt-3";
                    new GridRow()
                    .Append(
                        new GridCol("col-md-3")
                            .Append(
                                new Panel()
                                    .Append(
                                        new Link {
                                            Text = env.T("Открыть"),
                                            Controller = nameof(RegistersModule),
                                            Action = nameof(MnuForestryPieceView),
                                            RouteValues = new ForestryPieceViewArgs { MenuAction = "view", Id = forestryPieceModel.flId },
                                            CssClass = "btn btn-secondary"
                                        }
                                    )
                            )
                    )
                    .Append(
                        new GridCol("col-md-9")
                            .Append(
                                new Heading(HeadingLevel.h4, $"Лесной выдел № {forestryPieceModel.flNumber}, площадь, га: {forestryPieceModel.flArea}, описание: {forestryPieceModel.flDescription}")
                            )
                    )
                    .AppendTo(forestryPiecesRows)
                    ;
                });


                new GridRow()
                    .Append(
                        new GridCol("col-md-3")
                            .Append(
                                new Panel()
                                    .Append(
                                        new Link {
                                            Text = env.T("Открыть"),
                                            Controller = nameof(RegistersModule),
                                            Action = nameof(MnuQuarterView),
                                            RouteValues = new QuarterViewArgs { MenuAction = "view", Id = quarterModel.flId },
                                            CssClass = "btn btn-secondary"
                                        }
                                    )
                            )
                    )
                    .Append(
                        new GridCol("col-md-9")
                            .Append(
                                new Heading(HeadingLevel.h4, $"Квартал № {quarterModel.flNumber}, площадь, га: {quarterModel.flArea}, описание: {quarterModel.flDescription}")
                            )
                    )
                    .Append(
                        new GridCol("col-md-12")
                            .Append(
                                forestryPiecesRows
                            )
                    )
                    .AppendTo(quartersShortData)
                    ;
            });

            return quartersShortData;
        }

        private List<LinkBase> quartersActions(ForestryModel objectModel, bool isObjectSeller, ActionEnv<ForestryViewArgs> env) {
            var actions = new List<LinkBase>();

            if (!isObjectSeller) {
                return actions;
            }

            if (objectModel.flStatus == ForestryQuarterStatuses.Active) {
                actions.Add(new Link {
                    Controller = nameof(RegistersModule),
                    Action = nameof(MnuQuarterOrder),
                    RouteValues = new QuarterOrderQueryArgs { RevisionId = -1, MenuAction = "create-new", ForestryId = objectModel.flId },
                    Text = env.T("Добавить квартал"),
                    CssClass = "btn btn-success"
                });
            }
            return actions;
        }

        public Card renderFellingValue(ForestryPieceModel[] forestryPiecesModels, ActionEnv<ForestryViewArgs> env) {
            var card = new Card(env.T("Объем рубки"));
            var tbObject = new TbForestryPieces();
            var flFellingValue = forestryPiecesModels
                .SelectMany(forestryPieceModel => forestryPieceModel.flFellingValue)
                .GroupBy(forestryPieceModel => forestryPieceModel[FellingValueEditor.Fls.flFellingType.ToString()])
                .SelectMany(forestryPieceModelsGroup =>
                    forestryPieceModelsGroup
                        .GroupBy(forestryPieceModelsGroup => forestryPieceModelsGroup[FellingValueEditor.Fls.flForestryType.ToString()])
                        .Select(forestryPieceModelsGroupGroup => {
                            var ret = new Dictionary<string, object>();
                            ret.Add(FellingValueEditor.Fls.flFellingType.ToString(), forestryPieceModelsGroupGroup.First()[FellingValueEditor.Fls.flFellingType.ToString()]);
                            ret.Add(FellingValueEditor.Fls.flForestryType.ToString(), forestryPieceModelsGroupGroup.First()[FellingValueEditor.Fls.flForestryType.ToString()]);

                            ret.Add(FellingValueEditor.Fls.flForestryAge.ToString(), forestryPieceModelsGroupGroup.Average(row => Convert.ToDecimal(row[FellingValueEditor.Fls.flForestryAge.ToString()])));

                            ret.Add(FellingValueEditor.Fls.flArea.ToString(), forestryPieceModelsGroupGroup.Sum(row => Convert.ToDecimal(row[FellingValueEditor.Fls.flArea.ToString()])));
                            ret.Add(FellingValueEditor.Fls.flForestStock.ToString(), forestryPieceModelsGroupGroup.Sum(row => Convert.ToDecimal(row[FellingValueEditor.Fls.flForestStock.ToString()])));
                            ret.Add(FellingValueEditor.Fls.flLiquidWood.ToString(), forestryPieceModelsGroupGroup.Sum(row => Convert.ToDecimal(row[FellingValueEditor.Fls.flLiquidWood.ToString()])));
                            ret.Add(FellingValueEditor.Fls.flWoodForFirewood.ToString(), forestryPieceModelsGroupGroup.Sum(row => Convert.ToDecimal(row[FellingValueEditor.Fls.flWoodForFirewood.ToString()])));
                            return ret;
                        })
                ).ToArray();
            tbObject.flFellingValue.RenderCustom(card, env, flFellingValue, readOnly: true, hideLabel: true);
            return card;
        }

        public Card renderObjectGeometry(ForestryModel objectModel, QuarterModel[] quarterModels, ForestryPieceModel[] forestryPieceModels, ActionEnv<ForestryViewArgs> env)
        {
            var card = new Card(env.T("Геометрия"))
                .Append(new ObjectGeometryViewerComponent(objectModel.flWKT, wktsNeighbours: quarterModels.Select(x => x.flWKT).Concat(forestryPieceModels.Select(x => x.flWKT)).ToArray()));
            return card;
        }
    }

    public class ForestryViewArgs : ActionQueryArgsBase {
        public int Id { get; set; }
    }

}