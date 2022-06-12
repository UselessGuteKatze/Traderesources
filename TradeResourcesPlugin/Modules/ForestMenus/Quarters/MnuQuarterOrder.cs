using ForestSource.Helpers.Object;
using ForestSource.Models;
using ForestSource.QueryTables.Object;
using ForestSource.References.Object;
using System;
using System.Data;
using System.Linq;
using TradeResourcesPlugin.Helpers;
using TradeResourcesPlugin.Modules.Components;
using UsersResources;
using Yoda.Interfaces;
using Yoda.Interfaces.Forms;
using Yoda.Interfaces.Forms.Components;
using Yoda.YodaReferences;
using YodaHelpers.ActionMenus;
using YodaHelpers.Components;
using YodaHelpers.Fields;
using YodaHelpers.OrderHelpers;
using YodaQuery;
using YodaQuery.Yoda.Query.Expressions;
using YodaApp.YodaHelpers.OrderHelpers;
using System.Threading.Tasks;

namespace TradeResourcesPlugin.Modules.ForestMenus.Quarters {
    public class MnuQuarterOrder : MnuOrderBaseV2<QuarterModel, QuarterOrderTypes, QuarterOrderQueryArgs> {
        public const string MnuName = nameof(MnuQuarterOrder);

        public MnuQuarterOrder(string moduleName, OrderStandartPermissions perms)
            : base(moduleName, MnuName, "Приказ по кварталу", perms, () => new TbQuartersOrderResult(), () => new TbQuartersOrderNegotiations()) {
            AsCallback();
            Enabled(rc => {
                var xin = rc.User.GetUserXin(rc.QueryExecuter);
                if (xin == "050540004455"
                || xin == "050540000002"
                || (!rc.User.IsExternalUser() && !rc.User.IsGuest())
                || rc.User.HasRole("TRADERESOURCES-Лесные ресурсы-Создание объектов", rc.QueryExecuter)/*rc.User.HasCustomRole("forestobjects", "dataEdit", rc.QueryExecuter)*/) {
                    return true;
                }
                return false;
            });
        }

        public override Task<ActionItem[]> GetActions(ActionEnv<QuarterOrderQueryArgs> env) {
            var ret = base.GetActions(env);

            switch (env.Args.MenuAction) {
                case Actions.ViewOrder:
                break;
                case Actions.CreateFrom:
                break;
            }

            return ret;
        }

