using CommonSource.Models;
using CommonSource.QueryTables;
using CommonSource.SearchCollections;
using Source.SearchCollections;
using UsersResources;
using Yoda.Interfaces.Forms;
using Yoda.Interfaces.Helpers;
using YodaHelpers;
using YodaHelpers.ActionMenus;
using YodaHelpers.Components;
using YodaHelpers.Fields;
using YodaHelpers.SearchCollections;
using YodaQuery;

namespace TradeResourcesPlugin.Modules.Menus.Comission {
    public class MnuCommissionActions : MnuActionsExt<CommissionQueryArgs> {

        public MnuCommissionActions(string moduleName) : base(moduleName, nameof(MnuCommissionActions), "Члены комиссии") {
            AsCallback();
            Enabled(rc => { 
                if(
                rc.User.HasRole("TRADERESOURCES-Недропользование-Создание приказов", rc.QueryExecuter)/*rc.User.HasPermission(nameof(RegistersModule), RegistersModule.LocalPermissions.Landlords)*/
                || rc.User.HasRole("TRADERESOURCES-Охотничьи угодья-Создание приказов", rc.QueryExecuter)/*rc.User.HasCustomRole("huntingobjects", "dataEdit", rc.QueryExecuter)*/
                || rc.User.HasRole("TRADERESOURCES-Рыбохозяйственные водоёмы-Создание приказов", rc.QueryExecuter)/*rc.User.HasCustomRole("fishingobjects", "dataEdit", rc.QueryExecuter)*/
                || rc.User.HasRole("TRADERESOURCES-Земельные ресурсы-Создание приказов", rc.QueryExecuter)/*rc.User.HasCustomRole("landobjects", "appLandEdit", rc.QueryExecuter)*/
                || rc.User.HasRole("TRADERESOURCES-Лесные ресурсы-Выставление на торги", rc.QueryExecuter)
                ) {
                    return true;
                }
                return false;
            });
        }

        public class Actions {
            public const string Create = "create", View = "view", Edit = "edit", Delete = "delete", GetComissionData = "get-comission-data", Recover = "recover";
        }

        public class PhysPersonData {
            public string Iin { get; set; }
            public string Name { get; set; }
        }

