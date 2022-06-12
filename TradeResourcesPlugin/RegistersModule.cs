using Analytics;
using CommonSource.QueryTables;
using CommonSource.References.Application;
using CommonSource.References.DeprivedPersons;
using CommonSource.References.Object;
using CommonSource.References.OfficialsOrg;
using CommonSource.References.Trade;
using CommonSource.SearchCollections;
using CommonSource.SearchCollections.DeprivedPersons;
using CommonSource.SearchCollections.Object;
using CommonSource.SearchCollections.Trade;
using FishingSource.QueryTables.Reservoir;
using FishingSource.SearchCollections.Object;
using FishingSource.SearchCollections.Reservoir;
using ForestSource.SearchCollections.Object;
using HuntingSource.SearchCollections.Object;
using HydrocarbonSource.References.Application;
using HydrocarbonSource.References.Trade;
using HydrocarbonSource.SearchCollections.Object;
using LandSource.QueryTables.Common;
using LandSource.References.Objects;
using LandSource.SearchCollections.LandObject;
using LandSource.SearchCollections.LandObjectsTrades;
using LandSource.SearchCollections.Trades;
using Microsoft.AspNetCore.Mvc;
using Source.SearchCollections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelecomOperatorsSource.SearchCollections.Object;
using TradeResourcesPlugin.Helpers;
using TradeResourcesPlugin.Helpers.Agreements;
using TradeResourcesPlugin.Modules;
using TradeResourcesPlugin.Modules.Administration;
using TradeResourcesPlugin.Modules.Administration.DeprivedPersons;
using TradeResourcesPlugin.Modules.Administration.OfficialOrgs;
using TradeResourcesPlugin.Modules.FishingMenus;
using TradeResourcesPlugin.Modules.FishingMenus.Objects;
using TradeResourcesPlugin.Modules.FishingMenus.Reservoirs;
using TradeResourcesPlugin.Modules.FishingMenus.Trades;
using TradeResourcesPlugin.Modules.ForestMenus;
using TradeResourcesPlugin.Modules.ForestMenus.Forestries;
using TradeResourcesPlugin.Modules.ForestMenus.ForestryPieces;
using TradeResourcesPlugin.Modules.ForestMenus.Objects;
using TradeResourcesPlugin.Modules.ForestMenus.Quarters;
using TradeResourcesPlugin.Modules.ForestMenus.Trades;
using TradeResourcesPlugin.Modules.HuntingMenus;
using TradeResourcesPlugin.Modules.HuntingMenus.Agreements;
using TradeResourcesPlugin.Modules.HuntingMenus.Objects;
using TradeResourcesPlugin.Modules.HuntingMenus.Trades;
using TradeResourcesPlugin.Modules.HydrocarbonMenus.Applications;
using TradeResourcesPlugin.Modules.HydrocarbonMenus.Objects;
using TradeResourcesPlugin.Modules.HydrocarbonMenus.Trades;
using TradeResourcesPlugin.Modules.LandObjectsMenus;
using TradeResourcesPlugin.Modules.LandObjectsMenus.Applications;
using TradeResourcesPlugin.Modules.LandObjectsMenus.Object;
using TradeResourcesPlugin.Modules.LandObjectsMenus.Trade;
using TradeResourcesPlugin.Modules.LandObjectsMenus.Trades;
using TradeResourcesPlugin.Modules.Menus;
using TradeResourcesPlugin.Modules.Menus.Comission;
using TradeResourcesPlugin.Modules.Menus.Objects;
using TradeResourcesPlugin.Modules.TelecomOperatorsMenus.Object;
using TradeResourcesPlugin.Modules.TelecomOperatorsMenus.Objects;
using TradeResourcesPlugin.Modules.TelecomOperatorsMenus.Trades;
using UsersResources;
using Yoda.Application;
using Yoda.Interfaces;
using Yoda.Interfaces.Forms;
using Yoda.Interfaces.Forms.Components;
using Yoda.Interfaces.Helpers;
using Yoda.Interfaces.Menu;
using Yoda.Interfaces.Permissions;
using YodaApp.UsersResources.QueryTables.V2;
using YodaCommonReferences;
using YodaHelpers;
using YodaHelpers.Fields;
using YodaHelpers.OrderHelpers;
using YodaHelpers.SearchCollections;
using YodaQuery;
using static TradeResourcesPlugin.Helpers.DefaultDocTemplate;
using RefKato = CommonSource.References.Object.RefKato;

namespace TradeResourcesPlugin {
    public class RegistersModule : YodaModule, IProjectTileProvider {
        public override string Text => "Реестры";

        public override QueryTable[] DataSchema() {
            return new QueryTable[] {

                new VwIacKatoMapping(),

                new TbComissionMembers(),

                new TbTaxOrgs(),
                new TbMsuOrgs(),
                new TbGuidIds(),

                new TbOfficialOrg(),
                new TbOfficialOrgRevisions(),
                new TbOfficialOrgOrderResult(),
                new TbOfficialOrgOrderNegotiations(),
                new TbOfficialOrgActivityTypes(),

                new TbDeprivedPersons(),
                
                #region Договоры

                new TbAgreements(),
                new TbAgreementModels(),
                new TbAgreementPdfs(),
                new TbAgreementSigns(),

                #endregion
                
                #region Платежи

                new TbPayments(),
                new TbPaymentMatches(),
                new TbPaymentMatchesRevisions(),
                new TbPaymentMatchesOrderResult(),
                new TbPaymentMatchesOrderNegotiations(),

                #endregion

                
                #region Углеводороды

                new HydrocarbonSource.QueryTables.Object.TbObjects(),
                new HydrocarbonSource.QueryTables.Object.TbObjectsRevisions(),
                new HydrocarbonSource.QueryTables.Object.TbObjectsOrderResult(),
                new HydrocarbonSource.QueryTables.Object.TbObjectsOrderNegotiations(),

                new HydrocarbonSource.QueryTables.Trade.TbTradeChanges(),
                new HydrocarbonSource.QueryTables.Trade.TbTrades(),
                new HydrocarbonSource.QueryTables.Trade.TbTradesRevisions(),
                new HydrocarbonSource.QueryTables.Trade.TbTradesOrderResult(),
                new HydrocarbonSource.QueryTables.Trade.TbTradesOrderNegotiations(),

                new HydrocarbonSource.QueryTables.Application.TbTradePreApplication(),
                new HydrocarbonSource.QueryTables.Application.TbTradePreApplicationApproovals(),
                new HydrocarbonSource.QueryTables.Application.TbTradePreApplicationSigns(),

                #endregion
                

                #region Земельные ресурсы

                new LandSource.QueryTables.Applications.TbLandApplications(),
                new LandSource.QueryTables.LandObject.TbLandObjects(),
                new LandSource.QueryTables.LandObject.TbLandObjectsRevisions(),
                new LandSource.QueryTables.LandObject.TbLandObjectsOrderResult(),
                new LandSource.QueryTables.LandObject.TbLandObjectsOrderNegotiations(),

                new LandSource.QueryTables.Trades.TbLandObjectsTrades(),
                new LandSource.QueryTables.Trades.TbLandObjectsTradesRevisions(),
                new LandSource.QueryTables.Trades.TbLandObjectsTradesOrderResult(),
                new LandSource.QueryTables.Trades.TbLandObjectsTradesOrderNegotiations(),
                new LandSource.QueryTables.Trades.TbLandObjectsTradesPublications(),
                new LandSource.QueryTables.Trades.TbLandObjectTradeChanges(),

                #endregion
                
                #region Рыба

                new FishingSource.QueryTables.Object.TbObjects(),
                new FishingSource.QueryTables.Object.TbObjectsRevisions(),
                new FishingSource.QueryTables.Object.TbObjectsOrderResult(),
                new FishingSource.QueryTables.Object.TbObjectsOrderNegotiations(),

                new FishingSource.QueryTables.Reservoir.TbReservoirs(),
                new FishingSource.QueryTables.Reservoir.TbReservoirsRevisions(),
                new FishingSource.QueryTables.Reservoir.TbReservoirOrderResult(),
                new FishingSource.QueryTables.Reservoir.TbReservoirOrdersNegotiations(),

                new FishingSource.QueryTables.Trade.TbTradeChanges(),
                new FishingSource.QueryTables.Trade.TbTrades(),
                new FishingSource.QueryTables.Trade.TbTradesRevisions(),
                new FishingSource.QueryTables.Trade.TbTradesOrderResult(),
                new FishingSource.QueryTables.Trade.TbTradesOrderNegotiations(),

                new FishingSource.QueryTables.Common.TbSellerSigners(),

                #endregion
                
                
                #region Охота

                new HuntingSource.QueryTables.Object.TbObjects(),
                new HuntingSource.QueryTables.Object.TbObjectsRevisions(),
                new HuntingSource.QueryTables.Object.TbObjectsOrderResult(),
                new HuntingSource.QueryTables.Object.TbObjectsOrderNegotiations(),

                new HuntingSource.QueryTables.Trade.TbTradeChanges(),
                new HuntingSource.QueryTables.Trade.TbTrades(),
                new HuntingSource.QueryTables.Trade.TbTradesRevisions(),
                new HuntingSource.QueryTables.Trade.TbTradesOrderResult(),
                new HuntingSource.QueryTables.Trade.TbTradesOrderNegotiations(),

                new HuntingSource.QueryTables.Common.TbSellerSigners(),

                #endregion
                
                
                #region Лес

                new ForestSource.QueryTables.Object.TbForestries(),
                new ForestSource.QueryTables.Object.TbForestriesRevisions(),
                new ForestSource.QueryTables.Object.TbForestriesOrderResult(),
                new ForestSource.QueryTables.Object.TbForestriesOrderNegotiations(),

                new ForestSource.QueryTables.Object.TbQuarters(),
                new ForestSource.QueryTables.Object.TbQuartersRevisions(),
                new ForestSource.QueryTables.Object.TbQuartersOrderResult(),
                new ForestSource.QueryTables.Object.TbQuartersOrderNegotiations(),

                new ForestSource.QueryTables.Object.TbForestryPieces(),
                new ForestSource.QueryTables.Object.TbForestryPiecesRevisions(),
                new ForestSource.QueryTables.Object.TbForestryPiecesOrderResult(),
                new ForestSource.QueryTables.Object.TbForestryPiecesOrderNegotiations(),

                new ForestSource.QueryTables.Object.TbObjects(),
                new ForestSource.QueryTables.Object.TbObjectsRevisions(),
                new ForestSource.QueryTables.Object.TbObjectsOrderResult(),
                new ForestSource.QueryTables.Object.TbObjectsOrderNegotiations(),

                new ForestSource.QueryTables.Trade.TbTradeChanges(),
                new ForestSource.QueryTables.Trade.TbTrades(),
                new ForestSource.QueryTables.Trade.TbTradesRevisions(),
                new ForestSource.QueryTables.Trade.TbTradesOrderResult(),
                new ForestSource.QueryTables.Trade.TbTradesOrderNegotiations(),

                new ForestSource.QueryTables.Common.TbSellerCreators(),

                #endregion

                #region Операторы

                new TelecomOperatorsSource.QueryTables.Object.TbObjects(),
                new TelecomOperatorsSource.QueryTables.Object.TbObjectsRevisions(),
                new TelecomOperatorsSource.QueryTables.Object.TbObjectsOrderResult(),
                new TelecomOperatorsSource.QueryTables.Object.TbObjectsOrderNegotiations(),

                new TelecomOperatorsSource.QueryTables.Trade.TbTradeChanges(),
                new TelecomOperatorsSource.QueryTables.Trade.TbTrades(),
                new TelecomOperatorsSource.QueryTables.Trade.TbTradesRevisions(),
                new TelecomOperatorsSource.QueryTables.Trade.TbTradesOrderResult(),
                new TelecomOperatorsSource.QueryTables.Trade.TbTradesOrderNegotiations(),

                #endregion
                
            };
        }

