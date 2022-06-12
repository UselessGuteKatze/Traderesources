using CommonSource;
using TelecomOperatorsSource.Helpers;
using TelecomOperatorsSource.Helpers.Object;
using TelecomOperatorsSource.Models;
using TelecomOperatorsSource.QueryTables.Object;
using TelecomOperatorsSource.References.Object;
using TelecomOperatorsSource.SearchCollections.Object;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
using FileStoreInterfaces;
using YodaApp.YodaHelpers.OrderHelpers;
using System.Threading.Tasks;

namespace TradeResourcesPlugin.Modules.TelecomOperatorsMenus.Objects {
    public class MnuTelecomOperatorsObjectOrder : MnuOrderBaseV2<TelecomOperatorsObjectModel, ObjectsOrderTypes, ObjectOrderQueryArgs> {

        public MnuTelecomOperatorsObjectOrder(string moduleName, OrderStandartPermissions perms)
            : base(moduleName, nameof(MnuTelecomOperatorsObjectOrder), "Приказ по объектам", perms, () => new TbObjectsOrderResult(), () => new TbObjectsOrderNegotiations())
        {
            AsCallback();
            Enabled(rc => {
                var xin = rc.User.GetUserXin(rc.QueryExecuter);
                if (xin == "050540004455"
                || xin == "050540000002"
                || (!rc.User.IsExternalUser() && !rc.User.IsGuest())
                || rc.User.HasRole("TRADERESOURCES-Ресурсы связи-Для операторов связи-Создание приказов", rc.QueryExecuter))
                {
                    return true;
                }
                return false;
            });
        }

        public override Task<ActionItem[]> GetActions(ActionEnv<ObjectOrderQueryArgs> env)
        {
            var ret = base.GetActions(env);

            switch (env.Args.MenuAction)
            {
                case Actions.ViewOrder:
                    break;
                case Actions.CreateFrom:
                    break;
            }

            return ret;
        }

