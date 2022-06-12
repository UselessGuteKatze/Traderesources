using FishingSource.Helpers.Object;
using FishingSource.Helpers.Trade;
using FishingSource.Models;
using FishingSource.QueryTables.Common;
using FishingSource.QueryTables.Object;
using FishingSource.QueryTables.Trade;
using FishingSource.References.Object;
using FishingSource.References.Trade;
using FishingSource.SearchCollections.Object;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TradeResourcesPlugin.Helpers;
using TradeResourcesPlugin.Helpers.Agreements;
using TradeResourcesPlugin.Modules.Components;
using TradeResourcesPlugin.Modules.FishingMenus.Agreements;
using TradeResourcesPlugin.Modules.FishingMenus.Trades;
using UsersResources;
using Yoda.Interfaces;
using Yoda.Interfaces.Forms;
using Yoda.Interfaces.Forms.Components;
using YodaApp.YodaHelpers.Components;
using YodaHelpers.ActionMenus;
using YodaHelpers.Fields;
using YodaHelpers.SearchCollections;
using YodaQuery;

namespace TradeResourcesPlugin.Modules.FishingMenus.Objects {
    public class MnuFishingObjectView: MnuActionsExt<FishingObjectViewArgs> {
        public const string MnuName = nameof(MnuFishingObjectView);

        public MnuFishingObjectView(string moduleName) : base(moduleName, MnuName, "Объекты") {
            AsCallback();
            //Enabled(rc => rc.User.IsAuthentificated);
        }

        public class Actions {
            public const string View = "view";
        }
        public const string ActionCssClass = "btn btn-sm btn-dark font-weight-normal text-wrap mr-2 mb-2";

        private void renderTradeCommitErrors(RenderActionEnv<FishingObjectViewArgs> re)
        {
            var obj = ObjectHelper.GetObjectModel(re.Args.Id, re.QueryExecuter);

            var objectSeller = obj.flSallerBin;
            var currentUser = re.User.GetUserXin(re.QueryExecuter);
            var isObjectSeller = currentUser == objectSeller;
            var isInternal = !re.User.IsExternalUser() && !re.User.IsGuest();

            if (!isObjectSeller && !isInternal)
            {
                return;
            }

            var hasDataEdit = re.User.HasRole("TRADERESOURCES-Рыбохозяйственные водоёмы-Создание приказов", re.QueryExecuter)/*re.User.HasCustomRole("fishingobjects", "dataEdit", re.QueryExecuter)*/;

            if (!hasDataEdit && !isInternal)
            {
                re.Form.AddComponent(new Panel("alert alert-danger mt-2") { Elements = new YodaFormElementCollection() { new HtmlText("У вас нет прав на создание или изменение данных или период их активности подощёл к концу. Обратитесь к главе организации или коллеге с доступом к распределению прав.") } });
            }

            if (obj.flBlock == FishingObjectBlocks.ActiveToBeTraded.ToString())
            {
                var tbTrades = new TbTrades().AddFilter(t => t.flObjectId, obj.flId);
                var tradeRevisionId = tbTrades.Select(new FieldAlias[] { tbTrades.flRevisionId }, re.QueryExecuter).AsEnumerable().OrderByDescending(r => tbTrades.flRevisionId.GetRowVal(r)).Select(r => tbTrades.flRevisionId.GetRowVal(r)).ToArray()[0];

                var tbTradesChanges = new TbTradeChanges().AddFilter(t => t.flRevisionId, tradeRevisionId);
                var message = tbTradesChanges.SelectScalar(t => t.flMessage, re.QueryExecuter);
                if (!string.IsNullOrEmpty(message))
                {
                    re.Form.AddComponent(new Panel("alert alert-danger mt-2") { Elements = new YodaFormElementCollection() { new HtmlText(message) } });
                }
            }

        }

        public override void Configure(ActionConfig<FishingObjectViewArgs> config) {
            config
                .OnAction(Actions.View, action => action
                    .IsValid(env => new OkResult())
                    .OnRendering(re => {
                        renderTradeCommitErrors(re);
                        renderObjectView(re.Form, re);
                    }));
        }

