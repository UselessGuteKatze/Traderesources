using Yoda.Interfaces.Forms;
using Yoda.Interfaces.Forms.Components;
using YodaHelpers.ActionMenus;
using YodaHelpers.Fields;
using Yoda.Interfaces;
using LandSource.Components;
using LandSource.Helpers;
using LandSource.QueryTables.LandObject;
using LandSource.References.LandObject;
using TradeResourcesPlugin.Modules.LandObjectsMenus.Trade;
using LandSource.Models;
using LandSource.QueryTables.Trades;
using System.Collections.Generic;
using TradeResourcesPlugin.Helpers;
using Microsoft.AspNetCore.Mvc.Routing;
using UsersResources;
using YodaHelpers.SearchCollections;
using LandSource.SearchCollections.LandObject;
using CommonSource;
using YodaQuery;
using System;
using LandSource.SearchCollections.LandObjectsTrades;
using LandSource.References.Trades;
using YodaHelpers.OrderHelpers;
using System.Data;
using System.Linq;
using TradeResourcesPlugin.Modules.LandObjectsMenus.Trades;
using TradeResourcesPlugin.Modules.Components;
using YodaApp.Yoda.Interfaces.Forms.Components;
using TradeResourcesPlugin.Helpers.Agreements;
using TradeResourcesPlugin.Modules.LandObjectsMenus.Agreements;

namespace TradeResourcesPlugin.Modules.LandObjectsMenus.Object {
    public class MnuLandObjectView: MnuActionsExt<LandObjectViewActionQueryArgs> {
        public const string MnuName = nameof(MnuLandObjectView);

        public MnuLandObjectView(string moduleName) : base(moduleName, MnuName, "Объект") {
            AsCallback();
            //Enabled(rc => rc.User.IsAuthentificated);
        }

        public class Actions {
            public const string View = "view";
        }

        public const string ActionCssClass = "btn btn-sm btn-dark font-weight-normal text-wrap mr-2 mb-2";

        public override void Configure(ActionConfig<LandObjectViewActionQueryArgs> config) {
            config
                .OnAction(Actions.View, action => action
                    .IsValid(env => new OkResult())
                    .OnRendering(re => {
                        renderTradeCommitErrors(re);
                        renderObjectView(re.Form, re);
                    }));
        }

        private void renderTradeCommitErrors(RenderActionEnv<LandObjectViewActionQueryArgs> re)
        {
            var obj = LandObjectModelHelper.GetObjectModel(re.Args.Id, re.QueryExecuter);

            var objectSeller = obj.SallerBin;
            var currentUser = re.User.GetUserXin(re.QueryExecuter);
            var isObjectSeller = currentUser == objectSeller;
            var isInternal = !re.User.IsExternalUser() && !re.User.IsGuest();

            if (!isObjectSeller && !isInternal)
            {
                return;
            }

            var hasDataEdit = re.User.HasRole("TRADERESOURCES-Земельные ресурсы-Создание приказов", re.QueryExecuter)/*re.User.HasCustomRole("landobjects", "appLandEdit", re.QueryExecuter)*/;

            if (!hasDataEdit && !isInternal)
            {
                re.Form.AddComponent(new Panel("alert alert-danger mt-2") { Elements = new YodaFormElementCollection() { new HtmlText("У вас нет прав на создание или изменение данных или период их активности подощёл к концу. Обратитесь к главе организации или коллеге с доступом к распределению прав.") } });
            }

            if (obj.Block == LandObjectBlocks.ActiveToBeTraded.ToString())
            {
                var tbTrades = new TbLandObjectsTrades().AddFilter(t => t.flObjectId, obj.Id);
                var tradeRevisionId = tbTrades.Select(new FieldAlias[] { tbTrades.flRevisionId }, re.QueryExecuter).AsEnumerable().OrderByDescending(r => tbTrades.flRevisionId.GetRowVal(r)).Select(r => tbTrades.flRevisionId.GetRowVal(r)).ToArray()[0];

                var tbTradesChanges = new TbLandObjectTradeChanges().AddFilter(t => t.flRevisionId, tradeRevisionId);
                var message = tbTradesChanges.SelectScalar(t => t.flMessage, re.QueryExecuter);
                if (!string.IsNullOrEmpty(message))
                {
                    re.Form.AddComponent(new Panel("alert alert-danger mt-2") { Elements = new YodaFormElementCollection() { new HtmlText(message) } });
                }
            }

        }