        public override IWizardFormBuilderWithStep<ObjectOrderQueryArgs, ModelOrderArgs<ObjectsOrderTypes, ObjectOrderQueryArgs>, TelecomOperatorsObjectModel>
            EditModel(IWizardFormBuilderWithFinishButton<ObjectOrderQueryArgs, ModelOrderArgs<ObjectsOrderTypes, ObjectOrderQueryArgs>, TelecomOperatorsObjectModel> wizard)
        {
            return wizard
                .Step("Удаление", step => step
                    .Enabled(env => env.Args.OrderType.In(ObjectsOrderTypes.Remove))
                    .OnRendering(re => {
                        var tbRev = new TbObjectsRevisions();
                        var model = re.Model;

                        tbRev.flId.RenderCustom(re.Panel, re.Env, model.flId, readOnly: true);
                        tbRev.flStatus.RenderCustom(re.Panel, re.Env, TelecomOperatorsObjectStatuses.Deleted.ToString(), cssClass: "hidden");
                        tbRev.flStatus.RenderCustom(re.Panel, re.Env, TelecomOperatorsObjectStatuses.Deleted.ToString(), readOnly: true);
                        tbRev.flBlock.RenderCustom(re.Panel, re.Env, null);
                    })
                    .OnValidating(ve => {
                    })
                    .OnProcessing(pe => {
                        var tbRev = new TbObjectsRevisions();
                        pe.Model.flStatus = TelecomOperatorsObjectStatuses.Deleted;
                        pe.Model.flBlock = Enum.Parse< TelecomOperatorsObjectBlocks>(tbRev.flBlock.GetVal(pe.Env));
                    })
                )
                .Step("Основные данные", step => step
                    .Enabled(env => env.Args.OrderType.In(ObjectsOrderTypes.Create, ObjectsOrderTypes.Edit))
                    .OnRendering(re => {
                        var tbRev = new TbObjectsRevisions();
                        var model = re.Model;
                        int? objectId = null;
                        if (model.flId != 0)
                        {
                            objectId = model.flId;
                        }

                        tbRev.flSellerBin.RenderCustom(re.Panel, re.Env, model.flSellerBin, readOnly: true);
                        tbRev.flStatus.RenderCustom(re.Panel, re.Env, model.flStatus.ToString(), readOnly: true);
                        tbRev.flBlock.RenderCustom(re.Panel, re.Env, model.flBlock.ToString(), readOnly: true);

                        tbRev.flName.RenderCustom(re.Panel, re.Env, model.flName);
                        tbRev.flDescription.RenderCustom(re.Panel, re.Env, model.flDescription);
                        tbRev.flNote.RenderCustom(re.Panel, re.Env, model.flNote);

                        tbRev.flDocs.RenderCustomT(re.Panel, re.Env, JsonConvert.SerializeObject(re.Model.flDocs ?? new FileInfo[] { }));

                        tbRev.flAnotherInformation.RenderCustom(re.Panel, re.Env, model.flAnotherInformation);
                    })
                    .OnValidating(ve => {
                        var tbRev = new TbObjectsRevisions();
                        var checkFields = new Field[] {
                            tbRev.flName,
                            tbRev.flDescription,
                            tbRev.flNote,
                            tbRev.flDocs
                        };
                        checkFields.Each(f => f.Validate(ve.Env));

                    })
                    .OnProcessing(pe => {
                        var tbRev = new TbObjectsRevisions();
                        pe.Model.flName = tbRev.flName.GetVal(pe.Env);
                        pe.Model.flDescription = tbRev.flDescription.GetVal(pe.Env);
                        pe.Model.flNote = tbRev.flNote.GetVal(pe.Env);
                        pe.Model.flDocs = tbRev.flDocs.GetGoodValue(pe.Env);
                        pe.Model.flAnotherInformation = tbRev.flAnotherInformation.GetVal(pe.Env);
                    })
                )
                .Step("Географическое расположение", step => step
                    .Enabled(env => env.Args.OrderType.In(ObjectsOrderTypes.Create, ObjectsOrderTypes.Edit))
                    .OnRendering(re => {
                        var tbRev = new TbObjectsRevisions();
                        var model = re.Model;

                        tbRev.flKfsLevel1.RenderCustom(re.Panel, re.Env, model.flKfsLevel1);
                        tbRev.flKfsLevel2.RenderCustom(re.Panel, re.Env, model.flKfsLevel2);
                        tbRev.flKfsLevel3.RenderCustom(re.Panel, re.Env, model.flKfsLevel3);
                        tbRev.flKfsLevel4.RenderCustom(re.Panel, re.Env, model.flKfsLevel4);
                        tbRev.flKfsLevel5.RenderCustom(re.Panel, re.Env, model.flKfsLevel5);

                        tbRev.flCountry.RenderCustom(re.Panel, re.Env, model.flCountry);
                        tbRev.flRegion.RenderCustom(re.Panel, re.Env, model.flRegion);
                        tbRev.flDistrict.RenderCustom(re.Panel, re.Env, model.flDistrict);
                        tbRev.flAddress.RenderCustom(re.Panel, re.Env, model.flAddress);

                        if (re.Args.OrderType == ObjectsOrderTypes.Edit || re.Args.OriginalArgs.MenuAction == "edit")
                        {
                            re.Panel.AddComponent(new Referencebox("flDraw", new RefDrawGeom(), "Геометрия") { CssClass = " form-control " });
                        }
                    })
                    .OnValidating(ve => {
                        var tbRev = new TbObjectsRevisions();
                        var checkFields = new Field[] {
                            tbRev.flKfsLevel1,
                            tbRev.flKfsLevel2,
                            tbRev.flKfsLevel3,
                            tbRev.flKfsLevel4,
                            tbRev.flKfsLevel5,

                            tbRev.flCountry,
                            tbRev.flRegion,
                            tbRev.flDistrict,
                            tbRev.flAddress,
                        };
                        checkFields.Each(f => f.Validate(ve.Env));

                        if (ve.Args.OrderType == ObjectsOrderTypes.Edit || ve.Args.OriginalArgs.MenuAction == "edit")
                        {
                            if (new Referencebox("flDraw", new RefDrawGeom(), "Геометрия").GetPostedValues<string>(ve.Env.FormCollection).Length == 0)
                            {
                                ve.Env.AddError("flDraw", ve.Env.T("Поле \"Геометрия\" обзяательно для заполнения"));
                            }
                        }
                    })
                    .OnProcessing(pe => {
                        var tbRev = new TbObjectsRevisions();

                        pe.Model.flKfsLevel1 = tbRev.flKfsLevel1.GetVal(pe.Env);
                        pe.Model.flKfsLevel2 = tbRev.flKfsLevel2.GetVal(pe.Env);
                        pe.Model.flKfsLevel3 = tbRev.flKfsLevel3.GetVal(pe.Env);
                        pe.Model.flKfsLevel4 = tbRev.flKfsLevel4.GetVal(pe.Env);
                        pe.Model.flKfsLevel5 = tbRev.flKfsLevel5.GetVal(pe.Env);

                        pe.Model.flCountry = tbRev.flCountry.GetVal(pe.Env);
                        pe.Model.flRegion = tbRev.flRegion.GetVal(pe.Env);
                        pe.Model.flDistrict = tbRev.flDistrict.GetVal(pe.Env);
                        pe.Model.flAddress = tbRev.flAddress.GetVal(pe.Env);

                        if (pe.Args.OrderType == ObjectsOrderTypes.Edit || pe.Args.OriginalArgs.MenuAction == "edit")
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
                    .Enabled(env => env.Args.OrderType.In(ObjectsOrderTypes.Create, ObjectsOrderTypes.Edit) && string.IsNullOrEmpty(env.Model.flWKT))
                    .OnRendering(re => {
                        var tbRev = new TbObjects();
                        var model = re.Model;

                        tbRev.AddFilter(t => t.flSellerBin, model.flSellerBin);
                        tbRev.AddFilter(t => t.flStatus, TelecomOperatorsObjectStatuses.Active.ToString());
                        tbRev.AddFilterNot(t => t.flId, model.flId);
                        var wkts = tbRev.Select(new FieldAlias[] {
                            tbRev.flWKT
                        }, re.Env.QueryExecuter).AsEnumerable().Select(r => tbRev.flWKT.GetRowVal(r)).ToArray();

                        re.Panel.AddComponent(new Textbox("flWKT", cssClass: "d-none"));
                        re.Panel.AddComponent(new Textbox("flCoords", cssClass: "d-none"));


                        string oldver = null;
                        if (re.Args.OrderType == ObjectsOrderTypes.Edit || re.Args.OriginalArgs.MenuAction == "edit")
                        {
                            oldver = new TbObjects().AddFilter(t => t.flId, model.flId).SelectScalar(t => t.flWKT, re.Env.QueryExecuter);
                        }

                        re.Panel.AddComponent(new ObjectDrawByCoordsComponent("flWKT", "flCoords", wkts, null, oldver));

                    })
                    .OnValidating(ve => {
                        var tbRev = new TbObjectsRevisions();
                        var model = ve.Model;
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
                                var tbActRevs = new TbObjects();
                                tbActRevs.AddFilter(t => t.flSellerBin, model.flSellerBin);
                                tbActRevs.AddFilterNot(t => t.flId, ve.Model.flId);
                                tbActRevs.AddFilter(t => t.flStatus, TelecomOperatorsObjectStatuses.Active.ToString());
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
                        var tbRev = new TbObjectsRevisions();
                        pe.Model.flWKT = tbRev.flWKT.GetVal(pe.Env);
                        pe.Model.flCoords = tbRev.flCoords.GetVal(pe.Env);
                    })
                )
                ;
        }


        public override TelecomOperatorsObjectModel GetModel(ActionEnv<ObjectOrderQueryArgs> env, GetModelFrom from, ObjectsOrderTypes orderType, ITransaction transaction)
        {
            var model = from switch
            {
                GetModelFrom.Empty => new TelecomOperatorsObjectModel()
                {
                    flSellerBin = env.User.GetUserBin(env.QueryExecuter),
                    flStatus = TelecomOperatorsObjectStatuses.Active,
                    flBlock = TelecomOperatorsObjectBlocks.ActiveFree
                },
                GetModelFrom.Entity =>
                    new TbObjects()
                        .AddFilter(t => t.flId, env.Args.Id.Value)
                        .GetObjectModelFirst(env.QueryExecuter, transaction),
                GetModelFrom.Revision =>
                    new TbObjectsRevisions()
                        .AddFilter(t => t.flRevisionId, env.Args.RevisionId.Value)
                        .GetObjectModelFirst(env.QueryExecuter, transaction),
                _ => throw new NotImplementedException()
            };

            if (from == GetModelFrom.Empty)
            {
                switch (orderType)
                {
                    case ObjectsOrderTypes.Create:
                        model.flStatus = TelecomOperatorsObjectStatuses.Active;
                        model.flBlock = TelecomOperatorsObjectBlocks.ActiveFree;
                        break;
                }
            }
            if (from == GetModelFrom.Entity)
            {
                switch (orderType)
                {
                    case ObjectsOrderTypes.Remove:
                        model.flStatus = TelecomOperatorsObjectStatuses.Deleted;
                        model.flBlock = TelecomOperatorsObjectBlocks.DeletedDisqualified;
                        break;
                }
            }

            return model;
        }

        public override ObjectsOrderTypes GetOrderType(ActionEnv<ObjectOrderQueryArgs> env, GetOrderModelFrom orderModelFrom)
        {
            return orderModelFrom switch
            {
                GetOrderModelFrom.Empty => ObjectsOrderTypes.Create,
                _ => env.Args.OrderType switch
                {
                    ObjectsOrderTypeActions.Remove => ObjectsOrderTypes.Remove,
                    ObjectsOrderTypeActions.Edit => ObjectsOrderTypes.Edit,
                    _ => throw new ArgumentException($"OrderType: {env.Args.OrderType}")
                }
            };
        }

        public override string GetWizardCancelRedirectUrl(ActionEnv<ObjectOrderQueryArgs> env, ObjectsOrderTypes orderType, GetModelFrom modelFrom)
        {
            var urlHelper = env.RequestContext.GetUrlHelper();
            //TODO
            return modelFrom switch
            {
                GetModelFrom.Empty => urlHelper.YodaAction(ModuleName, nameof(MnuTelecomOperatorsObjectOrder)),
                GetModelFrom.Entity => urlHelper.YodaAction(ModuleName, nameof(MnuTelecomOperatorsObjectOrder), new ObjectOrderQueryArgs { Id = env.Args.Id.Value, MenuAction = "view" }),
                GetModelFrom.Revision => urlHelper.YodaAction(ModuleName, MenuName, new ObjectOrderQueryArgs { RevisionId = env.Args.RevisionId, MenuAction = Actions.ViewOrder }),
                _ => throw new NotImplementedException()
            };
        }

        public override ValidationResultBase IsCreateFromActionValid(ActionEnv<ObjectOrderQueryArgs> env) {
            if (env.Args.Id == null) {
                return new RedirectResult(new ArgumentException("Id"));
            }

            if (env.Args.OrderType == null) {
                return new RedirectResult(new ArgumentException("OrderType"));
            }

            if (!CanCreateOrder(env.Args.Id.Value, GetOrderType(env, GetOrderModelFrom.Entity), out var error, env)) {
                if (env.Args.Id.HasValue) {
                    return new RedirectResult(ModuleName, MnuTelecomOperatorsObjectView.MnuName, new TelecomOperatorsObjectViewArgs() { Id = env.Args.Id.Value, MenuAction = MnuTelecomOperatorsObjectView.Actions.View }, error);
                }
                else {
                    return new RedirectResult(ModuleName, "ActionError", null, error);
                }
            }

            return new OkResult();
        }

        public static bool TryGetCurrentInWorkOrderRevId(int id, out int revId, IQueryExecuter queryExecuter)
        {
            var ret = new TbObjectsOrderResult()
                .AddFilterNotFinishedOrders()
                .JoinT("tbOrders", new TbObjectsRevisions().AddFilter(t => t.flId, id), "tbRevs")
                .On((t1, t2) => new Join(t1.flSubjectId, t2.flRevisionId))
                .SelectScalar(t => t.L.flSubjectId, queryExecuter);

            if (ret == null)
            {
                revId = 0;
                return false;
            }

            revId = ret.Value;
            return true;
        }

        public static bool CanCreateOrder(int id, ObjectsOrderTypes orderType, out string error, ActionEnv<ObjectOrderQueryArgs> env)
        {
            if (TryGetCurrentInWorkOrderRevId(id, out _, env.QueryExecuter))
            {
                error = env.T("Для данного объекта имеется не исполненый приказ");
                return false;
            }

            var tradeResourceObject = new TbObjects().AddFilter(t => t.flId, id).GetObjectModelFirst(env.QueryExecuter);

            bool checkFieldActive(out string err)
            {
                if (tradeResourceObject.flStatus != TelecomOperatorsObjectStatuses.Active)
                {
                    err = env.T("Изменение разрешено только для активного объекта");
                    return false;
                }
                err = null;
                return true;
            }
            bool checkActive(out string err)
            {
                if (tradeResourceObject.flStatus == TelecomOperatorsObjectStatuses.Deleted)
                {
                    err = env.T("Операция невозможна, так как объект не активен");
                    return false;
                }
                err = null;
                return true;
            }

            switch (orderType)
            {
                case ObjectsOrderTypes.Create:
                    error = env.T("Недопустимый тип приказа");
                    return false;
                case ObjectsOrderTypes.Edit:
                    return checkFieldActive(out error) && checkActive(out error);
                case ObjectsOrderTypes.Remove:
                    if (!checkFieldActive(out error))
                    {
                        return false;
                    }

                    if (tradeResourceObject.flStatus == TelecomOperatorsObjectStatuses.Deleted)
                    {
                        error = env.T("Удаление разрешено только при неактивном земельном участке");
                        return false;
                    }
                    error = null;
                    return true;
                default:
                    throw new NotImplementedException($"ObjectsOrderTypes: {orderType}");
            }
        }

        public override void SaveModel(int revisionId, TelecomOperatorsObjectModel model, ObjectsOrderTypes orderType, ActionEnv<ObjectOrderQueryArgs> env, ITransaction transaction, GetModelFrom modelFrom)
        {
            var tbObjRevs = new TbObjectsRevisions();

            if (modelFrom == GetModelFrom.Empty)
            {
                model.flId = new TbObjects().flId.GetNextId(env.QueryExecuter, transaction);
            }

            var updateOrInsert = modelFrom switch
            {
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

        public static void SetModel<TObjectsTable>(DataModifingQueryProxy<TObjectsTable> updateOrInsert, TelecomOperatorsObjectModel model) where TObjectsTable : TbObjectsBase
        {
            TelecomOperatorsObjectStatuses status = model.flStatus;

            var geometry = new GeomFromWKT(model.flWKT, 4326);
            var point = new PointFromGeom(new SqlExpVal(geometry));

            updateOrInsert
                .SetT(t => t.flId, model.flId)
                .SetT(t => t.flRevisionId, model.flRevisionId)

                .SetT(t => t.flSellerBin, model.flSellerBin)
                .Set(t => t.flStatus, model.flStatus.ToString())
                .Set(t => t.flBlock, model.flBlock.ToString())

                .Set(t => t.flName, model.flName)
                .Set(t => t.flDescription, model.flDescription)
                .Set(t => t.flNote, model.flNote)
                .Set(t => t.flAnotherInformation, model.flAnotherInformation)
                .SetT(t => t.flDocs, JsonConvert.SerializeObject(model.flDocs))

                .Set(t => t.flCountry, model.flCountry)
                .Set(t => t.flRegion, model.flRegion)
                .Set(t => t.flDistrict, model.flDistrict)
                .Set(t => t.flAddress, model.flAddress)
                .Set(t => t.flWKT, model.flWKT)
                .SetT(t => t.flCoords, model.flCoords)
                .Set(t => t.flGeometry, geometry)
                .Set(t => t.flPoint, point)

                .Set(t => t.flKfsLevel1, model.flKfsLevel1)
                .Set(t => t.flKfsLevel2, model.flKfsLevel2)
                .Set(t => t.flKfsLevel3, model.flKfsLevel3)
                .Set(t => t.flKfsLevel4, model.flKfsLevel4)
                .Set(t => t.flKfsLevel5, model.flKfsLevel5)

                ;
        }

        public override bool TryAcceptModel(int revisionId, TelecomOperatorsObjectModel model, ObjectsOrderTypes orderType, ActionEnv<ObjectOrderQueryArgs> env, ITransaction transaction, out string error)
        {
            if (orderType == ObjectsOrderTypes.Create)
            {

            }

            var updateOrInsert = orderType switch
            {
                ObjectsOrderTypes.Create =>
                    new TbObjects()
                        .Insert(),
                _ =>
                    new TbObjects()
                        .AddFilter(t => t.flId, model.flId)
                        .Update()
            };

            SetModel(updateOrInsert, model);

            updateOrInsert.Execute(env.QueryExecuter, transaction);

            error = null;
            return true;
        }

        public override void ViewModel(RenderActionEnv<ObjectOrderQueryArgs> env, TelecomOperatorsObjectModel model, ObjectsOrderTypes orderType, OrderStatus orderStatus)
        {
            ViewModel(env.Form, env, model);
        }
        public static void ViewModel(WidgetBase widget, FormEnvironment env, TelecomOperatorsObjectModel model)
        {
            env.RequestContext.Title = "Объект";
            viewModelMainData(widget, env, model);
            viewModelGeoPositionData(widget, env, model);
            viewModelGeometryData(widget, env, model);
        }

        public static void viewModelMainData(WidgetBase widget, FormEnvironment env, TelecomOperatorsObjectModel model, bool collapsed = false)
        {
            var tbRevs = new TbObjectsRevisions();
            var groupbox = new Accordion(env.T("Основные данные объекта"), collapsed);
            widget.AddComponent(groupbox);

            tbRevs.flId.RenderCustom(groupbox, env, model.flId.ToString() ?? env.T("*Генерируется автоматически при исполнении приказа"), readOnly: true);
            tbRevs.flRevisionId.RenderCustom(groupbox, env, model.flRevisionId, readOnly: true);
            tbRevs.flSellerBin.RenderCustom(groupbox, env, model.flSellerBin, readOnly: true);
            tbRevs.flStatus.RenderCustom(groupbox, env, model.flStatus.ToString(), readOnly: true);
            tbRevs.flBlock.RenderCustom(groupbox, env, model.flBlock.ToString(), readOnly: true);
            tbRevs.flName.RenderCustom(groupbox, env, model.flName, readOnly: true);
            tbRevs.flDescription.RenderCustom(groupbox, env, model.flDescription, readOnly: true);
            tbRevs.flNote.RenderCustom(groupbox, env, model.flNote, readOnly: true);

            tbRevs.flDocs.RenderCustom(groupbox, env, JsonConvert.SerializeObject(model.flDocs ?? new FileInfo[] { }), readOnly: true);
        }

        public static void viewModelGeoPositionData(WidgetBase widget, FormEnvironment env, TelecomOperatorsObjectModel model, bool collapsed = false)
        {
            var tbRevs = new TbObjectsRevisions();
            var groupbox = new Accordion(env.T("Географическое расположение"), collapsed);
            widget.AddComponent(groupbox);

            tbRevs.flCountry.RenderCustom(groupbox, env, model.flCountry, readOnly: true);
            tbRevs.flRegion.RenderCustom(groupbox, env, model.flRegion, readOnly: true);
            tbRevs.flDistrict.RenderCustom(groupbox, env, model.flDistrict, readOnly: true);
            tbRevs.flAddress.RenderCustom(groupbox, env, model.flAddress, readOnly: true);
        }

        public static void viewModelGeometryData(WidgetBase widget, FormEnvironment env, TelecomOperatorsObjectModel model, bool collapsed = false)
        {
            var groupbox = new Accordion(env.T("Геометрия"), collapsed);
            widget.AddComponent(groupbox);
            var row = new GridRow();
            groupbox.AddComponent(row);

            row.AddComponent(new ObjectGeometryViewerComponent(model.flWKT, "col-md-6"));
            row.AddComponent(new ObjectCoordsViewerComponent(model.flCoords, "col-md-6"));
        }

        protected override bool HasAccessToOrder(int revisionId, ActionEnv<ObjectOrderQueryArgs> env)
        {
            if ((!env.User.IsExternalUser() && !env.User.IsGuest()))
            {
                return true;
            }
            if (env.Args.MenuAction != "view-order" && !env.User.HasRole("TRADERESOURCES-Ресурсы связи-Для операторов связи-Создание приказов", env.QueryExecuter))
            {
                throw new AccessDeniedException();
            }
            var hasRecords = new TbObjectsRevisions()
                .AddFilter(t => t.flRevisionId, revisionId)
                .AddFilter(t => t.flSellerBin, env.User.GetUserBin(env.QueryExecuter))
                .Count(env.QueryExecuter) > 0;

            return hasRecords;
        }
    }

    public class ObjectOrderQueryArgs : OrderQueryArgs {
        public int? Id { get; set; }
        public ObjectsOrderTypeActions? OrderType { get; set; }
    }
    public enum ObjectsOrderTypeActions {
        Create,
        Edit,
        Remove
    }
    public enum ObjectFillSteps {
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
