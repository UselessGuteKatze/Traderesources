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
using TradeResourcesPlugin.Modules.ForestMenus.Forestries;
using TradeResourcesPlugin.Modules.ForestMenus.ForestryPieces;
using UsersResources;
using Yoda.Interfaces;
using Yoda.Interfaces.Forms;
using Yoda.Interfaces.Forms.Components;
using YodaApp.Yoda.Interfaces.Forms.Components;
using YodaHelpers.ActionMenus;
using YodaHelpers.Fields;
using YodaQuery;

namespace TradeResourcesPlugin.Modules.ForestMenus.Quarters {
    public class MnuQuarterView : MnuActionsExt<QuarterViewArgs> {
        public const string MnuName = nameof(MnuQuarterView);

        public MnuQuarterView(string moduleName) : base(moduleName, MnuName, "Квартал")
        {
            AsCallback();
            //Enabled(rc => rc.User.IsAuthentificated);
        }

        public class Actions {
            public const string View = "view";
        }

        public const string ActionCssClass = "btn btn-sm btn-dark font-weight-normal text-wrap mr-2 mb-2";

        public override void Configure(ActionConfig<QuarterViewArgs> config)
        {
            config
                .OnAction(Actions.View, action => action
                    .IsValid(env => new OkResult())
                    .OnRendering(re => {
                        renderObjectView(re.Form, re);
                    }));
        }

        private void renderObjectView(WidgetBase widget, ActionEnv<QuarterViewArgs> env)
        {
            var objectModel = new TbQuarters()
                .AddFilter(t => t.flId, env.Args.Id)
                .GetQuarterModelFirst(env.QueryExecuter);

            var objectSeller = objectModel.flSellerBin;
            var currentUser = env.User.GetUserXin(env.QueryExecuter);
            var isObjectSeller = currentUser == objectSeller && env.User.HasRole("TRADERESOURCES-Лесные ресурсы-Создание объектов", env.QueryExecuter)/*env.User.HasCustomRole("forestobjects", "dataEdit", env.QueryExecuter)*/;
            //var isAgreementSigner = currentUser == objectSeller && env.User.HasCustomRole("forestobjects", "dataEdit", env.QueryExecuter);

            var forestryModel = new TbForestries()
                .AddFilter(t => t.flId, objectModel.flForestry)
                .GetForestryModelFirst(env.QueryExecuter);

            var forestryPieceModels = new TbForestryPieces()
                .AddFilterNot(t => t.flStatus, ForestObjectStatuses.Deleted.ToString())
                .AddFilter(t => t.flQuarter, objectModel.flId)
                .GetForestryPieceModels(env.QueryExecuter);

            var mainRow = new GridRow().AppendTo(widget);
            new GridCol("col-md-9")
                .Append(renderObjectData(objectModel, forestryPieceModels, isObjectSeller, env))
                .Append(renderFellingValue(forestryPieceModels, env))
                .Append(renderObjectForestryPieces(objectModel, forestryPieceModels, isObjectSeller, env))
                .AppendTo(mainRow);
            new GridCol("col-md-3")
                .Append(renderObjectGeometry(objectModel, forestryModel, forestryPieceModels, env))
                .AppendTo(mainRow);
        }

        private Card renderObjectData(QuarterModel objectModel, ForestryPieceModel[] forestryPieceModels, bool isObjectSeller, ActionEnv<QuarterViewArgs> env)
        {
            var objectData = new Card($"Квартал № {objectModel.flNumber}", objectModel.flStatus == ForestryQuarterStatuses.Deleted ? "text-decoration-line-through" : "");
            var objectDataActions = objectActions(objectModel, forestryPieceModels, isObjectSeller, env);
            if (objectDataActions.Count > 0)
            {
                new Panel("bg-white bordered border-top form-wizard-header mt-1 pt-2 pl-2").AppendRange(objectDataActions).AppendTo(objectData);
            }
            var tabPanel = new HyperTabs().AppendTo(objectData);

            var tbObject = new TbQuarters();

            var objectShortData = new Panel();
            tbObject.flId.RenderCustom(objectShortData, env, objectModel.flId, readOnly: true);
            tbObject.flForestry.RenderCustom(objectShortData, env, objectModel.flForestry, readOnly: true);
            tbObject.flStatus.RenderCustom(objectShortData, env, objectModel.flStatus, readOnly: true);
            tabPanel.AddNewTab("Краткая информация", objectShortData);

            var objectFullData = new Panel();
            MnuQuarterOrder.viewModelMainData(objectFullData, env, objectModel, true);
            tabPanel.AddNewTab("Подробная информация", objectFullData);

            return objectData;
        }

