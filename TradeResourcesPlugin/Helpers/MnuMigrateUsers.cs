//using AuthorizeRegistrationPlugin.Menus;
//using Microsoft.Extensions.DependencyInjection;
//using PostgreSQLQueryExecuterEngine;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using UsersResources;
//using UsersResources.QueryTables;
//using Yoda.Application;
//using Yoda.Application.Queries;
//using Yoda.Interfaces;
//using Yoda.Interfaces.Jobs;
//using Yoda.Interfaces.Menu;
//using YodaApp.UsersResources.QueryTables;
//using YodaApp.UsersResources.QueryTables.V2;
//using YodaApp.Yoda.Application.UserHelpers;
//using YodaCorePlugin.AuthorizeRegistration.Menus;
//using YodaQuery;

//namespace TradeResourcesPlugin.Helpers {
//    public class MnuMigrateUsers : FrmMenu {

//        public MnuMigrateUsers(string moduleName) : base(nameof(MnuMigrateUsers), "Миграция пользователей")
//        {
//            MenuType(Yoda.Interfaces.Menu.MenuType.Normal);
//            Access();
//            Enabled((rc) => {
//                return true;
//            });
//            OnRendering(re => {

//                //"accountAndUserChangeHookUrls": [
//                //    "https://wallet.gosreestr.kz/acc-user-change-web-hook",
//                //    "https://rent-schools.gosreestr.kz/acc-user-change-web-hook",
//                //    "https://traderesources.gosreestr.kz/acc-user-change-web-hook"
//                //]


//                //"dbKeys": {
//                //    "dbTradeResources": { "schema": "trades" },
//                //    "dbYodaSystem": { "schema": "yoda_system" },
//                //    "dbYoda": { "schema": "yoda_system" },
//                //    "dbYodaUsers": { "schema": "users" },
//                //    "dbJobs": { "schema": "jobs" },
//                //    "dbOldUsers": {
//                //                    "dbConnection": "dbOldUsers",
//                //    "schema": "users"
//                //    }
//                //}


//            var userCreationHelper = ActivatorUtilities
//                            .CreateInstance<UserCreationHelper>(re.RequestContext.AppEnv.ServiceProvider);

//                var oldUsers = new TbUsers() { DbKey = "dbOldUsers" }.Self(out var tbOldUserss).Select(new FieldAlias[] { tbOldUserss.flLogin }, re.QueryExecuter).AsEnumerable().Select(r => tbOldUserss.flLogin.GetRowVal(r));


//                var newUsers = new TbUsers().Self(out var tbNewUserss).Select(new FieldAlias[] { tbNewUserss.flLogin }, re.QueryExecuter).AsEnumerable().Select(r => tbNewUserss.flLogin.GetRowVal(r));

//                oldUsers = oldUsers.Where(login => !newUsers.Contains(login));

//                var count = 0;

//                oldUsers.Each(login => {

//                    var oldUser = new TbUsers() { DbKey = "dbOldUsers" }.Self(out var tbOldUsers).AddFilter(t => t.flLogin, login).SelectFirst(tbOldUsers.Fields.ToAliases(), re.QueryExecuter);
//                    var oldAccount = new TbAccounts() { DbKey = "dbOldUsers" }.Self(out var tbOldAccounts).AddFilter(t => t.flAccountId, tbOldAccounts.flAccountId.GetRowVal(oldUser)).SelectFirst(tbOldAccounts.Fields.ToAliases(), re.QueryExecuter);

//                    var userRegData = new UserRegData()
//                    {
//                        AccountType = (AccountType)Enum.Parse(typeof(AccountType), tbOldUsers.flAccountType.GetRowVal(oldUser)),
//                        CertificateData = new Yoda.Interfaces.Forms.Components.CertificateData()
//                        {
//                            Iin = tbOldUsers.flIin.GetRowVal(oldUser),
//                            Bin = tbOldUsers.flXin.GetRowVal(oldUser) == tbOldUsers.flIin.GetRowVal(oldUser) ? null : tbOldUsers.flXin.GetRowVal(oldUser),
//                            LastName = tbOldUsers.flLastName.GetRowVal(oldUser),
//                            FirstName = tbOldUsers.flFirstName.GetRowVal(oldUser),
//                            MiddleName = tbOldUsers.flMiddleName.GetRowVal(oldUser),
//                            CertOwnerCorpName = oldAccount == null ? null : tbOldAccounts.flNameRu.GetRowVal(oldAccount)
//                        },
//                        Email = tbOldUsers.flEmail.GetRowVal(oldUser),
//                        IndividualCorpName = oldAccount == null ? null : tbOldAccounts.flNameRu.GetRowVal(oldAccount),
//                        MobilePhone = tbOldUsers.flMobilePhone.GetRowVal(oldUser),
//                        TermsAndConditions = new YodaCorePlugin.AuthorizeRegistration.Helpers.TermsAndConditionsHelper.TermsAndConditions(0, "")
//                    };

