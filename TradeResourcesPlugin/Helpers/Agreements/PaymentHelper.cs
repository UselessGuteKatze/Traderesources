using CommonSource.SearchCollections.Object;
using HydrocarbonSource.SearchCollections.Object;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using PaymentsApi.Client;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using TradeResourcesPlugin.Modules.FishingMenus.Agreements;
using TradeResourcesPlugin.Modules.HuntingMenus.Agreements;
using TradeResourcesPlugin.Modules.LandObjectsMenus.Agreements;
using UsersResources;
using Yoda.Interfaces;
using Yoda.Interfaces.Forms;
using Yoda.Interfaces.Forms.Components;
using YodaHelpers.Fields;
using YodaHelpers.Payments;
using YodaQuery;
using static TradeResourcesPlugin.Helpers.DefaultAgrTemplate;

namespace TradeResourcesPlugin.Helpers.Agreements {
    public static class PaymentHelper
    {
        public static Card RenderPaymentData(PaymentModel paymentModel, AgreementsModel agreementsModel, bool isObjectSeller, bool isAgreementSigner, FormEnvironment env)
        {
            var isInternal = !env.User.IsExternalUser() && !env.User.IsGuest();
            var paymentData = new Card(env.T("Оплата"));
            var paymentDataActions = PaymentActions(paymentModel, agreementsModel, isObjectSeller, isAgreementSigner, env);
            if (paymentDataActions.Count > 0)
            {
                new Panel("bg-white bordered border-top form-wizard-header mt-1 pt-2 pl-2").AppendRange(paymentDataActions).AppendTo(paymentData);
            }
            if (paymentModel != null) {
                var tbPayments = new TbPayments();
                var tbPaymentMatches = new TbPaymentMatches();
                var tbPaymentItems = new TbRenderPaymentItems();

                var row = new GridRow().AppendTo(paymentData);
                var rowCol = new GridCol("col col-md-6").AppendTo(row);
                var rowCol2 = new GridCol("col col-md-6").AppendTo(row);

                tbPayments.flPaymentStatus.RenderCustom(rowCol, env, paymentModel.flPaymentStatus, readOnly: true);
                var percent = (int)decimal.Round((100 / paymentModel.flPayAmount) * paymentModel.flPaidAmount);
                if (percent > 100) percent = 100;
                new Panel("bg-secondary rounded mb-3 form-control p-0") {
                    Attributes = new Dictionary<string, object> {
                        { "style", $"position: relative;" }
                    }
                }
                    .Append(
                        new Panel("bg-success rounded p-2") {
                            Attributes = new Dictionary<string, object> {
                                { "style", $"width: {percent}%; height: 100%;" }
                            }
                        }
                    ).Append(
                        new Label($"{paymentModel.flPaidAmount:#,##0.00} тг. / {paymentModel.flPayAmount:#,##0.00} тг.", "d-relative text-center font-weight-bold text-light") {
                            Attributes = new Dictionary<string, object> {
                                { "style", $"width: 100%; position: absolute; top: 50%; transform: translateY(-50%);" }
                            }
                        }
                    )
                    .AppendTo(rowCol2);

                if ((isObjectSeller || isAgreementSigner || isInternal) && agreementsModel.flAgreementSignDate.HasValue) {
                    var cols = 2;
                    var paymentMatches = new Accordion("Привязки платежей").AppendTo(paymentData);
                    var refPaymentMatchStatuses = new RefPaymentMatchStatuses();
                    paymentModel.flPaymentMatches.OrderByDescending(paymentMatch => paymentMatch.flDateTime).Each(paymentMatch => {
                        var internPanel = new Panel();
                        if (!env.User.IsExternalUser() && !env.User.IsGuest()) {
                            tbPaymentMatches.flAmount.RenderCustom(internPanel, env, paymentMatch.flAmount, readOnly: true);
                            tbPaymentMatches.flGuaranteeAmount.RenderCustom(internPanel, env, paymentMatch.flGuaranteeAmount, readOnly: true);
                            tbPaymentMatches.flRealAmount.RenderCustom(internPanel, env, paymentMatch.flRealAmount, readOnly: true);
                            tbPaymentMatches.flHasSendAmount.RenderCustom(internPanel, env, paymentMatch.flHasSendAmount, readOnly: true);
                            tbPaymentMatches.flSendAmount.RenderCustom(internPanel, env, paymentMatch.flSendAmount, readOnly: true);
                            tbPaymentMatches.flOverpayment.RenderCustom(internPanel, env, paymentMatch.flOverpayment, readOnly: true);
                            tbPaymentMatches.flOverpaymentAmount.RenderCustom(internPanel, env, paymentMatch.flOverpaymentAmount, readOnly: true);
                            tbPaymentMatches.flSendOverpayment.RenderCustom(internPanel, env, paymentMatch.flSendOverpayment, readOnly: true);
                            tbPaymentMatches.flOverpaymentSendAmount.RenderCustom(internPanel, env, paymentMatch.flOverpaymentSendAmount, readOnly: true);
                        }

                        new Accordion($"{paymentMatch.flDateTime:dd.MM.yyyy hh:mm} - {paymentMatch.flAmount:#,##0.00} тг. ({refPaymentMatchStatuses.Search(paymentMatch.flStatus.ToString()).Text})", true).Append(internPanel).Append(new GridRow().AppendRange(paymentMatch.flPaymentItems.OrderByDescending(item => item.flDateTime).Select(item => {
                            var col = new GridCol($"col col-md-{12 / cols}");
                            var card = new Card($"№{item.flId.Id} - {item.flAmount:#,##0.00} тг. от {item.flDateTime:dd.MM.yyyy hh:mm}{(item.flIsGuarantee ? " (гарантийный взнос)" : "")}") {
                                CssClass = "card-fluid"
                            }.AppendTo(col);
                            //tbPaymentItems.flPaymentItemId.RenderCustom(card, env, item.flId.Id, readOnly: true, orientation: FormOrientation.Basic, readOnlyCssClass: "d-block h5");
                            //tbPaymentItems.flPaymentItemStatus.RenderCustom(card, env, item.flPaymentItemStatus.ToString(), readOnly: true, orientation: FormOrientation.Basic, readOnlyCssClass: "d-block h5");
                            //tbPaymentItems.flAmount.RenderCustom(card, env, item.flAmount, readOnly: true, orientation: FormOrientation.Basic, readOnlyCssClass: "d-block h5");
                            //tbPaymentItems.flDateTime.RenderCustom(card, env, item.flDateTime, readOnly: true, orientation: FormOrientation.Basic, readOnlyCssClass: "d-block h5");

                            tbPaymentItems.flPurpose.RenderCustom(card, env, item.flPurpose, readOnly: true, orientation: FormOrientation.Basic, readOnlyCssClass: "d-block h5", hideLabel: true);
                            return col;
                        }))).AppendTo(paymentMatches);
                    });

                    if (paymentModel.flPaymentStatus == PaymentStatus.Paid) {
                        var client = env.RequestContext.AppEnv.ServiceProvider.GetRequiredService<ITreasuryPaymentsClientFactory>().CreateClientAsync().Result;
                        var paymentsProject = agreementsModel.flAgreementType switch {
                            nameof(ДоговорНаВедениеОхотничьегоХозяйства) => Projects.AnimalWorldHunting,
                            nameof(ДоговорНаВедениеРыбногоХозяйстваОтрхИСрх) => Projects.AnimalWorldFishing,
                            nameof(ДоговорНаВедениеРыбногоХозяйстваПрхИЛрх) => Projects.AnimalWorldFishing,
                            nameof(ДоговорКпЗемельногоУчастка) => Projects.LandResources,
                            nameof(ДоговорКпПраваАрендыЗемельногоУчастка) => Projects.LandResources,
                            _ => throw new NotImplementedException($"Unknown agreement type {agreementsModel.flAgreementType}")
                        };
                        var sendedPayments = client.GetPaymentsAsync(new PaymentsArgs() { Year = agreementsModel.flAgreementSignDate.Value.Year, Project = paymentsProject }).Result;
                        var equalPayments = sendedPayments.Result.Payments.Where(sendedPayment => sendedPayment.Description.Contains(agreementsModel.flAuctionId.ToString()));
                        equalPayments.Each(async sendedPayment => {
                            //var sendedPaymentData = await client.GetPaymentAsync(new PaymentArgs() { PaymentId = sendedPayment.Id, Year = 2022, Project = Projects.AnimalWorldFishing });
                            var beneficiaryName = sendedPayment.BeneficiaryName;
                            try {
                                beneficiaryName = new GrObjectSearchCollection().GetItem(sendedPayment.BeneficiaryBin, env.RequestContext).SearchItemText;
                            }
                            catch (Exception ex) {
                            }
                            new Panel("alert alert-success").AppendTo(paymentData).Append(new HtmlText(beneficiaryName + "<br>" + sendedPayment.Amount.ToString("#,##0.00") + " тг. - " + sendedPayment.Description));
                        });
                    }
                }
            }
            return paymentData;
        }
        public static List<LinkBase> PaymentActions(PaymentModel paymentModel, AgreementsModel agreementsModel, bool isObjectSeller, bool isAgreementSigner, FormEnvironment env) {
            var actions = new List<LinkBase>();
            if (agreementsModel != null && agreementsModel.flAgreementStatus == AgreementStatuses.Signed) {

                if (!(paymentModel != null && paymentModel.flPaidAmount >= paymentModel.flPayAmount)) {

                    var notAcceptedRevisions = new TbPaymentMatchesRevisions()
                        .AddFilter(t => t.flAgreementId, agreementsModel.flAgreementId)
                        .Select(t => new FieldAlias[] { t.flId }, env.QueryExecuter)
                        .Select(r => r.GetVal(t => t.flId))
                        .ToArray();

                    if (notAcceptedRevisions.Length > 0) {
                        notAcceptedRevisions = new TbPaymentMatchesOrderResult()
                        .AddFilter(t => t.flSubjectId, ConditionOperator.In, notAcceptedRevisions)
                        .AddFilter(t => t.flStatus, ConditionOperator.NotIn, new[] { YodaHelpers.OrderHelpers.OrderStatus.Canceled.ToString(), YodaHelpers.OrderHelpers.OrderStatus.Complished.ToString() })
                        .Select(t => new FieldAlias[] { t.flSubjectId }, env.QueryExecuter)
                        .Select(r => r.GetVal(t => t.flSubjectId))
                        .ToArray();
                    }

                    if (notAcceptedRevisions.Length > 0) {
                        notAcceptedRevisions.Each(notAcceptedRevision => {
                            actions.Add(new Link {
                                CssClass = "btn btn-sm btn-dark font-weight-normal text-wrap mr-2 mb-2",
                                Text = env.T($"Перейти к приказу №{notAcceptedRevision} на привязку платежей"),
                                Controller = nameof(RegistersModule),
                                Action = nameof(MnuPaymentMatchOrder),
                                RouteValues = new PaymentMatchesOrderQueryArgs { MenuAction = MnuPaymentMatchOrder.Actions.ViewOrder, AgreementId = agreementsModel.flAgreementId, RevisionId = notAcceptedRevision }
                            });
                        });
                    }
                    else {
                        if (isObjectSeller) {
                            actions.Add(new Link {
                                CssClass = "btn btn-sm btn-dark font-weight-normal text-wrap mr-2 mb-2",
                                Text = env.T("Создать приказ на привязку платежей"),
                                Controller = nameof(RegistersModule),
                                Action = nameof(MnuPaymentMatchOrder),
                                RouteValues = new PaymentMatchesOrderQueryArgs { MenuAction = MnuPaymentMatchOrder.Actions.CreateNew, AgreementId = agreementsModel.flAgreementId }
                            });
                        }
                    }
                }
            }
            return actions;
        }
        public static PaymentModel GetPaymentModel(this SelectFirstResultProxy<TbPayments> paymentRow, IQueryExecuter queryExecuter, ITransaction transaction = null)
        {
            if (!paymentRow.IsFirstRowExists)
            {
                return null;
            }

            var paymentModel = new PaymentModel
            {
                flPaymentId = paymentRow.GetVal(t => t.flPaymentId),
                flAgreementId = paymentRow.GetVal(t => t.flAgreementId),
                flPaymentStatus = paymentRow.GetVal(t => t.flPaymentStatus),
                flPayAmount = paymentRow.GetVal(t => t.flPayAmount),
                flPaidAmount = paymentRow.GetVal(t => t.flPaidAmount),
                flPaymentMatches = new TbPaymentMatches()
                    .AddFilter(t => t.flPaymentId, paymentRow.GetVal(t => t.flPaymentId))
                    .GetPaymentMatches(queryExecuter, transaction)
            };

            return paymentModel;
        }
        public static PaymentModel GetPaymentModelFirstOrDefault(this TbPayments tbPayments, IQueryExecuter queryExecuter, ITransaction transaction = null)
        {
            return tbPayments.SelectFirstOrDefault(t => t.Fields.ToFieldsAliases(), queryExecuter, transaction).GetPaymentModel(queryExecuter, transaction);
        }

