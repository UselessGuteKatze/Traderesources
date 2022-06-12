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

namespace TradeResourcesPlugin.Modules.ForestMenus.Forestries {
    public class MnuForestryOrder : MnuOrderBaseV2<ForestryModel, ForestryOrderTypes, ForestryOrderQueryArgs> {
        public const string MnuName = nameof(MnuForestryOrder);

        public MnuForestryOrder(string moduleName, OrderStandartPermissions perms)
            : base(moduleName, MnuName, "Приказ по лесному хозяйству", perms, () => new TbForestriesOrderResult(), () => new TbForestriesOrderNegotiations()) {
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

        public override Task<ActionItem[]> GetActions(ActionEnv<ForestryOrderQueryArgs> env) {
            var ret = base.GetActions(env);

            switch (env.Args.MenuAction) {
                case Actions.ViewOrder:
                break;
                case Actions.CreateFrom:
                break;
            }

            return ret;
        }

        public override IWizardFormBuilderWithStep<ForestryOrderQueryArgs, ModelOrderArgs<ForestryOrderTypes, ForestryOrderQueryArgs>, ForestryModel>
            EditModel(IWizardFormBuilderWithFinishButton<ForestryOrderQueryArgs, ModelOrderArgs<ForestryOrderTypes, ForestryOrderQueryArgs>, ForestryModel> wizard) {
            return wizard
                .Step("Удаление", step => step
                    .Enabled(env => env.Args.OrderType.In(ForestryOrderTypes.Remove))
                    .OnRendering(re => {
                        var tbRev = new TbForestriesRevisions();
                        var model = re.Model;

                        tbRev.flId.RenderCustom(re.Panel, re.Env, model.flId, readOnly: true);
                        tbRev.flStatus.RenderCustom(re.Panel, re.Env, ForestryQuarterStatuses.Deleted, readOnly: true);
                    })
                    .OnValidating(ve => {
                    })
                    .OnProcessing(pe => {
                        var tbRev = new TbForestriesRevisions();
                        pe.Model.flStatus = ForestryQuarterStatuses.Deleted;
                    })
                )
                .Step("Основные данные", step => step
                    .Enabled(env => env.Args.OrderType.In(ForestryOrderTypes.Create, ForestryOrderTypes.Edit))
                    .OnRendering(re => {
                        var tbRev = new TbForestriesRevisions();
                        var model = re.Model;

                        tbRev.flSellerBin.RenderCustom(re.Panel, re.Env, model.flSellerBin, readOnly: true);
                        tbRev.flStatus.RenderCustom(re.Panel, re.Env, model.flStatus, readOnly: true);

                        tbRev.flName.RenderCustom(re.Panel, re.Env, model.flName);
                        tbRev.flArea.RenderCustom(re.Panel, re.Env, model.flArea);
                        tbRev.flDescription.RenderCustom(re.Panel, re.Env, model.flDescription);
                        tbRev.flNote.RenderCustom(re.Panel, re.Env, model.flNote);

                        tbRev.flAnotherInformation.RenderCustom(re.Panel, re.Env, model.flAnotherInformation);
                    })
                    .OnValidating(ve => {
                        var tbRev = new TbForestriesRevisions();
                        var checkFields = new Field[] {
                            tbRev.flName,
                            tbRev.flArea,
                            tbRev.flDescription,
                            tbRev.flNote
                        };
                        checkFields.Each(f => f.Validate(ve.Env));

                    })
                    .OnProcessing(pe => {
                        var tbRev = new TbForestriesRevisions();
                        pe.Model.flName = tbRev.flName.GetVal(pe.Env);
                        pe.Model.flArea = tbRev.flArea.GetVal(pe.Env);
                        pe.Model.flDescription = tbRev.flDescription.GetVal(pe.Env);
                        pe.Model.flNote = tbRev.flNote.GetVal(pe.Env);
                        pe.Model.flAnotherInformation = tbRev.flAnotherInformation.GetVal(pe.Env);
                    })
                )
                .Step("Географическое расположение", step => step
                    .Enabled(env => env.Args.OrderType.In(ForestryOrderTypes.Create, ForestryOrderTypes.Edit))
                    .OnRendering(re => {
                        var tbRev = new TbForestriesRevisions();
                        var model = re.Model;

                        tbRev.flCountry.RenderCustom(re.Panel, re.Env, model.flCountry);
                        tbRev.flRegion.RenderCustom(re.Panel, re.Env, model.flRegion);
                        tbRev.flDistrict.RenderCustom(re.Panel, re.Env, model.flDistrict);
                        tbRev.flLocation.RenderCustom(re.Panel, re.Env, model.flLocation);

                        if (re.Args.OrderType == ForestryOrderTypes.Edit || re.Args.OriginalArgs.MenuAction == "edit") {
                            re.Panel.AddComponent(new Referencebox("flDraw", new RefDrawGeom(), "Геометрия", new[] { RefDrawGeom.Values.DONTDRAW }) { CssClass = " form-control " });
                        }
                    })
                    .OnValidating(ve => {
                        var tbRev = new TbForestriesRevisions();
                        var checkFields = new Field[] {
                            tbRev.flCountry,
                            tbRev.flRegion,
                            tbRev.flDistrict,
                            tbRev.flLocation
                        };
                        checkFields.Each(f => f.Validate(ve.Env));

                        if (ve.Env.IsValid) {
                            var tb = new TbForestries()
                                .AddFilter(t => t.flName, ve.Model.flName)
                                .AddFilterNot(t => t.flId, ve.Model.flId)
                                .AddFilter(t => t.flRegion, tbRev.flRegion.GetVal(ve.Env));

                            if (tb.Count(ve.Env.QueryExecuter) > 0) {
                                ve.Env.AddError(tbRev.flRegion.FieldName, ve.Env.T("В выбранной области уже создано лесное хозяйство с аналогичным названием (возможно это оно и есть, в противном случае переименуйте создаваемое хозяйство, например добавьте число в конце названия)"));
                            }
                        }

                        if (ve.Args.OrderType == ForestryOrderTypes.Edit || ve.Args.OriginalArgs.MenuAction == "edit") {
                            if (new Referencebox("flDraw", new RefDrawGeom(), "Геометрия").GetPostedValues<string>(ve.Env.FormCollection).Length == 0) {
                                ve.Env.AddError("flDraw", ve.Env.T("Поле \"Геометрия\" обзяательно для заполнения"));
                            }
                        }
                    })
                    .OnProcessing(pe => {
                        var tbRev = new TbForestriesRevisions();

                        pe.Model.flCountry = tbRev.flCountry.GetVal(pe.Env);
                        pe.Model.flRegion = tbRev.flRegion.GetVal(pe.Env);
                        pe.Model.flDistrict = tbRev.flDistrict.GetVal(pe.Env);
                        pe.Model.flLocation = tbRev.flLocation.GetVal(pe.Env);

                        if (pe.Args.OrderType == ForestryOrderTypes.Edit || pe.Args.OriginalArgs.MenuAction == "edit") {
                            if (new Referencebox("flDraw", new RefDrawGeom(), "Геометрия").GetPostedValues<string>(pe.Env.FormCollection).Contains(RefDrawGeom.Values.DRAW)) {
                                pe.Model.flWKT = null;
                                pe.Model.flCoords = null;
                            }
                        }
                    })
                )
                .Step("Геометрия", step => step
                    .Enabled(env => env.Args.OrderType.In(ForestryOrderTypes.Create, ForestryOrderTypes.Edit) && string.IsNullOrEmpty(env.Model.flWKT))
                    .OnRendering(re => {
                        var tbRev = new TbForestries();
                        var model = re.Model;


                        tbRev.AddFilter(t => t.flStatus, ForestryQuarterStatuses.Active);
                        tbRev.AddFilterNot(t => t.flId, model.flId);
                        tbRev.AddFilterNotNull(t => t.flGeometry);
                        var wkts = tbRev.Select(new FieldAlias[] {
                            tbRev.flWKT
                        }, re.Env.QueryExecuter).AsEnumerable().Select(r => tbRev.flWKT.GetRowVal(r)).ToArray();

                        re.Panel.AddComponent(new Textbox("flWKT", cssClass: "d-none"));
                        re.Panel.AddComponent(new Textbox("flCoords", cssClass: "d-none"));

                        string oldver = null;
                        if (re.Args.OrderType == ForestryOrderTypes.Edit || re.Args.OriginalArgs.MenuAction == "edit") {
                            oldver = new TbForestries().AddFilter(t => t.flId, model.flId).SelectScalar(t => t.flWKT, re.Env.QueryExecuter);
                        }

                        re.Panel.AddComponent(new ObjectDrawByCoordsComponent("flWKT", "flCoords", wkts, null, oldver));

                    })
                    .OnValidating(ve => {
                        var tbRev = new TbForestriesRevisions();
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
                                var tbActRevs = new TbForestries();
                                tbActRevs.AddFilter(t => t.flStatus, ForestryQuarterStatuses.Active);
                                tbActRevs.AddFilterNot(t => t.flId, ve.Model.flId);
                                tbActRevs.AddFilterNotNull(t => t.flGeometry);

                                var geometry = new GeomFromWKT(tbRev.flWKT.GetVal(ve.Env), 4326);

                                var contains = new GeomContains(new SqlExpVal(new FieldToSqlExp(tbActRevs.flGeometry)), new SqlExpVal(geometry));
                                var inresects = new GeomIntersects(new SqlExpVal(new ExteriorRing(new SqlExpVal(new FieldToSqlExp(tbActRevs.flGeometry)))), new SqlExpVal(geometry));

                                var result = tbActRevs.Select(new FieldAlias[] {
                                new FieldAlias(contains, "flContains"),
                                new FieldAlias(inresects, "flIntersects"),
                            }, ve.Env.QueryExecuter).AsEnumerable();

                                if (result.Any(r => Convert.ToBoolean(r["flContains"]) || Convert.ToBoolean(r["flIntersects"]))) {
                                    ve.Env.AddError(tbRev.flWKT.FieldName, ve.Env.T("Нарисованная вами геометрия пересекается с существующими"));
                                }

                            }
                        }
                    })
                    .OnProcessing(pe => {
                        var tbRev = new TbForestriesRevisions();

                        pe.Model.flWKT = tbRev.flWKT.GetVal(pe.Env);
                        pe.Model.flCoords = tbRev.flCoords.GetVal(pe.Env);
                    })
                )
                ;
        }


