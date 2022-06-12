using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeResourcesPlugin.Modules.Administration;
using UsersResources;
using Yoda.Interfaces;
using Yoda.Interfaces.Forms;
using Yoda.Interfaces.Forms.Components;
using YodaApp.YodaHelpers.OrderHelpers;
using YodaHelpers.ActionMenus;
using YodaHelpers.Components;
using YodaHelpers.Fields;
using YodaHelpers.OrderHelpers;
using YodaQuery;
using static TradeResourcesPlugin.Helpers.Agreements.PaymentHelper;

namespace TradeResourcesPlugin.Helpers.Agreements {
    public class MnuPaymentMatchOrder : MnuOrderBaseV2<PaymentMatchModel, PaymentMatchesOrderTypes, PaymentMatchesOrderQueryArgs> {

        public MnuPaymentMatchOrder(string moduleName, OrderStandartPermissions perms)
            : base(moduleName, nameof(MnuPaymentMatchOrder), "Приказы по привязкам платежей", perms, () => new TbPaymentMatchesOrderResult(), () => new TbPaymentMatchesOrderNegotiations()) {
            AsCallback();
            Enabled(rc => {
                return true;
            });
        }

        public override Task<ActionItem[]> GetActions(ActionEnv<PaymentMatchesOrderQueryArgs> env) {
            var ret = base.GetActions(env).Result.ToList();

            switch (env.Args.MenuAction) {
                case Actions.ViewOrder:
                var rev = new TbPaymentMatchesRevisions().AddFilter(t => t.flId, env.Args.RevisionId)
                    .GetPaymentMatchModelFirstOrDefault(env.QueryExecuter);
                if (rev != null) {
                    ret.Add(new ActionItem(env.T("Перейти к договору"), new ActionRedirectData(ModuleName, nameof(MnuDefaultAgrWizard), new DefaultAgrTemplateArgs { MenuAction = MnuDefaultAgrWizard.Actions.View, AgreementId = rev.flAgreementId })));
                }
                break;
                case Actions.CreateFrom:
                break;
            }

            return Task.FromResult(ret.ToArray());
        }

