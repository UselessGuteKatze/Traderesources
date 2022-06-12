//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using UsersResources;
//using UsersResources.QueryTables;
//using Yoda.Interfaces;
//using Yoda.Interfaces.Forms;
//using Yoda.Interfaces.Forms.Components;
//using Yoda.Interfaces.Helpers;
//using Yoda.Interfaces.Menu;
//using Yoda.Interfaces.Permissions;
//using Yoda.Localization;
//using Yoda.YodaReferences;
//using YodaApp.UsersResources.QueryTables.V2;
//using YodaApp.Yoda.Interfaces.CustomRoles.QueryTables;
//using YodaHelpers;
//using YodaHelpers.ActionMenus;
//using YodaHelpers.DateTimeHelper;
//using YodaHelpers.Fields;
//using YodaQuery;

//namespace TradeResourcesPlugin.Helpers {
//    public class CustomRolesConfigurator : IRoleProviderNamespace, IProject, IMenuPath, IListLinkText, ILinkText, IRoleProvider, IInit {
//        private readonly List<IRole> _roles = new List<IRole>();
//        private string _namespace;
//        private string _menuPath;
//        private string[] _projects;
//        private LocalizableText _linkText;
//        private LocalizableText _listLinkText;
//        private CustomRolesConfigurator() { }
//        public static IRoleProviderNamespace Begin()
//        {
//            return new CustomRolesConfigurator();
//        }
//        public void Init(YodaModuleConfig config)
//        {
//            var adminPermission = new SystemUserAdminRole();
//            _roles.Add(adminPermission);
//            config.Permissions.Add(new Permission($"{adminPermission.Name}-{_namespace}", adminPermission.Text));
//            config.Menus.Add(new MnuCustomRolesList(config.ModuleName, _namespace, _menuPath, _projects, _listLinkText, _roles.ToArray()));
//            config.Menus.Add(new MnuCustomRolesManager(config.ModuleName, _namespace, _menuPath, _projects, _linkText, _roles.ToArray()));
//            config.QueryTables.Add(new TbCustomRoles(_namespace));
//        }

//        public IRoleProvider LinkText(LocalizableText linkText)
//        {
//            _linkText = linkText;
//            return this;
//        }

//        public ILinkText ListLinkText(LocalizableText linkText)
//        {
//            _listLinkText = linkText;
//            return this;
//        }

//        public IListLinkText MenuPath(string path)
//        {
//            _menuPath = path;
//            return this;
//        }

//        public IProject Namespace(string roleConfiguratorNamespace)
//        {
//            _namespace = roleConfiguratorNamespace;
//            return this;
//        }

//        public IMenuPath ProjectConfig(params string[] projects)
//        {
//            _projects = projects;
//            return this;
//        }

//        public IInit Roles(params IRole[] roles)
//        {
//            _roles.AddRange(roles);
//            return this;
//        }
//    }