        private void renderObjectView(WidgetBase widget, ActionEnv<LandObjectViewActionQueryArgs> env)
        {
            var objectModel = LandObjectModelHelper.GetObjectModel(env.Args.Id, env.QueryExecuter);

            var tbTrades = new TbLandObjectsTrades();
            tbTrades.AddFilter(tbTrades.flObjectId, objectModel.Id);
            tbTrades.AddFilterIn(tbTrades.flStatus, new[] { RefLandObjectTradeStatuses.Wait, RefLandObjectTradeStatuses.Held });
            tbTrades.OrderBy = new[] { new OrderField(tbTrades.flId, OrderType.Desc) };
            LandObjectTradeModel tradeModel = null;
            AgreementsModel agreementsModel = null;
            PaymentModel paymentModel = null;
            if (tbTrades.Count(env.QueryExecuter) > 0)
            {
                tradeModel = tbTrades.GetTradeModelFirst(env.QueryExecuter, null);

                agreementsModel = new TbAgreements()
                    .AddFilter(t => t.flObjectId, objectModel.Id)
                    .AddFilter(t => t.flTradeId, tradeModel.Id)
                    .AddFilter(t => t.flObjectType, ConditionOperator.Contain, "Land")
                    .AddFilterNot(t => t.flAgreementStatus, AgreementStatuses.Deleted)
                    .AddFilterNot(t => t.flAgreementStatus, AgreementStatuses.Canceled)
                    .GetAgreementsModelFirstOrDefault(env.QueryExecuter);

                if (agreementsModel != null)
                {
                    var tbPayments = new TbPayments()
                        .AddFilter(t => t.flAgreementId, agreementsModel.flAgreementId);
                    if (tbPayments.Count(env.QueryExecuter) > 0)
                    {
                        paymentModel = tbPayments.GetPaymentModelFirstOrDefault(env.QueryExecuter, null);
                    }
                }
            }

            var objectSeller = objectModel.SallerBin;
            var currentUser = env.User.GetUserXin(env.QueryExecuter);
            var isObjectSeller = currentUser == objectSeller && env.User.HasRole("TRADERESOURCES-Земельные ресурсы-Создание приказов", env.QueryExecuter);
            var isAgreementSigner = currentUser == objectSeller && env.User.HasRole("TRADERESOURCES-Земельные ресурсы-Создание договора", env.QueryExecuter);
            var isInternal = !env.User.IsExternalUser() && !env.User.IsGuest();

            var mainRow = new GridRow()
                .AppendTo(widget);
            new GridCol("col-md-9")
                .Append(renderObjectData(objectModel, isObjectSeller, isInternal, env))
                .Append(renderTradeData(objectModel, tradeModel, isObjectSeller, env))
                .Append(renderAgreementShortData(objectModel, tradeModel, agreementsModel, isObjectSeller, isAgreementSigner, env))
                .Append(PaymentHelper.RenderPaymentData(paymentModel, agreementsModel, isObjectSeller, isAgreementSigner, env))
                .AppendTo(mainRow);
            new GridCol("col-md-3")
                .Append(renderObjectTimeLine(objectModel, tradeModel, agreementsModel, paymentModel, env))
                .Append(renderObjectGeometry(objectModel, env))
                .AppendTo(mainRow);

        }