        public override IWizardFormBuilderWithStep<PaymentMatchesOrderQueryArgs, ModelOrderArgs<PaymentMatchesOrderTypes, PaymentMatchesOrderQueryArgs>, PaymentMatchModel>
            EditModel(IWizardFormBuilderWithFinishButton<PaymentMatchesOrderQueryArgs, ModelOrderArgs<PaymentMatchesOrderTypes, PaymentMatchesOrderQueryArgs>, PaymentMatchModel> wizard) {
            return wizard
                .Step("Привязка платежей", step => step
                    .OnRendering(re => {
                        re.Model.flDateTime = DateTime.Now;
                        var paymentsProvider = new TraderesourcesPaymentsProvider(re.Model.flOverpaymentRequisites.flXin, re.Model.flOverpaymentRequisites.flName, re.Env.RequestContext);
                        var guaranteePaymentsList = paymentsProvider.GuaranteePayments.GetAllPayments().Where(x => re.Model.flPaymentItems.Any(y => y.flId.Id == x.Id.Id));
                        var freePaymentsList = paymentsProvider.SellPayments.GetFreePayments();
                        var freePayments = freePaymentsList.OrderByDescending(item => item.Date).ToArray();

                        var agreementType = AgreementHelper.GetAgreementType(re.Model.flAgreementId, re.Env.QueryExecuter);
                        var agrTempl = AgreementHelper.GetAgreementTypeModel(agreementType);

                        PaymentModel paymentModel = null;
                        var tbPayments = new TbPayments()
                                .AddFilter(t => t.flAgreementId, re.Model.flAgreementId);
                        if (tbPayments.Count(re.Env.QueryExecuter) > 0) {
                            paymentModel = tbPayments.GetPaymentModelFirstOrDefault(re.Env.QueryExecuter, null);
                        }

                        var paidAmount = paymentModel == null ? 0 : paymentModel.flPaidAmount;

                        var flPayAmount = (paymentModel == null ? Math.Ceiling(agrTempl.GetSellPrice(re.Env, re.Model.flAgreementId).Value) : paymentModel.flPayAmount);
                        var needToPay = flPayAmount - paidAmount;

                        var checkBoxesBox = new CheckBoxesBox("payments", CheckBoxesBox.ColumnsCount.col1, $"Цена продажи / Требуемая / Выбранная / Оставшаяся сумма - {flPayAmount:#,##0.00} / {needToPay:#,##0.00} / {0:#,##0.00} / {needToPay:#,##0.00} тг.").AppendTo(re.Panel);

                        var tbPaymentItems = new TbRenderPaymentItems();

                        guaranteePaymentsList.Each(guaranteePaymentItem => {
                            var card = new Card($"Платеж №{guaranteePaymentItem.Id.Id} - {guaranteePaymentItem.Amount:#,##0.00} тг. от {guaranteePaymentItem.Date:dd.MM.yyyy hh:mm}") {
                                CssClass = "payment-item",
                                Attributes = new Dictionary<string, object>() {
                                    { "amount", guaranteePaymentItem.Amount }
                                }
                            };
                            //var cardRow = new GridRow().AppendTo(card);
                            //tbPaymentItems.flPaymentItemId.RenderCustom(cardRow, re.Env, freePayment.Id.Id, readOnly: true, /*orientation: FormOrientation.Basic,*/ readOnlyCssClass: "d-block h5", cssClass: "col-sm-4");
                            //tbPaymentItems.flAmount.RenderCustom(cardRow, re.Env, freePayment.Amount, readOnly: true, /*orientation: FormOrientation.Basic,*/ readOnlyCssClass: "d-block h5", cssClass: "col-sm");
                            //tbPaymentItems.flDateTime.RenderCustom(cardRow, re.Env, freePayment.Date, readOnly: true, /*orientation: FormOrientation.Basic,*/ readOnlyCssClass: "d-block h5", cssClass: "col-sm");
                            tbPaymentItems.flPurpose.RenderCustom(card, re.Env, guaranteePaymentItem.Purpose, readOnly: true, hideLabel: true, /*orientation: FormOrientation.Basic,*/ readOnlyCssClass: "d-block h5 my-0");
                            checkBoxesBox.AppendCheckbox($"{guaranteePaymentItem.Id.Id}", card, true, "w-100", false);
                        });

                        re.Panel.AddComponent(new UiPackages("payments-choose-counter"));

                        foreach (var freePayment in freePayments.OrderBy(freePayment => !re.Model.flPaymentItems.Any(y => y.flId.Id == freePayment.Id.Id)).ThenBy(freePayment => freePayment.Date)) {
                            var card = new Card($"Платеж №{freePayment.Id.Id} - {freePayment.Amount:#,##0.00} тг. от {freePayment.Date:dd.MM.yyyy hh:mm}") {
                                CssClass = "payment-item",
                                Attributes = new Dictionary<string, object>() {
                                    { "amount", freePayment.Amount }
                                }
                            };
                            //var cardRow = new GridRow().AppendTo(card);
                            //tbPaymentItems.flPaymentItemId.RenderCustom(cardRow, re.Env, freePayment.Id.Id, readOnly: true, /*orientation: FormOrientation.Basic,*/ readOnlyCssClass: "d-block h5", cssClass: "col-sm-4");
                            //tbPaymentItems.flAmount.RenderCustom(cardRow, re.Env, freePayment.Amount, readOnly: true, /*orientation: FormOrientation.Basic,*/ readOnlyCssClass: "d-block h5", cssClass: "col-sm");
                            //tbPaymentItems.flDateTime.RenderCustom(cardRow, re.Env, freePayment.Date, readOnly: true, /*orientation: FormOrientation.Basic,*/ readOnlyCssClass: "d-block h5", cssClass: "col-sm");
                            tbPaymentItems.flPurpose.RenderCustom(card, re.Env, freePayment.Purpose, readOnly: true, hideLabel: true, /*orientation: FormOrientation.Basic,*/ readOnlyCssClass: "d-block h5 my-0");
                            checkBoxesBox.AppendCheckbox($"{freePayment.Id.Id}", card, re.Model.flPaymentItems.Any(y => y.flId.Id == freePayment.Id.Id), "w-100");
                        }

                    })
                    .OnValidating(ve => {
                        var paymentsProvider = new TraderesourcesPaymentsProvider(ve.Model.flOverpaymentRequisites.flXin, ve.Model.flOverpaymentRequisites.flName, ve.Env.RequestContext);
                        var guaranteePaymentsList = paymentsProvider.GuaranteePayments.GetAllPayments().Where(x => ve.Model.flPaymentItems.Any(y => y.flId.Id == x.Id.Id));
                        var freePaymentsList = paymentsProvider.SellPayments.GetAllPayments().ToList();
                        freePaymentsList.AddRange(guaranteePaymentsList);
                        var freePayments = freePaymentsList.ToArray();

                        var choosenPaymentIds = new CheckBoxesBox("payments", CheckBoxesBox.ColumnsCount.col4).GetPostedValue(ve.Env).ToList();
                        choosenPaymentIds.AddRange(ve.Model.flPaymentItems.Select(y => y.flId.Id.ToString()));
                        choosenPaymentIds = choosenPaymentIds.Distinct().ToList();
                        //if (choosenPaymentIds.Count == 0)
                        //{
                        //    ve.Env.AddError("payments", ve.Env.T("Нужно выбрать хотя-бы один платеж!"));
                        //}
                        if (ve.Env.IsValid) {
                            var choosenPayments = freePayments.Where(freePayment => choosenPaymentIds.Contains(freePayment.Id.Id.ToString())).ToList();

                            var agreementType = AgreementHelper.GetAgreementType(ve.Model.flAgreementId, ve.Env.QueryExecuter);
                            var agrTempl = AgreementHelper.GetAgreementTypeModel(agreementType);

                            if (!agrTempl.HasPayment()) {
                                ve.Env.AddError("payments", ve.Env.T("Для данного типа договора не предусматривается оплата"));
                            }
                            if (ve.Env.IsValid) {
                                var commitPayments = choosenPayments.Select(x => new PaymentItemModel() {
                                    flId = x.Id,
                                    flDateTime = x.Date,
                                    flAmount = x.Amount,
                                    flPurpose = x.Purpose,
                                    flIsGuarantee = guaranteePaymentsList.Any(gp => gp.Id.Id == x.Id.Id)
                                }).ToArray();

                                PaymentModel paymentModel = null;
                                var tbPayments = new TbPayments()
                                        .AddFilter(t => t.flAgreementId, ve.Model.flAgreementId);
                                if (tbPayments.Count(ve.Env.QueryExecuter) > 0) {
                                    paymentModel = tbPayments.GetPaymentModelFirstOrDefault(ve.Env.QueryExecuter, null);
                                }

                                if ((!commitPayments.Any(x => x.flIsGuarantee)) && paymentModel == null) {
                                    ve.Env.AddError("payments", ve.Env.T("В первой привязке обязательно нужно выбрать гарантийный взнос!"));
                                }

                                if (ve.Env.IsValid) {
                                    var choosenAmount = choosenPayments.Sum(x => x.Amount);

                                    var flPayAmount = (paymentModel == null ? Math.Ceiling(agrTempl.GetSellPrice(ve.Env, ve.Model.flAgreementId).Value) : paymentModel.flPayAmount);

                                    var sellPrice = flPayAmount;

                                    if (choosenAmount < sellPrice && !agrTempl.IsInstallment()) {
                                        ve.Env.AddError("payments", ve.Env.T("Если догвор без рассрочки, нужно выбрать платежи, полностью покрывающие требуемую сумму") + $" ({choosenAmount:#,##0.00} тг. < {sellPrice:#,##0.00} тг.)");
                                    }
                                    if (paymentModel != null && !agrTempl.IsInstallment()) {
                                        ve.Env.AddError("payments", ve.Env.T("К договору уже приаязана оплата!") + $" ({choosenAmount:#,##0.00} тг. < {sellPrice:#,##0.00} тг.)");
                                    }
                                }
                            }
                        }
                    })
                    .OnProcessing(pe => {
                        var paymentsProvider = new TraderesourcesPaymentsProvider(pe.Model.flOverpaymentRequisites.flXin, pe.Model.flOverpaymentRequisites.flName, pe.Env.RequestContext);
                        var guaranteePaymentsList = paymentsProvider.GuaranteePayments.GetAllPayments().Where(x => pe.Model.flPaymentItems.Any(y => y.flId.Id == x.Id.Id));
                        var freePaymentsList = paymentsProvider.SellPayments.GetAllPayments().ToList();
                        freePaymentsList.AddRange(guaranteePaymentsList);
                        var freePayments = freePaymentsList.ToArray();
                        var choosenPaymentIds = new CheckBoxesBox("payments", CheckBoxesBox.ColumnsCount.col4).GetPostedValue(pe.Env).ToList();
                        //choosenPaymentIds.AddRange(pe.Model.flPaymentItems.Select(y => y.flId.Id.ToString()));
                        var choosenPayments = freePayments.Where(freePayment => choosenPaymentIds.Contains(freePayment.Id.Id.ToString())).ToArray();

                        var flPaymentItemIds = choosenPayments.Select(x => x.Id.Id.ToString()).ToArray();


                        var agreementType = AgreementHelper.GetAgreementType(pe.Model.flAgreementId, pe.Env.QueryExecuter);
                        var agrTempl = AgreementHelper.GetAgreementTypeModel(agreementType);

                        PaymentModel paymentModel = null;
                        var tbPayments = new TbPayments()
                                .AddFilter(t => t.flAgreementId, pe.Model.flAgreementId);
                        if (tbPayments.Count(pe.Env.QueryExecuter) > 0) {
                            paymentModel = tbPayments.GetPaymentModelFirstOrDefault(pe.Env.QueryExecuter, null);
                        }

                        var flIsUpdate = paymentModel != null;
                        pe.Model.flAmount = choosenPayments.Sum(x => x.Amount);
                        var flPaidAmount = paymentModel == null ? 0 : paymentModel.flPaidAmount;
                        var flPayAmount = paymentModel == null ? Math.Ceiling(agrTempl.GetSellPrice(pe.Env, pe.Model.flAgreementId).Value) : paymentModel.flPayAmount;

                        var flNeedForFullPayAmount = flPayAmount - flPaidAmount;
                        flPaidAmount += pe.Model.flAmount;

                        pe.Model.flOverpayment = flPaidAmount > flPayAmount;
                        pe.Model.flSendOverpayment = pe.Model.flOverpayment;
                        pe.Model.flOverpaymentAmount = 0;
                        pe.Model.flOverpaymentSendAmount = 0;
                        if (pe.Model.flOverpayment) {
                            pe.Model.flOverpaymentAmount = flPaidAmount - flPayAmount;
                            pe.Model.flOverpaymentSendAmount = pe.Model.flOverpaymentAmount;
                            flPaidAmount = flPayAmount;
                        }

                        pe.Model.flPaymentItems = choosenPayments.Select(x => new PaymentItemModel() {
                            flId = x.Id,
                            flDateTime = x.Date,
                            flAmount = x.Amount,
                            flPurpose = x.Purpose,
                            flIsGuarantee = guaranteePaymentsList.Any(gp => gp.Id.Id == x.Id.Id)
                        }).ToArray();

                        pe.Model.flRealAmount = pe.Model.flPaymentItems.Where(x => !x.flIsGuarantee).Sum(x => x.flAmount);
                        pe.Model.flGuaranteeAmount = pe.Model.flPaymentItems.Where(x => x.flIsGuarantee).Sum(x => x.flAmount);
                        pe.Model.flSendAmount = pe.Model.flRealAmount - pe.Model.flOverpaymentAmount.Value;
                        pe.Model.flHasSendAmount = true;
                        if (pe.Model.flSendAmount <= 0) {
                            pe.Model.flHasSendAmount = false;
                            pe.Model.flSendAmount = 0;
                        }
                        if (pe.Model.flGuaranteeAmount >= flNeedForFullPayAmount) {
                            pe.Model.flOverpaymentSendAmount -= pe.Model.flGuaranteeAmount - flNeedForFullPayAmount;
                            if (pe.Model.flOverpaymentSendAmount <= 0) {
                                pe.Model.flSendOverpayment = false;
                                pe.Model.flOverpaymentSendAmount = 0;
                            }
                        }
                    })
                )
                .Step("Реквизиты для возврата переплаты победителю", step => step
                    .Enabled(env => {
                        return env.Model.flSendOverpayment;
                    })
                    .OnRendering(re => {
                        if (re.Model.flOverpayment) {
                            new Panel("alert alert-warning")
                                .Append(new HtmlText(re.Env.T("Обнаружена переплата!")))
                                .AppendTo(re.Panel);
                        }
                        var tbRenderPaymentMatches = new TbRenderPaymentMatches();
                        re.Panel.AddLabel($"Контакты победителя: {re.Model.flOverpaymentRequisites.flContacts}", "d-block alert alert-secondary p-2 mb-3");
                        tbRenderPaymentMatches.flOverpaymentXin.RenderCustomT(re.Panel, re.Env, re.Model.flOverpaymentRequisites.flXin, readOnly: true);
                        tbRenderPaymentMatches.flOverpaymentName.RenderCustomT(re.Panel, re.Env, re.Model.flOverpaymentRequisites.flName, readOnly: true);
                        tbRenderPaymentMatches.flOverpaymentBik.RenderCustomT(re.Panel, re.Env, re.Model.flOverpaymentRequisites.flBik);
                        tbRenderPaymentMatches.flOverpaymentIban.RenderCustomT(re.Panel, re.Env, re.Model.flOverpaymentRequisites.flIban);
                        tbRenderPaymentMatches.flOverpaymentKbe.RenderCustomT(re.Panel, re.Env, re.Model.flOverpaymentRequisites.flKbe);
                        tbRenderPaymentMatches.flOverpaymentKnp.RenderCustomT(re.Panel, re.Env, re.Model.flOverpaymentRequisites.flKnp);
                        //tbRenderPaymentMatches.flOverpaymentKbk.RenderCustomT(re.Panel, re.Env, re.Model.flOverpaymentKbk);

                    })
                    .OnValidating(ve => {
                        var tbRenderPaymentMatches = new TbRenderPaymentMatches();
                        new Field[] {
                            tbRenderPaymentMatches.flOverpaymentBik,
                            tbRenderPaymentMatches.flOverpaymentIban,
                            tbRenderPaymentMatches.flOverpaymentKbe,
                            tbRenderPaymentMatches.flOverpaymentKnp,
                            //tbRenderPaymentMatches.flOverpaymentKbk
                        }.Each(x => x.Validate(ve.Env));
                    })
                    .OnProcessing(pe => {
                        var tbRenderPaymentMatches = new TbRenderPaymentMatches();
                        pe.Model.flOverpaymentRequisites.flBik = tbRenderPaymentMatches.flOverpaymentBik.GetVal(pe.Env);
                        pe.Model.flOverpaymentRequisites.flIban = tbRenderPaymentMatches.flOverpaymentIban.GetVal(pe.Env);
                        pe.Model.flOverpaymentRequisites.flKbe = tbRenderPaymentMatches.flOverpaymentKbe.GetVal(pe.Env);
                        pe.Model.flOverpaymentRequisites.flKnp = tbRenderPaymentMatches.flOverpaymentKnp.GetVal(pe.Env);
                        //pe.Model.flOverpaymentKbk = tbRenderPaymentMatches.flOverpaymentKbk.GetVal(pe.Env);
                    })
                )
                .Step("Просмотр", step => step
                    .OnRendering(re => {
                        ViewModel(re.Panel, new RenderActionEnv<PaymentMatchesOrderQueryArgs> ("", re.Args.OriginalArgs, re.Env.RequestContext, re.Env.FormCollection, re.Env.ViewData, re.Env.Redirect, null, null), re.Model);
                    })
                )
                ;
        }


