using System;
using System.Collections.Generic;
using Yoda.Interfaces;
using YodaQuery;
using YodaHelpers.ActionMenus;
using YodaHelpers.Fields;
using YodaHelpers.OrderHelpers;
using System.Threading.Tasks;
using CommonSource.QueryTables;

namespace TradeResourcesPlugin.Modules.Administration.OfficialOrgs {
    public class MnuOfficialOrg : MnuActions<OfficialOrgQueryArgs> {
        public const string MnuName = "MnuOfficialOrg";

        private readonly ViewWithOrderStandartPermissions _perms;
        public MnuOfficialOrg(string moduleName, ViewWithOrderStandartPermissions perms) : base(moduleName, MnuName, "Компетентный орган") {
            AsCallback();
            _perms = perms;
            Access(_perms.ViewObject.Name);
        }

        public override Task<ActionItem[]> GetActions(ActionEnv<OfficialOrgQueryArgs> env) {
            var ret = new List<ActionItem>();

            var revId = new TbOfficialOrgOrderResult()
                .AddFilterNotFinishedOrders()
                .JoinT("tbOrders", new TbOfficialOrgRevisions().AddFilter(t => t.flOrgId, env.Args.OrgId), "tbRevs")
                .On((t1, t2) => new Join(t1.flSubjectId, t2.flRevisionId))
                .SelectScalar(t => t.L.flSubjectId, env.QueryExecuter);

            if (revId == null) {
                ret.Add(new ActionItem(
                    env.T("Редактировать"),
                    new ActionRedirectData(ModuleName,
                        MnuOfficialOrgOrder.MnuName,
                        new OfficialOrgOrderQueryArgs { OrgId = env.Args.OrgId, MenuAction = MnuOfficialOrgOrder.Actions.CreateFrom }
                    )
                ));
            } else {
                ret.Add(new ActionItem(
                    env.T("Открыть приказ"),
                    new ActionRedirectData(ModuleName,
                        MnuOfficialOrgOrder.MnuName,
                        new OfficialOrgOrderQueryArgs { RevisionId = revId, MenuAction = MnuOfficialOrgOrder.Actions.ViewOrder }
                    )
                ));
            }
            return Task.FromResult(ret.ToArray());
        }

        public override Task<IsActionValidResult> IsActionValid(ActionEnv<OfficialOrgQueryArgs> env) {
            var count = new TbOfficialOrg()
                 .AddFilter(t => t.flOrgId, env.Args.OrgId)
                 .Count(env.QueryExecuter);
            if (count == 0) {
                return Task.FromResult(new IsActionValidResult(false, new ActionValidityFailRedirectData(new ArgumentException())));
            }
            return Task.FromResult(new IsActionValidResult(true, null));
        }

        public override Task OnRendering(RenderActionEnv<OfficialOrgQueryArgs> env) {
            var tbOrg = new TbOfficialOrg().AddFilter(t => t.flOrgId, env.Args.OrgId);
            var r = tbOrg.SelectFirst(t => t.Fields.ToFieldsAliases(), env.QueryExecuter);
            var valsBag = new ValuesBag(r.FirstRow);

            if (env.User.IsAuthentificated) {
                new Field[]{
                    tbOrg.flBin,
                    tbOrg.flNameRu,
                    tbOrg.flNameKz,
                    tbOrg.flOpf,
                    tbOrg.flRegistrationDate,
                    tbOrg.flRegistrationNum,
                    tbOrg.flFirstPerson,
                    tbOrg.flFirstPersonIin,
                    tbOrg.flAccountant,
                    tbOrg.flAccountantIin,

                    tbOrg.flAdrCountry,
                    tbOrg.flAdrIndex,
                    tbOrg.flAdrObl,
                    tbOrg.flAdrReg,
                    tbOrg.flAdrMail,
                    tbOrg.flAdrMobile,
                    tbOrg.flAdrPhone,
                    tbOrg.flAdrWeb,
                    tbOrg.flActivityTypes,
                    tbOrg.flNote,
                }.Each(f => f.RenderCustom(env.Form, env, valsBag.GetValueOrDefault(f), readOnly: true));
            } else {
                new Field[]{
                    tbOrg.flBin,
                    tbOrg.flNameRu,
                    tbOrg.flNameKz,
                    tbOrg.flOpf,
                    tbOrg.flRegistrationDate,
                    tbOrg.flRegistrationNum,
                    tbOrg.flFirstPerson,
                    tbOrg.flAccountant,
                    tbOrg.flAdrCountry,
                    tbOrg.flAdrIndex,
                    tbOrg.flAdrObl,
                    tbOrg.flAdrReg,
                    tbOrg.flAdrMail,
                    tbOrg.flAdrMobile,
                    tbOrg.flAdrPhone,
                    tbOrg.flAdrWeb,
                    tbOrg.flActivityTypes,
                    tbOrg.flNote,
                }.Each(f => f.RenderCustom(env.Form, env, valsBag.GetValueOrDefault(f), readOnly: true));
            }

            return Task.CompletedTask;
        }

        public override Task OnValidating(ActionEnv<OfficialOrgQueryArgs> env) {
            throw new System.NotImplementedException();
        }

        public override Task OnProcessing(ActionEnv<OfficialOrgQueryArgs> env) {
            throw new System.NotImplementedException();
        }

        public class Actions {
            public const string View = "view";
        }
    }

    public class OfficialOrgQueryArgs : ActionQueryArgsBase {
        public int OrgId { get; set; }
    }
}
