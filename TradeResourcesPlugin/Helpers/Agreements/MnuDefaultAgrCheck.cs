using System;
using System.Collections.Generic;
using System.Text;
using TradeResourcesPlugin.Helpers;
using Yoda.Interfaces;
using YodaHelpers.ActionMenus;
using YodaHelpers.Components;
using YodaHelpers.Fields;
using YodaQuery;

namespace TradeResourcesPlugin.Helpers {

    public class MnuDefaultAgrCheckArgs : ActionQueryArgsBase {
        public int? flAgreementId { get; set; }
        public string flSellerBin { get; set; }
    }
    public class MnuDefaultAgrCheck : MnuActionsExt<MnuDefaultAgrCheckArgs> {
        public const string MnuName = nameof(MnuDefaultAgrCheck);
        public MnuDefaultAgrCheck(string moduleName) : base(moduleName, MnuName, "Проверка договоров")
        {
        }

        protected override string GetDefaultActionName()
        {
            return Actions.Check;
        }

        public override void Configure(ActionConfig<MnuDefaultAgrCheckArgs> config)
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
                    var model = new MnuDefaultAgrCheckArgs
                    {
                        flAgreementId = env.Args.flAgreementId,
                        flSellerBin = env.Args.flSellerBin
                    };

                    return model;
                })
                .CancelBtn("Отмена", env => new ActionRedirectData(ModuleName, MnuName, env.Args))
                .FinishBtn(string.Empty)
                .Step("Данные договора", step =>
                {
                    return step
                    .OnRendering(env =>
                    {
                        var model = env.Model;
                        var tbApps = new TbAgreements();
                        tbApps.flAgreementId.RenderCustom(env.Panel, env.Env, model.flAgreementId);
                        tbApps.flSellerBin.RenderCustom(env.Panel, env.Env, model.flSellerBin);

                    })
                    .OnValidating(env =>
                    {
                        var tbApps = new TbAgreements();
                        var checkFields = new Field[] {
                                    tbApps.flAgreementId,
                                    tbApps.flSellerBin
                        };
                        checkFields.Each(f => f.Validate(env.Env));

                        if (env.Env.IsValid)
                        {
                            if (tbApps.flAgreementId.GetValOrNull(env.Env) == null)
                            {
                                env.Env.AddError(tbApps.flAgreementId.FieldName, env.Env.T($"Поле \"{tbApps.flAgreementId.Text}\" обязательно для заполнения!"));
                            }
                            if (string.IsNullOrEmpty(tbApps.flSellerBin.GetVal(env.Env)))
                            {
                                env.Env.AddError(tbApps.flSellerBin.FieldName, env.Env.T($"Поле \"{tbApps.flSellerBin.Text}\" обязательно для заполнения!"));
                            }

                            if (env.Env.IsValid)
                            {
                                var appExists = tbApps
                                    .AddFilter(t => t.flAgreementId, tbApps.flAgreementId.GetVal(env.Env))
                                    .AddFilter(t => t.flSellerBin, tbApps.flSellerBin.GetVal(env.Env))
                                    .Count(env.Env.QueryExecuter) > 0;
                                if (!appExists)
                                {
                                    if (tbApps.flAgreementId.GetValOrNull(env.Env) == null)
                                    {
                                        env.Env.AddError(tbApps.flAgreementId.FieldName, env.Env.T($"Заявление не существует."));
                                    }
                                    if (string.IsNullOrEmpty(tbApps.flSellerBin.GetVal(env.Env)))
                                    {
                                        env.Env.AddError(tbApps.flSellerBin.FieldName, string.Empty);
                                    }
                                }
                            }
                        }
                    })
                    .OnProcessing(env =>
                    {
                        var model = env.Model;
                        var tbApps = new TbAgreements();

                        model.flAgreementId = tbApps.flAgreementId.GetVal(env.Env);
                        model.flSellerBin = tbApps.flSellerBin.GetVal(env.Env);
                    })
                    ;
                })
                .Step("Договор", step =>
                {
                    return step
                    .OnRendering(env =>
                    {
                        var model = env.Model;
                        var tbApps = new TbAgreements();
                        tbApps.AddFilter(t => t.flAgreementId, model.flAgreementId);
                        tbApps.flAgreementId.RenderCustom(env.Panel, env.Env, model.flAgreementId, readOnly: true);
                        tbApps.flSellerBin.RenderCustom(env.Panel, env.Env, model.flSellerBin, readOnly: true);
                        tbApps.flWinnerXin.RenderCustom(env.Panel, env.Env, tbApps.SelectScalar(t => t.flWinnerXin, env.Env.QueryExecuter), readOnly: true);
                        tbApps.flAgreementStatus.RenderCustom(env.Panel, env.Env, tbApps.SelectScalar(t => t.flAgreementStatus, env.Env.QueryExecuter), readOnly: true);
                        tbApps.flAgreementSignDate.RenderCustom(env.Panel, env.Env, tbApps.SelectScalar(t => t.flAgreementSignDate, env.Env.QueryExecuter), readOnly: true);

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