//    public class MnuCustomRolesList : FrmMenu {
//        public MnuCustomRolesList(string moduleName, string menuNamespace, string menuPath, string[] projects, LocalizableText linkText, IRole[] roles) : base($"{menuNamespace}MnuCustomRolesList", linkText)
//        {
//            Path(menuPath + "/role-list");
//            Enabled(rc => CustomRolesAccessHelper.AccessCheck(moduleName, menuNamespace, rc).HasAccess);
//            AccessCheck((args, rc) => CustomRolesAccessHelper.AccessCheck(moduleName, menuNamespace, rc));
//            ProjectsConfig(projects);
//            OnRendering(env =>
//            {
//                var tbRoles = new TbCustomRoles(menuNamespace);
//                if (!CustomRolesAccessHelper.IsGlobalSystemUserAdmin(env.RequestContext))
//                {
//                    tbRoles.AddFilter(t => t.flBin, env.User.GetUserBin(env.QueryExecuter));
//                }
//                tbRoles
//                    .AddFilter(t => t.flNamespace, menuNamespace)
//                    .Order(t => t.flId, OrderType.Desc)
//                    .ToSearchWidget(env)
//                    .AddHiddenFields(tbRoles.flId, tbRoles.flNamespace, tbRoles.flRole, tbRoles.flFrom.ToAlias("flFromHidden"), tbRoles.flTo.ToAlias("flToHidden"))
//                    .AddReturnFields(tbRoles.Fields.Except(new Field[] { tbRoles.flId, tbRoles.flNamespace, tbRoles.flRole }).ToFieldsAliases())
//                    .AddComputedCells(
//                        new ComputedCell("flUserRole")
//                        {
//                            Text = "Роль",
//                            GetCellContent = (r, v) =>
//                            {
//                                return new Label(roles.First(x => x.Name.EqualsIgnoreCase(tbRoles.flRole.GetRowVal(r))).Text);
//                            }
//                        },
//                        new ComputedCell("flStatus")
//                        {
//                            Text = "Статус",
//                            GetCellContent = (r, v) =>
//                            {
//                                var date = env.QueryExecuter.GetDateTime();
//                                var status = date.Between(tbRoles.flFrom.GetRowVal(r, "flFromHidden"), tbRoles.flTo.GetRowVal(r, "flToHidden")) ? env.T("Активный") : env.T("Не активный");
//                                return new Label(status);
//                            }
//                        }
//                    )
//                    .AddFilters(tbRoles.flIin)
//                    .AddToolbarItemIf(env.User.IsExternalUser(), new Link(env.T("Добавить"), moduleName, $"{menuNamespace}MnuCustomRolesManager", new CustomRoleQueryArgs { RoleId = 0, MenuAction = MnuCustomRolesManager.Actions.Create }))
//                    .AddRowActions(r => new Link(env.T("Открыть"), moduleName, $"{menuNamespace}MnuCustomRolesManager", new CustomRoleQueryArgs { RoleId = tbRoles.flId.GetRowVal(r), MenuAction = MnuCustomRolesManager.Actions.View }))
//                    .AutoExecuteQuery(true)
//                    .CanConfigureFilterFields(true)
//                    .CanConfigureOutputFields(true)
//                    .Print(env.Form, $"{menuNamespace}MnuCustomRolesList");
//            });
//        }
//    }

//    public class MnuCustomRolesManager : MnuActionsExt<CustomRoleQueryArgs> {
//        private readonly string _menuNamespace;
//        private readonly IRole[] _roles;
//        public MnuCustomRolesManager(string moduleName, string menuNamespace, string menuPath, string[] projects, LocalizableText linkText, IRole[] roles) : base(moduleName, $"{menuNamespace}MnuCustomRolesManager", linkText)
//        {
//            AsCallback();
//            Enabled(rc => CustomRolesAccessHelper.AccessCheck(moduleName, menuNamespace, rc).HasAccess);
//            AccessCheck((args, rc) => CustomRolesAccessHelper.AccessCheck(moduleName, menuNamespace, rc));
//            Path(menuPath + "/role-list/manage-role/{RoleId}/{MenuAction}");
//            ProjectsConfig(projects);
//            _roles = roles;
//            _menuNamespace = menuNamespace;
//        }

