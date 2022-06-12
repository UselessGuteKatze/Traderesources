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
using System.Collections.Generic;

namespace TradeResourcesPlugin.Modules.ForestMenus.ForestryPieces {
    public class MnuForestryPieceOrder : MnuOrderBaseV2<ForestryPieceModel, ForestryPieceOrderTypes, ForestryPieceOrderQueryArgs> {
        public const string MnuName = nameof(MnuForestryPieceOrder);

        public MnuForestryPieceOrder(string moduleName, OrderStandartPermissions perms)
            : base(moduleName, MnuName, "Приказ по лесным выделам", perms, () => new TbForestryPiecesOrderResult(), () => new TbForestryPiecesOrderNegotiations()) {
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

        public override Task<ActionItem[]> GetActions(ActionEnv<ForestryPieceOrderQueryArgs> env) {
            var ret = base.GetActions(env);

            switch (env.Args.MenuAction) {
                case Actions.ViewOrder:
                break;
                case Actions.CreateFrom:
                break;
            }

            return ret;
        }

        public override IWizardFormBuilderWithStep<ForestryPieceOrderQueryArgs, ModelOrderArgs<ForestryPieceOrderTypes, ForestryPieceOrderQueryArgs>, ForestryPieceModel>
            EditModel(IWizardFormBuilderWithFinishButton<ForestryPieceOrderQueryArgs, ModelOrderArgs<ForestryPieceOrderTypes, ForestryPieceOrderQueryArgs>, ForestryPieceModel> wizard) {
            return wizard
                .Step("Удаление", step => step
                    .Enabled(env => env.Args.OrderType.In(ForestryPieceOrderTypes.Remove))
                    .OnRendering(re => {
                        var tbRev = new TbForestryPiecesRevisions();
                        var model = re.Model;

                        tbRev.flId.RenderCustom(re.Panel, re.Env, model.flId, readOnly: true);
                        tbRev.flStatus.RenderCustom(re.Panel, re.Env, ForestryPieceStatuses.Deleted.ToString(), cssClass: "hidden");
                        tbRev.flStatus.RenderCustom(re.Panel, re.Env, ForestryPieceStatuses.Deleted.ToString(), readOnly: true);
                        tbRev.flBlock.RenderCustom(re.Panel, re.Env, null);
                    })
                    .OnValidating(ve => {
                    })
                    .OnProcessing(pe => {
                        var tbRev = new TbForestryPiecesRevisions();
                        pe.Model.flStatus = ForestryPieceStatuses.Deleted;
                        pe.Model.flBlock = Enum.Parse<ForestryPieceBlocks>(tbRev.flBlock.GetVal(pe.Env));
                    })
                )
                .Step("Основные данные", step => step
                    .Enabled(env => env.Args.OrderType.In(ForestryPieceOrderTypes.Create, ForestryPieceOrderTypes.Edit))
                    .OnRendering(re => {
                        var tbRev = new TbForestryPiecesRevisions();
                        var model = re.Model;

                        tbRev.flSellerBin.RenderCustom(re.Panel, re.Env, model.flSellerBin, readOnly: true);
                        tbRev.flStatus.RenderCustom(re.Panel, re.Env, model.flStatus.ToString(), readOnly: true);
                        tbRev.flBlock.RenderCustom(re.Panel, re.Env, model.flBlock.ToString(), readOnly: true);

                        tbRev.flNumber.RenderCustom(re.Panel, re.Env, model.flNumber);
                        tbRev.flDescription.RenderCustom(re.Panel, re.Env, model.flDescription);
                        tbRev.flQuarter.RenderCustom(re.Panel, re.Env, model.flQuarter, readOnly: true);
                        tbRev.flArea.RenderCustom(re.Panel, re.Env, model.flArea);
                        tbRev.flForestFundCategory.RenderCustom(re.Panel, re.Env, model.flForestFundCategory.ToString());
                        tbRev.flRestrictions.RenderCustom(re.Panel, re.Env, model.flRestrictions);
                        tbRev.flFellingValue.RenderCustom(re.Panel, re.Env, model.flFellingValue);
                        tbRev.flNote.RenderCustom(re.Panel, re.Env, model.flNote);

                        tbRev.flAnotherInformation.RenderCustom(re.Panel, re.Env, model.flAnotherInformation);

                        if (re.Args.OrderType == ForestryPieceOrderTypes.Edit || re.Args.OriginalArgs.MenuAction == "edit") {
                            re.Panel.AddComponent(new Referencebox("flDraw", new RefDrawGeom(), "Геометрия", new[] { RefDrawGeom.Values.DONTDRAW }) { CssClass = " form-control " });
                        }
                    })
                    .OnValidating(ve => {
                        var tbRev = new TbForestryPiecesRevisions();
                        var checkFields = new Field[] {
                            tbRev.flNumber,
                            tbRev.flDescription,
                            tbRev.flArea,
                            tbRev.flForestFundCategory,
                            tbRev.flRestrictions,
                            tbRev.flFellingValue,
                            tbRev.flNote
                        };
                        checkFields.Each(f => f.Validate(ve.Env));

                        if (ve.Args.OrderType == ForestryPieceOrderTypes.Edit || ve.Args.OriginalArgs.MenuAction == "edit") {
                            if (new Referencebox("flDraw", new RefDrawGeom(), "Геометрия").GetPostedValues<string>(ve.Env.FormCollection).Length == 0) {
                                ve.Env.AddError("flDraw", ve.Env.T("Поле \"Геометрия\" обзяательно для заполнения"));
                            }
                        }

                    })
                    .OnProcessing(pe => {
                        var tbRev = new TbForestryPiecesRevisions();
                        pe.Model.flNumber = tbRev.flNumber.GetVal(pe.Env);
                        pe.Model.flDescription = tbRev.flDescription.GetVal(pe.Env);
                        pe.Model.flArea = tbRev.flArea.GetVal(pe.Env);
                        pe.Model.flForestFundCategory = tbRev.flForestFundCategory.GetVal(pe.Env);
                        pe.Model.flRestrictions = tbRev.flRestrictions.GetVal(pe.Env);
                        pe.Model.flFellingValue = (Dictionary<string, object>[])tbRev.flFellingValue.GetValue(pe.Env);
                        pe.Model.flNote = tbRev.flNote.GetVal(pe.Env);
                        pe.Model.flAnotherInformation = tbRev.flAnotherInformation.GetVal(pe.Env);

                        if (pe.Args.OrderType == ForestryPieceOrderTypes.Edit || pe.Args.OriginalArgs.MenuAction == "edit") {
                            if (new Referencebox("flDraw", new RefDrawGeom(), "Геометрия").GetPostedValues<string>(pe.Env.FormCollection).Contains(RefDrawGeom.Values.DRAW)) {
                                pe.Model.flWKT = null;
                                pe.Model.flCoords = null;
                            }
                        }
                    })
                )
                .Step("Геометрия", step => step
                    .Enabled(env => env.Args.OrderType.In(ForestryPieceOrderTypes.Create, ForestryPieceOrderTypes.Edit) && string.IsNullOrEmpty(env.Model.flWKT))
                    .OnRendering(re => {
                        var tbRev = new TbForestryPieces();
                        var model = re.Model;


                        tbRev.AddFilterNot(t => t.flStatus, ForestryPieceStatuses.Deleted.ToString());
                        tbRev.AddFilter(t => t.flQuarter, model.flQuarter);
                        tbRev.AddFilterNot(t => t.flId, model.flId);
                        tbRev.AddFilterNotNull(t => t.flGeometry);
                        var wkts = tbRev.Select(new FieldAlias[] {
                            tbRev.flWKT
                        }, re.Env.QueryExecuter).AsEnumerable().Select(r => tbRev.flWKT.GetRowVal(r)).ToArray();

                        re.Panel.AddComponent(new Textbox("flWKT", cssClass: "d-none"));
                        re.Panel.AddComponent(new Textbox("flCoords", cssClass: "d-none"));

                        string oldver = null;
                        if (re.Args.OrderType == ForestryPieceOrderTypes.Edit || re.Args.OriginalArgs.MenuAction == "edit") {
                            oldver = new TbForestryPieces().AddFilter(t => t.flId, model.flId).SelectScalar(t => t.flWKT, re.Env.QueryExecuter);
                        }

                        string parent = new TbQuarters()
                            .AddFilter(t => t.flId, model.flQuarter)
                            .SelectScalar(t => t.flWKT, re.Env.QueryExecuter);

                        re.Panel.AddComponent(new ObjectDrawByCoordsComponent("flWKT", "flCoords", wkts, parent, oldver));

                    })
                    .OnValidating(ve => {
                        var tbRev = new TbForestryPiecesRevisions();
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

                                var tbActRevs = new TbForestryPieces();
                                tbActRevs.AddFilter(t => t.flStatus, ForestryPieceStatuses.Active.ToString());
                                tbActRevs.AddFilterNot(t => t.flId, ve.Model.flId);
                                tbActRevs.AddFilter(t => t.flQuarter, ve.Model.flQuarter);
                                tbActRevs.AddFilterNotNull(t => t.flGeometry);

                                var containsForestryPiece = new GeomContains(new SqlExpVal(new FieldToSqlExp(tbActRevs.flGeometry)), new SqlExpVal(geometry));
                                var inresectsForestryPiece = new GeomIntersects(new SqlExpVal(new ExteriorRing(new SqlExpVal(new FieldToSqlExp(tbActRevs.flGeometry)))), new SqlExpVal(geometry));

                                var resultForestryPiece = tbActRevs.Select(new FieldAlias[] {
                                    new FieldAlias(containsForestryPiece, "flContains"),
                                    new FieldAlias(inresectsForestryPiece, "flIntersects"),
                                }, ve.Env.QueryExecuter).AsEnumerable();

                                var tbQuarter = new TbQuarters()
                                    .AddFilter(t => t.flId, ve.Model.flQuarter);

                                var containsQuarter = new GeomContains(new SqlExpVal(new FieldToSqlExp(tbQuarter.flGeometry)), new SqlExpVal(geometry));
                                var inresectsQuarter = new GeomIntersects(new SqlExpVal(new ExteriorRing(new SqlExpVal(new FieldToSqlExp(tbQuarter.flGeometry)))), new SqlExpVal(geometry));

                                var resultQuarter = tbQuarter.Select(new FieldAlias[] {
                                    new FieldAlias(containsQuarter, "flContains"),
                                    new FieldAlias(inresectsQuarter, "flIntersects"),
                                }, ve.Env.QueryExecuter).AsEnumerable();

                                if (resultForestryPiece.Any(r => Convert.ToBoolean(r["flContains"]) || Convert.ToBoolean(r["flIntersects"])) || resultQuarter.Any(r => !Convert.ToBoolean(r["flContains"]) || Convert.ToBoolean(r["flIntersects"]))) {
                                    ve.Env.AddError(tbRev.flWKT.FieldName, ve.Env.T("Нарисованная вами геометрия пересекается с существующими или находится за пределами родителя"));
                                }

                            }
                        }
                    })
                    .OnProcessing(pe => {
                        var tbRev = new TbForestryPiecesRevisions();

                        pe.Model.flWKT = tbRev.flWKT.GetVal(pe.Env);
                        pe.Model.flCoords = tbRev.flCoords.GetVal(pe.Env);
                    })
                )
                ;
        }


