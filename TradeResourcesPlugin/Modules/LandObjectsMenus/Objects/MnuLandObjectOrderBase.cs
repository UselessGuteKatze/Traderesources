using FileStoreInterfaces;
using CommonSource;
using LandSource.Helpers;
using LandSource.Models;
using LandSource.QueryTables.Applications;
using LandSource.QueryTables.LandObject;
using LandSource.QueryTables.Trades;
using LandSource.References.LandObject;
using LandSource.References.Trades;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TradeResourcesPlugin.Helpers;
using TradeResourcesPlugin.Modules.Components;
using UsersResources;
using Yoda.Interfaces;
using Yoda.Interfaces.Forms;
using Yoda.Interfaces.Forms.Components;
using Yoda.Interfaces.Helpers;
using Yoda.YodaReferences;
using YodaApp.YodaHelpers.OrderHelpers;
using YodaHelpers.ActionMenus;
using YodaHelpers.Components;
using YodaHelpers.Fields;
using YodaHelpers.OrderHelpers;
using YodaQuery;
using YodaQuery.Yoda.Query.Expressions;

namespace TradeResourcesPlugin.Modules.LandObjectsMenus.Object {
    public class MnuLandObjectOrderBase : MnuOrderBaseV2<LandObjectModel, LandObjectsOrderTypes, LandObjectOrderQueryArgs> {

        public MnuLandObjectOrderBase(string moduleName, string menuName, OrderStandartPermissions perms)
            : base(moduleName, menuName, "Приказ по земельным ресурсам", perms, () => new TbLandObjectsOrderResult(), () => new TbLandObjectsOrderNegotiations()) {
            AsCallback();
            Enabled(rc => {
                var xin = rc.User.GetUserXin(rc.QueryExecuter);
                if (xin == "050540004455"
                || xin == "050540000002" 
                || (!rc.User.IsExternalUser() && !rc.User.IsGuest())
                || rc.User.HasRole("TRADERESOURCES-Земельные ресурсы-Создание приказов", rc.QueryExecuter)/*rc.User.HasCustomRole("landobjects", "appLandEdit", rc.QueryExecuter)*/) {
                    return true;
                }
                return false;
            });
        }

        public override Task<ActionItem[]> GetActions(ActionEnv<LandObjectOrderQueryArgs> env) {
            var ret = base.GetActions(env).Result.ToList();

            switch (env.Args.MenuAction) {
                case Actions.ViewOrder:
                    //var obj = new TbLandObjectsRevisions().AddFilter(t => t.flRevisionId, env.Args.RevisionId)
                    //    .GetObjectModelFirst(env.QueryExecuter, null);
                    //var objectId = obj.Id;

                    //var objActRev = new TbLandObjects().AddFilter(t => t.flRevisionId, env.Args.RevisionId).SelectFirstOrDefault(env.QueryExecuter);

                    //if (objActRev != null && env.RequestContext.Project == "cabinetResourceSeller" && obj.Status != LandObjectStatuses.Deleted.ToString()) {
                        //ret.Add(new ActionItem("Приказ на корректировку", Actions.CreateFrom, new LandObjectOrderQueryArgs {
                        //    Id = objectId,
                        //    RevisionId = env.Args.RevisionId,
                        //    MenuAction = Actions.CreateFrom,
                        //    OrderType = LandObjectsOrderTypeActions.Edit
                        //}));
                        //ret.Add(new ActionItem("Удалить", Actions.CreateFrom, new LandObjectOrderQueryArgs { Id = objectId, RevisionId = env.Args.RevisionId, MenuAction = Actions.CreateFrom, OrderType = LandObjectsOrderTypeActions.Remove }));
                    //}

                    //if (objActRev != null && env.RequestContext.Project == "cabinetResourceSeller" && obj.Status != LandObjectStatuses.Deleted.ToString() && obj.Block == LandObjectBlocks.ActiveFree.ToString()) {
                    //    ret.Add(new ActionItem(env.T("Создать торги"), new ActionRedirectData(ModuleName, nameof(MnuTradesOrderBaseV2), new TradeOrderQueryArgs { MenuAction = "create-new", ObjectId = objectId, RevisionId = -1 })));
                    //}

                    break;
                case Actions.CreateFrom:
                    break;
            }

            return Task.FromResult(ret.ToArray());
        }