        public override IWizardFormBuilderWithStep<QuarterOrderQueryArgs, ModelOrderArgs<QuarterOrderTypes, QuarterOrderQueryArgs>, QuarterModel>
            EditModel(IWizardFormBuilderWithFinishButton<QuarterOrderQueryArgs, ModelOrderArgs<QuarterOrderTypes, QuarterOrderQueryArgs>, QuarterModel> wizard) {
            return wizard
                .Step("Удаление", step => step
                    .Enabled(env => env.Args.OrderType.In(QuarterOrderTypes.Remove))
                    .OnRendering(re => {
                        var tbRev = new TbQuartersRevisions();
                        var model = re.Model;

                        tbRev.flId.RenderCustom(re.Panel, re.Env, model.flId, readOnly: true);
                        tbRev.flStatus.RenderCustom(re.Panel, re.Env, ForestryQuarterStatuses.Deleted, readOnly: true);
                    })
                    .OnValidating(ve => {
                    })
                    .OnProcessing(pe => {
                        var tbRev = new TbQuartersRevisions();
                        pe.Model.flStatus = ForestryQuarterStatuses.Deleted;
                    })
                )
                .Step("Основные данные", step => step
                    .Enabled(env => env.Args.OrderType.In(QuarterOrderTypes.Create, QuarterOrderTypes.Edit))
                    .OnRendering(re => {
                        var tbRev = new TbQuartersRevisions();
                        var model = re.Model;

                        tbRev.flSellerBin.RenderCustom(re.Panel, re.Env, model.flSellerBin, readOnly: true);
                        tbRev.flStatus.RenderCustom(re.Panel, re.Env, model.flStatus, readOnly: true);

                        tbRev.flNumber.RenderCustom(re.Panel, re.Env, model.flNumber);
                        tbRev.flForestry.RenderCustom(re.Panel, re.Env, model.flForestry, readOnly: true);
                        tbRev.flArea.RenderCustom(re.Panel, re.Env, model.flArea);
                        tbRev.flDescription.RenderCustom(re.Panel, re.Env, model.flDescription);
                        tbRev.flNote.RenderCustom(re.Panel, re.Env, model.flNote);

                        tbRev.flAnotherInformation.RenderCustom(re.Panel, re.Env, model.flAnotherInformation);

                        if (re.Args.OrderType == QuarterOrderTypes.Edit || re.Args.OriginalArgs.MenuAction == "edit") {
                            re.Panel.AddComponent(new Referencebox("flDraw", new RefDrawGeom(), "Геометрия", new[] { RefDrawGeom.Values.DONTDRAW }) { CssClass = " form-control " });
                        }
                    })
                    .OnValidating(ve => {
                        var tbRev = new TbQuartersRevisions();
                        var checkFields = new Field[] {
                            tbRev.flNumber,
                            tbRev.flArea,
                            tbRev.flDescription,
                            tbRev.flNote
                        };
                        checkFields.Each(f => f.Validate(ve.Env));

                        if (ve.Args.OrderType == QuarterOrderTypes.Edit || ve.Args.OriginalArgs.MenuAction == "edit") {
                            if (new Referencebox("flDraw", new RefDrawGeom(), "Геометрия").GetPostedValues<string>(ve.Env.FormCollection).Length == 0) {
                                ve.Env.AddError("flDraw", ve.Env.T("Поле \"Геометрия\" обзяательно для заполнения"));
                            }
                        }

                    })
                    .OnProcessing(pe => {
                        var tbRev = new TbQuartersRevisions();
                        pe.Model.flNumber = tbRev.flNumber.GetVal(pe.Env);
                        pe.Model.flArea = tbRev.flArea.GetVal(pe.Env);
                        pe.Model.flDescription = tbRev.flDescription.GetVal(pe.Env);
                        pe.Model.flNote = tbRev.flNote.GetVal(pe.Env);
                        pe.Model.flAnotherInformation = tbRev.flAnotherInformation.GetVal(pe.Env);

                        if (pe.Args.OrderType == QuarterOrderTypes.Edit || pe.Args.OriginalArgs.MenuAction == "edit") {
                            if (new Referencebox("flDraw", new RefDrawGeom(), "Геометрия").GetPostedValues<string>(pe.Env.FormCollection).Contains(RefDrawGeom.Values.DRAW)) {
                                pe.Model.flWKT = null;
                                pe.Model.flCoords = null;
                            }
                        }
                    })
                )
                .Step("Геометрия", step => step
                    .Enabled(env => env.Args.OrderType.In(QuarterOrderTypes.Create, QuarterOrderTypes.Edit) && string.IsNullOrEmpty(env.Model.flWKT))
                    .OnRendering(re => {
                        var tbRev = new TbQuarters();
                        var model = re.Model;


                        tbRev.AddFilter(t => t.flStatus, ForestryQuarterStatuses.Active);
                        tbRev.AddFilter(t => t.flForestry, model.flForestry);
                        tbRev.AddFilterNot(t => t.flId, model.flId);
                        tbRev.AddFilterNotNull(t => t.flGeometry);
                        var wkts = tbRev.Select(new FieldAlias[] {
                            tbRev.flWKT
                        }, re.Env.QueryExecuter).AsEnumerable().Select(r => tbRev.flWKT.GetRowVal(r)).ToArray();

                        re.Panel.AddComponent(new Textbox("flWKT", cssClass: "d-none"));
                        re.Panel.AddComponent(new Textbox("flCoords", cssClass: "d-none"));

                        string oldver = null;
                        if (re.Args.OrderType == QuarterOrderTypes.Edit || re.Args.OriginalArgs.MenuAction == "edit") {
                            oldver = new TbQuarters().AddFilter(t => t.flId, model.flId).SelectScalar(t => t.flWKT, re.Env.QueryExecuter);
                        }

                        string parent = new TbForestries()
                            .AddFilter(t => t.flId, model.flForestry)
                            .SelectScalar(t => t.flWKT, re.Env.QueryExecuter);

                        re.Panel.AddComponent(new ObjectDrawByCoordsComponent("flWKT", "flCoords", wkts, parent, oldver));

                    })
                    .OnValidating(ve => {
                        var tbRev = new TbQuartersRevisions();
                        var checkFields = new Field[] {
                            tbRev.flWKT,
                            tbRev.flCoords,
                        };
                        checkFields.Each(f => f.Validate(ve.Env));

                        if (ve.Env.IsValid) {
                            if (string.IsNullOrEmpty(tbRev.flWKT.GetVal(ve.Env)) || tbRev.flWKT.GetVal(ve.Env) == "ERROR") {
                                ve.Env.AddError(tbRev.flWKT.FieldName, ve.Env.T("Перед переходом к следующему шагу проверьте введённые координаты"));
                            }
                            else {
                                var geometry = new GeomFromWKT(tbRev.flWKT.GetVal(ve.Env), 4326);

                                var tbActRevs = new TbQuarters();
                                tbActRevs.AddFilter(t => t.flStatus, ForestryQuarterStatuses.Active);
                                tbActRevs.AddFilterNot(t => t.flId, ve.Model.flId);
                                tbActRevs.AddFilter(t => t.flForestry, ve.Model.flForestry);
                                tbActRevs.AddFilterNotNull(t => t.flGeometry);

                                var containsQuarter = new GeomContains(new SqlExpVal(new FieldToSqlExp(tbActRevs.flGeometry)), new SqlExpVal(geometry));
                                var inresectsQuarter = new GeomIntersects(new SqlExpVal(new ExteriorRing(new SqlExpVal(new FieldToSqlExp(tbActRevs.flGeometry)))), new SqlExpVal(geometry));

                                var resultQuarter = tbActRevs.Select(new FieldAlias[] {
                                    new FieldAlias(containsQuarter, "flContains"),
                                    new FieldAlias(inresectsQuarter, "flIntersects"),
                                }, ve.Env.QueryExecuter).AsEnumerable();

                                var tbForestry = new TbForestries()
                                    .AddFilter(t => t.flId, ve.Model.flForestry);

                                var containsForestry = new GeomContains(new SqlExpVal(new FieldToSqlExp(tbForestry.flGeometry)), new SqlExpVal(geometry));
                                var inresectsForestry = new GeomIntersects(new SqlExpVal(new ExteriorRing(new SqlExpVal(new FieldToSqlExp(tbForestry.flGeometry)))), new SqlExpVal(geometry));

                                var resultForestry = tbForestry.Select(new FieldAlias[] {
                                    new FieldAlias(containsForestry, "flContains"),
                                    new FieldAlias(inresectsForestry, "flIntersects"),
                                }, ve.Env.QueryExecuter).AsEnumerable();

                                if (resultQuarter.Any(r => Convert.ToBoolean(r["flContains"]) || Convert.ToBoolean(r["flIntersects"])) || resultForestry.Any(r => !Convert.ToBoolean(r["flContains"]) || Convert.ToBoolean(r["flIntersects"]))) {
                                    ve.Env.AddError(tbRev.flWKT.FieldName, ve.Env.T("Нарисованная вами геометрия пересекается с существующими или находится за пределами родителя"));
                                }

                            }
                        }
                    })
                    .OnProcessing(pe => {
                        var tbRev = new TbQuartersRevisions();

                        pe.Model.flWKT = tbRev.flWKT.GetVal(pe.Env);
                        pe.Model.flCoords = tbRev.flCoords.GetVal(pe.Env);
                    })
                )
                ;
        }