        private Card renderObjectData(LandObjectModel objectModel, bool isObjectSeller, bool isInternal, ActionEnv<LandObjectViewActionQueryArgs> env)
        {
            var objectData = new Card(objectModel.Name, objectModel.Status == LandObjectStatuses.Deleted.ToString() ? "text-decoration-line-through" : "");
            var objectDataActions = objectActions(objectModel, isObjectSeller, isInternal, env);
            if (objectDataActions.Count > 0)
            {
                new Panel("bg-white bordered border-top form-wizard-header mt-1 pt-2 pl-2").AppendRange(objectDataActions).AppendTo(objectData);
            }
            var tabPanel = new HyperTabs().AppendTo(objectData);

            var tbObject = new TbLandObjects();

            var objectShortData = new Panel();
            tbObject.flId.RenderCustom(objectShortData, env, objectModel.Id.ToString(), readOnly: true);
            tbObject.flType.RenderCustom(objectShortData, env, objectModel.Type, readOnly: true);
            tbObject.flStatus.RenderCustom(objectShortData, env, objectModel.Status, readOnly: true);
            tbObject.flBlock.RenderCustom(objectShortData, env, objectModel.Block, readOnly: true);
            tbObject.flKadNumber.RenderCustom(objectShortData, env, objectModel.KadNumber, readOnly: true);
            tbObject.flEgknId.RenderCustom(objectShortData, env, objectModel.EgknId, readOnly: true);
            tbObject.flLandArea.RenderCustom(objectShortData, env, objectModel.LandArea, readOnly: true);
            tbObject.flRegion.RenderCustom(objectShortData, env, objectModel.Region, readOnly: true);
            tbObject.flDistrict.RenderCustom(objectShortData, env, objectModel.District, readOnly: true);
            tbObject.flAddress.RenderCustom(objectShortData, env, objectModel.Address, readOnly: true);
            tabPanel.AddNewTab("Краткая информация", objectShortData);

            var objectFullData = new Panel();
            MnuLandObjectOrderBase.viewModelMainData(objectFullData, env, objectModel, true);
            MnuLandObjectOrderBase.viewModelAdditionalData(objectFullData, env, objectModel, true);
            MnuLandObjectOrderBase.viewModelGeoPositionData(objectFullData, env, objectModel, true);
            MnuLandObjectOrderBase.viewModelChargePresenceData(objectFullData, env, objectModel, true);
            tabPanel.AddNewTab("Подробная информация", objectFullData);

            return objectData;
        }

        private List<LinkBase> objectActions(LandObjectModel objectModel, bool isObjectSeller, bool isInternal, ActionEnv<LandObjectViewActionQueryArgs> env) {
            var actions = new List<LinkBase>();

            if (objectModel.Block == LandObjectBlocks.ActiveFree.ToString()) {
                if (isObjectSeller) {
                    actions.Add(new Link {
                        CssClass = ActionCssClass,
                        Text = env.T("Редактировать объект"),
                        Controller = ModuleName,
                        Action = nameof(MnuLandObjectOrderBase),
                        RouteValues = new LandObjectOrderQueryArgs { Id = objectModel.Id, RevisionId = objectModel.RevisionId, OrderType = LandObjectsOrderTypeActions.Edit, MenuAction = MnuLandObjectOrderBase.Actions.CreateFrom }
                    });
                    actions.Add(new Link {
                        CssClass = ActionCssClass,
                        Text = env.T("Удалить объект"),
                        Controller = ModuleName,
                        Action = nameof(MnuLandObjectOrderBase),
                        RouteValues = new LandObjectOrderQueryArgs { Id = objectModel.Id, RevisionId = objectModel.RevisionId, OrderType = LandObjectsOrderTypeActions.Remove, MenuAction = MnuLandObjectOrderBase.Actions.CreateFrom }
                    });
                }
            }
            if (isInternal) {
                actions.Add(new Link {
                    CssClass = ActionCssClass,
                    Text = env.T("Изменить продавца"),
                    Controller = ModuleName,
                    Action = nameof(MnuLandObjectOrderBase),
                    RouteValues = new LandObjectOrderQueryArgs { Id = objectModel.Id, RevisionId = objectModel.RevisionId, OrderType = LandObjectsOrderTypeActions.EditOwner, MenuAction = MnuLandObjectOrderBase.Actions.CreateFrom }
                });
            }
            return actions;
        }