        public override void InitializationsCompleted(IYodaAppEnvInit env) {
            FieldsHandlers.Instance.RegisterHander(new CommonSource.FieldEditors.FieldHandler());
            FieldsHandlers.Instance.RegisterHander(new FishingSource.FieldEditors.FieldHandler());
            FieldsHandlers.Instance.RegisterHander(new HuntingSource.FieldEditors.FieldHandler());
            FieldsHandlers.Instance.RegisterHander(new ForestSource.FieldEditors.FieldHandler());
            FieldsHandlers.Instance.RegisterHander(new HydrocarbonSource.FieldEditors.FieldHandler());

            SearchCollectionsProvider.Instance.Add(GrObjectSearchCollection.CollectionName, new GrObjectSearchCollection());
            SearchCollectionsProvider.Instance.Add(GrObjectsWithKatoSearchCollection.CollectionName, new GrObjectsWithKatoSearchCollection());
            SearchCollectionsProvider.Instance.Add(GbdFlObjectCollection.CollectionName, new GbdFlObjectCollection());
            SearchCollectionsProvider.Instance.Add(GbdUlObjectCollection.CollectionName, new GbdUlObjectCollection());
            SearchCollectionsProvider.Instance.Add(ReasonForInclusionSearchCollection.CollectionName, new ReasonForInclusionSearchCollection());

            SearchCollectionsProvider.Instance.Add(TaxOrgsSearchCollection.CollectionName, new TaxOrgsSearchCollection());
            SearchCollectionsProvider.Instance.Add(TaxAndMsuOrgsSearchCollection.CollectionName, new TaxAndMsuOrgsSearchCollection());

            #region Углеводороды

            SearchCollectionsProvider.Instance.Add(HydrocarbonObjectsSearchCollection.CollectionName, new HydrocarbonObjectsSearchCollection());

            #endregion

            #region Земельные ресурсы

            SearchCollectionsProvider.Instance.Add(LandObjectsSearchCollection.CollectionName, new LandObjectsSearchCollection());
            SearchCollectionsProvider.Instance.Add(LandObjectsTradesSearchCollection.CollectionName, new LandObjectsTradesSearchCollection());

            #endregion

            #region Рыба

            SearchCollectionsProvider.Instance.Add(FishingObjectsSearchCollection.CollectionName, new FishingObjectsSearchCollection());
            SearchCollectionsProvider.Instance.Add(ReservoirSearchCollection.CollectionName, new ReservoirSearchCollection());
            SearchCollectionsProvider.Instance.Add(FaunaSearchCollection.CollectionName, new FaunaSearchCollection());

            #endregion

            #region Охота

            SearchCollectionsProvider.Instance.Add(HuntingObjectsSearchCollection.CollectionName, new HuntingObjectsSearchCollection());

            #endregion

            #region Лес

            SearchCollectionsProvider.Instance.Add(ForestriesSearchCollection.CollectionName, new ForestriesSearchCollection());
            SearchCollectionsProvider.Instance.Add(QuartersSearchCollection.CollectionName, new QuartersSearchCollection());
            SearchCollectionsProvider.Instance.Add(ForestryPiecesSearchCollection.CollectionName, new ForestryPiecesSearchCollection());
            SearchCollectionsProvider.Instance.Add(ForestObjectsSearchCollection.CollectionName, new ForestObjectsSearchCollection());

            #endregion

            #region Операторы

            SearchCollectionsProvider.Instance.Add(TelecomOperatorsObjectsSearchCollection.CollectionName, new TelecomOperatorsObjectsSearchCollection());

            #endregion
        }

