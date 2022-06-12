using ForestSource.Helpers.Object;
using ForestSource.Models;
using ForestSource.QueryTables.Object;
using ForestSource.References.Object;
using System.Collections.Generic;
using System.Linq;
using TradeResourcesPlugin.Helpers;
using TradeResourcesPlugin.Modules.Components;
using TradeResourcesPlugin.Modules.ForestMenus.Objects;
using TradeResourcesPlugin.Modules.ForestMenus.Quarters;
using UsersResources;
using Yoda.Interfaces.Forms;
using Yoda.Interfaces.Forms.Components;
using YodaHelpers.ActionMenus;
using YodaHelpers.Fields;
using YodaQuery;

namespace TradeResourcesPlugin.Modules.ForestMenus.ForestryPieces {
    public class MnuForestryPieceView : MnuActionsExt<ForestryPieceViewArgs> {
        public const string MnuName = nameof(MnuForestryPieceView);

        public MnuForestryPieceView(string moduleName) : base(moduleName, MnuName, "Лесной выдел")
        {
            AsCallback();
            //Enabled(rc => rc.User.IsAuthentificated);
        }

        public class Actions {
            public const string View = "view";
        }

        public const string ActionCssClass = "btn btn-sm btn-dark font-weight-normal text-wrap mr-2 mb-2";

        public override void Configure(ActionConfig<ForestryPieceViewArgs> config)
        {
            config
                .OnAction(Actions.View, action => action
                    .IsValid(env => new OkResult())
                    .OnRendering(re => {
                        renderObjectView(re.Form, re);
                    }));
        }

        private void renderObjectView(WidgetBase widget, ActionEnv<ForestryPieceViewArgs> env)
        {
            var objectModel = new TbForestryPieces()
                .AddFilter(t => t.flId, env.Args.Id)
                .GetForestryPieceModelFirst(env.QueryExecuter);

            var objectSeller = objectModel.flSellerBin;
            var currentUser = env.User.GetUserXin(env.QueryExecuter);
            var isObjectSeller = currentUser == objectSeller && env.User.HasRole("TRADERESOURCES-Лесные ресурсы-Создание объектов", env.QueryExecuter)/*env.User.HasCustomRole("forestobjects", "dataEdit", env.QueryExecuter)*/;
            //var isAgreementSigner = currentUser == objectSeller && env.User.HasCustomRole("forestobjects", "dataEdit", env.QueryExecuter);

            var quarterModel = new TbQuarters()
                .AddFilter(t => t.flId, objectModel.flQuarter)
                .GetQuarterModelFirst(env.QueryExecuter);
            var forestryModel = new TbForestries()
                .AddFilter(t => t.flId, quarterModel.flForestry)
                .GetForestryModelFirst(env.QueryExecuter);

            var mainRow = new GridRow().AppendTo(widget);
            new GridCol("col-md-9")
                .Append(renderObjectData(objectModel, isObjectSeller, env))
                .Append(renderFellingValue(objectModel, env))
                .AppendTo(mainRow);
            new GridCol("col-md-3")
                .Append(renderObjectGeometry(objectModel, quarterModel, forestryModel, env))
                .AppendTo(mainRow);
        }

        private Card renderObjectData(ForestryPieceModel objectModel, bool isObjectSeller, ActionEnv<ForestryPieceViewArgs> env)
        {
            var objectData = new Card($"Лесной выдел № {objectModel.flNumber}", objectModel.flStatus == ForestryPieceStatuses.Deleted ? "text-decoration-line-through" : "");
            var objectDataActions = objectActions(objectModel, isObjectSeller, env);
            if (objectDataActions.Count > 0)
            {
                new Panel("bg-white bordered border-top form-wizard-header mt-1 pt-2 pl-2").AppendRange(objectDataActions).AppendTo(objectData);
            }
            var tabPanel = new HyperTabs().AppendTo(objectData);

            var tbObject = new TbForestryPieces();

            var objectShortData = new Panel();
            tbObject.flId.RenderCustom(objectShortData, env, objectModel.flId, readOnly: true);
            tbObject.flQuarter.RenderCustom(objectShortData, env, objectModel.flQuarter, readOnly: true);
            tbObject.flStatus.RenderCustom(objectShortData, env, objectModel.flStatus.ToString(), readOnly: true);
            tbObject.flBlock.RenderCustom(objectShortData, env, objectModel.flBlock.ToString(), readOnly: true);
            tabPanel.AddNewTab("Краткая информация", objectShortData);

            var objectFullData = new Panel();
            MnuForestryPieceOrder.viewModelMainData(objectFullData, env, objectModel, true);
            tabPanel.AddNewTab("Подробная информация", objectFullData);

            return objectData;
        }