        private Card renderTradeData(LandObjectModel objectModel, LandObjectTradeModel tradeModel, bool isObjectSeller, ActionEnv<LandObjectViewActionQueryArgs> env)
        {
            var tradeData = new Card(env.T("Торги"));
            var tradeDataActions = tradeActions(objectModel, tradeModel, isObjectSeller, env);
            if (tradeDataActions.Count > 0)
            {
                new Panel("bg-white bordered border-top form-wizard-header mt-1 pt-2 pl-2").AppendRange(tradeDataActions).AppendTo(tradeData);
            }
            var tabPanel = new HyperTabs().AppendTo(tradeData);

            var tbTrade = new TbLandObjectsTrades();

            if (tradeModel != null)
            {
                var tradeShortData = new Panel();
                tbTrade.flId.RenderCustom(tradeShortData, env, tradeModel.Id > 0 ? tradeModel.Id.ToString() : "", readOnly: true);
                tbTrade.flForm.RenderCustom(tradeShortData, env, tradeModel.Form, readOnly: true);
                tbTrade.flMethod.RenderCustom(tradeShortData, env, tradeModel.Method, readOnly: true);
                tbTrade.flType.RenderCustom(tradeShortData, env, tradeModel.Type, readOnly: true);
                tbTrade.flStatus.RenderCustom(tradeShortData, env, tradeModel.Status, readOnly: true);
                tbTrade.flDateTime.RenderCustom(tradeShortData, env, tradeModel.DateTime, readOnly: true);
                tbTrade.flDeposit.RenderCustom(tradeShortData, env, tradeModel.Deposit, readOnly: true);
                tbTrade.flCost.RenderCustom(tradeShortData, env, tradeModel.Cost, readOnly: true);
                tabPanel.AddNewTab("Краткая информация", tradeShortData);

                var tradeFullData = new Panel();
                MnuLandObjectTradeOrderBase.viewModelMainData(tradeFullData, env, tradeModel, true);
                if (tradeModel.InitialCost > 0)
                {
                    MnuLandObjectTradeOrderBase.viewModelRequisitesData(tradeFullData, env, tradeModel, true);
                }
                if (tradeModel.Method == RefLandObjectTradeForms.Values.LandTradesPriceDown)
                {
                    MnuLandObjectTradeOrderBase.viewModelPriceDownData(tradeFullData, env, tradeModel, true);
                }
                if (tradeModel.Type == RefLandObjectTradeTypes.RentOwnership)
                {
                    MnuLandObjectTradeOrderBase.viewModelRentData(tradeFullData, env, tradeModel, true);
                }
                if (tradeModel.Type == RefLandObjectTradeTypes.PrivateOwnership)
                {
                    MnuLandObjectTradeOrderBase.viewModelPrivateData(tradeFullData, env, tradeModel, true);
                }

                if (tradeModel.Form == RefLandObjectTradeForms.Values.EAuction)
                {
                    MnuLandObjectTradeOrderBase.viewModelCalculatedData(tradeFullData, env, tradeModel, true);
                }
                else if (tradeModel.Form == RefLandObjectTradeForms.Values.Contest)
                {
                    MnuLandObjectTradeOrderBase.viewModelContestData(tradeFullData, env, tradeModel, true);
                }

                if (!string.IsNullOrEmpty(tradeModel.WinnerBin))
                {
                    MnuLandObjectTradeOrderBase.viewModelSaleData(tradeFullData, env, tradeModel, true);
                }
                if (tradeModel.IsSaleProtocolExists == true)
                {
                    MnuLandObjectTradeOrderBase.viewModelProtocolData(tradeFullData, env, tradeModel, true);
                }
                if (tradeModel.IsSaleAgreementExists == true)
                {
                    MnuLandObjectTradeOrderBase.viewModelAgreementData(tradeFullData, env, tradeModel, true);
                }

                //MnuLandObjectTradeOrderBase.viewModelPublicationsData(tradeFullData, env, tradeModel, true);
                tabPanel.AddNewTab("Подробная информация", tradeFullData);
            }

            var tbTrades = new TbLandObjectsTrades();
            tbTrades.AddFilter(tbTrades.flObjectId, objectModel.Id);
            var tradesCount = tbTrades.Count(env.QueryExecuter);
            if ((tradeModel != null && tradesCount > 1) || (tradeModel == null && tradesCount > 0))
            {
                var objectTradesData = new TimeLine();
                tbTrades.Select(t => new FieldAlias[] { t.flId, t.flDateTime }, env.QueryExecuter).AsEnumerable().OrderBy(r => r.GetVal(t => t.flDateTime)).Each(r =>
                {
                    var tradeId = r.GetVal(t => t.flId);
                    var tradeDateTime = r.GetVal(t => t.flDateTime);
                    var isCurrentTrade = tradeModel != null && tradeModel.Id == tradeId;

                    objectTradesData
                        .Append(new TimeLineItem(env.T("Торги" + $" №{tradeId}"),
                            tradeDateTime.ToString("dd.MM.yyyy hh:mm"),
                            iconClass: isCurrentTrade ? "bg-success" : "bg-light",
                            messageElement: new Link
                            {
                                CssClass = "badge badge-dark font-weight-normal p-1 ml-2",
                                Text = env.T("Перейти"),
                                Controller = ModuleName,
                                Action = nameof(MnuLandObjectTradeView),
                                RouteValues = new MnuLandObjectTradeViewArgs { tradeId = tradeId }
                            }
                        )
                    );
                });
                tabPanel.AddNewTab("Все торги", objectTradesData);
            } 
            if (tradesCount == 0)
            {
                tradeData.Elements.Clear();
                if (tradeDataActions.Count > 0)
                {
                    new Panel("bg-white bordered border-top form-wizard-header mt-1 pt-2 pl-2").AppendRange(tradeDataActions).AppendTo(tradeData);
                }
            }

            return tradeData;
        }

