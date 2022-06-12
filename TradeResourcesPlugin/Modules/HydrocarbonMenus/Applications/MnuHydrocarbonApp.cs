using CommonSource.Models;
using CommonSource.References.Application;
using FileStoreInterfaces;
using CommonSource;
using HydrocarbonSource.Helpers.Object;
using HydrocarbonSource.Models;
using HydrocarbonSource.QueryTables.Application;
using HydrocarbonSource.QueryTables.Object;
using HydrocarbonSource.References.Application;
using HydrocarbonSource.References.Object;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TradeResourcesPlugin.Helpers;
using TradeResourcesPlugin.Modules.HydrocarbonMenus.Objects;
using UsersResources;
using UsersResources.QueryTables;
using UsersResources.Refs;
using Yoda.Application;
using Yoda.Interfaces;
using Yoda.Interfaces.Forms;
using Yoda.Interfaces.Forms.Components;
using Yoda.Interfaces.Helpers;
using Yoda.Interfaces.Menu;
using YodaApp.YodaHelpers.SearchCollections;
using YodaHelpers;
using YodaHelpers.ActionMenus;
using YodaHelpers.Components;
using YodaHelpers.Fields;
using YodaHelpers.HtmlDocumentBuilder;
using YodaQuery;
using OkResult = YodaHelpers.ActionMenus.OkResult;

namespace TradeResourcesPlugin.Modules.HydrocarbonMenus.Applications {
    public class MnuHydrocarbonApp : MnuActionsExt<SubsoilsApplicationArgs> {

        public static SyncDic _sync = new SyncDic();
        public const string MnuName = nameof(MnuHydrocarbonApp);
        public MnuHydrocarbonApp(string moduleName) : base(moduleName, MnuName, "Заявление на проведение аукциона") {
            AccessCheck((args, env) => {
                if (env.User.IsGuest()) {
                    return new AccessCheckResult {
                        HasAccess = false,
                        ExceptionMessage = "Доступ запрещен"
                    };
                }
                return new AccessCheckResult {
                    HasAccess = true
                };
            });
            AsCallback();
        }


