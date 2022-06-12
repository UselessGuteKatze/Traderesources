using CommonSource.QueryTables;
using CommonSource.References.DeprivedPersons;
using CommonSource.SearchCollections;
using Source.SearchCollections;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yoda.Interfaces.Forms;
using Yoda.Interfaces.Forms.Components;
using Yoda.Interfaces.Helpers;
using Yoda.Interfaces.Menu;
using YodaHelpers.ActionMenus;
using YodaHelpers.Fields;
using YodaQuery;
using static CommonSource.NpGlobal;

namespace TradeResourcesPlugin.Modules.Administration.DeprivedPersons {
    public class MnuDeprivedPersonsReestrSearch : FrmMenu {
        public const string MnuName = nameof(MnuDeprivedPersonsReestrSearch);
        public MnuDeprivedPersonsReestrSearch(string moduleName) : base(MnuName, "Реестр недобросовестных заявителей") {
            Path("traderesources/res-subsoil/res-hydrocarbon/reestr/deprived-persons");
            OnRendering(re => {
                var tbPersons = new TbDeprivedPersons();
                tbPersons.OrderBy = new[] { new OrderField(tbPersons.flId, OrderType.Desc) };
                tbPersons
                    .ToSearchWidget(re.AsFormEnv())
                    .AddToolbarItem(new Link("Добавить", moduleName, MnuDeprivedPersonsReestrActions.MnuName, new DeprivedPersonsReestrQueryArgs { MenuAction = DeprivedPersonsReestrActions.Add, PersonId = -1, Xin = "1" }))
                    .AddReturnFields(tbPersons.GetReturnFields().ToFieldsAliases())
                    .AddFilters(tbPersons.flFio, tbPersons.flXin, tbPersons.flStatus)
                    .AddHiddenFields(tbPersons.flId)
                    .AddRowActions(r => new Link(re.T("Просмотр"), moduleName, MnuDeprivedPersonsReestrActions.MnuName, new DeprivedPersonsReestrQueryArgs { PersonId = tbPersons.flId.GetRowVal(r), MenuAction = DeprivedPersonsReestrActions.View, Xin = "1" }))
                    .AutoExecuteQuery(true)
                    .HideSearchButton(false)
                    .CanConfigureOutputFields(true)
                    .CanConfigureFilterFields(true)
                    .Print(re.Form, MnuName);
            });
        }
    }
	public class MnuDeprivedPersonsReestrActions : MnuActions<DeprivedPersonsReestrQueryArgs> {
        public const string MnuName = nameof(MnuDeprivedPersonsReestrActions);
        public MnuDeprivedPersonsReestrActions(string moduleName) : base(moduleName, MnuName, "Реестр недобросовестных заявителей") {
            Path("traderesources/res-subsoil/res-hydrocarbon/reestr/deprived-persons/{MenuAction}/{PersonId}/{Xin}");
            AsCallback();
        }
        public class PersonData {
            public string Xin { get; set; }
            public string Name { get; set; }
        }
        public override Task<ActionItem[]> GetActions(ActionEnv<DeprivedPersonsReestrQueryArgs> env) {
            var ret = new List<ActionItem>();
            switch(env.CurAction) {
                case DeprivedPersonsReestrActions.View: {
                    ret.Add(new ActionItem("Изменить", new ActionRedirectData(ModuleName, MnuName, new DeprivedPersonsReestrQueryArgs {
                        MenuAction = DeprivedPersonsReestrActions.Edit,
                        PersonId = env.Args.PersonId,
                        Xin = "1"
                    })));
                    break;
                }
                case DeprivedPersonsReestrActions.Add: {
                    break;
                }
                case DeprivedPersonsReestrActions.Edit: {
                    break;
                }
                case DeprivedPersonsReestrActions.GetCommisionData: {
                    break;
                }

            }
            return Task.FromResult(ret.ToArray());
        }

		public override Task<IsActionValidResult> IsActionValid(ActionEnv<DeprivedPersonsReestrQueryArgs> env) {
            switch(env.CurAction) {
                case DeprivedPersonsReestrActions.View: {
                    break;
                }
                case DeprivedPersonsReestrActions.Add: {
                    break;
                }
                case DeprivedPersonsReestrActions.Edit: {
                    break;
                }
                case DeprivedPersonsReestrActions.GetCommisionData: {
                    break;
                }
            }
            return Task.FromResult(new IsActionValidResult(true, null));
        }

		