        public override IWizardFormBuilderWithStep<LandObjectOrderQueryArgs, ModelOrderArgs<LandObjectsOrderTypes, LandObjectOrderQueryArgs>, LandObjectModel>
            EditModel(IWizardFormBuilderWithFinishButton<LandObjectOrderQueryArgs, ModelOrderArgs<LandObjectsOrderTypes, LandObjectOrderQueryArgs>, LandObjectModel> wizard) {
            return wizard
                .Step("Удаление", step => step
                    .Enabled(env => env.Args.OrderType.In(LandObjectsOrderTypes.Remove))
                    .OnRendering(re => {
                        var tbRev = new TbLandObjectsRevisions();
                        var model = re.Model;

                        tbRev.flId.RenderCustom(re.Panel, re.Env, model.Id, readOnly: true);
                        tbRev.flStatus.RenderCustom(re.Panel, re.Env, LandObjectStatuses.Deleted.ToString(), cssClass: "hidden");
                        tbRev.flStatus.RenderCustom(re.Panel, re.Env, LandObjectStatuses.Deleted.ToString(), readOnly: true);
                        tbRev.flBlock.RenderCustom(re.Panel, re.Env, null);
                    })
                    .OnValidating(ve => {
                        var tbRev = new TbLandObjectsRevisions();
                        var model = ve.Model;

                        var tbTrades = new TbLandObjectsTrades();
                        tbTrades.AddFilter(tbTrades.flStatus, RefLandObjectTradeStatuses.Wait);
                        var tbObjects = new TbLandObjects();
                        tbObjects.AddFilter(tbObjects.flId, model.Id);

                        var join = tbTrades
                            .Join(tbTrades.Name, tbObjects, tbObjects.Name, JoinType.Inner)
                            .On(new Condition(tbTrades.flObjectId, tbObjects.flId));

                        var hasWaitTrades = join.Select(new FieldAlias[] { tbTrades.flId},ve.Env.QueryExecuter).Rows.Count > 0;

                        if (hasWaitTrades) {
                            ve.Env.AddError(tbRev.flId.FieldName, ve.Env.T("Данный объект задействован в ожидающихся торгах."));
                        }

                    })
                    .OnProcessing(pe => {
                        var tbRev = new TbLandObjectsRevisions();
                        pe.Model.Status = LandObjectStatuses.Deleted.ToString();
                        pe.Model.Block = tbRev.flBlock.GetVal(pe.Env);
                    })
                )
                .Step("Изменение продавца", step => step
                    .Enabled(env => env.Args.OrderType.In(LandObjectsOrderTypes.EditOwner))
                    .OnRendering(re => {
                        var tbRev = new TbLandObjectsRevisions();
                        var model = re.Model;

                        tbRev.flSallerBin.RenderCustom(re.Panel, re.Env, model.SallerBin);
                    })
                    .OnValidating(ve => {
                        var tbRev = new TbLandObjectsRevisions();
                        var model = ve.Model;

                        var checkFields = new Field[] {
                            tbRev.flSallerBin
                        };
                        checkFields.Each(f => f.Validate(ve.Env));

                    })
                    .OnProcessing(pe => {
                        var tbRev = new TbLandObjectsRevisions();
                        pe.Model.SallerBin = tbRev.flSallerBin.GetVal(pe.Env);
                    })
                )
                .Step("Основные данные", step => step
                    .Enabled(env => env.Args.OrderType.In(LandObjectsOrderTypes.Create, LandObjectsOrderTypes.Edit))
                    .OnRendering(re => {
                        var tbRev = new TbLandObjectsRevisions();
                        var model = re.Model;
                        int? objectId = null;
                        if (model.Id != 0) {
                            objectId = model.Id;
                        }

                        tbRev.flSallerBin.RenderCustom(re.Panel, re.Env, model.SallerBin, readOnly: true);
                        tbRev.flStatus.RenderCustom(re.Panel, re.Env, model.Status, readOnly: true);
                        tbRev.flBlock.RenderCustom(re.Panel, re.Env, model.Block.ToString(), readOnly: true);
                        tbRev.flType.RenderCustom(re.Panel, re.Env, model.Type);
                        tbRev.flKfsLevel1.RenderCustom(re.Panel, re.Env, model.KfsLevel1);
                        tbRev.flKfsLevel2.RenderCustom(re.Panel, re.Env, model.KfsLevel2);
                        tbRev.flKfsLevel3.RenderCustom(re.Panel, re.Env, model.KfsLevel3);
                        tbRev.flKfsLevel4.RenderCustom(re.Panel, re.Env, model.KfsLevel4);
                        tbRev.flKfsLevel5.RenderCustom(re.Panel, re.Env, model.KfsLevel5);

                        tbRev.flConsolidatedStatementDateTime.RenderCustom(re.Panel, re.Env, model.ConsolidatedStatementDateTime.HasValue ? model.ConsolidatedStatementDateTime.Value : DateTime.Now);
                        tbRev.flConsolidatedStatementReconciliationDateTime.RenderCustom(re.Panel, re.Env, model.ConsolidatedStatementReconciliationDateTime.HasValue ? model.ConsolidatedStatementReconciliationDateTime.Value : DateTime.Now);

                        var refField = new RefLandObjectDataSources.Ref();
                        refField.flFieldLandObjectDataSources.RenderCustom(re.Panel, re.Env, RefLandObjectDataSources.Values.GZK);

                        var refField2 = new RefLandObjectDrawGeom.Ref();
                        refField2.flFieldLandObjectDrawGeom.RenderCustom(re.Panel, re.Env, RefLandObjectDrawGeom.Values.DONTDRAW);

                        tbRev.flKadNumber.RenderCustom(re.Panel, re.Env, model.KadNumber);
                        tbRev.flEgknId.RenderCustom(re.Panel, re.Env, model.EgknId);
                    })
                    .OnValidating(ve => {
                        var tbRev = new TbLandObjectsRevisions();
                        var checkFields = new Field[] {
                            tbRev.flType,
                            tbRev.flKfsLevel1,
                            tbRev.flKfsLevel2,
                            tbRev.flKfsLevel3,
                            tbRev.flKfsLevel4,
                            tbRev.flKfsLevel5,
                            tbRev.flConsolidatedStatementDateTime,
                            tbRev.flConsolidatedStatementReconciliationDateTime,
                            tbRev.flKadNumber,
                            tbRev.flEgknId,
                        };
                        checkFields.Each(f => f.Validate(ve.Env));

                        if (ve.Env.IsValid)
                        {
                            var refField = new RefLandObjectDataSources.Ref();
                            refField.flFieldLandObjectDataSources.Validate(ve.Env);
                            var mainSource = refField.flFieldLandObjectDataSources.GetVal(ve.Env);

                            var refField2 = new RefLandObjectDrawGeom.Ref();
                            refField2.flFieldLandObjectDrawGeom.Validate(ve.Env);
                            var redrawGeom = refField2.flFieldLandObjectDrawGeom.GetVal(ve.Env);

                            if (!string.IsNullOrEmpty(tbRev.flKadNumber.GetVal(ve.Env)) && redrawGeom == RefLandObjectDrawGeom.Values.RN)
                            {
                                ve.Model.WKT = null;
                                ve.Model.Coords = null;

                                var retWkt = GzkSiteGeomRequestor.GetLandGeomWkt4326(tbRev.flKadNumber.GetVal(ve.Env), out var successResult, out var gzkSiteStatusCode, out var gzkSiteStatusText);
                                var RnKadData = MnuGetRnKadData.GetRnKadData((tbRev.flKadNumber.GetVal(ve.Env) + string.Empty).Replace("-", ":"), ve.Env.RequestContext);
                                var GzkKadData = MnuGetGzkKadData.GetGzkKadData((tbRev.flKadNumber.GetVal(ve.Env) + string.Empty), ve.Env.RequestContext);

                                MnuGetRnKadData.SetRnDataToModel(ve.Model, RnKadData);
                                MnuGetGzkKadData.SetGzkDataToModel(ve.Model, GzkKadData);

                                if (mainSource == RefLandObjectDataSources.Values.GZK)
                                {
                                    MnuGetGzkKadData.SetGzkDataToModel(ve.Model, GzkKadData);
                                }
                                else
                                {
                                    MnuGetRnKadData.SetRnDataToModel(ve.Model, RnKadData);
                                }

                                if (successResult && !string.IsNullOrEmpty(retWkt))
                                {
                                    ve.Model.WKT = retWkt;
                                }

                            }

                            if (redrawGeom == RefLandObjectDrawGeom.Values.DRAW)
                            {
                                ve.Model.WKT = null;
                                ve.Model.Coords = null;
                            }
                        }
                    })
                    .OnProcessing(pe => {
                        var tbRev = new TbLandObjectsRevisions();
                        pe.Model.Type = tbRev.flType.GetVal(pe.Env);
                        pe.Model.KfsLevel1 = tbRev.flKfsLevel1.GetVal(pe.Env);
                        pe.Model.KfsLevel2 = tbRev.flKfsLevel2.GetVal(pe.Env);
                        pe.Model.KfsLevel3 = tbRev.flKfsLevel3.GetVal(pe.Env);
                        pe.Model.KfsLevel4 = tbRev.flKfsLevel4.GetVal(pe.Env);
                        pe.Model.KfsLevel5 = tbRev.flKfsLevel5.GetVal(pe.Env);
                        pe.Model.ConsolidatedStatementDateTime = tbRev.flConsolidatedStatementDateTime.GetVal(pe.Env);
                        pe.Model.ConsolidatedStatementReconciliationDateTime = tbRev.flConsolidatedStatementReconciliationDateTime.GetVal(pe.Env);
                        pe.Model.KadNumber = tbRev.flKadNumber.GetVal(pe.Env);
                        pe.Model.EgknId = tbRev.flEgknId.GetVal(pe.Env);
                    })
                )
                .Step("Геометрия", step => step
                    .Enabled(env => env.Args.OrderType.In(LandObjectsOrderTypes.Create, LandObjectsOrderTypes.Edit) && string.IsNullOrEmpty(env.Model.WKT))
                    .OnRendering(re => {
                        var tbRev = new TbLandObjectsRevisions();
                        var model = re.Model;

                        re.Panel.AddComponent(new Textbox("flWKT", cssClass: "d-none"));
                        re.Panel.AddComponent(new Textbox("flCoords", cssClass: "d-none"));

                        re.Panel.AddComponent(new ObjectDrawByCoordsComponent("flWKT", "flCoords"));

                    })
                    .OnValidating(ve => {
                        var tbRev = new TbLandObjectsRevisions();
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
                        }
                    })
                    .OnProcessing(pe => {
                        var tbRev = new TbLandObjectsRevisions();

                        pe.Model.WKT = tbRev.flWKT.GetVal(pe.Env);
                        pe.Model.Coords = tbRev.flCoords.GetVal(pe.Env);
                    })
                )
                .Step("Дополнительные данные", step => step
                    .Enabled(env => env.Args.OrderType.In(LandObjectsOrderTypes.Create, LandObjectsOrderTypes.Edit))
                    .OnRendering(re => {
                        var tbRev = new TbLandObjectsRevisions();
                        var model = re.Model;

                        if (model.Coords == null)
                        {
                            re.Panel.AddComponent(new Label("Геометрия ЗУ успешно загружена", "h6 alert-success center"));
                        }

                        tbRev.flBalanceHolderBin.RenderCustom(re.Panel, re.Env, model.BalanceHolderBin);
                        tbRev.flName.RenderCustom(re.Panel, re.Env, model.Name);
                        tbRev.flDescription.RenderCustom(re.Panel, re.Env, model.Description);

                        if (model.Type != LandObjectTypes.Agricultural.ToString())
                        {
                            tbRev.flCost.RenderCustom(re.Panel, re.Env, model.Cost);
                        }

                        tbRev.flTaxSum.RenderCustom(re.Panel, re.Env, model.flTaxSum);
                        tbRev.flRentSum.RenderCustom(re.Panel, re.Env, model.flRentSum);

                        tbRev.flLandCategory.RenderCustom(re.Panel, re.Env, model.LandCategory);

                        tbRev.flLandArea.RenderCustom(re.Panel, re.Env, model.LandArea);
                        tbRev.flFuncPurpose1.RenderCustom(re.Panel, re.Env, model.FuncPurpose1);
                        tbRev.flFuncPurpose2.RenderCustom(re.Panel, re.Env, model.FuncPurpose2);
                        tbRev.flFuncPurpose3.RenderCustom(re.Panel, re.Env, model.FuncPurpose3);
                        tbRev.flFuncPurpose4.RenderCustom(re.Panel, re.Env, model.FuncPurpose4);
                        tbRev.flUsageAim.RenderCustom(re.Panel, re.Env, model.UsageAim);
                        tbRev.flPurpose.RenderCustom(re.Panel, re.Env, model.Purpose);
                        tbRev.flPropertyType.RenderCustom(re.Panel, re.Env, model.PropertyType);
                        tbRev.flIsDivisible.RenderCustom(re.Panel, re.Env, model.IsDivisible);

                        tbRev.flNote.RenderCustom(re.Panel, re.Env, model.Note);
                        tbRev.flPhotos.RenderCustomT(re.Panel, re.Env, JsonConvert.SerializeObject(re.Model.Photos ?? new FileInfo[] { }));
                        tbRev.flDocs.RenderCustomT(re.Panel, re.Env, JsonConvert.SerializeObject(re.Model.Docs ?? new FileInfo[] { }));
                        
                        //tbRev.flTechConditions.RenderCustom(re.Panel, re.Env, model.TechConditions);
                        //tbRev.flRegistered.RenderCustom(re.Panel, re.Env, model.Registered);

                    })
                    .OnValidating(ve => {
                        var tbRev = new TbLandObjectsRevisions();
                        var model = ve.Model;

                        var checkFields = new Field[] {
                            tbRev.flBalanceHolderBin,
                            tbRev.flName,
                            tbRev.flDescription,
                            tbRev.flLandCategory,
                            tbRev.flLandArea,
                            tbRev.flFuncPurpose1,
                            tbRev.flFuncPurpose2,
                            tbRev.flFuncPurpose3,
                            tbRev.flFuncPurpose4,
                            tbRev.flUsageAim,
                            tbRev.flPurpose,
                            tbRev.flPropertyType,
                            tbRev.flIsDivisible,
                            tbRev.flNote,
                            tbRev.flPhotos,
                            tbRev.flDocs,
                            tbRev.flTaxSum,
                            tbRev.flRentSum,

                            //tbRev.flRegistered,
                            //tbRev.flTechConditions
                        };
                        checkFields.Each(f => f.Validate(ve.Env));

                        if (model.Type != LandObjectTypes.Agricultural.ToString())
                        {
                            tbRev.flCost.Required().Validate(ve.Env);
                        }

                        if (ve.Env.IsValid)
                        {
                            if (tbRev.flName.GetVal(ve.Env).Length > 300)
                            {
                                ve.Env.AddError(tbRev.flName.FieldName, ve.Env.T($"{ve.Env.T(tbRev.flName.Text)}: {ve.Env.T("Максимальная длина - 300 символов")}"));
                            }

                            if (tbRev.flPhotos.GetGoodValue(ve.Env).Length < 3)
                            {
                                ve.Env.AddError(tbRev.flPhotos.FieldName, ve.Env.T("Необходимо прикрепить минимум 3 фотографии"));
                            }

                            var docs = tbRev.flDocs.GetGoodValue(ve.Env);
                            if (!docs.Any(x => x.Description == "Акт об определении кадастровой (оценочной) стоимости объекта торгов")){
                                ve.Env.AddError(tbRev.flPhotos.FieldName, ve.Env.T("Необходимо прикрепить \"Акт об определении кадастровой (оценочной) стоимости объекта торгов\" (скопируйте это наименование в описание соответствующего файла)"));
                            }

                            if (!docs.Any(x => x.Description == "Проект договора купли-продажи земельного участка или права аренды земельного участка"))
                            {
                                ve.Env.AddError(tbRev.flPhotos.FieldName, ve.Env.T("Необходимо прикрепить \"Проект договора купли-продажи земельного участка или права аренды земельного участка\" (скопируйте это наименование в описание соответствующего файла)"));
                            }

                            if (tbRev.flLandArea.GetVal(ve.Env) == 0)
                            {
                                ve.Env.AddError(tbRev.flLandArea.FieldName, ve.Env.T($"{tbRev.flLandArea.Text} не может быть равна нулю"));
                            }
                            if (model.Type != LandObjectTypes.Agricultural.ToString())
                            {
                                if (tbRev.flCost.GetVal(ve.Env) == 0)
                                {
                                    ve.Env.AddError(tbRev.flCost.FieldName, ve.Env.T($"{tbRev.flCost.Text} не может быть равна нулю"));
                                }
                            }

                            if ((tbRev.flTaxSum.GetValOrNull(ve.Env).HasValue && tbRev.flRentSum.GetValOrNull(ve.Env).HasValue) || (!tbRev.flTaxSum.GetValOrNull(ve.Env).HasValue && !tbRev.flRentSum.GetValOrNull(ve.Env).HasValue))
                            {
                                ve.Env.AddError(tbRev.flTaxSum.FieldName, ve.Env.T($"{ve.Env.T(tbRev.flTaxSum.Text)}, {ve.Env.T(tbRev.flRentSum.Text)}: {ve.Env.T("Нужно заполнить одно из этих полей")}"));
                            }
                        }
                    })
                    .OnProcessing(pe => {
                        var tbRev = new TbLandObjectsRevisions();
                        var model = pe.Model;

                        pe.Model.setAdditionalData(
                            0,
                            tbRev.flBalanceHolderBin.GetVal(pe.Env),
                            tbRev.flName.GetVal(pe.Env),
                            tbRev.flDescription.GetVal(pe.Env),
                            tbRev.flLandArea.GetVal(pe.Env),
                            tbRev.flLandCategory.GetVal(pe.Env),
                            null,
                            tbRev.flFuncPurpose1.GetVal(pe.Env),
                            tbRev.flFuncPurpose2.GetVal(pe.Env),
                            tbRev.flFuncPurpose3.GetVal(pe.Env),
                            tbRev.flFuncPurpose4.GetVal(pe.Env),
                            tbRev.flUsageAim.GetVal(pe.Env),
                            tbRev.flPurpose.GetVal(pe.Env),
                            tbRev.flPropertyType.GetVal(pe.Env),
                            tbRev.flIsDivisible.GetVal(pe.Env),
                            tbRev.flNote.GetVal(pe.Env),
                            tbRev.flPhotos.GetGoodValue(pe.Env),
                            tbRev.flDocs.GetGoodValue(pe.Env)
                        );

                        if (model.Type != LandObjectTypes.Agricultural.ToString())
                        {
                            pe.Model.Cost = tbRev.flCost.GetVal(pe.Env);
                        }

                    })
                )
                .Step("Местоположение", step => step
                    .Enabled(env => env.Args.OrderType.In(LandObjectsOrderTypes.Create, LandObjectsOrderTypes.Edit))
                    .OnRendering(re => {
                        var tbRev = new TbLandObjectsRevisions();
                        var model = re.Model;

                        tbRev.flCountry.RenderCustom(re.Panel, re.Env, model.Country);
                        tbRev.flRegion.RenderCustom(re.Panel, re.Env, model.Region);
                        tbRev.flDistrict.RenderCustom(re.Panel, re.Env, model.District);
                        tbRev.flAddress.RenderCustom(re.Panel, re.Env, model.Address);

                    })
                    .OnValidating(ve => {
                        var tbRev = new TbLandObjectsRevisions();
                        var checkFields = new Field[] {
                            tbRev.flCountry,
                            tbRev.flRegion,
                            tbRev.flDistrict,
                            tbRev.flAddress
                        };
                        checkFields.Each(f => f.Validate(ve.Env));
                    })
                    .OnProcessing(pe => {
                        var tbRev = new TbLandObjectsRevisions();

                        pe.Model.setGeoPosition(
                            tbRev.flCountry.GetVal(pe.Env),
                            tbRev.flRegion.GetVal(pe.Env),
                            tbRev.flDistrict.GetVal(pe.Env),
                            tbRev.flAddress.GetVal(pe.Env)
                        );

                    })
                )
                .Step("Обременения (Аресты)", step => step
                    .Enabled(env => env.Args.OrderType.In(LandObjectsOrderTypes.Create, LandObjectsOrderTypes.Edit))
                    .OnRendering(re => {
                        var tbRev = new TbLandObjectsRevisions();
                        var model = re.Model;

                        tbRev.flChargePresence.RenderCustom(re.Panel, re.Env, model.ChargePresence);
                        tbRev.flCharge.RenderCustom(re.Panel, re.Env, model.Charge);
                        tbRev.flRestriction.RenderCustom(re.Panel, re.Env, model.Restriction);
                        tbRev.flArest.RenderCustom(re.Panel, re.Env, model.Arest);

                    })
                    .OnValidating(ve => {
                        var tbRev = new TbLandObjectsRevisions();
                        var checkFields = new Field[] {
                            tbRev.flChargePresence,
                            tbRev.flCharge,
                            tbRev.flRestriction,
                            tbRev.flArest
                        };
                        checkFields.Each(f => f.Validate(ve.Env));

                        if (ve.Env.IsValid)
                        {
                            if (tbRev.flChargePresence.GetValue(ve.Env) != null)
                            {
                                if (tbRev.flChargePresence.GetVal(ve.Env) == RefBoolChargePresence.Values.HasNoChargeArest && ((tbRev.flCharge.GetValue(ve.Env) + string.Empty).Length > 0 || (tbRev.flRestriction.GetValue(ve.Env) + string.Empty).Length > 0 || (tbRev.flArest.GetValue(ve.Env) + string.Empty).Length > 0))
                                {
                                    ve.Env.AddError(tbRev.flChargePresence.FieldName, ve.Env.T("Не нужно заполнять поля, если нет обременений (арестов)"));
                                }
                            }
                        }
                    })
                    .OnProcessing(pe => {
                        var tbRev = new TbLandObjectsRevisions();

                        pe.Model.setChargePresence(
                            tbRev.flChargePresence.GetVal(pe.Env),
                            tbRev.flCharge.GetVal(pe.Env),
                            tbRev.flRestriction.GetVal(pe.Env),
                            tbRev.flArest.GetVal(pe.Env)
                        );

                    })
                )
                //.Step("Технические условия", step => step
                //    .Enabled(env => env.Args.OrderType.In(LandObjectsOrderTypes.Create, LandObjectsOrderTypes.Edit) && env.Model.TechConditions == UpgsLandObjectsTechnicalConditions.Has.ToString())
                //    .OnRendering(re => {
                //        var tbRev = new TbLandObjectsRevisions();
                //        var model = re.Model;

