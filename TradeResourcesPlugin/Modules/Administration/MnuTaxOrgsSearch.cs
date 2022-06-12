using CommonSource.QueryTables;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UsersResources;
using Yoda.Interfaces;
using Yoda.Interfaces.Forms;
using Yoda.Interfaces.Forms.Components;
using Yoda.Interfaces.Helpers;
using Yoda.Interfaces.Menu;
using YodaHelpers.ActionMenus;
using YodaHelpers.Fields;
using YodaQuery;

namespace TradeResourcesPlugin.Modules.Administration {
    public class MnuTaxOrgsSearch: FrmMenu {

        public const string MenuName = nameof(MnuTaxOrgsSearch);
        public MnuTaxOrgsSearch(string moduleName) : base(MenuName, "Налоговые органы")
        {
            MenuType(Yoda.Interfaces.Menu.MenuType.Normal);
            Enabled((rc) => {
                var xin = rc.User.GetUserXin(rc.QueryExecuter);
                // IAC
                if ((!rc.User.IsExternalUser() && !rc.User.IsGuest()))
                {
                    return true;
                }

                return false;
            });
            OnRendering(re => {

                var tbTaxOrgs = new TbTaxOrgs();

                tbTaxOrgs
                    .ToSearchWidget(re)
                    .AddToolbarItem(new Link {
                        Controller = moduleName,
                        Action = MnuTaxOrg.MenuName,
                        RouteValues = new MnuTaxOrgQueryArgs { Id = -1, MenuAction = MnuTaxOrg.Actions.Create },
                        Text = re.T("Добавить объекты")
                    })
                    .AddFilters(tbTaxOrgs.flBin, tbTaxOrgs.flStatus)
                    .HideSearchButton(false)
                    .AddReturnFields(tbTaxOrgs.flBin, tbTaxOrgs.flNameRu, tbTaxOrgs.flNameKz, tbTaxOrgs.flStatus)
                    .AddHiddenFields(tbTaxOrgs.flId)
                    .AddRowActions(r => new Link(re.T("Открыть"), moduleName, MnuTaxOrg.MenuName, new MnuTaxOrgQueryArgs { Id = tbTaxOrgs.flId.GetRowVal(r, "flId"), MenuAction = MnuTaxOrg.Actions.View }, true, null, "btn btn-default btn-sm"))
                    .AutoExecuteQuery(true)
                    .Print(re.Form, nameof(MnuTaxOrgsSearch));

            });
        }

    }

    public class MnuTaxOrg: MnuActions<MnuTaxOrgQueryArgs> {

        public const string MenuName = nameof(MnuTaxOrgQueryArgs);

        public class Actions {
            public const string View = "view";
            public const string Delete = "delete";
            public const string Recover = "recover";
            public const string Create = "create";
        }

        public MnuTaxOrg(string moduleName) : base(moduleName, MenuName, "Налоговый орган")
        {
            AsCallback();
            PageTitle("Налоговый орган");
        }

        public override Task<IsActionValidResult> IsActionValid(ActionEnv<MnuTaxOrgQueryArgs> env)
        {
            ActionValidityFailRedirectData redirectIfNotValid;
            bool valid;

            switch (env.Args.MenuAction)
            {
                case Actions.Create:
                    if (env.User.IsExternalUser())
                    {
                        valid = ActionAccessDenied(out redirectIfNotValid);
                        return Task.FromResult(new IsActionValidResult(valid, redirectIfNotValid));
                    }
                    valid = ActionValid(out redirectIfNotValid);
                    return Task.FromResult(new IsActionValidResult(valid, redirectIfNotValid));
                case Actions.View:
                case Actions.Delete:
                case Actions.Recover:

                    if (env.User.IsExternalUser())
                    {
                        valid = ActionAccessDenied(out redirectIfNotValid);
                        return Task.FromResult(new IsActionValidResult(valid, redirectIfNotValid));
                    }

                    if (env.Args.Id > 0)
                    {
                        var tbTaxOrgs = new TbTaxOrgs();
                        tbTaxOrgs.AddFilter(tbTaxOrgs.flId, env.Args.Id);
                        var r = tbTaxOrgs.SelectFirstOrDefault(t => t.Fields.ToAliases(), env.QueryExecuter);
                        if (!r.IsFirstRowExists)
                        {
                            valid = ActionAccessDenied(out redirectIfNotValid);
                            return Task.FromResult(new IsActionValidResult(valid, redirectIfNotValid));
                        }
                    } else
                    {
                        valid = ActionAccessDenied(out redirectIfNotValid);
                        return Task.FromResult(new IsActionValidResult(valid, redirectIfNotValid));
                    }

                    valid = ActionValid(out redirectIfNotValid);
                    return Task.FromResult(new IsActionValidResult(valid, redirectIfNotValid));
                default:
                    throw new NotImplementedException($"curAction:{env.Args.MenuAction}");
            }
        }