        public override PaymentMatchModel GetModel(ActionEnv<PaymentMatchesOrderQueryArgs> env, GetModelFrom from, PaymentMatchesOrderTypes orderType, ITransaction transaction) {

            PaymentMatchModel getPaymentMatchModel() {
                var AgreementType = AgreementHelper.GetAgreementType(env.Args.AgreementId.Value, env.QueryExecuter);
                var agrTempl = AgreementHelper.GetAgreementTypeModel(AgreementType);
                var requisites = agrTempl.GetPaymentAndOverpaymentRequisites(env, env.Args.AgreementId.Value);
                var guaranteePaymentItems = agrTempl.GetGuaranteePayments(env, env.Args.AgreementId.Value);

                return new PaymentMatchModel() {
                    flAgreementId = env.Args.AgreementId.Value,

                    flPaymentItems = guaranteePaymentItems,

                    flRequisites = new DefaultAgrTemplate.RequisitesModel() {
                        flName = requisites.flPayment.flName,
                        flXin = requisites.flPayment.flXin,
                        flBik = requisites.flPayment.flBik,
                        flIban = requisites.flPayment.flIban,
                        flKbe = requisites.flPayment.flKbe,
                        flKnp = requisites.flPayment.flKnp,
                        flKbk = requisites.flPayment.flKbk
                    },

                    flOverpaymentRequisites = new DefaultAgrTemplate.RequisitesModel() {
                        flName = requisites.flOverPayment.flName,
                        flXin = requisites.flOverPayment.flXin,
                        flBik = requisites.flOverPayment.flBik,
                        flIban = requisites.flOverPayment.flIban,
                        flKbe = requisites.flOverPayment.flKbe,
                        flKnp = requisites.flOverPayment.flKnp,
                        flKbk = requisites.flOverPayment.flKbk,
                        flContacts = requisites.flOverPayment.flContacts
                    }
                };
            }

            var model = from switch {
                GetModelFrom.Empty => getPaymentMatchModel(),
                GetModelFrom.Entity =>
                    new TbPaymentMatches()
                        .AddFilter(t => t.flId, env.Args.Id.Value)
                        .GetPaymentMatchModelFirstOrDefault(env.QueryExecuter, transaction),
                GetModelFrom.Revision =>
                    new TbPaymentMatchesRevisions()
                        .AddFilter(t => t.flId, env.Args.RevisionId.Value)
                        .GetPaymentMatchModelFirstOrDefault(env.QueryExecuter, transaction),
                _ => throw new NotImplementedException()
            };

            return model;
        }