                //        var elecGroup = new Accordion(re.Env.T("Электроснабжение"));
                //        tbRev.flPowerOutage.RenderCustom(elecGroup, re.Env, model.PowerOutage);
                //        tbRev.flLoadTypePhaseOne.RenderCustom(elecGroup, re.Env, model.LoadTypePhaseOne);
                //        tbRev.flLoadTypePhaseThree.RenderCustom(elecGroup, re.Env, model.LoadTypePhaseThree);
                //        tbRev.flPhoneAccessPoint.RenderCustom(elecGroup, re.Env, model.PhoneAccessPoint);
                //        re.Panel.AddComponent(elecGroup);

                //        var waterGroup = new Accordion(re.Env.T("Водоснабжение"));
                //        tbRev.flHotWaterWhenGasified.RenderCustom(waterGroup, re.Env, model.HotWaterWhenGasified);
                //        tbRev.flTotalWaterUsage.RenderCustom(waterGroup, re.Env, model.TotalWaterUsage);
                //        tbRev.flForDrinkingPurpose.RenderCustom(waterGroup, re.Env, model.ForDrinkingPurpose);
                //        tbRev.flForProductionPurpose.RenderCustom(waterGroup, re.Env, model.ForProductionPurpose);
                //        re.Panel.AddComponent(waterGroup);

