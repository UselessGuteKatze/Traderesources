using Yoda.Interfaces.Forms;
using Yoda.Interfaces.Forms.Components;
using YodaHelpers.ActionMenus;
using Yoda.Interfaces;
using FishingSource.QueryTables.Reservoir;
using System.Collections.Generic;
using TradeResourcesPlugin.Helpers;
using Microsoft.AspNetCore.Mvc.Routing;
using UsersResources;
using FishingSource.Helpers.Reservoir;
using YodaApp.YodaHelpers.Components;
using FishingSource.References.Reservoir;
using FishingSource.Models;

namespace TradeResourcesPlugin.Modules.FishingMenus.Reservoirs {
    public class MnuReservoirView: MnuActionsExt<ReservoirViewActionQueryArgs> {
        public const string MnuName = nameof(MnuReservoirView);

        public MnuReservoirView(string moduleName) : base(moduleName, MnuName, "Водоёмы") {
            AsCallback();
            //Enabled(rc => rc.User.IsAuthentificated);
        }

        public class Actions {
            public const string View = "view";
        }

        private void renderRedirectButtons(RenderActionEnv<ReservoirViewActionQueryArgs> re) {

            if (!re.User.HasRole("TRADERESOURCES-Рыбохозяйственные водоёмы-Создание приказов", re.QueryExecuter)/*!re.User.HasCustomRole("fishingobjects", "dataEdit", re.QueryExecuter)*/)
            {
                return;
            }

            var obj = ReservoirHelper.GetReservoirModel(re.Args.Id, re.QueryExecuter);

            if (obj.flStatus == ReservoirStatuses.Active.ToString()) {
                re.RequestContext.AddLocalTask(new Link {
                    Text = re.T("Редактировать объект"),
                    Controller = ModuleName,
                    Action = nameof(MnuReservoirOrderBase),
                    RouteValues = new ReservoirOrderQueryArgs { Id = obj.flId, RevisionId = obj.flRevisionId, OrderType = ReservoirsOrderTypeActions.Edit, MenuAction = "create-from" }
                });
                re.RequestContext.AddLocalTask(new Link {
                    Text = re.T("Удалить объект"),
                    Controller = ModuleName,
                    Action = nameof(MnuReservoirOrderBase),
                    RouteValues = new ReservoirOrderQueryArgs { Id = obj.flId, RevisionId = obj.flRevisionId, OrderType = ReservoirsOrderTypeActions.Remove, MenuAction = "create-from" }
                });
            }

        }

        private void renderObjectMainData(ActionEnv<ReservoirViewActionQueryArgs> env, WidgetBase container) {
            var tbObj = new TbReservoirs();
            var model = ReservoirHelper.GetReservoirModel(env.Args.Id, env.QueryExecuter);
            var groupbox = new Accordion(env.T("Объект"));
            container.AddComponent(groupbox);

            MnuReservoirOrderBase.viewModelMainData(groupbox, env, model);
            MnuReservoirOrderBase.viewModelGeoPositionData(groupbox, env, model);
            MnuReservoirOrderBase.viewModelGeometryData(groupbox, env, model);

        }

        private void renderInfoPage(RenderActionEnv<ReservoirViewActionQueryArgs> re) {
        }

        public override void Configure(ActionConfig<ReservoirViewActionQueryArgs> config) {
            config
                .OnAction(Actions.View, action => action
                    .IsValid(env => new OkResult())
                    .OnRendering(re => {
                        renderRedirectButtons(re);
                        renderInfoPage(re);
                        renderStageList(re.Form, re);
                    }));
        }

        private void renderStageList(WidgetBase widget, ActionEnv<ReservoirViewActionQueryArgs> env) {
            var blockWeights = new Dictionary<ReservoirStatuses, int> {
                {ReservoirStatuses.Active, 1 },
                {ReservoirStatuses.Deleted, 2 }
            };

            int getBlockWeight(ReservoirStatuses block) {
                if (blockWeights.ContainsKey(block)) {
                    return blockWeights[block];
                }
                throw new KeyNotFoundException($"Block: {block}");
            }
            StageState getStageState(ReservoirStatuses curObjectBlockVal, ReservoirStatuses blockVal) {
                var curBlockWeight = getBlockWeight(curObjectBlockVal);
                var blockWeight = getBlockWeight(blockVal);
                if (blockWeight < curBlockWeight) {
                    return StageState.Done;
                }
                if (blockWeight == curBlockWeight) {
                    return StageState.Active;
                }
                return StageState.Inactive;
            }

            var urlHelper = env.RequestContext.GetUrlHelper();
            var xin = env.User.GetUserXin(env.QueryExecuter);
            var model = ReservoirHelper.GetReservoirModel(env.Args.Id, env.QueryExecuter);

            var panel = new StageList()
                .Stage(stage => stage
                    .Model(() => {
                        return new ReservoirApplicationStepModel(model, StageState.Done);
                    })
                    .Title(model => {
                        return env.T("Информация об объекте");
                    })
                    .State(model => StageState.Done)
                    .SkipDescription()
                    .SkipButtons()
                    .Render((model, container) => {
                        renderObjectMainData(env, container);
                    })
                )
                .Build();

            widget.AddComponent(panel);
        }
    }

    public class ReservoirViewActionQueryArgs: ActionQueryArgsBase {
        public int Id { get; set; }
    }

}