        public override void Configure(ActionConfig<SubsoilsApplicationArgs> config) {
            config
            #region CreateApp
                .OnAction(Actions.Create,
                    c => c
                    .IsValid(env => {
                        return new OkResult();
                    })
                    .Wizard(wizard => wizard
                        .Args(args => args.Args)
                        .Model(env => {
                            var model = new PreApplicationModel {
                                GuidId = Guid.NewGuid().ToString("N")
                            };
                            if (env.Args.ObjectId > 0) {
                                var row = new TbObjects()
                                    .Self(out var tbObjects)
                                    .AddFilter(t => t.flId, env.Args.ObjectId)
                                    .SelectFirst(j => new FieldAlias[] { j.flId, j.flCoords, j.flName }, env.QueryExecuter);
                                model.ObjectItem = new ObjectItem(env.Args.ObjectId, row.GetVal(t => t.flName), string.Join(", ", row.GetVal(t => t.flCoords).MainRing.Select(coord => $"{coord.AppropriateX} {coord.AppropriateY}"))
                                );
                            }
                            return model;
                        })
                        .CancelBtn("Отмена", ev => new ActionRedirectData(ModuleName, MnuHydrocarbonAppsSearch.MnuName))
                        .FinishBtn("Подписать")
                    #region Info
                        .Step("Данные заявителя", step => step
                        .OnRendering(env => {
                            //env.Env.RequestContext.SyncCurrentUserData();
                            var isCorporate = env.Env.User.IsCorporateUser(env.Env.QueryExecuter);
                            var userAdrData = CommonUserHelpers.GetCurrentUserAdrData(env.Env.RequestContext);
                            var applicantInfoGroupbox = new Accordion(env.Env.T("Сведения о заявителе"));
                            env.Panel.AddComponent(applicantInfoGroupbox);

                            applicantInfoGroupbox.Append(new LinkUrl(env.Env.T("Редактировать данные пользователя"), env.Env.RequestContext.Configuration["UserEditSelfDataUrl"], openInNewWindow: true));

                            var tbApps = new TbTradePreApplication();
                            var accountData = env.Env.User.GetAccountData(env.Env.QueryExecuter).Account;
                            if (isCorporate) {
                                tbApps.flApplicantXin.RenderCustom(applicantInfoGroupbox, env.Env, env.Env.User.GetUserXin(env.Env.QueryExecuter), readOnly: true);
                                tbApps.flApplicantName.RenderCustom(applicantInfoGroupbox, env.Env, accountData.NameRu, readOnly: true);
                                tbApps.flApplicantAddress.RenderCustom(applicantInfoGroupbox, env.Env, userAdrData.Adr, readOnly: true);
                                tbApps.flApplicantPhoneNumber.RenderCustom(applicantInfoGroupbox, env.Env, userAdrData.Phone, readOnly: true);
                            }
                            else {
                                var docInfo = CommonUserHelpers.GetFizAndEnterpreneurData(env.Env.RequestContext);
                                var userInfo = env.Env.User.GetUserInfo(env.Env.QueryExecuter);
                                tbApps.flApplicantXin.RenderCustom(applicantInfoGroupbox, env.Env, env.Env.User.GetUserXin(env.Env.QueryExecuter), readOnly: true);
                                if (userInfo.UserType == UserType.IndividualCorp) {
                                    tbApps.flApplicantName.RenderCustom(applicantInfoGroupbox, env.Env, accountData.NameRu, readOnly: true);
                                }
                                tbApps.flApplicantName.RenderCustom(applicantInfoGroupbox, env.Env, docInfo.UserName, readOnly: true);
                                tbApps.flApplicantAddress.RenderCustom(applicantInfoGroupbox, env.Env, userAdrData.Adr, readOnly: true);
                                tbApps.flApplicantPhoneNumber.RenderCustom(applicantInfoGroupbox, env.Env, userAdrData.Phone, readOnly: true);
                                tbApps.flCitizenship.RenderCustom(applicantInfoGroupbox, env.Env, env.Model.ApplicantInfo?.Citizenship);
                            }
                            var bankRequisitionsGroupbox = new Accordion(env.Env.T("Сведения текущего счета заявителя в банке второго уровня"));
                            env.Panel.AddComponent(bankRequisitionsGroupbox);

                            bankRequisitionsGroupbox.Append(new LinkUrl(env.Env.T("Изменить банковские реквизиты"), env.Env.RequestContext.YodaActionUrl("ExternalRegistrationModule", "MnuUserBankDetails", new { MenuAction = "view" }), openInNewWindow: true));

                            var userBankDetails = env.Env.User.GetAccountData(env.Env.QueryExecuter).BankAccount;
                            if (userBankDetails != null) {
                                var bankName = new BankSearchCollection().GetItem(userBankDetails.Bic, env.Env.RequestContext).Name;
                                bankRequisitionsGroupbox.AddComponent(new Label($"Банковские реквизиты: {bankName}; {userBankDetails.Bic}; {userBankDetails.Iban}; {userBankDetails.Kbe}"));
                            }
                            else {
                                bankRequisitionsGroupbox.AddComponent(new Label("Банковские реквизиты не обнаружены"));
                            }
                            //var bankRequisition = env.Model.ApplicantBankRequisitions;
                            //int? bankRequisitionId;
                            //var userBankDetailsCollection = SearchCollectionsProvider.Instance.Get<UserBankDetailsSearchCollection>();
                            //if (bankRequisition != null) {
                            //    bankRequisitionId = userBankDetailsCollection.GetActiveBankDetailId(bankRequisition.Bik, bankRequisition.Iik, bankRequisition.Kbe, env.Env.RequestContext);
                            //} else {
                            //    bankRequisitionId = userBankDetailsCollection.GetUserDefaultBankDetailId(env.Env.RequestContext);
                            //}
                            //new TbApplicantBankDetailSchema(DbKeys.dbHydrocarbon).flBankDetailsId.RenderCustom(bankRequisitionsGroupbox, env.Env, bankRequisitionId);
                        })
                        .OnValidating(env => {
                            //var tbBankRequistionSchema = new TbApplicantBankDetailSchema(DbKeys.dbHydrocarbon);
                            //tbBankRequistionSchema.flBankDetailsId.Validate(env.Env);
                            var tbApps = new TbTradePreApplication();
                            var isCorporate = env.Env.User.IsCorporateUser(env.Env.QueryExecuter);
                            if (!isCorporate) {
                                tbApps.flCitizenship.Validate(env.Env);
                            }
                            var userBankDetails = env.Env.User.GetAccountData(env.Env.QueryExecuter).BankAccount;
                            if (userBankDetails == null) {
                                env.Env.AddError("userBankDetails", "Банковские реквизиты не обнаружены");
                            }
                        })
                        .OnProcessing(env => {
                            var userAdrData = CommonUserHelpers.GetCurrentUserAdrData(env.Env.RequestContext);
                            var isCorporate = env.Env.User.IsCorporateUser(env.Env.QueryExecuter);
                            if (isCorporate) {
                                var accountData = env.Env.User.GetAccountData(env.Env.QueryExecuter).Account;
                                var corpData = env.Env.User.GetAccountData(env.Env.QueryExecuter).CorpData;
                                env.Model.ApplicantInfo = new ApplicantInfo {
                                    JurInfo = new JurEntityInfo {
                                        FirstPerson = corpData.FirstPersonFio,
                                        CorpName = accountData.NameRu
                                    },
                                    ApplicantType = ApplicantType.Jur,
                                    Xin = env.Env.User.GetUserXin(env.Env.QueryExecuter),
                                    Address = userAdrData.Adr,
                                    PhoneNumber = userAdrData.Phone,
                                    Name = accountData.NameRu
                                };
                            }
                            else {

                                var docInfo = CommonUserHelpers.GetFizAndEnterpreneurData(env.Env.RequestContext);
                                var userInfo = env.Env.User.GetUserInfo(env.Env.QueryExecuter);

                                if (userInfo.UserType == UserType.Individual) {
                                    env.Model.ApplicantInfo = new ApplicantInfo {
                                        ApplicantType = ApplicantType.Fiz,
                                        Xin = env.Env.User.GetUserXin(env.Env.QueryExecuter),
                                        Address = userAdrData.Adr,
                                        PhoneNumber = userAdrData.Phone,
                                        Name = docInfo.UserName,
                                        IdentityDocInfo = new IdentiyDoc {
                                            Number = docInfo.IdentityDoc.Number,
                                            DocDate = docInfo.IdentityDoc.DocDate,
                                            IssuerOrg = docInfo.IdentityDoc.IssuerOrg,
                                            DocName = docInfo.IdentityDoc.DocName,
                                            Note = docInfo.IdentityDoc.Note
                                        }
                                    };
                                }
                                else if (userInfo.UserType == UserType.IndividualCorp) {
                                    var corpName = YodaUserHelpers.GetUserOrgName(env.Env.RequestContext.User.Id, env.Env.RequestContext.QueryExecuter);
                                    env.Model.ApplicantInfo = new ApplicantInfo {
                                        ApplicantType = ApplicantType.Enterpreneur,
                                        Xin = env.Env.User.GetUserXin(env.Env.QueryExecuter),
                                        Address = userAdrData.Adr,
                                        PhoneNumber = userAdrData.Phone,
                                        Name = docInfo.EnterpreneurCorpName.CorpName,
                                        IdentityDocInfo = new IdentiyDoc {
                                            Number = docInfo.IdentityDoc.Number,
                                            DocDate = docInfo.IdentityDoc.DocDate,
                                            IssuerOrg = docInfo.IdentityDoc.IssuerOrg,
                                            DocName = docInfo.IdentityDoc.DocName,
                                            Note = docInfo.IdentityDoc.Note
                                        },
                                        EnterpreneurInfo = new EnterpreneurAdditionalInfo {
                                            FirstPersonName = docInfo.UserName
                                        },
                                        JurInfo = new JurEntityInfo {
                                            CorpName = corpName
                                        },
                                    };
                                }
                                var tbApps = new TbTradePreApplication();
                                env.Model.ApplicantInfo.Citizenship = tbApps.flCitizenship.GetVal(env.Env);
                            }

                            //var tbBankRequistionSchema = new TbApplicantBankDetailSchema(DbKeys.dbHydrocarbon);
                            //var userBankDetailsCollection = SearchCollectionsProvider.Instance.Get<UserBankDetailsSearchCollection>();
                            //var userBankDetails = userBankDetailsCollection.GetItem(tbBankRequistionSchema.flBankDetailsId.GetVal(env.Env), env.Env.RequestContext);
                            var userBankDetails = env.Env.User.GetAccountData(env.Env.QueryExecuter).BankAccount;
                            var bankName = new BankSearchCollection().GetItem(userBankDetails.Bic, env.Env.RequestContext).Name;
                            env.Model.ApplicantBankRequisitions = new BankRequisitions {
                                BankName = bankName,
                                Bik = userBankDetails.Bic,
                                Iik = userBankDetails.Iban,
                                Kbe = userBankDetails.Kbe
                            };
                        })
                    )
                    #endregion
                        .Step("Дополнительные данные", step => step
                            .OnRendering(re => {
                                var tbApps = new TbTradePreApplication();
                                int? objectId = null;
                                if (re.Args.ObjectId > 0) {
                                    objectId = re.Model.ObjectItem != null ? re.Model.ObjectItem.ItemId : re.Args.ObjectId;
                                }
                                else {
                                    objectId = re.Model.ObjectItem != null ? (int?)re.Model.ObjectItem.ItemId : null;
                                }
                                tbApps.flSubsoilsObjectId.RenderCustomT(re.Panel, re.Env, objectId);
                                //tbApps.flSubsoilsName.RenderCustomT(re.Panel, re.Env, re.Model.SubsoilsItem.Name, readOnly: true);
                                var isCorporate = re.Env.User.IsCorporateUser(re.Env.QueryExecuter);
                                if (isCorporate) {
                                    tbApps.flControlingCountry.RenderCustomT(re.Panel, re.Env, re.Model.ControllingObject?.Country);
                                    tbApps.flControlingPerson.RenderCustomT(re.Panel, re.Env, re.Model.ControllingObject?.Person);
                                    tbApps.flDocuments.RenderCustomT(re.Panel, re.Env, JsonConvert.SerializeObject(re.Model.Documents ?? new FileInfo[] { }));
                                }
                            })
                            .OnValidating(ve => {
                                var isCorporate = ve.Env.User.IsCorporateUser(ve.Env.QueryExecuter);
                                var tbApps = new TbTradePreApplication();
                                tbApps.flSubsoilsObjectId.Required().Validate(ve.Env);
                                if (isCorporate) {
                                    tbApps.flControlingCountry.Required().Validate(ve.Env);
                                    tbApps.flControlingPerson.Required().Validate(ve.Env);
                                }
                                if (ve.Env.IsValid) {
                                    var objectId = tbApps.flSubsoilsObjectId.GetVal(ve.Env);
                                    var row = new TbObjects()
                                        .Self(out var tbObjects)
                                        .AddFilter(t => t.flId, objectId)
                                        .SelectFirst(j => new FieldAlias[] { j.flBlock }, ve.Env.QueryExecuter);
                                    if (row.GetVal(t => t.flBlock) != HydrocarbonObjectBlocks.ActiveFree.ToString()) {
                                        ve.Env.AddError(tbApps.flSubsoilsObjectId.FieldName, ve.Env.T("По данному участку уже есть заявление"));
                                    }
                                }
                            })
                            .OnProcessing(pe => {
                                var isCorporate = pe.Env.User.IsCorporateUser(pe.Env.QueryExecuter);
                                var tbApps = new TbTradePreApplication();
                                if (isCorporate) {
                                    var country = tbApps.flControlingCountry.GetVal(pe.Env);
                                    pe.Model.ControllingObject = new ControllingObject(country, tbApps.flControlingPerson.GetVal(pe.Env), tbApps.flControlingCountry.GetDisplayText((object)country, pe.Env));
                                }
                                var objectId = tbApps.flSubsoilsObjectId.GetVal(pe.Env);
                                var row = new TbObjects()
                                    .Self(out var tbObjects)
                                    .AddFilter(t => t.flId, objectId)
                                    .SelectFirst(j => new FieldAlias[] { j.flId, j.flCoords, j.flName, j.flBlock }, pe.Env.QueryExecuter);
                                pe.Model.ObjectItem = new ObjectItem(objectId, row.GetVal(t => t.flName), string.Join(", ", row.GetVal(t => t.flCoords).MainRing.Select(coord => $"{coord.AppropriateX} {coord.AppropriateY}")));
                                var postedDocs = tbApps.flDocuments.GetGoodValue(pe.Env);
                                if (postedDocs == null) {
                                    postedDocs = new FileInfo[] { };
                                }
                                pe.Model.Documents = postedDocs;
                            })
                        )
                        .Step("Подпись", step => step
                          .OnRendering(re => {
                              var appContentHtml = getAppText(re.Model, re.Env);
                              re.Panel.AddComponent(
                                  new Panel()
                                    .Append(new HtmlText(appContentHtml))
                                    .Append(new UiPackages("default-doc-package"))
                                    .Append(new UiPackages("hide-term-on-sign"))
                                    .Append(new HtmlText("<br/>"))
                                    .Append(re.Env.GetSignBox(appContentHtml))
                            );
                          })
                            .OnValidating(ve => {
                                var appContentHtml = getAppText(ve.Model, ve.Env);
                                if (!ve.Env.ValidateSign(appContentHtml, out var certData, out var error)) {
                                    ve.Env.AddError("certError", error);
                                    return;
                                }
                            })
                            .OnProcessing(pe => {
                                var tbApps = new TbTradePreApplication();
                                var env = pe.Env;
                                var appContentHtml = getAppText(pe.Model, pe.Env);
                                if (!pe.Env.ValidateSign(appContentHtml, out var certData, out var error)) {
                                    pe.Env.AddError("certError", error);
                                    return;
                                }

                                lock (_sync[$"user:{env.User.GetUserXin(env.QueryExecuter)}"])
                                    using (var transaction = env.QueryExecuter.BeginTransaction(NpGlobal.DbKeys.DbHydrocarbon)) {
                                        var year = DateTime.Now.Year;
                                        var curDate = env.QueryExecuter.GetDateTime(NpGlobal.DbKeys.DbHydrocarbon);
                                        var appId = tbApps.flAppId.GetNextId(env.QueryExecuter, transaction);
                                        var userInfo = env.User.GetUserInfo(env.RequestContext.QueryExecuter);
                                        var appNumber = GetAppNumber(year, appId);

                                        var controlingCountry = pe.Model.ControllingObject == null ? (object)DBNull.Value : pe.Model.ControllingObject.Country;
                                        var person = pe.Model.ControllingObject == null ? (object)DBNull.Value : pe.Model.ControllingObject.Person;
                                        new TbTradePreApplication()
                                            .Insert()
                                            .SetT(t => t.flAppId, appId)
                                            .SetT(t => t.flApplicantXin, pe.Model.ApplicantInfo.Xin)
                                            .SetT(t => t.flAppGuidId, pe.Model.GuidId)
                                            .SetT(t => t.flAppNumber, appNumber)
                                            .SetT(t => t.flStatus, PreApplicationStatus.Registered)
                                            .SetT(t => t.flRegDate, curDate)
                                            .SetT(t => t.flAppContent, appContentHtml)
                                            .SetT(t => t.flRegByUserId, pe.Env.User.Id)
                                            .SetT(t => t.flApplicantType, getUserType(pe.Env))
                                            .SetT(t => t.flApplicantName, pe.Model.ApplicantInfo.Name)
                                            .SetT(t => t.flApplicantAddress, pe.Model.ApplicantInfo.Address)
                                            .SetT(t => t.flApplicantPhoneNumber, pe.Model.ApplicantInfo.PhoneNumber)

                                            .SetT(t => t.flSubsoilsObjectId, pe.Model.ObjectItem.ItemId)
                                            .SetT(t => t.flSubsoilsName, pe.Model.ObjectItem.Name)
                                            .SetT(t => t.flSubsoilsCoordinates, pe.Model.ObjectItem.Coordinates)
                                            .Set(t => t.flControlingCountry, controlingCountry)
                                            .Set(t => t.flControlingPerson, person)
                                            .Set(t => t.flCitizenship, pe.Model.ApplicantInfo.Citizenship)
                                            .SetT(t => t.flAppContent, appContentHtml)
                                            .SetT(t => t.flAppModel, pe.Model)
                                            .SetT(t => t.flDocuments, JsonConvert.SerializeObject(pe.Model.Documents))
                                            .Execute(env.QueryExecuter, transaction);


                                        new TbTradePreApplicationSigns()
                                            .Insert()
                                            .SetT(t => t.flAppId, appId)
                                            .SetCertData(certData, curDate, env.User.Id)
                                            .SetT(t => t.flSignerType, ApplicationSigner.Applicant)
                                            .Execute(env.QueryExecuter, transaction);

                                        if (!hasApplication(pe.Model.ObjectItem.ItemId, PreApplicationStatus.Accepted, env.QueryExecuter, null)) {
                                            ObjectHelper.SetBlock(pe.Model.ObjectItem.ItemId, HydrocarbonObjectBlocks.ActiveHasApplication, env.QueryExecuter, transaction);
                                        }

                                        transaction.Commit();

                                        env.Redirect.SetRedirect(ModuleName, MenuName, new SubsoilsApplicationArgs { AppId = appId, MenuAction = Actions.ViewApplicant });
                                        env.SetPostbackMessage(env.T("Заявление на проведение успешно зарегистрировано"));
                                    }
                            })
                        ).Build()
                    )
                )
            #endregion 
            #region view applicant
                .OnAction(Actions.ViewApplicant, action => action
                        .IsValid(e => {
                            return new OkResult();
                        })
                        .Tasks(t => {
                            t.AddIfValid(Actions.DownloadPdf, "Скачать");
                            t.AddIfValid(Actions.ViewObject, "Перейти к объекту");
                        })
                        .OnRendering(re => {
                            render(re.Args.AppId, re);
                        })
                        )
            #endregion
            #region OfficialOrgView
                .OnAction(Actions.OfficialOrgView, action => action
                .IsValid(e => {
                    if (e.User.HasRole("TRADERESOURCES-Недропользование-Создание приказов", e.QueryExecuter)/*e.User.HasCustomRole("traderesources", "appView", e.QueryExecuter)*/) {
                        return new OkResult();
                    }
                    if (e.User.HasRole("TRADERESOURCES-Недропользование-Согласование заявлений", e.QueryExecuter)/*e.User.HasCustomRole("traderesources", "appNegotiation", e.QueryExecuter)*/) {
                        return new OkResult();
                    }
                    if (e.User.HasRole("TRADERESOURCES-Недропользование-Принятие заявлений", e.QueryExecuter)/*e.User.HasCustomRole("traderesources", "appExecute", e.QueryExecuter)*/) {
                        return new OkResult();
                    }
                    if (!e.User.IsExternalUser() && !e.User.IsGuest()) {
                        return new OkResult();
                    }
                    return new AccessDeniedResult();
                })

                .Tasks(t => {
                    t.AddIfValid(Actions.OfficialOrgApprove, "Обработать");
                    t.AddIfValid(Actions.OfficialOrgAccept, "Принять в работу");
                    t.AddIfValid(Actions.OfficialOrgReject, "Отказать в рассмотрении");
                    t.AddIfValid(Actions.DownloadPdf, "Скачать");
                })

                    .OnRendering(re => {
                        var dtApprovals = new TbTradePreApplicationApproovals().AddFilter(t => t.flAppId, re.Args.AppId).Select(re.QueryExecuter);
                        foreach (DataRow approval in dtApprovals.Rows) {
                            renderApproval(re.Args.AppId, Convert.ToInt32(approval["flId"] + string.Empty), re);
                        }
                        render(re.Args.AppId, re);
                    })
                )
            #endregion
            #region OfficialOrgReject
                .OnAction(Actions.OfficialOrgReject, action => action
                .IsValid(e => {
                    if (new TbTradePreApplication()
                        .AddFilter(t => t.flAppId, e.Args.AppId)
                        .AddFilter(t => t.flStatus, PreApplicationStatus.Registered)
                        .Count(e.QueryExecuter) == 0) {
                        return new AccessDeniedResult();
                    }

                    if (!e.User.HasRole("TRADERESOURCES-Недропользование-Принятие заявлений", e.QueryExecuter)/*!e.User.HasCustomRole("traderesources", "appExecute", e.QueryExecuter)*/) {
                        return new AccessDeniedResult();
                    }

                    var negotiationCount = new TbUserRoles()
                        .Self(out var tbUserRoles)
                        .JoinT("TbUserRoles", new TbRoles().AddFilter(t => t.flRoleName, "TRADERESOURCES-Недропользование-Согласование заявлений"), "TbRoles")
                        .On((t1, t2) => new Join(t1.flRoleId, t2.flRoleId))
                        .Count(new FieldAlias[] { tbUserRoles.flUserId }, e.QueryExecuter);

                    if (new TbTradePreApplicationApproovals()
                    .AddFilter(t => t.flAppId, e.Args.AppId)
                    .AddFilter(t => t.flIsSessionClosed, false)
                    .Count(e.QueryExecuter) != negotiationCount/*CustomRolesAccessHelper.GetUsersRoleCount("traderesources", "appNegotiation", e.QueryExecuter)*/
                    ) {
                        return new AccessDeniedResult();
                    }

                    if (!e.User.GetUserXin(e.QueryExecuter).In("140940023346", "050540004455")) {
                        return new AccessDeniedResult();
                    }
                    return new OkResult();
                })

                    .OnRendering(re => {
                        re.Form.AddSubmitButton(re.T("Отказать в рассмотрении"));
                        var tbApps = new TbTradePreApplication();
                        tbApps.flRejectReason.RenderCustom(re.Form, re);
                    })
                    .OnValidating(ve => {
                        var tbApps = new TbTradePreApplication();
                        tbApps.flRejectReason
                            .Required()
                            .Validate(ve);
                    })
                    .OnProcessing(pe => {
                        var tbApps = new TbTradePreApplication();
                        tbApps.AddFilter(t => t.flAppId, pe.Args.AppId);

                        var reason = tbApps.flRejectReason.GetVal(pe);

                        using (var transaction = pe.QueryExecuter.BeginTransaction(tbApps.DbKey)) {
                            var curDate = pe.QueryExecuter.GetDateTime(tbApps.DbKey);

                            setApplicationApproval(PreApplicationApprovalStatuses.Denied, pe, transaction, true);

                            new TbTradePreApplication()
                                .AddFilter(t => t.flAppId, pe.Args.AppId)
                                .Update()
                                .SetT(t => t.flStatus, PreApplicationStatus.Declined)
                                .SetT(t => t.flRejectReason, reason)
                                .SetT(t => t.flRejectDate, curDate)
                                .Execute(pe.QueryExecuter, transaction);

                            var objectId = getObjectId(pe.Args.AppId, pe.QueryExecuter, transaction);
                            if (!hasApplication(objectId, PreApplicationStatus.Accepted, pe.QueryExecuter, transaction)) {
                                if (!hasApplication(objectId, PreApplicationStatus.Registered, pe.QueryExecuter, transaction)) {
                                    ObjectHelper.SetBlock(objectId, HydrocarbonObjectBlocks.ActiveFree, pe.QueryExecuter, transaction);
                                }
                                else {
                                    ObjectHelper.SetBlock(objectId, HydrocarbonObjectBlocks.ActiveHasApplication, pe.QueryExecuter, transaction);
                                }
                            }

                            transaction.Commit();


                            pe.Redirect.SetRedirect(ModuleName, MenuName, new SubsoilsApplicationArgs { AppId = pe.Args.AppId, MenuAction = MnuHydrocarbonApp.Actions.OfficialOrgView });
                            pe.SetPostbackMessage("Заявление отклонено");
                        }
                    })
                )
            #endregion
            #region OfficialOrgApproove
                .OnAction(Actions.OfficialOrgApprove, action => action
                .IsValid(e => {
                    if (new TbTradePreApplication()
                        .AddFilter(t => t.flAppId, e.Args.AppId)
                        .AddFilter(t => t.flStatus, PreApplicationStatus.Registered)
                        .Count(e.QueryExecuter) == 0) {
                        return new AccessDeniedResult();
                    }
                    if (!e.User.HasRole("TRADERESOURCES-Недропользование-Согласование заявлений", e.QueryExecuter)/*!e.User.HasCustomRole("traderesources", "appNegotiation", e.QueryExecuter)*/) {
                        return new AccessDeniedResult();
                    }
                    if (new TbTradePreApplicationApproovals()
                    .AddFilter(t => t.flAppId, e.Args.AppId)
                    .AddFilter(t => t.flIsSessionClosed, false)
                    .AddFilter(t => t.flUserId, e.User.Id)
                    .Count(e.QueryExecuter) == 1) {
                        return new AccessDeniedResult();
                    }
                    if (!e.User.GetUserXin(e.QueryExecuter).In("140940023346", "050540004455")) {
                        return new AccessDeniedResult();
                    }

                    return new OkResult();
                })
                .OnRendering(re => {
                    re.Form.AddSubmitButton("agree", re.T("Согласовать"));
                    re.Form.AddSubmitButton("decline", re.T("Отклонить"));
                    renderApproval(re.Args.AppId, -1, re, false);
                    render(re.Args.AppId, re);
                })
                .OnValidating(ve => {
                    var tbAppApprove = new TbTradePreApplicationApproovals();
                    var note = tbAppApprove.flNote.GetVal(ve) + string.Empty;
                    if (ve.FormCollection.AllKeys.Contains("decline")) {
                        if (string.IsNullOrEmpty(note)) {
                            ve.AddError(tbAppApprove.flNote.FieldName, "Примечание обязательно для заполнения");
                        }
                    }
                })
                .OnProcessing(pe => {
                    var status = pe.FormCollection.AllKeys.Contains("agree") ? PreApplicationApprovalStatuses.Approved : PreApplicationApprovalStatuses.Denied;
                    setApplicationApproval(status, pe);
                    pe.Redirect.SetRedirect(ModuleName, MenuName, new SubsoilsApplicationArgs { AppId = pe.Args.AppId, MenuAction = MnuHydrocarbonApp.Actions.OfficialOrgView });
                }))
            #endregion
            #region OfficialOrgAccept
                .OnAction(Actions.OfficialOrgAccept, action => action
                .IsValid(e => {
                    if (!e.User.HasRole("TRADERESOURCES-Недропользование-Принятие заявлений", e.QueryExecuter)/*!e.User.HasCustomRole("traderesources", "appExecute", e.QueryExecuter)*/) {
                        return new AccessDeniedResult();
                    }

                    var negotiationCount = new TbUserRoles()
                        .Self(out var tbUserRoles)
                        .JoinT("TbUserRoles", new TbRoles().AddFilter(t => t.flRoleName, "TRADERESOURCES-Недропользование-Согласование заявлений"), "TbRoles")
                        .On((t1, t2) => new Join(t1.flRoleId, t2.flRoleId))
                        .Count(new FieldAlias[] { tbUserRoles.flUserId }, e.QueryExecuter);

                    if (new TbTradePreApplicationApproovals()
                    .AddFilter(t => t.flAppId, e.Args.AppId)
                    .AddFilter(t => t.flIsSessionClosed, false)
                    .Count(e.QueryExecuter) != negotiationCount/*CustomRolesAccessHelper.GetUsersRoleCount("traderesources", "appNegotiation", e.QueryExecuter)*/
                    ) {
                        return new AccessDeniedResult();
                    }
                    if (new TbTradePreApplication()
                        .AddFilter(t => t.flAppId, e.Args.AppId)
                        .AddFilter(t => t.flStatus, PreApplicationStatus.Registered)
                        .Count(e.QueryExecuter) == 0) {
                        return new AccessDeniedResult();
                    }
                    if (!e.User.GetUserXin(e.QueryExecuter).In("140940023346", "050540004455")) {
                        return new AccessDeniedResult();
                    }
                    return new OkResult();
                })
                    .OnRendering(re => {
                        re.Form.AddSubmitButton(re.T("Принять в работу"));
                        render(re.Args.AppId, re);
                    })
                    .OnProcessing(pe => {

                        var tbApps = new TbTradePreApplication();
                        using (var transaction = pe.QueryExecuter.BeginTransaction(tbApps.DbKey)) {
                            var curDate = pe.QueryExecuter.GetDateTime(tbApps.DbKey);

                            setApplicationApproval(PreApplicationApprovalStatuses.Approved, pe, transaction, true);

                            new TbTradePreApplication()
                                .AddFilter(t => t.flAppId, pe.Args.AppId)
                                .Update()
                                .SetT(t => t.flStatus, PreApplicationStatus.Accepted)
                                .SetT(t => t.flAcceptDate, curDate)
                                .Execute(pe.QueryExecuter, transaction);

                            var objectId = getObjectId(pe.Args.AppId, pe.QueryExecuter, transaction);

                            ObjectHelper.SetBlock(objectId, HydrocarbonObjectBlocks.ActivePositiveDecision, pe.QueryExecuter, transaction);

                            transaction.Commit();


                            pe.Redirect.SetRedirect(ModuleName, MenuName, new SubsoilsApplicationArgs { AppId = pe.Args.AppId, MenuAction = MnuHydrocarbonApp.Actions.OfficialOrgView });
                            pe.SetPostbackMessage("Заявление принято");
                        }
                    })
                )
            #endregion
            #region OfficialOrgInProcess
                //    .OnAction(Actions.OfficialOrgInProcess, action => action
                //    .IsValid(e => {
                //        if (new TbTradePreApplication()
                //            .AddFilter(t => t.flAppId, e.Args.AppId)
                //            .AddFilter(t => t.flStatus, PreApplicationStatus.Registered)
                //            .Count(e.QueryExecuter) == 0) {
                //            return new AccessDeniedResult();
                //        }
                //        return new OkResult();
                //    })
                //    .OnRendering(re =>{
                //        re.Form.AddSubmitButton(re.T("Принять в работу"));
                //        render(re.Args.AppId, re);

                //    })
                //    .OnValidating(ve => { })
                //    .OnProcessing(pe => {

                //        var tbApps = new TbTradePreApplication();
                //        using (var transaction = pe.QueryExecuter.BeginTransaction(tbApps.DbKey)) {
                //            var curDate = pe.QueryExecuter.GetDateTime(tbApps.DbKey);

                //            new TbTradePreApplication()
                //                .AddFilter(t => t.flAppId, pe.Args.AppId)
                //                .Update()
                //                .SetT(t => t.flStatus, PreApplicationStatus.InProcess)
                //                .Execute(pe.QueryExecuter, transaction);

                //            transaction.Commit();


                //            pe.Redirect.SetRedirect(ModuleName, MenuName, new SubsoilsApplicationArgs { AppId = pe.Args.AppId, MenuAction = MnuHydrocarbonApp.Actions.OfficialOrgView });
                //            pe.SetPostbackMessage("Заявление принято");
                //        }
                //    })

                //)

            #endregion
            #region DownloadPdf
                .OnAction(Actions.DownloadPdf, action => action
                    .IsValid(e => {
                        return new OkResult();
                    })
                    .OnRendering(async re => {
                        var app = new TbTradePreApplication()
                                .AddFilter(t => t.flAppId, re.Args.AppId)
                                .SelectFirstOrDefault(t => t.Fields.ToAliases(), re.QueryExecuter);
                        var pdfBuilder = new PdfBuilder(getDownloadText(app.GetVal(t => t.flAppContent), app))
                        .SetHeader()
                        .Signs(new TbTradePreApplicationSigns()
                            .AddFilter(t => t.flAppId, re.Args.AppId)
                            .Order(t => t.flSignDate)
                            .Select(t => t.Fields.ToAliases(), re.QueryExecuter), re.RequestContext
                        )
                        .UiPackages("default-doc-package", "electronic-agreements-print-data");
                        var pdf = await pdfBuilder.Build(re.RequestContext);
                        re.Redirect.SetRedirectToFile(new FileContentResult(pdf, "application/pdf") { FileDownloadName = $"application_{re.Args.AppId}.pdf" });
                    })
                )
            #endregion
            #region ViewObject
                .OnAction(Actions.ViewObject, action => action
                    .IsValid(e => {
                        return new OkResult();
                    })
                    .OnRendering(re => {
                        var objectId = new TbTradePreApplication()
                            .AddFilter(t => t.flAppId, re.Args.AppId)
                            .SelectScalar(t => t.flSubsoilsObjectId, re.QueryExecuter);
                        re.Redirect.SetRedirect(nameof(RegistersModule), nameof(MnuHydrocarbonObjectView), new HydrocarbonObjectViewArgs { MenuAction = MnuHydrocarbonObjectView.Actions.View, Id = (int)objectId });
                    })
                )
            #endregion
            ;
        }