		public override Task OnRendering(RenderActionEnv<DeprivedPersonsReestrQueryArgs> env) {
            switch(env.CurAction) {
                case DeprivedPersonsReestrActions.View: {
                    var groupBox = new Accordion("Просмотр");
                    var tbPersons = new TbDeprivedPersons().AddFilter(t => t.flId, env.Args.PersonId);
                    var drPerson = tbPersons.SelectFirstOrDefault(t => t.Fields.ToFieldsAliases(), env.QueryExecuter).FirstRow;
                    tbPersons.flXin.RenderCustom(groupBox, env, tbPersons.flXin.GetRowVal(drPerson), readOnly: true);
                    tbPersons.flFio.RenderCustom(groupBox, env, tbPersons.flFio.GetRowVal(drPerson), readOnly: true);
                    tbPersons.flReasonForInclusion.RenderCustom(groupBox, env, tbPersons.flReasonForInclusion.GetRowVal(drPerson), readOnly: true);
                    tbPersons.flDateOfInclusion.RenderCustom(groupBox, env, tbPersons.flDateOfInclusion.GetRowVal(drPerson), readOnly: true);
                    tbPersons.flDateOfExclusion.RenderCustom(groupBox, env, tbPersons.flDateOfExclusion.GetRowValOrDefault(drPerson, (DateTime?)null), readOnly: true);
                    tbPersons.flNote.RenderCustom(groupBox, env, tbPersons.flNote.GetRowVal(drPerson), readOnly: true);
                    tbPersons.flStatus.RenderCustom(groupBox, env, tbPersons.flStatus.GetRowVal(drPerson), readOnly: true);

                    env.Form.AddComponent(groupBox);
                    return Task.CompletedTask;
                }
                case DeprivedPersonsReestrActions.Add: {
                    var groupBox = new Accordion("Добавление");
                    var tbPersons = new TbDeprivedPersons();
                    env.Form.AddComponent(new UiPackages("comission-gbd-caller-package"));
                    tbPersons.flXin.RenderCustom(groupBox, env, readOnly: false);
                    groupBox.AddHtml("<div class='form-group row '><label class='col-sm-3 control-label'></label><div class='col-sm-9'><a class='btn btn-primary' name='btn-xin-caller' id='btn-xin-caller' Value='Запросить' >Запросить</a></div></div>");
                    tbPersons.flFio.RenderCustom(groupBox, env, readOnly: false);
                    tbPersons.flReasonForInclusion.RenderCustom(groupBox, env, readOnly: false);
                    tbPersons.flDateOfInclusion.RenderCustom(groupBox, env, readOnly: false);
                    tbPersons.flDateOfExclusion.RenderCustom(groupBox, env, readOnly: false);
                    tbPersons.flNote.RenderCustom(groupBox, env, readOnly: false);
                    groupBox.AddSubmitButton("","Добавить");

                    env.Form.AddComponent(groupBox);
                    return Task.CompletedTask;
                }
                case DeprivedPersonsReestrActions.Edit: {
                    var groupBox = new Accordion("Изменение");
                    var tbPersons = new TbDeprivedPersons().AddFilter(t => t.flId, env.Args.PersonId);
                    var drPerson = tbPersons.SelectFirstOrDefault(t => t.Fields.ToFieldsAliases(), env.QueryExecuter).FirstRow;

                    tbPersons.flXin.RenderCustom(groupBox, env, tbPersons.flXin.GetRowVal(drPerson), readOnly: false);
                    tbPersons.flFio.RenderCustom(groupBox, env, tbPersons.flFio.GetRowVal(drPerson), readOnly: false);
                    tbPersons.flReasonForInclusion.RenderCustomT(groupBox, env, tbPersons.flReasonForInclusion.GetRowVal(drPerson), readOnly: false);
                    tbPersons.flDateOfInclusion.RenderCustom(groupBox, env, tbPersons.flDateOfInclusion.GetRowVal(drPerson), readOnly: false);
                    tbPersons.flDateOfExclusion.RenderCustom(groupBox, env, tbPersons.flDateOfExclusion.GetRowValOrDefault(drPerson, (DateTime?)null), readOnly: false);
                    tbPersons.flNote.RenderCustom(groupBox, env, tbPersons.flNote.GetRowVal(drPerson), readOnly: false);
                    tbPersons.flStatus.RenderCustomT(groupBox, env, tbPersons.flStatus.GetRowVal(drPerson), readOnly: false);
                    groupBox.AddSubmitButton("", "Изменить");

                    env.Form.AddComponent(groupBox);
                    return Task.CompletedTask;
                }
                case DeprivedPersonsReestrActions.GetCommisionData: {
                    var data = new PersonData();
                    var xin = env.Args.Xin;
                    if(string.IsNullOrEmpty(xin)) {
                        data.Xin = "error";
                        data.Name = "error";
                        env.Redirect.SetRedirectToJson(new Microsoft.AspNetCore.Mvc.JsonResult(data));
                    }
                    var physSearchCollection = new GbdFlObjectCollection();
                    var phys = physSearchCollection.GetFullInfo(xin, env.RequestContext);
                    data.Xin = xin;
                    data.Name = phys.SearchItemText;
                    if(data.Name.StartsWith("Запрос был провален")) {
                        var UlSearchCollection = new GbdUlObjectCollection();
                        var ul = UlSearchCollection.GetFullInfo(xin, env.RequestContext);
                        data.Xin = xin;
                        data.Name = ul.SearchItemText;
                    }
                    env.Redirect.SetRedirectToJson(new Microsoft.AspNetCore.Mvc.JsonResult(data));
                    return Task.CompletedTask;
                }
            }
            throw new NotImplementedException($"Unknown action: {env.CurAction}");
        }

