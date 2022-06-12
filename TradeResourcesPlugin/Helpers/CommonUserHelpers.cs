using CommonSource.Models;
using System.Text;
using UsersResources;
using Yoda.Interfaces;
using Yoda.Interfaces.Helpers;
using YodaHelpers;

namespace TradeResourcesPlugin.Helpers {
    public static class CommonUserHelpers {

        public static ApplicantAdrData GetCurrentUserAdrData(this IYodaRequestContext requestContext) {
            return GetCurrentUserAdrData(requestContext.User, requestContext);
        }

        public static ApplicantAdrData GetCurrentUserAdrData(IUser user, IYodaRequestContext context) {
            var accountId = user.GetAccountId(context.QueryExecuter);
            var userId = user.Id;
            //var phone = YodaUserHelpers.GetUserPhone(accountId, context.QueryExecuter);
            var mobilePhone = YodaUserHelpers.GetUserMobilePhone(userId, context.QueryExecuter);
            var sbPhoneNums = new StringBuilder();
            //if (!string.IsNullOrWhiteSpace(phone)) {
            //    sbPhoneNums.AppendHtml("тел.: {0}; ", phone);
            //}
            if (!string.IsNullOrWhiteSpace(mobilePhone)) {
                sbPhoneNums.AppendHtml("мобильный телефон: {0}; ", mobilePhone);
            }
            return new ApplicantAdrData {
                Adr = YodaUserHelpers.GetAddress(accountId, context.QueryExecuter),
                Phone = sbPhoneNums.ToString().Trim()
            };
        }


        public static FizAndEnterpreneurDataExt GetFizAndEnterpreneurData(this IYodaRequestContext context) {
            return GetFizAndEnterpreneurData(context.User, context);
        }

        public static FizAndEnterpreneurDataExt GetFizAndEnterpreneurData(this IUser user, IYodaRequestContext context) {
            //var tbUsers = new TbUsersExternalProfileData();
            //var r = tbUsers.AddFilter(t => t.flUserId, userId)
            //    .SelectFirst(t => new FieldAlias[] { t.flDocType, t.flDocNum, t.flDocDate, t.flDocGivenBy, t.flUserType }, context.QueryExecuter);

            //var ret = new FizAndEnterpreneurDataExt {
            //    UserName = YodaUserHelpers.GetUserFullName(userId, context.QueryExecuter),
            //    IdentityDoc = new IdentiyDoc {
            //        Number = r.Query.flDocNum.GetVal(r.FirstRow),
            //        DocDate = r.Query.flDocDate.GetVal(r.FirstRow),
            //        DocName = r.Query.flDocType.GetDisplayText(r.Query.flDocType.GetVal(r.FirstRow), context),
            //        IssuerOrg = r.Query.flDocGivenBy.GetVal(r.FirstRow)
            //    }
            //};

            //if (tbUsers.flUserType.GetRowVal(r.FirstRow) == RefUserType.Values.IndividualCorp) {
            //    var corpName = new TbUsersExternalProfileDisplayNames()
            //        .AddFilter(t => t.flUserId, userId)
            //        .SelectScalar(t => t.flCorpName, context.QueryExecuter);
            //    ret.EnterpreneurCorpName = new EnterpreneurName { CorpName = corpName };
            //}

            var accountData = user.GetAccountData(context.QueryExecuter);

            var ret = new FizAndEnterpreneurDataExt {
                UserName = YodaUserHelpers.GetUserFullName(user.Id, context.QueryExecuter),
                //IdentityDoc = new IdentiyDoc
                //{
                //    Number = accountData.AccountAddresses,
                //    DocDate = r.Query.flDocDate.GetVal(r.FirstRow),
                //    DocName = r.Query.flDocType.GetDisplayText(r.Query.flDocType.GetVal(r.FirstRow), context),
                //    IssuerOrg = r.Query.flDocGivenBy.GetVal(r.FirstRow)
                //}
            };

            if (accountData.CorpData != null) {
                ret.EnterpreneurCorpName = new EnterpreneurName { CorpName = YodaUserHelpers.GetUserOrgName(user.Id, context.QueryExecuter) };
            }

            return ret;
        }

    }
    public class ApplicantAdrData {
        public string Adr { get; set; }
        public string Phone { get; set; }
    }
    public class JurData {
        public string FirstPerson { get; set; }
        public string CorpName { get; set; }
    }
    public class FizAndEnterpreneurDataExt {
        public EnterpreneurName EnterpreneurCorpName { get; set; }
        public string UserName { get; set; }
        public IdentiyDoc IdentityDoc { get; set; }
    }
    public class EnterpreneurName {
        public string CorpName { get; set; }
    }
}