        public override ForestryPieceModel GetModel(ActionEnv<ForestryPieceOrderQueryArgs> env, GetModelFrom from, ForestryPieceOrderTypes orderType, ITransaction transaction) {
            var model = from switch {
                GetModelFrom.Empty => new ForestryPieceModel() {
                    flSellerBin = env.User.GetUserBin(env.QueryExecuter),
                    flQuarter = env.Args.QuarterId.Value,
                    flStatus = ForestryPieceStatuses.Active,
                    flBlock = ForestryPieceBlocks.ActiveFree
                },
                GetModelFrom.Entity =>
                    new TbForestryPieces()
                        .AddFilter(t => t.flId, env.Args.Id.Value)
                        .GetForestryPieceModelFirst(env.QueryExecuter, transaction),
                GetModelFrom.Revision =>
                    new TbForestryPiecesRevisions()
                        .AddFilter(t => t.flRevisionId, env.Args.RevisionId.Value)
                        .GetForestryPieceModelFirst(env.QueryExecuter, transaction),
                _ => throw new NotImplementedException()
            };

            if (from == GetModelFrom.Empty) {
                switch (orderType) {
                    case ForestryPieceOrderTypes.Create:
                    model.flStatus = ForestryPieceStatuses.Active;
                    model.flBlock = ForestryPieceBlocks.ActiveFree;
                    break;
                }
            }
            if (from == GetModelFrom.Entity) {
                switch (orderType) {
                    case ForestryPieceOrderTypes.Remove:
                    model.flStatus = ForestryPieceStatuses.Deleted;
                    model.flBlock = ForestryPieceBlocks.DeletedDisqualified;
                    break;
                }
            }

            return model;
        }