        public override ForestryModel GetModel(ActionEnv<ForestryOrderQueryArgs> env, GetModelFrom from, ForestryOrderTypes orderType, ITransaction transaction) {
            var model = from switch {
                GetModelFrom.Empty => new ForestryModel() {
                    flSellerBin = env.User.GetUserBin(env.QueryExecuter),
                    flStatus = ForestryQuarterStatuses.Active
                },
                GetModelFrom.Entity =>
                    new TbForestries()
                        .AddFilter(t => t.flId, env.Args.Id.Value)
                        .GetForestryModelFirst(env.QueryExecuter, transaction),
                GetModelFrom.Revision =>
                    new TbForestriesRevisions()
                        .AddFilter(t => t.flRevisionId, env.Args.RevisionId.Value)
                        .GetForestryModelFirst(env.QueryExecuter, transaction),
                _ => throw new NotImplementedException()
            };

            if (from == GetModelFrom.Empty) {
                switch (orderType) {
                    case ForestryOrderTypes.Create:
                    model.flStatus = ForestryQuarterStatuses.Active;
                    break;
                }
            }
            if (from == GetModelFrom.Entity) {
                switch (orderType) {
                    case ForestryOrderTypes.Remove:
                    model.flStatus = ForestryQuarterStatuses.Deleted;
                    break;
                }
            }

            return model;
        }