        private List<LinkBase> tradeActions(LandObjectModel objectModel, LandObjectTradeModel tradeModel, bool isObjectSeller, ActionEnv<LandObjectViewActionQueryArgs> env)
        {
            var actions = new List<LinkBase>();

            if (objectModel.Block == LandObjectBlocks.ActiveFree.ToString() && tradeModel == null)
            {
                if (isObjectSeller)
                {
                    actions.Add(new Link
                    {
                        CssClass = ActionCssClass,
                        Text = env.T("Создать торги"),
                        Controller = ModuleName,
                        Action = nameof(MnuLandObjectTradeOrderBase),
                        RouteValues = new LandObjectTradeOrderQueryArgs { OrderType = LandObjectTradesOrderTypeActions.Create, MenuAction = "create-new", ObjectId = objectModel.Id, RevisionId = -1, OwnerXin = objectModel.SallerBin }
                    });
                }
            }

            if ((objectModel.Block == LandObjectBlocks.ActiveToBeTraded.ToString() || objectModel.Status == LandObjectStatuses.Saled.ToString()) && tradeModel != null)
            {
                actions.Add(new Link
                {
                    CssClass = ActionCssClass,
                    Text = env.T("Перейти к торгам"),
                    Controller = ModuleName,
                    Action = nameof(MnuLandObjectTradeView),
                    RouteValues = new MnuLandObjectTradeViewArgs { tradeId = tradeModel.Id }
                });

                var tbLandObjectTradeChanges = new TbLandObjectTradeChanges();
                tbLandObjectTradeChanges
                    .AddFilter(t => t.flTradeId, tradeModel.Id);
                var etpAuctionId = tbLandObjectTradeChanges.SelectScalar(t => t.flAuctionId, env.QueryExecuter);
                if (etpAuctionId.HasValue)
                {
                    if (isObjectSeller)
                    {
                        actions.Add(new LinkUrl(env.T("Перейти на ЭТП"), $"https://cabinet-auction.gosreestr.kz/p/ru/auctions/{etpAuctionId}/seller-view", ActionCssClass));
                    }
                    else
                    {
                        if (DateTime.Now <= tradeModel.DateTime.Value.AddDays(-1) && tradeModel.Status == RefLandObjectTradeStatuses.Wait)
                        {
                            if (env.User.IsGuest())
                            {
                                actions.Add(new LinkUrl(env.T("Подать заявление на участие в торгах"), $"https://e-auction.gosreestr.kz/p/ru/auctions/{etpAuctionId}/view", ActionCssClass));
                            }
                            else
                            {
                                actions.Add(new LinkUrl(env.T("Подать заявление на участие в торгах"), $"https://cabinet-auction.gosreestr.kz/p/ru/auctions/{etpAuctionId}/user-view", ActionCssClass));
                            }
                        }
                        else
                        {
                            if (env.User.IsGuest())
                            {
                                actions.Add(new LinkUrl(env.T("Перейти на ЭТП"), $"https://e-auction.gosreestr.kz/p/ru/auctions/{etpAuctionId}/view", ActionCssClass));
                            }
                            else
                            {
                                actions.Add(new LinkUrl(env.T("Перейти на ЭТП"), $"https://cabinet-auction.gosreestr.kz/p/ru/auctions/{etpAuctionId}/user-view", ActionCssClass));
                            }
                        }
                    }
                }
            }
            return actions;
        }