        private List<LinkBase> objectActions(QuarterModel objectModel, ForestryPieceModel[] forestryPieceModels, bool isObjectSeller, ActionEnv<QuarterViewArgs> env)
        {
            var actions = new List<LinkBase>();

            actions.Add(new Link {
                Text = env.T("Перейти к лесному хозяйству"),
                Controller = nameof(RegistersModule),
                Action = nameof(MnuForestryView),
                RouteValues = new ForestryViewArgs { MenuAction = "view", Id = objectModel.flForestry },
                CssClass = ActionCssClass
            });

            if (!isObjectSeller)
            {
                return actions;
            }

            if (objectModel.flStatus == ForestryQuarterStatuses.Active)
            {
                actions.Add(new Link
                {
                    CssClass = ActionCssClass,
                    Text = env.T("Редактировать объект"),
                    Controller = ModuleName,
                    Action = nameof(MnuQuarterOrder),
                    RouteValues = new QuarterOrderQueryArgs  { Id = objectModel.flId, RevisionId = objectModel.flRevisionId, OrderType = QuartersOrderTypeActions.Edit, MenuAction = "create-from" }
                });

                if (forestryPieceModels.Length == 0) {
                    actions.Add(new Link
                    {
                        CssClass = ActionCssClass,
                        Text = env.T("Удалить объект"),
                        Controller = ModuleName,
                        Action = nameof(MnuQuarterOrder),
                        RouteValues = new QuarterOrderQueryArgs { Id = objectModel.flId, RevisionId = objectModel.flRevisionId, OrderType = QuartersOrderTypeActions.Remove, MenuAction = "create-from" }
                    });
                }
            }
            return actions;
        }

        public Card renderFellingValue(ForestryPieceModel[] forestryPieceModels, ActionEnv<QuarterViewArgs> env) {
            var card = new Card(env.T("Объем рубки"));
            var tbObject = new TbForestryPieces();
            var flFellingValue = forestryPieceModels
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

        private Card renderObjectForestryPieces(QuarterModel objectModel, ForestryPieceModel[] forestryPieceModels, bool isObjectSeller, ActionEnv<QuarterViewArgs> env) {
            var quartersShortData = new Card(env.T("Лесные выделы"));
            var quartersShortDataActions = forestryPiecesActions(objectModel, isObjectSeller, env);
            if (quartersShortDataActions.Count > 0) {
                new Panel("bg-white bordered border-top form-wizard-header mt-1 pt-2 pl-2").AppendRange(quartersShortDataActions).AppendTo(quartersShortData);
            }

            forestryPieceModels.Each(forestryPieceModel => {
                new GridRow()
                    .Append(
                        new GridCol("col-md-3")
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
                    .Append(
                        new GridCol("col-md-9")
                            .Append(
                                new Heading(HeadingLevel.h4, $"Лесной выдел № {forestryPieceModel.flNumber}, площадь, га: {forestryPieceModel.flArea}, описание: {forestryPieceModel.flDescription}")
                            )
                    )
                    .AppendTo(quartersShortData)
                    ;
            });

            return quartersShortData;
        }

        private List<LinkBase> forestryPiecesActions(QuarterModel objectModel, bool isObjectSeller, ActionEnv<QuarterViewArgs> env) {
            var actions = new List<LinkBase>();

            if (!isObjectSeller) {
                return actions;
            }

            if (objectModel.flStatus == ForestryQuarterStatuses.Active) {
                actions.Add(new Link {
                    Controller = nameof(RegistersModule),
                    Action = nameof(MnuForestryPieceOrder),
                    RouteValues = new ForestryPieceOrderQueryArgs { RevisionId = -1, MenuAction = "create-new", QuarterId = objectModel.flId },
                    Text = env.T("Добавить лесной выдел"),
                    CssClass = "btn btn-success"
                });
            }
            return actions;
        }

        public Card renderObjectGeometry(QuarterModel objectModel, ForestryModel forestryModel, ForestryPieceModel[] forestryPieceModels, ActionEnv<QuarterViewArgs> env)
        {
            var card = new Card(env.T("Геометрия"))
                .Append(new ObjectGeometryViewerComponent(objectModel.flWKT, wktsNeighbours: forestryPieceModels.Select(x => x.flWKT).Concat(new[] { forestryModel.flWKT }).ToArray()));
            return card;
        }
    }

    public class QuarterViewArgs : ActionQueryArgsBase {
        public int Id { get; set; }
    }

}