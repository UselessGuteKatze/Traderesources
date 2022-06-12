using HydrocarbonSource.QueryTables.Application;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using UsersResources;
using Yoda.Interfaces;
using Yoda.Interfaces.Forms.Components;
using Yoda.Interfaces.Helpers;
using YodaHelpers;
using YodaHelpers.ActionMenus;
using YodaHelpers.HtmlDocumentBuilder;
using YodaQuery;

namespace TradeResourcesPlugin.Helpers {
    public static class CertHelper {
        public static string[] GetSignsDataHtml<T>(this TbAppSignBase<T> tbl, int appId, IYodaRequestContext context, Func<T, string> whoseSignature = null, Func<T, string> getWhenSignedLabel = null) where T : struct, IConvertible {
            var result = new List<string>();
            var signs = tbl
                .AddFilter(t => t.flAppId, appId)
                .Order(t => t.flSignDate)
                .Select(t => new FieldAlias[] { t.flSignDate, t.flCertInfo, t.flSignerType }, context.QueryExecuter);

            foreach (DataRow r in signs.DataTable.Rows) {
                var certInfo = signs.Query.flCertInfo.GetVal(r);
                var signDate = signs.Query.flSignDate.GetVal(r);
                var signerType = signs.Query.flSignerType.GetVal(r);
                var whoseSignatureText = $@"{signs.Query.flSignerType.GetDisplayText(signerType.ToString(), context).ToHtml()}";
                if (whoseSignature != null) {
                    whoseSignatureText = whoseSignature(signerType);
                }
                var whenSignedText = "Подписано в";
                if (getWhenSignedLabel != null) {
                    whenSignedText = getWhenSignedLabel(signerType);
                }
                var html = $@"<br/><div class='sign-content'>
{whoseSignatureText}:<br />
{whenSignedText} {signDate:HH:mm:ss dd.MM.yyyy} года;<br />
Данные из ЭЦП:<br />{new DocumentBuilder.SignInfo(certInfo).DisplayHtmlText}
</div>";
                result.Add(html);
            }
            return result.ToArray();
        }
        public static string[] GetSignsDataHtml(this TbSignStoreBase tbl, int id, IYodaRequestContext context, Func<string> whoseSignature = null, Func<string> getWhenSignedLabel = null) {
            var result = new List<string>();
            var signs = tbl
                .AddFilter(t => t.GetObjIdField(), id)
                .Order(t => t.GetSignDateField())
                .Select(t => new FieldAlias[] { t.GetSignDateField(), t.GetCertInfoField() }, context.QueryExecuter);

            foreach (var r in signs.DataTable.Rows.Cast<DataRow>()) {
                var certInfo = signs.Query.GetCertInfoField().GetVal(r);
                var signDate = signs.Query.GetSignDateField().GetVal(r);

                var whoseSignatureText = string.Empty;
                if (whoseSignature != null) {
                    whoseSignatureText = whoseSignature();
                }
                var whenSignedText = "Подписано в";
                if (getWhenSignedLabel != null) {
                    whenSignedText = getWhenSignedLabel() + ":<br />";
                }
                var html = $@"<br/><div class='sign-content'>
{whoseSignatureText}
{whenSignedText} {signDate:HH:mm:ss dd.MM.yyyy} года;<br />
Данные из ЭЦП:<br />{new DocumentBuilder.SignInfo(certInfo).DisplayHtmlText}
</div>";
                result.Add(html);
            }
            return result.ToArray();
        }



        public static CertificateSignBox GetSignBox(IYodaRequestContext context, string dataToSign) {
            var signAlgName = context.User.GetUserCertSignAlg(context.QueryExecuter);
            var keyUsageType = KeyUsageType.Sign;
            var bin = context.User.IsExternalUser()
                ? context.User.GetUserBin(context.QueryExecuter)
                : context.User.GetUserBin(context.QueryExecuter);
            if (bin != null && ("050540004455".EqualsIgnoreCase(bin) || (context.Configuration["MnuActions::ForceUseRsaForSigning:Bins"] + string.Empty).Contains(bin))) {
                signAlgName = "rsa";
                keyUsageType = KeyUsageType.Auth;
            }
            var dataToSignEncoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(dataToSign));
            var certSignBox = new CertificateSignBox("SignData", context, keyUsageType, signAlgName, dataToSignEncoded);
            return certSignBox;
        }


        public static bool ValidateSign(System.Collections.Specialized.NameValueCollection formCollection, IYodaRequestContext context, string dataToSign, out CertificateData certData, out string errorText) {
            var sign = GetSignBox(context, dataToSign);
            certData = sign.GetCertificateData(formCollection, true);
            if (!certData.Valid) {
                errorText = "Не удалось подписать данные: " + string.Join("; ", certData.Errors.Select(x => x.Text));
                return false;
            }
            return CertHelpers.CanCurUserSignWithChoosedCert(context, certData, out errorText);
        }
    }
}