        public class Actions {
            public const string
            Create = "create",
            ViewApplicant = "view-applicant",
            OfficialOrgView = "organization-view",
            OfficialOrgApprove = "organization-approve",
            OfficialOrgAccept = "organization-accept",
            OfficialOrgInProcess = "organization-process",
            DownloadPdf = "download-pdf",
            OfficialOrgReject = "organization-reject",
            ViewObject = "view-object";
        }

        private int getObjectId(int appId, IQueryExecuter queryExecuter, ITransaction transaction) {
            return (int)new TbTradePreApplication()
                .AddFilter(t => t.flAppId, appId)
                .SelectScalar(t => t.flSubsoilsObjectId, queryExecuter, transaction);
        }

        private bool hasApplication(int objectId, PreApplicationStatus status, IQueryExecuter queryExecuter, ITransaction transaction) {
            return new TbTradePreApplication()
                .Self(out var tbApps)
                .AddFilter(t => t.flSubsoilsObjectId, objectId)
                .AddFilter(t => t.flStatus, status)
                .Count(new FieldAlias[] { tbApps.flAppId }, queryExecuter, transaction) > 0;
        }

        private void setApplicationApproval(PreApplicationApprovalStatuses status, ActionEnv<SubsoilsApplicationArgs> pe, ITransaction transaction = null, bool isSessionCloed = false) {
            var tbAppApprove = new TbTradePreApplicationApproovals();
            var flId = pe.QueryExecuter.GetNextId(tbAppApprove, tbAppApprove.flId.FieldName, transaction);
            tbAppApprove
            .Insert()
            .SetT(t => t.flId, flId)
            .SetT(t => t.flDateTime, DateTime.Now)
            .SetT(t => t.flSessionId, 1)
            .SetT(t => t.flUserId, pe.User.Id)
            .SetT(t => t.flUserIin, pe.User.GetUserIin(pe.QueryExecuter))
            .SetT(t => t.flStatus, status)
            .SetT(t => t.flIsSessionClosed, isSessionCloed)
            .SetT(t => t.flAppId, pe.Args.AppId)
            .SetT(t => t.flNote, (tbAppApprove.flNote.GetVal(pe) + string.Empty))
            .Execute(pe.QueryExecuter, transaction);
        }