        public override QuarterModel GetModel(ActionEnv<QuarterOrderQueryArgs> env, GetModelFrom from, QuarterOrderTypes orderType, ITransaction transaction) {
            var model = from switch {
                GetModelFrom.Empty => new QuarterModel() {
                    flSellerBin = env.User.GetUserBin(env.QueryExecuter),
                    flForestry = env.Args.ForestryId.Value,
                    flStatus = ForestryQuarterStatuses.Active
                },
                GetModelFrom.Entity =>
                    new TbQuarters()
                        .AddFilter(t => t.flId, env.Args.Id.Value)
                        .GetQuarterModelFirst(env.QueryExecuter, transaction),
                GetModelFrom.Revision =>
                    new TbQuartersRevisions()
                        .AddFilter(t => t.flRevisionId, env.Args.RevisionId.Value)
                        .GetQuarterModelFirst(env.QueryExecuter, transaction),
                _ => throw new NotImplementedException()
            };

            if (from == GetModelFrom.Empty) {
                switch (orderType) {
                    case QuarterOrderTypes.Create:
                    model.flStatus = ForestryQuarterStatuses.Active;
                    break;
                }
            }
            if (from == GetModelFrom.Entity) {
                switch (orderType) {
                    case QuarterOrderTypes.Remove:
                    model.flStatus = ForestryQuarterStatuses.Deleted;
                    break;
                }
            }

            return model;
        }

