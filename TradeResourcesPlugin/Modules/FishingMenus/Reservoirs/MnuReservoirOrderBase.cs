using CommonSource;
using FishingSource.Helpers.Reservoir;
using FishingSource.Models;
using FishingSource.QueryTables.Reservoir;
using FishingSource.References.Reservoir;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using TradeResourcesPlugin.Helpers;
using TradeResourcesPlugin.Modules.Components;
using UsersResources;
using Yoda.Interfaces;
using Yoda.Interfaces.Forms;
using Yoda.Interfaces.Forms.Components;
using Yoda.YodaReferences;
using YodaApp.YodaHelpers.OrderHelpers;
using YodaHelpers.ActionMenus;
using YodaHelpers.Components;
using YodaHelpers.Fields;
using YodaHelpers.OrderHelpers;
using YodaQuery;
using YodaQuery.Yoda.Query.Expressions;


namespace TradeResourcesPlugin.Modules.FishingMenus.Reservoirs {
    public class MnuReservoirOrderBase : MnuOrderBaseV2<ReservoirModel, ReservoirsOrderTypes, ReservoirOrderQueryArgs> {

        public MnuReservoirOrderBase(string moduleName, string menuName, OrderStandartPermissions perms)
            : base(moduleName, menuName, "Приказ по водоёмам", perms, () => new TbReservoirOrderResult(), () => new TbReservoirOrdersNegotiations()) {
            AsCallback();
            Enabled(rc => {
                var xin = rc.User.GetUserXin(rc.QueryExecuter);
                if (xin == "050540004455"
                || xin == "050540000002" 
                || (!rc.User.IsExternalUser() && !rc.User.IsGuest())
                || rc.User.HasRole("TRADERESOURCES-Рыбохозяйственные водоёмы-Создание приказов", rc.QueryExecuter)/*rc.User.HasCustomRole("fishingobjects", "dataEdit", rc.QueryExecuter)*/)
                {
                    return true;
                }
                return false;
            });
        }

        public override Task<ActionItem[]> GetActions(ActionEnv<ReservoirOrderQueryArgs> env) {
            var ret = base.GetActions(env).Result.ToList();

            switch (env.Args.MenuAction) {
                case Actions.ViewOrder:
                    break;
                case Actions.CreateFrom:
                    break;
            }

            return Task.FromResult(ret.ToArray());
        }

