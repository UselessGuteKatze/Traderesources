using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using TradeResourcesPlugin.Helpers.Agreements;
using TradeResourcesPlugin.Modules.FishingMenus.Agreements;
using TradeResourcesPlugin.Modules.HuntingMenus.Agreements;
using TradeResourcesPlugin.Modules.LandObjectsMenus.Agreements;
using Yoda.Interfaces;
using Yoda.Interfaces.Forms;
using Yoda.Interfaces.Forms.Components;
using YodaHelpers.ActionMenus;
using YodaHelpers.Components;
using YodaHelpers.Fields;
using YodaHelpers.HtmlDocumentBuilder;
using YodaQuery;
using static TradeResourcesPlugin.Helpers.DefaultDocTemplate;

namespace TradeResourcesPlugin.Helpers {
    public static class AgreementHelper {
        public static Card GetDataPanelHtml(ActionEnv<DefaultAgrTemplateArgs> env)
        {
            if (env.Args.AgreementId == 0)
                throw new NotImplementedException("AgreementId args null");

            var card = new Card("Договор");

            var row = new TbAgreements().Self(out var tbAgrs).AddFilter(t => t.flAgreementId, env.Args.AgreementId).SelectFirst(tbAgrs.Fields.ToFieldsAliases(), env.QueryExecuter);

            tbAgrs.flAgreementNumber.RenderCustom(card, env, tbAgrs.flAgreementNumber.GetRowVal(row), readOnly: true);
            tbAgrs.flAgreementId.RenderCustom(card, env, tbAgrs.flAgreementId.GetRowVal(row), readOnly: true);
            tbAgrs.flAgreementRevisionId.RenderCustom(card, env, tbAgrs.flAgreementRevisionId.GetRowVal(row), readOnly: true);
            tbAgrs.flAgreementType.RenderCustom(card, env, tbAgrs.flAgreementType.GetRowVal(row), readOnly: true);
            tbAgrs.flAgreementStatus.RenderCustom(card, env, tbAgrs.flAgreementStatus.GetRowVal(row), readOnly: true);
            tbAgrs.flObjectId.RenderCustom(card, env, tbAgrs.flObjectId.GetRowVal(row), readOnly: true);
            tbAgrs.flObjectType.RenderCustom(card, env, tbAgrs.flObjectType.GetRowVal(row), readOnly: true);
            tbAgrs.flTradeId.RenderCustom(card, env, tbAgrs.flTradeId.GetRowVal(row), readOnly: true);
            tbAgrs.flTradeType.RenderCustom(card, env, tbAgrs.flTradeType.GetRowVal(row), readOnly: true);
            tbAgrs.flAuctionId.RenderCustom(card, env, tbAgrs.flAuctionId.GetRowVal(row), readOnly: true);
            tbAgrs.flSellerBin.RenderCustom(card, env, tbAgrs.flSellerBin.GetRowVal(row), readOnly: true);
            tbAgrs.flWinnerXin.RenderCustom(card, env, tbAgrs.flWinnerXin.GetRowVal(row), readOnly: true);

            return card;
        }
        public static Accordion GetStoryPanelHtml(ActionEnv<DefaultAgrTemplateArgs> env)
        {
            if (env.Args.AgreementId == 0)
                throw new NotImplementedException("AgreementId args null");

            var card = new Accordion("История", true);
            var timeLine = new TimeLine();
            card.Elements.Add(timeLine);

            var reference = new RefAgreementStatuses();

            var flAgreementRevisionNumber = 1;
            var flPrevComment = "";

            new TbAgreementModels() { Name = "z_history_tbagreementmodels" }
                .Self(out var tbAgrs)
                .AddFilter(t => t.flAgreementId, env.Args.AgreementId)
                .Select(t => new FieldAlias[] { t.flAgreementRevisionId, t.flAgreementStatus, t.flDateTime, t.flComment, t.flCommentDateTime, t.flRequestDate }, env.QueryExecuter)
                .AsEnumerable()
                .OrderBy(r => tbAgrs.flRequestDate.GetRowVal(r.FirstRow))
                .Each(r => {
                    var flAgreementRevisionId = tbAgrs.flAgreementRevisionId.GetRowVal(r.FirstRow);
                    //var flDateTime = tbAgrs.flDateTime.GetRowVal(r.FirstRow).ToString("dd.MM.yyyy HH:mm");
                    var flAgreementStatus = reference.Search(tbAgrs.flAgreementStatus.GetRowVal(r.FirstRow)).Text.Text;
                    var flAgreementStatusDateTime = tbAgrs.flRequestDate.GetRowVal(r.FirstRow).ToString("dd.MM.yyyy HH:mm");
                    var flComment = tbAgrs.flComment.GetRowVal(r.FirstRow);
                    var text = $"{flAgreementRevisionNumber++}. Ревизия: {flAgreementRevisionId}, Статус: {flAgreementStatus}{(string.IsNullOrEmpty(flComment) || flPrevComment == flComment ? string.Empty : $", Примечание: {flComment}")}";
                    var timeLineItem = new TimeLineItem(text, flAgreementStatusDateTime);
                    timeLine.Elements.Add(timeLineItem);
                    flPrevComment = flComment;
                });



            return card;
        }
        public static void GetIfNeedPaymentPanelHtml(RenderActionEnv<DefaultAgrTemplateArgs> env)
        {
            if (env.Args.AgreementId == 0)
                throw new NotImplementedException("AgreementId args null");

            env.Args.AgreementType = GetAgreementType(env.Args.AgreementId, env.QueryExecuter);
            var agrTempl = GetAgreementTypeModel(env.Args.AgreementType);

            if (agrTempl.HasPayment())
            {
                PaymentModel paymentModel = null;
                var tbPayments = new TbPayments()
                        .AddFilter(t => t.flAgreementId, env.Args.AgreementId);
                if (tbPayments.Count(env.QueryExecuter) > 0)
                {
                    paymentModel = tbPayments.GetPaymentModelFirstOrDefault(env.QueryExecuter, null);
                }
                var agreementModel = new TbAgreements()
                    .AddFilter(t => t.flAgreementId, env.Args.AgreementId)
                    .GetAgreementsModelFirstOrDefault(env.QueryExecuter);
                env.Form.AddComponent(PaymentHelper.RenderPaymentData(paymentModel, agreementModel, agrTempl.IsSeller(env), agrTempl.IsAgreementCreator(env), env));
            }

        }
        public static bool AgreementExists(ActionEnv<DefaultAgrTemplateArgs> env)
        {
            var tbAgreements = new TbAgreements()
                .AddFilter(t => t.flAgreementType, env.Args.AgreementType)
                .AddFilterNot(t => t.flAgreementStatus, AgreementStatuses.Deleted)
                .AddFilterNot(t => t.flAgreementStatus, AgreementStatuses.Canceled);

            if (env.Args.ObjectId != null && !string.IsNullOrEmpty(env.Args.ObjectType))
            {
                tbAgreements
                .AddFilter(tbAgreements.flObjectId, ConditionOperator.Equal, env.Args.ObjectId)
                .AddFilter(tbAgreements.flObjectType, ConditionOperator.Equal, env.Args.ObjectType);
            }
            else if (env.Args.TradeId != null && !string.IsNullOrEmpty(env.Args.TradeType))
            {
                tbAgreements
                .AddFilter(tbAgreements.flTradeId, ConditionOperator.Equal, env.Args.TradeId)
                .AddFilter(tbAgreements.flTradeType, ConditionOperator.Equal, env.Args.TradeType);
            }
            else
            {
                throw new NotImplementedException("ObjectId, ObjectType and TradeId, TradeType args null");
            }

            return tbAgreements.Count(new FieldAlias[] { tbAgreements.flAgreementId }, env.QueryExecuter) > 0;
        }
        public static int GetAgreementActiveRevisionId(int agreementId, IQueryExecuter queryExecuter)
        {
            var agrs = new TbAgreements().AddFilter(t => t.flAgreementId, agreementId);
            return agrs.SelectScalar(t => t.flAgreementRevisionId, queryExecuter).Value;
        }
        public static int GetAgreementId(string agreementNumber, IQueryExecuter queryExecuter)
        {
            var agrs = new TbAgreements().AddFilter(t => t.flAgreementNumber, agreementNumber);
            return agrs.SelectScalar(t => t.flAgreementId, queryExecuter).Value;
        }
        public static int GetAgreementObjectId(int agreementId, IQueryExecuter queryExecuter)
        {
            var agrs = new TbAgreements().AddFilter(t => t.flAgreementId, agreementId);
            return agrs.SelectScalar(t => t.flObjectId, queryExecuter).Value;
        }
        public static int GetAgreementTradeId(int agreementId, IQueryExecuter queryExecuter, ITransaction transaction = null)
        {
            var agrs = new TbAgreements().AddFilter(t => t.flAgreementId, agreementId);
            return agrs.SelectScalar(t => t.flTradeId, queryExecuter, transaction).Value;
        }
        public static int GetAgreementAuctionId(int agreementId, IQueryExecuter queryExecuter, ITransaction transaction = null)
        {
            var agrs = new TbAgreements().AddFilter(t => t.flAgreementId, agreementId);
            return agrs.SelectScalar(t => t.flAuctionId, queryExecuter, transaction).Value;
        }
        public static string GetAgreementType(int agreementId, IQueryExecuter queryExecuter)
        {
            var agrs = new TbAgreements().AddFilter(t => t.flAgreementId, agreementId);
            return agrs.SelectScalar(t => t.flAgreementType, queryExecuter);
        }
        public static string GetAgreementStatus(int agreementId, IQueryExecuter queryExecuter)
        {
            var agrs = new TbAgreements().AddFilter(t => t.flAgreementId, agreementId);
            return agrs.SelectScalar(t => t.flAgreementStatus, queryExecuter);
        }
        public static bool AgreementHasSign(int agreementId, AgreementSignerRoles role, IQueryExecuter queryExecuter)
        {
            var signs = new TbAgreementSigns().AddFilter(t => t.flAgreementId, agreementId).AddFilter(t => t.flSignerRole, role);
            return signs.Count(queryExecuter) > 0;
        }
        public static DateTime GetAgreementStatusDateTime(int agreementId, IQueryExecuter queryExecuter)
        {
            var agrs = new TbAgreementModels().AddFilter(t => t.flAgreementId, agreementId);
            agrs.OrderBy = new OrderField[] { new OrderField(agrs.flRequestDate, OrderType.Desc) };
            return agrs.SelectScalar(t => t.flRequestDate, queryExecuter).Value;
        }
        public static string GetAgreementStatusByNumber(string agreementNumber, IQueryExecuter queryExecuter)
        {
            var agrs = new TbAgreements().AddFilter(t => t.flAgreementNumber, agreementNumber);
            return agrs.SelectScalar(t => t.flAgreementStatus, queryExecuter);
        }
        public static DefaultAgrTemplate[] GetAgreementModels(ActionEnv<DefaultAgrTemplateArgs> env)
        {
            if (!string.IsNullOrEmpty(env.Args.AgreementType) || env.Args.AgreementId > 0)
            {
                if (env.Args.AgreementId > 0)
                {
                    env.Args.AgreementType = GetAgreementType(env.Args.AgreementId, env.QueryExecuter);
                }

                return GetAgreementTypeModel(env.Args.AgreementType).SetModels(env);
            }
            throw new NotImplementedException($"Null AgreementType or AgreementId arg value");
        }
        public static void OnSignEnd(this IEnumerable<DefaultAgrTemplate> agrs, ActionEnv<DefaultAgrTemplateArgs> env, ITransaction transaction)
        {
            agrs.First().OnSignEnd(env, transaction);
        }
        public static DefaultAgrTemplate GetEmptyAgreementModel(ActionEnv<DefaultAgrTemplateArgs> env)
        {
            if (!string.IsNullOrEmpty(env.Args.AgreementType) || env.Args.AgreementId != null)
            {
                if (env.Args.AgreementId > 0)
                {
                    env.Args.AgreementType = GetAgreementType(env.Args.AgreementId, env.QueryExecuter);
                }

                return GetAgreementTypeModel(env.Args.AgreementType);
            }
            throw new NotImplementedException($"Null AgreementType or AgreementId arg value");
        }
        public static DefaultAgrTemplate GetAgreementTypeModel(string AgreementType)
        {
            switch (AgreementType)
            {
                case nameof(ДоговорНаВедениеОхотничьегоХозяйства):
                    {
                        return new ДоговорНаВедениеОхотничьегоХозяйства();
                    }
                case nameof(ДоговорНаВедениеРыбногоХозяйстваОтрхИСрх):
                    {
                        return new ДоговорНаВедениеРыбногоХозяйстваОтрхИСрх();
                    }
                case nameof(ДоговорНаВедениеРыбногоХозяйстваПрхИЛрх):
                    {
                        return new ДоговорНаВедениеРыбногоХозяйстваПрхИЛрх();
                    }
                case nameof(ДоговорКпЗемельногоУчастка):
                    {
                        return new ДоговорКпЗемельногоУчастка();
                    }
                case nameof(ДоговорКпПраваАрендыЗемельногоУчастка):
                    {
                        return new ДоговорКпПраваАрендыЗемельногоУчастка();
                    }
                default:
                    {
                        throw new NotImplementedException($"Unknown agreement type: {AgreementType}");
                    }
            }
        }
        public static Panel GetPanelHtml(this IEnumerable<DefaultAgrTemplate> agrs)
        {
            var panel = new Panel();
            var row = new GridRow();
            panel.Elements.Add(row);

            agrs.Each(aggr => {
                var col = new GridCol("col");
                col.Elements.Add(aggr.GetPanelHtml());
                row.Elements.Add(col);
            });

            return panel;
        }
        public static Panel GetAgreementHtmlContent(int AgreementId, IQueryExecuter queryExecuter)
        {
            if (AgreementId == 0)
                throw new NotImplementedException("AgreementId args null");

            var revId = GetAgreementActiveRevisionId(AgreementId, queryExecuter);

            var acc = new Panel();

            acc.AddComponent(new HtmlText(new TbAgreementModels().AddFilter(t => t.flAgreementRevisionId, revId).SelectScalar(t => t.flContent, queryExecuter)));

            acc.AddComponent(new UiPackages(GetAgreementTypeModel(new TbAgreements().AddFilter(t => t.flAgreementRevisionId, revId).SelectScalar(t => t.flAgreementType, queryExecuter)).GetUIPackages()));
            return acc;
        }
        public static string GetPdfContent(this IEnumerable<DefaultAgrTemplate> agrs)
        {
            var content = "";

            agrs.Each(aggr => {
                var lang = new Panel("page-break");
                lang.Elements.Add(new HtmlText(aggr.GetContent()));
                content += lang.ToHtmlString(null).Value;
            });

            return content;
        }
        public static string GetPdfContentWithSigns(this IEnumerable<DefaultAgrTemplate> agrs, int agreementId, DateTime signDate, IYodaRequestContext requestContext, IQueryExecuter queryExecuter, ITransaction transaction)
        {
            var sellerSignInfo = GetSignInfo(agreementId, AgreementSignerRoles.Seller, queryExecuter, transaction);
            var winnerSignInfo = GetSignInfo(agreementId, AgreementSignerRoles.Winner, queryExecuter, transaction);
            var sellerSignText = GetSignInfoString(sellerSignInfo);
            var winnerSignText = GetSignInfoString(winnerSignInfo);

            var agrRow = new TbAgreements().Self(out var tbAgrs).AddFilter(t => t.flAgreementId, agreementId).SelectFirst(tbAgrs.Fields.ToFieldsAliases(), queryExecuter);

            var content = "";
            content += new DocumentBuilder()
                .Doc(doc =>
                {
                    var qrUrl = requestContext.GetUrlHelper().YodaAction(nameof(RegistersModule), nameof(MnuDefaultAgrCheck), new MnuDefaultAgrCheckArgs()
                    {
                        flSellerBin = tbAgrs.flSellerBin.GetRowVal(agrRow),
                        flAgreementId = agreementId,
                        MenuAction = MnuDefaultAgrCheck.Actions.Check
                    }, urlWithSchema: true);
                    var qrCodeImageBase64 = Convert.ToBase64String(QrGenerator.GenerateQr(qrUrl));
                    var linkUrl = requestContext.GetUrlHelper().YodaAction(nameof(RegistersModule), nameof(MnuDefaultAgrCheck), null, urlWithSchema: true);
                    doc.AddSection(s => s.Body(b =>
                    {
                        b.Html($@"
<div class=""head-container"">
	<table class=""head-table"">
		<tr>
			<td colspan=""2"">
				<div class=""bg-dark text-light font-weight-bold p-1 rounded font-14"">Оператор системы: АО ""Информационно-учетный центр"" | +7 (7172) 55-29-81 | iac@gosreestr.kz | www.gosreestr.kz</div>
			</td>
		</tr>
		<tr>
			<td>
				<div class=""head-data bg-light rounded"">
					<div class=""head-data-qr-container"">
						<div class=""head-data-qr rounded"">
							<img src='data:image/bmp;base64,{qrCodeImageBase64}'/>
						</div>
					</div>
					<div class=""head-data-text"">
						<div class=""head-text"">
						    <span class=""head-header"">ОРГАНИЗАЦИЯ И ПРОВЕДЕНИЕ<br>ТОРГОВ ПРИРОДНЫМИ И НАЦИОНАЛЬНЫМИ<br>РЕСУРСАМИ</span>
					    </div>
						<span class=""head-data-qr-link"">Проверить документ можно по ссылке:</span>
						<span class=""head-data-qr-link""><a href=""{linkUrl}"">{linkUrl}</a></span>
					</div>

				</div>
			</td>
		</tr>
	</table>
</div>
")
                        .BrTag()
                       .Table(new[] { new DocTableCol("attr", null, 30, cellCssClass: "align-top border text-nowrap", headerCellCssClass: "d-none"), new DocTableCol("value", null, 0, cellCssClass: "align-top border", headerCellCssClass: "d-none") }, new[] {
                        new { attr = "Номер документа", value = tbAgrs.flAgreementNumber.GetRowVal(agrRow) },
                        new { attr = "Id договора", value = agreementId.ToString() },
                        new { attr = "Статус", value = "Подписан" },
                        new { attr = "Дата создания", value = tbAgrs.flAgreementCreateDate.GetRowVal(agrRow).ToString("dd.MM.yyyy HH:mm") },
                        new { attr = "Дата подписания", value = signDate.ToString("dd.MM.yyyy HH:mm") },
                        new { attr = "Подпись организатора (продавца)", value = sellerSignText },
                        new { attr = "Подпись победителя", value = winnerSignText },
                       }, tableCssClass: "wide-table table-td-p-10");
                    })
                    );
                }).Build();

            content += new DocumentBuilder()
                .Doc(doc =>
                {
                    doc.AddSection(s => s.Body(b =>
                    {
                        agrs.Each(aggr =>
                     {
                         var lang = new Panel("page-break");
                         lang.Elements.Add(new HtmlText(aggr.GetContent()));
                         b.Html(lang.ToHtmlString(null).Value);
                     });
                    })
                 );
                }).Build();

            content += new DocumentBuilder()
                .Doc(doc =>
                {
                    doc.AddSection(s => s.Body(b =>
                    {

                        var signs = new Panel("app-content page-break");
                        signs.Elements.Add(new HtmlText($@"
<p><b>Подпись организатора (продавца): </b></p><p>{sellerSignText}</p><br/>
<p><b>Подпись победителя: </b></p><p>{winnerSignText}</p>
"));
                        b.Html(signs.ToHtmlString(null).Value);

                    })
                 );
                }).Build();

            return content;
        }
        public class UserSignInfo {
            public UserSignInfo(string certData, DateTime signDate)
            {
                CertData = certData;
                SignDate = signDate;
            }
            public string CertData { get; set; }
            public DateTime SignDate { get; set; }

        }
        public static UserSignInfo GetSignInfo(int agreementId, AgreementSignerRoles signerRole, IQueryExecuter queryExecuter, ITransaction transaction)
        {
            var tbSigns = new TbAgreementSigns()
                .AddFilter(t => t.flAgreementId, agreementId)
                .AddFilter(t => t.flSignerRole, signerRole.ToString());
            var signInfo = tbSigns.SelectFirst(t => new FieldAlias[] { tbSigns.GetCertInfoField(), tbSigns.GetSignDateField() }, queryExecuter, transaction);
            return new UserSignInfo(tbSigns.GetCertInfoField().GetRowVal(signInfo.FirstRow), tbSigns.GetSignDateField().GetRowVal(signInfo.FirstRow));
        }
        public static string GetSignInfoString(UserSignInfo signInfo)
        {
            string signatureInfo = "Дата и время подписи: " + signInfo.SignDate.ToString("dd.MM.yyyy HH:mm") + "; ";
            var certParts = CertificateInfoExtractor.GetCertificateInfo(Convert.FromBase64String(signInfo.CertData));
            signatureInfo = certParts.Except(certParts.Where(x => (x.Code.In("SERIALNUMBER", "E")))).Aggregate(signatureInfo, (current, part) => current + (part.Title + ": " + part.Value + "; "));
            return signatureInfo;
        }
        public static Panel RenderInputs(this IEnumerable<DefaultAgrTemplate> agrs)
        {
            var panel = new Panel();

            agrs.Each(aggr => {
                panel.Elements.Add(aggr.RenderInputs());
            });

            return panel;
        }
        public static void ValidateInputs(this IEnumerable<DefaultAgrTemplate> agrs, NameValueCollection FormCollection, out Dictionary<string, string> outErrors)
        {
            var errors = new Dictionary<string, string>();
            agrs.Each(x => {
                x.ValidateInputs(FormCollection, out var langErrors);
                langErrors.Each(y => errors.Add(y.Key, y.Value));
            });
            outErrors = errors;
        }
        public static void SetInputsValues(this IEnumerable<DefaultAgrTemplate> agrs, NameValueCollection FormCollection)
        {
            agrs.Each(x => x.SetInputsValues(FormCollection));
        }
        public static IEnumerable<DefaultAgrTemplate> SetAgreementNumber(this IEnumerable<DefaultAgrTemplate> agrs, int agreementId)
        {
            agrs.Each(aggr => {
                aggr.SetAgreementNumber(agreementId);
            });
            return agrs;
        }
        public static string GetAgreementNumber(this IEnumerable<DefaultAgrTemplate> agrs)
        {
            return agrs.First().DocNumber;
        }
        public static AgreementsModel GetAgreementsModelFirstOrDefault(this TbAgreements tbAgreements, IQueryExecuter queryExecuter, ITransaction transaction = null)
        {
            var r = tbAgreements.SelectFirstOrDefault(t => t.Fields.ToFieldsAliases(), queryExecuter, transaction);

            if (!r.IsFirstRowExists)
            {
                return null;
            }

            return new AgreementsModel
            {
                flAgreementId = r.GetVal(t => t.flAgreementId),
                flAgreementNumber = r.GetVal(t => t.flAgreementNumber),
                flAgreementRevisionId = r.GetVal(t => t.flAgreementRevisionId),
                flAgreementType = r.GetVal(t => t.flAgreementType),
                flAgreementStatus = r.GetVal(t => t.flAgreementStatus),
                flAgreementCreateDate = r.GetVal(t => t.flAgreementCreateDate),
                flAgreementSignDate = r.GetValOrNull(t => t.flAgreementSignDate),
                flObjectId = r.GetVal(t => t.flObjectId),
                flObjectType = r.GetVal(t => t.flObjectType),
                flTradeId = r.GetValOrNull(t => t.flTradeId),
                flTradeType = r.GetVal(t => t.flTradeType),
                flAuctionId = r.GetVal(t => t.flAuctionId),
                flAgreementCreatorBin = r.GetVal(t => t.flAgreementCreatorBin),
                flSellerBin = r.GetVal(t => t.flSellerBin),
                flWinnerXin = r.GetVal(t => t.flWinnerXin)
            };
        }

        public static AgreementsModel GetAgreementsModel(int agreementId, IQueryExecuter queryExecuter)
        {
            return new TbAgreements()
                .AddFilter(t => t.flAgreementId, agreementId)
                .GetAgreementsModelFirstOrDefault(queryExecuter, null);
        }
    }
}