        public override QuarterOrderTypes GetOrderType(ActionEnv<QuarterOrderQueryArgs> env, GetOrderModelFrom orderModelFrom) {
            return orderModelFrom switch {
                GetOrderModelFrom.Empty => QuarterOrderTypes.Create,
                _ => env.Args.OrderType switch {
                    QuartersOrderTypeActions.Remove => QuarterOrderTypes.Remove,
                    QuartersOrderTypeActions.Edit => QuarterOrderTypes.Edit,
                    _ => throw new ArgumentException($"OrderType: {env.Args.OrderType}")
                }
            };
        }

        public override string GetWizardCancelRedirectUrl(ActionEnv<QuarterOrderQueryArgs> env, QuarterOrderTypes orderType, GetModelFrom modelFrom) {
            var urlHelper = env.RequestContext.GetUrlHelper();
            //TODO
            return modelFrom switch {
                GetModelFrom.Empty => urlHelper.YodaAction(ModuleName, nameof(MnuQuarterOrder)),
                GetModelFrom.Entity => urlHelper.YodaAction(ModuleName, nameof(MnuQuarterOrder), new QuarterOrderQueryArgs { Id = env.Args.Id.Value, MenuAction = "view" }),
                GetModelFrom.Revision => urlHelper.YodaAction(ModuleName, MenuName, new QuarterOrderQueryArgs { RevisionId = env.Args.RevisionId, MenuAction = Actions.ViewOrder }),
                _ => throw new NotImplementedException()
            };
        }

        public override ValidationResultBase IsCreateFromActionValid(ActionEnv<QuarterOrderQueryArgs> env) {
            if (env.Args.Id == null) {
                return new RedirectResult(new ArgumentException("Id"));
            }

            if (env.Args.OrderType == null) {
                return new RedirectResult(new ArgumentException("OrderType"));
            }

            if (!CanCreateOrder(env.Args.Id.Value, GetOrderType(env, GetOrderModelFrom.Entity), out var error, env)) {
                if (env.Args.Id.HasValue) {
                    return new RedirectResult(ModuleName, MnuQuarterView.MnuName, new QuarterViewArgs() { Id = env.Args.Id.Value, MenuAction = MnuQuarterView.Actions.View }, error);
                }
                else {
                    return new RedirectResult(ModuleName, "ActionError", null, error);
                }
            }

            return new OkResult();
        }

        public static bool TryGetCurrentInWorkOrderRevId(int id, out int revId, IQueryExecuter queryExecuter) {
            var ret = new TbQuartersOrderResult()
                .AddFilterNotFinishedOrders()
                .JoinT("tbOrders", new TbQuartersRevisions().AddFilter(t => t.flId, id), "tbRevs")
                .On((t1, t2) => new Join(t1.flSubjectId, t2.flRevisionId))
                .SelectScalar(t => t.L.flSubjectId, queryExecuter);

            if (ret == null) {
                revId = 0;
                return false;
            }

            revId = ret.Value;
            return true;
        }