        public override PaymentMatchesOrderTypes GetOrderType(ActionEnv<PaymentMatchesOrderQueryArgs> env, GetOrderModelFrom orderModelFrom) {
            return orderModelFrom switch {
                GetOrderModelFrom.Empty => PaymentMatchesOrderTypes.Create,
                _ => throw new ArgumentException($"OrderType: {env.Args.OrderType}")
            };
        }

        public override string GetWizardCancelRedirectUrl(ActionEnv<PaymentMatchesOrderQueryArgs> env, PaymentMatchesOrderTypes orderType, GetModelFrom modelFrom) {
            var urlHelper = env.RequestContext.GetUrlHelper();
            //TODO
            return modelFrom switch {
                GetModelFrom.Empty => urlHelper.YodaAction(ModuleName, nameof(PaymentMatchesOrderQueryArgs)),
                GetModelFrom.Entity => urlHelper.YodaAction(ModuleName, nameof(PaymentMatchesOrderQueryArgs), new PaymentMatchesOrderQueryArgs { Id = env.Args.Id.Value, MenuAction = "view" }),
                GetModelFrom.Revision => urlHelper.YodaAction(ModuleName, MenuName, new PaymentMatchesOrderQueryArgs { RevisionId = env.Args.RevisionId, MenuAction = Actions.ViewOrder }),
                _ => throw new NotImplementedException()
            };
        }