        public override ReferenceInfo[] ReferenceItems(IQueryExecuter queryExecuter) {
            return new ReferenceInfo[] {


                new ReferenceInfo(RefGrObjectsOpf.RefName, () => new RefGrObjectsOpf()),
                new ReferenceInfo(RefApplicantType.RefName, () => new RefApplicantType()),
                new ReferenceInfo(RefPreapplicationSigners.RefName, () => new RefPreapplicationSigners()),
                new ReferenceInfo(RefDeprivedPersonsStatus.RefName, () => new RefDeprivedPersonsStatus()),
                new ReferenceInfo(RefReasonForInclusion.RefName, () => new RefReasonForInclusion()),
                new ReferenceInfo(RefPreApplicationApprovalSteps.RefName, () => new RefPreApplicationApprovalSteps()),
                new ReferenceInfo(RefPreApplicationApprovalStatuses.RefName, () => new RefPreApplicationApprovalStatuses()),
                new ReferenceInfo(RefOfficialOrgResourcesTypes.RefName, () => new RefOfficialOrgResourcesTypes()),

                new ReferenceInfo(nameof(RefKato), () => new RefKato()),
                new ReferenceInfo(RefAr.RefName, () => RefAr.Instance),

                new ReferenceInfo(nameof(RefTradeTypes), () => new RefTradeTypes()),
                new ReferenceInfo(nameof(RefTradeStatuses), () => new RefTradeStatuses()),
                new ReferenceInfo(nameof(RefTradeStatusesForCanceling), () => new RefTradeStatusesForCanceling()),
                new ReferenceInfo(nameof(RefTradeCommissionMembersRoles), () => new RefTradeCommissionMembersRoles()),
                new ReferenceInfo(nameof(RefTradeOperationTypes), () => new RefTradeOperationTypes()),
                new ReferenceInfo(nameof(RefComissionStatus), () => new RefComissionStatus()),



                new ReferenceInfo(nameof(RefTaxOrgStatuses), () => new RefTaxOrgStatuses()),

                new ReferenceInfo(nameof(RefPaymentMatchesOrderTypes), () => new RefPaymentMatchesOrderTypes()),


                new ReferenceInfo(nameof(RefAgreementStatuses), () => new RefAgreementStatuses()),
                new ReferenceInfo(nameof(RefAgreementTypes), () => new RefAgreementTypes()),
                new ReferenceInfo(nameof(RefAgreementObjectTypes), () => new RefAgreementObjectTypes()),
                new ReferenceInfo(nameof(RefAgreementTradeTypes), () => new RefAgreementTradeTypes()),
                new ReferenceInfo(nameof(RefPaymentStatuses), () => new RefPaymentStatuses()),
                new ReferenceInfo(nameof(RefPaymentMatchStatuses), () => new RefPaymentMatchStatuses()),
                
                #region Углеводороды
                new ReferenceInfo(HydrocarbonSource.References.Object.RefObjectStatuses.RefName, () => new HydrocarbonSource.References.Object.RefObjectStatuses()),
                new ReferenceInfo(HydrocarbonSource.QueryTables.Object.RefObjectOrderTypes.RefName, () => new HydrocarbonSource.QueryTables.Object.RefObjectOrderTypes()),

                new ReferenceInfo(HydrocarbonSource.References.Trade.RefTradeStatuses.RefName, () => new HydrocarbonSource.References.Trade.RefTradeStatuses()),
                new ReferenceInfo(HydrocarbonSource.References.Trade.RefTradeStatusesForCanceling.RefName, () => new HydrocarbonSource.References.Trade.RefTradeStatusesForCanceling()),
                new ReferenceInfo(HydrocarbonSource.References.Trade.RefTradeTypes.RefName, () => new HydrocarbonSource.References.Trade.RefTradeTypes()),
                new ReferenceInfo(HydrocarbonSource.References.Trade.RefKbk.RefName, () => new HydrocarbonSource.References.Trade.RefKbk()),
                new ReferenceInfo(HydrocarbonSource.QueryTables.Trade.RefTradeOrderTypes.RefName, () => new HydrocarbonSource.QueryTables.Trade.RefTradeOrderTypes()),
                new ReferenceInfo(nameof(HydrocarbonSource.References.Trade.RefTradeAdditionalConditions), () => new HydrocarbonSource.References.Trade.RefTradeAdditionalConditions()),
                new ReferenceInfo(nameof(HydrocarbonSource.References.Trade.RefTradeOperationTypes), () => new HydrocarbonSource.References.Trade.RefTradeOperationTypes()),

                new ReferenceInfo(nameof(HydrocarbonSource.References.Application.RefPreApplicationApprovalStatuses), () => new HydrocarbonSource.References.Application.RefPreApplicationApprovalStatuses()),
                new ReferenceInfo(nameof(HydrocarbonSource.References.Application.RefPreApplicationApprovalSteps), () => new HydrocarbonSource.References.Application.RefPreApplicationApprovalSteps()),
                new ReferenceInfo(nameof(HydrocarbonSource.References.Application.RefPreapplicationSigners), () => new HydrocarbonSource.References.Application.RefPreapplicationSigners()),
                new ReferenceInfo(nameof(HydrocarbonSource.References.Application.RefPreApplicationStatus), () => new HydrocarbonSource.References.Application.RefPreApplicationStatus()),
                #endregion

                #region Земельные ресурсы

                new ReferenceInfo(LandSource.References.Applications.RefApplicationStatuses.RefName, () => new LandSource.References.Applications.RefApplicationStatuses()),
                new ReferenceInfo(LandSource.References.LandObject.RefBoolChargePresence.RefName, () => new LandSource.References.LandObject.RefBoolChargePresence()),
                new ReferenceInfo(LandSource.References.LandObject.RefBoolRegistered.RefName, () => new LandSource.References.LandObject.RefBoolRegistered()),
                new ReferenceInfo(LandSource.References.LandObject.RefGrKfs.RefName, () => LandSource.References.LandObject.RefGrKfs.GetReference("2019", queryExecuter)),
                new ReferenceInfo(LandSource.References.LandObject.RefLandObjectStatuses.RefName, () => new LandSource.References.LandObject.RefLandObjectStatuses()),
                new ReferenceInfo(LandSource.References.LandObject.RefLandObjectTypes.RefName, () => new LandSource.References.LandObject.RefLandObjectTypes()),
                new ReferenceInfo(LandSource.References.LandObject.RefUpgsLandObjectsFuncPurpose.RefName, () => LandSource.References.LandObject.RefUpgsLandObjectsFuncPurpose.GetReference(queryExecuter)),
                new ReferenceInfo(LandSource.References.LandObject.RefUpgsLandObjectsIsDivisible.RefName, () => LandSource.References.LandObject.RefUpgsLandObjectsIsDivisible.GetReference(queryExecuter)),
                new ReferenceInfo(LandSource.References.LandObject.RefUpgsLandObjectsOwnershipType.RefName, () => LandSource.References.LandObject.RefUpgsLandObjectsOwnershipType.GetReference(queryExecuter)),
                new ReferenceInfo(LandSource.References.LandObject.RefUpgsLandObjectsTechnicalConditions.RefName, () => new LandSource.References.LandObject.RefUpgsLandObjectsTechnicalConditions()),
                new ReferenceInfo(LandSource.References.LandObject.RefUpgsLandObjectsUsageAim.RefName, () => LandSource.References.LandObject.RefUpgsLandObjectsUsageAim.GetReference(queryExecuter)),
                new ReferenceInfo(LandSource.References.LandObject.RefLandObjectDataSources.RefName, () => new LandSource.References.LandObject.RefLandObjectDataSources()),
                new ReferenceInfo(LandSource.References.LandObject.RefLandObjectDrawGeom.RefName, () => new LandSource.References.LandObject.RefLandObjectDrawGeom()),
                new ReferenceInfo(LandSource.References.LandObject.RefIisLandTypes.RefName, () => LandSource.References.LandObject.RefIisLandTypes.GetReference(queryExecuter)),
                new ReferenceInfo(LandSource.QueryTables.LandObject.RefObjectOrderTypes.RefName, () => new LandSource.QueryTables.LandObject.RefObjectOrderTypes()),


                new ReferenceInfo(LandSource.References.Trades.RefConditionParticipants.RefName, () => new LandSource.References.Trades.RefConditionParticipants()),
                new ReferenceInfo(LandSource.References.Trades.RefCostMinCondition.RefName, () => new LandSource.References.Trades.RefCostMinCondition()),
                new ReferenceInfo(LandSource.References.Trades.RefKbk.RefName, () => new LandSource.References.Trades.RefKbk()),
                new ReferenceInfo(LandSource.References.Trades.RefLandLeaseAvailable.RefName, () => new LandSource.References.Trades.RefLandLeaseAvailable()),
                new ReferenceInfo(LandSource.References.Trades.RefLandObjectTradeForms.RefName, () => new LandSource.References.Trades.RefLandObjectTradeForms()),
                new ReferenceInfo(LandSource.References.Trades.RefLandObjectTradeStatuses.RefName, () => new LandSource.References.Trades.RefLandObjectTradeStatuses()),
                new ReferenceInfo(LandSource.References.Trades.RefLandObjectTradeTypes.RefName, () => new LandSource.References.Trades.RefLandObjectTradeTypes()),
                new ReferenceInfo(LandSource.References.Trades.RefOwnershipMonths.RefName, () => new LandSource.References.Trades.RefOwnershipMonths()),
                new ReferenceInfo(LandSource.QueryTables.Trades.RefTradeOrderTypes.RefName, () => new LandSource.QueryTables.Trades.RefTradeOrderTypes()),
                new ReferenceInfo(LandSource.References.Trades.RefNewspapers.RefName, () => LandSource.References.Trades.RefNewspapers.GetReference(queryExecuter)),

            #endregion

                #region Рыба
                new ReferenceInfo(FishingSource.References.Object.RefObjectStatuses.RefName, () => new FishingSource.References.Object.RefObjectStatuses()),
                new ReferenceInfo(FishingSource.References.Object.RefPropertyLevel.RefName, () => new FishingSource.References.Object.RefPropertyLevel()),
                new ReferenceInfo(FishingSource.References.Object.RefFauna.RefName, () => new FishingSource.References.Object.RefFauna()),
                new ReferenceInfo(FishingSource.References.Object.RefFaunaDetailed.RefName, () => new FishingSource.References.Object.RefFaunaDetailed()),
                new ReferenceInfo(FishingSource.QueryTables.Object.RefObjectOrderTypes.RefName, () => new FishingSource.QueryTables.Object.RefObjectOrderTypes()),

                new ReferenceInfo(FishingSource.References.Reservoir.RefReservoirStatuses.RefName, () => new FishingSource.References.Reservoir.RefReservoirStatuses()),
                new ReferenceInfo(FishingSource.QueryTables.Reservoir.RefReservoirOrderTypes.RefName, () => new FishingSource.QueryTables.Reservoir.RefReservoirOrderTypes()),
                new ReferenceInfo(FishingSource.QueryTables.Trade.RefTradeOrderTypes.RefName, () => new FishingSource.QueryTables.Trade.RefTradeOrderTypes()),

                new ReferenceInfo(FishingSource.References.Trade.RefTradesStatuses.RefName, () => new FishingSource.References.Trade.RefTradesStatuses()),
                new ReferenceInfo(FishingSource.References.Trade.RefKbk.RefName, () => new FishingSource.References.Trade.RefKbk()),
                #endregion

                #region Охота
                new ReferenceInfo(HuntingSource.References.Object.RefAnimalTypes.RefName, () => new HuntingSource.References.Object.RefAnimalTypes()),
                new ReferenceInfo(HuntingSource.References.Object.RefBiotechnicalActivities.RefName, () => new HuntingSource.References.Object.RefBiotechnicalActivities()),
                new ReferenceInfo(HuntingSource.References.Object.RefBuildingTypes.RefName, () => new HuntingSource.References.Object.RefBuildingTypes()),
                new ReferenceInfo(HuntingSource.References.Object.RefHarmfulAnimalTypes.RefName, () => new HuntingSource.References.Object.RefHarmfulAnimalTypes()),
                new ReferenceInfo(HuntingSource.References.Object.RefHunterAnimalTypes.RefName, () => new HuntingSource.References.Object.RefHunterAnimalTypes()),
                new ReferenceInfo(HuntingSource.References.Object.RefObjectCategories.RefName, () => new HuntingSource.References.Object.RefObjectCategories()),
                new ReferenceInfo(HuntingSource.References.Object.RefObjectStatuses.RefName, () => new HuntingSource.References.Object.RefObjectStatuses()),
                new ReferenceInfo(HuntingSource.References.Object.RefRuleTypes.RefName, () => new HuntingSource.References.Object.RefRuleTypes()),
                new ReferenceInfo(HuntingSource.References.Object.RefTransportTypes.RefName, () => new HuntingSource.References.Object.RefTransportTypes()),
                new ReferenceInfo(HuntingSource.References.Object.RefWeightUnits.RefName, () => new HuntingSource.References.Object.RefWeightUnits()),
                new ReferenceInfo(HuntingSource.References.Object.RefWildAnimalsFeedTypes.RefName, () => new HuntingSource.References.Object.RefWildAnimalsFeedTypes()),
                new ReferenceInfo(HuntingSource.References.Object.RefWildfowlTypes.RefName, () => new HuntingSource.References.Object.RefWildfowlTypes()),
                new ReferenceInfo(HuntingSource.References.Object.RefWorkerTypes.RefName, () => new HuntingSource.References.Object.RefWorkerTypes()),
                new ReferenceInfo(HuntingSource.References.Object.RefWorkTypes.RefName, () => new HuntingSource.References.Object.RefWorkTypes()),
                new ReferenceInfo(HuntingSource.References.Object.RefWorkUnits.RefName, () => new HuntingSource.References.Object.RefWorkUnits()),
                new ReferenceInfo(HuntingSource.QueryTables.Object.RefObjectOrderTypes.RefName, () => new HuntingSource.QueryTables.Object.RefObjectOrderTypes()),

                new ReferenceInfo(HuntingSource.References.Trade.RefTradesStatuses.RefName, () => new HuntingSource.References.Trade.RefTradesStatuses()),
                new ReferenceInfo(HuntingSource.References.Trade.RefKbk.RefName, () => new HuntingSource.References.Trade.RefKbk()),
                new ReferenceInfo(HuntingSource.QueryTables.Trade.RefTradeOrderTypes.RefName, () => new HuntingSource.QueryTables.Trade.RefTradeOrderTypes()),
                #endregion
                
                #region Лес
                new ReferenceInfo(nameof(ForestSource.References.Object.RefFellingTypes), () => new ForestSource.References.Object.RefFellingTypes()),
                new ReferenceInfo(nameof(ForestSource.References.Object.RefForestFundCategories), () => new ForestSource.References.Object.RefForestFundCategories()),
                new ReferenceInfo(nameof(ForestSource.References.Object.RefForestryQuarterStatuses), () => new ForestSource.References.Object.RefForestryQuarterStatuses()),
                new ReferenceInfo(nameof(ForestSource.References.Object.RefForestryPieceStatuses), () => new ForestSource.References.Object.RefForestryPieceStatuses()),
                new ReferenceInfo(nameof(ForestSource.References.Object.RefForestryTypes), () => new ForestSource.References.Object.RefForestryTypes()),
                new ReferenceInfo(ForestSource.References.Object.RefObjectStatuses.RefName, () => new ForestSource.References.Object.RefObjectStatuses()),
                new ReferenceInfo(nameof(ForestSource.References.Object.RefForestUsing), () => new ForestSource.References.Object.RefForestUsing()),
                new ReferenceInfo(nameof(ForestSource.References.Object.RefRestrictions), () => new ForestSource.References.Object.RefRestrictions()),
                new ReferenceInfo(nameof(ForestSource.QueryTables.Object.RefForestryOrderTypes), () => new ForestSource.QueryTables.Object.RefForestryOrderTypes()),
                new ReferenceInfo(nameof(ForestSource.QueryTables.Object.RefQuarterOrderTypes), () => new ForestSource.QueryTables.Object.RefQuarterOrderTypes()),
                new ReferenceInfo(nameof(ForestSource.QueryTables.Object.RefForestryPieceOrderTypes), () => new ForestSource.QueryTables.Object.RefForestryPieceOrderTypes()),
                new ReferenceInfo(ForestSource.QueryTables.Object.RefObjectOrderTypes.RefName, () => new ForestSource.QueryTables.Object.RefObjectOrderTypes()),

                new ReferenceInfo(ForestSource.References.Trade.RefTradesStatuses.RefName, () => new ForestSource.References.Trade.RefTradesStatuses()),
                new ReferenceInfo(ForestSource.References.Trade.RefKbk.RefName, () => new ForestSource.References.Trade.RefKbk()),
                new ReferenceInfo(ForestSource.QueryTables.Trade.RefTradeOrderTypes.RefName, () => new ForestSource.QueryTables.Trade.RefTradeOrderTypes()),
                #endregion
                
                #region Операторы
                
                new ReferenceInfo(TelecomOperatorsSource.References.Object.RefObjectStatuses.RefName, () => new TelecomOperatorsSource.References.Object.RefObjectStatuses()),
                new ReferenceInfo(TelecomOperatorsSource.QueryTables.Object.RefObjectOrderTypes.RefName, () => new TelecomOperatorsSource.QueryTables.Object.RefObjectOrderTypes()),

                new ReferenceInfo(TelecomOperatorsSource.References.Trade.RefTradesStatuses.RefName, () => new TelecomOperatorsSource.References.Trade.RefTradesStatuses()),
                new ReferenceInfo(TelecomOperatorsSource.References.Trade.RefKbk.RefName, () => new TelecomOperatorsSource.References.Trade.RefKbk()),
                new ReferenceInfo(TelecomOperatorsSource.QueryTables.Trade.RefTradeOrderTypes.RefName, () => new TelecomOperatorsSource.QueryTables.Trade.RefTradeOrderTypes()),

                #endregion

            };
        }