        public override Task<ActionItem[]> GetActions(ActionEnv<MnuTaxOrgQueryArgs> env)
        {

            var ret = new List<ActionItem>();

            switch (env.Args.MenuAction)
            {
                case Actions.View:

                    var tbTaxOrgs = new TbTaxOrgs();
                    tbTaxOrgs.AddFilter(tbTaxOrgs.flId, env.Args.Id);
                    var r = tbTaxOrgs.SelectFirstOrDefault(t => t.Fields.ToAliases(), env.QueryExecuter);

                    if ((!env.User.IsExternalUser() && !env.User.IsGuest()))
                    {
                        if (tbTaxOrgs.flStatus.GetRowVal(r.FirstRow) == RefTaxOrgStatuses.Values.ACT)
                        {
                            ret.Add(new ActionItem(env.T("Удалить"), Actions.Delete));
                        } else if (tbTaxOrgs.flStatus.GetRowVal(r.FirstRow) == RefTaxOrgStatuses.Values.DEL)
                        {
                            ret.Add(new ActionItem(env.T("Восстановить"), Actions.Recover));
                        }
                    }

                    break;
                case Actions.Create:
                    ret.Add(new ActionItem(env.T("Отмена"), new ActionRedirectData(ModuleName, MnuTaxOrgsSearch.MenuName)));
                    break;
                case Actions.Delete:
                case Actions.Recover:
                    break;
                default:
                    throw new NotImplementedException($"curAction:{env.Args.MenuAction}");
            }

            return Task.FromResult(ret.ToArray());

        }

        public override Task OnRendering(RenderActionEnv<MnuTaxOrgQueryArgs> env)
        {
            void render(bool isReadonly)
            {
                var tbTaxOrgs = new TbTaxOrgs();
                tbTaxOrgs.AddFilter(tbTaxOrgs.flId, env.Args.Id);
                var r = tbTaxOrgs.SelectFirstOrDefault(t => t.Fields.ToAliases(), env.QueryExecuter);
                var valuesBag = new ValuesBag();
                if (r.IsFirstRowExists)
                {
                    valuesBag.Set(r.FirstRow);
                }

                if (isReadonly)
                {
                    new Field[] {
                        tbTaxOrgs.flBin,
                        tbTaxOrgs.flNameRu,
                        tbTaxOrgs.flNameKz,
                        tbTaxOrgs.flStatus
                    }.Each(f => f.RenderCustom(env.Form, env, valuesBag.GetValueOrDefault(f), readOnly: isReadonly));

                    env.Form.AddSubmitButton("update", env.T("Обновить"), "btn btn-success");

                }
                else
                {
                    tbTaxOrgs.flBin.RenderCustom(env.Form, env);
                    env.Form.AddSubmitButton("save", env.T("Сохранить"), "btn btn-success");
                }
            }
            switch (env.CurAction)
            {
                case null:
                case Actions.View:
                    render(true);
                    return Task.CompletedTask;
                case Actions.Create:
                    render(false);
                    return Task.CompletedTask;
                case Actions.Delete:
                case Actions.Recover:
                    var tbTaxOrgs = new TbTaxOrgs();
                    tbTaxOrgs.AddFilter(tbTaxOrgs.flId, env.Args.Id);
                    var update = tbTaxOrgs.Update();
                    update.Set(t => t.flStatus, (env.CurAction == Actions.Delete ? RefTaxOrgStatuses.Values.DEL : RefTaxOrgStatuses.Values.ACT));
                    update.Execute(env.QueryExecuter);
                    env.Redirect.SetRedirect(ModuleName, MenuName, new MnuTaxOrgQueryArgs() { Id = env.Args.Id, MenuAction = Actions.View });
                    return Task.CompletedTask;
                default:
                    throw new NotImplementedException($"Unknown action:{env.CurAction}");
            }
        }