        public override ValidationResultBase IsCreateFromActionValid(ActionEnv<PaymentMatchesOrderQueryArgs> env) {
            if (env.Args.Id == null) {
                return new RedirectResult(new ArgumentException("Id"));
            }

            if (env.Args.OrderType == null) {
                return new RedirectResult(new ArgumentException("OrderType"));
            }

            if (!CanCreateOrder(env.Args.Id.Value, GetOrderType(env, GetOrderModelFrom.Entity), out var error, env)) {
                if (env.Args.Id.HasValue) {
                    return new RedirectResult(ModuleName, MenuName, new PaymentMatchesOrderQueryArgs() { Id = env.Args.Id, AgreementId = env.Args.AgreementId, RevisionId = env.Args.RevisionId, MenuAction = Actions.ViewOrder }, error);
                }
                else {
                    return new RedirectResult(ModuleName, "ActionError", null, error);
                }
            }

            return new OkResult();
        }

        public static bool CanCreateOrder(int id, PaymentMatchesOrderTypes orderType, out string error, ActionEnv<PaymentMatchesOrderQueryArgs> env) {
            switch (orderType) {
                case PaymentMatchesOrderTypes.Create:
                error = env.T("Недопустимый тип приказа");
                return false;
                default:
                throw new NotImplementedException($"Unknown PaymentMatchesOrderType: {orderType}");
            }
        }