        public override IWizardFormBuilderWithStep<ReservoirOrderQueryArgs, ModelOrderArgs<ReservoirsOrderTypes, ReservoirOrderQueryArgs>, ReservoirModel>
            EditModel(IWizardFormBuilderWithFinishButton<ReservoirOrderQueryArgs, ModelOrderArgs<ReservoirsOrderTypes, ReservoirOrderQueryArgs>, ReservoirModel> wizard) {
            return wizard
                .Step("Удаление", step => step
                    .Enabled(env => env.Args.OrderType.In(ReservoirsOrderTypes.Remove))
                    .OnRendering(re => {
                        var tbRev = new TbReservoirsRevisions();
                        var model = re.Model;

                        tbRev.flId.RenderCustom(re.Panel, re.Env, model.flId, readOnly: true);
                        tbRev.flStatus.RenderCustom(re.Panel, re.Env, ReservoirStatuses.Deleted.ToString(), readOnly: true);
                    })
                    .OnValidating(ve => {
                        //var tbRev = new TbReservoirsRevisions();
                        //var model = ve.Model;
                        //
                        //var tbTrades = new TbObjectsTrades();
                        //tbTrades.AddFilter(tbTrades.flStatus, RefObjectTradeStatuses.Wait);
                        //var tbObjects = new TbObjects();
                        //tbObjects.AddFilter(tbObjects.flId, model.Id);
                        //
                        //var join = tbTrades
                        //    .Join(tbTrades.Name, tbObjects, tbObjects.Name, JoinType.Inner)
                        //    .On(new Condition(tbTrades.flObjectId, tbObjects.flId));
                        //
                        //var hasWaitTrades = join.Select(new FieldAlias[] { tbTrades.flId},ve.Env.QueryExecuter).Rows.Count > 0;
                        //
                        //if (hasWaitTrades) {
                        //    ve.Env.AddError(tbRev.flId.FieldName, ve.Env.T("Данный объект задействован в ожидающихся торгах."));
                        //}

                    })
                    .OnProcessing(pe => {
                        var tbRev = new TbReservoirsRevisions();
                        pe.Model.flStatus = ReservoirStatuses.Deleted.ToString();
                    })
                )
                .Step("Основные данные", step => step
                    .Enabled(env => env.Args.OrderType.In(ReservoirsOrderTypes.Create, ReservoirsOrderTypes.Edit))
                    .OnRendering(re => {
                        var tbRev = new TbReservoirsRevisions();
                        var model = re.Model;
                        int? objectId = null;
                        if (model.flId != 0) {
                            objectId = model.flId;
                        }

                        if (!string.IsNullOrEmpty(re.Args.OriginalArgs.Message))
                        {
                            re.Panel.AddComponent(new Label(re.Args.OriginalArgs.Message, "h6 alert-warning center"));
                        }

                        tbRev.flStatus.RenderCustom(re.Panel, re.Env, model.flStatus, readOnly: true);

                        tbRev.flName.RenderCustom(re.Panel, re.Env, model.flName);
                        tbRev.flDescription.RenderCustom(re.Panel, re.Env, model.flDescription);
                        tbRev.flNote.RenderCustom(re.Panel, re.Env, model.flNote);
                        tbRev.flArea.RenderCustom(re.Panel, re.Env, model.flArea);
                    })
                    .OnValidating(ve => {
                        var tbRev = new TbReservoirsRevisions();
                        var checkFields = new Field[] {
                            tbRev.flName,
                            tbRev.flDescription,
                            tbRev.flNote,
                            tbRev.flArea
                        };
                        checkFields.Each(f => f.Validate(ve.Env));

                    })
                    .OnProcessing(pe => {
                        var tbRev = new TbReservoirsRevisions();
                        pe.Model.setMainData(
                            tbRev.flName.GetVal(pe.Env),
                            tbRev.flDescription.GetVal(pe.Env),
                            tbRev.flNote.GetVal(pe.Env),
                            tbRev.flArea.GetVal(pe.Env)
                        );
                    })
                )
                .Step("Географическое расположение", step => step
                    .Enabled(env => env.Args.OrderType.In(ReservoirsOrderTypes.Create, ReservoirsOrderTypes.Edit))
                    .OnRendering(re => {
                        var tbRev = new TbReservoirsRevisions();
                        var model = re.Model;

                        tbRev.flCountry.RenderCustom(re.Panel, re.Env, model.flCountry);
                        tbRev.flRegion.RenderCustom(re.Panel, re.Env, model.flRegion);
                        tbRev.flDistrict.RenderCustom(re.Panel, re.Env, model.flDistrict);
                        tbRev.flLocation.RenderCustom(re.Panel, re.Env, model.flLocation);

                        if (re.Args.OrderType == ReservoirsOrderTypes.Edit || re.Args.OriginalArgs.MenuAction == "edit")
                        {
                            re.Panel.AddComponent(new Referencebox("flDraw", new RefDrawGeom(), "Геометрия", new[] { RefDrawGeom.Values.DONTDRAW }) { CssClass = " form-control " });
                        }
                    })
                    .OnValidating(ve => {
                        var tbRev = new TbReservoirsRevisions();
                        var checkFields = new Field[] {
                            tbRev.flCountry,
                            tbRev.flRegion,
                            tbRev.flDistrict,
                            tbRev.flLocation
                        };
                        checkFields.Each(f => f.Validate(ve.Env));

                        if (ve.Env.IsValid)
                        {
                            var tb = new TbReservoirs()
                                .AddFilter(t => t.flName, ve.Model.flName)
                                .AddFilterNot(t => t.flId, ve.Model.flId)
                                .AddFilter(t => t.flRegion, tbRev.flRegion.GetVal(ve.Env));

                            if (tb.Count(ve.Env.QueryExecuter) > 0)
                            {
                                ve.Env.AddError(tbRev.flRegion.FieldName, ve.Env.T("В выбранной области уже создан водоём с аналогичным названием (возможно это он и есть, в противном случае переименуйте создаваемомый водоём, например добавьте число в конце названия)"));
                            }
                        }

                        if (ve.Args.OrderType == ReservoirsOrderTypes.Edit || ve.Args.OriginalArgs.MenuAction == "edit")
                        {
                            if (new Referencebox("flDraw", new RefDrawGeom(), "Геометрия").GetPostedValues<string>(ve.Env.FormCollection).Length == 0)
                            {
                                ve.Env.AddError("flDraw", ve.Env.T("Поле \"Геометрия\" обзяательно для заполнения"));
                            }
                        }
                    })
                    .OnProcessing(pe => {
                        var tbRev = new TbReservoirsRevisions();

                        pe.Model.setGeoPosition(
                            tbRev.flCountry.GetVal(pe.Env),
                            tbRev.flRegion.GetVal(pe.Env),
                            tbRev.flDistrict.GetVal(pe.Env),
                            tbRev.flLocation.GetVal(pe.Env)
                        );

                        if (pe.Args.OrderType == ReservoirsOrderTypes.Edit || pe.Args.OriginalArgs.MenuAction == "edit")
                        {
                            if (new Referencebox("flDraw", new RefDrawGeom(), "Геометрия").GetPostedValues<string>(pe.Env.FormCollection).Contains(RefDrawGeom.Values.DRAW))
                            {
                                pe.Model.flWKT = null;
                                pe.Model.flCoords = null;
                            }
                        }
                    })
                )
                .Step("Геометрия", step => step
                    .Enabled(env => env.Args.OrderType.In(ReservoirsOrderTypes.Create, ReservoirsOrderTypes.Edit) && string.IsNullOrEmpty(env.Model.flWKT))
                    .OnRendering(re => {
                        var tbRev = new TbReservoirs();
                        var model = re.Model;


                        tbRev.AddFilter(t => t.flStatus, ReservoirStatuses.Active.ToString());
                        tbRev.AddFilterNot(t => t.flId, model.flId);
                        tbRev.AddFilterNotNull(t => t.flGeometry);
                        var wkts = tbRev.Select(new FieldAlias[] {
                            tbRev.flWKT
                        }, re.Env.QueryExecuter).AsEnumerable().Select(r => tbRev.flWKT.GetRowVal(r)).ToArray();

                        re.Panel.AddComponent(new Textbox("flWKT", cssClass: "d-none"));
                        re.Panel.AddComponent(new Textbox("flCoords", cssClass: "d-none"));

                        string oldver = null;
                        if (re.Args.OrderType == ReservoirsOrderTypes.Edit || re.Args.OriginalArgs.MenuAction == "edit")
                        {
                            oldver = new TbReservoirs().AddFilter(t => t.flId, model.flId).SelectScalar(t => t.flWKT, re.Env.QueryExecuter);
                        }

                        re.Panel.AddComponent(new ObjectDrawByCoordsComponent("flWKT", "flCoords", wkts, null, oldver));

                    })
                    .OnValidating(ve => {
                        var tbRev = new TbReservoirsRevisions();
                        var checkFields = new Field[] {
                            tbRev.flWKT,
                            tbRev.flCoords,
                        };
                        checkFields.Each(f => f.Validate(ve.Env));

                        if (ve.Env.IsValid)
                        {
                            if (string.IsNullOrEmpty(tbRev.flWKT.GetVal(ve.Env)) || tbRev.flWKT.GetVal(ve.Env) == "ERROR")
                            {
                                ve.Env.AddError(tbRev.flWKT.FieldName, ve.Env.T("Перед переходом к следующему шагу проверьте введённые координаты"));
                            }
                            else
                            {
                                var tbActRevs = new TbReservoirs();
                                tbActRevs.AddFilter(t => t.flStatus, ReservoirStatuses.Active.ToString());
                                tbActRevs.AddFilterNot(t => t.flId, ve.Model.flId);
                                tbActRevs.AddFilterNotNull(t => t.flGeometry);

                                var geometry = new GeomFromWKT(tbRev.flWKT.GetVal(ve.Env), 4326);

                                var contains = new GeomContains(new SqlExpVal(new FieldToSqlExp(tbActRevs.flGeometry)), new SqlExpVal(geometry));
                                var inresects = new GeomIntersects(new SqlExpVal(new FieldToSqlExp(tbActRevs.flGeometry)), new SqlExpVal(geometry));

                                var result = tbActRevs.Select(new FieldAlias[] {
                                new FieldAlias(contains, "flContains"),
                                new FieldAlias(inresects, "flIntersects"),
                            }, ve.Env.QueryExecuter).AsEnumerable();

                                if (result.Any(r => Convert.ToBoolean(r["flContains"]) || Convert.ToBoolean(r["flIntersects"])))
                                {
                                    ve.Env.AddError(tbRev.flWKT.FieldName, ve.Env.T("Нарисованная вами геометрия пересекается с существующими"));
                                }

                            }
                        }
                    })
                    .OnProcessing(pe => {
                        var tbRev = new TbReservoirsRevisions();

                        pe.Model.setGeometry(
                            tbRev.flWKT.GetVal(pe.Env),
                            tbRev.flCoords.GetVal(pe.Env)
                        );
                    })
                )
                ;
        }


