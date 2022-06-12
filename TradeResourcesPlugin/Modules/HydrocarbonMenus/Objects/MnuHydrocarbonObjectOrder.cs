using HydrocarbonSource.Helpers.Object;
using HydrocarbonSource.Models;
using HydrocarbonSource.QueryTables.Object;
using HydrocarbonSource.References.Object;
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
using YodaHelpers.SearchCollections;
using HydrocarbonSource.SearchCollections.Object;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using static HydrocarbonSource.QueryTables.Object.TbObjectsBase;

namespace TradeResourcesPlugin.Modules.HydrocarbonMenus.Objects {
    public class MnuHydrocarbonObjectOrder : MnuOrderBaseV2<HydrocarbonObjectModel, ObjectsOrderTypes, ObjectOrderQueryArgs> {

        public MnuHydrocarbonObjectOrder(string moduleName, string menuName, OrderStandartPermissions perms)
            : base(moduleName, menuName, "Приказ по охотничьим угодьям", perms, () => new TbObjectsOrderResult(), () => new TbObjectsOrderNegotiations())
        {
            AsCallback();
            Enabled(rc => {
                var xin = rc.User.GetUserXin(rc.QueryExecuter);
                if (xin == "050540004455"
                || xin == "050540000002"
                || (!rc.User.IsExternalUser() && !rc.User.IsGuest())
                || rc.User.HasRole("TRADERESOURCES-Недропользование-Создание приказов", rc.QueryExecuter)/*rc.User.HasPermission(nameof(RegistersModule), RegistersModule.LocalPermissions.Landlords)*/)
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

        public override IWizardFormBuilderWithStep<ObjectOrderQueryArgs, ModelOrderArgs<ObjectsOrderTypes, ObjectOrderQueryArgs>, HydrocarbonObjectModel>
            EditModel(IWizardFormBuilderWithFinishButton<ObjectOrderQueryArgs, ModelOrderArgs<ObjectsOrderTypes, ObjectOrderQueryArgs>, HydrocarbonObjectModel> wizard)
        {
            return wizard
                .Step("Удаление", step => step
                    .Enabled(env => env.Args.OrderType.In(ObjectsOrderTypes.Remove))
                    .OnRendering(re => {
                        var tbRev = new TbObjectsRevisions();
                        var model = re.Model;

                        tbRev.flId.RenderCustom(re.Panel, re.Env, model.flId, readOnly: true);
                        tbRev.flStatus.RenderCustom(re.Panel, re.Env, HydrocarbonObjectStatuses.Deleted.ToString(), cssClass: "hidden");
                        tbRev.flStatus.RenderCustom(re.Panel, re.Env, HydrocarbonObjectStatuses.Deleted.ToString(), readOnly: true);
                        tbRev.flBlock.RenderCustom(re.Panel, re.Env, null);
                    })
                    .OnValidating(ve => {
                    })
                    .OnProcessing(pe => {
                        var tbRev = new TbObjectsRevisions();
                        pe.Model.flStatus = HydrocarbonObjectStatuses.Deleted;
                        pe.Model.flBlock = Enum.Parse<HydrocarbonObjectBlocks>(tbRev.flBlock.GetVal(pe.Env));
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
                        tbRev.flNumber.RenderCustom(re.Panel, re.Env, model.flNumber);
                        tbRev.flDescription.RenderCustom(re.Panel, re.Env, model.flDescription);
                        tbRev.flNote.RenderCustom(re.Panel, re.Env, model.flNote);

                        tbRev.flRegion.RenderCustom(re.Panel, re.Env, model.flRegion);
                        tbRev.flArea.RenderCustom(re.Panel, re.Env, model.flArea);
                        tbRev.flBlocksCount.RenderCustom(re.Panel, re.Env, model.flBlocksCount);
                        tbRev.flExceptions.RenderCustom(re.Panel, re.Env, model.flExceptions);

                        re.Panel.Append(new HtmlText("<style>input#coordsExcel::-webkit-file-upload-button { display: none; }</style>"));
                        re.Panel.Append(new GridRow("form-group mb-3")
                            .Append(new FlexibleElement("label", "col-md-3 col-form-label")
                                .Append(new HtmlText("Таблица координат (Excel)"))
                            )
                            .Append(new GridCol("col-md-9")
                                .Append(new FileInput {
                                    Name = "coordsExcel",
                                    CssClass = "btn btn-light rounded form-control"
                                })
                            )
                        );
                        re.Panel.Append(new HtmlText(@"
<label>Пример:</label>
<table class=""table table-bordered table-centered text-center"">
<tbody>
  <tr>
    <td colspan=""6"">Координаты</td>
  </tr>
  <tr>
    <td colspan=""3"">Восточная долгота</td>
    <td colspan=""3"">Северная широта</td>
  </tr>
  <tr>
    <td>градусы</td>
    <td>минуты</td>
    <td>секунды</td>
    <td>градусы</td>
    <td>минуты</td>
    <td>секунды</td>
  </tr>
  <tr>
    <td>55</td>
    <td>57</td>
    <td>0</td>
    <td>50</td>
    <td>0</td>
    <td>0</td>
  </tr>
  <tr>
    <td>55</td>
    <td>57</td>
    <td>0</td>
    <td>50</td>
    <td>7</td>
    <td>0</td>
  </tr>
  <tr>
    <td>55</td>
    <td>52</td>
    <td>0</td>
    <td>50</td>
    <td>7</td>
    <td>0</td>
  </tr>
</tbody>
</table>
"));
                        tbRev.flAnotherInformation.RenderCustom(re.Panel, re.Env, model.flAnotherInformation);
                    })
                    .OnValidating(ve => {
                        var tbRev = new TbObjectsRevisions();
                        var checkFields = new Field[] {
                            tbRev.flName,
                            tbRev.flNumber,
                            tbRev.flDescription,
                            tbRev.flNote,
                            tbRev.flRegion,
                            tbRev.flArea,
                            tbRev.flBlocksCount,
                            tbRev.flExceptions,
                        };
                        checkFields.Each(f => f.Validate(ve.Env));

                        var files = ve.Env.RequestContext.ActionContext.HttpContext.Request.Form.Files;
                        if (files.Count == 0) {
                            ve.Env.AddError("coordsExcel", "Укажите файл таблицы координат");
                        } else {
                            if (files[0].ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") {
                                ve.Env.AddError("coordsExcel", "Укажите файл таблицы Excel");
                            } else {
                                try {
                                    getPolygonFromExcel(files[0]);
                                } catch {
                                    ve.Env.AddError("coordsExcel", "Неверный формат таблицы координат");
                                }
                            }
                        }
                    })
                    .OnProcessing(pe => {
                        var tbRev = new TbObjectsRevisions();

                        pe.Model.flName = tbRev.flName.GetVal(pe.Env);
                        pe.Model.flNumber = tbRev.flNumber.GetVal(pe.Env);
                        pe.Model.flDescription = tbRev.flDescription.GetVal(pe.Env);
                        pe.Model.flNote = tbRev.flNote.GetVal(pe.Env);
                        pe.Model.flRegion = tbRev.flRegion.GetVal(pe.Env);
                        pe.Model.flArea = tbRev.flArea.GetVal(pe.Env);
                        pe.Model.flBlocksCount = tbRev.flBlocksCount.GetVal(pe.Env);
                        pe.Model.flExceptions = tbRev.flExceptions.GetVal(pe.Env);
                        pe.Model.flAnotherInformation = tbRev.flAnotherInformation.GetVal(pe.Env);

                        var files = pe.Env.RequestContext.ActionContext.HttpContext.Request.Form.Files;
                        pe.Model.flCoords = getPolygonFromExcel(files[0]);

                        var polygonCoords = new List<Coords>();
                        polygonCoords.AddRange(pe.Model.flCoords.MainRing);
                        polygonCoords.Add(pe.Model.flCoords.MainRing[0]);
                        pe.Model.flWKT = $"POLYGON(({string.Join(",", polygonCoords.Select(coord => $"{coord.X:00.0000000000} {coord.Y:00.0000000000}".Replace(",", ".")))}))";
                    })
                )
                .Step("Предпросмотр геометрии", step => step
                    .Enabled(env => env.Args.OrderType.In(ObjectsOrderTypes.Create, ObjectsOrderTypes.Edit))
                    .OnRendering(re => {
                        re.Panel.Append(new ObjectGeometryViewerComponent(re.Model.flWKT));
                    })
                    .OnValidating(ve => {
                    })
                    .OnProcessing(pe => {
                    })
                )
                ;
        }

        private Polygon getPolygonFromExcel(IFormFile file) {
            var polygon = new Polygon();
            polygon.MainRing = new List<Coords>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var pck = new ExcelPackage()) {
                using (var stream = file.OpenReadStream()) {
                    pck.Load(stream);
                }
                var ws = pck.Workbook.Worksheets.First();

                var startRow = 4;
                for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++) {
                    var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                    string getElement(int index) {
                        return wsRow.ElementAt(index).Value.ToString();
                    }
                    polygon.MainRing.Add(new Coords {
                        AppropriateX = $"{Convert.ToInt32(getElement(0))}° {Convert.ToInt32(getElement(1))}′ {Convert.ToDecimal(getElement(2))}″",
                        AppropriateY = $"{Convert.ToInt32(getElement(3))}° {Convert.ToInt32(getElement(4))}′ {Convert.ToDecimal(getElement(5))}″",
                        X = Convert.ToDecimal(getElement(0)) + (1m / 60m * Convert.ToDecimal(getElement(1))) + ((1m / 60m) / 60m * Convert.ToDecimal(getElement(2))),
                        Y = Convert.ToDecimal(getElement(3)) + (1m / 60m * Convert.ToDecimal(getElement(4))) + ((1m / 60m) / 60m * Convert.ToDecimal(getElement(5)))
                    });
                }
            }
            return polygon;
        }

        public override HydrocarbonObjectModel GetModel(ActionEnv<ObjectOrderQueryArgs> env, GetModelFrom from, ObjectsOrderTypes orderType, ITransaction transaction)
        {
            var model = from switch
            {
                GetModelFrom.Empty => new HydrocarbonObjectModel()
                {
                    flSellerBin = env.User.GetUserBin(env.QueryExecuter),
                    flStatus = HydrocarbonObjectStatuses.Active,
                    flBlock = HydrocarbonObjectBlocks.ActiveFree
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
                        model.flStatus = HydrocarbonObjectStatuses.Active;
                        model.flBlock = HydrocarbonObjectBlocks.ActiveFree;
                        break;
                }
            }
            if (from == GetModelFrom.Entity)
            {
                switch (orderType)
                {
                    case ObjectsOrderTypes.Remove:
                        model.flStatus = HydrocarbonObjectStatuses.Deleted;
                        model.flBlock = HydrocarbonObjectBlocks.DeletedExcempted;
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
                GetModelFrom.Empty => urlHelper.YodaAction(ModuleName, nameof(MnuHydrocarbonObjectOrder)),
                GetModelFrom.Entity => urlHelper.YodaAction(ModuleName, nameof(MnuHydrocarbonObjectOrder), new ObjectOrderQueryArgs { Id = env.Args.Id.Value, MenuAction = "view" }),
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
                    return new RedirectResult(ModuleName, MnuHydrocarbonObjectView.MnuName, new HydrocarbonObjectViewArgs() { Id = env.Args.Id.Value, MenuAction = MnuHydrocarbonObjectView.Actions.View }, error);
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

            var tradeResourceObject = new TbObjects()
                .AddFilter(t => t.flId, id)
                .GetObjectModelFirst(env.QueryExecuter);

            bool checkFieldActive(out string err)
            {
                if (tradeResourceObject.flStatus != HydrocarbonObjectStatuses.Active)
                {
                    err = env.T("Изменение разрешено только для активного объекта");
                    return false;
                }
                err = null;
                return true;
            }
            bool checkActive(out string err)
            {
                if (tradeResourceObject.flStatus == HydrocarbonObjectStatuses.Deleted)
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

                    if (tradeResourceObject.flStatus == HydrocarbonObjectStatuses.Deleted)
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

        public override void SaveModel(int revisionId, HydrocarbonObjectModel model, ObjectsOrderTypes orderType, ActionEnv<ObjectOrderQueryArgs> env, ITransaction transaction, GetModelFrom modelFrom)
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

        public static void SetModel<TObjectsTable>(DataModifingQueryProxy<TObjectsTable> updateOrInsert, HydrocarbonObjectModel model) where TObjectsTable : TbObjectsBase
        {
            HydrocarbonObjectStatuses status = model.flStatus;

            var geometry = new GeomFromWKT(model.flWKT, 4326);
            var point = new PointFromGeom(new SqlExpVal(geometry));

            updateOrInsert
                .SetT(t => t.flId, model.flId)
                .SetT(t => t.flRevisionId, model.flRevisionId)

                .Set(t => t.flName, model.flName)
                .Set(t => t.flNumber, model.flNumber)
                .Set(t => t.flDescription, model.flDescription)
                .Set(t => t.flNote, model.flNote)

                .SetT(t => t.flSellerBin, model.flSellerBin)

                .Set(t => t.flStatus, model.flStatus.ToString())
                .Set(t => t.flBlock, model.flBlock.ToString())

                .Set(t => t.flRegion, model.flRegion)
                .Set(t => t.flArea, model.flArea)
                .Set(t => t.flBlocksCount, model.flBlocksCount)
                .Set(t => t.flExceptions, model.flExceptions)
                .Set(t => t.flWKT, model.flWKT)
                .SetT(t => t.flCoords, model.flCoords)
                .Set(t => t.flGeometry, geometry)
                .Set(t => t.flPoint, point)

                .Set(t => t.flAnotherInformation, model.flAnotherInformation)

                ;
        }

        public override bool TryAcceptModel(int revisionId, HydrocarbonObjectModel model, ObjectsOrderTypes orderType, ActionEnv<ObjectOrderQueryArgs> env, ITransaction transaction, out string error)
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

        public override void ViewModel(RenderActionEnv<ObjectOrderQueryArgs> env, HydrocarbonObjectModel model, ObjectsOrderTypes orderType, OrderStatus orderStatus)
        {
            ViewModel(env.Form, env, model);
        }
        public static void ViewModel(WidgetBase widget, FormEnvironment env, HydrocarbonObjectModel model)
        {
            env.RequestContext.Title = "Объект";
            viewModelMainData(widget, env, model);
        }

        public static void viewModelMainData(WidgetBase widget, FormEnvironment env, HydrocarbonObjectModel model, bool collapsed = false)
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

            tbRevs.flAnotherInformation.RenderCustom(groupbox, env, model.flAnotherInformation, readOnly: true);
        }

        protected override bool HasAccessToOrder(int revisionId, ActionEnv<ObjectOrderQueryArgs> env)
        {
            if ((!env.User.IsExternalUser() && !env.User.IsGuest()))
            {
                return true;
            }
            if (env.Args.MenuAction != "view-order" && !env.User.HasRole("TRADERESOURCES-Недропользование-Создание приказов", env.QueryExecuter)/*!env.User.HasPermission(nameof(RegistersModule), RegistersModule.LocalPermissions.Landlords)*/)
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