        public override ForestryOrderTypes GetOrderType(ActionEnv<ForestryOrderQueryArgs> env, GetOrderModelFrom orderModelFrom) {
            return orderModelFrom switch {
                GetOrderModelFrom.Empty => ForestryOrderTypes.Create,
                _ => env.Args.OrderType switch {
                    ForestriesOrderTypeActions.Remove => ForestryOrderTypes.Remove,
                    ForestriesOrderTypeActions.Edit => ForestryOrderTypes.Edit,
                    _ => throw new ArgumentException($"OrderType: {env.Args.OrderType}")
                }
            };
        }

        public override string GetWizardCancelRedirectUrl(ActionEnv<ForestryOrderQueryArgs> env, ForestryOrderTypes orderType, GetModelFrom modelFrom) {
            var urlHelper = env.RequestContext.GetUrlHelper();
            //TODO
            return modelFrom switch {
                GetModelFrom.Empty => urlHelper.YodaAction(ModuleName, nameof(MnuForestryOrder)),
                GetModelFrom.Entity => urlHelper.YodaAction(ModuleName, nameof(MnuForestryOrder), new ForestryOrderQueryArgs { Id = env.Args.Id.Value, MenuAction = "view" }),
                GetModelFrom.Revision => urlHelper.YodaAction(ModuleName, MenuName, new ForestryOrderQueryArgs { RevisionId = env.Args.RevisionId, MenuAction = Actions.ViewOrder }),
                _ => throw new NotImplementedException()
            };
        }