//        public override void Configure(ActionConfig<CustomRoleQueryArgs> config)
//        {
//            config
//                 .OnAction(Actions.Create, action => action
//                     .IsValid(env => {
//                         if (env.User.IsExternalUser())
//                         {
//                             return new OkResult();
//                         }
//                         else
//                         {
//                             return new AccessDeniedResult();
//                         }
//                     })
//                     .OnRendering(env => {
//                         var tbRoles = new TbCustomRoles(_menuNamespace);
//                         tbRoles.flBin.RenderCustomT(env.Form, env, CustomRolesAccessHelper.IsGlobalSystemUserAdmin(env.RequestContext) ? null : env.User.GetUserBin(env.QueryExecuter), readOnly: !CustomRolesAccessHelper.IsGlobalSystemUserAdmin(env.RequestContext));
//                         tbRoles.flOrgName.RenderCustomT(env.Form, env, CustomRolesAccessHelper.IsGlobalSystemUserAdmin(env.RequestContext) ? null : env.User.GetUserOrgName(env.QueryExecuter), readOnly: !CustomRolesAccessHelper.IsGlobalSystemUserAdmin(env.RequestContext));
//                         tbRoles.flIin.RenderCustomT(env.Form, env, null);
//                         tbRoles.flPosition.RenderCustomT(env.Form, env, null);
//                         renderRoleField("flRole", "Роль", getRolesReference(_roles), env.Form, env.RequestContext, false);
//                         tbRoles.flFrom.RenderCustomT(env.Form, env, null);
//                         tbRoles.flTo.RenderCustomT(env.Form, env, null);
//                         env.Form.AddSubmitButton("btnSave", env.T("Сохранить"));
//                     })
//                     .OnValidating(env => {
//                         var tbRoles = new TbCustomRoles(_menuNamespace);
//                         if (CustomRolesAccessHelper.IsGlobalSystemUserAdmin(env.RequestContext))
//                         {
//                             tbRoles.flBin.Validate(env);
//                             tbRoles.flOrgName.Validate(env);
//                             tbRoles.flPosition.Validate(env);
//                         }