        private void renderApproval(int appId, int apprId, RenderActionEnv<SubsoilsApplicationArgs> env, bool readOnly = true) {
            var tbAppApproval = new TbTradePreApplicationApproovals();
            var dtApp = tbAppApproval.AddFilter(t => t.flAppId, appId).AddFilter(t => t.flId, apprId).Select(env.QueryExecuter);

            if (dtApp.Rows.Count == 0 && readOnly) {
                return;
            }

            DataRow drApp;
            if (dtApp.Rows.Count == 0) {
                drApp = dtApp.NewRow();
                var now = env.QueryExecuter.GetDateTime();
                drApp[tbAppApproval.flDateTime.FieldName] = now;
                drApp[tbAppApproval.flUserId.FieldName] = env.User.Id;
                drApp[tbAppApproval.flStatus.FieldName] = string.Empty;
            }
            else {
                drApp = dtApp.Rows[0];
            }

            var groupbox = new Accordion("Согласования");
            tbAppApproval.flDateTime.RenderCustom(groupbox, env, tbAppApproval.flDateTime.GetRowVal(drApp), readOnly: true);
            tbAppApproval.flUserId.Text = env.T("Пользователь");
            tbAppApproval.flUserId.RenderCustom(groupbox, env, YodaUserHelpers.GetUserFullName(tbAppApproval.flUserId.GetRowVal(drApp), env.QueryExecuter), readOnly: true);
            tbAppApproval.flStatus.RenderCustom(groupbox, env, dtApp.Rows.Count == 0 ? null : tbAppApproval.flStatus.GetRowVal(drApp), readOnly: true);
            tbAppApproval.flNote.RenderCustom(groupbox, env, tbAppApproval.flNote.GetRowVal(drApp), readOnly: readOnly);
            env.Form.AddComponent(groupbox);
        }