                //        var heatGroup = new Accordion(re.Env.T("Теплоснабжение"));
                //        tbRev.flTotalHeatLoad.RenderCustom(heatGroup, re.Env, model.TotalHeatLoad);
                //        tbRev.flHeating.RenderCustom(heatGroup, re.Env, model.Heating);
                //        tbRev.flVentilation.RenderCustom(heatGroup, re.Env, model.Ventilation);
                //        tbRev.flHotWaterProvision.RenderCustom(heatGroup, re.Env, model.HotWaterProvision);
                //        re.Panel.AddComponent(heatGroup);

                //        var gasGroup = new Accordion(re.Env.T("Газоснабжение"));
                //        tbRev.flGasTotalUsage.RenderCustom(gasGroup, re.Env, model.GasTotalUsage);
                //        tbRev.flGasMealUsage.RenderCustom(gasGroup, re.Env, model.GasMealUsage);
                //        re.Panel.AddComponent(gasGroup);

                //        var heatAirGroup = new Accordion(re.Env.T("Отопление"));
                //        tbRev.flHeatAirVentilation.RenderCustom(heatAirGroup, re.Env, model.HeatAirVentilation);
                //        tbRev.flHeatAirCooling.RenderCustom(heatAirGroup, re.Env, model.HeatAirCooling);
                //        re.Panel.AddComponent(heatAirGroup);

                //        var canalGroup = new Accordion(re.Env.T("Канализационная система"));
                //        tbRev.flWaterDrainage.RenderCustom(canalGroup, re.Env, model.WaterDrainage);
                //        tbRev.flCanalizationTotalWasteWater.RenderCustom(canalGroup, re.Env, model.CanalizationTotalWasteWater);
                //        tbRev.flCanalizationFeces.RenderCustom(canalGroup, re.Env, model.CanalizationFeces);
                //        tbRev.flCanalizationPollutedByProduction.RenderCustom(canalGroup, re.Env, model.CanalizationPollutedByProduction);
                //        tbRev.flCanalizationConditionallyPolluted.RenderCustom(canalGroup, re.Env, model.CanalizationConditionallyPolluted);
                //        re.Panel.AddComponent(canalGroup);


                //    })
                //    .OnValidating(ve => {
                //        var tbRev = new TbLandObjectsRevisions();
                //        var checkFields = new Field[] {
                //            tbRev.flPowerOutage,
                //            tbRev.flLoadTypePhaseOne,
                //            tbRev.flLoadTypePhaseThree,
                //            tbRev.flPhoneAccessPoint,

                //            tbRev.flHotWaterWhenGasified,
                //            tbRev.flTotalWaterUsage,
                //            tbRev.flForDrinkingPurpose,
                //            tbRev.flForProductionPurpose,