        public override ReservoirModel GetModel(ActionEnv<ReservoirOrderQueryArgs> env, GetModelFrom from, ReservoirsOrderTypes orderType, ITransaction transaction) {
            var model = from switch {
                GetModelFrom.Empty => new ReservoirModel() {
                    flStatus = ReservoirStatuses.Active.ToString()
                },
                GetModelFrom.Entity =>
                    new TbReservoirs()
                        .AddFilter(t => t.flId, env.Args.Id.Value)
                        .GetReservoirModelFirst(env.QueryExecuter, transaction),
                GetModelFrom.Revision =>
                    new TbReservoirsRevisions()
                        .AddFilter(t => t.flRevisionId, env.Args.RevisionId.Value)
                        .GetReservoirModelFirst(env.QueryExecuter, transaction),
                _ => throw new NotImplementedException()
            };

            if (from == GetModelFrom.Empty) {
                switch (orderType) {
                    case ReservoirsOrderTypes.Create:
                        model.flStatus = ReservoirStatuses.Active.ToString();
                        break;
                }
            }
            if(from == GetModelFrom.Entity) {
                switch (orderType) {
                    case ReservoirsOrderTypes.Remove:
                        model.flStatus = ReservoirStatuses.Deleted.ToString();
                        break;
                }
            }

            return model;
        }