//                    var passwordHash = tbOldUsers.flPassword.GetRowVal(oldUser);

//                    userCreationHelper.TryAddUser(userRegData, passwordHash, out var error, re.QueryExecuter);

//                    count++;

//                    Console.WriteLine($"{count}/{oldUsers.Count()}");

//                });


//            });
//        }

//    }


//    public class UserCreationHelper {
//        private readonly IAccountManagerProvider _accountManagerProvider;
//        private readonly IUserManagerProvider _userManagerProvider;
//        public UserCreationHelper(
//            IAccountManagerProvider accountManagerProvider,
//            IUserManagerProvider userManagerProvider)
//        {
//            _accountManagerProvider = accountManagerProvider;
//            _userManagerProvider = userManagerProvider;
//        }

//        public bool TryAddUser(UserRegData userRegData, string passwordHash, out string error, IQueryExecuter queryExecuter)
//        {
//            var certData = userRegData.CertificateData;
//            var login = getUserLogin(userRegData.AccountType.Value, certData.Bin, certData.Iin);

//            var signDate = queryExecuter.GetDateTime(DbUsers.DbKey);

//            string fullName = new[] {
//                            certData.LastName,
//                            certData.FirstName,
//                            certData.MiddleName
//                        }.Where(x => !string.IsNullOrWhiteSpace(x)).JoinStr(" ");


//            if (_userManagerProvider.TryGet(login, out var userManager, queryExecuter))
//            {

//                //var iinRelatedJobExists = new TbUserRelatedJobs()
//                //.AddFilter(t => t.flIin, userRegData.CertificateData.Iin)
//                //.Count(queryExecuter) > 0;

//                //if (iinRelatedJobExists)
//                //{
//                //    var jobId = new TbUserRelatedJobs()
//                //        .AddFilter(t => t.flIin, userRegData.CertificateData.Iin)
//                //        .SelectScalar(t => t.flGbdFlSyncJobId, queryExecuter);

//                //    UserRelatedJobs.SyncUserWithGbdFl.GetJobsTable()
//                //        .AddFilter(t => t.flJobId, jobId)
//                //        .Update()
//                //        .Set(t => t.flStartAfter, new CurDateSqlExp())
//                //        .Execute(queryExecuter);
//                //}
//                //else
//                //{

//                //    var syncGbdFlJobId = UserRelatedJobs.SyncUserWithGbdFl.Add(new IinModel(userRegData.CertificateData.Iin), out var addSyncGbdFlJob);

//                //    addSyncGbdFlJob.Execute(queryExecuter);
//                //    new TbUserRelatedJobs()
//                //        .Insert()
//                //        .SetT(t => t.flIin, userRegData.CertificateData.Iin)
//                //        .SetT(t => t.flGbdFlSyncJobId, syncGbdFlJobId.Id)
//                //        .Execute(queryExecuter);
//                //}

//                error = "Вы уже прошли процедуру регистрации в системе ранее. Пожалуйста, войдите в систему на странице входа.";
//                return false;
//            }

//            var oldInviteData = new TbUserInvites() { DbKey = "dbOldUsers" }
//                        .Self(out var oldTbUserInvites)
//                        .AddFilter(x => x.flIin, userRegData.CertificateData.Iin)
//                        .SelectFirstOrDefault(oldTbUserInvites.Fields.ToAliases(), queryExecuter);

//            if (oldInviteData != null)
//            {
//                userRegData.InviteCode = oldTbUserInvites.flInviteCode.GetRowVal(oldInviteData);

//                var newInviteData = new TbUserInvites()
//                        .AddFilter(x => x.flIin, userRegData.CertificateData.Iin)
//                        .Count(queryExecuter) == 0;

//                if (newInviteData)
//                {
//                    new TbUserInvites()
//                    .Insert()
//                        .SetT(t => t.flIin, userRegData.CertificateData.Iin)
//                        .SetT(t => t.flEmail, userRegData.Email)
//                        .SetT(t => t.flGroupName, oldTbUserInvites.flGroupName.GetRowVal(oldInviteData))
//                        .SetT(t => t.flAutoBlockAfter, oldTbUserInvites.flAutoBlockAfter.GetRowVal(oldInviteData))
//                        .SetT(t => t.flInviteExpiresAt, oldTbUserInvites.flInviteExpiresAt.GetRowVal(oldInviteData))
//                        .SetT(t => t.flInviteAccountId, oldTbUserInvites.flInviteAccountId.GetRowVal(oldInviteData))
//                        .SetT(t => t.flInviteCode, userRegData.InviteCode)
//                        .SetT(t => t.flUsed, false)
//                    .Execute(queryExecuter);
//                }
//            }