                //            tbRev.flTotalHeatLoad,
                //            tbRev.flHeating,
                //            tbRev.flVentilation,
                //            tbRev.flHotWaterProvision,

                //            tbRev.flGasTotalUsage,
                //            tbRev.flGasMealUsage,

                //            tbRev.flHeatAirVentilation,
                //            tbRev.flHeatAirCooling,

                //            tbRev.flWaterDrainage,
                //            tbRev.flCanalizationTotalWasteWater,
                //            tbRev.flCanalizationFeces,
                //            tbRev.flCanalizationPollutedByProduction,
                //            tbRev.flCanalizationConditionallyPolluted,
                //        };
                //        checkFields.Each(f => f.Validate(ve.Env));
                //    })
                //    .OnProcessing(pe => {
                //        var tbRev = new TbLandObjectsRevisions();

                //        pe.Model.setTechConditions(
                //            tbRev.flPowerOutage.GetValOrNull(pe.Env),
                //            tbRev.flLoadTypePhaseOne.GetValOrNull(pe.Env),
                //            tbRev.flLoadTypePhaseThree.GetValOrNull(pe.Env),
                //            tbRev.flPhoneAccessPoint.GetValOrNull(pe.Env),

                //            tbRev.flHotWaterWhenGasified.GetValOrNull(pe.Env),
                //            tbRev.flTotalWaterUsage.GetValOrNull(pe.Env),
                //            tbRev.flForDrinkingPurpose.GetValOrNull(pe.Env),
                //            tbRev.flForProductionPurpose.GetValOrNull(pe.Env),

                //            tbRev.flWaterDrainage.GetValOrNull(pe.Env),
                //            tbRev.flCanalizationTotalWasteWater.GetValOrNull(pe.Env),
                //            tbRev.flCanalizationFeces.GetValOrNull(pe.Env),
                //            tbRev.flCanalizationPollutedByProduction.GetValOrNull(pe.Env),
                //            tbRev.flCanalizationConditionallyPolluted.GetValOrNull(pe.Env),

                //            tbRev.flTotalHeatLoad.GetValOrNull(pe.Env),
                //            tbRev.flHeating.GetValOrNull(pe.Env),
                //            tbRev.flVentilation.GetValOrNull(pe.Env),
                //            tbRev.flHotWaterProvision.GetValOrNull(pe.Env),

                //            tbRev.flGasTotalUsage.GetValOrNull(pe.Env),
                //            tbRev.flGasMealUsage.GetValOrNull(pe.Env),

                //            tbRev.flHeatAirVentilation.GetValOrNull(pe.Env),
                //            tbRev.flHeatAirCooling.GetValOrNull(pe.Env)
                //        );

                //    })
                //)
                ;
        }


        public override LandObjectModel GetModel(ActionEnv<LandObjectOrderQueryArgs> env, GetModelFrom from, LandObjectsOrderTypes orderType, ITransaction transaction) {
            var model = from switch {
                GetModelFrom.Empty => new LandObjectModel() {
                    SallerBin = env.User.GetUserBin(env.QueryExecuter),
                    SallerId = -1,
                    Status = LandObjectStatuses.Active.ToString(),
                    Block = LandObjectBlocks.ActiveFree.ToString()
                },
                GetModelFrom.Entity =>
                    new TbLandObjects()
                        .AddFilter(t => t.flId, env.Args.Id.Value)
                        .GetObjectModelFirst(env.QueryExecuter, transaction),
                GetModelFrom.Revision =>
                    new TbLandObjectsRevisions()
                        .AddFilter(t => t.flRevisionId, env.Args.RevisionId.Value)
                        .GetObjectModelFirst(env.QueryExecuter, transaction),
                _ => throw new NotImplementedException()
            };

            if (from == GetModelFrom.Empty) {
                switch (orderType) {
                    case LandObjectsOrderTypes.Create:
                        model.Status = LandObjectStatuses.Active.ToString();
                        model.Block = LandObjectBlocks.ActiveFree.ToString();

                        if (env.Args.AppId.HasValue)
                        {
                            var app = new TbLandApplications().GetAppModel(env.Args.AppId.Value, env.QueryExecuter);
                            model.flApplicationId = env.Args.AppId.Value;
                            model.UsageAim = app.flUsageAim;
                            model.WKT = app.flWKT;
                            model.Coords = app.flCoords;
                            model.Country = app.flCountry;
                            model.Region = app.flRegion;
                            model.District = app.flDistrict;
                            model.Address = app.flAddress;
                        }

                        break;
                }
            }
            if(from == GetModelFrom.Entity) {
                switch (orderType) {
                    case LandObjectsOrderTypes.Remove:
                        model.Status = LandObjectStatuses.Deleted.ToString();
                        model.Block = LandObjectBlocks.DeletedDisqualified.ToString();
                        break;
                }
            }

            return model;
        }

        public override LandObjectsOrderTypes GetOrderType(ActionEnv<LandObjectOrderQueryArgs> env, GetOrderModelFrom orderModelFrom) {
            return orderModelFrom switch {
                GetOrderModelFrom.Empty => LandObjectsOrderTypes.Create,
                _ => env.Args.OrderType switch {
                    LandObjectsOrderTypeActions.Remove => LandObjectsOrderTypes.Remove,
                    LandObjectsOrderTypeActions.Edit => LandObjectsOrderTypes.Edit,
                    LandObjectsOrderTypeActions.EditOwner => LandObjectsOrderTypes.EditOwner,
                    _ => throw new ArgumentException($"OrderType: {env.Args.OrderType}")
                }
            };
        }

        public override string GetWizardCancelRedirectUrl(ActionEnv<LandObjectOrderQueryArgs> env, LandObjectsOrderTypes orderType, GetModelFrom modelFrom) {
            var urlHelper = env.RequestContext.GetUrlHelper();
            //TODO
            return modelFrom switch {
                GetModelFrom.Empty => urlHelper.YodaAction(ModuleName, nameof(MnuLandObjectOrderBase)),
                GetModelFrom.Entity => urlHelper.YodaAction(ModuleName, nameof(MnuLandObjectOrderBase), new LandObjectOrderQueryArgs { Id = env.Args.Id.Value, MenuAction = "view" }),
                GetModelFrom.Revision => urlHelper.YodaAction(ModuleName, MenuName, new LandObjectOrderQueryArgs { RevisionId = env.Args.RevisionId, MenuAction = Actions.ViewOrder }),
                _ => throw new NotImplementedException()
            };
        }

        public override ValidationResultBase IsCreateFromActionValid(ActionEnv<LandObjectOrderQueryArgs> env) {
            if (env.Args.Id == null) {
                return new RedirectResult(new ArgumentException("Id"));
            }

            if (env.Args.OrderType == null) {
                return new RedirectResult(new ArgumentException("OrderType"));
            }

            if (!CanCreateOrder(env.Args.Id.Value, GetOrderType(env, GetOrderModelFrom.Entity), out var error, env)) {
                if (env.Args.Id.HasValue) {
                    return new RedirectResult(ModuleName, MnuLandObjectView.MnuName, new LandObjectViewActionQueryArgs() { Id = env.Args.Id.Value, MenuAction = MnuLandObjectView.Actions.View }, error);
                }
                else {
                    return new RedirectResult(ModuleName, "ActionError", null, error);
                }
            }

            return new OkResult();
        }

        public static bool TryGetCurrentInWorkOrderRevId(int id, out int revId, IQueryExecuter queryExecuter) {
            var ret = new TbLandObjectsOrderResult()
                .AddFilterNotFinishedOrders()
                .JoinT("tbOrders", new TbLandObjectsRevisions().AddFilter(t => t.flId, id), "tbRevs")
                .On((t1, t2) => new Join(t1.flSubjectId, t2.flRevisionId))
                .SelectScalar(t => t.L.flSubjectId, queryExecuter);

            if (ret == null) {
                revId = 0;
                return false;
            }

            revId = ret.Value;
            return true;
        }