        public override ReservoirsOrderTypes GetOrderType(ActionEnv<ReservoirOrderQueryArgs> env, GetOrderModelFrom orderModelFrom) {
            return orderModelFrom switch {
                GetOrderModelFrom.Empty => ReservoirsOrderTypes.Create,
                _ => env.Args.OrderType switch {
                    ReservoirsOrderTypeActions.Remove => ReservoirsOrderTypes.Remove,
                    ReservoirsOrderTypeActions.Edit => ReservoirsOrderTypes.Edit,
                    _ => throw new ArgumentException($"OrderType: {env.Args.OrderType}")
                }
            };
        }

        public override string GetWizardCancelRedirectUrl(ActionEnv<ReservoirOrderQueryArgs> env, ReservoirsOrderTypes orderType, GetModelFrom modelFrom) {
            var urlHelper = env.RequestContext.GetUrlHelper();
            //TODO
            return modelFrom switch {
                GetModelFrom.Empty => urlHelper.YodaAction(ModuleName, nameof(MnuReservoirOrderBase)),
                GetModelFrom.Entity => urlHelper.YodaAction(ModuleName, nameof(MnuReservoirOrderBase), new ReservoirOrderQueryArgs { Id = env.Args.Id.Value, MenuAction = "view" }),
                GetModelFrom.Revision => urlHelper.YodaAction(ModuleName, MenuName, new ReservoirOrderQueryArgs { RevisionId = env.Args.RevisionId, MenuAction = Actions.ViewOrder }),
                _ => throw new NotImplementedException()
            };
        }

        public override ValidationResultBase IsCreateFromActionValid(ActionEnv<ReservoirOrderQueryArgs> env) {
            if (env.Args.Id == null) {
                return new RedirectResult(new ArgumentException("Id"));
            }

            if (env.Args.OrderType == null) {
                return new RedirectResult(new ArgumentException("OrderType"));
            }

            if (!CanCreateOrder(env.Args.Id.Value, GetOrderType(env, GetOrderModelFrom.Entity), out var error, env.QueryExecuter))
            {
                if (env.Args.Id.HasValue) {
                    return new RedirectResult(ModuleName, MnuReservoirView.MnuName, new ReservoirViewActionQueryArgs() { Id = env.Args.Id.Value, MenuAction = MnuReservoirView.Actions.View }, error);
                }
                else {
                    return new RedirectResult(ModuleName, "ActionError", null, error);
                }
            }

            return new OkResult();
        }