//                         tbRoles.flIin.Validate(env);
//                        //tbRoles.flFrom.Validate(env);
//                        //tbRoles.flTo.Validate(env);
//                        var from = tbRoles.flFrom.GetValOrNull(env);
//                         var to = tbRoles.flTo.GetValOrNull(env);
//                         if (from == null)
//                         {
//                             env.AddError(tbRoles.flFrom.FieldName, env.T($"Поле \"{tbRoles.flFrom.Text}\" обязательно для заполнения"));
//                             return;
//                         }
//                         if (to == null)
//                         {
//                             env.AddError(tbRoles.flTo.FieldName, env.T($"Поле \"{tbRoles.flTo.Text}\" обязательно для заполнения"));
//                             return;
//                         }
//                         if (from > to)
//                         {
//                             env.AddError("periodError", env.T($"Поле \"{tbRoles.flFrom.Text}\" не может превышать поле \"{tbRoles.flTo.Text}\""));
//                         }
//                         var role = new Referencebox("flRole", getRolesReference(_roles)).GetPostedValues<string>(env.FormCollection);
//                         if (role.Length == 0)
//                         {
//                             env.AddError("flRole", env.T("Поле \"Роль\" обязательно для заполнения"));
//                         }
//                         else
//                         {
//                             tbRoles
//                                .AddFilter(t => t.flNamespace, _menuNamespace)
//                                .AddFilter(t => t.flRole, role.First())
//                                .AddFilter(t => t.flBin, CustomRolesAccessHelper.IsGlobalSystemUserAdmin(env.RequestContext) ? tbRoles.flBin.GetVal(env) : env.User.GetUserBin(env.QueryExecuter))
//                                  .AddFilter(t => t.flIin, tbRoles.flIin.GetVal(env));
//                             if (tbRoles.Count(env.QueryExecuter) > 0)
//                             {
//                                 env.AddError("duplicate", env.T("У данного пользователя роль уже проставлена"));
//                             }
//                         }
//                     })
//                     .OnProcessing(env => {
//                         var role = new Referencebox("flRole", getRolesReference(_roles)).GetPostedValues<string>(env.FormCollection).First();
//                         var tbRoles = new TbCustomRoles(_menuNamespace);
//                         using (ITransaction transaction = env.QueryExecuter.BeginTransaction(tbRoles.DbKey))
//                         {
//                             var roleId = tbRoles.flId.GetNextId(env.QueryExecuter, transaction);
//                             var iin = tbRoles.flIin.GetVal(env);
//                             var bin = CustomRolesAccessHelper.IsGlobalSystemUserAdmin(env.RequestContext) ? tbRoles.flBin.GetVal(env) : env.User.GetUserBin(env.QueryExecuter);
//                             var orgName = CustomRolesAccessHelper.IsGlobalSystemUserAdmin(env.RequestContext) ? tbRoles.flOrgName.GetVal(env) : env.User.GetUserOrgName(env.QueryExecuter);
//                             tbRoles
//                                 .Insert()
//                                 .SetT(t => t.flId, roleId)
//                                 .SetT(t => t.flNamespace, _menuNamespace)
//                                 .SetT(t => t.flBin, bin)
//                                 .SetT(t => t.flOrgName, orgName)
//                                 .SetT(t => t.flPosition, tbRoles.flPosition.GetVal(env))
//                                 .SetT(t => t.flIin, iin)
//                                 .SetT(t => t.flFio, getUserFio(iin, env.QueryExecuter))
//                                 .SetT(t => t.flRole, role)
//                                 .SetT(t => t.flFrom, tbRoles.flFrom.GetVal(env))
//                                 .SetT(t => t.flTo, tbRoles.flTo.GetVal(env))
//                                 .Execute(env.QueryExecuter, transaction);
//                             transaction.Commit();
//                             env.SetPostbackMessage(env.T("Данные успешно сохранены"));
//                             env.Redirect.SetRedirect(ModuleName, $"{_menuNamespace}MnuCustomRolesManager", new CustomRoleQueryArgs { RoleId = roleId, MenuAction = Actions.View });
//                         }
//                     })
//                 )
//                 .OnAction(Actions.View, action => action
//                     .IsValid(env => {
//                         return new OkResult();
//                     })
//                     .Tasks(a => a.AddIfValid(Actions.Edit, "Редактировать"))
//                     .OnRendering(env => {
//                         var tbRoles = new TbCustomRoles(_menuNamespace)
//                             .AddFilter(t => t.flId, env.Args.RoleId);
//                         var r = tbRoles.SelectFirst(t => t.Fields.ToFieldsAliases(), env.QueryExecuter);
//                         tbRoles.flBin.RenderCustomT(env.Form, env, r.GetVal(t => t.flBin), readOnly: true);
//                         tbRoles.flOrgName.RenderCustomT(env.Form, env, r.GetVal(t => t.flOrgName), readOnly: true);
//                         tbRoles.flIin.RenderCustomT(env.Form, env, r.GetVal(t => t.flIin), readOnly: true);
//                         tbRoles.flPosition.RenderCustomT(env.Form, env, r.GetVal(t => t.flPosition), readOnly: true);
//                         tbRoles.flRole.RenderCustomT(env.Form, env, _roles.First(x => x.Name.EqualsIgnoreCase(r.GetVal(t => t.flRole))).Text, readOnly: true);
//                         tbRoles.flFrom.RenderCustomT(env.Form, env, r.GetVal(t => t.flFrom), readOnly: true);
//                         tbRoles.flTo.RenderCustomT(env.Form, env, r.GetVal(t => t.flTo), readOnly: true);
//                         var date = env.QueryExecuter.GetDateTime();
//                         var status = date.Between(r.GetVal(t => t.flFrom), r.GetVal(t => t.flTo)) ? env.T("Активный") : env.T("Не активный");
//                         renderField("flStatus", "Статус", new Label(status), env.Form, env.RequestContext, true);
//                     })
//                 )
//                 .OnAction(Actions.Edit, action => action
//                     .IsValid(env => {
//                         if (env.User.IsExternalUser())
//                         {
//                             return new OkResult();
//                         } else
//                         {
//                             return new AccessDeniedResult();
//                         }
//                     })
//                     .OnRendering(env => {
//                         var tbRoles = new TbCustomRoles(_menuNamespace)
//                             .AddFilter(t => t.flId, env.Args.RoleId);
//                         var r = tbRoles.SelectFirst(t => t.Fields.ToFieldsAliases(), env.QueryExecuter);
//                         tbRoles.flBin.RenderCustomT(env.Form, env, r.GetVal(t => t.flBin), readOnly: !CustomRolesAccessHelper.IsGlobalSystemUserAdmin(env.RequestContext));
//                         tbRoles.flOrgName.RenderCustomT(env.Form, env, env.User.GetUserOrgName(env.QueryExecuter), readOnly: !CustomRolesAccessHelper.IsGlobalSystemUserAdmin(env.RequestContext));
//                         tbRoles.flIin.RenderCustomT(env.Form, env, r.GetVal(t => t.flIin));
//                         tbRoles.flPosition.RenderCustomT(env.Form, env, r.GetVal(t => t.flPosition));
//                         renderRoleField("flRole", "Роль", getRolesReference(_roles), env.Form, env.RequestContext, false, new[] { r.GetVal(t => t.flRole) });
//                         tbRoles.flFrom.RenderCustomT(env.Form, env, r.GetVal(t => t.flFrom));
//                         tbRoles.flTo.RenderCustomT(env.Form, env, r.GetVal(t => t.flTo));
//                         env.Form.AddSubmitButton("btnSave", env.T("Сохранить"));
//                     })
//                     .OnValidating(env => {
//                         var tbRoles = new TbCustomRoles(_menuNamespace);
//                         if (CustomRolesAccessHelper.IsGlobalSystemUserAdmin(env.RequestContext))
//                         {
//                             tbRoles.flBin.Validate(env);
//                             tbRoles.flOrgName.Validate(env);
//                         }
//                         tbRoles.flIin.Validate(env);
//                        //tbRoles.flFrom.Validate(env);
//                        //tbRoles.flTo.Validate(env);
//                        var from = tbRoles.flFrom.GetValOrNull(env);
//                         var to = tbRoles.flTo.GetValOrNull(env);
//                         if (from == null)
//                         {
//                             env.AddError(tbRoles.flFrom.FieldName, env.T($"Поле \"{tbRoles.flFrom.Text}\" обязательно для заполнения"));
//                             return;
//                         }
//                         if (to == null)
//                         {
//                             env.AddError(tbRoles.flTo.FieldName, env.T($"Поле \"{tbRoles.flTo.Text}\" обязательно для заполнения"));
//                             return;
//                         }
//                         if (from > to)
//                         {
//                             env.AddError("periodError", env.T($"Поле \"{tbRoles.flFrom.Text}\" не может превышать поле \"{tbRoles.flTo.Text}\""));
//                         }
//                         if (new Referencebox("flRole", getRolesReference(_roles)).GetPostedValues<string>(env.FormCollection).Length == 0)
//                         {
//                             env.AddError("flRole", env.T("Поле \"Роль\" обязательно для заполнения"));
//                         }
//                     })
//                     .OnProcessing(env => {
//                         var role = new Referencebox("flRole", getRolesReference(_roles)).GetPostedValues<string>(env.FormCollection).First();
//                         var tbRoles = new TbCustomRoles(_menuNamespace)
//                             .AddFilter(t => t.flId, env.Args.RoleId);
//                         using (ITransaction transaction = env.QueryExecuter.BeginTransaction(tbRoles.DbKey))
//                         {
//                             var iin = tbRoles.flIin.GetVal(env);
//                             var bin = CustomRolesAccessHelper.IsGlobalSystemUserAdmin(env.RequestContext) ? tbRoles.flBin.GetVal(env) : env.User.GetUserBin(env.QueryExecuter);
//                             var orgName = CustomRolesAccessHelper.IsGlobalSystemUserAdmin(env.RequestContext) ? tbRoles.flOrgName.GetVal(env) : env.User.GetUserOrgName(env.QueryExecuter);
//                             tbRoles
//                                 .Update()
//                                 .SetT(t => t.flIin, iin)
//                                 .SetT(t => t.flBin, bin)
//                                 .SetT(t => t.flOrgName, orgName)
//                                 .SetT(t => t.flFio, getUserFio(iin, env.QueryExecuter))
//                                 .SetT(t => t.flPosition, tbRoles.flPosition.GetVal(env))
//                                 .SetT(t => t.flFrom, tbRoles.flFrom.GetVal(env))
//                                 .SetT(t => t.flTo, tbRoles.flTo.GetVal(env))
//                                 .SetT(t => t.flRole, role)
//                                 .Execute(env.QueryExecuter, transaction);
//                             transaction.Commit();
//                             env.SetPostbackMessage(env.T("Данные успешно сохранены"));
//                             env.Redirect.SetRedirect(ModuleName, $"{_menuNamespace}MnuCustomRolesManager", new CustomRoleQueryArgs { RoleId = env.Args.RoleId, MenuAction = Actions.View });
//                         }
//                     })
//                 );
//        }