        public static bool CanCreateOrder(int id, LandObjectsOrderTypes orderType, out string error, ActionEnv<LandObjectOrderQueryArgs> env) {

            var isInternal = !env.User.IsExternalUser() && !env.User.IsGuest();
            var isSeller = env.User.HasRole("TRADERESOURCES-Земельные ресурсы-Создание приказов", env.QueryExecuter);

            if (TryGetCurrentInWorkOrderRevId(id, out _, env.QueryExecuter)) {
                error = env.T("Для данного объекта имеется не исполненый приказ");
                return false;
            }

            var tradeResourceObject = LandObjectModelHelper.GetObjectModel(id, env.QueryExecuter);

            bool checkLandActive(out string err) {
                if (tradeResourceObject.Status != LandObjectStatuses.Active.ToString()) {
                    err = env.T("Изменение разрешено только для активного объекта");
                    return false;
                }
                err = null;
                return true;
            }
            bool checkLandDeleted(out string err) {
                if (tradeResourceObject.Status != LandObjectStatuses.Deleted.ToString()) {
                    err = env.T("Нельзя удалять удалённые объекты");
                    return true;
                }
                err = null;
                return false;
            }

            switch (orderType) {
                case LandObjectsOrderTypes.Create:
                    error = env.T("Недопустимый тип приказа");
                    return false;
                case LandObjectsOrderTypes.Edit:
                    return checkLandActive(out error);
                case LandObjectsOrderTypes.EditOwner: {
                        error = isInternal ? null : "";
                        return isInternal;
                }
                case LandObjectsOrderTypes.Remove:
                    return !checkLandDeleted(out error);
                default:
                    throw new NotImplementedException($"ObjectsOrderTypes: {orderType}");
            }
        }

        public override void SaveModel(int revisionId, LandObjectModel model, LandObjectsOrderTypes orderType, ActionEnv<LandObjectOrderQueryArgs> env, ITransaction transaction, GetModelFrom modelFrom) {
            var tbObjRevs = new TbLandObjectsRevisions();

            if (modelFrom == GetModelFrom.Empty) {
                model.Id = new TbLandObjects().flId.GetNextId(env.QueryExecuter, transaction);
            }

            var updateOrInsert = modelFrom switch {
                GetModelFrom.Revision =>
                    tbObjRevs
                        .AddFilter(t => t.flRevisionId, revisionId)
                        .Update(),

                _ => tbObjRevs
                    .Insert()
            };
            model.RevisionId = revisionId;

            setModel(updateOrInsert, model);

            if (orderType == LandObjectsOrderTypes.Create && modelFrom == GetModelFrom.Empty && env.Args.AppId.HasValue)
            {
                var tbApps = new TbLandApplications();
                tbApps.AddFilter(t => t.flId, env.Args.AppId);

                tbApps.Update()
                .SetT(t => t.flAcceptDate, env.QueryExecuter.GetDateTime("dbAgreements"))
                .SetT(t => t.flStatus, LandSource.References.Applications.ApplicationStatuses.Принята)
                .SetT(t => t.flLandObjectId, model.Id)
                .Execute(env.QueryExecuter, transaction);
            }

            updateOrInsert
                .Execute(env.QueryExecuter, transaction);
        }

        private void setModel<TLandObjectsTable>(DataModifingQueryProxy<TLandObjectsTable> updateOrInsert, LandObjectModel model) where TLandObjectsTable : TbLandObjectsBase {
            LandObjectStatuses status = model.Status == LandObjectStatuses.Active.ToString() ? LandObjectStatuses.Active : LandObjectStatuses.Deleted;

            var geometry = new GeomFromWKT(model.WKT, 4326);
            var point = new PointFromGeom(new SqlExpVal(geometry));

            updateOrInsert
                .SetT(t => t.flId, model.Id)
                .SetT(t => t.flRevisionId, model.RevisionId)
                .SetT(t => t.flApplicationId, model.flApplicationId)
                .Set(t => t.flType, model.Type)
                .SetT(t => t.flSallerId, model.SallerId)
                .SetT(t => t.flSallerBin, model.SallerBin)
                .Set(t => t.flStatus, model.Status)
                .Set(t => t.flBlock, model.Block)
                .Set(t => t.flKfsLevel1, model.KfsLevel1)
                .Set(t => t.flKfsLevel2, model.KfsLevel2)
                .Set(t => t.flKfsLevel3, model.KfsLevel3)
                .Set(t => t.flKfsLevel4, model.KfsLevel4)
                .Set(t => t.flKfsLevel5, model.KfsLevel5)
                .Set(t => t.flBalanceHolderId, model.BalanceHolderId)
                .SetT(t => t.flBalanceHolderBin, model.BalanceHolderBin)
                .SetT(t => t.flName, model.Name)
                .SetT(t => t.flDescription, model.Description)
                //.Set(t => t.flRegistered, model.Registered)
                .SetT(t => t.flKadNumber, model.KadNumber)
                .SetT(t => t.flEgknId, model.EgknId)
                .SetT(t => t.flLandArea, model.LandArea)
                .Set(t => t.flFuncPurpose1, model.FuncPurpose1)
                .Set(t => t.flFuncPurpose2, model.FuncPurpose2)
                .Set(t => t.flFuncPurpose3, model.FuncPurpose3)
                .Set(t => t.flFuncPurpose4, model.FuncPurpose4)
                .Set(t => t.flUsageAim, model.UsageAim)
                .Set(t => t.flPurpose, model.Purpose)
                .Set(t => t.flPropertyType, model.PropertyType)
                .Set(t => t.flIsDivisible, model.IsDivisible)

                .SetT(t => t.flPhotos, JsonConvert.SerializeObject(model.Photos))
                .SetT(t => t.flDocs, JsonConvert.SerializeObject(model.Docs))
                //.Set(t => t.flTechConditions, model.TechConditions)
                .Set(t => t.flNote, model.Note)
                .Set(t => t.flCountry, model.Country)
                .Set(t => t.flRegion, model.Region)
                .Set(t => t.flDistrict, model.District)
                .SetT(t => t.flAddress, model.Address)

                .SetT(t => t.flConsolidatedStatementDateTime, model.ConsolidatedStatementDateTime)
                .SetT(t => t.flConsolidatedStatementReconciliationDateTime, model.ConsolidatedStatementReconciliationDateTime)
                .SetT(t => t.flWKT, model.WKT)
                .Set(t => t.flGeometry, geometry)
                .Set(t => t.flPoint, point)
                .SetT(t => t.flCoords, model.Coords)
                .Set(t => t.flChargePresence, model.ChargePresence)
                .SetT(t => t.flCharge, model.Charge)
                .SetT(t => t.flRestriction, model.Restriction)
                .SetT(t => t.flArest, model.Arest)
                .Set(t => t.flLandCategory, model.LandCategory)
                .SetT(t => t.flCost, model.Cost)
                .SetT(t => t.flTaxSum, model.flTaxSum)
                .SetT(t => t.flRentSum, model.flRentSum);
                //.SetT(t => t.flPowerOutage, model.PowerOutage)
                //.SetT(t => t.flLoadTypePhaseOne, model.LoadTypePhaseOne)
                //.SetT(t => t.flLoadTypePhaseThree, model.LoadTypePhaseThree)
                //.SetT(t => t.flPhoneAccessPoint, model.PhoneAccessPoint)
                //.SetT(t => t.flHotWaterWhenGasified, model.HotWaterWhenGasified)
                //.SetT(t => t.flTotalWaterUsage, model.TotalWaterUsage)
                //.SetT(t => t.flForDrinkingPurpose, model.ForDrinkingPurpose)
                //.SetT(t => t.flForProductionPurpose, model.ForProductionPurpose)
                //.SetT(t => t.flWaterDrainage, model.WaterDrainage)
                //.SetT(t => t.flCanalizationTotalWasteWater, model.CanalizationTotalWasteWater)
                //.SetT(t => t.flCanalizationFeces, model.CanalizationFeces)
                //.SetT(t => t.flCanalizationPollutedByProduction, model.CanalizationPollutedByProduction)
                //.SetT(t => t.flCanalizationConditionallyPolluted, model.CanalizationConditionallyPolluted)
                //.SetT(t => t.flTotalHeatLoad, model.TotalHeatLoad)
                //.SetT(t => t.flHeating, model.Heating)
                //.SetT(t => t.flVentilation, model.Ventilation)
                //.SetT(t => t.flHotWaterProvision, model.HotWaterProvision)
                //.SetT(t => t.flGasTotalUsage, model.GasTotalUsage)
                //.SetT(t => t.flGasMealUsage, model.GasMealUsage)
                //.SetT(t => t.flHeatAirVentilation, model.HeatAirVentilation)
                //.SetT(t => t.flHeatAirCooling, model.HeatAirCooling);
        }