        public static PaymentModel[] GetPaymentModels(this TbPayments tbPayments, IQueryExecuter queryExecuter, ITransaction transaction = null)
        {
            return tbPayments.Select(t => t.Fields.ToFieldsAliases(), queryExecuter, transaction).Select(row => row.GetPaymentModel(queryExecuter, transaction)).ToArray();
        }

        public static PaymentMatchModel GetPaymentMatchModel(this SelectFirstResultProxy<TbPaymentMatches> paymentItemRow)
        {
            if (!paymentItemRow.IsFirstRowExists)
            {
                return null;
            }

            return new PaymentMatchModel
            {
                flId = paymentItemRow.GetVal(t => t.flId),
                flPaymentId = paymentItemRow.GetVal(t => t.flPaymentId),
                flAgreementId = paymentItemRow.GetVal(t => t.flAgreementId),
                flDateTime = paymentItemRow.GetVal(t => t.flDateTime),
                flPaymentItems = paymentItemRow.GetVal(t => t.flPaymentItems),
                flStatus = paymentItemRow.GetVal(t => t.flStatus),
                flMatchResult = paymentItemRow.GetValOrNull(t => t.flMatchResult),
                flAmount = paymentItemRow.GetVal(t => t.flAmount),
                flGuaranteeAmount = paymentItemRow.GetVal(t => t.flGuaranteeAmount),
                flRealAmount = paymentItemRow.GetVal(t => t.flRealAmount),
                flHasSendAmount = paymentItemRow.GetVal(t => t.flHasSendAmount),
                flSendAmount = paymentItemRow.GetVal(t => t.flSendAmount),
                flMatchBlockResult = paymentItemRow.GetValOrNull(t => t.flMatchBlockResult),
                flRequisites = paymentItemRow.GetVal(t => t.flRequisites),
                flOverpayment = paymentItemRow.GetVal(t => t.flOverpayment),
                flOverpaymentAmount = paymentItemRow.GetValOrNull(t => t.flOverpaymentAmount),
                flSendOverpayment = paymentItemRow.GetVal(t => t.flSendOverpayment),
                flOverpaymentSendAmount = paymentItemRow.GetValOrNull(t => t.flOverpaymentSendAmount),
                flOverpaymentMatchBlockResult = paymentItemRow.GetValOrNull(t => t.flOverpaymentMatchBlockResult),
                flOverpaymentRequisites = paymentItemRow.GetValOrNull(t => t.flOverpaymentRequisites)
            };
        }
        