        public static bool TryGetCurrentInWorkOrderRevId(int id, out int revId, IQueryExecuter queryExecuter) {
            var ret = new TbReservoirOrderResult()
                .AddFilterNotFinishedOrders()
                .JoinT("tbOrders", new TbReservoirsRevisions().AddFilter(t => t.flId, id), "tbRevs")
                .On((t1, t2) => new Join(t1.flSubjectId, t2.flRevisionId))
                .SelectScalar(t => t.L.flSubjectId, queryExecuter);

            if (ret == null) {
                revId = 0;
                return false;
            }

            revId = ret.Value;
            return true;
        }

        public static bool CanCreateOrder(int id, ReservoirsOrderTypes orderType, out string error, IQueryExecuter queryExecuter) {
            if (TryGetCurrentInWorkOrderRevId(id, out _, queryExecuter)) {
                error = "Для данного объекта имеется не исполненый приказ";
                return false;
            }

            var tradeResourceObject = ReservoirHelper.GetReservoirModel(id, queryExecuter);

            bool checkFieldActive(out string err) {
                if (tradeResourceObject.flStatus != ReservoirStatuses.Active.ToString()) {
                    err = "Изменение разрешено только для активного объекта";
                    return false;
                }
                err = null;
                return true;
            }
            bool checkActive(out string err) {
                if (tradeResourceObject.flStatus == ReservoirStatuses.Deleted.ToString()) {
                    err = "Операция невозможна, так как объект не активен";
                    return false;
                }
                err = null;
                return true;
            }

            switch (orderType) {
                case ReservoirsOrderTypes.Create:
                    error = "Недопустимый тип приказа";
                    return false;
                case ReservoirsOrderTypes.Edit:
                    return checkFieldActive(out error) && checkActive(out error);
                case ReservoirsOrderTypes.Remove:
                    if (!checkFieldActive(out error)) {
                        return false;
                    }

                    if (tradeResourceObject.flStatus == ReservoirStatuses.Deleted.ToString()) {
                        error = "Удаление разрешено только при неактивном земельном участке";
                        return false;
                    }
                    error = null;
                    return true;
                default:
                    throw new NotImplementedException($"ReservoirsOrderTypes: {orderType}");
            }
        }