        public override bool TryAcceptModel(int revisionId, LandObjectModel model, LandObjectsOrderTypes orderType, ActionEnv<LandObjectOrderQueryArgs> env, ITransaction transaction, out string error) {
            if (orderType == LandObjectsOrderTypes.Create) {

            }

            var updateOrInsert = orderType switch {
                LandObjectsOrderTypes.Create =>
                    new TbLandObjects()
                        .Insert(),
                _ =>
                    new TbLandObjects()
                        .AddFilter(t => t.flId, model.Id)
                        .Update()
            };

            setModel(updateOrInsert, model);

            updateOrInsert.Execute(env.QueryExecuter, transaction);

            error = null;
            return true;
        }

        public override void ViewModel(RenderActionEnv<LandObjectOrderQueryArgs> env, LandObjectModel model, LandObjectsOrderTypes orderType, OrderStatus orderStatus) {
            env.RequestContext.Title = env.T("Объект");
            viewModelMainData(env.Form, env, model);
            viewModelGeometryData(env.Form, env, model);
            viewModelAdditionalData(env.Form, env, model);
            viewModelGeoPositionData(env.Form, env, model);
            viewModelChargePresenceData(env.Form, env, model);
            //if (model.TechConditions == UpgsLandObjectsTechnicalConditions.Has.ToString()) {
            //    viewModelTechConditionsData(env.Form, env, model, env.Args);
            //}
        }

        public static void viewModelMainData(WidgetBase widget, FormEnvironment env, LandObjectModel model, bool collapsed = false) {
            var tbRevs = new TbLandObjectsRevisions();
            var groupbox = new Accordion(env.T("Основные данные объекта"), collapsed);
            widget.AddComponent(groupbox);

            tbRevs.flId.RenderCustom(groupbox, env, model.Id.ToString() ?? env.T("*Генерируется автоматически при исполнении приказа"), readOnly: true);
            tbRevs.flRevisionId.RenderCustom(groupbox, env, model.RevisionId, readOnly: true);
            tbRevs.flType.RenderCustom(groupbox, env, model.Type, readOnly: true);
            tbRevs.flSallerBin.RenderCustom(groupbox, env, model.SallerBin, readOnly: true);
            tbRevs.flStatus.RenderCustom(groupbox, env, model.Status, readOnly: true);
            tbRevs.flBlock.RenderCustom(groupbox, env, model.Block, readOnly: true);
            tbRevs.flKfsLevel1.RenderCustom(groupbox, env, model.KfsLevel1, readOnly: true);
            tbRevs.flKfsLevel2.RenderCustom(groupbox, env, model.KfsLevel2, readOnly: true);
            tbRevs.flKfsLevel3.RenderCustom(groupbox, env, model.KfsLevel3, readOnly: true);
            tbRevs.flKfsLevel4.RenderCustom(groupbox, env, model.KfsLevel4, readOnly: true);
            tbRevs.flKfsLevel5.RenderCustom(groupbox, env, model.KfsLevel5, readOnly: true);
            tbRevs.flKadNumber.RenderCustom(groupbox, env, model.KadNumber, readOnly: true);
            //tbRevs.flEgknId.RenderCustom(groupbox, env, model.EgknId, readOnly: true);
            //groupbox.AddHtml($"<div render-field-name='flLandUrl' data-field-readonly='true' class='form-group row '><label for='' class='col-sm-3 control-label'>{tbRevs.flLandUrl.Text}</label><div class='col-sm-9'><span class=' form-control'><a href='{model.LandUrl}' target='_blank'>Перейти на карту</a></span></div></div>");
        }

        public static void viewModelAdditionalData(WidgetBase widget, FormEnvironment env, LandObjectModel model, bool collapsed = false) {
            var tbRevs = new TbLandObjectsRevisions();
            var groupbox = new Accordion(env.T("Дополнительные данные объекта"), collapsed);
            widget.AddComponent(groupbox);

            tbRevs.flBalanceHolderBin.RenderCustom(groupbox, env, model.BalanceHolderBin, readOnly: true);
            tbRevs.flName.RenderCustom(groupbox, env, model.Name, readOnly: true);
            tbRevs.flDescription.RenderCustom(groupbox, env, model.Description, readOnly: true);


            //SHIT
            //tbRevs.flCost.RenderCustom(groupbox, env, model.Cost, readOnly: true);
            tbRevs.flName.Text = tbRevs.flLandArea.Text;
            tbRevs.flName.RenderCustom(groupbox, env, model.LandArea.ToString("N4"), readOnly: true);


            tbRevs.flLandCategory.RenderCustom(groupbox, env, model.LandCategory, readOnly: true);
            if (model.Type != LandObjectTypes.Agricultural.ToString()) {
                tbRevs.flCost.RenderCustom(groupbox, env, model.Cost, readOnly: true);
            }
            tbRevs.flFuncPurpose1.RenderCustom(groupbox, env, model.FuncPurpose1, readOnly: true);
            tbRevs.flFuncPurpose2.RenderCustom(groupbox, env, model.FuncPurpose2, readOnly: true);
            tbRevs.flFuncPurpose3.RenderCustom(groupbox, env, model.FuncPurpose3, readOnly: true);
            tbRevs.flFuncPurpose4.RenderCustom(groupbox, env, model.FuncPurpose4, readOnly: true);
            tbRevs.flUsageAim.RenderCustom(groupbox, env, model.UsageAim, readOnly: true);
            tbRevs.flPurpose.RenderCustom(groupbox, env, model.Purpose, readOnly: true);
            tbRevs.flPropertyType.RenderCustom(groupbox, env, model.PropertyType, readOnly: true);
            tbRevs.flIsDivisible.RenderCustom(groupbox, env, model.IsDivisible, readOnly: true);
            //tbRevs.flRegistered.RenderCustom(groupbox, env, model.Registered, readOnly: true);
            //tbRevs.flTechConditions.RenderCustom(groupbox, env, model.TechConditions, readOnly: true);
            tbRevs.flPhotos.RenderCustom(groupbox, env, JsonConvert.SerializeObject(model.Photos ?? new FileInfo[] { }), readOnly: true);
            tbRevs.flDocs.RenderCustom(groupbox, env, JsonConvert.SerializeObject(model.Docs ?? new FileInfo[] { }), readOnly: true);
            tbRevs.flNote.RenderCustom(groupbox, env, model.Note, readOnly: true);
        }

