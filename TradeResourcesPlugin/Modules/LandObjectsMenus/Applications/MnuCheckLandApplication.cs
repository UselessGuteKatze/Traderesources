using LandSource.QueryTables.Applications;
using System;
using System.Collections.Generic;
using System.Text;
using Yoda.Interfaces;
using YodaHelpers.ActionMenus;
using YodaHelpers.Components;
using YodaHelpers.Fields;
using YodaQuery;

namespace TradeResourcesPlugin.Modules.LandObjectsMenus.Applications {

    public class MnuCheckLandApplicationArgs : ActionQueryArgsBase {
        public int? flApplicationId { get; set; }
        public string flApplicantXin { get; set; }
    }
    public class MnuCheckLandApplication : MnuActionsExt<MnuCheckLandApplicationArgs> {
        public const string MnuName = nameof(MnuCheckLandApplication);
        public MnuCheckLandApplication(string moduleName) : base(moduleName, MnuName, "Проверка заявления")
        {
        }

        protected override string GetDefaultActionName()
        {
            return Actions.Check;
        }

        public override void Configure(ActionConfig<MnuCheckLandApplicationArgs> config)
        {
            config
                .OnAction(Actions.Check, c => {
                    return c
            .IsValid(env =>
            {
                return new OkResult();
            })
            .Wizard(wizard => wizard
                .Args(args => args.Args)
                .Model(env =>
                {
                    var model = new MnuCheckLandApplicationArgs
                    {
                        flApplicationId = env.Args.flApplicationId,
                        flApplicantXin = env.Args.flApplicantXin
                    };

                    return model;
                })
                .CancelBtn("Отмена", env => new ActionRedirectData(ModuleName, MnuName, env.Args))
                .FinishBtn(string.Empty)
                .Step("Данные заявки", step =>
                {
                    return step
                    .OnRendering(env =>
                    {
                        var model = env.Model;
                        var tbApps = new TbLandApplications();
                        tbApps.flId.RenderCustom(env.Panel, env.Env, model.flApplicationId);
                        tbApps.flApplicantXin.RenderCustom(env.Panel, env.Env, model.flApplicantXin);

                    })
                    .OnValidating(env =>
                    {
                        var tbApps = new TbLandApplications();
                        var checkFields = new Field[] {
                                    tbApps.flId,
                                    tbApps.flApplicantXin
                        };
                        checkFields.Each(f => f.Validate(env.Env));

                        if (env.Env.IsValid)
                        {
                            if (tbApps.flId.GetValOrNull(env.Env) == null)
                            {
                                env.Env.AddError(tbApps.flId.FieldName, env.Env.T($"Поле \"{tbApps.flId.Text}\" обязательно для заполнения!"));
                            }
                            if (string.IsNullOrEmpty(tbApps.flApplicantXin.GetVal(env.Env)))
                            {
                                env.Env.AddError(tbApps.flApplicantXin.FieldName, env.Env.T($"Поле \"{tbApps.flApplicantXin.Text}\" обязательно для заполнения!"));
                            }

                            if (env.Env.IsValid)
                            {
                                var appExists = tbApps
                                    .AddFilter(t => t.flId, tbApps.flId.GetVal(env.Env))
                                    .AddFilter(t => t.flApplicantXin, tbApps.flApplicantXin.GetVal(env.Env))
                                    .Count(env.Env.QueryExecuter) > 0;
                                if (!appExists)
                                {
                                    if (tbApps.flId.GetValOrNull(env.Env) == null)
                                    {
                                        env.Env.AddError(tbApps.flId.FieldName, env.Env.T($"Заявление не существует."));
                                    }
                                    if (string.IsNullOrEmpty(tbApps.flApplicantXin.GetVal(env.Env)))
                                    {
                                        env.Env.AddError(tbApps.flApplicantXin.FieldName, string.Empty);
                                    }
                                }
                            }
                        }
                    })
                    .OnProcessing(env =>
                    {
                        var model = env.Model;
                        var tbApps = new TbLandApplications();

                        model.flApplicationId = tbApps.flId.GetVal(env.Env);
                        model.flApplicantXin = tbApps.flApplicantXin.GetVal(env.Env);
                    })
                    ;
                })
                .Step("Заявка", step =>
                {
                    return step
                    .OnRendering(env =>
                    {
                        var model = env.Model;
                        var tbApps = new TbLandApplications();
                        tbApps.AddFilter(t => t.flId, model.flApplicationId);
                        tbApps.flId.RenderCustom(env.Panel, env.Env, model.flApplicationId, readOnly: true);
                        tbApps.flApplicantXin.RenderCustom(env.Panel, env.Env, model.flApplicantXin, readOnly: true);
                        tbApps.flSignSendDate.RenderCustom(env.Panel, env.Env, tbApps.SelectScalar(t => t.flSignSendDate, env.Env.QueryExecuter), readOnly: true);

                    })
                    ;
                })
               .Build()
            );
                })
                ;
        }

        public class Actions {
            public const string
            Check = "check";
        }
    }

}