        private void render(int appId, RenderActionEnv<SubsoilsApplicationArgs> env) {
            render(appId, env.Form, env);
        }

        private void render(int appId, WidgetBase container, RenderActionEnv<SubsoilsApplicationArgs> env) {
            var tbApps = new TbTradePreApplication();
            var apps = tbApps
                    .AddFilter(t => t.flAppId, appId)
                        .SelectFirst(t => new FieldAlias[] {
                            t.flAppNumber,
                            t.flRegDate,
                            t.flApplicantXin,
                            t.flApplicantName,
                            t.flApplicantAddress,
                            t.flApplicantPhoneNumber,
                            t.flSubsoilsName,
                            t.flSubsoilsObjectId,
                            t.flSubsoilsCoordinates,
                            t.flControlingCountry,
                            t.flControlingPerson,
                            t.flStatus,
                            t.flAppContent,
                            t.flRejectDate,
                            t.flRejectReason,
                            t.flDocuments
                        }, env.RequestContext.QueryExecuter);

            var aboutApplication = new Accordion("Данные заявления");
            tbApps.flAppNumber.RenderCustom(aboutApplication, env, apps.GetVal(t => t.flAppNumber), readOnly: true);
            tbApps.flRegDate.RenderCustom(aboutApplication, env, apps.GetVal(t => t.flRegDate), readOnly: true);
            tbApps.flApplicantXin.RenderCustom(aboutApplication, env, apps.GetVal(t => t.flApplicantXin), readOnly: true);
            tbApps.flApplicantName.RenderCustom(aboutApplication, env, apps.GetVal(t => t.flApplicantName), readOnly: true);
            tbApps.flApplicantAddress.RenderCustom(aboutApplication, env, apps.GetVal(t => t.flApplicantAddress), readOnly: true);
            tbApps.flApplicantPhoneNumber.RenderCustom(aboutApplication, env, apps.GetVal(t => t.flApplicantPhoneNumber), readOnly: true);
            tbApps.flSubsoilsObjectId.RenderCustom(aboutApplication, env, apps.GetVal(t => t.flSubsoilsObjectId), readOnly: true);
            //tbApps.flSubsoilsName.RenderCustom(aboutApplication, env, apps.GetVal(t => t.flSubsoilsName), readOnly: true);
            tbApps.flControlingCountry.RenderCustom(aboutApplication, env, apps.GetVal(t => t.flControlingCountry), readOnly: true);
            tbApps.flControlingPerson.RenderCustom(aboutApplication, env, apps.GetVal(t => t.flControlingPerson), readOnly: true);
            tbApps.flStatus.RenderCustom(aboutApplication, env, apps.GetVal(t => t.flStatus), readOnly: true);
            tbApps.flDocuments.RenderCustom(aboutApplication, env, apps.GetVal(t => t.flDocuments), readOnly: true);
            var reject = apps.GetValOrNull(t => t.flRejectDate);

            if (reject != null) {
                tbApps.flRejectDate.RenderCustom(aboutApplication, env, apps.GetVal(t => t.flRejectDate), readOnly: true);
                tbApps.flRejectReason.RenderCustom(aboutApplication, env, apps.GetVal(t => t.flRejectReason), readOnly: true);
            }


            container.AddComponent(aboutApplication);

            var docViewer = new DocumentViewer(apps.GetVal(t => t.flAppContent));

            var tbSigns = new TbTradePreApplicationSigns();

            tbSigns.GetSignsDataHtml(
                appId,
                env.RequestContext,
                (signerType) => $@"{tbSigns.flSignerType.GetDisplayText(signerType.ToString(), env.RequestContext).ToHtml()}",
                (signerType) => "Подписано в"
                ).Each(html => docViewer.Append(new Panel("app-content").Append(new HtmlText(html))));
            container.AddComponent(docViewer);
            container.AddComponent(new UiPackages("default-doc-package"));
        }

