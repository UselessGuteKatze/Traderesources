using FileStoreInterfaces;
using CommonSource;
using FishingSource.Helpers;
using FishingSource.Helpers.Object;
using FishingSource.Helpers.Reservoir;
using FishingSource.Models;
using FishingSource.QueryTables.Object;
using FishingSource.QueryTables.Reservoir;
using FishingSource.References.Object;
using FishingSource.References.Reservoir;
using FishingSource.SearchCollections.Object;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using TradeResourcesPlugin.Helpers;
using TradeResourcesPlugin.Modules.Components;
using TradeResourcesPlugin.Modules.FishingMenus.Reservoirs;
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

namespace TradeResourcesPlugin.Modules.FishingMenus.Objects {
    public class MnuFishingObjectOrderBase : MnuOrderBaseV2<FishingObjectModel, ObjectsOrderTypes, ObjectOrderQueryArgs> {

        public MnuFishingObjectOrderBase(string moduleName, string menuName, OrderStandartPermissions perms)
            : base(moduleName, menuName, "Приказ по рыбохозяйственным водоёмам", perms, () => new TbObjectsOrderResult(), () => new TbObjectsOrderNegotiations()) {
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

        public override Task<ActionItem[]> GetActions(ActionEnv<ObjectOrderQueryArgs> env) {
            var ret = base.GetActions(env);

            switch (env.Args.MenuAction) {
                case Actions.ViewOrder:
                    break;
                case Actions.CreateFrom:
                    break;
            }

            return ret;
        }

        public override IWizardFormBuilderWithStep<ObjectOrderQueryArgs, ModelOrderArgs<ObjectsOrderTypes, ObjectOrderQueryArgs>, FishingObjectModel>
            EditModel(IWizardFormBuilderWithFinishButton<ObjectOrderQueryArgs, ModelOrderArgs<ObjectsOrderTypes, ObjectOrderQueryArgs>, FishingObjectModel> wizard) {
            return wizard
                .Step("Удаление", step => step
                    .Enabled(env => env.Args.OrderType.In(ObjectsOrderTypes.Remove))
                    .OnRendering(re => {
                        var tbRev = new TbObjectsRevisions();
                        var model = re.Model;

                        tbRev.flId.RenderCustom(re.Panel, re.Env, model.flId, readOnly: true);
                        tbRev.flStatus.RenderCustom(re.Panel, re.Env, FishingObjectStatuses.Deleted.ToString(), cssClass: "hidden");
                        tbRev.flStatus.RenderCustom(re.Panel, re.Env, FishingObjectStatuses.Deleted.ToString(), readOnly: true);
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
                        pe.Model.flStatus = FishingObjectStatuses.Deleted.ToString();
                        pe.Model.flBlock = tbRev.flBlock.GetVal(pe.Env);
                    })
                )
                .Step("Водоём", step => step
                    .Enabled(env => env.Args.OrderType.In(ObjectsOrderTypes.Create, ObjectsOrderTypes.Edit))
                    .OnRendering(re => {
                        var tbRev = new TbObjectsRevisions();
                        var model = re.Model;
                        int? objectId = null;
                        if (model.flId != 0)
                        {
                            objectId = model.flId;
                        }

                        tbRev.flReservoirId.RenderCustom(re.Panel, re.Env, model.flReservoirId);

                        re.Panel.AddComponent(new Link
                        {
                            Controller = ModuleName,
                            Action = nameof(MnuReservoirOrderBase),
                            RouteValues = new ReservoirOrderQueryArgs { RevisionId = -1, MenuAction = "create-new" },
                            Text = re.Env.T("Добавить отсутсвующий в списке водоём"),
                            CssClass = "btn btn-sm btn-success"
                        });

                    })
                    .OnValidating(ve => {
                        var tbRev = new TbObjectsRevisions();
                        var checkFields = new Field[] {
                            tbRev.flReservoirId
                        };
                        checkFields.Each(f => f.Validate(ve.Env));

                        if (ve.Env.IsValid)
                        {
                            var reservoir = ReservoirHelper.GetReservoirModel(tbRev.flReservoirId.GetVal(ve.Env), ve.Env.QueryExecuter);

                            if (
                                !reservoir.flArea.HasValue ||
                                string.IsNullOrEmpty(reservoir.flCountry) ||
                                string.IsNullOrEmpty(reservoir.flRegion) ||
                                string.IsNullOrEmpty(reservoir.flDistrict) ||
                                string.IsNullOrEmpty(reservoir.flWKT)
                            )
                            {
                                ve.Env.Redirect.SetRedirect(
                                    ModuleName,
                                    nameof(MnuReservoirOrderBase),
                                    new ReservoirOrderQueryArgs
                                    {
                                        Id = reservoir.flId,
                                        RevisionId = reservoir.flRevisionId,
                                        OrderType = ReservoirsOrderTypeActions.Edit,
                                        MenuAction = "create-from",
                                        Message = ve.Env.T("Вы перенаправлены на приказ на редактирование водоёма, т.к. не все данные заполнены")
                                    }
                                );
                            }
                        }
                    })
                    .OnProcessing(pe => {
                        var tbRev = new TbObjectsRevisions();
                        pe.Model.flReservoirId = tbRev.flReservoirId.GetVal(pe.Env);
                    })
                )
                .Step("Основные данные", step => step
                    .Enabled(env => env.Args.OrderType.In(ObjectsOrderTypes.Create, ObjectsOrderTypes.Edit))
                    .OnRendering(re => {
                        var tbRev = new TbObjectsRevisions();
                        var model = re.Model;
                        int? objectId = null;
                        if (model.flId != 0) {
                            objectId = model.flId;
                        }

                        tbRev.flSallerBin.RenderCustom(re.Panel, re.Env, model.flSallerBin, readOnly: true);
                        tbRev.flStatus.RenderCustom(re.Panel, re.Env, model.flStatus, readOnly: true);
                        tbRev.flBlock.RenderCustom(re.Panel, re.Env, model.flBlock.ToString(), readOnly: true);

                        tbRev.flName.RenderCustom(re.Panel, re.Env, model.flName);
                        tbRev.flDescription.RenderCustom(re.Panel, re.Env, model.flDescription);
                        tbRev.flNote.RenderCustom(re.Panel, re.Env, model.flNote);
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
                            tbRev.flDocs.GetGoodValue(pe.Env)
                        );
                    })
                )
                .Step("Географическое расположение", step => step
                    .Enabled(env => env.Args.OrderType.In(ObjectsOrderTypes.Create, ObjectsOrderTypes.Edit))
                    .OnRendering(re => {
                        var tbRev = new TbObjectsRevisions();
                        var model = re.Model;

                        if (string.IsNullOrEmpty(model.flRegion))
                        {
                            var reservoir = ReservoirHelper.GetReservoirModel(model.flReservoirId, re.Env.QueryExecuter);
                            model.flCountry = reservoir.flCountry;
                            model.flRegion = reservoir.flRegion;
                            model.flDistrict = reservoir.flDistrict;
                        }

                        tbRev.flCountry.RenderCustom(re.Panel, re.Env, model.flCountry);
                        tbRev.flRegion.RenderCustom(re.Panel, re.Env, model.flRegion);
                        tbRev.flDistrict.RenderCustom(re.Panel, re.Env, model.flDistrict);
                        tbRev.flLocation.RenderCustom(re.Panel, re.Env, model.flLocation);
                        tbRev.flPasportCoords.RenderCustom(re.Panel, re.Env, model.flPasportCoords);

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
                            tbRev.flLocation,
                            tbRev.flPasportCoords
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

                        var flPasportCoords = (Dictionary<string, object>[])tbRev.flPasportCoords.GetValue(pe.Env);

                        pe.Model.setGeoPosition(
                            tbRev.flCountry.GetVal(pe.Env),
                            tbRev.flRegion.GetVal(pe.Env),
                            tbRev.flDistrict.GetVal(pe.Env),
                            tbRev.flLocation.GetVal(pe.Env),
                            flPasportCoords
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
                        tbRev.AddFilterNot(t => t.flStatus, FishingObjectStatuses.Deleted.ToString());
                        tbRev.AddFilterNot(t => t.flId, model.flId);
                        var wkts = tbRev.Select(new FieldAlias[] {
                            tbRev.flWKT
                        }, re.Env.QueryExecuter).AsEnumerable().Select(r => tbRev.flWKT.GetRowVal(r)).ToArray();

                        var reservoirWkt = new TbReservoirs().AddFilter(t => t.flId, model.flReservoirId).SelectScalar(t => t.flWKT, re.Env.QueryExecuter);

                        re.Panel.AddComponent(new Textbox("flWKT", cssClass: "d-none"));
                        re.Panel.AddComponent(new Textbox("flCoords", cssClass: "d-none"));


                        string oldver = null;
                        if (re.Args.OrderType == ObjectsOrderTypes.Edit || re.Args.OriginalArgs.MenuAction == "edit")
                        {
                            oldver = new TbObjects().AddFilter(t => t.flId, model.flId).SelectScalar(t => t.flWKT, re.Env.QueryExecuter);
                        }

                        re.Panel.AddComponent(new ObjectDrawByCoordsComponent("flWKT", "flCoords", wkts, reservoirWkt, oldver));

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
                                tbActRevs.AddFilterNot(t => t.flStatus, FishingObjectStatuses.Deleted.ToString());
                                tbActRevs.AddFilterNot(t => t.flId, ve.Model.flId);
                                tbActRevs.AddFilter(t => t.flSallerBin, model.flSallerBin);
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

                                var tbReservoirActRevs = new TbReservoirs();
                                tbReservoirActRevs.AddFilter(t => t.flId, ve.Model.flReservoirId);
                                var reservoirContains = new GeomContains(new SqlExpVal(new FieldToSqlExp(tbReservoirActRevs.flGeometry)), new SqlExpVal(geometry));
                                var reservoirInresects = new GeomIntersects(new SqlExpVal(new ExteriorRing(new SqlExpVal(new FieldToSqlExp(tbReservoirActRevs.flGeometry)))), new SqlExpVal(geometry));

                                var rservoirResult = tbReservoirActRevs.Select(new FieldAlias[] {
                                new FieldAlias(reservoirContains, "flContains"),
                                new FieldAlias(reservoirInresects, "flIntersects"),
                            }, ve.Env.QueryExecuter).AsEnumerable();

                                if (rservoirResult.Any(r => !Convert.ToBoolean(r["flContains"])))
                                {
                                    ve.Env.AddError(tbRev.flWKT.FieldName, ve.Env.T("Нарисованная вами геометрия находится за границами водоёма"));
                                }

                                if (rservoirResult.Any(r => Convert.ToBoolean(r["flIntersects"])))
                                {
                                    ve.Env.AddError(tbRev.flWKT.FieldName, ve.Env.T("Нарисованная вами геометрия пересекается с геометрией водоёма"));
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

                        tbRev.flLength.RenderCustom(re.Panel, re.Env, model.flLength);
                        tbRev.flWidth.RenderCustom(re.Panel, re.Env, model.flWidth);
                        tbRev.flArea.RenderCustom(re.Panel, re.Env, model.flArea);
                        tbRev.flMaxDepth.RenderCustom(re.Panel, re.Env, model.flMaxDepth);
                        tbRev.flAvgDepth.RenderCustom(re.Panel, re.Env, model.flAvgDepth);

                    })
                    .OnValidating(ve => {
                        var tbRev = new TbObjectsRevisions();
                        var checkFields = new Field[] {
                            tbRev.flLength,
                            tbRev.flWidth,
                            tbRev.flArea,
                            tbRev.flMaxDepth,
                            tbRev.flAvgDepth
                        };
                        checkFields.Each(f => f.Validate(ve.Env));
                    })
                    .OnProcessing(pe => {
                        var tbRev = new TbObjectsRevisions();

                        pe.Model.setPhysicalCharacteristic(
                            tbRev.flLength.GetVal(pe.Env),
                            tbRev.flWidth.GetVal(pe.Env),
                            tbRev.flArea.GetVal(pe.Env),
                            tbRev.flMaxDepth.GetVal(pe.Env),
                            tbRev.flAvgDepth.GetVal(pe.Env)
                        );

                    })
                )
                .Step("Биологическая характеристика", step => step
                    .Enabled(env => env.Args.OrderType.In(ObjectsOrderTypes.Create, ObjectsOrderTypes.Edit))
                    .OnRendering(re => {
                        var tbRev = new TbObjectsRevisions();
                        var refFauna = new RefFaunaDetailed();
                        var tbObjFlds = new TbObjectFields();
                        var model = re.Model;

                        tbRev.flEmergentVegetation.RenderCustom(re.Panel, re.Env, model.flEmergentVegetation);
                        tbRev.flEmergentVegetationValue.RenderCustom(re.Panel, re.Env, model.flEmergentVegetationValue);
                        tbRev.flUnderwaterVegetation.RenderCustom(re.Panel, re.Env, model.flUnderwaterVegetation);
                        tbRev.flUnderwaterVegetationValue.RenderCustom(re.Panel, re.Env, model.flUnderwaterVegetationValue);
                        tbRev.flPhytoplankton.RenderCustom(re.Panel, re.Env, model.flPhytoplankton);
                        tbRev.flPhytoplanktonValue.RenderCustom(re.Panel, re.Env, model.flPhytoplanktonValue);

                        if (model.flFauna == null)
                        {
                            model.flFauna = new string[] { };
                            model.flСommercialFauna = new string[] { };
                        }

                        tbObjFlds.flFaunaSearch.RenderCustom(re.Panel, re.Env, JsonConvert.SerializeObject(model.flFauna.Select(x => new FaunaSearchItem() { SearchItemId = x, SearchItemText = $"{refFauna.Search(x).Parent.Text} - {refFauna.Search(x).Text}" })));
                        tbObjFlds.flСommercialFaunaSearch.RenderCustom(re.Panel, re.Env, JsonConvert.SerializeObject(model.flСommercialFauna.Select(x => new FaunaSearchItem() { SearchItemId = x, SearchItemText = $"{refFauna.Search(x).Parent.Text} - {refFauna.Search(x).Text}" })));

                        tbRev.flFishProductivity.RenderCustom(re.Panel, re.Env, model.flFishProductivity);

                    })
                    .OnValidating(ve => {
                        var tbRev = new TbObjectsRevisions();
                        var tbObjFlds = new TbObjectFields();

                        var checkFields = new Field[] {
                            tbRev.flEmergentVegetation,
                            tbRev.flEmergentVegetationValue,
                            tbRev.flUnderwaterVegetation,
                            tbRev.flUnderwaterVegetationValue,
                            tbRev.flPhytoplankton,
                            tbRev.flPhytoplanktonValue,
                            tbObjFlds.flFaunaSearch,
                            tbObjFlds.flСommercialFaunaSearch,
                            tbRev.flFishProductivity
                        };
                        checkFields.Each(f => f.Validate(ve.Env));
                    })
                    .OnProcessing(pe => {
                        var tbRev = new TbObjectsRevisions();
                        var tbObjFlds = new TbObjectFields();

                        var flFauna = tbObjFlds.flFaunaSearch.GetVal(pe.Env).Select(x => x.SearchItemId.ToString()).ToArray();
                        var flСommercialFauna = tbObjFlds.flСommercialFaunaSearch.GetVal(pe.Env).Select(x => x.SearchItemId.ToString()).ToArray();
                        var flFishProductivity = (Dictionary<string,object>[])tbRev.flFishProductivity.GetValue(pe.Env);

                        pe.Model.setBiologicalCharacteristic(
                            tbRev.flEmergentVegetation.GetVal(pe.Env),
                            tbRev.flEmergentVegetationValue.GetVal(pe.Env),
                            tbRev.flUnderwaterVegetation.GetVal(pe.Env),
                            tbRev.flUnderwaterVegetationValue.GetVal(pe.Env),
                            tbRev.flPhytoplankton.GetVal(pe.Env),
                            tbRev.flPhytoplanktonValue.GetVal(pe.Env),
                            flFauna,
                            flСommercialFauna,
                            flFishProductivity
                        );

                    })
                )
                .Step("Хозяйственная характеристика", step => step
                    .Enabled(env => env.Args.OrderType.In(ObjectsOrderTypes.Create, ObjectsOrderTypes.Edit))
                    .OnRendering(re => {
                        var tbRev = new TbObjectsRevisions();
                        var model = re.Model;

                        tbRev.flPollution.RenderCustom(re.Panel, re.Env, model.flPollution);
                        tbRev.flHasWaterIntakes.RenderCustomT(re.Panel, re.Env, model.flHasWaterIntakes);
                        tbRev.flWaterIntakes.RenderCustom(re.Panel, re.Env, model.flWaterIntakes);
                        tbRev.flPermanentUsingPlacesCount.RenderCustom(re.Panel, re.Env, model.flPermanentUsingPlacesCount);
                        tbRev.flAnotherInformation.RenderCustom(re.Panel, re.Env, model.flAnotherInformation);

                    })
                    .OnValidating(ve => {
                        var tbRev = new TbObjectsRevisions();
                        var checkFields = new Field[] {
                            tbRev.flPollution,
                            tbRev.flHasWaterIntakes,
                            tbRev.flWaterIntakes,
                            tbRev.flPermanentUsingPlacesCount,
                            tbRev.flAnotherInformation
                        };
                        checkFields.Each(f => f.Validate(ve.Env));
                    })
                    .OnProcessing(pe => {
                        var tbRev = new TbObjectsRevisions();

                        pe.Model.setFarmicCharacteristic(
                            tbRev.flPollution.GetVal(pe.Env),
                            tbRev.flHasWaterIntakes.GetVal(pe.Env),
                            (Dictionary<string, object>[])tbRev.flWaterIntakes.GetValue(pe.Env),
                            tbRev.flPermanentUsingPlacesCount.GetVal(pe.Env),
                            tbRev.flAnotherInformation.GetVal(pe.Env)
                        );

                    })
                )
                ;
        }


        public override FishingObjectModel GetModel(ActionEnv<ObjectOrderQueryArgs> env, GetModelFrom from, ObjectsOrderTypes orderType, ITransaction transaction) {
            var model = from switch {
                GetModelFrom.Empty => new FishingObjectModel() {
                    flSallerBin = env.User.GetUserBin(env.QueryExecuter),
                    flSallerId = -1,
                    flStatus = FishingObjectStatuses.Active.ToString(),
                    flBlock = FishingObjectBlocks.ActiveFree.ToString()
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

            if (from == GetModelFrom.Empty) {
                switch (orderType) {
                    case ObjectsOrderTypes.Create:
                        model.flStatus = FishingObjectStatuses.Active.ToString();
                        model.flBlock = FishingObjectBlocks.ActiveFree.ToString();
                        break;
                }
            }
            if(from == GetModelFrom.Entity) {
                switch (orderType) {
                    case ObjectsOrderTypes.Remove:
                        model.flStatus = FishingObjectStatuses.Deleted.ToString();
                        model.flBlock = FishingObjectBlocks.DeletedDisqualified.ToString();
                        break;
                }
            }

            return model;
        }

        public override ObjectsOrderTypes GetOrderType(ActionEnv<ObjectOrderQueryArgs> env, GetOrderModelFrom orderModelFrom) {
            return orderModelFrom switch {
                GetOrderModelFrom.Empty => ObjectsOrderTypes.Create,
                _ => env.Args.OrderType switch {
                    ObjectsOrderTypeActions.Remove => ObjectsOrderTypes.Remove,
                    ObjectsOrderTypeActions.Edit => ObjectsOrderTypes.Edit,
                    _ => throw new ArgumentException($"OrderType: {env.Args.OrderType}")
                }
            };
        }

        public override string GetWizardCancelRedirectUrl(ActionEnv<ObjectOrderQueryArgs> env, ObjectsOrderTypes orderType, GetModelFrom modelFrom) {
            var urlHelper = env.RequestContext.GetUrlHelper();
            //TODO
            return modelFrom switch {
                GetModelFrom.Empty => urlHelper.YodaAction(ModuleName, nameof(MnuFishingObjectOrderBase)),
                GetModelFrom.Entity => urlHelper.YodaAction(ModuleName, nameof(MnuFishingObjectOrderBase), new ObjectOrderQueryArgs { Id = env.Args.Id.Value, MenuAction = "view" }),
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

            if (!CanCreateOrder(env.Args.Id.Value, GetOrderType(env, GetOrderModelFrom.Entity), out var error, env.QueryExecuter))
            {
                if (env.Args.Id.HasValue) {
                    return new RedirectResult(ModuleName, MnuFishingObjectView.MnuName, new FishingObjectViewArgs() { Id = env.Args.Id.Value, MenuAction = MnuFishingObjectView.Actions.View }, error);
                }
                else {
                    return new RedirectResult(ModuleName, "ActionError", null, error);
                }
            }

            return new OkResult();
        }

        public static bool TryGetCurrentInWorkOrderRevId(int id, out int revId, IQueryExecuter queryExecuter) {
            var ret = new TbObjectsOrderResult()
                .AddFilterNotFinishedOrders()
                .JoinT("tbOrders", new TbObjectsRevisions().AddFilter(t => t.flId, id), "tbRevs")
                .On((t1, t2) => new Join(t1.flSubjectId, t2.flRevisionId))
                .SelectScalar(t => t.L.flSubjectId, queryExecuter);

            if (ret == null) {
                revId = 0;
                return false;
            }

            revId = ret.Value;
            return true;
        }

        public static bool CanCreateOrder(int id, ObjectsOrderTypes orderType, out string error, IQueryExecuter queryExecuter) {
            if (TryGetCurrentInWorkOrderRevId(id, out _, queryExecuter)) {
                error = "Для данного объекта имеется не исполненый приказ";
                return false;
            }

            var tradeResourceObject = ObjectHelper.GetObjectModel(id, queryExecuter);

            bool checkFieldActive(out string err) {
                if (tradeResourceObject.flStatus != FishingObjectStatuses.Active.ToString()) {
                    err = "Изменение разрешено только для активного объекта";
                    return false;
                }
                err = null;
                return true;
            }
            bool checkActive(out string err) {
                if (tradeResourceObject.flStatus == FishingObjectStatuses.Deleted.ToString()) {
                    err = "Операция невозможна, так как объект не активен";
                    return false;
                }
                err = null;
                return true;
            }

            switch (orderType) {
                case ObjectsOrderTypes.Create:
                    error = "Недопустимый тип приказа";
                    return false;
                case ObjectsOrderTypes.Edit:
                    return checkFieldActive(out error) && checkActive(out error);
                case ObjectsOrderTypes.Remove:
                    if (!checkFieldActive(out error)) {
                        return false;
                    }

                    if (tradeResourceObject.flStatus == FishingObjectStatuses.Deleted.ToString()) {
                        error = "Удаление разрешено только при неактивном земельном участке";
                        return false;
                    }
                    error = null;
                    return true;
                default:
                    throw new NotImplementedException($"ObjectsOrderTypes: {orderType}");
            }
        }

        public override void SaveModel(int revisionId, FishingObjectModel model, ObjectsOrderTypes orderType, ActionEnv<ObjectOrderQueryArgs> env, ITransaction transaction, GetModelFrom modelFrom) {
            var tbObjRevs = new TbObjectsRevisions();

            if (modelFrom == GetModelFrom.Empty) {
                model.flId = new TbObjects().flId.GetNextId(env.QueryExecuter, transaction);
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

        public static void SetModel<TObjectsTable>(DataModifingQueryProxy<TObjectsTable> updateOrInsert, FishingObjectModel model) where TObjectsTable : TbObjectsBase {
            FishingObjectStatuses status = model.flStatus == FishingObjectStatuses.Active.ToString() ? FishingObjectStatuses.Active : FishingObjectStatuses.Deleted;

            var geometry = new GeomFromWKT(model.flWKT, 4326);
            var point = new PointFromGeom(new SqlExpVal(geometry));

            updateOrInsert
                .SetT(t => t.flId, model.flId)
                .SetT(t => t.flRevisionId, model.flRevisionId)

                .SetT(t => t.flReservoirId, model.flReservoirId)

                .SetT(t => t.flSallerId, model.flSallerId)
                .SetT(t => t.flSallerBin, model.flSallerBin)
                .Set(t => t.flStatus, model.flStatus)
                .Set(t => t.flBlock, model.flBlock)

                .Set(t => t.flName, model.flName)
                .Set(t => t.flDescription, model.flDescription)
                .Set(t => t.flNote, model.flNote)
                .SetT(t => t.flDocs, JsonConvert.SerializeObject(model.flDocs))

                .Set(t => t.flCountry, model.flCountry)
                .Set(t => t.flRegion, model.flRegion)
                .Set(t => t.flDistrict, model.flDistrict)
                .Set(t => t.flLocation, model.flLocation)
                .SetT(t => t.flPasportCoords, model.flPasportCoords)
                .Set(t => t.flWKT, model.flWKT)
                .SetT(t => t.flCoords, model.flCoords)
                .Set(t => t.flGeometry, geometry)
                .Set(t => t.flPoint,  point)

                .Set(t => t.flLength, model.flLength)
                .Set(t => t.flWidth, model.flWidth)
                .Set(t => t.flArea, model.flArea)
                .Set(t => t.flMaxDepth, model.flMaxDepth)
                .Set(t => t.flAvgDepth, model.flAvgDepth)

                .Set(t => t.flEmergentVegetation, model.flEmergentVegetation)
                .Set(t => t.flEmergentVegetationValue, model.flEmergentVegetationValue)
                .Set(t => t.flUnderwaterVegetation, model.flUnderwaterVegetation)
                .Set(t => t.flUnderwaterVegetationValue, model.flUnderwaterVegetationValue)
                .Set(t => t.flPhytoplankton, model.flPhytoplankton)
                .Set(t => t.flPhytoplanktonValue, model.flPhytoplanktonValue)
                .SetT(t => t.flFauna, model.flFauna)
                .SetT(t => t.flСommercialFauna, model.flСommercialFauna)
                .SetT(t => t.flFishProductivity, model.flFishProductivity)

                .Set(t => t.flPollution, model.flPollution)
                .SetT(t => t.flWaterIntakes, model.flWaterIntakes)
                .Set(t => t.flHasWaterIntakes, model.flHasWaterIntakes)
                .Set(t => t.flPermanentUsingPlacesCount, model.flPermanentUsingPlacesCount)
                .Set(t => t.flAnotherInformation, model.flAnotherInformation)
                ;
        }

        public override bool TryAcceptModel(int revisionId, FishingObjectModel model, ObjectsOrderTypes orderType, ActionEnv<ObjectOrderQueryArgs> env, ITransaction transaction, out string error) {
            if (orderType == ObjectsOrderTypes.Create) {

            }

            var updateOrInsert = orderType switch {
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

        public override void ViewModel(RenderActionEnv<ObjectOrderQueryArgs> env, FishingObjectModel model, ObjectsOrderTypes orderType, OrderStatus orderStatus) {
            ViewModel(env.Form, env, model);
        }

        public static void ViewModel(WidgetBase widget, FormEnvironment env, FishingObjectModel model) {
            env.RequestContext.Title = "Объект";
            viewModelMainData(widget, env, model);
            viewModelGeoPositionData(widget, env, model);
            viewModelGeometryData(widget, env, model);
            viewModelPhysicalCharacteristicData(widget, env, model);
            viewModelBiologicalCharacteristicData(widget, env, model);
            viewModelFarmicCharacteristicData(widget, env, model);
        }

        public static void viewModelMainData(WidgetBase widget, FormEnvironment env, FishingObjectModel model, bool collapsed = false) {
            var tbRevs = new TbObjectsRevisions();
            var groupbox = new Accordion(env.T("Основные данные объекта"), collapsed);
            widget.AddComponent(groupbox);

            tbRevs.flId.RenderCustom(groupbox, env, model.flId.ToString() ?? "*Генерируется автоматически при исполнении приказа", readOnly: true);
            tbRevs.flRevisionId.RenderCustom(groupbox, env, model.flRevisionId, readOnly: true);
            tbRevs.flSallerBin.RenderCustom(groupbox, env, model.flSallerBin, readOnly: true);
            tbRevs.flStatus.RenderCustom(groupbox, env, model.flStatus, readOnly: true);
            tbRevs.flBlock.RenderCustom(groupbox, env, model.flBlock, readOnly: true);
            tbRevs.flName.RenderCustom(groupbox, env, model.flName, readOnly: true);
            tbRevs.flDescription.RenderCustom(groupbox, env, model.flDescription, readOnly: true);
            tbRevs.flNote.RenderCustom(groupbox, env, model.flNote, readOnly: true);
            tbRevs.flDocs.RenderCustom(groupbox, env, JsonConvert.SerializeObject(model.flDocs ?? new FileInfo[] { }), readOnly: true);
        }

        public static void viewModelGeoPositionData(WidgetBase widget, FormEnvironment env, FishingObjectModel model, bool collapsed = false) {
            var tbRevs = new TbObjectsRevisions();
            var groupbox = new Accordion(env.T("Географическое расположение"), collapsed);
            widget.AddComponent(groupbox);

            tbRevs.flCountry.RenderCustom(groupbox, env, model.flCountry, readOnly: true);
            tbRevs.flRegion.RenderCustom(groupbox, env, model.flRegion, readOnly: true);
            tbRevs.flDistrict.RenderCustom(groupbox, env, model.flDistrict, readOnly: true);
            tbRevs.flLocation.RenderCustom(groupbox, env, model.flLocation, readOnly: true);
            tbRevs.flPasportCoords.RenderCustom(groupbox, env, model.flPasportCoords, readOnly: true);
        }

        public static void viewModelGeometryData(WidgetBase widget, FormEnvironment env, FishingObjectModel model, bool collapsed = false)
        {
            var groupbox = new Accordion(env.T("Геометрия"), collapsed);
            widget.AddComponent(groupbox);
            var row = new GridRow();
            groupbox.AddComponent(row);

            row.AddComponent(new ObjectGeometryViewerComponent(model.flWKT, "col-md-6"));
            row.AddComponent(new ObjectCoordsViewerComponent(model.flCoords, "col-md-6"));
        }

        public static void viewModelPhysicalCharacteristicData(WidgetBase widget, FormEnvironment env, FishingObjectModel model, bool collapsed = false) {
            var tbRevs = new TbObjectsRevisions();
            var groupbox = new Accordion(env.T("Физическая характеристика"), collapsed);
            widget.AddComponent(groupbox);

            tbRevs.flLength.RenderCustom(groupbox, env, model.flLength, readOnly: true);
            tbRevs.flWidth.RenderCustom(groupbox, env, model.flWidth, readOnly: true);
            tbRevs.flArea.RenderCustom(groupbox, env, model.flArea, readOnly: true);
            tbRevs.flMaxDepth.RenderCustom(groupbox, env, model.flMaxDepth, readOnly: true);
            tbRevs.flAvgDepth.RenderCustom(groupbox, env, model.flAvgDepth, readOnly: true);
        }

        public static void viewModelBiologicalCharacteristicData(WidgetBase widget, FormEnvironment env, FishingObjectModel model, bool collapsed = false)
        {
            var refFauna = new RefFaunaDetailed();
            var tbRevs = new TbObjectsRevisions();
            var groupbox = new Accordion(env.T("Биологическая характеристика"), collapsed);
            widget.AddComponent(groupbox);

            tbRevs.flEmergentVegetation.RenderCustom(groupbox, env, model.flEmergentVegetation, readOnly: true);
            tbRevs.flEmergentVegetationValue.RenderCustom(groupbox, env, model.flEmergentVegetationValue, readOnly: true);
            tbRevs.flUnderwaterVegetation.RenderCustom(groupbox, env, model.flUnderwaterVegetation, readOnly: true);
            tbRevs.flUnderwaterVegetationValue.RenderCustom(groupbox, env, model.flUnderwaterVegetationValue, readOnly: true);
            tbRevs.flPhytoplankton.RenderCustom(groupbox, env, model.flPhytoplankton, readOnly: true);
            tbRevs.flPhytoplanktonValue.RenderCustom(groupbox, env, model.flPhytoplanktonValue, readOnly: true);
            tbRevs.flFauna.RenderCustom(groupbox, env, ReferenceHelper.GetGroupedString(refFauna, model.flFauna), readOnly: true);
            tbRevs.flСommercialFauna.RenderCustom(groupbox, env, ReferenceHelper.GetGroupedString(refFauna, model.flСommercialFauna), readOnly: true);
            tbRevs.flFishProductivity.RenderCustom(groupbox, env, model.flFishProductivity, readOnly: true);
        }

        public static void viewModelFarmicCharacteristicData(WidgetBase widget, FormEnvironment env, FishingObjectModel model, bool collapsed = false)
        {
            var tbRevs = new TbObjectsRevisions();
            var groupbox = new Accordion(env.T("Хозяйственная характеристика"), collapsed);
            widget.AddComponent(groupbox);

            tbRevs.flPollution.RenderCustom(groupbox, env, model.flPollution, readOnly: true);
            tbRevs.flHasWaterIntakes.RenderCustom(groupbox, env, model.flHasWaterIntakes, readOnly: true);
            tbRevs.flWaterIntakes.RenderCustom(groupbox, env, model.flWaterIntakes, readOnly: true);
            tbRevs.flPermanentUsingPlacesCount.RenderCustom(groupbox, env, model.flPermanentUsingPlacesCount, readOnly: true);
            tbRevs.flAnotherInformation.RenderCustom(groupbox, env, model.flAnotherInformation, readOnly: true);
        }

        protected override bool HasAccessToOrder(int revisionId, ActionEnv<ObjectOrderQueryArgs> env) {
            if ((!env.User.IsExternalUser() && !env.User.IsGuest())) {
                return true;
            }
            if (env.Args.MenuAction != "view-order" && !env.User.HasRole("TRADERESOURCES-Рыбохозяйственные водоёмы-Создание приказов", env.QueryExecuter)/*!env.User.HasCustomRole("fishingobjects", "dataEdit", env.QueryExecuter)*/)
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