        public override ForestryPieceOrderTypes GetOrderType(ActionEnv<ForestryPieceOrderQueryArgs> env, GetOrderModelFrom orderModelFrom) {
            return orderModelFrom switch {
                GetOrderModelFrom.Empty => ForestryPieceOrderTypes.Create,
                _ => env.Args.OrderType switch {
                    ForestryPiecesOrderTypeActions.Remove => ForestryPieceOrderTypes.Remove,
                    ForestryPiecesOrderTypeActions.Edit => ForestryPieceOrderTypes.Edit,
                    _ => throw new ArgumentException($"OrderType: {env.Args.OrderType}")
                }
            };
        }

        public override string GetWizardCancelRedirectUrl(ActionEnv<ForestryPieceOrderQueryArgs> env, ForestryPieceOrderTypes orderType, GetModelFrom modelFrom) {
            var urlHelper = env.RequestContext.GetUrlHelper();
            //TODO
            return modelFrom switch {
                GetModelFrom.Empty => urlHelper.YodaAction(ModuleName, nameof(MnuForestryPieceOrder)),
                GetModelFrom.Entity => urlHelper.YodaAction(ModuleName, nameof(MnuForestryPieceOrder), new ForestryPieceOrderQueryArgs { Id = env.Args.Id.Value, MenuAction = "view" }),
                GetModelFrom.Revision => urlHelper.YodaAction(ModuleName, MenuName, new ForestryPieceOrderQueryArgs { RevisionId = env.Args.RevisionId, MenuAction = Actions.ViewOrder }),
                _ => throw new NotImplementedException()
            };
        }