        public static PaymentMatchModel GetPaymentMatchModelFirstOrDefault(this TbPaymentMatches tbPaymentItems, IQueryExecuter queryExecuter, ITransaction transaction = null)
        {
            return tbPaymentItems.SelectFirstOrDefault(t => t.Fields.ToFieldsAliases(), queryExecuter, transaction).GetPaymentMatchModel();
        }

        public static PaymentMatchModel[] GetPaymentMatches(this TbPaymentMatches tbPaymentItems, IQueryExecuter queryExecuter, ITransaction transaction = null)
        {
            return tbPaymentItems.Select(t => t.Fields.ToFieldsAliases(), queryExecuter, transaction).Select(row => row.GetPaymentMatchModel()).OrderBy(item => item.flDateTime).ToArray();
        }

        public class TraderesourcesPaymentsProvider
        {
            public TraderesourcesPaymentsProvider(string userXin, string userFio, IYodaRequestContext rc)
            {
                GuaranteePayments = new IacPaymentsToSystemProvider(userXin, userFio, userXin, "171", rc.AppEnv.QueryExecuterProvider.GetDbConnectionStringSuperUser("dbYodaPaymentsGr"));
                SellPayments = new IacPaymentsToSystemProvider(userXin, userFio, userXin, "730", rc.AppEnv.QueryExecuterProvider.GetDbConnectionStringSuperUser("dbYodaPaymentsGr"));
            }