        public override void SaveModel(int revisionId, PaymentMatchModel model, PaymentMatchesOrderTypes orderType, ActionEnv<PaymentMatchesOrderQueryArgs> env, ITransaction transaction, GetModelFrom modelFrom) {
            var tbPaymentMatchesRevs = new TbPaymentMatchesRevisions();

            if (modelFrom == GetModelFrom.Empty) {
                model.flId = new TbPaymentMatches().flId.GetNextId(env.QueryExecuter, transaction);
            }

            var updateOrInsert = modelFrom switch {
                GetModelFrom.Revision =>
                    tbPaymentMatchesRevs
                        .AddFilter(t => t.flId, revisionId)
                        .Update(),

                _ => tbPaymentMatchesRevs
                    .Insert()
            };
            model.flId = revisionId;

            setModel(updateOrInsert, model);

            updateOrInsert
                .Execute(env.QueryExecuter, transaction);


        }

        private void setModel<TTradesTable>(DataModifingQueryProxy<TTradesTable> updateOrInsert, PaymentMatchModel model) where TTradesTable : TbPaymentMatches {

            updateOrInsert
                .SetT(t => t.flId, model.flId)
                .SetT(t => t.flPaymentId, model.flPaymentId)
                .SetT(t => t.flAgreementId, model.flAgreementId)
                .SetT(t => t.flDateTime, model.flDateTime)
                .SetT(t => t.flPaymentItems, model.flPaymentItems)
                .SetT(t => t.flStatus, PaymentMatchStatus.Linked)
                .SetT(t => t.flAmount, model.flAmount)
                .SetT(t => t.flGuaranteeAmount, model.flGuaranteeAmount)
                .SetT(t => t.flRealAmount, model.flRealAmount)
                .SetT(t => t.flHasSendAmount, model.flHasSendAmount)
                .SetT(t => t.flSendAmount, model.flSendAmount)
                .SetT(t => t.flRequisites, model.flRequisites)
                .SetT(t => t.flOverpayment, model.flOverpayment)
                .SetT(t => t.flSendOverpayment, model.flSendOverpayment)
                .SetT(t => t.flOverpaymentRequisites, model.flOverpaymentRequisites)
                ;

            if (model.flOverpayment) {
                updateOrInsert
                    .SetT(t => t.flOverpaymentAmount, model.flOverpaymentAmount);
            }
            else {
                updateOrInsert
                    .SetT(t => t.flOverpaymentAmount, 0);
            }

            if (model.flSendOverpayment) {
                updateOrInsert
                    .SetT(t => t.flOverpaymentSendAmount, model.flOverpaymentSendAmount);
            }
            else {
                updateOrInsert
                    .SetT(t => t.flOverpaymentSendAmount, 0);
            }

        }