        private List<LinkBase> objectActions(ForestryPieceModel objectModel, bool isObjectSeller, ActionEnv<ForestryPieceViewArgs> env)
        {
            var actions = new List<LinkBase>();

            actions.Add(new Link {
                Text = env.T("Перейти к кварталу"),
                Controller = nameof(RegistersModule),
                Action = nameof(MnuQuarterView),
                RouteValues = new QuarterViewArgs { MenuAction = "view", Id = objectModel.flQuarter },
                CssClass = ActionCssClass
            });

            var forestObjectIdFirst = new TbObjects()
                .AddFilter(t => t.flForestryPieces, ConditionOperator.ContainsWord, $"\"SearchItemId\":{objectModel.flId}")
                .AddFilter(t => t.flBlock, objectModel.flBlock.ToString())
                .Select(t => new FieldAlias[] { t.flId }, env.QueryExecuter);
            if (forestObjectIdFirst.Count() > 0) {
                actions.Add(new Link {
                    Text = env.T("Перейти к объекту"),
                    Controller = nameof(RegistersModule),
                    Action = nameof(MnuForestObjectView),
                    RouteValues = new ForestObjectViewArgs { MenuAction = "view", Id = forestObjectIdFirst.First().GetVal(t => t.flId) },
                    CssClass = ActionCssClass
                });
            }

            if (!isObjectSeller)
            {
                return actions;
            }

            if (objectModel.flBlock == ForestryPieceBlocks.ActiveFree)
            {
                actions.Add(new Link
                {
                    CssClass = ActionCssClass,
                    Text = env.T("Редактировать объект"),
                    Controller = ModuleName,
                    Action = nameof(MnuForestryPieceOrder),
                    RouteValues = new ForestryPieceOrderQueryArgs  { Id = objectModel.flId, RevisionId = objectModel.flRevisionId, OrderType = ForestryPiecesOrderTypeActions.Edit, MenuAction = "create-from" }
                });
                actions.Add(new Link
                {
                    CssClass = ActionCssClass,
                    Text = env.T("Удалить объект"),
                    Controller = ModuleName,
                    Action = nameof(MnuForestryPieceOrder),
                    RouteValues = new ForestryPieceOrderQueryArgs { Id = objectModel.flId, RevisionId = objectModel.flRevisionId, OrderType = ForestryPiecesOrderTypeActions.Remove, MenuAction = "create-from" }
                });
            }
            return actions;
        }

        public Card renderFellingValue(ForestryPieceModel objectModel, ActionEnv<ForestryPieceViewArgs> env)
        {
            var card = new Card(env.T("Объем рубки"));
            var tbObject = new TbForestryPieces();
            tbObject.flFellingValue.RenderCustom(card, env, objectModel.flFellingValue, readOnly: true, hideLabel: true);
            return card;
        }

        public Card renderObjectGeometry(ForestryPieceModel objectModel, QuarterModel quarterModel, ForestryModel forestryModel, ActionEnv<ForestryPieceViewArgs> env)
        {
            var card = new Card(env.T("Геометрия"))
                .Append(new ObjectGeometryViewerComponent(objectModel.flWKT, wktsNeighbours: new[] { quarterModel.flWKT, forestryModel.flWKT }));
            return card;
        }
    }

    public class ForestryPieceViewArgs : ActionQueryArgsBase {
        public int Id { get; set; }
    }

}