using CommonSource;
using HuntingSource.Helpers;
using HuntingSource.Helpers.Object;
using HuntingSource.Models;
using HuntingSource.QueryTables.Object;
using HuntingSource.References.Object;
using HuntingSource.FieldViewers.Object;
using HuntingSource.SearchCollections.Object;
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

namespace TradeResourcesPlugin.Modules.HuntingMenus.Objects {
    public class MnuHuntingObjectOrderBase : MnuOrderBaseV2<HuntingObjectModel, ObjectsOrderTypes, ObjectOrderQueryArgs> {

        public MnuHuntingObjectOrderBase(string moduleName, string menuName, OrderStandartPermissions perms)
            : base(moduleName, menuName, "Приказ по охотничьим угодьям", perms, () => new TbObjectsOrderResult(), () => new TbObjectsOrderNegotiations())
        {
            AsCallback();
            Enabled(rc => {
                var xin = rc.User.GetUserXin(rc.QueryExecuter);
                if (xin == "050540004455"
                || xin == "050540000002"
                || (!rc.User.IsExternalUser() && !rc.User.IsGuest())
                || rc.User.HasRole("TRADERESOURCES-Охотничьи угодья-Создание приказов", rc.QueryExecuter)/*rc.User.HasCustomRole("huntingobjects", "dataEdit", rc.QueryExecuter)*/)
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

        public override IWizardFormBuilderWithStep<ObjectOrderQueryArgs, ModelOrderArgs<ObjectsOrderTypes, ObjectOrderQueryArgs>, HuntingObjectModel>
            EditModel(IWizardFormBuilderWithFinishButton<ObjectOrderQueryArgs, ModelOrderArgs<ObjectsOrderTypes, ObjectOrderQueryArgs>, HuntingObjectModel> wizard)
        {
            return wizard
                .Step("Удаление", step => step
                    .Enabled(env => env.Args.OrderType.In(ObjectsOrderTypes.Remove))
                    .OnRendering(re => {
                        var tbRev = new TbObjectsRevisions();
                        var model = re.Model;

                        tbRev.flId.RenderCustom(re.Panel, re.Env, model.flId, readOnly: true);
                        tbRev.flStatus.RenderCustom(re.Panel, re.Env, HuntingObjectStatuses.Deleted.ToString(), cssClass: "hidden");
                        tbRev.flStatus.RenderCustom(re.Panel, re.Env, HuntingObjectStatuses.Deleted.ToString(), readOnly: true);
                        tbRev.flBlock.RenderCustom(re.Panel, re.Env, null);
                    })
                    .OnValidating(ve => {
                        //var tbRev = new TbObjectsRevisions();
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
                        var tbRev = new TbObjectsRevisions();
                        pe.Model.flStatus = HuntingObjectStatuses.Deleted.ToString();
                        pe.Model.flBlock = tbRev.flBlock.GetVal(pe.Env);
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

                        tbRev.flSallerBin.RenderCustom(re.Panel, re.Env, model.flSallerBin, readOnly: true);
                        tbRev.flStatus.RenderCustom(re.Panel, re.Env, model.flStatus, readOnly: true);
                        tbRev.flBlock.RenderCustom(re.Panel, re.Env, model.flBlock.ToString(), readOnly: true);

                        tbRev.flName.RenderCustom(re.Panel, re.Env, model.flName);
                        tbRev.flDescription.RenderCustom(re.Panel, re.Env, model.flDescription);
                        tbRev.flNote.RenderCustom(re.Panel, re.Env, model.flNote);

                        tbRev.flYear.RenderCustom(re.Panel, re.Env, model.flYear);
                        tbRev.flCategory.RenderCustom(re.Panel, re.Env, model.flCategory);
                        tbRev.flWildfowlThroughput.RenderCustom(re.Panel, re.Env, model.flWildfowlThroughput);
                        tbRev.flDocs.RenderCustomT(re.Panel, re.Env, JsonConvert.SerializeObject(re.Model.flDocs ?? new FileInfo[] { }));
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
                        pe.Model.setMainData(
                            tbRev.flName.GetVal(pe.Env),
                            tbRev.flDescription.GetVal(pe.Env),
                            tbRev.flNote.GetVal(pe.Env),
                            tbRev.flYear.GetVal(pe.Env),
                            tbRev.flCategory.GetVal(pe.Env),
                            (Dictionary<string, object>[])tbRev.flWildfowlThroughput.GetValue(pe.Env),
                            tbRev.flDocs.GetGoodValue(pe.Env)
                        );
                    })
                )
                .Step("Географическое расположение", step => step
                    .Enabled(env => env.Args.OrderType.In(ObjectsOrderTypes.Create, ObjectsOrderTypes.Edit))
                    .OnRendering(re => {
                        var tbRev = new TbObjectsRevisions();
                        var model = re.Model;

                        tbRev.flCountry.RenderCustom(re.Panel, re.Env, model.flCountry);
                        tbRev.flRegion.RenderCustom(re.Panel, re.Env, model.flRegion);
                        tbRev.flDistrict.RenderCustom(re.Panel, re.Env, model.flDistrict);
                        tbRev.flLocation.RenderCustom(re.Panel, re.Env, model.flLocation);

                        if (re.Args.OrderType == ObjectsOrderTypes.Edit || re.Args.OriginalArgs.MenuAction == "edit")
                        {
                            re.Panel.AddComponent(new Referencebox("flDraw", new RefDrawGeom(), "Геометрия") { CssClass = " form-control " });
                        }
                    })
                    .OnValidating(ve => {
                        var tbRev = new TbObjectsRevisions();
                        var checkFields = new Field[] {
                            tbRev.flCountry,
                            tbRev.flRegion,
                            tbRev.flDistrict,
                            tbRev.flLocation
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

                        pe.Model.setGeoPosition(
                            tbRev.flCountry.GetVal(pe.Env),
                            tbRev.flRegion.GetVal(pe.Env),
                            tbRev.flDistrict.GetVal(pe.Env),
                            tbRev.flLocation.GetVal(pe.Env)
                        );

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

                        tbRev.AddFilter(t => t.flSallerBin, model.flSallerBin);
                        tbRev.AddFilter(t => t.flStatus, HuntingObjectStatuses.Active.ToString());
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
                                tbActRevs.AddFilter(t => t.flSallerBin, model.flSallerBin);
                                tbActRevs.AddFilterNot(t => t.flId, ve.Model.flId);
                                tbActRevs.AddFilter(t => t.flStatus, HuntingObjectStatuses.Active.ToString());
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

                        pe.Model.setGeometry(
                            tbRev.flWKT.GetVal(pe.Env),
                            tbRev.flCoords.GetVal(pe.Env)
                        );
                    })
                )
                .Step("Физическая характеристика", step => step
                    .Enabled(env => env.Args.OrderType.In(ObjectsOrderTypes.Create, ObjectsOrderTypes.Edit))
                    .OnRendering(re => {
                        var tbRev = new TbObjectsRevisions();
                        var model = re.Model;

                        tbRev.flHuntArea.RenderCustom(re.Panel, re.Env, model.flHuntArea);
                        tbRev.flForestArea.RenderCustom(re.Panel, re.Env, model.flForestArea);
                        tbRev.flAgriArea.RenderCustom(re.Panel, re.Env, model.flAgriArea);
                        tbRev.flWaterArea.RenderCustom(re.Panel, re.Env, model.flWaterArea);
                        tbRev.flLandReserveArea.RenderCustom(re.Panel, re.Env, model.flLandReserveArea);
                        tbRev.flOtherArea.RenderCustom(re.Panel, re.Env, model.flOtherArea);
                        tbRev.flRangerSites.RenderCustom(re.Panel, re.Env, model.flRangerSites);

                    })
                    .OnValidating(ve => {
                        var tbRev = new TbObjectsRevisions();
                        var checkFields = new Field[] {
                            tbRev.flHuntArea,
                            tbRev.flForestArea,
                            tbRev.flAgriArea,
                            tbRev.flWaterArea,
                            tbRev.flLandReserveArea,
                            tbRev.flOtherArea,
                            tbRev.flRangerSites
                        };
                        checkFields.Each(f => f.Validate(ve.Env));

                    })
                    .OnProcessing(pe => {
                        var tbRev = new TbObjectsRevisions();

                        pe.Model.setPhysicalCharacteristic(
                            tbRev.flHuntArea.GetVal(pe.Env),
                            tbRev.flForestArea.GetVal(pe.Env),
                            tbRev.flAgriArea.GetVal(pe.Env),
                            tbRev.flWaterArea.GetVal(pe.Env),
                            tbRev.flLandReserveArea.GetVal(pe.Env),
                            tbRev.flOtherArea.GetVal(pe.Env),
                            tbRev.flRangerSites.GetVal(pe.Env)
                        );
                    })
                )
                .Step("Особые данные", step => step
                    .Enabled(env => env.Args.OrderType.In(ObjectsOrderTypes.Create, ObjectsOrderTypes.Edit))
                    .OnRendering(re => {
                        var tbRev = new TbObjectsRevisions();
                        var model = re.Model;

                        tbRev.flInsideOrganization.RenderCustom(re.Panel, re.Env, model.flInsideOrganization);
                        tbRev.flHuntingState.RenderCustom(re.Panel, re.Env, model.flHuntingState);
                        tbRev.flHarmfulAnimals.RenderCustom(re.Panel, re.Env, model.flHarmfulAnimals);
                        tbRev.flAnimalsAdmission.RenderCustom(re.Panel, re.Env, model.flAnimalsAdmission);
                        tbRev.flBiotechnicalActivities.RenderCustom(re.Panel, re.Env, model.flBiotechnicalActivities);
                        tbRev.flConsideredAnimals.RenderCustom(re.Panel, re.Env, model.flConsideredAnimals);
                        tbRev.flWildAnimalsFeed.RenderCustom(re.Panel, re.Env, model.flWildAnimalsFeed);
                        tbRev.flInvestedWork.RenderCustom(re.Panel, re.Env, model.flInvestedWork);
                        tbRev.flAntipoaching.RenderCustom(re.Panel, re.Env, model.flAntipoaching);
                        tbRev.flHuntingDogsAndDecoyDucks.RenderCustom(re.Panel, re.Env, model.flHuntingDogsAndDecoyDucks);
                        tbRev.flBuildings.RenderCustom(re.Panel, re.Env, model.flBuildings);
                        tbRev.flTransport.RenderCustom(re.Panel, re.Env, model.flTransport);
                    })
                    .OnValidating(ve => {
                        var tbRev = new TbObjectsRevisions();
                        var checkFields = new Field[] {
                            tbRev.flInsideOrganization,
                            tbRev.flHuntingState,
                            tbRev.flHarmfulAnimals,
                            tbRev.flAnimalsAdmission,
                            tbRev.flBiotechnicalActivities,
                            tbRev.flConsideredAnimals,
                            tbRev.flWildAnimalsFeed,
                            tbRev.flInvestedWork,
                            tbRev.flAntipoaching,
                            tbRev.flHuntingDogsAndDecoyDucks,
                            tbRev.flBuildings,
                            tbRev.flTransport,
                        };
                        checkFields.Each(f => f.Validate(ve.Env));

                    })
                    .OnProcessing(pe => {
                        var tbRev = new TbObjectsRevisions();

                        var flInsideOrganization = (Dictionary<string, object>[])tbRev.flInsideOrganization.GetValue(pe.Env);
                        var flHuntingState = (Dictionary<string, object>[])tbRev.flHuntingState.GetValue(pe.Env);
                        var flHarmfulAnimals = (Dictionary<string, object>[])tbRev.flHarmfulAnimals.GetValue(pe.Env);
                        var flAnimalsAdmission = (Dictionary<string, object>[])tbRev.flAnimalsAdmission.GetValue(pe.Env);
                        var flBiotechnicalActivities = (Dictionary<string, object>[])tbRev.flBiotechnicalActivities.GetValue(pe.Env);
                        var flConsideredAnimals = (Dictionary<string, object>[])tbRev.flConsideredAnimals.GetValue(pe.Env);
                        var flWildAnimalsFeed = (Dictionary<string, object>[])tbRev.flWildAnimalsFeed.GetValue(pe.Env);
                        var flInvestedWork = (Dictionary<string, object>[])tbRev.flInvestedWork.GetValue(pe.Env);
                        var flAntipoaching = (Dictionary<string, object>[])tbRev.flAntipoaching.GetValue(pe.Env);
                        var flHuntingDogsAndDecoyDucks = (Dictionary<string, object>[])tbRev.flHuntingDogsAndDecoyDucks.GetValue(pe.Env);
                        var flBuildings = (Dictionary<string, object>[])tbRev.flBuildings.GetValue(pe.Env);
                        var flTransport = (Dictionary<string, object>[])tbRev.flTransport.GetValue(pe.Env);

                        pe.Model.setSpecificCharacteristic(
                            flInsideOrganization,
                            flHuntingState,
                            flHarmfulAnimals,
                            flAnimalsAdmission,
                            flBiotechnicalActivities,
                            flConsideredAnimals,
                            flWildAnimalsFeed,
                            flInvestedWork,
                            flAntipoaching,
                            flHuntingDogsAndDecoyDucks,
                            flBuildings,
                            flTransport
                        );
                    })
                )
                ;
        }


        public override HuntingObjectModel GetModel(ActionEnv<ObjectOrderQueryArgs> env, GetModelFrom from, ObjectsOrderTypes orderType, ITransaction transaction)
        {
            var model = from switch
            {
                GetModelFrom.Empty => new HuntingObjectModel()
                {
                    flSallerBin = env.User.GetUserBin(env.QueryExecuter),
                    flSallerId = -1,
                    flStatus = HuntingObjectStatuses.Active.ToString(),
                    flBlock = HuntingObjectBlocks.ActiveFree.ToString()
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
                        model.flStatus = HuntingObjectStatuses.Active.ToString();
                        model.flBlock = HuntingObjectBlocks.ActiveFree.ToString();
                        break;
                }
            }
            if (from == GetModelFrom.Entity)
            {
                switch (orderType)
                {
                    case ObjectsOrderTypes.Remove:
                        model.flStatus = HuntingObjectStatuses.Deleted.ToString();
                        model.flBlock = HuntingObjectBlocks.DeletedDisqualified.ToString();
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
                GetModelFrom.Empty => urlHelper.YodaAction(ModuleName, nameof(MnuHuntingObjectOrderBase)),
                GetModelFrom.Entity => urlHelper.YodaAction(ModuleName, nameof(MnuHuntingObjectOrderBase), new ObjectOrderQueryArgs { Id = env.Args.Id.Value, MenuAction = "view" }),
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

            if (!CanCreateOrder(env.Args.Id.Value, GetOrderType(env, GetOrderModelFrom.Entity), out var error, env))
            {
                if (env.Args.Id.HasValue) {
                    return new RedirectResult(ModuleName, MnuHuntingObjectView.MnuName, new HuntingObjectViewArgs() { Id = env.Args.Id.Value, MenuAction = MnuHuntingObjectView.Actions.View }, error);
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

            var tradeResourceObject = ObjectHelper.GetObjectModel(id, env.QueryExecuter);

            bool checkFieldActive(out string err)
            {
                if (tradeResourceObject.flStatus != HuntingObjectStatuses.Active.ToString())
                {
                    err = env.T("Изменение разрешено только для активного объекта");
                    return false;
                }
                err = null;
                return true;
            }
            bool checkActive(out string err)
            {
                if (tradeResourceObject.flStatus == HuntingObjectStatuses.Deleted.ToString())
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

                    if (tradeResourceObject.flStatus == HuntingObjectStatuses.Deleted.ToString())
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

        public override void SaveModel(int revisionId, HuntingObjectModel model, ObjectsOrderTypes orderType, ActionEnv<ObjectOrderQueryArgs> env, ITransaction transaction, GetModelFrom modelFrom)
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

        public static void SetModel<TObjectsTable>(DataModifingQueryProxy<TObjectsTable> updateOrInsert, HuntingObjectModel model) where TObjectsTable : TbObjectsBase
        {
            HuntingObjectStatuses status = model.flStatus == HuntingObjectStatuses.Active.ToString() ? HuntingObjectStatuses.Active : HuntingObjectStatuses.Deleted;

            var geometry = new GeomFromWKT(model.flWKT, 4326);
            var point = new PointFromGeom(new SqlExpVal(geometry));

            updateOrInsert
                .SetT(t => t.flId, model.flId)
                .SetT(t => t.flRevisionId, model.flRevisionId)

                .SetT(t => t.flSallerId, model.flSallerId)
                .SetT(t => t.flSallerBin, model.flSallerBin)
                .Set(t => t.flStatus, model.flStatus)
                .Set(t => t.flBlock, model.flBlock)

                .Set(t => t.flName, model.flName)
                .Set(t => t.flDescription, model.flDescription)
                .Set(t => t.flNote, model.flNote)
                .Set(t => t.flYear, model.flYear)
                .Set(t => t.flCategory, model.flCategory)
                .SetT(t => t.flWildfowlThroughput, model.flWildfowlThroughput)
                .SetT(t => t.flDocs, JsonConvert.SerializeObject(model.flDocs))

                .Set(t => t.flCountry, model.flCountry)
                .Set(t => t.flRegion, model.flRegion)
                .Set(t => t.flDistrict, model.flDistrict)
                .Set(t => t.flLocation, model.flLocation)
                .Set(t => t.flWKT, model.flWKT)
                .SetT(t => t.flCoords, model.flCoords)
                .Set(t => t.flGeometry, geometry)
                .Set(t => t.flPoint, point)

                .Set(t => t.flHuntArea, model.flHuntArea)
                .Set(t => t.flForestArea, model.flForestArea)
                .Set(t => t.flAgriArea, model.flAgriArea)
                .Set(t => t.flWaterArea, model.flWaterArea)
                .Set(t => t.flLandReserveArea, model.flLandReserveArea)
                .Set(t => t.flOtherArea, model.flOtherArea)
                .Set(t => t.flRangerSites, model.flRangerSites)

                .SetT(t => t.flInsideOrganization, model.flInsideOrganization)
                .SetT(t => t.flHuntingState, model.flHuntingState)
                .SetT(t => t.flHarmfulAnimals, model.flHarmfulAnimals)
                .SetT(t => t.flAnimalsAdmission, model.flAnimalsAdmission)
                .SetT(t => t.flBiotechnicalActivities, model.flBiotechnicalActivities)
                .SetT(t => t.flConsideredAnimals, model.flConsideredAnimals)
                .SetT(t => t.flWildAnimalsFeed, model.flWildAnimalsFeed)
                .SetT(t => t.flInvestedWork, model.flInvestedWork)
                .SetT(t => t.flAntipoaching, model.flAntipoaching)
                .SetT(t => t.flHuntingDogsAndDecoyDucks, model.flHuntingDogsAndDecoyDucks)
                .SetT(t => t.flBuildings, model.flBuildings)
                .SetT(t => t.flTransport, model.flTransport)
                ;
        }

        public override bool TryAcceptModel(int revisionId, HuntingObjectModel model, ObjectsOrderTypes orderType, ActionEnv<ObjectOrderQueryArgs> env, ITransaction transaction, out string error)
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

        public override void ViewModel(RenderActionEnv<ObjectOrderQueryArgs> env, HuntingObjectModel model, ObjectsOrderTypes orderType, OrderStatus orderStatus)
        {
            ViewModel(env.Form, env, model);
        }
        public static void ViewModel(WidgetBase widget, FormEnvironment env, HuntingObjectModel model)
        {
            env.RequestContext.Title = "Объект";
            viewModelMainData(widget, env, model);
            viewModelGeoPositionData(widget, env, model);
            viewModelGeometryData(widget, env, model);
            viewModelPhysicalCharacteristicData(widget, env, model);
            viewModelSpecificCharacteristicData(widget, env, model);
        }

        public static void viewModelMainData(WidgetBase widget, FormEnvironment env, HuntingObjectModel model, bool collapsed = false)
        {
            var tbRevs = new TbObjectsRevisions();
            var groupbox = new Accordion(env.T("Основные данные объекта"), collapsed);
            widget.AddComponent(groupbox);

            tbRevs.flId.RenderCustom(groupbox, env, model.flId.ToString() ?? env.T("*Генерируется автоматически при исполнении приказа"), readOnly: true);
            tbRevs.flRevisionId.RenderCustom(groupbox, env, model.flRevisionId, readOnly: true);
            tbRevs.flSallerBin.RenderCustom(groupbox, env, model.flSallerBin, readOnly: true);
            tbRevs.flStatus.RenderCustom(groupbox, env, model.flStatus, readOnly: true);
            tbRevs.flBlock.RenderCustom(groupbox, env, model.flBlock, readOnly: true);
            tbRevs.flName.RenderCustom(groupbox, env, model.flName, readOnly: true);
            tbRevs.flDescription.RenderCustom(groupbox, env, model.flDescription, readOnly: true);
            tbRevs.flNote.RenderCustom(groupbox, env, model.flNote, readOnly: true);

            tbRevs.flYear.RenderCustom(groupbox, env, model.flYear, readOnly: true);
            tbRevs.flCategory.RenderCustom(groupbox, env, model.flCategory, readOnly: true);
            tbRevs.flWildfowlThroughput.RenderCustom(groupbox, env, model.flWildfowlThroughput, readOnly: true);
            tbRevs.flDocs.RenderCustom(groupbox, env, JsonConvert.SerializeObject(model.flDocs ?? new FileInfo[] { }), readOnly: true);
        }

        public static void viewModelGeoPositionData(WidgetBase widget, FormEnvironment env, HuntingObjectModel model, bool collapsed = false)
        {
            var tbRevs = new TbObjectsRevisions();
            var groupbox = new Accordion(env.T("Географическое расположение"), collapsed);
            widget.AddComponent(groupbox);

            tbRevs.flCountry.RenderCustom(groupbox, env, model.flCountry, readOnly: true);
            tbRevs.flRegion.RenderCustom(groupbox, env, model.flRegion, readOnly: true);
            tbRevs.flDistrict.RenderCustom(groupbox, env, model.flDistrict, readOnly: true);
            tbRevs.flLocation.RenderCustom(groupbox, env, model.flLocation, readOnly: true);
        }

        public static void viewModelGeometryData(WidgetBase widget, FormEnvironment env, HuntingObjectModel model, bool collapsed = false)
        {
            var groupbox = new Accordion(env.T("Геометрия"), collapsed);
            widget.AddComponent(groupbox);
            var row = new GridRow();
            groupbox.AddComponent(row);

            row.AddComponent(new ObjectGeometryViewerComponent(model.flWKT, "col-md-6"));
            row.AddComponent(new ObjectCoordsViewerComponent(model.flCoords, "col-md-6"));
        }

        public static void viewModelPhysicalCharacteristicData(WidgetBase widget, FormEnvironment env, HuntingObjectModel model, bool collapsed = false)
        {
            var tbRevs = new TbObjectsRevisions();
            var groupbox = new Accordion(env.T("Физическая характеристика"), collapsed);
            widget.AddComponent(groupbox);

            tbRevs.flHuntArea.RenderCustom(groupbox, env, model.flHuntArea, readOnly: true);
            tbRevs.flForestArea.RenderCustom(groupbox, env, model.flForestArea, readOnly: true);
            tbRevs.flAgriArea.RenderCustom(groupbox, env, model.flAgriArea, readOnly: true);
            tbRevs.flWaterArea.RenderCustom(groupbox, env, model.flWaterArea, readOnly: true);
            tbRevs.flLandReserveArea.RenderCustom(groupbox, env, model.flLandReserveArea, readOnly: true);
            tbRevs.flOtherArea.RenderCustom(groupbox, env, model.flOtherArea, readOnly: true);
            tbRevs.flRangerSites.RenderCustom(groupbox, env, model.flRangerSites, readOnly: true);
        }

        public static void viewModelSpecificCharacteristicData(WidgetBase widget, FormEnvironment env, HuntingObjectModel model, bool collapsed = false)
        {
            var tbRevs = new TbObjectsRevisions();
            var groupbox = new Accordion(env.T("Особые данные"), collapsed);
            widget.AddComponent(groupbox);

            tbRevs.flInsideOrganization.RenderCustom(groupbox, env, model.flInsideOrganization, readOnly: true);
            tbRevs.flHuntingState.RenderCustom(groupbox, env, model.flHuntingState, readOnly: true);
            tbRevs.flHarmfulAnimals.RenderCustom(groupbox, env, model.flHarmfulAnimals, readOnly: true); //, customEditor: new TypeYearTable("sdsd", "text", "flHarmfulAnimalType", "flYear", "flCount", env));
            tbRevs.flAnimalsAdmission.RenderCustom(groupbox, env, model.flAnimalsAdmission, readOnly: true);
            tbRevs.flBiotechnicalActivities.RenderCustom(groupbox, env, model.flBiotechnicalActivities, readOnly: true);
            tbRevs.flConsideredAnimals.RenderCustom(groupbox, env, model.flConsideredAnimals, readOnly: true);
            tbRevs.flWildAnimalsFeed.RenderCustom(groupbox, env, model.flWildAnimalsFeed, readOnly: true);
            tbRevs.flInvestedWork.RenderCustom(groupbox, env, model.flInvestedWork, readOnly: true);
            tbRevs.flAntipoaching.RenderCustom(groupbox, env, model.flAntipoaching, readOnly: true);
            tbRevs.flHuntingDogsAndDecoyDucks.RenderCustom(groupbox, env, model.flHuntingDogsAndDecoyDucks, readOnly: true);
            tbRevs.flBuildings.RenderCustom(groupbox, env, model.flBuildings, readOnly: true);
            tbRevs.flTransport.RenderCustom(groupbox, env, model.flTransport, readOnly: true);
        }

        protected override bool HasAccessToOrder(int revisionId, ActionEnv<ObjectOrderQueryArgs> env)
        {
            if ((!env.User.IsExternalUser() && !env.User.IsGuest()))
            {
                return true;
            }
            if (env.Args.MenuAction != "view-order" && !env.User.HasRole("TRADERESOURCES-Охотничьи угодья-Создание приказов", env.QueryExecuter)/*!env.User.HasCustomRole("huntingobjects", "dataEdit", env.QueryExecuter)*/)
            {
                throw new AccessDeniedException();
            }
            var hasRecords = new TbObjectsRevisions()
                .AddFilter(t => t.flRevisionId, revisionId)
                .AddFilter(t => t.flSallerBin, env.User.GetUserBin(env.QueryExecuter))
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