        public override bool TryAcceptModel(int revisionId, PaymentMatchModel model, PaymentMatchesOrderTypes orderType, ActionEnv<PaymentMatchesOrderQueryArgs> env, ITransaction transaction, out string error) {
            var result = true;
            var errorsList = new List<string>() { };

            if (orderType == PaymentMatchesOrderTypes.Create) {
                if (new TbPayments().AddFilter(t => t.flAgreementId, model.flAgreementId).AddFilter(t => t.flPaymentStatus, PaymentStatus.Paid).Count(env.QueryExecuter) > 0) {
                    result = false;
                    errorsList.Add(env.T("Оплата уже привязана!"));
                }
            }

            if (env.IsValid && errorsList.Count == 0 && result == true) {
                if (env.Args.MenuAction == MnuOrderV2Actions.Execute) {

                    model.flDateTime = DateTime.Now;

                    var tbPayments = new TbPayments()
                                .AddFilter(t => t.flAgreementId, model.flAgreementId);

                    var agreementType = AgreementHelper.GetAgreementType(model.flAgreementId, env.QueryExecuter);
                    var agrTempl = AgreementHelper.GetAgreementTypeModel(agreementType);
                    PaymentModel paymentModel = null;
                    if (tbPayments.Count(env.QueryExecuter) > 0) {
                        paymentModel = tbPayments.GetPaymentModelFirstOrDefault(env.QueryExecuter, null);
                    }
                    var flIsUpdate = paymentModel != null;
                    var flPayAmount = (paymentModel == null ? Math.Ceiling(agrTempl.GetSellPrice(env, model.flAgreementId).Value) : paymentModel.flPayAmount);
                    var flPaidAmount = (paymentModel == null ? model.flPaymentItems.Sum(x => x.flAmount) : paymentModel.flPaidAmount + model.flPaymentItems.Sum(x => x.flAmount));
                    if (model.flOverpayment) {
                        flPaidAmount = flPayAmount;
                    }
                    model.flPaymentId = paymentModel != null ? paymentModel.flPaymentId : tbPayments.flPaymentId.GetNextId(env.QueryExecuter);

                    var tbPaymentsUpdateOrInsert = flIsUpdate ? tbPayments.Update() : tbPayments.Insert();
                    tbPaymentsUpdateOrInsert
                        .SetT(t => t.flAgreementId, model.flAgreementId)
                        .SetT(t => t.flPaymentId, model.flPaymentId)
                        .SetT(t => t.flPaymentStatus, flPaidAmount >= flPayAmount ? PaymentStatus.Paid : PaymentStatus.Paying)
                        .SetT(t => t.flPayAmount, flPayAmount)
                        .SetT(t => t.flPaidAmount, flPaidAmount)
                        ;

                    var tbPaymentMatches = new TbPaymentMatches();
                    var tbPaymentMatchesInsert = tbPaymentMatches.Insert();
                    setModel(tbPaymentMatchesInsert, model);

                    var paymentsProvider = new TraderesourcesPaymentsProvider(model.flOverpaymentRequisites.flXin, model.flOverpaymentRequisites.flName, env.RequestContext);
                    var paymentIds = model.flPaymentItems.Where(x => !x.flIsGuarantee).Select(x => x.flId).ToArray();
                    if (paymentIds.Length > 0) {
                        var matchResult = paymentsProvider.SellPayments.MatchPayments(paymentIds, model.flId.ToString(), "traderesources-agreements-payments-match");
                        tbPaymentMatchesInsert.SetT(t => t.flMatchResult, matchResult);
                        paymentsProvider.SellPayments.SettlePayments(paymentIds, matchResult);
                    }

                    tbPaymentsUpdateOrInsert.Execute(env.QueryExecuter, transaction);
                    tbPaymentMatchesInsert.Execute(env.QueryExecuter, transaction);
                }
            }

            error = string.Join(" ", errorsList);
            return result;
        }

        public override bool TryValidateModel(int revisionId, PaymentMatchModel model, PaymentMatchesOrderTypes orderType, ActionEnv<PaymentMatchesOrderQueryArgs> env, out string[] errors) {
            var result = true;
            var errorsList = new List<string>() { };

            errors = errorsList.ToArray();
            return result;
        }

        public override void ViewModel(RenderActionEnv<PaymentMatchesOrderQueryArgs> env, PaymentMatchModel trade, PaymentMatchesOrderTypes orderType, OrderStatus orderStatus) {
            ViewModel(env.Form, env, trade);
        }