        private string getAppText(PreApplicationModel model, FormEnvironment env) {
            var tbApps = new TbTradePreApplication();
            var sb = new StringBuilder();

            switch (model.ApplicantInfo.ApplicantType) {
                case ApplicantType.Jur:
                sb.Append($@"<p>Наименование:  <b>{model.ApplicantInfo.JurInfo.CorpName}</b></p> ");
                sb.Append($@"<p>Место нахождения: <b>{model.ApplicantInfo.Address}</b></p>");
                sb.Append($@"<p>Бизнес-идентификационный номер: <b>{model.ApplicantInfo.Xin}</b></p>");
                sb.Append($@"<p>Сведения о государственной регистрации в качестве юридического лица (выписка из торгового реестра или другой легализованный документ, удостоверяющий, что заявитель является юридическим лицом по законодательству иностранного государства): -</p>");
                sb.Append($@"<p>Фамилия, имя и отчество (при его наличии) руководителя: <b>{model.ApplicantInfo.JurInfo.FirstPerson}</b></p>");
                sb.Append($@"<p>Адрес: <b>{model.ApplicantInfo.Address}</b></p>");
                sb.Append($@"<p>Номер телефона (факса): {model.ApplicantInfo.PhoneNumber}</b></p>");
                sb.Append($@"<p>Банковские реквизиты: </p>");
                sb.Append($@"<p>Индивидуальный идентификационный код: <b>{model.ApplicantBankRequisitions?.Iik}</b></p>");
                sb.Append($@"<p>Банковский идентификационный код:  <b>{model.ApplicantBankRequisitions?.Bik}</b></p>");
                sb.Append($@"<p>Наименование банка:  <b>{model.ApplicantBankRequisitions?.BankName}</b></p>");
                sb.Append($@"<p>Код бенефициара: <b>{model.ApplicantBankRequisitions?.Kbe}</b></p>");

                break;
                case ApplicantType.Fiz:
                sb.Append($@"<p>Фамилия, имя и отчество (при его наличии): <b>{model.ApplicantInfo.Name}</b></p>");
                sb.Append($@"<p>Индивидуальный идентификационный номер: <b>{model.ApplicantInfo.Xin}</b> </p>");
                sb.Append($@"<p>Паспортные данные:</p> ");
                sb.Append($@"<p>Адрес: <b>{model.ApplicantInfo.Address}</b></p> ");
                sb.Append($@"<p>Фактическое место жительства: <b>{model.ApplicantInfo.Address}</b></p>");
                sb.Append($@"<p>Гражданство: <b>{tbApps.flCitizenship.GetDisplayText((object)model.ApplicantInfo.Citizenship, env)}</b></p>");
                sb.Append($@"<p>Номер телефона (факса): <b>{model.ApplicantInfo.PhoneNumber}</b></p>");
                sb.Append($@"<p>Банковские реквизиты: </p>");
                sb.Append($@"<p>Индивидуальный идентификационный код: <b>{model.ApplicantBankRequisitions?.Iik}</b></p>");
                sb.Append($@"<p>Банковский идентификационный код: <b>{model.ApplicantBankRequisitions?.Bik}</b></p> ");
                sb.Append($@"<p>Наименование банка: <b>{model.ApplicantBankRequisitions?.BankName}</b></p>");
                sb.Append($@"<p>Код бенефициара: <b>{model.ApplicantBankRequisitions?.Kbe}</b> </p>");
                break;
                case ApplicantType.Enterpreneur:
                sb.Append($@"<p>Наименование:  <b>{model.ApplicantInfo.JurInfo.CorpName}</b></p> ");
                sb.Append($@"<p>Фамилия, имя и отчество (при его наличии): <b>{model.ApplicantInfo.Name}</b></p>");
                sb.Append($@"<p>Индивидуальный идентификационный номер: <b>{model.ApplicantInfo.Xin}</b> </p>");
                sb.Append($@"<p>Паспортные данные:</p> ");
                sb.Append($@"<p>Адрес: <b>{model.ApplicantInfo.Address}</b></p> ");
                sb.Append($@"<p>Фактическое место жительства: <b>{model.ApplicantInfo.Address}</b></p>");
                sb.Append($@"<p>Гражданство: <b>{tbApps.flCitizenship.GetDisplayText((object)model.ApplicantInfo.Citizenship, env)}</b></p>");
                sb.Append($@"<p>Номер телефона (факса): <b>{model.ApplicantInfo.PhoneNumber}</b></p>");
                sb.Append($@"<p>Банковские реквизиты: </p>");
                sb.Append($@"<p>Индивидуальный идентификационный код: <b>{model.ApplicantBankRequisitions?.Iik}</b></p>");
                sb.Append($@"<p>Банковский идентификационный код: <b>{model.ApplicantBankRequisitions?.Bik}</b></p> ");
                sb.Append($@"<p>Наименование банка: <b>{model.ApplicantBankRequisitions?.BankName}</b></p>");
                sb.Append($@"<p>Код бенефициара: <b>{model.ApplicantBankRequisitions?.Kbe}</b> </p>");
                break;

            }

            var coordinates = new TbObjects().AddFilter(t => t.flId, model.ObjectItem.ItemId).GetObjectModelFirst(env.QueryExecuter).flCoords.MainRing;
            var table = new StringBuilder();
            table
                .Append("<table class='table table-bordered table-centered text-center font-10'>")
                .Append("<tr>")
                .Append("<td class='text-bold'>Долгота</td>")
                .Append("<td class='text-bold'>Широта</td>")
                .Append("</tr>");
            foreach (var coordinate in coordinates) {
                table
                    .Append("<tr>")
                    .Append($"<td>{coordinate.AppropriateX}</td>")
                    .Append($"<td>{coordinate.AppropriateY}</td>")
                    .Append("</tr>");
            }
            table.Append("</table>");

            return new DocumentBuilder()
                .Doc(doc => doc
                    .AddSection(s => s
                        .Body(b => b
                        .Div(@"Заявление
на проведение аукциона", new CssClass("text-center font-weight-bold"))
                        .Paragraph($@"1. Рассмотрев программу управления государственным фондом недр, прошу(им) провести аукцион на предоставление права недропользования по углеводородам по следующему участку недр:")
                        .Paragraph($@"Наименование участка указанного в программе управления государственным фондом недр: <b>{model.ObjectItem.Name}</b>;")
                        .Paragraph($@"Географические координаты предоставляемого для разведки и добычи или добычи углеводородов на основании аукциона: <br/>")
                        .Html(table.ToString())
                        .Paragraph($@"2. Сведения о физических лицах, юридических лицах, государствах и международных организациях, прямо или косвенно контролирующих меня (нас) (для юридических лиц):")
                        .Paragraph($@"Наименование государства: <b>{model.ControllingObject?.CountryText}</b>")
                        .Paragraph($@"Наименование лица, организации: <b>{model.ControllingObject?.Person}</b>")
                        .Paragraph($@"3. Подтверждаю достоверность представленной информации, осведомлен об ответственности за представление недостоверных сведений в соответствии с законодательством Республики Казахстан и даю согласие на использование сведений, составляющих охраняемую законом тайну, а также на сбор, обработку, хранение, выгрузку и использование персональных данных и иной информации.")
                        .Paragraph($@"4. Представляю(-ем) сведения о себе:")
                        .Html($@"{sb}")

                        ))).Build();
        }

        private string getApplicationHeader(string xin, string appNumber, string appDate) {
            var subtitle = "ОРГАНИЗАЦИЯ И ПРОВЕДЕНИЕ ТОРГОВ ПРИРОДНЫМИ РЕСУРСАМИ";
            var systemOperator = "Оператор системы: АО \"Информационно-учетный центр\"";
            var qrCodeHtml = getDataAsQrCode(xin, appNumber, appDate);

            return
                @"
<div class='app-content'>
<div style='position: relative;background: #222;color: #fff;text-align: center;padding-top: 5px;padding-bottom: 5px;border-radius: 5px 5px 0 0;font-size: 15px;'>
            " + systemOperator + @" | +7 (7172) 55-29-81 | iac@gosreestr.kz | www.gosreestr.kz
                </div>
                <div style='position: relative;min-height: 90px;background: #332ED9;color: #fff;text-align: left;padding-left: 120px;font-size: 15pt;'>
                    <div class='logo-png'>
                    <img style='width: 100px;height: 100px;' src='https://traderesources.gosreestr.kz/Theme/ui-packages/project-theme/logo/traderesources/logo-light-sm.svg'>
                </div>
                <div style='padding: 0;padding-top: 30px;padding-bottom: 15px;font-weight: bold;'>
                           " + subtitle + @"
                </div>          
                </div>" + qrCodeHtml + "</div>";
        }

        private string getDownloadText(string text, SelectFirstResultProxy<TbTradePreApplication> app) {
            var xin = app.GetVal(t => t.flApplicantXin);
            var regDate = app.GetVal(t => t.flRegDate).ToString("dd.MM.yyyy HH:mm");
            var appNumber = app.GetVal(t => t.flAppNumber);


            return getApplicationHeader(xin, appNumber, regDate) + text;
        }

        private static string getLink() {
            return "https://traderesources.gosreestr.kz/ru/traderesources/res-subsoil/res-hydrocarbon/check-app";
        }



        private static string getDataAsQrCode(string xin, string appNumber, string appDate) {
            var sb = new StringBuilder();
            var link = getLink();
            var linkText = "Проверить документ можно по ссылке";
            var iinText = "ИИН/БИН";
            var appNumberText = "НОМЕР ЗАЯВКИ";
            var appDateText = "ДАТА ЗАЯВКИ";

            sb.Append($"" +
                $"<div style='position: relative;min-height: 156px;background: #efefef;color: #000;text-align: left;padding-left: 160px;border-radius: 0 0 5px 5px;'>" +
                    $"<div class='qr-png' style='position: absolute;left: 5px;top:5px;width: 146px;height: 146px;'>" +
                        $"<img src='data:image/png;base64,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' width='80px' height='80px'/>" +
                    $"</div>" +
                    $"<div style='font-size: 16px;padding-top: 10px;padding-bottom: 10px;'>{linkText}: <br/><a target='_blank' href='{link}'>{link}</a></div>" +
                    $"<div style='float:left;font-size: 13pt;'>" +
                        $"{iinText}: <caps style='font-weight: bold;'>{xin}</caps><br>" +
                        $"{appNumberText}: <caps style='font-weight: bold;'>{appNumber}</caps><br>" +
                        $"{appDateText}: <caps style='font-weight: bold;'>{appDate}</caps><br>" +
                    $"</div>" +
                $"</div>");
            return sb.ToString();
        }

        private ApplicantType getUserType(FormEnvironment env) {
            var type = env.User.GetUserInfo(env.QueryExecuter).UserType;
            var dict = new Dictionary<UserType, ApplicantType>();
            dict.Add(UserType.IndividualCorp, ApplicantType.Enterpreneur);
            dict.Add(UserType.Individual, ApplicantType.Fiz);
            dict.Add(UserType.Corporate, ApplicantType.Jur);

            if (!dict.ContainsKey(type)) {
                throw new Exception($"Not contain key '{Enum.GetName(typeof(UserType), type)}'");
            }

            return dict[type];
        }

        private static string GetAppNumber(int year, int appId) {
            var appNumberPrefix = $"{year}-01";
            return $"{appNumberPrefix}-{appId.ToString().PadLeft(3, '0')}";
        }
    }

    static class UserManagementServicesExtensions {
        public static IAccountManagerProvider GetAccountManagerProvider(this IYodaRequestContext context)
            => context.AppEnv.ServiceProvider.GetRequiredService<IAccountManagerProvider>();

        public static IAccountManager GetCurrentAccountManager(this IYodaRequestContext context)
            => context.GetAccountManagerProvider().Get(context.User.GetAccountId(context.QueryExecuter), context.QueryExecuter);

        public static IUserManagerProvider GetUserManagerProvider(this IYodaRequestContext context)
            => context.AppEnv.ServiceProvider.GetRequiredService<IUserManagerProvider>();

        public static IUserManager GetUserManager(this IYodaRequestContext context, string login)
            => context.GetUserManagerProvider().Get(login, context.QueryExecuter);

        public static IUserManager GetCurrentUserManager(this IYodaRequestContext context)
            => context.GetUserManagerProvider().Get(context.User.Name, context.QueryExecuter);
    }

    public class SubsoilsApplicationArgs : ActionQueryArgsBase {
        public int AppId { get; set; }
        public int ObjectId { get; set; }
    }
}