//        private Reference getRolesReference(IRole[] roles)
//        {
//            var reference = new Reference();
//            foreach (IRole role in roles)
//            {
//                reference.Items.Add(role.Name, role.Text);
//            }
//            return reference;
//        }

//        private string getUserFio(string iin, IQueryExecuter queryExecuter)
//        {
//            var user = new TbUsers()
//                .AddFilter(t => t.flIin, iin)
//                .SelectFirst(t => new FieldAlias[] { t.flLastName, t.flFirstName, t.flMiddleName }, queryExecuter);
//            return $"{user.GetVal(t => t.flLastName)} {user.GetVal(t => t.flFirstName)} {user.GetVal(t => t.flMiddleName)}";
//        }

//        private void renderRoleField(string fieldName, string title, Reference reference, WidgetBase widget, IYodaRequestContext env, bool readOnly, object[] defaultValue = null)
//        {
//            var editor = new Referencebox(fieldName, reference, selectedValues: defaultValue) { CssClass = " form-control" };
//            renderField(fieldName, title, editor, widget, env, readOnly);
//        }

//        private void renderField(string fieldName, string title, YodaFormElement editor, WidgetBase widget, IYodaRequestContext env, bool readOnly)
//        {
//            var pnl = new Panel
//            {
//                CssClass = "form-group row",
//                Attributes = {
//                    {"render-field-name", fieldName},
//                    {"data-field-readonly", readOnly ? "true" : "false"}
//                }
//            };
//            var sb = new StringBuilder();
//            sb.AppendHtml("<label for='{0}' class='col-sm-3 control-label'>{1}</label>", fieldName, env.T(title));
//            pnl.AddComponent(new HtmlText(sb.ToString()));
//            editor.CssClass = " form-control";
//            var editorWrapper = new Panel
//            {
//                CssClass = "col-sm-9"
//            };
//            editorWrapper.AddComponent(editor);
//            pnl.AddComponent(editorWrapper);
//            widget.AddComponent(pnl);
//        }