        public static void ViewModel(WidgetBase widget, RenderActionEnv<PaymentMatchesOrderQueryArgs> env, PaymentMatchModel model) {
            if (model.flOverpayment) {
                new Panel("alert alert-warning")
                    .Append(new HtmlText(env.T("Обнаружена переплата! Внимательно изучите итоговые вычисления!")))
                    .AppendTo(widget);
            }

            var cols = 2;
            var tbPaymentItems = new TbRenderPaymentItems();
            new Card($"{model.flDateTime:dd.MM.yyyy hh:mm} - {model.flAmount:#,##0.00} тг.").Append(new GridRow().AppendRange(model.flPaymentItems.OrderByDescending(item => item.flDateTime).Select(item =>
            {
                var col = new GridCol($"col col-md-{12 / cols}");
                var card = new Card($"№{item.flId.Id} - {item.flAmount:#,##0.00} тг. от {item.flDateTime:dd.MM.yyyy hh:mm}{(item.flIsGuarantee ? " (гарантийный взнос)" : "")}") {
                    CssClass = "card-fluid"
                }.AppendTo(col);
                tbPaymentItems.flPurpose.RenderCustom(card, env, item.flPurpose, readOnly: true, orientation: FormOrientation.Basic, readOnlyCssClass: "d-block h5", hideLabel: true);
                return col;
            }))).AppendTo(widget);

            var tbPaymentMatches = new TbPaymentMatches();
            tbPaymentMatches.flAmount.RenderCustomT(widget, env, model.flAmount, readOnly: true);
            tbPaymentMatches.flGuaranteeAmount.RenderCustomT(widget, env, model.flGuaranteeAmount, readOnly: true);
            tbPaymentMatches.flRealAmount.RenderCustomT(widget, env, model.flRealAmount, readOnly: true);
            tbPaymentMatches.flHasSendAmount.RenderCustomT(widget, env, model.flHasSendAmount, readOnly: true);
            if (model.flHasSendAmount) {
                tbPaymentMatches.flSendAmount.RenderCustomT(widget, env, model.flSendAmount, readOnly: true);
            }

            var agreementType = AgreementHelper.GetAgreementType(model.flAgreementId, env.QueryExecuter);
            var agrTempl = AgreementHelper.GetAgreementTypeModel(agreementType);
            PaymentModel paymentModel = null;
            var tbPayments = new TbPayments()
                    .AddFilter(t => t.flAgreementId, model.flAgreementId);
            if (tbPayments.Count(env.QueryExecuter) > 0) {
                paymentModel = tbPayments.GetPaymentModelFirstOrDefault(env.QueryExecuter, null);
            }
            var flPayAmount = (paymentModel == null ? Math.Ceiling(agrTempl.GetSellPrice(env, model.flAgreementId).Value) : paymentModel.flPayAmount);
            var flPaidAmount = (paymentModel == null ? model.flPaymentItems.Sum(x => x.flAmount) : paymentModel.flPaymentMatches.Any(paymentMatch => paymentMatch.flId == model.flId) ? paymentModel.flPaidAmount : paymentModel.flPaidAmount + model.flPaymentItems.Sum(x => x.flAmount));

            tbPayments.flPayAmount.RenderCustomT(widget, env, flPayAmount, readOnly: true);
            tbPayments.flPaidAmount.RenderCustomT(widget, env, flPaidAmount, readOnly: true);

            var tbRenderPaymentMatches = new TbRenderPaymentMatches();
            var budgetCard = new Card("Реквизиты для отправки денег в бюджет").AppendTo(widget);
            tbRenderPaymentMatches.flXin.RenderCustomT(budgetCard, env, model.flRequisites.flXin, readOnly: true);
            tbRenderPaymentMatches.flName.RenderCustomT(budgetCard, env, model.flRequisites.flName, readOnly: true);
            tbRenderPaymentMatches.flBik.RenderCustomT(budgetCard, env, model.flRequisites.flBik, readOnly: true);
            tbRenderPaymentMatches.flIban.RenderCustomT(budgetCard, env, model.flRequisites.flIban, readOnly: true);
            tbRenderPaymentMatches.flKnp.RenderCustomT(budgetCard, env, model.flRequisites.flKnp, readOnly: true);
            tbRenderPaymentMatches.flKbe.RenderCustomT(budgetCard, env, model.flRequisites.flKbe, readOnly: true);
            tbRenderPaymentMatches.flKbk.RenderCustomT(budgetCard, env, model.flRequisites.flKbk, readOnly: true);


            tbPaymentMatches.flOverpayment.RenderCustomT(widget, env, model.flOverpayment, readOnly: true);

            if (model.flSendOverpayment) {
                tbPaymentMatches.flOverpaymentAmount.RenderCustomT(widget, env, model.flOverpaymentAmount, readOnly: true);
            }

            tbPaymentMatches.flSendOverpayment.RenderCustomT(widget, env, model.flSendOverpayment, readOnly: true);

            if (model.flSendOverpayment) {
                widget.Append(new Br());
                tbPaymentMatches.flOverpaymentSendAmount.RenderCustomT(widget, env, model.flOverpaymentSendAmount, readOnly: true);
                var returnCard = new Card("Реквизиты для возврата переплаты победителю").AppendTo(widget);

                tbRenderPaymentMatches.flOverpaymentXin.RenderCustomT(returnCard, env, model.flOverpaymentRequisites.flXin, readOnly: true);
                tbRenderPaymentMatches.flOverpaymentName.RenderCustomT(returnCard, env, model.flOverpaymentRequisites.flName, readOnly: true);
                tbRenderPaymentMatches.flOverpaymentBik.RenderCustomT(returnCard, env, model.flOverpaymentRequisites.flBik, readOnly: true);
                tbRenderPaymentMatches.flOverpaymentIban.RenderCustomT(returnCard, env, model.flOverpaymentRequisites.flIban, readOnly: true);
                tbRenderPaymentMatches.flOverpaymentKnp.RenderCustomT(returnCard, env, model.flOverpaymentRequisites.flKnp, readOnly: true);
                tbRenderPaymentMatches.flOverpaymentKbe.RenderCustomT(returnCard, env, model.flOverpaymentRequisites.flKbe, readOnly: true);
                //tbRenderPaymentMatches.flOverpaymentKbk.RenderCustomT(returnCard, env, model.flOverpaymentKbk, readOnly: true);
            }
        }

        protected override bool HasAccessToOrder(int revisionId, ActionEnv<PaymentMatchesOrderQueryArgs> env) {
            if ((!env.User.IsExternalUser() && !env.User.IsGuest())) {
                return true;
            }
            if (env.Args.MenuAction != "view-order" && !(env.User.HasPermission(ModuleName, AccountActivityTypesProvider.PaymentOrders.CreateOrder.Name) && env.User.HasPermission(ModuleName, AccountActivityTypesProvider.PaymentOrders.EditOrder.Name))) {
                throw new AccessDeniedException();
            }
            return true;
        }
    }


    public class PaymentMatchesOrderQueryArgs : OrderQueryArgs {
        public int? Id { get; set; }
        public int? AgreementId { get; set; }
        public PaymentMatchesOrderTypeActions? OrderType { get; set; }
    }
    public enum PaymentMatchesOrderTypeActions {
        Create,
        Edit
    }
}