        public Card renderAgreementShortData(LandObjectModel objectModel, LandObjectTradeModel tradeModel, AgreementsModel agreementsModel, bool isObjectSeller, bool isAgreementSigner, ActionEnv<LandObjectViewActionQueryArgs> env)
        {
            var agreementShortData = new Card(env.T("Договор"));
            var agreementShortDataActions = agreementActions(objectModel, tradeModel, agreementsModel, isObjectSeller, isAgreementSigner, env);
            if (agreementShortDataActions.Count > 0)
            {
                new Panel("bg-white bordered border-top form-wizard-header mt-1 pt-2 pl-2").AppendRange(agreementShortDataActions).AppendTo(agreementShortData);
            }
            if (agreementsModel != null)
            {
                var tbAgreements = new TbAgreements();
                tbAgreements.flAgreementNumber.RenderCustom(agreementShortData, env, agreementsModel.flAgreementNumber, readOnly: true);
                tbAgreements.flAgreementCreateDate.RenderCustom(agreementShortData, env, agreementsModel.flAgreementCreateDate, readOnly: true);
                tbAgreements.flAgreementSignDate.RenderCustom(agreementShortData, env, agreementsModel.flAgreementSignDate, readOnly: true);
            }
            return agreementShortData;
        }

        private List<LinkBase> agreementActions(LandObjectModel objectModel, LandObjectTradeModel tradeModel, AgreementsModel agreementsModel, bool isObjectSeller, bool isAgreementSigner, ActionEnv<LandObjectViewActionQueryArgs> env)
        {
            var actions = new List<LinkBase>();
            if (objectModel.Status == LandObjectStatuses.Saled.ToString())
            {
                if (agreementsModel == null && objectModel.Block == LandObjectBlocks.SaledProt.ToString())
                {
                    if (isAgreementSigner)
                    {
                        Dictionary<string, string> AuctionTradeMethods = new Dictionary<string, string>() {
                            {RefLandObjectTradeForms.Values.LandTradesPriceUp, "PriceUpLand" },
                            {RefLandObjectTradeForms.Values.LandTradesPriceDown, "PriceDownLand" },
                            {RefLandObjectTradeForms.Values.ContestRent, "CompetitionAgricultureLand" },
                        };
                        Dictionary<string, string> AuctionObjectTypes = new Dictionary<string, string>() {
                            {nameof(LandObjectTypes.Agricultural), "LandAgricultural" },
                            {nameof(LandObjectTypes.Сommercial), "Land" },
                        };
                        if (tradeModel.Type == RefLandObjectTradeTypes.PrivateOwnership)
                        {
                            actions.Add(new Link
                            {
                                CssClass = ActionCssClass,
                                Text = env.T("Создать договор купли-продажи земельного участка"),
                                Controller = ModuleName,
                                Action = nameof(MnuDefaultAgrWizard),
                                RouteValues = new DefaultAgrTemplateArgs { MenuAction = MnuDefaultAgrWizard.Actions.Create, AuctionId = Convert.ToInt32(tradeModel.SaleProtocolNumber), TradeId = tradeModel.Id, TradeType = AuctionTradeMethods[tradeModel.Method], ObjectId = objectModel.Id, ObjectType = AuctionObjectTypes[objectModel.Type], AgreementType = nameof(ДоговорКпЗемельногоУчастка), WinnerXin = tradeModel.WinnerBin, SellerBin = objectModel.SallerBin }
                            });
                        } else
                        {
                            actions.Add(new Link
                            {
                                CssClass = ActionCssClass,
                                Text = env.T("Создать договор купли-продажи права аренды земельного участка"),
                                Controller = ModuleName,
                                Action = nameof(MnuDefaultAgrWizard),
                                RouteValues = new DefaultAgrTemplateArgs { MenuAction = MnuDefaultAgrWizard.Actions.Create, AuctionId = Convert.ToInt32(tradeModel.SaleProtocolNumber), TradeId = tradeModel.Id, TradeType = AuctionTradeMethods[tradeModel.Method], ObjectId = objectModel.Id, ObjectType = AuctionObjectTypes[objectModel.Type], AgreementType = nameof(ДоговорКпПраваАрендыЗемельногоУчастка), WinnerXin = tradeModel.WinnerBin, SellerBin = objectModel.SallerBin }
                            });
                        }
                    }
                }
                else if (agreementsModel != null)
                {
                    if (isAgreementSigner || isObjectSeller)
                    {
                        actions.Add(new Link
                        {
                            CssClass = ActionCssClass,
                            Text = env.T("Перейти к договору"),
                            Controller = ModuleName,
                            Action = nameof(MnuDefaultAgrWizard),
                            RouteValues = new DefaultAgrTemplateArgs { MenuAction = MnuDefaultAgrWizard.Actions.View, AgreementId = agreementsModel.flAgreementId }
                        });
                    }
                }
            }
            return actions;
        }