        public override Task OnValidating(ActionEnv<MnuTaxOrgQueryArgs> env)
        {

            var tbTaxOrgs = new TbTaxOrgs();
            tbTaxOrgs.AddFilter(tbTaxOrgs.flId, env.Args.Id);

            switch (env.CurAction)
            {
                case Actions.View:

                    var bin = tbTaxOrgs.flBin.GetRowVal(tbTaxOrgs.SelectFirstOrDefault(t => t.Fields.ToAliases(), env.QueryExecuter).FirstRow);
                    hasInGrObjects(bin, env.RequestContext);

                    return Task.CompletedTask;
                case Actions.Create:

                    var newBin = tbTaxOrgs.flBin.GetVal(env);
                    hasInGrObjects(newBin, env.RequestContext);

                    var tbTaxOrgsTest = new TbTaxOrgs();
                    tbTaxOrgsTest.AddFilter(tbTaxOrgsTest.flBin, newBin);
                    var r = tbTaxOrgsTest.SelectFirstOrDefault(t => t.Fields.ToAliases(), env.QueryExecuter);
                    if (r.IsFirstRowExists) {
                        env.SetPostbackMessage(env.T("Этот орган уже существует"));
                        env.Redirect.SetRedirect(ModuleName, MenuName, new MnuTaxOrgQueryArgs() { Id = tbTaxOrgsTest.flId.GetRowVal(r.FirstRow), MenuAction = Actions.View });
                    }

                    return Task.CompletedTask;
                default:
                    throw new NotImplementedException($"curAction:{env.CurAction}");
            }

            void hasInGrObjects(string bin, IYodaRequestContext context)
            {
                var result = CommonIntegrations.CommonIntegrationsHelper.Instance(context.Configuration).GetGrObject(bin, context);
                if (result.StatusCode != "OK")
                {
                    env.AddError(tbTaxOrgs.flBin.FieldName, env.T("БИН не найден в Госреестре"));
                }
            }
        }

        public override Task OnProcessing(ActionEnv<MnuTaxOrgQueryArgs> env)
        {
            var tbTaxOrgs = new TbTaxOrgs();
            tbTaxOrgs.AddFilter(tbTaxOrgs.flId, env.Args.Id);

            switch (env.CurAction)
            {
                case Actions.View:

                    var bin = tbTaxOrgs.flBin.GetRowVal(tbTaxOrgs.SelectFirstOrDefault(t => t.Fields.ToAliases(), env.QueryExecuter).FirstRow);
                    var update = tbTaxOrgs.Update();
                    setGrObjectData(update, bin, env.Args.Id, env.RequestContext);
                    update.Execute(env.QueryExecuter);
                    env.SetPostbackMessage(env.T("Успешно обновлён"));
                    env.Redirect.SetRedirect(ModuleName, MenuName, new MnuTaxOrgQueryArgs() { Id = env.Args.Id, MenuAction = Actions.View });
                    return Task.CompletedTask;
                case Actions.Create:

                    var newBin = tbTaxOrgs.flBin.GetVal(env);
                    var insert = tbTaxOrgs.Insert();
                    env.Args.Id = tbTaxOrgs.flId.GetNextId(env.QueryExecuter);
                    setGrObjectData(insert, newBin, env.Args.Id, env.RequestContext);
                    insert.Execute(env.QueryExecuter);
                    env.SetPostbackMessage(env.T("Успешно создан"));
                    env.Redirect.SetRedirect(ModuleName, MenuName, new MnuTaxOrgQueryArgs() { Id = env.Args.Id, MenuAction = Actions.View });
                    return Task.CompletedTask;
                default:
                    throw new NotImplementedException($"curAction:{env.CurAction}");
            }

            void setGrObjectData(DataModifingQueryProxy<TbTaxOrgs> table, string bin, int Id,  IYodaRequestContext context)
            {
                var result = CommonIntegrations.CommonIntegrationsHelper.Instance(context.Configuration).GetGrObject(bin, context);
                table.Set(t => t.flId, Id);
                table.Set(t => t.flBin, bin);
                table.Set(t => t.flNameRu, result.OrganizationData.NameRu);
                table.Set(t => t.flNameKz, result.OrganizationData.NameKz);
                table.Set(t => t.flStatus, RefTaxOrgStatuses.Values.ACT);
            }
        }

    }

    public class MnuTaxOrgQueryArgs: ActionQueryArgsBase {
        public int Id { get; set; }
    }

}