        public override ValidationResultBase IsCreateFromActionValid(ActionEnv<ForestryOrderQueryArgs> env) {
            if (env.Args.Id == null) {
                return new RedirectResult(new ArgumentException("Id"));
            }

            if (env.Args.OrderType == null) {
                return new RedirectResult(new ArgumentException("OrderType"));
            }

            if (!CanCreateOrder(env.Args.Id.Value, GetOrderType(env, GetOrderModelFrom.Entity), out var error, env)) {
                if (env.Args.Id.HasValue) {
                    return new RedirectResult(ModuleName, MnuForestryView.MnuName, new ForestryViewArgs() { Id = env.Args.Id.Value, MenuAction = MnuForestryView.Actions.View }, error);
                }
                else {
                    return new RedirectResult(ModuleName, "ActionError", null, error);
                }
            }

            return new OkResult();
        }

        public static bool TryGetCurrentInWorkOrderRevId(int id, out int revId, IQueryExecuter queryExecuter) {
            var ret = new TbForestriesOrderResult()
                .AddFilterNotFinishedOrders()
                .JoinT("tbOrders", new TbForestriesRevisions().AddFilter(t => t.flId, id), "tbRevs")
                .On((t1, t2) => new Join(t1.flSubjectId, t2.flRevisionId))
                .SelectScalar(t => t.L.flSubjectId, queryExecuter);

            if (ret == null) {
                revId = 0;
                return false;
            }

            revId = ret.Value;
            return true;
        }