//            var (accountId, groupName) = userRegData.AccountType switch
//            {
//                AccountType.Corporate => (certData.Bin, $"БИН:{certData.Bin}"),
//                AccountType.Individual => (login, "ФЛ"),
//                AccountType.IndividualCorp => (login, "ИП"),
//                AccountType.Internal => (login, getInternalGroupName(login, userRegData.InviteCode, queryExecuter)),
//                _ => throw new NotImplementedException()
//            };


//            var autoAssignRoles = getOldProjectRoles(login, userRegData.AccountType, queryExecuter);

//            var batchQuery = new BatchQuery();

//            var newAccountCreated = false;
//            if (!_accountManagerProvider.TryGet(accountId, out var accountManager, queryExecuter))
//            {
//                var (xin, accountName) = userRegData.AccountType switch
//                {
//                    AccountType.Corporate => (certData.Bin, certData.CertOwnerCorpName),
//                    AccountType.Individual => (certData.Iin, fullName),
//                    AccountType.IndividualCorp => (certData.Iin, userRegData.IndividualCorpName),
//                    AccountType.Internal => (certData.Iin, fullName),
//                    _ => throw new NotImplementedException()
//                };

//                UserLogin[] accountAdminLogins;
//                // У Юр.лица первичный администратор это руководитель, мы его позже через синхронизацию с ЮЛ заполним
//                // а у остальных (ФЛ, ИП, Сотрудник) кто сейчас регистрируется тот и есть администратор своего аккаунта
//                if (userRegData.AccountType == AccountType.Corporate)
//                {
//                    accountAdminLogins = new UserLogin[] { };
//                }
//                else
//                {
//                    accountAdminLogins = new UserLogin[] { new UserLogin(login) };
//                }

//                accountManager = _accountManagerProvider.Create(
//                    new AccountData(
//                        new Account(
//                           accountId, userRegData.AccountType.Value, xin, accountName, accountName, false, null, null
//                        ),
//                        new AccountAddr[0],
//                        userRegData.AccountType == AccountType.Corporate
//                            ? new CorpData(
//                                certData.Bin,
//                                firstPersonFio: null,
//                                firstPersonIin: null,
//                                userRegData.Email,
//                                userRegData.MobilePhone,
//                                corpStationaryPhone: null
//                              )
//                            : null,
//                        bankAccount: null,
//                        accountAdminLogins
//                    ),
//                    out var insertQueries
//                );
//                newAccountCreated = true;
//                batchQuery.AddRange(insertQueries);
//            }


//            var userId = new TbUsers().flUserId.GetNextId(queryExecuter);
//            userManager = _userManagerProvider.Create(
//                new YodaApp.Yoda.Application.UserHelpers.UserData(
//                    accountId,
//                    accountManager.AccountData.Account.AccountType,
//                    accountManager.AccountData.Account.Xin,
//                    userId,
//                    new User(
//                        login,
//                        certData.Iin,
//                        groupName,
//                        certData.FirstName,
//                        certData.LastName,
//                        certData.MiddleName,
//                        fullName,
//                        userRegData.Email,
//                        userRegData.MobilePhone,
//                        false,
//                        null,
//                        false,
//                        null,
//                        userRegData.AccountType == AccountType.Internal
//                            ? getInternalAutoBlockAfterDate(certData.Iin, userRegData.InviteCode, queryExecuter)
//                            : (DateTime?)null,
//                        queryExecuter.GetDateTime()
//                    ),
//                    new UserDoc[] { },
//                    autoAssignRoles
//                        .Select(x => x.Id)
//                        .ToArray()
//                ),
//                passwordHash,
//                out var addUserQueries,
//                new UserRegistrationContext(
//                    newAccountCreated,
//                    new ServiceAgreementInfo(
//                        userRegData.TermsAndConditions.Version.ToString(),
//                        signDate
//                    )
//                ),
//                userRegData.InviteCode
//            );
//            batchQuery.AddRange(addUserQueries);

//            //Т.к. Проект будто-бы новый, пользователи должны будут заново подписать соглашение

//            //var userServiceAgreementManager = _userServiceAggrementsManagerProvider
//            //    .Create(login, userId);

//            //userServiceAgreementManager.AddServiceAgreement(
//            //    new ServiceAgreement(
//            //        userRegData.TermsAndConditions.Version,
//            //        signDate,
//            //        userRegData.TermsAndConditions.TextInBase64,
//            //        new Sign(
//            //            certData.CertData,
//            //            certData.SignedData
//            //        )
//            //    ),
//            //    out var addServiceAgreementQueries
//            //);

//            //batchQuery.AddRange(addServiceAgreementQueries);
            
            
            
//            batchQuery.Execute(queryExecuter);