//        public class Actions {
//            public const string
//                Create = "create",
//                View = "view",
//                Edit = "edit";
//        }
//    }

//    public static class CustomRolesAccessHelper {
//        public static string GetOrgDirectorIin(IYodaRequestContext context)
//        {
//            return context.User.GetAccountData(context.QueryExecuter).CorpData.FirstPersonFio;
//        }

//        public static AccessCheckResult AccessCheck(string moduleName, string menuNamespace, IYodaRequestContext context)
//        {
//            if (!context.User.IsExternalUser() && !context.User.IsGuest())
//            {
//                return new AccessCheckResult { HasAccess = true };
//            }
//            if (!context.User.IsCorporateUser(context.QueryExecuter))
//            {
//                return new AccessCheckResult
//                {
//                    HasAccess = false,
//                    ExceptionMessage = "Доступ запрещен"
//                };
//            }
//            if (context.User.GetUserIin(context.QueryExecuter).EqualsIgnoreCase(GetOrgDirectorIin(context)))
//            {
//                return new AccessCheckResult { HasAccess = true };
//            }
//            if (IsGivenSystemUserAdmin(menuNamespace, context))
//            {
//                return new AccessCheckResult { HasAccess = true };
//            }
//            if (IsSystemUserAdmin(moduleName, menuNamespace, context))
//            {
//                return new AccessCheckResult { HasAccess = true };
//            }
//            if (IsGlobalSystemUserAdmin(context))
//            {
//                return new AccessCheckResult { HasAccess = true };
//            }
//            return new AccessCheckResult
//            {
//                HasAccess = false,
//                ExceptionMessage = "Доступ запрещен"
//            };
//        }