        public override Permission[] Permissions(IQueryExecuter queryExecuter) {
            //var ret = new List<Permission> {
            //    new Permission(LocalPermissions.InternalUsers, "Администратор"),
            //    new Permission(LocalPermissions.Landlords, "Компетентный орган"),
            //    new Permission(LocalPermissions.Tenants, "Участники")
            //};

            var ret = new List<Permission>();

            ret.AddRange(AccountActivityTypesProvider.OfficialOrgOrders);

            ret.AddRange(AccountActivityTypesProvider.SubsoilObjectsOrders);
            ret.AddRange(AccountActivityTypesProvider.SubsoilTradesOrders);
            ret.AddRange(new[] {
                AccountActivityTypesProvider.SubsoilAppNegotiation,
                AccountActivityTypesProvider.SubsoilAppExecute,
                AccountActivityTypesProvider.SubsoilAgreementCreate,
                AccountActivityTypesProvider.SubsoilAgreementSign
            });

            ret.AddRange(AccountActivityTypesProvider.LandObjectsOrders);
            ret.AddRange(AccountActivityTypesProvider.LandTradesOrders);
            ret.AddRange(new[] {
                AccountActivityTypesProvider.LandAgreementCreate,
                AccountActivityTypesProvider.LandAgreementSign
            });

            ret.AddRange(AccountActivityTypesProvider.HuntingObjectsOrders);
            ret.AddRange(AccountActivityTypesProvider.HuntingTradesOrders);
            ret.AddRange(new[] {
                AccountActivityTypesProvider.HuntingAgreementCreate,
                AccountActivityTypesProvider.HuntingAgreementSign
            });

            ret.AddRange(AccountActivityTypesProvider.FishingObjectsOrders);
            ret.AddRange(AccountActivityTypesProvider.FishingTradesOrders);
            ret.AddRange(new[] {
                AccountActivityTypesProvider.FishingAgreementCreate,
                AccountActivityTypesProvider.FishingAgreementSign
            });

            ret.AddRange(AccountActivityTypesProvider.ForestOrders);
            ret.AddRange(AccountActivityTypesProvider.ForestObjectsOrders);
            ret.AddRange(AccountActivityTypesProvider.ForestTradesOrders);
            ret.AddRange(new[] {
                AccountActivityTypesProvider.ForestAgreementCreate,
                AccountActivityTypesProvider.ForestAgreementSign
            });

            ret.AddRange(AccountActivityTypesProvider.PaymentOrders);

            return ret.ToArray();
        }