        public static bool CanCreateOrder(int id, QuarterOrderTypes orderType, out string error, ActionEnv<QuarterOrderQueryArgs> env) {
            if (TryGetCurrentInWorkOrderRevId(id, out _, env.QueryExecuter)) {
                error = env.T("Для данного объекта имеется не исполненый приказ");
                return false;
            }

            var foresry = new TbQuarters()
                .AddFilter(t => t.flId, id)
                .GetQuarterModelFirst(env.QueryExecuter);

            bool checkFieldActive(out string err) {
                if (foresry.flStatus != ForestryQuarterStatuses.Active) {
                    err = env.T("Изменение разрешено только для активного объекта");
                    return false;
                }
                err = null;
                return true;
            }
            bool checkActive(out string err) {
                if (foresry.flStatus == ForestryQuarterStatuses.Deleted) {
                    err = env.T("Операция невозможна, так как объект не активен");
                    return false;
                }
                err = null;
                return true;
            }

            switch (orderType) {
                case QuarterOrderTypes.Create:
                error = env.T("Недопустимый тип приказа");
                return false;
                case QuarterOrderTypes.Edit:
                return checkFieldActive(out error) && checkActive(out error);
                case QuarterOrderTypes.Remove:
                if (!checkFieldActive(out error)) {
                    return false;
                }

                if (foresry.flStatus == ForestryQuarterStatuses.Deleted) {
                    error = env.T("Удаление разрешено только при неактивном земельном участке");
                    return false;
                }
                error = null;
                return true;
                default:
                throw new NotImplementedException($"QuarterOrderTypes: {orderType}");
            }
        }

        public override void SaveModel(int revisionId, QuarterModel model, QuarterOrderTypes orderType, ActionEnv<QuarterOrderQueryArgs> env, ITransaction transaction, GetModelFrom modelFrom) {
            var tbObjRevs = new TbQuartersRevisions();

            if (modelFrom == GetModelFrom.Empty) {
                model.flId = new TbQuarters().flId.GetNextId(env.QueryExecuter, transaction);
            }

            var updateOrInsert = modelFrom switch {
                GetModelFrom.Revision =>
                    tbObjRevs
                        .AddFilter(t => t.flRevisionId, revisionId)
                        .Update(),

                _ => tbObjRevs
                    .Insert()
            };
            model.flRevisionId = revisionId;

            SetModel(updateOrInsert, model);

            updateOrInsert
                .Execute(env.QueryExecuter, transaction);
        }

        public static void SetModel<TQuartersTable>(DataModifingQueryProxy<TQuartersTable> updateOrInsert, QuarterModel model) where TQuartersTable : TbQuartersBase {
            ForestryQuarterStatuses status = model.flStatus;

            var geometry = new GeomFromWKT(model.flWKT, 4326);
            var point = new PointFromGeom(new SqlExpVal(geometry));

            updateOrInsert
                .SetT(t => t.flId, model.flId)
                .SetT(t => t.flRevisionId, model.flRevisionId)
                .SetT(t => t.flForestry, model.flForestry)

                .SetT(t => t.flSellerBin, model.flSellerBin)
                .Set(t => t.flStatus, model.flStatus.ToString())

                .Set(t => t.flNumber, model.flNumber)
                .Set(t => t.flArea, model.flArea)
                .Set(t => t.flDescription, model.flDescription)
                .Set(t => t.flNote, model.flNote)

                .Set(t => t.flWKT, model.flWKT)
                .SetT(t => t.flCoords, model.flCoords)
                .Set(t => t.flGeometry, geometry)
                .Set(t => t.flPoint, point)

                .Set(t => t.flAnotherInformation, model.flAnotherInformation)

                ;
        }

        public override bool TryAcceptModel(int revisionId, QuarterModel model, QuarterOrderTypes orderType, ActionEnv<QuarterOrderQueryArgs> env, ITransaction transaction, out string error) {
            if (orderType == QuarterOrderTypes.Create) {

            }

            var updateOrInsert = orderType switch {
                QuarterOrderTypes.Create =>
                    new TbQuarters()
                        .Insert(),
                _ =>
                    new TbQuarters()
                        .AddFilter(t => t.flId, model.flId)
                        .Update()
            };

            SetModel(updateOrInsert, model);

            updateOrInsert.Execute(env.QueryExecuter, transaction);

            error = null;
            return true;
        }