//        public static bool IsGivenSystemUserAdmin(string menuNamespace, IYodaRequestContext context)
//        {
//            return new TbCustomRoles(menuNamespace)
//                .AddFilter(t => t.flNamespace, menuNamespace)
//                .AddFilter(t => t.flBin, context.User.GetUserBin(context.QueryExecuter))
//                .AddFilter(t => t.flIin, context.User.GetUserIin(context.QueryExecuter))
//                .AddFilter(t => t.flRole, $"SystemUserAdmin")
//                .Count(context.QueryExecuter) > 0;
//        }

//        public static bool IsSystemUserAdmin(string moduleName, string menuNamespace, IYodaRequestContext context)
//        {
//            return context.User.HasPermission(moduleName, $"{new SystemUserAdminRole().Name}-{menuNamespace}");
//        }

//        public static bool IsGlobalSystemUserAdmin(IYodaRequestContext context)
//        {
//            return context.User.HasPermission("CustomRolesConfigurationInitModule", "custom-user-roles-admin");
//        }

//        public static bool HasCustomRole(this IUser user, string roleNamespace, string roleName, IQueryExecuter queryExecuter)
//        {
//            if (user.IsSuperUser)
//            {
//                return true;
//            }
//            if (user.IsGuest() || user.IsIndividualUser(queryExecuter))
//            {
//                return false;
//            }
//            var date = queryExecuter.GetDateTime();
//            return new TbCustomRoles(roleNamespace)
//                .AddFilter(t => t.flNamespace, roleNamespace)
//                .AddFilter(t => t.flBin, user.GetUserXin(queryExecuter))
//                .AddFilter(t => t.flIin, user.GetUserIin(queryExecuter))
//                .AddFilter(t => t.flRole, roleName)
//                .AddFilter(t => t.flFrom, ConditionOperator.LessOrEqual, date)
//                .AddFilter(t => t.flTo, ConditionOperator.GreateOrEqual, date)
//                .Count(queryExecuter) > 0;
//        }

//        public static int GetUsersRoleCount(string roleNamespace, string roleName, IQueryExecuter queryExecuter)
//        {
//            return new TbCustomRoles(roleNamespace)
//                .AddFilter(t => t.flNamespace, roleNamespace)
//                .AddFilter(t => t.flRole, roleName)
//                .Count(queryExecuter);
//        }
//    }

//    public class CustomRoleQueryArgs : ActionQueryArgsBase {
//        public int RoleId { get; set; }
//    }
//    public interface IListLinkText {
//        ILinkText ListLinkText(LocalizableText linkText);
//    }
//    public interface ILinkText {
//        IRoleProvider LinkText(LocalizableText linkText);
//    }
//    public interface IMenuPath {
//        IListLinkText MenuPath(string path);
//    }
//    public interface IProject {
//        IMenuPath ProjectConfig(params string[] projects);
//    }
//    public interface IRoleProviderNamespace {
//        public IProject Namespace(string roleConfiguratorNamespace);
//    }
//    public interface IRoleProvider {
//        IInit Roles(IRole[] roles);
//    }
//    public interface IInit {
//        void Init(YodaModuleConfig config);
//    }
//    public interface IRole {
//        public string Name { get; set; }
//        public string Text { get; set; }
//    }
//    public class CustomRole : IRole {
//        public CustomRole() { }
//        public CustomRole(string name, string text)
//        {
//            Name = name;
//            Text = text;
//        }
//        public string Name { get; set; }
//        public string Text { get; set; }
//    }
//    public class SystemUserAdminRole : IRole {
//        public SystemUserAdminRole()
//        {
//            Name = $"SystemUserAdmin";
//            Text = $"Администратор пользователей";
//        }
//        public string Name { get; set; }
//        public string Text { get; set; }
//    }
//}