        public override MenuLink[] Menu() {
            var ret = new List<MenuLink> {

                new EmptyMenuLink("MnuOfficialOrgEmpty", "Администрирование", "traderesources-common") { 
                    ProjectsConfig = new ProjectsConfiguration {
                        ProjectsList = new ProjectsList{
                            Projects = new []{ "cabinetResourceSeller", "traderesources" }
                        }
                    },
                    IconCssClass = "uil uil-users-alt",
                    EnabledFunc = rc => {
                        if (rc.Project == "traderesources" && rc.User.IsExternalUser()) {
                            return false;
                        } else
                        {
                            return true;
                        }
                    }
                },

                new MnuObjectViewFromMap(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/view-object-from-map/{ObjectId}/{ObjectType}")
                    .AsCallback(),

                new MnuCommissionMembersSearch()
                    .ProjectsConfig("traderesources", "cabinetResourceSeller")
                    .Path("traderesources-common/commission-members"),
                new MnuCommissionActions(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller")
                    .Path("traderesources-common/commission-members/actions"),


                #region Недобросовестные заявители

                new MnuDeprivedPersonsReestrActions(ModuleName)
                    .ProjectsConfig("cabinetResourceSeller"),
                new MnuDeprivedPersonsReestrSearch(ModuleName)
                    .ProjectsConfig("cabinetResourceSeller"),

                #endregion


                #region Компетентные органы

                new EmptyMenuLink("MnuOfficialOrgInteranlEmpty", "Субъекты", "subjects"){
                    ProjectsConfig = new ProjectsConfiguration("traderesources"),
                    IconCssClass = "mdi mdi-account-tie"
                },
                new MnuOfficialOrgList(ModuleName, AccountActivityTypesProvider.OfficialOrgOrders)
                    .ProjectsConfig("traderesources")
                    .Path("subjects/official-orgs"),
                new MnuOfficialOrg(ModuleName, AccountActivityTypesProvider.OfficialOrgOrders)
                    .ProjectsConfig("traderesources")
                    .Path("subjects/official-orgs/{OrgId}/{MenuAction}"),
                new MnuOfficialOrgOrderList(ModuleName, AccountActivityTypesProvider.OfficialOrgOrders)
                    .ProjectsConfig("traderesources")
                    .Path("subjects/official-orgs-orders"),
                new MnuOfficialOrgOrder(ModuleName, AccountActivityTypesProvider.OfficialOrgOrders)
                    .ProjectsConfig("traderesources")
                    .Path("subjects/official-orgs-orders/{MenuAction}"),

                #endregion


                #region Договоры
                new EmptyMenuLink("MnuEmptyAgreements") {
                    ProjectsConfig = new ProjectsConfiguration{ProjectsList = new ProjectsList{ Projects = new []{ "traderesources", "cabinetResourceSeller", "cabinetResourceUser" } }},
                    LinkText = "Договоры",
                    IconCssClass = "uil uil-books",
                    Path = "traderesources/agreements"
                },
                new MnuDefaultAgrSearch(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/agreements/register"),
                new MnuDefaultAgrCheck(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/agreements/check"),
                new MnuDefaultAgrWizard(ModuleName, "MnuDefaultAgrWizard", "Договор")
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/agreements/register/agreement"),
                new MnuPaymentMatchOrderList(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/agreements/payment-orders"),
                new MnuPaymentMatchOrder(ModuleName, AccountActivityTypesProvider.PaymentOrders)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/agreements/payment-orders/revision"),
                #endregion
                
                new DefaultDocPdfMnu(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/agreements/pdf"),

                #region Углеводороды
                
                new MnuSubsoilsViewMap("T", RefTradeObjectTypes.Values.HYDROCARBON)
                    .ProjectsConfig("traderesources")
                    .Path("traderesources/res-subsoil/res-hydrocarbon/map"),
                new MnuHydrocarbonStatistics(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-subsoil/res-hydrocarbon/stat"),
                new MnuSubsoilsViewMap("C", RefTradeObjectTypes.Values.HYDROCARBON)
                    .ProjectsConfig("cabinetResourceSeller")
                    .Path("traderesources/res-subsoil/res-hydrocarbon/map-c"),
                new MnuSubsoilsViewMap("U", RefTradeObjectTypes.Values.HYDROCARBON)
                    .ProjectsConfig("cabinetResourceUser")
                    .Path("traderesources/res-subsoil/res-hydrocarbon/map-u"),

                new MnuHydrocarbonAppsSearch(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-subsoil/res-hydrocarbon/apps"),
                new MnuHydrocarbonApp(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-subsoil/res-hydrocarbon/apps/{AppId}"),

                new EmptyMenuLink("MnuEmptyHydrocarbonObjectOperations") {
                    ProjectsConfig = new ProjectsConfiguration{ProjectsList = new ProjectsList{ Projects = new []{ "traderesources", "cabinetResourceSeller" } }},
                    LinkText = "Операции",
                    Path = "traderesources/res-subsoil/res-hydrocarbon/operations"
                },


                new MnuHydrocarbonObjectOrdersSearch(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller")
                    .Path("traderesources/res-subsoil/res-hydrocarbon/operations/object-orders"),
                new MnuHydrocarbonObjectOrder(ModuleName, nameof(MnuHydrocarbonObjectOrder), AccountActivityTypesProvider.SubsoilObjectsOrders)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller")
                    .Path("traderesources/res-subsoil/res-hydrocarbon/operations/object-orders/{MenuAction}"),

                new MnuHydrocarbonTradeOrdersSearch(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller")
                    .Path("traderesources/res-subsoil/res-hydrocarbon/operations/trade-orders"),
                new MnuHydrocarbonTradeOrder(ModuleName, nameof(MnuHydrocarbonTradeOrder), AccountActivityTypesProvider.SubsoilTradesOrders)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller")
                    .Path("traderesources/res-subsoil/res-hydrocarbon/operations/trade-orders/{MenuAction}"),



                new EmptyMenuLink("MnuEmptyHydrocarbonObjectregister") {
                    ProjectsConfig = new ProjectsConfiguration{ProjectsList = new ProjectsList{ Projects = new []{ "traderesources", "cabinetResourceSeller" } }},
                    LinkText = "Реестр",
                    Path = "traderesources/res-subsoil/res-hydrocarbon/register"
                },


                new MnuHydrocarbonObjectsSearch(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-subsoil/res-hydrocarbon/register/objects"),
                new MnuHydrocarbonObjectView(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-subsoil/res-hydrocarbon/register/objects/{MenuAction}"),

                new MnuHydrocarbonTradesSearch(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-subsoil/res-hydrocarbon/register/trades"),
                new MnuHydrocarbonTradeView(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-subsoil/res-hydrocarbon/register/trades/{tradeId}"),

                #endregion


                #region Земельные ресурсы

                new EmptyMenuLink("MnuEmptyLandApplications") {
                    ProjectsConfig = new ProjectsConfiguration{ProjectsList = new ProjectsList{ Projects = new []{ "traderesources", "cabinetResourceSeller", "cabinetResourceUser" } }},
                    LinkText = "Предложения о вынесении свободного земельного участка на торги",
                    Path = "traderesources/res-land/apps"
                },
                new MnuLandApplicationsSearch(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-land/apps/search"),
                new MnuCheckLandApplication(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-land/apps/check"),
                new MnuLandApplications(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-land/apps/model"),

                new MnuLandObjectStatistics(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-land/stat"),

                new EmptyMenuLink("MnuEmptyLandObjectOperations") {
                    ProjectsConfig = new ProjectsConfiguration{ProjectsList = new ProjectsList{ Projects = new []{ "traderesources", "cabinetResourceSeller" } }},
                    LinkText = "Операции",
                    Path = "traderesources/res-land/operations"
                },

                new MnuLandObjectsOrderSearch(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-land/operations/object-orders"),
                new MnuLandObjectOrderBase(ModuleName, nameof(MnuLandObjectOrderBase), AccountActivityTypesProvider.LandObjectsOrders)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller")
                    .Path("traderesources/res-land/operations/object-orders/{MenuAction}"),
                new MnuLandObjectsTradeOrdersSearch(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-land/operations/object-trade-orders"),
                new MnuLandObjectTradeOrderBase(ModuleName, nameof(MnuLandObjectTradeOrderBase), AccountActivityTypesProvider.LandTradesOrders)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller")
                    .Path("traderesources/res-land/operations/object-trade-orders/{MenuAction}"),

                new EmptyMenuLink("MnuEmptyLandObjectregister") {
                    ProjectsConfig = new ProjectsConfiguration{ProjectsList = new ProjectsList{ Projects = new []{ "traderesources", "cabinetResourceSeller", "cabinetResourceUser" } }},
                    LinkText = "Реестр",
                    Path = "traderesources/res-land/register"
                },
                new MnuLandObjectsSearch(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-land/register/objects"),
                new MnuLandObjectView(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-land/register/objects/{MenuAction}"),
                new MnuLandObjectsTradesSearch(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-land/register/object-trades"),
                new MnuLandObjectTradeView(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller")
                    .Path("traderesources/res-land/register/object-trades/{tradeId}"),


                new MnuSubsoilsViewMap("T", RefTradeObjectTypes.Values.LANDOBJECT)
                    .ProjectsConfig("traderesources")
                    .Path("traderesources/res-land/map"),
                new MnuSubsoilsViewMap("C",RefTradeObjectTypes.Values.LANDOBJECT)
                    .ProjectsConfig("cabinetResourceSeller")
                    .Path("traderesources/res-land/map-c"),
                new MnuSubsoilsViewMap("U",RefTradeObjectTypes.Values.LANDOBJECT)
                    .ProjectsConfig("cabinetResourceUser")
                    .Path("traderesources/res-land/map-u"),


                #endregion


                #region Рыба
                new Modules.FishingMenus.Objects.MnuSellerSigners(ModuleName, "MnuSellerSignersFish", "КРХ и территориальные инспекции (рыбохозяйственные водоёмы)")
                    .ProjectsConfig("traderesources", "cabinetResourceSeller")
                    .Path("traderesources-common/fishing-seller-signers"),
                new MnuFishingStatistics(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-animal/fishing/stat"),

                new EmptyMenuLink("MnuEmptyFishingObject") {
                    ProjectsConfig = new ProjectsConfiguration{ProjectsList = new ProjectsList{ Projects = new []{ "traderesources", "cabinetResourceSeller", "cabinetResourceUser" } }},
                    LinkText = "4.1 Рыбохозяйственные водоёмы",
                    Path = "traderesources/res-animal/fishing"
                },


                new EmptyMenuLink("MnuEmptyFishingObjectOperations") {
                    ProjectsConfig = new ProjectsConfiguration{ProjectsList = new ProjectsList{ Projects = new []{ "traderesources", "cabinetResourceSeller" } }},
                    LinkText = "Операции",
                    Path = "traderesources/res-animal/fishing/operations"
                },


                new MnuFishingObjectsOrderSearch(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-animal/fishing/operations/object-orders"),
                new MnuFishingObjectOrderBase(ModuleName, nameof(MnuFishingObjectOrderBase), AccountActivityTypesProvider.FishingObjectsOrders)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller")
                    .Path("traderesources/res-animal/fishing/operations/object-orders/{MenuAction}"),

                new MnuReservoirsOrdersSearch(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-animal/fishing/operations/reservoir-orders"),
                new MnuReservoirOrderBase(ModuleName, nameof(MnuReservoirOrderBase), AccountActivityTypesProvider.FishingObjectsOrders)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller")
                    .Path("traderesources/res-animal/fishing/operations/reservoir-orders/{MenuAction}"),

                new MnuFishingTradeOrdersSearch(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-animal/fishing/operations/trade-orders"),
                new MnuFishingTradeOrderBase(ModuleName, nameof(MnuFishingTradeOrderBase), AccountActivityTypesProvider.FishingTradesOrders)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller")
                    .Path("traderesources/res-animal/fishing/operations/trade-orders/{MenuAction}"),


                new EmptyMenuLink("MnuEmptyFishingObjectregister") {
                    ProjectsConfig = new ProjectsConfiguration{ProjectsList = new ProjectsList{ Projects = new []{ "traderesources", "cabinetResourceSeller" } }},
                    LinkText = "Реестр",
                    Path = "traderesources/res-animal/fishing/register"
                },


                new MnuFishingObjectsSearch(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-animal/fishing/register/objects"),
                new MnuFishingObjectView(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-animal/fishing/register/objects/{MenuAction}"),

                //new MnuUploadReservoirs(ModuleName)
                //    .Enabled((rc) => rc.User.HasPermission(ModuleName, AccountActivityTypesProvider.FishingObjectsOrders.ExecOrder.Name))
                //    .ProjectsConfig("traderesources")
                //    .Path("traderesources/res-animal/fishing/register/upload-reservoirs"),
                new MnuReservoirsSearch(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-animal/fishing/register/reservoirs"),
                new MnuReservoirView(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-animal/fishing/register/reservoirs/{MenuAction}"),

                new MnuFishingTradesSearch(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-animal/fishing/register/trades"),
                new MnuFishingTradeView(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-animal/fishing/register/trades/{tradeId}"),


                new MnuSubsoilsViewMap("T", RefTradeObjectTypes.Values.FISHINGOBJECT)
                    .ProjectsConfig("traderesources")
                    .Path("traderesources/res-animal/fishing/map"),
                new MnuSubsoilsViewMap("C",RefTradeObjectTypes.Values.FISHINGOBJECT)
                    .ProjectsConfig("cabinetResourceSeller")
                    .Path("traderesources/res-animal/fishing/map-c"),
                new MnuSubsoilsViewMap("U",RefTradeObjectTypes.Values.FISHINGOBJECT)
                    .ProjectsConfig("cabinetResourceUser")
                    .Path("traderesources/res-animal/fishing/map-u"),


                #endregion


                #region Охота
                new Modules.HuntingMenus.Objects.MnuSellerSigners(ModuleName, "MnuSellerSignersHunt", "Акиматы и территориальные инспекции (охотничьи угодья)")
                    .ProjectsConfig("traderesources", "cabinetResourceSeller")
                    .Path("traderesources-common/hunting-seller-signers"),
                new MnuHuntingStatistics(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-animal/hunting/stat"),

                new EmptyMenuLink("MnuEmptyHuntingObject") {
                    ProjectsConfig = new ProjectsConfiguration{ProjectsList = new ProjectsList{ Projects = new []{ "traderesources", "cabinetResourceSeller", "cabinetResourceUser" } }},
                    LinkText = "4.2 Охотничьи угодья",
                    Path = "traderesources/res-animal/hunting"
                },


                new EmptyMenuLink("MnuEmptyHuntingObjectOperations") {
                    ProjectsConfig = new ProjectsConfiguration{ProjectsList = new ProjectsList{ Projects = new []{ "traderesources", "cabinetResourceSeller" } }},
                    LinkText = "Операции",
                    Path = "traderesources/res-animal/hunting/operations"
                },


                new MnuHuntingObjectOrdersSearch(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-animal/hunting/operations/object-orders"),
                new MnuHuntingObjectOrderBase(ModuleName, nameof(MnuHuntingObjectOrderBase), AccountActivityTypesProvider.HuntingObjectsOrders)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller")
                    .Path("traderesources/res-animal/hunting/operations/object-orders/{MenuAction}"),

                new MnuHuntingTradeOrdersSearch(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-animal/hunting/operations/trade-orders"),
                new MnuHuntingTradeOrderBase(ModuleName, nameof(MnuHuntingTradeOrderBase), AccountActivityTypesProvider.HuntingTradesOrders)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller")
                    .Path("traderesources/res-animal/hunting/operations/trade-orders/{MenuAction}"),



                new EmptyMenuLink("MnuEmptyHuntingObjectregister") {
                    ProjectsConfig = new ProjectsConfiguration{ProjectsList = new ProjectsList{ Projects = new []{ "traderesources", "cabinetResourceSeller" } }},
                    LinkText = "Реестр",
                    Path = "traderesources/res-animal/hunting/register"
                },


                new MnuHuntingObjectsSearch(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-animal/hunting/register/objects"),
                new MnuHuntingObjectView(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-animal/hunting/register/objects/{MenuAction}"),

                new MnuHuntingTradesSearch(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-animal/hunting/register/trades"),
                new MnuHuntingTradeView(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-animal/hunting/register/trades/{tradeId}"),



                new MnuSubsoilsViewMap("T", RefTradeObjectTypes.Values.HUNTINGOBJECT)
                    .ProjectsConfig("traderesources")
                    .Path("traderesources/res-animal/hunting/map"),
                new MnuSubsoilsViewMap("C",RefTradeObjectTypes.Values.HUNTINGOBJECT)
                    .ProjectsConfig("cabinetResourceSeller")
                    .Path("traderesources/res-animal/hunting/map-c"),
                new MnuSubsoilsViewMap("U",RefTradeObjectTypes.Values.HUNTINGOBJECT)
                    .ProjectsConfig("cabinetResourceUser")
                    .Path("traderesources/res-animal/hunting/map-u"),


                #endregion
                

                #region Лес

                new MnuForestStatistics(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-forest/stat"),

                new Modules.ForestMenus.Objects.MnuSellerCreators(ModuleName, "MnuSellerCreatorsForest", "УПР и КГУ (лесные ресурсы)")
                    .ProjectsConfig("traderesources", "cabinetResourceSeller")
                    .Path("traderesources-common/forest-seller-creators"),



                new EmptyMenuLink("MnuEmptyForestObjectOperations") {
                    ProjectsConfig = new ProjectsConfiguration{ProjectsList = new ProjectsList{ Projects = new []{ "traderesources", "cabinetResourceSeller" } }},
                    LinkText = "Операции",
                    Path = "traderesources/res-forest/operations"
                },

                new MnuForestryOrdersSearch(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-forest/operations/forestry-orders"),
                new MnuForestryOrder(ModuleName, AccountActivityTypesProvider.ForestOrders)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller")
                    .Path("traderesources/res-forest/operations/forestry-orders/{MenuAction}"),
                
                new MnuQuarterOrdersSearch(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-forest/operations/quarter-orders"),
                new MnuQuarterOrder(ModuleName, AccountActivityTypesProvider.ForestOrders)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller")
                    .Path("traderesources/res-forest/operations/quarter-orders/{MenuAction}"),
                
                new MnuForestryPieceOrdersSearch(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-forest/operations/forestry-piece-orders"),
                new MnuForestryPieceOrder(ModuleName, AccountActivityTypesProvider.ForestOrders)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller")
                    .Path("traderesources/res-forest/operations/forestry-piece-orders/{MenuAction}"),

                new MnuForestObjectOrdersSearch(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-forest/operations/object-orders"),
                new MnuForestObjectOrder(ModuleName, nameof(MnuForestObjectOrder), AccountActivityTypesProvider.ForestObjectsOrders)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller")
                    .Path("traderesources/res-forest/operations/object-orders/{MenuAction}"),

                new MnuForestTradeOrdersSearch(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-forest/operations/trade-orders"),
                new MnuForestTradeOrder(ModuleName, nameof(MnuForestTradeOrder), AccountActivityTypesProvider.ForestTradesOrders)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller")
                    .Path("traderesources/res-forest/operations/trade-orders/{MenuAction}"),



                new EmptyMenuLink("MnuEmptyForestObjectregister") {
                    ProjectsConfig = new ProjectsConfiguration{ProjectsList = new ProjectsList{ Projects = new []{ "traderesources", "cabinetResourceSeller" } }},
                    LinkText = "Реестр",
                    Path = "traderesources/res-forest/register"
                },


                new MnuForestriesSearch(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-forest/register/forestries"),
                new MnuForestryView(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-forest/register/forestries/{MenuAction}"),
                
                new MnuQuartersSearch(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-forest/register/quarters"),
                new MnuQuarterView(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-forest/register/quarters/{MenuAction}"),
                
                new MnuForestryPiecesSearch(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-forest/register/forestry-pieces"),
                new MnuForestryPieceView(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-forest/register/forestry-pieces/{MenuAction}"),

                new MnuForestObjectsSearch(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-forest/register/objects"),
                new MnuForestObjectView(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-forest/register/objects/{MenuAction}"),

                new MnuForestTradesSearch(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-forest/register/trades"),
                new MnuForestTradeView(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-forest/register/trades/{tradeId}"),



                new MnuSubsoilsViewMap("T", RefTradeObjectTypes.Values.FORESTOBJECT)
                    .ProjectsConfig("traderesources")
                    .Path("traderesources/res-forest/map"),
                new MnuSubsoilsViewMap("C",RefTradeObjectTypes.Values.FORESTOBJECT)
                    .ProjectsConfig("cabinetResourceSeller")
                    .Path("traderesources/res-forest/map-c"),
                new MnuSubsoilsViewMap("U",RefTradeObjectTypes.Values.FORESTOBJECT)
                    .ProjectsConfig("cabinetResourceUser")
                    .Path("traderesources/res-forest/map-u"),


                #endregion
                
                #region Операторы

                new EmptyMenuLink("MnuEmptyOperatorsObjectOperations") {
                    ProjectsConfig = new ProjectsConfiguration{ProjectsList = new ProjectsList{ Projects = new []{ "traderesources", "cabinetResourceSeller" } }},
                    LinkText = "Операции",
                    Path = "traderesources/res-communication/operators/operations"
                },

                new MnuTelecomOperatorsObjectOrdersSearch(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-communication/operators/operations/object-orders"),
                new MnuTelecomOperatorsObjectOrder(ModuleName, AccountActivityTypesProvider.TelecomOperatorsObjectsOrders)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller")
                    .Path("traderesources/res-communication/operators/operations/object-orders/{MenuAction}"),

                new MnuTelecomOperatorsTradeOrdersSearch(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-communication/operators/operations/trade-orders"),
                new MnuTelecomOperatorsTradeOrder(ModuleName, AccountActivityTypesProvider.TelecomOperatorsTradesOrders)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller")
                    .Path("traderesources/res-communication/operators/operations/trade-orders/{MenuAction}"),


                new EmptyMenuLink("MnuEmptyOperatorsObjectRegister") {
                    ProjectsConfig = new ProjectsConfiguration{ProjectsList = new ProjectsList{ Projects = new []{ "traderesources", "cabinetResourceSeller" } }},
                    LinkText = "Реестр",
                    Path = "traderesources/res-communication/operators/register"
                },

                new MnuTelecomOperatorsObjectsSearch(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-communication/operators/register/objects"),
                new MnuTelecomOperatorsObjectView(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-communication/operators/register/objects/{MenuAction}"),

                new MnuTelecomOperatorsTradesSearch(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-communication/operators/register/trades"),
                new MnuTelecomOperatorsTradeView(ModuleName)
                    .ProjectsConfig("traderesources", "cabinetResourceSeller", "cabinetResourceUser")
                    .Path("traderesources/res-communication/operators/register/trades/{tradeId}"),


                new MnuSubsoilsViewMap("T", RefTradeObjectTypes.Values.TELECOMOPERATORSOBJECT)
                    .ProjectsConfig("traderesources")
                    .Path("traderesources/res-communication/operators/map"),
                new MnuSubsoilsViewMap("C",RefTradeObjectTypes.Values.TELECOMOPERATORSOBJECT)
                    .ProjectsConfig("cabinetResourceSeller")
                    .Path("traderesources/res-communication/operators/map-c"),
                new MnuSubsoilsViewMap("U",RefTradeObjectTypes.Values.TELECOMOPERATORSOBJECT)
                    .ProjectsConfig("cabinetResourceUser")
                    .Path("traderesources/res-communication/operators/map-u"),


                #endregion


                #region Твердые полезные ископаемые
                
                new MnuSubsoilsViewMap("T", RefTradeObjectTypes.Values.TPI)
                    .ProjectsConfig("traderesources")
                    .Path("traderesources/res-subsoil/res-tpi/map"),
                new MnuSubsoilsViewMap("C",RefTradeObjectTypes.Values.TPI)
                    .ProjectsConfig("cabinetResourceSeller")
                    .Path("traderesources/res-subsoil/res-tpi/map-c"),
                new MnuSubsoilsViewMap("U",RefTradeObjectTypes.Values.TPI)
                    .ProjectsConfig("cabinetResourceUser")
                    .Path("traderesources/res-subsoil/res-tpi/map-u"),

                #endregion

                new MnuTaxOrgsSearch(ModuleName)
                .IconCssClass("mdi mdi-bank")
                .ProjectsConfig("traderesources")
                .Path("operations/taxorgs"),

                new MnuTaxOrg(ModuleName)
                .ProjectsConfig("traderesources")
                .Path("operations/taxorgs/{MenuAction}"),

                new MnuTest(),
                //new MnuAnalyticsMap()

            };
            ret.Add(new FrmMenu("MnuReadinessProbeUrlV2", "Страница диагностики v2")
                .AsCallback()
                .Path("local-probe")
                .ProjectsConfig(ProjectsList.All)
                .OnRendering(re => {
                    re.Redirect.SetRedirectToJson(new JsonResult(new { Test = "Hell no World" }));
                })
            );
            ret.Add(new FrmMenu("GenRoles", "GenRoles")
                .AsCallback()
                .Path("gen-roles")
                .ProjectsConfig(ProjectsList.All)
                .OnRendering(re => {
                    var roles = AccountActivityTypesProvider.Roles();
                    re.Form.Append(new HtmlText(roles));
                })
            );
            ret.Add(new FrmMenu("ActionError", "")
                .AsCallback()
                .Path("action-error")
                .ProjectsConfig(ProjectsList.All)
                .OnRendering(re => {
                    re.Form.Append(new HtmlText("<div class='mdi mdi-emoticon-sad-outline text-muted text-center' style='font-size: 150px;margin-bottom: -50px;'></div>"));
                    re.Form.Append(new HtmlText("<div class='h1 text-muted text-center mb-5'>УПС!</div>"));
                })
            );
            ret.Add(new FrmMenu("MnuSuperUserAuthorizeByAnyUser", "Авторизация под другим пользователем")
                .Path("superuser-to-any-user-mnu")
                .Enabled(rc => rc.User.IsSuperUser)
                .ProjectsConfig(ProjectsList.All)
                .OnRendering(re => {
                    new TbUsers().flLogin.RenderCustomT(re.Form, re.AsFormEnv(), null);
                    re.Form.AddSubmitButton("sbmt", "Авторизоваться", "mt-1");
                })
                .OnValidating(ve => {
                    if (!ve.User.IsSuperUser)
                    {
                        ve.AsFormEnv().AddError("CommonError", "Вы не суперюзер!!!!!!");
                    }
                    new TbUsers().flLogin.Required().Validate(ve.AsFormEnv());
                    if (ve.AsFormEnv().IsValid)
                    {
                        if (string.IsNullOrEmpty(new TbUsers().flLogin.GetVal(ve.AsFormEnv())))
                        {
                            ve.AsFormEnv().AddError("flLogin", "Введите логин");
                        }
                    }
                })
                .OnProcessing(pe => {
                    pe.RequestContext.AppEnv.UserProvider.UserLogIn(pe.RequestContext, new TbUsers().flLogin.GetVal(pe.AsFormEnv()));
                })
            );
            return ret.ToArray();
        }
        
        public class subs {
            public string Name { get; set; }
            public string Region { get; set; }
            public string Info { get; set; }
            public decimal? Area { get; set; }
            public int? Blocks { get; set; }
            public List<crd> Coords { get; set; }
        }

        public class crd {
            public int Number { get; set; }
            public string longit { get; set; }
            public string latit { get; set; }
            public decimal X { get; set; }
            public decimal Y { get; set; }
        }

        public enum colNames {
            name,
            region,
            info,
            coordNum,
            lg,
            lm,
            ls,
            rg,
            rm,
            rs
        };

        public Task<ProjectTileInfo[]> GetInternalProjectTiles(IYodaRequestContext context) {
            var ret = new List<ProjectTileInfo> { };
            ret.Add(new ProjectTileInfo(context.T("Торги природными и национальными ресурсами"), "HtmlPagesModule", "traderesources-public-info", "", weight: 100, project: "traderesources") { });
            ret.Add(new ProjectTileInfo(context.T("Торги природными и национальными ресурсами - заявитель"), "HtmlPagesModule", "cabinetResourceUser-public-info", "", weight: 100, project: "cabinetResourceUser") { });
            return Task.FromResult(ret.ToArray());
        }

        public Task<ProjectTileInfo[]> GetProjectTiles(IYodaRequestContext context) {
            var ret = new List<ProjectTileInfo> { };

            ret.Add(new ProjectTileInfo(context.T("Торги природными и национальными ресурсами"), "HtmlPagesModule", "traderesources-public-info", "", weight: 100, project: "traderesources") { });
            if (context.User.IsCorporateUser(context.QueryExecuter)) {
                ret.Add(new ProjectTileInfo(context.T("Торги природными и национальными ресурсами - регистратор объектов"), "HtmlPagesModule", "cabinetResourceSeller-public-info", "", weight: 100, project: "cabinetResourceSeller") { });
            }
            ret.Add(new ProjectTileInfo(context.T("Торги природными и национальными ресурсами - заявитель"), "HtmlPagesModule", "cabinetResourceUser-public-info", "", weight: 100, project: "cabinetResourceUser") { });

            return Task.FromResult(ret.ToArray());
        }

        //public class LocalPermissions {
        //    public const string InternalUsers = "traderesources-internal-users";
        //    public const string Landlords = "traderesources-land-lords";
        //    public const string Tenants = "traderesources-tenants";
        //}
    }
}