        private Card renderObjectTimeLine(LandObjectModel objectModel, LandObjectTradeModel tradeModel, AgreementsModel agreementsModel, PaymentModel paymentModel, ActionEnv<LandObjectViewActionQueryArgs> env)
        {
            var objectCreateDateTime = LandObjectModelHelper.GetObjectFisrstOrderDateTime(objectModel.Id, env.QueryExecuter);
            var tradeCreateDateTime = tradeModel == null ? new DateTime() : LandObjectTradeModelHelper.GetTradeFisrstOrderDateTime(tradeModel.Id, env.QueryExecuter);

            var timeLineCard = new Card(env.T("Шаги"));
            var timeLine = new TimeLine()
                .Append(new TimeLineItem(env.T("Объект создан"),
                        objectCreateDateTime.ToString("dd.MM.yyyy hh:mm"),
                        iconClass:
                            objectModel.Status != LandObjectStatuses.Deleted.ToString() ?
                                "bg-success"
                            :
                                "bg-warning"
                    )
                );
            if (objectModel.Status == LandObjectStatuses.Deleted.ToString())
            {
                var objectDeleteDateTime = LandObjectModelHelper.GetObjectLastOrderDateTime(objectModel.Id, env.QueryExecuter);
                timeLine
                    .Append(new TimeLineItem(env.T("Объект удалён"),
                            objectDeleteDateTime.ToString("dd.MM.yyyy hh:mm"),
                            iconClass: "bg-danger"
                        )
                    );
            } else {
                timeLine
                    .Append(new TimeLineItem(env.T("Объект выставлен на торги"),
                            tradeModel != null ?
                                tradeCreateDateTime.ToString("dd.MM.yyyy hh:mm")
                            :
                                "__.__.____ __:__",
                            iconClass:
                                tradeModel != null ?
                                    "bg-success"
                                :
                                    "bg-light"
                        )
                    )
                    .Append(new TimeLineItem(env.T("Торги состоялись"),
                            tradeModel != null ?
                                tradeModel.DateTime.Value.ToString("dd.MM.yyyy hh:mm")
                            :
                                "__.__.____ __:__",
                            iconClass:
                                tradeModel != null && tradeModel.Status == RefLandObjectTradeStatuses.Held ?
                                    "bg-success"
                                :
                                    "bg-light"
                        )
                    )
                    .Append(new TimeLineItem(env.T("Протокол подписан"),
                            tradeModel != null && tradeModel.SaleProtocolDate.HasValue ?
                                tradeModel.SaleProtocolDate.Value.ToString("dd.MM.yyyy hh:mm")
                            :
                                "__.__.____ __:__",
                            iconClass:
                                tradeModel != null && tradeModel.SaleProtocolDate.HasValue ?
                                    "bg-success"
                                :
                                    "bg-light"
                        )
                    )
                    .Append(new TimeLineItem(env.T("Договор подписан"),
                            agreementsModel != null && agreementsModel.flAgreementStatus == AgreementStatuses.Signed ?
                                agreementsModel.flAgreementSignDate.Value.ToString("dd.MM.yyyy hh:mm")
                            :
                                "__.__.____ __:__",
                            iconClass:
                                agreementsModel != null && agreementsModel.flAgreementStatus == AgreementStatuses.Signed ?
                                    "bg-success"
                                :
                                    "bg-light"
                        )
                    )
                    .Append(new TimeLineItem(env.T("Оплата произведена"),
                            iconClass:
                                paymentModel != null && paymentModel.flPaymentStatus == PaymentStatus.Paid ?
                                    "bg-success"
                                :
                                    "bg-light"
                        )
                    );
            }
            timeLine
                .AppendTo(timeLineCard);

            return timeLineCard;
        }

        public Card renderObjectGeometry(LandObjectModel objectModel, ActionEnv<LandObjectViewActionQueryArgs> env)
        {
            var card = new Card(env.T("Геометрия"))
                //.Append(new GeomRenderer(objectModel.WKT, GeomType.Geometry, new GeomRenderer.GeometryStyle(4326, background: GeomRenderer.MapBackgrounds.google, height: 400, fillColor: "#0acf9710", strokeColor: "#0acf97", strokeWidth: 4, scale: 3, canMove: true, rounded: true), readOnly: true));
                .Append(new ObjectGeometryViewerComponent(objectModel.WKT));
            return card;
        }

    }

    public class LandObjectViewActionQueryArgs: ActionQueryArgsBase {
        public int Id { get; set; }
    }

}