		public override Task OnValidating(ActionEnv<DeprivedPersonsReestrQueryArgs> env) {
            switch(env.CurAction) {
                case DeprivedPersonsReestrActions.Add: {
                    var tbPersons = new TbDeprivedPersons();
                    tbPersons.flFio.Validate(env);
                    tbPersons.flXin.Validate(env);
                    tbPersons.flReasonForInclusion.Validate(env);
                    tbPersons.flDateOfInclusion.Validate(env);
                    tbPersons.flDateOfExclusion.Validate(env);
                    tbPersons.flNote.Validate(env);
                    if(DateTime.TryParse(env.FormCollection["flDateOfExclusion"], out var date) && date < DateTime.Parse(env.FormCollection["flDateOfInclusion"]))
                        env.AddError("flDateExclusion", $"Поле \"{tbPersons.flDateOfExclusion.Text}\" не может быть ранее поля \"{tbPersons.flDateOfInclusion.Text}\"");
                    return Task.CompletedTask;
                }
                case DeprivedPersonsReestrActions.Edit: {
                    var tbPersons = new TbDeprivedPersons();
                    tbPersons.flFio.Validate(env);
                    tbPersons.flXin.Validate(env);
                    tbPersons.flNote.Validate(env);
                    tbPersons.flReasonForInclusion.Validate(env);
                    tbPersons.flDateOfInclusion.Validate(env);
                    tbPersons.flDateOfExclusion.Validate(env);
                    tbPersons.flStatus.Validate(env);
                    return Task.CompletedTask;
                }
            }
            throw new NotImplementedException($"Unknown action: {env.CurAction}");
        }
        public override Task OnProcessing(ActionEnv<DeprivedPersonsReestrQueryArgs> env) {
            switch(env.CurAction) {
                case DeprivedPersonsReestrActions.Add: {
                    var tbPersons = new TbDeprivedPersons();
                    using(var transaction = env.QueryExecuter.BeginTransaction(DbKeys.DbTradeResources)) {
                        var id = env.QueryExecuter.GetNextId(tbPersons, tbPersons.flId.FieldName, transaction);
                        tbPersons.Insert()
                            .Set(t => t.flId, id)
                            .Set(t => t.flFio, tbPersons.flFio.GetVal(env))
                            .Set(t => t.flXin, tbPersons.flXin.GetVal(env))
                            .Set(t => t.flDateOfInclusion, tbPersons.flDateOfInclusion.GetVal(env))
                            .Set(t => t.flDateOfExclusion, tbPersons.flDateOfExclusion.GetValOrNull(env) ?? (object) DBNull.Value)
                            .Set(t => t.flNote, tbPersons.flNote.GetVal(env))
                            .SetT(t => t.flReasonForInclusion, tbPersons.flReasonForInclusion.GetVal(env))
                            .SetT(t => t.flStatus, DeprivedPersonStatus.Active)
                            .Execute(env.QueryExecuter, transaction);
                        transaction.Commit();
                        env.Redirect.SetRedirect(ModuleName, MnuName, new DeprivedPersonsReestrQueryArgs { PersonId = id, MenuAction = DeprivedPersonsReestrActions.View, Xin = "1"});
                    }
                    
                    return Task.CompletedTask;
                }
                case DeprivedPersonsReestrActions.Edit: {
                    var tbPersons = new TbDeprivedPersons().AddFilter(t => t.flId, env.Args.PersonId);
                    using(var transaction = env.QueryExecuter.BeginTransaction(DbKeys.DbTradeResources)) {
                        tbPersons.Update()
                            .Set(t => t.flFio, tbPersons.flFio.GetVal(env))
                            .Set(t => t.flXin, tbPersons.flXin.GetVal(env))
                            .Set(t => t.flDateOfInclusion, tbPersons.flDateOfInclusion.GetVal(env))
                            .Set(t => t.flDateOfExclusion, tbPersons.flDateOfExclusion.GetValOrNull(env) ?? (object)DBNull.Value)
                            .Set(t => t.flNote, tbPersons.flNote.GetVal(env))
                            .SetT(t => t.flReasonForInclusion, tbPersons.flReasonForInclusion.GetVal(env))
                            .SetT(t => t.flStatus, tbPersons.flStatus.GetVal(env))
                            .Execute(env.QueryExecuter, transaction);
                        transaction.Commit();
                    }
                    env.Redirect.SetRedirect(ModuleName, MnuName, new DeprivedPersonsReestrQueryArgs { PersonId = env.Args.PersonId, MenuAction = DeprivedPersonsReestrActions.View, Xin = "1" });
                    
                    return Task.CompletedTask;
                }
            }
            throw new NotImplementedException($"Unknown action: {env.CurAction}");
        }
    }
    public class DeprivedPersonsReestrActions {
        public const string Add = "add";
        public const string Edit = "edit";
        public const string View = "view";
        public const string GetCommisionData = "get-comission-data";
    }
    public class DeprivedPersonsReestrQueryArgs : ActionQueryArgsBase { 
        public int? PersonId { get; set; }
        public string? Xin { get; set; }
    }
}