        private void renderObjectView(WidgetBase widget, ActionEnv<FishingObjectViewArgs> env)
        {
            var objectModel = ObjectHelper.GetObjectModel(env.Args.Id, env.QueryExecuter);
            FishingTradeModel tradeModel = null;
            AgreementsModel agreementsModel = null;
            PaymentModel paymentModel = null;

            var tbTrades = new TbTrades();
            tbTrades.AddFilter(tbTrades.flObjectId, objectModel.flId);
            tbTrades.AddFilterIn(tbTrades.flStatus, new[] { RefTradesStatuses.Wait, RefTradesStatuses.Held });
            tbTrades.OrderBy = new[] { new OrderField(tbTrades.flId, OrderType.Desc) };
            if (tbTrades.Count(env.QueryExecuter) > 0)
            {
                tradeModel = tbTrades.GetTradeModelFirst(env.QueryExecuter, null);

                agreementsModel = new TbAgreements()
                    .AddFilter(t => t.flObjectId, objectModel.flId)
                    .AddFilter(t => t.flTradeId, tradeModel.flId)
                    .AddFilter(t => t.flTradeType, "CompetitionHunt")
                    .AddFilter(t => t.flObjectType, "FishingReservoir")
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

            var objectSeller = objectModel.flSallerBin;
            var hasPair = new TbSellerSigners().GetPair(objectSeller, env.QueryExecuter, out var data);
            var currentUser = env.User.GetUserXin(env.QueryExecuter);
            var isAgreementSigner = data.flSignerBins.Contains(currentUser);
            var isObjectSeller = currentUser == objectSeller && env.User.HasRole("TRADERESOURCES-Рыбохозяйственные водоёмы-Создание приказов", env.QueryExecuter)/*env.User.HasCustomRole("fishingobjects", "dataEdit", env.QueryExecuter)*/;

            var mainRow = new GridRow().AppendTo(widget);
            new GridCol("col-md-9")
                .Append(renderObjectData(objectModel, isObjectSeller, env))
                .Append(renderTradeData(objectModel, tradeModel, isObjectSeller, isAgreementSigner, env))
                .Append(renderAgreementShortData(objectModel, tradeModel, agreementsModel, isObjectSeller, isAgreementSigner, env))
                .Append(PaymentHelper.RenderPaymentData(paymentModel, agreementsModel, isObjectSeller, isAgreementSigner, env))
                .AppendTo(mainRow);
            new GridCol("col-md-3")
                .Append(renderObjectTimeLine(objectModel, tradeModel, agreementsModel, paymentModel, env))
                .Append(renderObjectGeometry(objectModel, env))
                .AppendTo(mainRow);
        }

        private Card renderObjectData(FishingObjectModel objectModel, bool isObjectSeller, ActionEnv<FishingObjectViewArgs> env)
        {
            var objectData = new Card(objectModel.flName, objectModel.flStatus == FishingObjectStatuses.Deleted.ToString() ? "text-decoration-line-through" : "");
            var objectDataActions = objectActions(objectModel, isObjectSeller, env);
            if (objectDataActions.Count > 0)
            {
                new Panel("bg-white bordered border-top form-wizard-header mt-1 pt-2 pl-2").AppendRange(objectDataActions).AppendTo(objectData);
            }
            var tabPanel = new HyperTabs().AppendTo(objectData);

            var tbObject = new TbObjects();

            var objectShortData = new Panel();
            tbObject.flId.RenderCustom(objectShortData, env, objectModel.flId.ToString(), readOnly: true);
            tbObject.flStatus.RenderCustom(objectShortData, env, objectModel.flStatus, readOnly: true);
            tbObject.flBlock.RenderCustom(objectShortData, env, objectModel.flBlock, readOnly: true);
            tbObject.flArea.RenderCustom(objectShortData, env, objectModel.flArea, readOnly: true);
            tbObject.flRegion.RenderCustom(objectShortData, env, objectModel.flRegion, readOnly: true);
            tbObject.flDistrict.RenderCustom(objectShortData, env, objectModel.flDistrict, readOnly: true);
            tbObject.flLocation.RenderCustom(objectShortData, env, objectModel.flLocation, readOnly: true);
            tabPanel.AddNewTab("Краткая информация", objectShortData);

            var objectFullData = new Panel();
            MnuFishingObjectOrderBase.viewModelMainData(objectFullData, env, objectModel, true);
            MnuFishingObjectOrderBase.viewModelGeoPositionData(objectFullData, env, objectModel, true);
            MnuFishingObjectOrderBase.viewModelPhysicalCharacteristicData(objectFullData, env, objectModel, true);
            MnuFishingObjectOrderBase.viewModelBiologicalCharacteristicData(objectFullData, env, objectModel, true);
            MnuFishingObjectOrderBase.viewModelFarmicCharacteristicData(objectFullData, env, objectModel, true);
            tabPanel.AddNewTab("Подробная информация", objectFullData);

            return objectData;
        }

        private List<LinkBase> objectActions(FishingObjectModel objectModel, bool isObjectSeller, ActionEnv<FishingObjectViewArgs> env)
        {
            var actions = new List<LinkBase>();

            if (!isObjectSeller)
            {
                return actions;
            }

            if (objectModel.flBlock == FishingObjectBlocks.ActiveFree.ToString())
            {
                actions.Add(new Link
                {
                    CssClass = ActionCssClass,
                    Text = env.T("Редактировать объект"),
                    Controller = ModuleName,
                    Action = nameof(MnuFishingObjectOrderBase),
                    RouteValues = new ObjectOrderQueryArgs { Id = objectModel.flId, RevisionId = objectModel.flRevisionId, OrderType = ObjectsOrderTypeActions.Edit, MenuAction = "create-from" }
                });
                actions.Add(new Link
                {
                    CssClass = ActionCssClass,
                    Text = env.T("Удалить объект"),
                    Controller = ModuleName,
                    Action = nameof(MnuFishingObjectOrderBase),
                    RouteValues = new ObjectOrderQueryArgs { Id = objectModel.flId, RevisionId = objectModel.flRevisionId, OrderType = ObjectsOrderTypeActions.Remove, MenuAction = "create-from" }
                });
            }
            return actions;
        }

        private Card renderTradeData(FishingObjectModel objectModel, FishingTradeModel tradeModel, bool isObjectSeller, bool isAgreementSigner, ActionEnv<FishingObjectViewArgs> env)
        {
            var tradeData = new Card(env.T("Торги"));
            var tradeDataActions = tradeActions(objectModel, tradeModel, isObjectSeller, isAgreementSigner, env);
            if (tradeDataActions.Count > 0)
            {
                new Panel("bg-white bordered border-top form-wizard-header mt-1 pt-2 pl-2").AppendRange(tradeDataActions).AppendTo(tradeData);
            }
            var tabPanel = new HyperTabs().AppendTo(tradeData);

            var tbTrade = new TbTrades();

            if (tradeModel != null)
            {
                var tradeShortData = new Panel();
                tbTrade.flId.RenderCustom(tradeShortData, env, tradeModel.flId > 0 ? tradeModel.flId.ToString() : "", readOnly: true);
                tbTrade.flStatus.RenderCustom(tradeShortData, env, tradeModel.flStatus, readOnly: true);
                tbTrade.flDateTime.RenderCustom(tradeShortData, env, tradeModel.flDateTime, readOnly: true);
                tbTrade.flDeposit.RenderCustom(tradeShortData, env, tradeModel.flDeposit, readOnly: true);
                tbTrade.flCost.RenderCustom(tradeShortData, env, tradeModel.flCost, readOnly: true);
                tabPanel.AddNewTab("Краткая информация", tradeShortData);

                var tradeFullData = new Panel();
                MnuFishingTradeOrderBase.viewModelMainData(tradeFullData, env, tradeModel, true);

                MnuFishingTradeOrderBase.viewModelRequisitesData(tradeFullData, env, tradeModel, true);

                MnuFishingTradeOrderBase.viewModelContestData(tradeFullData, env, tradeModel, true);

                if (!string.IsNullOrEmpty(tradeModel.flWinnerBin))
                {
                    MnuFishingTradeOrderBase.viewModelSaleData(tradeFullData, env, tradeModel, true);
                }
                if (tradeModel.flIsSaleProtocolExists == true)
                {
                    MnuFishingTradeOrderBase.viewModelProtocolData(tradeFullData, env, tradeModel, true);
                }
                if (tradeModel.flIsSaleAgreementExists == true)
                {
                    MnuFishingTradeOrderBase.viewModelAgreementData(tradeFullData, env, tradeModel, true);
                }

                MnuFishingTradeOrderBase.viewModelPublicationsData(tradeFullData, env, tradeModel, true);
                tabPanel.AddNewTab("Подробная информация", tradeFullData);
            }

            var tbTrades = new TbTrades();
            tbTrades.AddFilter(tbTrades.flObjectId, objectModel.flId);
            var tradesCount = tbTrades.Count(env.QueryExecuter);
            if ((tradeModel != null && tradesCount > 1) || (tradeModel == null && tradesCount > 0))
            {
                var objectTradesData = new TimeLine();
                tbTrades.Select(t => new FieldAlias[] { t.flId, t.flDateTime }, env.QueryExecuter).AsEnumerable().OrderBy(r => r.GetVal(t => t.flDateTime)).Each(r =>
                {
                    var tradeId = r.GetVal(t => t.flId);
                    var tradeDateTime = r.GetVal(t => t.flDateTime);
                    var isCurrentTrade = tradeModel != null && tradeModel.flId == tradeId;

                    objectTradesData
                        .Append(new TimeLineItem(env.T("Торги" + $" №{tradeId}"),
                            tradeDateTime.ToString("dd.MM.yyyy hh:mm"),
                            iconClass: isCurrentTrade ? "bg-success" : "bg-light",
                            messageElement: new Link
                            {
                                CssClass = "badge badge-dark font-weight-normal p-1 ml-2",
                                Text = env.T("Перейти"),
                                Controller = ModuleName,
                                Action = nameof(MnuFishingTradeView),
                                RouteValues = new MnuFishingTradeViewArgs { tradeId = tradeId }
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

        private List<LinkBase> tradeActions(FishingObjectModel objectModel, FishingTradeModel tradeModel, bool isObjectSeller, bool isAgreementSigner, ActionEnv<FishingObjectViewArgs> env)
        {
            var actions = new List<LinkBase>();

            if (objectModel.flBlock == FishingObjectBlocks.ActiveFree.ToString() && tradeModel == null)
            {
                if (isObjectSeller)
                {
                    actions.Add(new Link
                    {
                        CssClass = ActionCssClass,
                        Text = env.T("Создать торги"),
                        Controller = ModuleName,
                        Action = nameof(MnuFishingTradeOrderBase),
                        RouteValues = new FishingTradeOrderQueryArgs { OrderType = TradesOrderTypeActions.Create, MenuAction = "create-new", ObjectId = objectModel.flId, RevisionId = -1, OwnerXin = objectModel.flSallerBin }
                    });
                }
            }

            if ((objectModel.flBlock == FishingObjectBlocks.ActiveToBeTraded.ToString() || objectModel.flStatus == FishingObjectStatuses.Saled.ToString()) && tradeModel != null)
            {
                actions.Add(new Link
                {
                    CssClass = ActionCssClass,
                    Text = env.T("Перейти к торгам"),
                    Controller = ModuleName,
                    Action = nameof(MnuFishingTradeView),
                    RouteValues = new MnuFishingTradeViewArgs { tradeId = tradeModel.flId }
                });

                var TbTradeChanges = new TbTradeChanges();
                TbTradeChanges
                    .AddFilter(t => t.flTradeId, tradeModel.flId);
                var etpAuctionId = TbTradeChanges.SelectScalar(t => t.flAuctionId, env.QueryExecuter);
                if (etpAuctionId.HasValue)
                {
                    if (isObjectSeller || isAgreementSigner)
                    {
                        actions.Add(new LinkUrl(env.T("Перейти на ЭТП"), $"https://cabinet-auction.gosreestr.kz/p/ru/auctions/{etpAuctionId}/seller-view", ActionCssClass));
                    } else
                    {
                        if (DateTime.Now <= tradeModel.flDateTime.AddDays(-1) && tradeModel.flStatus == RefTradesStatuses.Wait)
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

        public Card renderAgreementShortData(FishingObjectModel objectModel, FishingTradeModel tradeModel, AgreementsModel agreementsModel, bool isObjectSeller, bool isAgreementSigner, ActionEnv<FishingObjectViewArgs> env)
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

        private List<LinkBase> agreementActions(FishingObjectModel objectModel, FishingTradeModel tradeModel, AgreementsModel agreementsModel, bool isObjectSeller, bool isAgreementSigner, ActionEnv<FishingObjectViewArgs> env)
        {
            var actions = new List<LinkBase>();
            if (objectModel.flStatus == FishingObjectStatuses.Saled.ToString())
            {
                if (agreementsModel == null && objectModel.flBlock == FishingObjectBlocks.SaledProt.ToString())
                {
                    if (isAgreementSigner)
                    {
                        actions.Add(new Link
                        {
                            CssClass = ActionCssClass,
                            Text = env.T("Создать договор на ведение рыбного хозяйства (отрх и срх)"),
                            Controller = ModuleName,
                            Action = nameof(MnuDefaultAgrWizard),
                            RouteValues = new DefaultAgrTemplateArgs { MenuAction = MnuDefaultAgrWizard.Actions.Create, AuctionId = Convert.ToInt32(tradeModel.flSaleProtocolNumber), TradeId = tradeModel.flId, TradeType = "CompetitionHunt", ObjectId = objectModel.flId, ObjectType = "FishingReservoir", AgreementType = nameof(ДоговорНаВедениеРыбногоХозяйстваОтрхИСрх), WinnerXin = tradeModel.flWinnerBin, SellerBin = objectModel.flSallerBin }
                        });
                        actions.Add(new Link
                        {
                            CssClass = ActionCssClass,
                            Text = env.T("Создать договор на ведение рыбного хозяйства (прх и лрх)"),
                            Controller = ModuleName,
                            Action = nameof(MnuDefaultAgrWizard),
                            RouteValues = new DefaultAgrTemplateArgs { MenuAction = MnuDefaultAgrWizard.Actions.Create, AuctionId = Convert.ToInt32(tradeModel.flSaleProtocolNumber), TradeId = tradeModel.flId, TradeType = "CompetitionHunt", ObjectId = objectModel.flId, ObjectType = "FishingReservoir", AgreementType = nameof(ДоговорНаВедениеРыбногоХозяйстваПрхИЛрх), WinnerXin = tradeModel.flWinnerBin, SellerBin = objectModel.flSallerBin }
                        });
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

        private Card renderObjectTimeLine(FishingObjectModel objectModel, FishingTradeModel tradeModel, AgreementsModel agreementsModel, PaymentModel paymentModel, ActionEnv<FishingObjectViewArgs> env)
        {
            var objectCreateDateTime = ObjectHelper.GetObjectFisrstOrderDateTime(objectModel.flId, env.QueryExecuter);
            var tradeCreateDateTime = tradeModel == null ? new DateTime() : TradeHelper.GetTradeFisrstOrderDateTime(tradeModel.flId, env.QueryExecuter);

            var timeLineCard = new Card(env.T("Шаги"));
            var timeLine = new TimeLine()
                .Append(new TimeLineItem(env.T("Объект создан"),
                        objectCreateDateTime.ToString("dd.MM.yyyy hh:mm"),
                        iconClass:
                            objectModel.flStatus != FishingObjectStatuses.Deleted.ToString() ?
                                "bg-success"
                            :
                                "bg-warning"
                    )
                );
            if (objectModel.flStatus == FishingObjectStatuses.Deleted.ToString())
            {
                var objectDeleteDateTime = ObjectHelper.GetObjectLastOrderDateTime(objectModel.flId, env.QueryExecuter);
                timeLine
                    .Append(new TimeLineItem(env.T("Объект удалён"),
                            objectDeleteDateTime.ToString("dd.MM.yyyy hh:mm"),
                            iconClass: "bg-danger"
                        )
                    );
            }
            else
            {
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
                                tradeModel.flDateTime.ToString("dd.MM.yyyy hh:mm")
                            :
                                "__.__.____ __:__",
                            iconClass:
                                tradeModel != null && tradeModel.flStatus == RefTradesStatuses.Held ?
                                    "bg-success"
                                :
                                    "bg-light"
                        )
                    )
                    .Append(new TimeLineItem(env.T("Протокол подписан"),
                            tradeModel != null && tradeModel.flSaleProtocolDate.HasValue ?
                                tradeModel.flSaleProtocolDate.Value.ToString("dd.MM.yyyy hh:mm")
                            :
                                "__.__.____ __:__",
                            iconClass:
                                tradeModel != null && tradeModel.flSaleProtocolDate.HasValue ?
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

        public Card renderObjectGeometry(FishingObjectModel objectModel, ActionEnv<FishingObjectViewArgs> env)
        {
            var card = new Card(env.T("Геометрия"))
                //.Append(new GeomRenderer(objectModel.flWKT, GeomType.Geometry, new GeomRenderer.GeometryStyle(4326, background: GeomRenderer.MapBackgrounds.google, height: 400, fillColor: "#0acf9710", strokeColor: "#0acf97", strokeWidth: 4, scale: 3, canMove: true, rounded: true), readOnly: true));
                .Append(new ObjectGeometryViewerComponent(objectModel.flWKT));
            return card;
        }
    }

    public class FishingObjectViewArgs: ActionQueryArgsBase {
        public int Id { get; set; }
    }

}