        public override ValidationResultBase IsCreateFromActionValid(ActionEnv<ForestryPieceOrderQueryArgs> env) {
            if (env.Args.Id == null) {
                return new RedirectResult(new ArgumentException("Id"));
            }

            if (env.Args.OrderType == null) {
                return new RedirectResult(new ArgumentException("OrderType"));
            }

            if (!CanCreateOrder(env.Args.Id.Value, GetOrderType(env, GetOrderModelFrom.Entity), out var error, env)) {
                if (env.Args.Id.HasValue) {
                    return new RedirectResult(ModuleName, MnuForestryPieceView.MnuName, new ForestryPieceViewArgs() { Id = env.Args.Id.Value, MenuAction = MnuForestryPieceView.Actions.View }, error);
                }
                else {
                    return new RedirectResult(ModuleName, "ActionError", null, error);
                }
            }

            return new OkResult();
        }

        public static bool TryGetCurrentInWorkOrderRevId(int id, out int revId, IQueryExecuter queryExecuter) {
            var ret = new TbForestryPiecesOrderResult()
                .AddFilterNotFinishedOrders()
                .JoinT("tbOrders", new TbForestryPiecesRevisions().AddFilter(t => t.flId, id), "tbRevs")
                .On((t1, t2) => new Join(t1.flSubjectId, t2.flRevisionId))
                .SelectScalar(t => t.L.flSubjectId, queryExecuter);

            if (ret == null) {
                revId = 0;
                return false;
            }

            revId = ret.Value;
            return true;
        }