        public override void SaveModel(int revisionId, ReservoirModel model, ReservoirsOrderTypes orderType, ActionEnv<ReservoirOrderQueryArgs> env, ITransaction transaction, GetModelFrom modelFrom) {
            var tbObjRevs = new TbReservoirsRevisions();

            if (modelFrom == GetModelFrom.Empty) {
                model.flId = new TbReservoirs().flId.GetNextId(env.QueryExecuter, transaction);
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

        public static void SetModel<TObjectsTable>(DataModifingQueryProxy<TObjectsTable> updateOrInsert, ReservoirModel model) where TObjectsTable : TbReservoirsBase
        {
            ReservoirStatuses status = model.flStatus == ReservoirStatuses.Active.ToString() ? ReservoirStatuses.Active : ReservoirStatuses.Deleted;

            var geometry = new GeomFromWKT(model.flWKT, 4326);
            var point = new PointFromGeom(new SqlExpVal(geometry));

            updateOrInsert
                .SetT(t => t.flId, model.flId)
                .SetT(t => t.flRevisionId, model.flRevisionId)
                .Set(t => t.flStatus, model.flStatus)

                .Set(t => t.flName, model.flName)
                .Set(t => t.flDescription, model.flDescription)
                .Set(t => t.flNote, model.flNote)
                .Set(t => t.flArea, model.flArea)

                .Set(t => t.flCountry, model.flCountry)
                .Set(t => t.flRegion, model.flRegion)
                .Set(t => t.flDistrict, model.flDistrict)
                .Set(t => t.flLocation, model.flLocation)
                .Set(t => t.flWKT, model.flWKT)
                .SetT(t => t.flCoords, model.flCoords)
                .Set(t => t.flGeometry, geometry)
                .Set(t => t.flPoint,  point)

                ;
        }

        public override bool TryAcceptModel(int revisionId, ReservoirModel model, ReservoirsOrderTypes orderType, ActionEnv<ReservoirOrderQueryArgs> env, ITransaction transaction, out string error) {
            if (orderType == ReservoirsOrderTypes.Create) {

            }

            var updateOrInsert = orderType switch {
                ReservoirsOrderTypes.Create =>
                    new TbReservoirs()
                        .Insert(),
                _ =>
                    new TbReservoirs()
                        .AddFilter(t => t.flId, model.flId)
                        .Update()
            };

            SetModel(updateOrInsert, model);

            updateOrInsert.Execute(env.QueryExecuter, transaction);

            error = null;
            return true;
        }

        public override void ViewModel(RenderActionEnv<ReservoirOrderQueryArgs> env, ReservoirModel model, ReservoirsOrderTypes orderType, OrderStatus orderStatus) {
            env.RequestContext.Title = "Объект";
            viewModelMainData(env.Form, env, model);
            viewModelGeoPositionData(env.Form, env, model);
            viewModelGeometryData(env.Form, env, model);
        }

        public static void viewModelMainData(WidgetBase widget, FormEnvironment env, ReservoirModel model) {
            var tbRevs = new TbReservoirsRevisions();
            var groupbox = new Accordion(env.T("Основные данные объекта"));
            widget.AddComponent(groupbox);

            tbRevs.flId.RenderCustom(groupbox, env, model.flId.ToString() ?? "*Генерируется автоматически при исполнении приказа", readOnly: true);
            tbRevs.flRevisionId.RenderCustom(groupbox, env, model.flRevisionId, readOnly: true);
            tbRevs.flStatus.RenderCustom(groupbox, env, model.flStatus, readOnly: true);
            tbRevs.flName.RenderCustom(groupbox, env, model.flName, readOnly: true);
            tbRevs.flDescription.RenderCustom(groupbox, env, model.flDescription, readOnly: true);
            tbRevs.flNote.RenderCustom(groupbox, env, model.flNote, readOnly: true);
            tbRevs.flArea.RenderCustom(groupbox, env, model.flArea, readOnly: true);
        }

        public static void viewModelGeoPositionData(WidgetBase widget, FormEnvironment env, ReservoirModel model) {
            var tbRevs = new TbReservoirsRevisions();
            var groupbox = new Accordion(env.T("Географическое расположение"));
            widget.AddComponent(groupbox);

            tbRevs.flCountry.RenderCustom(groupbox, env, model.flCountry, readOnly: true);
            tbRevs.flRegion.RenderCustom(groupbox, env, model.flRegion, readOnly: true);
            tbRevs.flDistrict.RenderCustom(groupbox, env, model.flDistrict, readOnly: true);
            tbRevs.flLocation.RenderCustom(groupbox, env, model.flLocation, readOnly: true);
        }

        public static void viewModelGeometryData(WidgetBase widget, FormEnvironment env, ReservoirModel model)
        {
            var groupbox = new Accordion(env.T("Геометрия"));
            widget.AddComponent(groupbox);
            var row = new GridRow();
            groupbox.AddComponent(row);

            row.AddComponent(new ObjectGeometryViewerComponent(model.flWKT, "col-md-6"));
            row.AddComponent(new ObjectCoordsViewerComponent(model.flCoords, "col-md-6"));
        }

        protected override bool HasAccessToOrder(int revisionId, ActionEnv<ReservoirOrderQueryArgs> env) {
            if ((!env.User.IsExternalUser() && !env.User.IsGuest())) {
                return true;
            }
            if (env.Args.MenuAction != "view-order" && !env.User.HasRole("TRADERESOURCES-Рыбохозяйственные водоёмы-Создание приказов", env.QueryExecuter)/*!env.User.HasCustomRole("fishingobjects", "dataEdit", env.QueryExecuter)*/)
            {
                throw new AccessDeniedException();
            }
            var hasRecords = new TbReservoirsRevisions()
                .AddFilter(t => t.flRevisionId, revisionId)
                .Count(env.QueryExecuter) > 0;

            return hasRecords;
        }
    }
    
    public class ReservoirOrderQueryArgs : OrderQueryArgs {
        public int? Id { get; set; }
        public ReservoirsOrderTypeActions? OrderType { get; set; }
        public string Message { get; set; }
    }
    public enum ReservoirsOrderTypeActions {
        Create,
        Edit,
        Remove
    }
    public enum ReserviorFillSteps {
        InitObject, FillData
    }

    public class RefDrawGeom : Reference {
        public const string RefName = nameof(RefDrawGeom);
        public RefDrawGeom() : base(RefName, "Геометрия")
        {
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