        public override void ViewModel(RenderActionEnv<QuarterOrderQueryArgs> env, QuarterModel model, QuarterOrderTypes orderType, OrderStatus orderStatus) {
            ViewModel(env.Form, env, model);
        }
        public static void ViewModel(WidgetBase widget, FormEnvironment env, QuarterModel model) {
            env.RequestContext.Title = "Объект";
            viewModelMainData(widget, env, model);
            viewModelGeometry(widget, env, model);
        }

        public static void viewModelMainData(WidgetBase widget, FormEnvironment env, QuarterModel model, bool collapsed = false) {
            var tbRevs = new TbQuartersRevisions();
            var groupbox = new Accordion(env.T("Основные данные объекта"), collapsed);
            widget.AddComponent(groupbox);

            tbRevs.flId.RenderCustom(groupbox, env, model.flId.ToString() ?? env.T("*Генерируется автоматически при исполнении приказа"), readOnly: true);
            tbRevs.flRevisionId.RenderCustom(groupbox, env, model.flRevisionId, readOnly: true);
            tbRevs.flSellerBin.RenderCustom(groupbox, env, model.flSellerBin, readOnly: true);
            tbRevs.flStatus.RenderCustom(groupbox, env, model.flStatus, readOnly: true);
            tbRevs.flNumber.RenderCustom(groupbox, env, model.flNumber, readOnly: true);
            tbRevs.flForestry.RenderCustom(groupbox, env, model.flForestry, readOnly: true);
            tbRevs.flArea.RenderCustom(groupbox, env, model.flArea, readOnly: true);
            tbRevs.flDescription.RenderCustom(groupbox, env, model.flDescription, readOnly: true);
            tbRevs.flNote.RenderCustom(groupbox, env, model.flNote, readOnly: true);

            tbRevs.flAnotherInformation.RenderCustom(groupbox, env, model.flAnotherInformation, readOnly: true);
        }

        public static void viewModelGeometry(WidgetBase widget, FormEnvironment env, QuarterModel model) {
            var forestryModel = new TbForestries()
                .AddFilter(t => t.flId, model.flForestry)
                .GetForestryModelFirst(env.QueryExecuter);

            var otherQuarters = new TbQuarters()
                .AddFilter(t => t.flForestry, forestryModel.flId)
                .AddFilterNot(t => t.flId, model.flId)
                .GetQuarterModels(env.QueryExecuter);

            var groupbox = new Accordion(env.T("Геометрия"))
                .Append(new ObjectGeometryViewerComponent(model.flWKT, wktsNeighbours: otherQuarters.Select(x => x.flWKT).Concat(new[] { forestryModel.flWKT }).ToArray()));
            widget.AddComponent(groupbox);
        }

        protected override bool HasAccessToOrder(int revisionId, ActionEnv<QuarterOrderQueryArgs> env) {
            if ((!env.User.IsExternalUser() && !env.User.IsGuest())) {
                return true;
            }
            if (env.Args.MenuAction != "view-order" && !env.User.HasRole("TRADERESOURCES-Лесные ресурсы-Создание объектов", env.QueryExecuter)/*!env.User.HasCustomRole("forestobjects", "dataEdit", env.QueryExecuter)*/) {
                throw new AccessDeniedException();
            }
            var hasRecords = new TbQuartersRevisions()
                .AddFilter(t => t.flRevisionId, revisionId)
                .AddFilter(t => t.flSellerBin, env.User.GetUserBin(env.QueryExecuter))
                .Count(env.QueryExecuter) > 0;

            return hasRecords;
        }
    }

    public class QuarterOrderQueryArgs : OrderQueryArgs {
        public int? Id { get; set; }
        public int? ForestryId { get; set; }
        public QuartersOrderTypeActions? OrderType { get; set; }
    }
    public enum QuartersOrderTypeActions {
        Create,
        Edit,
        Remove
    }
    public enum ObjectFillSteps {
        InitObject, FillData
    }

    public class RefDrawGeom : Reference {
        public const string RefName = nameof(RefDrawGeom);
        public RefDrawGeom() : base(RefName, "Геометрия") {
            Items = new ReferenceItemCollection {
                {Values.DRAW, "Рисовать"},
                {Values.DONTDRAW, "Не изменять"}
            };
        }

        public class Values {
            public const string
                DRAW = "DRAW",
                DONTDRAW = "DONTDRAW"
            ;
        }
    }

}