//            error = null;
//            return true;
//        }

//        internal static DateTime getInternalAutoBlockAfterDate(string iin, string inviteCode, IQueryExecuter queryExecuter)
//        {
//            return new TbUserInvites()
//               .AddFilter(x => x.flIin, iin)
//               .AddFilter(x => x.flInviteCode, inviteCode)
//               .SelectFirst(x => new FieldAlias[] { x.flAutoBlockAfter }, queryExecuter)
//               .GetVal(x => x.flAutoBlockAfter);
//        }

//        internal static string getInternalGroupName(string login, string inviteCode, IQueryExecuter queryExecuter)
//        {
//            return new TbUserInvites()
//                .AddFilter(x => x.flIin, login)
//                .AddFilter(x => x.flInviteCode, inviteCode)
//                .SelectFirst(x => new FieldAlias[] { x.flGroupName }, queryExecuter)
//                .GetVal(x => x.flGroupName);
//        }
//        internal static List<RoleInfo> getOldProjectRoles(string login, AccountType? accountType, IQueryExecuter queryExecuter)
//        {
//            var result = new List<RoleInfo>();

//            var newRolesDict = new TbRoles()
//                .Self(out var newTbRoles)
//                .Select(new FieldAlias[] { newTbRoles.flRoleName, newTbRoles.flRoleId }, queryExecuter)
//                .AsEnumerable()
//                .ToDictionary(r => newTbRoles.flRoleName.GetRowVal(r), r => newTbRoles.flRoleId.GetRowVal(r), StringComparer.OrdinalIgnoreCase);

//            var oldUserRoles = new TbRoles() { DbKey = "dbOldUsers" }
//                .Self(out var oldTbRoles)
//                .Join("oldTbRoles", new TbUserRoles() { DbKey = "dbOldUsers" }.Self(out var oldTbUserRoles), "oldTbUserRoles")
//                .On(new Condition(oldTbRoles.flRoleId, oldTbUserRoles.flRoleId))
//                .Join("oldTbRoles", new TbUsers() { DbKey = "dbOldUsers" }.Self(out var oldTbUsers).AddFilter(t => t.flLogin, login), "oldTbUsers")
//                .On(new Condition(oldTbUsers.flUserId, oldTbUserRoles.flUserId))
//                .Select(new FieldAlias[] { oldTbRoles.flRoleName }, queryExecuter)
//                .AsEnumerable()
//                .Select(r => oldTbRoles.flRoleName.GetRowVal(r));

//            oldUserRoles.Each(x => { 
//                if (newRolesDict.ContainsKey(x))
//                {
//                    result.Add(new RoleInfo(x, newRolesDict[x], false, new RoleProfile()));
//                }
//            });


//            var userProfile = accountType == AccountType.Internal
//                ? RoleProfile.Internal
//                : RoleProfile.External;

//            new TbRoles()
//                .Select(t => new FieldAlias[] { t.flRoleName, t.flRoleId, t.flAutoAssigned, t.flUserProfile }, queryExecuter)
//                .Select(r => new RoleInfo(
//                    r.GetVal(t => t.flRoleName),
//                    r.GetVal(t => t.flRoleId),
//                    r.GetVal(t => t.flAutoAssigned),
//                    r.GetVal(t => t.flUserProfile) switch
//                    {
//                        "external" => RoleProfile.External,
//                        "internal" => RoleProfile.Internal,
//                        _ => throw new NotImplementedException()
//                    }
//                ))
//                .Where(x => x.RoleProfile == userProfile && x.IsAutoAssignRole)
//                .Each(x => {
//                    if (!result.Any(t => t.Id == x.Id))
//                    {
//                        result.Add(x);
//                    }
//                });


//            return result;

//        }

//        internal static string getUserLogin(AccountType accountType, string bin, string iin)
//        {
//            switch (accountType)
//            {
//                case AccountType.Individual:
//                    if (!string.IsNullOrWhiteSpace(bin))
//                    {
//                        throw new Exception("БИН должен быть пустым при профиле ИП/Физ.лица");
//                    }
//                    return "ФЛ-" + iin;

//                case AccountType.IndividualCorp:
//                    if (!string.IsNullOrWhiteSpace(bin))
//                    {
//                        throw new Exception("БИН должен быть пустым при профиле ИП/Физ.лица");
//                    }
//                    return "ИП-" + iin;

//                case AccountType.Corporate:
//                    if (string.IsNullOrWhiteSpace(bin))
//                    {
//                        throw new Exception("БИН не может быть пустым при корпоративном профиле");
//                    }
//                    return string.Format("{0}-{1}", bin, iin);


//                case AccountType.Internal:
//                    return iin;

//                default:
//                    throw new NotImplementedException("AccountType: " + accountType);
//            }
//        }


//    }

//}