        public static bool CanCreateOrder(int id, ForestryPieceOrderTypes orderType, out string error, ActionEnv<ForestryPieceOrderQueryArgs> env) {
            if (TryGetCurrentInWorkOrderRevId(id, out _, env.QueryExecuter)) {
                error = env.T("Для данного объекта имеется не исполненый приказ");
                return false;
            }

            var foresry = new TbForestryPieces()
                .AddFilter(t => t.flId, id)
                .GetForestryPieceModelFirst(env.QueryExecuter);

            bool checkFieldActive(out string err) {
                if (foresry.flStatus != ForestryPieceStatuses.Active) {
                    err = env.T("Изменение разрешено только для активного объекта");
                    return false;
                }
                err = null;
                return true;
            }
            bool checkActive(out string err) {
                if (foresry.flStatus == ForestryPieceStatuses.Deleted) {
                    err = env.T("Операция невозможна, так как объект не активен");
                    return false;
                }
                err = null;
                return true;
            }

            switch (orderType) {
                case ForestryPieceOrderTypes.Create:
                error = env.T("Недопустимый тип приказа");
                return false;
                case ForestryPieceOrderTypes.Edit:
                return checkFieldActive(out error) && checkActive(out error);
                case ForestryPieceOrderTypes.Remove:
                if (!checkFieldActive(out error)) {
                    return false;
                }

                if (foresry.flStatus == ForestryPieceStatuses.Deleted) {
                    error = env.T("Удаление разрешено только при неактивном земельном участке");
                    return false;
                }
                error = null;
                return true;
                default:
                throw new NotImplementedException($"ForestryPieceOrderTypes: {orderType}");
            }
        }