        public static bool CanCreateOrder(int id, ForestryOrderTypes orderType, out string error, ActionEnv<ForestryOrderQueryArgs> env) {
            if (TryGetCurrentInWorkOrderRevId(id, out _, env.QueryExecuter)) {
                error = env.T("Для данного объекта имеется не исполненый приказ");
                return false;
            }

            var foresry = new TbForestries()
                .AddFilter(t => t.flId, id)
                .GetForestryModelFirst(env.QueryExecuter);

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
                case ForestryOrderTypes.Create:
                error = env.T("Недопустимый тип приказа");
                return false;
                case ForestryOrderTypes.Edit:
                return checkFieldActive(out error) && checkActive(out error);
                case ForestryOrderTypes.Remove:
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
                throw new NotImplementedException($"ForestryOrderTypes: {orderType}");
            }
        }

        public override void SaveModel(int revisionId, ForestryModel model, ForestryOrderTypes orderType, ActionEnv<ForestryOrderQueryArgs> env, ITransaction transaction, GetModelFrom modelFrom) {
            var tbObjRevs = new TbForestriesRevisions();

            if (modelFrom == GetModelFrom.Empty) {
                model.flId = new TbForestries().flId.GetNextId(env.QueryExecuter, transaction);
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

        public static void SetModel<TForestriesTable>(DataModifingQueryProxy<TForestriesTable> updateOrInsert, ForestryModel model) where TForestriesTable : TbForestriesBase {
            ForestryQuarterStatuses status = model.flStatus;

            var geometry = new GeomFromWKT(model.flWKT, 4326);
            var point = new PointFromGeom(new SqlExpVal(geometry));

            updateOrInsert
                .SetT(t => t.flId, model.flId)
                .SetT(t => t.flRevisionId, model.flRevisionId)

                .SetT(t => t.flSellerBin, model.flSellerBin)
                .Set(t => t.flStatus, model.flStatus.ToString())

                .Set(t => t.flName, model.flName)
                .Set(t => t.flArea, model.flArea)
                .Set(t => t.flDescription, model.flDescription)
                .Set(t => t.flNote, model.flNote)

                .Set(t => t.flCountry, model.flCountry)
                .Set(t => t.flRegion, model.flRegion)
                .Set(t => t.flDistrict, model.flDistrict)
                .Set(t => t.flLocation, model.flLocation)
                .Set(t => t.flWKT, model.flWKT)
                .SetT(t => t.flCoords, model.flCoords)
                .Set(t => t.flGeometry, geometry)
                .Set(t => t.flPoint, point)

                .Set(t => t.flAnotherInformation, model.flAnotherInformation)

                ;
        }

        public override bool TryAcceptModel(int revisionId, ForestryModel model, ForestryOrderTypes orderType, ActionEnv<ForestryOrderQueryArgs> env, ITransaction transaction, out string error) {
            if (orderType == ForestryOrderTypes.Create) {

            }

            var updateOrInsert = orderType switch {
                ForestryOrderTypes.Create =>
                    new TbForestries()
                        .Insert(),
                _ =>
                    new TbForestries()
                        .AddFilter(t => t.flId, model.flId)
                        .Update()
            };

            SetModel(updateOrInsert, model);

            updateOrInsert.Execute(env.QueryExecuter, transaction);

            error = null;
            return true;
        }

        public override void ViewModel(RenderActionEnv<ForestryOrderQueryArgs> env, ForestryModel model, ForestryOrderTypes orderType, OrderStatus orderStatus) {
            ViewModel(env.Form, env, model);
        }
        public static void ViewModel(WidgetBase widget, FormEnvironment env, ForestryModel model) {
            env.RequestContext.Title = "Объект";
            viewModelMainData(widget, env, model);
            viewModelGeoPositionData(widget, env, model);
            viewModelGeometry(widget, env, model);
        }

        public static void viewModelMainData(WidgetBase widget, FormEnvironment env, ForestryModel model, bool collapsed = false) {
            var tbRevs = new TbForestriesRevisions();
            var groupbox = new Accordion(env.T("Основные данные объекта"), collapsed);
            widget.AddComponent(groupbox);

            tbRevs.flId.RenderCustom(groupbox, env, model.flId.ToString() ?? env.T("*Генерируется автоматически при исполнении приказа"), readOnly: true);
            tbRevs.flRevisionId.RenderCustom(groupbox, env, model.flRevisionId, readOnly: true);
            tbRevs.flSellerBin.RenderCustom(groupbox, env, model.flSellerBin, readOnly: true);
            tbRevs.flStatus.RenderCustom(groupbox, env, model.flStatus, readOnly: true);
            tbRevs.flName.RenderCustom(groupbox, env, model.flName, readOnly: true);
            tbRevs.flDescription.RenderCustom(groupbox, env, model.flDescription, readOnly: true);
            tbRevs.flNote.RenderCustom(groupbox, env, model.flNote, readOnly: true);

            tbRevs.flAnotherInformation.RenderCustom(groupbox, env, model.flAnotherInformation, readOnly: true);
        }

        public static void viewModelGeoPositionData(WidgetBase widget, FormEnvironment env, ForestryModel model, bool collapsed = false) {
            var tbRevs = new TbForestriesRevisions();
            var groupbox = new Accordion(env.T("Географическое расположение"), collapsed);
            widget.AddComponent(groupbox);

            tbRevs.flCountry.RenderCustom(groupbox, env, model.flCountry, readOnly: true);
            tbRevs.flRegion.RenderCustom(groupbox, env, model.flRegion, readOnly: true);
            tbRevs.flDistrict.RenderCustom(groupbox, env, model.flDistrict, readOnly: true);
            tbRevs.flLocation.RenderCustom(groupbox, env, model.flLocation, readOnly: true);
        }

        public static void viewModelGeometry(WidgetBase widget, FormEnvironment env, ForestryModel model) {
            var groupbox = new Accordion(env.T("Геометрия"))
                .Append(new ObjectGeometryViewerComponent(model.flWKT));
            widget.AddComponent(groupbox);
        }

        protected override bool HasAccessToOrder(int revisionId, ActionEnv<ForestryOrderQueryArgs> env) {
            if ((!env.User.IsExternalUser() && !env.User.IsGuest())) {
                return true;
            }
            if (env.Args.MenuAction != "view-order" && !env.User.HasRole("TRADERESOURCES-Лесные ресурсы-Создание объектов", env.QueryExecuter)/*!env.User.HasCustomRole("forestobjects", "dataEdit", env.QueryExecuter)*/) {
                throw new AccessDeniedException();
            }
            var hasRecords = new TbForestriesRevisions()
                .AddFilter(t => t.flRevisionId, revisionId)
                .AddFilter(t => t.flSellerBin, env.User.GetUserBin(env.QueryExecuter))
                .Count(env.QueryExecuter) > 0;

            return hasRecords;
        }
    }

    public class ForestryOrderQueryArgs : OrderQueryArgs {
        public int? Id { get; set; }
        public ForestriesOrderTypeActions? OrderType { get; set; }
    }
    public enum ForestriesOrderTypeActions {
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