        public override void Configure(ActionConfig<CommissionQueryArgs> config) {
            config
                .OnAction(Actions.View, action => action
                    .IsValid(env => { return new OkResult(); })
                    .Tasks(t => t.AddIfValid(Actions.Edit, t.Env.T("Редактировать")).AddIfValid(Actions.Delete, t.Env.T("Удалить")).AddIfValid(Actions.Recover, t.Env.T("Восстановить")))
                    .OnRendering(re => {
                        var tbCommMemmbers = new TbComissionMembers();
                        var model = CommissionModelHelper.GetCommissionModel(re.Args.Id, re.QueryExecuter);
                        tbCommMemmbers.flId.RenderCustom(re.Form, re, model.Id.ToString() ?? re.T("*Генерируется автоматически"), readOnly: true);
                        tbCommMemmbers.flCompetentOrgBin.RenderCustom(re.Form, re, model.CompetentOrgBin, readOnly: true);
                        tbCommMemmbers.flIin.RenderCustom(re.Form, re, model.Iin, readOnly: true);
                        tbCommMemmbers.flFio.RenderCustom(re.Form, re, model.Fio, readOnly: true);
                        tbCommMemmbers.flInfo.RenderCustom(re.Form, re, model.Info, readOnly: true);
                        tbCommMemmbers.flStart.RenderCustom(re.Form, re, model.Start, readOnly: true);
                        tbCommMemmbers.flEnd.RenderCustom(re.Form, re, model.End, readOnly: true);
                        tbCommMemmbers.flStatus.RenderCustom(re.Form, re, model.Status, readOnly: true);
                    }))
                .OnAction(Actions.GetComissionData, action => action
                    .IsValid(env => { 
                        if (
                            env.User.HasRole("TRADERESOURCES-Недропользование-Создание приказов", env.QueryExecuter)
                            || env.User.HasRole("TRADERESOURCES-Охотничьи угодья-Создание приказов", env.QueryExecuter)
                            || env.User.HasRole("TRADERESOURCES-Рыбохозяйственные водоёмы-Создание приказов", env.QueryExecuter)
                            || env.User.HasRole("TRADERESOURCES-Земельные ресурсы-Создание приказов", env.QueryExecuter)
                            || env.User.HasRole("TRADERESOURCES-Лесные ресурсы-Выставление на торги", env.QueryExecuter)
                        )
                            return new OkResult();

                        return new AccessDeniedResult();
                    })
                    .OnRendering(re => {
                        var data = new PhysPersonData();
                        var iin = re.RequestContext.GetParamValue("com-member-iin", true);
                        if (string.IsNullOrEmpty(iin)) {
                            data.Iin = "error";
                            data.Name = "error";
                            re.Redirect.SetRedirectToJson(new Microsoft.AspNetCore.Mvc.JsonResult(data));
                        }
                        var physSearchCollection = SearchCollectionsProvider.Instance.Get(nameof(GbdFlObjectCollection));
                        var phys = (GbdFlObjectSearchItem)physSearchCollection.GetItem(iin, re.RequestContext);
                        data.Iin = iin;
                        data.Name = phys.SearchItemText;
                        re.Redirect.SetRedirectToJson(new Microsoft.AspNetCore.Mvc.JsonResult(data));
                    }))
                .OnAction(Actions.Create, action => action
                    .IsValid(env => { return new OkResult(); })
                    .Wizard(wizard => wizard
                        .Args(env => env.Args)
                        .Model(env => new CommissionModel { CompetentOrgBin = env.User.GetUserXin(env.QueryExecuter), Status = ComissionStatuses.Active.ToString() })
                        .CancelBtn("Отмена", env => new ActionRedirectData(ModuleName, nameof(MnuCommissionMembersSearch)))
                        .FinishBtn("Добавить")
                        .Step("Добавить члена комиссии", step => step
                            .OnRendering(re => {
                                re.Panel.AddComponent(new UiPackages("comission-gbdfl-caller-package"));
                                var tbCommMemmbers = new TbComissionMembers();
                                tbCommMemmbers.flId.RenderCustom(re.Panel, re.Env, re.Model.Id.ToString() ?? re.Env.T("*Генерируется автоматически"), readOnly: true);
                                tbCommMemmbers.flCompetentOrgBin.RenderCustom(re.Panel, re.Env, re.Model.CompetentOrgBin, readOnly: true);
                                tbCommMemmbers.flIin.RenderCustom(re.Panel, re.Env, re.Model.Iin);
                                re.Panel.AddHtml($"<div class='form-group row '><label class='col-sm-3 control-label'></label><div class='col-sm-9'><a class='' name='btn-fl-iin-caller' id='btn-fl-iin-caller' Value='{re.Env.T("Запросить")}' >{re.Env.T("Запросить")}</a></div></div>");
                                tbCommMemmbers.flFio.RenderCustom(re.Panel, re.Env, re.Model.Fio);
                                tbCommMemmbers.flInfo.RenderCustom(re.Panel, re.Env, re.Model.Info);
                                tbCommMemmbers.flStart.RenderCustom(re.Panel, re.Env, re.Model.Start);
                                tbCommMemmbers.flEnd.RenderCustom(re.Panel, re.Env, re.Model.End);
                                tbCommMemmbers.flStatus.RenderCustom(re.Panel, re.Env, re.Model.Status, readOnly: true);
                            })
                            .OnValidating(ve => {
                                var xin = ve.Env.User.GetUserXin(ve.Env.QueryExecuter);
                                var tbCommMemmbers = new TbComissionMembers();
                                tbCommMemmbers.flIin.Required().Validate(ve.Env);
                                tbCommMemmbers.flFio.Required().Validate(ve.Env);

                                if (ve.Env.IsValid)
                                {
                                    var iin = tbCommMemmbers.flIin.GetVal(ve.Env);
                                    if (!string.IsNullOrEmpty(iin) && CommissionModelHelper.IsComissionMemberExists(iin, xin, -1, ve.Env.QueryExecuter))
                                    {
                                        ve.Env.AddError(tbCommMemmbers.flIin.FieldName, ve.Env.T("Запись с таким же ИИН существует"));
                                    }
                                }
                            })
                            .OnProcessing(pe => {
                                var tbCommMemmbers = new TbComissionMembers();
                                var id = pe.Env.QueryExecuter.GetNextId(tbCommMemmbers, tbCommMemmbers.flId.FieldName);
                                tbCommMemmbers.Insert()
                                .SetT(t => t.flId, id)
                                .SetT(t => t.flCompetentOrgBin, pe.Model.CompetentOrgBin)
                                .SetT(t => t.flIin, tbCommMemmbers.flIin.GetVal(pe.Env))
                                .SetT(t => t.flFio, tbCommMemmbers.flFio.GetVal(pe.Env))
                                .SetT(t => t.flInfo, tbCommMemmbers.flInfo.GetVal(pe.Env))
                                .SetT(t => t.flStart, tbCommMemmbers.flStart.GetVal(pe.Env))
                                .SetT(t => t.flEnd, tbCommMemmbers.flEnd.GetVal(pe.Env))
                                .SetT(t => t.flStatus, ComissionStatuses.Active)
                                .Execute(pe.Env.QueryExecuter);
                                pe.Env.Redirect.SetRedirect(ModuleName, nameof(MnuCommissionActions), new CommissionQueryArgs { MenuAction = Actions.View, Id = id });
                            })
                        ).Build()
                    ))
                .OnAction(Actions.Edit, action => action
                    .IsValid(env => {
                        var model = CommissionModelHelper.GetCommissionModel(env.Args.Id, env.QueryExecuter);
                        if(model.Status == ComissionStatuses.Active.ToString()) {
                            return new OkResult();
                        }
                        return new AccessDeniedResult();
                    })
                    .Wizard(wizard => wizard
                        .Args(env => env.Args)
                        .Model(env => {
                            return CommissionModelHelper.GetCommissionModel(env.Args.Id, env.QueryExecuter);
                        })
                        .CancelBtn("Отмена", env => new ActionRedirectData(ModuleName, nameof(MnuCommissionActions), new CommissionQueryArgs { MenuAction = Actions.View, Id = env.Args.Id }))
                        .FinishBtn("Сохранить")
                        .Step("Редактировать члена комиссии", step => step
                            .OnRendering(re => {
                                re.Panel.AddComponent(new UiPackages("comission-gbdfl-caller-package"));
                                var tbCommMemmbers = new TbComissionMembers();
                                tbCommMemmbers.flId.RenderCustom(re.Panel, re.Env, re.Model.Id.ToString() ?? re.Env.T("*Генерируется автоматически"), readOnly: true);
                                tbCommMemmbers.flCompetentOrgBin.RenderCustom(re.Panel, re.Env, re.Model.CompetentOrgBin, readOnly: true);
                                tbCommMemmbers.flIin.RenderCustom(re.Panel, re.Env, re.Model.Iin);
                                re.Panel.AddHtml($"<div class='form-group row '><label class='col-sm-3 control-label'></label><div class='col-sm-9'><a class='' name='btn-fl-iin-caller' id='btn-fl-iin-caller' Value='{re.Env.T("Запросить")}' >{re.Env.T("Запросить")}</a></div></div>");
                                tbCommMemmbers.flFio.RenderCustom(re.Panel, re.Env, re.Model.Fio);
                                tbCommMemmbers.flInfo.RenderCustom(re.Panel, re.Env, re.Model.Info);
                                tbCommMemmbers.flStart.RenderCustom(re.Panel, re.Env, re.Model.Start);
                                tbCommMemmbers.flEnd.RenderCustom(re.Panel, re.Env, re.Model.End);
                                tbCommMemmbers.flStatus.RenderCustom(re.Panel, re.Env, re.Model.Status, readOnly: true);
                            })
                            .OnValidating(ve => {
                                var xin = ve.Env.User.GetUserXin(ve.Env.QueryExecuter);
                                var tbCommMemmbers = new TbComissionMembers();
                                tbCommMemmbers.flIin.Required().Validate(ve.Env);
                                tbCommMemmbers.flFio.Required().Validate(ve.Env);

                                if (ve.Env.IsValid)
                                {
                                    var iin = tbCommMemmbers.flIin.GetVal(ve.Env);
                                    if (!string.IsNullOrEmpty(iin) && CommissionModelHelper.IsComissionMemberExists(iin, xin, ve.Args.Id, ve.Env.QueryExecuter))
                                    {
                                        ve.Env.AddError(tbCommMemmbers.flIin.FieldName, ve.Env.T("Запись с таким же ИИН существует"));
                                    }
                                }
                            })
                            .OnProcessing(pe => {
                                var tbCommMemmbers = new TbComissionMembers();
                                tbCommMemmbers.AddFilter(t => t.flId, pe.Model.Id);

                                tbCommMemmbers.Update()
                                .SetT(t => t.flIin, tbCommMemmbers.flIin.GetVal(pe.Env))
                                .SetT(t => t.flFio, tbCommMemmbers.flFio.GetVal(pe.Env))
                                .SetT(t => t.flInfo, tbCommMemmbers.flInfo.GetVal(pe.Env))
                                .SetT(t => t.flStart, tbCommMemmbers.flStart.GetVal(pe.Env))
                                .SetT(t => t.flEnd, tbCommMemmbers.flEnd.GetVal(pe.Env))
                                .Execute(pe.Env.QueryExecuter);
                                pe.Env.Redirect.SetRedirect(ModuleName, nameof(MnuCommissionActions), new CommissionQueryArgs { MenuAction = Actions.View, Id = pe.Model.Id });
                            })
                        ).Build()
                    ))
                .OnAction(Actions.Delete, action => action
                    .IsValid(env => {
                        var model = CommissionModelHelper.GetCommissionModel(env.Args.Id, env.QueryExecuter);
                        if (model.Status == ComissionStatuses.Active.ToString()) {
                            return new OkResult();
                        }
                        return new AccessDeniedResult();
                    })
                    .Wizard(wizard => wizard
                        .Args(env => env.Args)
                        .Model(env => {
                            return CommissionModelHelper.GetCommissionModel(env.Args.Id, env.QueryExecuter);
                        })
                        .CancelBtn("Отмена", env => new ActionRedirectData(ModuleName, nameof(MnuCommissionActions), new CommissionQueryArgs { MenuAction = Actions.View, Id = env.Args.Id }))
                        .FinishBtn("Удалить")
                        .Step("Удалить члена комиссии", step => step
                            .OnRendering(re => {
                                re.Model.Status = ComissionStatuses.Deleted.ToString();
                                var tbCommMemmbers = new TbComissionMembers();
                                tbCommMemmbers.flId.RenderCustom(re.Panel, re.Env, re.Model.Id.ToString() ?? re.Env.T("*Генерируется автоматически"), readOnly: true);
                                tbCommMemmbers.flCompetentOrgBin.RenderCustom(re.Panel, re.Env, re.Model.CompetentOrgBin, readOnly: true);
                                tbCommMemmbers.flIin.RenderCustom(re.Panel, re.Env, re.Model.Iin, readOnly: true);
                                tbCommMemmbers.flFio.RenderCustom(re.Panel, re.Env, re.Model.Fio, readOnly: true);
                                tbCommMemmbers.flInfo.RenderCustom(re.Panel, re.Env, re.Model.Info, readOnly: true);
                                tbCommMemmbers.flStart.RenderCustom(re.Panel, re.Env, re.Model.Start, readOnly: true);
                                tbCommMemmbers.flEnd.RenderCustom(re.Panel, re.Env, re.Model.End, readOnly: true);
                                tbCommMemmbers.flStatus.RenderCustom(re.Panel, re.Env, re.Model.Status, readOnly: true);
                            })
                            .OnValidating(ve => { })
                            .OnProcessing(pe => {
                                pe.Model.Status = ComissionStatuses.Deleted.ToString();
                                var tbCommMemmbers = new TbComissionMembers();
                                tbCommMemmbers.AddFilter(t => t.flId, pe.Model.Id);

                                tbCommMemmbers
                                    .Update()
                                    .SetT(t => t.flStatus, ComissionStatuses.Deleted)
                                    .Execute(pe.Env.QueryExecuter);
                                pe.Env.Redirect.SetRedirect(ModuleName, nameof(MnuCommissionActions), new CommissionQueryArgs { MenuAction = Actions.View, Id = pe.Model.Id });
                            })
                        ).Build()
                    ))
                .OnAction(Actions.Recover, action => action
                    .IsValid(env => {
                        var model = CommissionModelHelper.GetCommissionModel(env.Args.Id, env.QueryExecuter);
                        if (model.Status == ComissionStatuses.Deleted.ToString()) {
                            return new OkResult();
                        }
                        return new AccessDeniedResult();
                    })
                    .Wizard(wizard => wizard
                        .Args(env => env.Args)
                        .Model(env => {
                            return CommissionModelHelper.GetCommissionModel(env.Args.Id, env.QueryExecuter);
                        })
                        .CancelBtn("Отмена", env => new ActionRedirectData(ModuleName, nameof(MnuCommissionActions), new CommissionQueryArgs { MenuAction = Actions.View, Id = env.Args.Id }))
                        .FinishBtn("Восстановить")
                        .Step("Восстановить члена комиссии", step => step
                            .OnRendering(re => {
                                re.Model.Status = ComissionStatuses.Active.ToString();
                                var tbCommMemmbers = new TbComissionMembers();
                                tbCommMemmbers.flId.RenderCustom(re.Panel, re.Env, re.Model.Id.ToString() ?? re.Env.T("*Генерируется автоматически"), readOnly: true);
                                tbCommMemmbers.flCompetentOrgBin.RenderCustom(re.Panel, re.Env, re.Model.CompetentOrgBin, readOnly: true);
                                tbCommMemmbers.flIin.RenderCustom(re.Panel, re.Env, re.Model.Iin, readOnly: true);
                                tbCommMemmbers.flFio.RenderCustom(re.Panel, re.Env, re.Model.Fio, readOnly: true);
                                tbCommMemmbers.flInfo.RenderCustom(re.Panel, re.Env, re.Model.Info, readOnly: true);
                                tbCommMemmbers.flStart.RenderCustom(re.Panel, re.Env, re.Model.Start, readOnly: true);
                                tbCommMemmbers.flEnd.RenderCustom(re.Panel, re.Env, re.Model.End, readOnly: true);
                                tbCommMemmbers.flStatus.RenderCustom(re.Panel, re.Env, re.Model.Status, readOnly: true);
                            })
                            .OnValidating(ve => { })
                            .OnProcessing(pe => {
                                pe.Model.Status = ComissionStatuses.Deleted.ToString();
                                var tbCommMemmbers = new TbComissionMembers();
                                tbCommMemmbers.AddFilter(t => t.flId, pe.Model.Id);

                                tbCommMemmbers
                                    .Update()
                                    .SetT(t => t.flStatus, ComissionStatuses.Active)
                                    .Execute(pe.Env.QueryExecuter);
                                pe.Env.Redirect.SetRedirect(ModuleName, nameof(MnuCommissionActions), new CommissionQueryArgs { MenuAction = Actions.View, Id = pe.Model.Id });
                            })
                        ).Build()
                    ));

        }
    }

    public class CommissionQueryArgs : ActionQueryArgsBase {
        public int Id { get; set; }
    }
}