            public IUserPaymentsToSystem GuaranteePayments { get; private set; }
            public IUserPaymentsToSystem SellPayments { get; private set; }
        }
    }
    public class CheckBoxesBox : YodaFormElement
    {
        public int Columns { get; set; }
        public string Text { get; set; }
        public CheckBoxesBox(string name, ColumnsCount columnsCount, string text = null)
        {
            Name = name;
            Text = text;
            Columns = columnsCount switch
            {
                ColumnsCount.col1 => 1,
                ColumnsCount.col2 => 2,
                ColumnsCount.col3 => 3,
                ColumnsCount.col4 => 4,
                ColumnsCount.col6 => 6,
                ColumnsCount.col12 => 12
            };
        }

        private List<CheckBoxElement> CheckBoxElements = new List<CheckBoxElement>();

        public CheckBoxesBox AppendCheckbox(string value, YodaFormElement element, bool isChecked, string cssClass = null, bool enabled = true)
        {
            CheckBoxElements.Add(new CheckBoxElement(value, element, isChecked, cssClass, enabled));
            return this;
        }
        public CheckBoxesBox Append(YodaFormElement element, string cssClass = null)
        {
            CheckBoxElements.Add(new CheckBoxElement(null, element, false, cssClass, false));
            return this;
        }

        private class CheckBoxElement {
            public CheckBoxElement(string value, YodaFormElement element, bool isChecked, string cssClass = null, bool enabled = true)
            {
                Value = value;
                Element = element;
                Checked = isChecked;
                CssClass = $"{cssClass}";
                Enabled = enabled;
            }
            public string Value { get; set; }
            public YodaFormElement Element { get; set; }
            public bool Checked { get; set; }
            public string CssClass { get; set; }
            public bool Enabled { get; set; }
        }