        public override void SaveModel(int revisionId, ForestryPieceModel model, ForestryPieceOrderTypes orderType, ActionEnv<ForestryPieceOrderQueryArgs> env, ITransaction transaction, GetModelFrom modelFrom) {
            var tbObjRevs = new TbForestryPiecesRevisions();

            if (modelFrom == GetModelFrom.Empty) {
                model.flId = new TbForestryPieces().flId.GetNextId(env.QueryExecuter, transaction);
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

        public static void SetModel<TForestryPiecesTable>(DataModifingQueryProxy<TForestryPiecesTable> updateOrInsert, ForestryPieceModel model) where TForestryPiecesTable : TbForestryPiecesBase {
            ForestryPieceStatuses status = model.flStatus;

            var geometry = new GeomFromWKT(model.flWKT, 4326);
            var point = new PointFromGeom(new SqlExpVal(geometry));

            updateOrInsert
                .SetT(t => t.flId, model.flId)
                .SetT(t => t.flRevisionId, model.flRevisionId)
                .SetT(t => t.flQuarter, model.flQuarter)

                .SetT(t => t.flSellerBin, model.flSellerBin)
                .Set(t => t.flStatus, model.flStatus.ToString())
                .Set(t => t.flBlock, model.flBlock.ToString())

                .Set(t => t.flNumber, model.flNumber)
                .Set(t => t.flDescription, model.flDescription)
                .Set(t => t.flArea, model.flArea)
                .Set(t => t.flForestFundCategory, model.flForestFundCategory.ToString())
                .Set(t => t.flRestrictions, model.flRestrictions.ToString())
                .SetT(t => t.flFellingValue, model.flFellingValue)
                .Set(t => t.flNote, model.flNote)

                .Set(t => t.flWKT, model.flWKT)
                .SetT(t => t.flCoords, model.flCoords)
                .Set(t => t.flGeometry, geometry)
                .Set(t => t.flPoint, point)

                .Set(t => t.flAnotherInformation, model.flAnotherInformation)

                ;
        }

        public override bool TryAcceptModel(int revisionId, ForestryPieceModel model, ForestryPieceOrderTypes orderType, ActionEnv<ForestryPieceOrderQueryArgs> env, ITransaction transaction, out string error) {
            if (orderType == ForestryPieceOrderTypes.Create) {

            }

            var updateOrInsert = orderType switch {
                ForestryPieceOrderTypes.Create =>
                    new TbForestryPieces()
                        .Insert(),
                _ =>
                    new TbForestryPieces()
                        .AddFilter(t => t.flId, model.flId)
                        .Update()
            };

            SetModel(updateOrInsert, model);

            updateOrInsert.Execute(env.QueryExecuter, transaction);

            error = null;
            return true;
        }

        public override void ViewModel(RenderActionEnv<ForestryPieceOrderQueryArgs> env, ForestryPieceModel model, ForestryPieceOrderTypes orderType, OrderStatus orderStatus) {
            ViewModel(env.Form, env, model);
        }
        public static void ViewModel(WidgetBase widget, FormEnvironment env, ForestryPieceModel model) {
            env.RequestContext.Title = "Объект";
            viewModelMainData(widget, env, model);
            viewModelGeometry(widget, env, model);
        }

        public static void viewModelMainData(WidgetBase widget, FormEnvironment env, ForestryPieceModel model, bool collapsed = false) {
            var tbRevs = new TbForestryPiecesRevisions();
            var groupbox = new Accordion(env.T("Основные данные объекта"), collapsed);
            widget.AddComponent(groupbox);

            tbRevs.flId.RenderCustom(groupbox, env, model.flId.ToString() ?? env.T("*Генерируется автоматически при исполнении приказа"), readOnly: true);
            tbRevs.flRevisionId.RenderCustom(groupbox, env, model.flRevisionId, readOnly: true);
            tbRevs.flQuarter.RenderCustom(groupbox, env, model.flQuarter, readOnly: true);
            tbRevs.flSellerBin.RenderCustom(groupbox, env, model.flSellerBin, readOnly: true);
            tbRevs.flNumber.RenderCustom(groupbox, env, model.flNumber, readOnly: true);
            tbRevs.flStatus.RenderCustom(groupbox, env, model.flStatus.ToString(), readOnly: true);
            tbRevs.flBlock.RenderCustom(groupbox, env, model.flBlock.ToString(), readOnly: true);
            tbRevs.flArea.RenderCustom(groupbox, env, model.flArea, readOnly: true);
            tbRevs.flForestFundCategory.RenderCustom(groupbox, env, model.flForestFundCategory, readOnly: true);
            tbRevs.flRestrictions.RenderCustom(groupbox, env, model.flRestrictions, readOnly: true);
            tbRevs.flFellingValue.RenderCustom(groupbox, env, model.flFellingValue, readOnly: true);
            tbRevs.flDescription.RenderCustom(groupbox, env, model.flDescription, readOnly: true);
            tbRevs.flNote.RenderCustom(groupbox, env, model.flNote, readOnly: true);

            tbRevs.flAnotherInformation.RenderCustom(groupbox, env, model.flAnotherInformation, readOnly: true);
        }

        public static void viewModelGeometry(WidgetBase widget, FormEnvironment env, ForestryPieceModel model) {
            var quarterModel = new TbQuarters()
                .AddFilter(t => t.flId, model.flQuarter)
                .GetQuarterModelFirst(env.QueryExecuter);

            var forestryModel = new TbForestries()
                .AddFilter(t => t.flId, quarterModel.flForestry)
                .GetForestryModelFirst(env.QueryExecuter);

            var otherForestryPieces = new TbForestryPieces()
                .AddFilter(t => t.flQuarter, quarterModel.flId)
                .AddFilterNot(t => t.flId, model.flId)
                .GetForestryPieceModels(env.QueryExecuter);

            var groupbox = new Accordion(env.T("Геометрия"))
                .Append(new ObjectGeometryViewerComponent(model.flWKT, wktsNeighbours: otherForestryPieces.Select(x => x.flWKT).Concat(new[] { forestryModel.flWKT, quarterModel.flWKT }).ToArray()));
            widget.AddComponent(groupbox);
        }

        protected override bool HasAccessToOrder(int revisionId, ActionEnv<ForestryPieceOrderQueryArgs> env) {
            if ((!env.User.IsExternalUser() && !env.User.IsGuest())) {
                return true;
            }
            if (env.Args.MenuAction != "view-order" && !env.User.HasRole("TRADERESOURCES-Лесные ресурсы-Создание объектов", env.QueryExecuter)/*!env.User.HasCustomRole("forestobjects", "dataEdit", env.QueryExecuter)*/) {
                throw new AccessDeniedException();
            }
            var hasRecords = new TbForestryPiecesRevisions()
                .AddFilter(t => t.flRevisionId, revisionId)
                .AddFilter(t => t.flSellerBin, env.User.GetUserBin(env.QueryExecuter))
                .Count(env.QueryExecuter) > 0;

            return hasRecords;
        }
    }

    public class ForestryPieceOrderQueryArgs : OrderQueryArgs {
        public int? Id { get; set; }
        public int? QuarterId { get; set; }
        public ForestryPiecesOrderTypeActions? OrderType { get; set; }
    }
    public enum ForestryPiecesOrderTypeActions {
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