        public static void viewModelGeoPositionData(WidgetBase widget, FormEnvironment env, LandObjectModel model, bool collapsed = false) {
            var tbRevs = new TbLandObjectsRevisions();
            var groupbox = new Accordion(env.T("Местоположение"), collapsed);
            widget.AddComponent(groupbox);

            tbRevs.flCountry.RenderCustom(groupbox, env, model.Country, readOnly: true);
            tbRevs.flRegion.RenderCustom(groupbox, env, model.Region, readOnly: true);
            tbRevs.flDistrict.RenderCustom(groupbox, env, model.District, readOnly: true);
            tbRevs.flAddress.RenderCustom(groupbox, env, model.Address, readOnly: true);
        }

        public static void viewModelGeometryData(WidgetBase widget, FormEnvironment env, LandObjectModel model)
        {
            var groupbox = new Accordion(env.T("Геометрия"));
            groupbox.CssClass += " container-fluid";
            widget.AddComponent(groupbox);

            var message = @$"<div>{env.T("1. Отображение границ земельного участка на электронной карте примерное и не является подтверждением его точных координат и поворотных точек.")}</div><div>{env.T("2. Координаты (долгота и широта) представлены из сводной ведомости, составленной @@@ConsolidatedStatementDateTime и сверенной с АИС ГЗК @@@ConsolidatedStatementReconciliationDateTime")}</div></br>";

            message = message.Replace("@@@ConsolidatedStatementDateTime", (model.ConsolidatedStatementDateTime.HasValue ? model.ConsolidatedStatementDateTime.Value.ToShortDateString() : "--.--.----"));
            message = message.Replace("@@@ConsolidatedStatementReconciliationDateTime", (model.ConsolidatedStatementReconciliationDateTime.HasValue ? model.ConsolidatedStatementReconciliationDateTime.Value.ToShortDateString() : "--.--.----"));


            groupbox.AddHtml(message);

            var row = new GridRow();
            groupbox.AddComponent(row);

            //tbRevs.flWKT.RenderCustom(groupbox, env, model.WKT, readOnly: true);
            row.AddComponent(new ObjectGeometryViewerComponent(model.WKT, "col-md-6"));
            row.AddComponent(new ObjectCoordsViewerComponent(model.Coords, "col-md-6"));
        }

        public static void viewModelChargePresenceData(WidgetBase widget, FormEnvironment env, LandObjectModel model, bool collapsed = false) {
            var tbRevs = new TbLandObjectsRevisions();
            var groupbox = new Accordion(env.T("Обременения (Аресты)"), collapsed);
            widget.AddComponent(groupbox);

            tbRevs.flChargePresence.RenderCustom(groupbox, env, model.ChargePresence, readOnly: true);
            tbRevs.flCharge.RenderCustom(groupbox, env, model.Charge, readOnly: true);
            tbRevs.flRestriction.RenderCustom(groupbox, env, model.Restriction, readOnly: true);
            tbRevs.flArest.RenderCustom(groupbox, env, model.Arest, readOnly: true);
        }

        //private void viewModelTechConditionsData(WidgetBase widget, FormEnvironment env, LandObjectModel model, LandObjectOrderQueryArgs args) {
        //    var tbRev = new TbLandObjectsRevisions();
        //    var groupbox = new Accordion(env.T("Обременения (Аресты)"));
        //    widget.AddComponent(groupbox);


        //    var elecGroup = new Accordion(env.T("Электроснабжение"));
        //    tbRev.flPowerOutage.RenderCustom(elecGroup, env, model.PowerOutage, readOnly: true);
        //    tbRev.flLoadTypePhaseOne.RenderCustom(elecGroup, env, model.LoadTypePhaseOne, readOnly: true);
        //    tbRev.flLoadTypePhaseThree.RenderCustom(elecGroup, env, model.LoadTypePhaseThree, readOnly: true);
        //    tbRev.flPhoneAccessPoint.RenderCustom(elecGroup, env, model.PhoneAccessPoint, readOnly: true);
        //    groupbox.AddComponent(elecGroup);

        //    var waterGroup = new Accordion(env.T("Водоснабжение"));
        //    tbRev.flHotWaterWhenGasified.RenderCustom(waterGroup, env, model.HotWaterWhenGasified, readOnly: true);
        //    tbRev.flTotalWaterUsage.RenderCustom(waterGroup, env, model.TotalWaterUsage, readOnly: true);
        //    tbRev.flForDrinkingPurpose.RenderCustom(waterGroup, env, model.ForDrinkingPurpose, readOnly: true);
        //    tbRev.flForProductionPurpose.RenderCustom(waterGroup, env, model.ForProductionPurpose, readOnly: true);
        //    groupbox.AddComponent(waterGroup);

        //    var heatGroup = new Accordion(env.T("Теплоснабжение"));
        //    tbRev.flTotalHeatLoad.RenderCustom(heatGroup, env, model.TotalHeatLoad, readOnly: true);
        //    tbRev.flHeating.RenderCustom(heatGroup, env, model.Heating, readOnly: true);
        //    tbRev.flVentilation.RenderCustom(heatGroup, env, model.Ventilation, readOnly: true);
        //    tbRev.flHotWaterProvision.RenderCustom(heatGroup, env, model.HotWaterProvision, readOnly: true);
        //    groupbox.AddComponent(heatGroup);

        //    var gasGroup = new Accordion(env.T("Газоснабжение"));
        //    tbRev.flGasTotalUsage.RenderCustom(gasGroup, env, model.GasTotalUsage, readOnly: true);
        //    tbRev.flGasMealUsage.RenderCustom(gasGroup, env, model.GasMealUsage, readOnly: true);
        //    groupbox.AddComponent(gasGroup);

        //    var heatAirGroup = new Accordion(env.T("Отопление"));
        //    tbRev.flHeatAirVentilation.RenderCustom(heatAirGroup, env, model.HeatAirVentilation, readOnly: true);
        //    tbRev.flHeatAirCooling.RenderCustom(heatAirGroup, env, model.HeatAirCooling, readOnly: true);
        //    groupbox.AddComponent(heatAirGroup);

        //    var canalGroup = new Accordion(env.T("Канализационная система"));
        //    tbRev.flWaterDrainage.RenderCustom(canalGroup, env, model.WaterDrainage, readOnly: true);
        //    tbRev.flCanalizationTotalWasteWater.RenderCustom(canalGroup, env, model.CanalizationTotalWasteWater, readOnly: true);
        //    tbRev.flCanalizationFeces.RenderCustom(canalGroup, env, model.CanalizationFeces, readOnly: true);
        //    tbRev.flCanalizationPollutedByProduction.RenderCustom(canalGroup, env, model.CanalizationPollutedByProduction, readOnly: true);
        //    tbRev.flCanalizationConditionallyPolluted.RenderCustom(canalGroup, env, model.CanalizationConditionallyPolluted, readOnly: true);
        //    groupbox.AddComponent(canalGroup);
        //}

        protected override bool HasAccessToOrder(int revisionId, ActionEnv<LandObjectOrderQueryArgs> env) {
            var isInternal = !env.User.IsExternalUser() && !env.User.IsGuest();
            var isSeller = env.User.HasRole("TRADERESOURCES-Земельные ресурсы-Создание приказов", env.QueryExecuter);
            if (isInternal) {
                return true;
            }
            if (env.Args.MenuAction != Actions.ViewOrder && !isSeller)
            {
                throw new AccessDeniedException();
            }
            var hasRecords = new TbLandObjectsRevisions()
                .AddFilter(t => t.flRevisionId, revisionId)
                .AddFilter(t => t.flSallerBin, env.User.GetUserBin(env.QueryExecuter))
                .Count(env.QueryExecuter) > 0;

            return hasRecords;
        }
    }
    public class LandObjectOrderQueryArgs : OrderQueryArgs {
        public int? Id { get; set; }
        public int? AppId { get; set; }
        public LandObjectsOrderTypeActions? OrderType { get; set; }
    }
    public enum LandObjectsOrderTypeActions {
        Create,
        Edit,
        Remove,
        EditOwner
    }
    public enum LandObjectFillSteps {
        InitObject, FillData
    }

}