        public enum ColumnsCount
        {
            col1,
            col2,
            col3,
            col4,
            col6,
            col12,
        }

        public string[] GetPostedValue(FormEnvironment env)
        {
            return JsonConvert.DeserializeObject<string[]>(env.FormCollection[Name]).Select(item => item.Replace($"{Name}-cbbi-", "")).ToArray();
        }

        public override string[] GetRequireUiPackages()
        {
            return new[] { "check-boxes-box" };
        }
        public override HtmlString ToHtmlString(IHtmlHelper html)
        {
            var panel = new Card(Text, "check-boxes-box-head", "check-boxes-box");
            panel.AddHtml($"<input type=\"text\" class=\"check-boxes-box-input d-none\" id=\"{Name}\" name=\"{Name}\">");
            var row = new GridRow().AppendRange(CheckBoxElements.Select(item =>
            {
                var col = new GridCol($"col col-md-{12 / Columns}");
                if (item.Enabled) {
                    col.AddHtml($"<input type=\"checkbox\" class=\"check-boxes-box-item btn-check d-none\" id=\"{Name}-cbbi-{item.Value}\" name=\"{Name}-cbbi-{item.Value}\" {(item.Checked ? "checked=\"checked\"" : "")} autocomplete=\"off\">");
                    item.Element.CssClass += " check-boxes-box-item-element";
                    col.AddHtml($"<label class=\"p-1 rounded card-fluid {item.CssClass}\" for=\"{Name}-cbbi-{item.Value}\">{item.Element.ToHtmlString(html)}</label>");
                } else {
                    col.AddHtml($"<input type=\"checkbox\" class=\"check-boxes-box-item btn-check d-none\" id=\"{Name}-cbbi-{item.Value}\" name=\"{Name}-cbbi-{item.Value}\" {(item.Checked ? "checked=\"checked\"" : "")} disabled=\"disabled\" autocomplete=\"off\">");
                    item.Element.CssClass += " check-boxes-box-item-element bg-light";
                    col.AddHtml($"<label class=\"p-1 rounded card-fluid {item.CssClass}\" for=\"{Name}-cbbi-{item.Value}\">{item.Element.ToHtmlString(html)}</label>");
                }
                return col;
            })).AppendTo(panel);

            return panel.ToHtmlString(html);
        }
    }
}
