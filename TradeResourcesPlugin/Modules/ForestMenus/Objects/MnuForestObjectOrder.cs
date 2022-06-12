using ForestSource.Helpers.Object;
using ForestSource.Models;
using ForestSource.QueryTables.Object;
using ForestSource.References.Object;
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
using ForestSource.SearchCollections.Object;
using Yoda.Interfaces.Helpers;

namespace TradeResourcesPlugin.Modules.ForestMenus.Objects {
    public class MnuForestObjectOrder : MnuOrderBaseV2<ForestObjectModel, ObjectsOrderTypes, ObjectOrderQueryArgs> {

        public MnuForestObjectOrder(string moduleName, string menuName, OrderStandartPermissions perms)
            : base(moduleName, menuName, "Приказ по охотничьим угодьям", perms, () => new TbObjectsOrderResult(), () => new TbObjectsOrderNegotiations())
        {
            AsCallback();
            Enabled(rc => {
                var xin = rc.User.GetUserXin(rc.QueryExecuter);
                if (xin == "050540004455"
                || xin == "050540000002"
                || (!rc.User.IsExternalUser() && !rc.User.IsGuest())
                || rc.User.HasRole("TRADERESOURCES-Лесные ресурсы-Выставление на торги", rc.QueryExecuter)/*rc.User.HasCustomRole("forestobjects", "dataEdit", rc.QueryExecuter)*/)
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

        public override IWizardFormBuilderWithStep<ObjectOrderQueryArgs, ModelOrderArgs<ObjectsOrderTypes, ObjectOrderQueryArgs>, ForestObjectModel>
            EditModel(IWizardFormBuilderWithFinishButton<ObjectOrderQueryArgs, ModelOrderArgs<ObjectsOrderTypes, ObjectOrderQueryArgs>, ForestObjectModel> wizard)
        {
            return wizard
                .Step("Удаление", step => step
                    .Enabled(env => env.Args.OrderType.In(ObjectsOrderTypes.Remove))
                    .OnRendering(re => {
                        var tbRev = new TbObjectsRevisions();
                        var model = re.Model;

                        tbRev.flId.RenderCustom(re.Panel, re.Env, model.flId, readOnly: true);
                        tbRev.flStatus.RenderCustom(re.Panel, re.Env, ForestObjectStatuses.Deleted.ToString(), cssClass: "hidden");
                        tbRev.flStatus.RenderCustom(re.Panel, re.Env, ForestObjectStatuses.Deleted.ToString(), readOnly: true);
                        tbRev.flBlock.RenderCustom(re.Panel, re.Env, null);
                    })
                    .OnValidating(ve => {
                    })
                    .OnProcessing(pe => {
                        var tbRev = new TbObjectsRevisions();
                        pe.Model.flStatus = ForestObjectStatuses.Deleted;
                        pe.Model.flBlock = Enum.Parse<ForestObjectBlocks>(tbRev.flBlock.GetVal(pe.Env));
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

                        tbRev.flForestryPieces.MetaData.Add(TypedSearchCollectionItemFieldEditorHelpers.CollectionQuerySettingsKey,
                            JsonConvert.SerializeObject(
                                model.flForestryPieces == null
                                ?
                                new ForestryPiecesSearchCollection.Settings() {
                                    OnlyActive = true
                                }
                                :
                                new ForestryPiecesSearchCollection.Settings() {
                                    OnlyActiveButWithIds = model.flForestryPieces.Select(x => Convert.ToInt32(x.SearchItemId)).ToArray()
                                }
                            )
                        );
                        tbRev.flForestryPieces.RenderCustomT(re.Panel, re.Env, (model.flForestryPieces ?? new ForestryPieceModel[] { }));

                        tbRev.flName.RenderCustom(re.Panel, re.Env, model.flName);
                        tbRev.flDescription.RenderCustom(re.Panel, re.Env, model.flDescription);
                        tbRev.flNote.RenderCustom(re.Panel, re.Env, model.flNote);

                        tbRev.flForestUsingType.RenderCustom(re.Panel, re.Env, model.flForestUsingType);
                        tbRev.flForestUsingPeriod.RenderCustom(re.Panel, re.Env, model.flForestUsingPeriod);
                        tbRev.flYearPay.RenderCustom(re.Panel, re.Env, model.flYearPay);
                        tbRev.flYearReproduction.RenderCustom(re.Panel, re.Env, model.flYearReproduction);

                        //tbRev.flDocs.RenderCustom(re.Panel, re.Env, JsonConvert.SerializeObject(model.flDocs ?? new FileInfo[] { }), description: new Label(re.Env.T("Требуются файлы следующего содержания") + ": " + string.Join(", ", new[] {
                        //    "Населенные пункты (таблица и описание)",
                        //    "Ресурсные данные",
                        //    "Размер платы на рубки главного пользования",
                        //    "Таблица: Разрешенные объемы лесопользования по рубкам главного пользования",
                        //    "Таблица: Разрешенные объемы ежегодного лесопользования по рубкам промежуточного пользования",
                        //    "Размер платы на рубки ухода",
                        //    "План проектируемых лесохозяйственных мероприятий",
                        //})));

                        var needFileCounter = 0;
                        new[] {
                            "Населенные пункты (таблица и описание)",
                            "Ресурсные данные",
                            "Размер платы на рубки главного пользования",
                            "Таблица: Разрешенные объемы лесопользования по рубкам главного пользования",
                            "Таблица: Разрешенные объемы ежегодного лесопользования по рубкам промежуточного пользования",
                            "Размер платы на рубки ухода",
                            "План проектируемых лесохозяйственных мероприятий",
                        }.Each(needFile => {
                            var field = new FilesField($"flDoc{++needFileCounter}", needFile, "ForestObjectDocuments", new FileUploadSettings() { AllowedExtensions = new string[] { "pdf" } });
                            var table = new QueryTable() { Name = $"flDocTable{needFileCounter}", Fields = new Field[] { field } };
                            table.Fields[0].RenderCustom(re.Panel, re.Env, JsonConvert.SerializeObject(model.flDocs?.Where(x => x.Description.Contains(needFile)) ?? new FileInfo[] { }));
                        });

                        tbRev.flAnotherInformation.RenderCustom(re.Panel, re.Env, model.flAnotherInformation);
                    })
                    .OnValidating(ve => {
                        var tbRev = new TbObjectsRevisions();
                        var checkFields = new Field[] {
                            tbRev.flForestryPieces,

                            tbRev.flName,
                            tbRev.flDescription,
                            tbRev.flNote,

                            tbRev.flForestUsingType,
                            tbRev.flForestUsingPeriod,
                            tbRev.flYearPay,
                            tbRev.flYearReproduction
                        };
                        checkFields.Each(f => f.Validate(ve.Env));

                        var needFileCounter = 0;
                        new[] {
                            "Населенные пункты (таблица и описание)",
                            "Ресурсные данные",
                            "Размер платы на рубки главного пользования",
                            "Таблица: Разрешенные объемы лесопользования по рубкам главного пользования",
                            "Таблица: Разрешенные объемы ежегодного лесопользования по рубкам промежуточного пользования",
                            "Размер платы на рубки ухода",
                            "План проектируемых лесохозяйственных мероприятий",
                        }.Each(needFile => {
                            var value = new FilesField($"flDoc{++needFileCounter}", needFile, "ForestObjectDocuments", new FileUploadSettings() { AllowedExtensions = new string[] { "pdf" } }).GetGoodValue(ve.Env);
                            if (value == null || value.Length == 0) {
                                ve.Env.AddError($"flDoc{needFileCounter}", ve.Env.T("Поле") + " \"" + ve.Env.T(needFile) + "\" " + ve.Env.T("обязательно для заполнения"));
                            }
                        });

                        //new[] {
                        //    "Населенные пункты (таблица и описание)",
                        //    "Ресурсные данные",
                        //    "Размер платы на рубки главного пользования",
                        //    "Таблица: Разрешенные объемы лесопользования по рубкам главного пользования",
                        //    "Таблица: Разрешенные объемы ежегодного лесопользования по рубкам промежуточного пользования",
                        //    "Размер платы на рубки ухода",
                        //    "План проектируемых лесохозяйственных мероприятий",
                        //}.Each(needDescription => {
                        //    if (!tbRev.flDocs.GetGoodValue(ve.Env).Any(fileInfo => fileInfo.Description.Contains(needDescription))) {
                        //        ve.Env.AddError(tbRev.flDocs.FieldName, ve.Env.T("Требуется файл с содержанием в тексте описания") + "\"" + ve.Env.T(needDescription) + "\"");
                        //    }
                        //});

                        //if (tbRev.flDocs.GetGoodValue(ve.Env).Length == 0) {
                        //    ve.Env.AddError(tbRev.flDocs.FieldName, ve.Env.T("Требуются файлы следующего содержания") + ":");
                        //    new[] {
                        //        "Населенные пункты (таблица и описание)",
                        //        "Ресурсные данные",
                        //        "Размер платы на рубки главного пользования",
                        //        "Таблица: Разрешенные объемы лесопользования по рубкам главного пользования",
                        //        "Таблица: Разрешенные объемы ежегодного лесопользования по рубкам промежуточного пользования",
                        //        "Размер платы на рубки ухода",
                        //        "План проектируемых лесохозяйственных мероприятий",
                        //    }.Each(needDescription => {
                        //        ve.Env.AddError(tbRev.flDocs.FieldName, ve.Env.T(needDescription));
                        //    });
                        //}

                        if (ve.Env.IsValid) {
                            var pieces = tbRev.flForestryPieces.GetVal(ve.Env);
                            if (pieces.Length == 0) {
                                ve.Env.AddError(tbRev.flForestryPieces.FieldName, ve.Env.T("Нужно выбрать хотя-бы один выдел"));
                            } else {
                                if (pieces.GroupBy(piece => piece.flQuarter).Count() /*В SearchItem модели лесного выдела flQuarter это Id лесного хозяйства, а не квартала*/ > 1) {
                                    ve.Env.AddError(tbRev.flForestryPieces.FieldName, ve.Env.T("Нужно выбрать выделы из одного лесного хозяйства"));
                                }
                            }
                        }

                    })
                    .OnProcessing(pe => {
                        var tbRev = new TbObjectsRevisions();
                        pe.Model.flForestryPieces = tbRev.flForestryPieces.GetVal(pe.Env);

                        pe.Model.flName = tbRev.flName.GetVal(pe.Env);
                        pe.Model.flDescription = tbRev.flDescription.GetVal(pe.Env);
                        pe.Model.flNote = tbRev.flNote.GetVal(pe.Env);

                        pe.Model.flForestUsingType = tbRev.flForestUsingType.GetVal(pe.Env);
                        pe.Model.flForestUsingPeriod = tbRev.flForestUsingPeriod.GetVal(pe.Env);
                        pe.Model.flYearPay = tbRev.flYearPay.GetVal(pe.Env);
                        pe.Model.flYearReproduction = tbRev.flYearReproduction.GetVal(pe.Env);


                        var needFileCounter = 0;
                        var flDocs = new[] {
                            "Населенные пункты (таблица и описание)",
                            "Ресурсные данные",
                            "Размер платы на рубки главного пользования",
                            "Таблица: Разрешенные объемы лесопользования по рубкам главного пользования",
                            "Таблица: Разрешенные объемы ежегодного лесопользования по рубкам промежуточного пользования",
                            "Размер платы на рубки ухода",
                            "План проектируемых лесохозяйственных мероприятий",
                        }.SelectMany(needFile => {
                            var files = new FilesField($"flDoc{++needFileCounter}", needFile, "ForestObjectDocuments", new FileUploadSettings() { AllowedExtensions = new string[] { "pdf" } }).GetGoodValue(pe.Env).Select(x => {
                                if (!x.Description.Contains(needFile)) {
                                    x.Description = needFile + ". " + pe.Env.T("Подробнее") + ": " + x.Description;
                                }
                                return x;
                            });
                            return files;
                        }).ToArray();
                        pe.Model.flDocs = flDocs;

                        pe.Model.flAnotherInformation = tbRev.flAnotherInformation.GetVal(pe.Env);
                    })
                )
                ;
        }


        public override ForestObjectModel GetModel(ActionEnv<ObjectOrderQueryArgs> env, GetModelFrom from, ObjectsOrderTypes orderType, ITransaction transaction)
        {
            var model = from switch
            {
                GetModelFrom.Empty => new ForestObjectModel()
                {
                    flSellerBin = env.User.GetUserBin(env.QueryExecuter),
                    flStatus = ForestObjectStatuses.Active,
                    flBlock = ForestObjectBlocks.ActiveFree
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
                        model.flStatus = ForestObjectStatuses.Active;
                        model.flBlock = ForestObjectBlocks.ActiveFree;
                        break;
                }
            }
            if (from == GetModelFrom.Entity)
            {
                switch (orderType)
                {
                    case ObjectsOrderTypes.Remove:
                        model.flStatus = ForestObjectStatuses.Deleted;
                        model.flBlock = ForestObjectBlocks.DeletedDisqualified;
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
                GetModelFrom.Empty => urlHelper.YodaAction(ModuleName, nameof(MnuForestObjectOrder)),
                GetModelFrom.Entity => urlHelper.YodaAction(ModuleName, nameof(MnuForestObjectOrder), new ObjectOrderQueryArgs { Id = env.Args.Id.Value, MenuAction = "view" }),
                GetModelFrom.Revision => urlHelper.YodaAction(ModuleName, MenuName, new ObjectOrderQueryArgs { RevisionId = env.Args.RevisionId, MenuAction = Actions.ViewOrder }),
                _ => throw new NotImplementedException()
            };
        }

        public override ValidationResultBase IsCreateFromActionValid(ActionEnv<ObjectOrderQueryArgs> env)
        {
            if (env.Args.Id == null)
            {
                return new RedirectResult(new ArgumentException("Id"));
            }

            if (env.Args.OrderType == null)
            {
                return new RedirectResult(new ArgumentException("OrderType"));
            }

            if (!CanCreateOrder(env.Args.Id.Value, GetOrderType(env, GetOrderModelFrom.Entity), out var error, env)) {
                if (env.Args.Id.HasValue) {
                    return new RedirectResult(ModuleName, MnuForestObjectView.MnuName, new ForestObjectViewArgs() { Id = env.Args.Id.Value, MenuAction = MnuForestObjectView.Actions.View }, error);
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
                if (tradeResourceObject.flStatus != ForestObjectStatuses.Active)
                {
                    err = env.T("Изменение разрешено только для активного объекта");
                    return false;
                }
                err = null;
                return true;
            }
            bool checkActive(out string err)
            {
                if (tradeResourceObject.flStatus == ForestObjectStatuses.Deleted)
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

                    if (tradeResourceObject.flStatus == ForestObjectStatuses.Deleted)
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

        public override void SaveModel(int revisionId, ForestObjectModel model, ObjectsOrderTypes orderType, ActionEnv<ObjectOrderQueryArgs> env, ITransaction transaction, GetModelFrom modelFrom)
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

        public static void SetModel<TObjectsTable>(DataModifingQueryProxy<TObjectsTable> updateOrInsert, ForestObjectModel model) where TObjectsTable : TbObjectsBase
        {
            ForestObjectStatuses status = model.flStatus;

            updateOrInsert
                .SetT(t => t.flId, model.flId)
                .SetT(t => t.flRevisionId, model.flRevisionId)

                .SetT(t => t.flForestryPieces, model.flForestryPieces)

                .Set(t => t.flName, model.flName)
                .Set(t => t.flDescription, model.flDescription)
                .Set(t => t.flNote, model.flNote)

                .SetT(t => t.flSellerBin, model.flSellerBin)

                .Set(t => t.flStatus, model.flStatus.ToString())
                .Set(t => t.flBlock, model.flBlock.ToString())

                .SetT(t => t.flForestUsingType, model.flForestUsingType)
                .Set(t => t.flForestUsingPeriod, model.flForestUsingPeriod)
                .Set(t => t.flYearPay, model.flYearPay)
                .Set(t => t.flYearReproduction, model.flYearReproduction)
                .SetT(t => t.flDocs, JsonConvert.SerializeObject(model.flDocs))

                .Set(t => t.flAnotherInformation, model.flAnotherInformation)

                ;
        }

        public override bool TryAcceptModel(int revisionId, ForestObjectModel model, ObjectsOrderTypes orderType, ActionEnv<ObjectOrderQueryArgs> env, ITransaction transaction, out string error)
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

            if (env.Args.MenuAction == MnuOrderV2Actions.Execute) {
                if (orderType == ObjectsOrderTypes.Remove) {
                    new TbForestryPieces().Self(out var tbForestryPieces);
                    tbForestryPieces.AddFilterIn(tbForestryPieces.flId, model.flForestryPieces.Select(x => Convert.ToInt32(x.SearchItemId)).ToArray());
                    tbForestryPieces.Update()
                        .Set(t => t.flBlock, ForestryPieceBlocks.ActiveFree.ToString())
                        .Execute(env.QueryExecuter, transaction);
                }
                else if (new[] { ObjectsOrderTypes.Create, ObjectsOrderTypes.Edit }.Contains(orderType)) {
                    new TbForestryPieces().Self(out var tbForestryPieces);
                    tbForestryPieces.AddFilterIn(tbForestryPieces.flId, model.flForestryPieces.Select(x => Convert.ToInt32(x.SearchItemId)).ToArray());
                    tbForestryPieces.Update()
                        .Set(t => t.flBlock, ForestryPieceBlocks.ActiveConnected.ToString())
                        .Execute(env.QueryExecuter, transaction);
                }
            }

            error = null;
            return true;
        }

        public override void ViewModel(RenderActionEnv<ObjectOrderQueryArgs> env, ForestObjectModel model, ObjectsOrderTypes orderType, OrderStatus orderStatus)
        {
            ViewModel(env.Form, env, model);
        }
        public static void ViewModel(WidgetBase widget, FormEnvironment env, ForestObjectModel model)
        {
            env.RequestContext.Title = "Объект";
            viewModelMainData(widget, env, model);
            viewModelGeometry(widget, env, model);
        }

        public static void viewModelMainData(WidgetBase widget, FormEnvironment env, ForestObjectModel model, bool collapsed = false)
        {
            var tbRevs = new TbObjectsRevisions();
            var groupbox = new Accordion(env.T("Основные данные объекта"), collapsed);
            widget.AddComponent(groupbox);

            tbRevs.flId.RenderCustom(groupbox, env, model.flId.ToString() ?? env.T("*Генерируется автоматически при исполнении приказа"), readOnly: true);
            tbRevs.flRevisionId.RenderCustom(groupbox, env, model.flRevisionId, readOnly: true);

            tbRevs.flForestryPieces.RenderCustom(groupbox, env, model.flForestryPieces, readOnly: true);

            tbRevs.flSellerBin.RenderCustom(groupbox, env, model.flSellerBin, readOnly: true);
            tbRevs.flStatus.RenderCustom(groupbox, env, model.flStatus.ToString(), readOnly: true);
            tbRevs.flBlock.RenderCustom(groupbox, env, model.flBlock.ToString(), readOnly: true);

            tbRevs.flName.RenderCustom(groupbox, env, model.flName, readOnly: true);
            tbRevs.flDescription.RenderCustom(groupbox, env, model.flDescription, readOnly: true);
            tbRevs.flNote.RenderCustom(groupbox, env, model.flNote, readOnly: true);

            tbRevs.flForestUsingType.RenderCustom(groupbox, env, model.flForestUsingType, readOnly: true);
            tbRevs.flForestUsingPeriod.RenderCustom(groupbox, env, model.flForestUsingPeriod, readOnly: true);
            tbRevs.flYearPay.RenderCustom(groupbox, env, model.flYearPay, readOnly: true);
            tbRevs.flYearReproduction.RenderCustom(groupbox, env, model.flYearReproduction, readOnly: true);
            tbRevs.flDocs.RenderCustom(groupbox, env, JsonConvert.SerializeObject(model.flDocs ?? new FileInfo[] { }), readOnly: true, description: new Label(env.T("Требуются файлы следующего содержания") + ": " + string.Join(", ", new[] {
                "Населенные пункты (таблица и описание)",
                "Ресурсные данные",
                "Размер платы на рубки главного пользования",
                "Таблица: Разрешенные объемы лесопользования по рубкам главного пользования",
                "Таблица: Разрешенные объемы ежегодного лесопользования по рубкам промежуточного пользования",
                "Размер платы на рубки ухода",
                "План проектируемых лесохозяйственных мероприятий",
            })));

            tbRevs.flAnotherInformation.RenderCustom(groupbox, env, model.flAnotherInformation, readOnly: true);
        }

        public static void viewModelGeometry(WidgetBase widget, FormEnvironment env, ForestObjectModel model) {

            new TbForestryPieces()
                .Self(out var tbForestryPieces)
                .AddFilterIn(tbForestryPieces.flId, model.flForestryPieces.Select(x => Convert.ToInt32(x.SearchItemId)).ToArray());
            var forestryPiecesModel = tbForestryPieces.GetForestryPieceModels(env.QueryExecuter);

            var quarterModel = new TbQuarters()
                .AddFilter(t => t.flId, forestryPiecesModel[0].flQuarter)
                .GetQuarterModelFirst(env.QueryExecuter);

            var forestryModel = new TbForestries()
                .AddFilter(t => t.flId, quarterModel.flForestry)
                .GetForestryModelFirst(env.QueryExecuter);

            var groupbox = new Accordion(env.T("Геометрия"))
                .Append(new ObjectGeometryViewerComponent(forestryPiecesModel.Select(x => x.flWKT).ToArray(), wktsNeighbours: new[] { forestryModel.flWKT, quarterModel.flWKT }));
            widget.AddComponent(groupbox);
        }

        protected override bool HasAccessToOrder(int revisionId, ActionEnv<ObjectOrderQueryArgs> env)
        {
            if ((!env.User.IsExternalUser() && !env.User.IsGuest()))
            {
                return true;
            }
            if (env.Args.MenuAction != "view-order" && !env.User.HasRole("TRADERESOURCES-Лесные ресурсы-Выставление на торги", env.QueryExecuter)/*!env.User.HasCustomRole("forestobjects", "dataEdit", env.QueryExecuter)*/)
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
