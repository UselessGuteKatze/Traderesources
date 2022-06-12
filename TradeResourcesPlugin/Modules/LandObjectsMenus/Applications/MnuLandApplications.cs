using CommonSource.Models;
using CommonSource.References.Application;
using CommonSource.References.Object;
using CommonSource.SearchCollections.Object;
using LandSource.QueryTables.Applications;
using LandSource.QueryTables.LandObject;
using LandSource.References.Applications;
using LandSource.References.LandObject;
using LandSource.SearchCollections.LandObject;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using TradeResourcesPlugin.Helpers;
using TradeResourcesPlugin.Modules.Components;
using TradeResourcesPlugin.Modules.LandObjectsMenus.Object;
using UsersResources;
using UsersResources.Refs;
using Yoda.Interfaces;
using Yoda.Interfaces.Forms;
using Yoda.Interfaces.Forms.Components;
using Yoda.Interfaces.Helpers;
using YodaApp.Yoda.Interfaces.Forms.Components;
using YodaCommonReferences;
using YodaHelpers;
using YodaHelpers.ActionMenus;
using YodaHelpers.Components;
using YodaHelpers.Fields;
using YodaHelpers.HtmlDocumentBuilder;
using YodaQuery;
using YodaQuery.Yoda.Query.Expressions;

namespace TradeResourcesPlugin.Modules.LandObjectsMenus.Applications {
    public class LandApplicationsArgs : ActionQueryArgsBase {
        public int AppId { get; set; }
    }
    public class MnuLandApplications : MnuActionsExt<LandApplicationsArgs> {
        public const string MnuName = nameof(MnuLandApplications);
        public MnuLandApplications(string moduleName) : base(moduleName, MnuName, "Предложение о вынесении свободного земельного участка на торги")
        {
            AsCallback();
        }


        public override void Configure(ActionConfig<LandApplicationsArgs> config)
        {
            config
                .OnAction(Actions.Create, c => {
                        return c
                .IsValid(env =>
                {
                    if (env.User.IsAuthentificated)
                    {
                        var userInfo = env.User.GetUserInfo(env.QueryExecuter);
                        if (userInfo.UserType != UserType.IndividualCorp)
                        {
                            return new OkResult();
                        }
                    }
                    return new RedirectResult(new AccessDeniedException("Подавать заявления могут только юридические и физические лица!"));
                })
                .Wizard(wizard => wizard
                    .Args(args => args.Args)
                    .Model(env =>
                    {
                        var model = new LandApplicationModel
                        {
                            flGuid = Guid.NewGuid().ToString("D").ToUpper(),
                            flStatus = ApplicationStatuses.Сохранена,
                            flRegDate = env.QueryExecuter.GetDateTime("dbAgreements"),
                            flRegByUserId = env.User.Id
                        };

                        return model;
                    })
                    .CancelBtn("Отмена", ev => new ActionRedirectData(ModuleName, MnuLandApplicationsSearch.MnuName))
                    .FinishBtn("Сохранить")
                    .Step("Данные заявителя", step =>
                    {
                        return step
                        .OnRendering(env =>
                        {
                            var isCorporate = env.Env.User.IsCorporateUser(env.Env.QueryExecuter);
                            var userAdrData = CommonUserHelpers.GetCurrentUserAdrData(env.Env.RequestContext);
                            var applicantInfoGroupbox = new Accordion(env.Env.T("Сведения о заявителе"));
                            env.Panel.AddComponent(applicantInfoGroupbox);

                            applicantInfoGroupbox.Append(new LinkUrl(
                            env.Env.T("Редактировать данные пользователя"),
                            env.Env.RequestContext.Configuration["UserEditSelfDataUrl"],
                            openInNewWindow: true
                            ));

                            var tbApps = new TbLandApplications();
                            var accountData = env.Env.User.GetAccountData(env.Env.QueryExecuter).Account;

                            tbApps.flGuid.RenderCustom(applicantInfoGroupbox, env.Env, env.Model.flGuid, readOnly: true);

                            if (isCorporate)
                            {
                                tbApps.flApplicantXin.RenderCustom(applicantInfoGroupbox, env.Env, env.Env.User.GetUserXin(env.Env.QueryExecuter), readOnly: true);
                                tbApps.flApplicantName.RenderCustom(applicantInfoGroupbox, env.Env, accountData.NameRu, readOnly: true);
                                tbApps.flApplicantAddress.RenderCustom(applicantInfoGroupbox, env.Env, userAdrData.Adr, readOnly: true);
                                tbApps.flApplicantPhoneNumber.RenderCustom(applicantInfoGroupbox, env.Env, userAdrData.Phone, readOnly: true);
                            }
                            else
                            {
                                var docInfo = CommonUserHelpers.GetFizAndEnterpreneurData(env.Env.RequestContext);
                                var userInfo = env.Env.User.GetUserInfo(env.Env.QueryExecuter);
                                tbApps.flApplicantXin.RenderCustom(applicantInfoGroupbox, env.Env, env.Env.User.GetUserXin(env.Env.QueryExecuter), readOnly: true);
                                if (userInfo.UserType == UserType.IndividualCorp)
                                {
                                    tbApps.flApplicantName.RenderCustom(applicantInfoGroupbox, env.Env, accountData.NameRu, readOnly: true);
                                }
                                tbApps.flApplicantName.RenderCustom(applicantInfoGroupbox, env.Env, docInfo.UserName, readOnly: true);
                                tbApps.flApplicantAddress.RenderCustom(applicantInfoGroupbox, env.Env, userAdrData.Adr, readOnly: true);
                                tbApps.flApplicantPhoneNumber.RenderCustom(applicantInfoGroupbox, env.Env, userAdrData.Phone, readOnly: true);
                            }
                        })
                        .OnValidating(env =>
                        {
                        })
                        .OnProcessing(env =>
                        {
                            env.Model.flApplicantXin = env.Env.User.GetUserXin(env.Env.QueryExecuter);
                            var userAdrData = CommonUserHelpers.GetCurrentUserAdrData(env.Env.RequestContext);
                            var isCorporate = env.Env.User.IsCorporateUser(env.Env.QueryExecuter);
                            if (isCorporate)
                            {
                                var accountData = env.Env.User.GetAccountData(env.Env.QueryExecuter).Account;
                                var corpData = env.Env.User.GetAccountData(env.Env.QueryExecuter).CorpData;
                                env.Model.flApplicantInfo = new ApplicantInfo
                                {
                                    JurInfo = new JurEntityInfo
                                    {
                                        FirstPerson = corpData.FirstPersonFio,
                                        CorpName = accountData.NameRu
                                    },
                                    ApplicantType = ApplicantType.Jur,
                                    Xin = env.Model.flApplicantXin,
                                    Address = userAdrData.Adr,
                                    PhoneNumber = userAdrData.Phone,
                                    Name = accountData.NameRu
                                };
                                env.Model.flApplicantType = ApplicantType.Jur;
                                env.Model.flApplicantName = accountData.NameRu;
                                env.Model.flApplicantAddress = userAdrData.Adr;
                                env.Model.flApplicantPhoneNumber = userAdrData.Phone;
                            }
                            else
                            {

                                var docInfo = CommonUserHelpers.GetFizAndEnterpreneurData(env.Env.RequestContext);
                                var userInfo = env.Env.User.GetUserInfo(env.Env.QueryExecuter);

                                if (userInfo.UserType == UserType.Individual)
                                {
                                    if (docInfo.IdentityDoc == null)
                                    {
                                        docInfo.IdentityDoc = new IdentiyDoc();
                                    }
                                    env.Model.flApplicantInfo = new ApplicantInfo
                                    {
                                        ApplicantType = ApplicantType.Fiz,
                                        Xin = env.Model.flApplicantXin,
                                        Address = userAdrData.Adr,
                                        PhoneNumber = userAdrData.Phone,
                                        Name = docInfo.UserName,
                                        IdentityDocInfo = new IdentiyDoc
                                        {
                                            Number = docInfo.IdentityDoc.Number,
                                            DocDate = docInfo.IdentityDoc.DocDate,
                                            IssuerOrg = docInfo.IdentityDoc.IssuerOrg,
                                            DocName = docInfo.IdentityDoc.DocName,
                                            Note = docInfo.IdentityDoc.Note
                                        }
                                    };
                                    env.Model.flApplicantType = ApplicantType.Fiz;
                                    env.Model.flApplicantName = docInfo.UserName;
                                    env.Model.flApplicantAddress = userAdrData.Adr;
                                    env.Model.flApplicantPhoneNumber = userAdrData.Phone;
                                }
                                else if (userInfo.UserType == UserType.IndividualCorp)
                                {
                                    var corpName = YodaUserHelpers.GetUserOrgName(env.Env.RequestContext.User.Id, env.Env.RequestContext.QueryExecuter);
                                    env.Model.flApplicantInfo = new ApplicantInfo
                                    {
                                        ApplicantType = ApplicantType.Enterpreneur,
                                        Xin = env.Model.flApplicantXin,
                                        Address = userAdrData.Adr,
                                        PhoneNumber = userAdrData.Phone,
                                        Name = docInfo.EnterpreneurCorpName.CorpName,
                                        IdentityDocInfo = new IdentiyDoc
                                        {
                                            Number = docInfo.IdentityDoc.Number,
                                            DocDate = docInfo.IdentityDoc.DocDate,
                                            IssuerOrg = docInfo.IdentityDoc.IssuerOrg,
                                            DocName = docInfo.IdentityDoc.DocName,
                                            Note = docInfo.IdentityDoc.Note
                                        },
                                        EnterpreneurInfo = new EnterpreneurAdditionalInfo
                                        {
                                            FirstPersonName = docInfo.UserName
                                        },
                                        JurInfo = new JurEntityInfo
                                        {
                                            CorpName = corpName
                                        },
                                    };
                                    env.Model.flApplicantType = ApplicantType.Enterpreneur;
                                    env.Model.flApplicantName = docInfo.EnterpreneurCorpName.CorpName;
                                    env.Model.flApplicantAddress = userAdrData.Adr;
                                    env.Model.flApplicantPhoneNumber = userAdrData.Phone;
                                }
                            }
                        });
                    })
                    .Step("Земельный участок", step =>
                    {
                        return step
                        .OnRendering(re =>
                        {
                            var tbApps = new TbLandApplications();
                            var model = re.Model;

                            tbApps.flCompetentOrgBin.RenderCustom(re.Panel, re.Env, model.flCompetentOrgBin);
                            tbApps.flUsageAim.RenderCustom(re.Panel, re.Env, model.flUsageAim);
                            tbApps.flUsageAimText.RenderCustom(re.Panel, re.Env, model.flUsageAimText);

                            tbApps.flCountry.RenderCustom(re.Panel, re.Env, model.flCountry);
                            tbApps.flRegion.RenderCustom(re.Panel, re.Env, model.flRegion);
                            tbApps.flDistrict.RenderCustom(re.Panel, re.Env, model.flDistrict);
                            tbApps.flAddress.RenderCustom(re.Panel, re.Env, model.flAddress);

                        })
                        .OnValidating(ve =>
                        {
                            var tbApps = new TbLandApplications();
                            var checkFields = new Field[] {
                                    tbApps.flCompetentOrgBin,
                                    tbApps.flUsageAim,
                                    tbApps.flUsageAimText,
                                    tbApps.flCountry,
                                    tbApps.flRegion,
                                    tbApps.flDistrict,
                                    tbApps.flAddress
                            };
                            checkFields.Each(f => f.Validate(ve.Env));

                            if (ve.Env.IsValid)
                            {
                                if (string.IsNullOrEmpty(tbApps.flUsageAimText.GetVal(ve.Env)))
                                {
                                    ve.Env.AddError(tbApps.flUsageAimText.FieldName, ve.Env.T($"Поле \"{tbApps.flUsageAimText.Text}\" обязательно для заполнения!"));
                                }
                                if (string.IsNullOrEmpty(tbApps.flAddress.GetVal(ve.Env)))
                                {
                                    ve.Env.AddError(tbApps.flAddress.FieldName, ve.Env.T($"Поле \"{tbApps.flAddress.Text}\" обязательно для заполнения!"));
                                }
                            }

                        })
                        .OnProcessing(pe =>
                        {
                            var tbApps = new TbLandApplications();

                            pe.Model.flCompetentOrgBin = tbApps.flCompetentOrgBin.GetVal(pe.Env);
                            pe.Model.flUsageAim = tbApps.flUsageAim.GetVal(pe.Env);
                            pe.Model.flUsageAimText = tbApps.flUsageAimText.GetVal(pe.Env);
                            pe.Model.flCountry = tbApps.flCountry.GetVal(pe.Env);
                            pe.Model.flRegion = tbApps.flRegion.GetVal(pe.Env);
                            pe.Model.flDistrict = tbApps.flDistrict.GetVal(pe.Env);
                            pe.Model.flAddress = tbApps.flAddress.GetVal(pe.Env);
                        });
                    })
                    .Step("Геометрия", step =>
                    {
                        return step
                        .OnRendering(re =>
                        {
                            var tbApps = new TbLandApplications();
                            var model = re.Model;

                            re.Panel.AddComponent(new Textbox("flWKT", cssClass: "d-none"));
                            re.Panel.AddComponent(new Textbox("flCoords", cssClass: "d-none"));

                            re.Panel.AddComponent(new ObjectDrawByCoordsComponent("flWKT", "flCoords"));

                        })
                        .OnValidating(ve =>
                        {
                            var tbApps = new TbLandApplications();
                            var checkFields = new Field[] {
                                    tbApps.flWKT,
                                    tbApps.flCoords,
                            };
                            checkFields.Each(f => f.Validate(ve.Env));

                            if (ve.Env.IsValid)
                            {
                                if (string.IsNullOrEmpty(tbApps.flWKT.GetVal(ve.Env)) || tbApps.flWKT.GetVal(ve.Env) == "ERROR")
                                {
                                    ve.Env.AddError(tbApps.flWKT.FieldName, ve.Env.T("Перед переходом к следующему шагу проверьте введённые координаты"));
                                }
                                else
                                {
                                    var tbActRevs = new TbLandObjects();
                                    tbActRevs.AddFilterNot(t => t.flStatus, LandObjectStatuses.Deleted.ToString());

                                    var geometry = new GeomFromWKT(tbApps.flWKT.GetVal(ve.Env), 4326);

                                    var contains = new GeomContains(new SqlExpVal(new FieldToSqlExp(tbActRevs.flGeometry)), new SqlExpVal(geometry));
                                    var inresects = new GeomIntersects(new SqlExpVal(new FieldToSqlExp(tbActRevs.flGeometry)), new SqlExpVal(geometry));

                                    var result = tbActRevs.Select(new FieldAlias[] {
                                            new FieldAlias(contains, "flContains"),
                                            new FieldAlias(inresects, "flIntersects"),
                                    }, ve.Env.QueryExecuter).AsEnumerable();

                                    if (result.Any(r => Convert.ToBoolean(r["flContains"]) || Convert.ToBoolean(r["flIntersects"])))
                                    {
                                        ve.Env.AddError(tbApps.flWKT.FieldName, ve.Env.T("Нарисованная вами геометрия пересекается с существующими объектами земельных ресурсов"));
                                    }
                                }
                            }
                        })
                        .OnProcessing(pe =>
                        {
                            var tbApps = new TbLandApplications();

                            pe.Model.flWKT = tbApps.flWKT.GetVal(pe.Env);
                            var area = tbApps.SelectScalar(t => new GeomArea(new SqlExpVal(new GeomFromWKT(tbApps.flWKT.GetVal(pe.Env), 4326))), pe.Env.QueryExecuter);
                            pe.Model.flArea = Convert.ToDecimal(area) * 0.0001m;
                            pe.Model.flCoords = tbApps.flCoords.GetVal(pe.Env);

                        });
                    })
                    .Step("Просмотр", step =>
                    {
                        return step
                        .OnRendering(async re =>
                        {
                            var tbApps = new TbLandApplications();
                            var model = re.Model;

                            var doc = new ApplicationDoc(model, re.Env.RequestContext);
                            re.Panel.AddComponent(doc.GetPanelHtml());
                        });
                    })
                    .Step("Подписание", step =>
                    {
                        return step
                        .OnRendering(async re =>
                        {
                            var tbApps = new TbLandApplications();
                            var model = re.Model;
                            if (model.flId == 0)
                            {
                                model.flId = tbApps.flId.GetNextId(re.Env.QueryExecuter);
                            }

                            var doc = new ApplicationDoc(model, re.Env.RequestContext);


                            var args = new DefaultDocPdfMnuArgs()
                            {
                                ContentCache = $"{MenuName}-{re.Env.User.Id}-{model.flRegDate.ToString("dd-MM-yyyy-hh-mm")}",
                                ContentUiPackagesCache = $"UI-Packages-{MenuName}-{re.Env.User.Id}-{model.flRegDate.ToString("dd-MM-yyyy-hh-mm")}"
                            };

                            re.Env.RequestContext.Cache.Set(args.ContentCache, doc.GetContent());
                            re.Env.RequestContext.Cache.Set(args.ContentUiPackagesCache, doc.GetUIPackages());

                            using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { ExecutablePath = re.Env.RequestContext.Configuration["ChromiumPath"], Headless = true });
                            using var page = await browser.NewPageAsync();
                            //await page.SetContentAsync(doc.GetPdfHtml());
                            await page.GoToAsync(re.Env.RequestContext.GetUrlHelper().YodaAction(ModuleName, nameof(DefaultDocPdfMnu), args, urlWithSchema: true));
                            var appContentHtml = await page.GetContentAsync();
                            re.Env.RequestContext.Cache.Set($"Rendered-{args.ContentCache}", appContentHtml);
                            browser.CloseAsync();




                            //var appContentHtml = new HtmlText(doc.GetPdfHtml()).Html;
                            re.Panel.AddComponent(
                                new DocumentViewer(appContentHtml)
                                    .Append(new UiPackages("hide-term-on-sign"))
                                    .Append(new HtmlText("<br/>"))
                                    .Append(re.Env.GetSignBox(appContentHtml))
                            );
                        })
                        .OnValidating(async ve =>
                        {
                            var model = ve.Model;
                            var doc = new ApplicationDoc(model, ve.Env.RequestContext);

                            var args = new DefaultDocPdfMnuArgs()
                            {
                                ContentCache = $"{MenuName}-{ve.Env.User.Id}-{model.flRegDate.ToString("dd-MM-yyyy-hh-mm")}",
                                ContentUiPackagesCache = $"UI-Packages-{MenuName}-{ve.Env.User.Id}-{model.flRegDate.ToString("dd-MM-yyyy-hh-mm")}"
                            };
                            var appContentHtml = ve.Env.RequestContext.Cache.Get<string>($"Rendered-{args.ContentCache}");
                            if (!ve.Env.ValidateSign(appContentHtml, out var certData, out var error))
                            {
                                ve.Env.AddError("certError", error);
                                return;
                            }

                        })
                        .OnProcessing(async pe =>
                        {
                            var model = pe.Model;

                            var args = new DefaultDocPdfMnuArgs()
                            {
                                ContentCache = $"{MenuName}-{pe.Env.User.Id}-{model.flRegDate.ToString("dd-MM-yyyy-hh-mm")}",
                                ContentUiPackagesCache = $"UI-Packages-{MenuName}-{pe.Env.User.Id}-{model.flRegDate.ToString("dd-MM-yyyy-hh-mm")}"
                            };
                            var appContentHtml = pe.Env.RequestContext.Cache.Get<string>($"Rendered-{args.ContentCache}");
                            if (!pe.Env.ValidateSign(appContentHtml, out var certData, out var error))
                            {
                                pe.Env.AddError("certError", error);
                                return;
                            }

                            var curDate = pe.Env.QueryExecuter.GetDateTime("dbAgreements");
                            var TbLandApplications = new TbLandApplications();
                            model.flSignSendDate = curDate;
                            model.flCertInfo = certData.CertData;

                            var doc = new ApplicationDoc(model, pe.Env.RequestContext);

                            pe.Env.RequestContext.Cache.Set(args.ContentCache, doc.GetContent());
                            pe.Env.RequestContext.Cache.Set(args.ContentUiPackagesCache, doc.GetUIPackages());

                            using var browserSigned = await Puppeteer.LaunchAsync(new LaunchOptions { ExecutablePath = pe.Env.RequestContext.Configuration["ChromiumPath"], Headless = true });
                            using var pageSigned = await browserSigned.NewPageAsync();
                            //await pageSigned.SetContentAsync(doc.GetPdfHtml());
                            await pageSigned.GoToAsync(pe.Env.RequestContext.GetUrlHelper().YodaAction(ModuleName, nameof(DefaultDocPdfMnu), args, urlWithSchema: true));
                            Thread.Sleep(5000);
                            var pdfDataWithSigns = await pageSigned.PdfDataAsync(new PdfOptions()
                            {
                                Format = PaperFormat.A4,
                                MarginOptions = new MarginOptions()
                                {
                                    Bottom = "1.3cm",
                                    Left = "2.6cm",
                                    Right = "1.3cm",
                                    Top = "1.3cm"
                                },
                                PrintBackground = true
                            });
                            browserSigned.CloseAsync();


                            pe.Env.RequestContext.Cache.Remove(args.ContentCache);
                            pe.Env.RequestContext.Cache.Remove(args.ContentUiPackagesCache);
                            pe.Env.RequestContext.Cache.Remove($"Rendered-{args.ContentCache}");


                            TbLandApplications.Insert()
                            .SetT(t => t.flId, model.flId)
                            .SetT(t => t.flGuid, model.flGuid)
                            //.SetT(t => t.flLandObjectId, model.flLandObjectId)
                            .SetT(t => t.flStatus, model.flStatus)
                            .SetT(t => t.flCompetentOrgBin, model.flCompetentOrgBin)
                            .SetT(t => t.flRegDate, model.flRegDate)
                            .SetT(t => t.flSignSendDate, model.flSignSendDate)
                            //.SetT(t => t.flAcceptDate, model.flAcceptDate)
                            //.SetT(t => t.flRejectDate, model.flRejectDate)
                            //.SetT(t => t.flRejectReason, model.flRejectReason)
                            .SetT(t => t.flRegByUserId, model.flRegByUserId)
                            .SetT(t => t.flApplicantType, model.flApplicantType)
                            .SetT(t => t.flApplicantXin, model.flApplicantXin)
                            .SetT(t => t.flApplicantName, model.flApplicantName)
                            .SetT(t => t.flApplicantAddress, model.flApplicantAddress)
                            .SetT(t => t.flApplicantPhoneNumber, model.flApplicantPhoneNumber)
                            .SetT(t => t.flApplicantInfo, model.flApplicantInfo)
                            .Set(t => t.flCountry, model.flCountry)
                            .Set(t => t.flRegion, model.flRegion)
                            .Set(t => t.flDistrict, model.flDistrict)
                            .SetT(t => t.flAddress, model.flAddress)
                            .SetT(t => t.flWKT, model.flWKT)
                            .SetT(t => t.flArea, model.flArea)
                            .SetT(t => t.flCoords, model.flCoords)
                            .Set(t => t.flUsageAim, model.flUsageAim)
                            .Set(t => t.flUsageAimText, model.flUsageAimText)
                            .SetT(t => t.flDataToSign, certData.DataToSign)
                            .SetT(t => t.flCertInfo, certData.CertData)
                            .SetT(t => t.flSignedData, certData.SignedData)
                            .Set(t => t.flPdf, pdfDataWithSigns)
                            .Execute(pe.Env.QueryExecuter);

                            pe.Env.Redirect.SetRedirect(ModuleName, MenuName, new LandApplicationsArgs() { AppId = model.flId, MenuAction = "view" });
                            pe.Env.SetPostbackMessage("Заявление создано");

                        });
                    })
                   .Build()
                );
                })
                .OnAction(Actions.View, c => {
                    return c
                    .IsValid(env =>
                    {
                        var isInternal = (!env.User.IsExternalUser() && !env.User.IsGuest());
                        var isUserRegistrator = env.User.HasRole("TRADERESOURCES-Земельные ресурсы-Создание приказов", env.QueryExecuter)/*env.User.HasCustomRole("landobjects", "appLandEdit", env.QueryExecuter) || env.User.HasCustomRole("landobjects", "appLandView", env.QueryExecuter)*/;
                        var xin = env.User.GetUserXin(env.QueryExecuter);

                        var tbLandApplications = new TbLandApplications();
                        tbLandApplications.AddFilter(t => t.flId, env.Args.AppId);

                        if (!isInternal)
                        {
                            var or = new LogicGrouper(GroupOperator.Or)
                                .AddFilter(tbLandApplications.flApplicantXin, ConditionOperator.Equal, xin)
                                .AddFilter(tbLandApplications.flCompetentOrgBin, ConditionOperator.Equal, xin);
                            tbLandApplications.AddLogicGrouper(or);
                        }
                        if (tbLandApplications.Count(env.QueryExecuter) > 0)
                        {
                            return new OkResult();
                        }
                        return new RedirectResult(new AccessDeniedException("Подавать заявления могут только юридические и физические лица!"));
                    })
                    .Tasks(t => {
                        t.AddIfValid(Actions.Download, "Скачать");
                        t.AddIfValid(Actions.Accept, "Принять");
                        t.AddIfValid(Actions.Reject, "Отклонить");
                    })
                    .OnRendering(re => {
                        GetAppView(new TbLandApplications().GetAppModel(re.Args.AppId, re.QueryExecuter), re);
                    })
                    ;
                })
                .OnAction(Actions.Download, c => {
                    return c
                    .IsValid(env =>
                    {
                        var isInternal = (!env.User.IsExternalUser() && !env.User.IsGuest());
                        var isUserRegistrator = env.User.HasRole("TRADERESOURCES-Земельные ресурсы-Создание приказов", env.QueryExecuter)/*env.User.HasCustomRole("landobjects", "appLandEdit", env.QueryExecuter) || env.User.HasCustomRole("landobjects", "appLandView", env.QueryExecuter)*/;
                        var xin = env.User.GetUserXin(env.QueryExecuter);

                        var tbLandApplications = new TbLandApplications();
                        tbLandApplications.AddFilter(t => t.flId, env.Args.AppId);

                        if (!isInternal)
                        {
                            var or = new LogicGrouper(GroupOperator.Or)
                                .AddFilter(tbLandApplications.flApplicantXin, ConditionOperator.Equal, xin)
                                .AddFilter(tbLandApplications.flCompetentOrgBin, ConditionOperator.Equal, xin);
                            tbLandApplications.AddLogicGrouper(or);
                        }
                        if (tbLandApplications.Count(env.QueryExecuter) > 0)
                        {
                            return new OkResult();
                        }
                        return new RedirectResult(new AccessDeniedException("Подавать заявления могут только юридические и физические лица!"));
                    })
                    .OnRendering(re => {
                        var pdfData = new TbLandApplications()
                            .AddFilter(t => t.flId, re.Args.AppId)
                            .SelectScalar(t => t.flPdf, re.QueryExecuter);
                        re.Redirect.SetRedirectToFile(pdfData, $"№{re.Args.AppId}.pdf", "application/pdf");
                    })
                    ;
                })
                .OnAction(Actions.Reject, c => {
                    return c
                    .IsValid(env =>
                    {
                        var isInternal = (!env.User.IsExternalUser() && !env.User.IsGuest());
                        var isUserRegistrator = env.User.HasRole("TRADERESOURCES-Земельные ресурсы-Создание приказов", env.QueryExecuter)/*env.User.HasCustomRole("landobjects", "appLandEdit", env.QueryExecuter) || env.User.HasCustomRole("landobjects", "appLandView", env.QueryExecuter)*/;
                        var xin = env.User.GetUserXin(env.QueryExecuter);

                        var tbLandApplications = new TbLandApplications().GetAppModel(env.Args.AppId, env.QueryExecuter);
                        
                        if (tbLandApplications.flStatus == ApplicationStatuses.Сохранена && ((isUserRegistrator && tbLandApplications.flCompetentOrgBin == xin) || isInternal))
                        {
                            return new OkResult();
                        }
                        return new RedirectResult(new AccessDeniedException("Подавать заявления могут только юридические и физические лица!"));
                    })
                    .OnRendering(re => {
                        re.Form.AddSubmitButton("reject", re.T("Отказать в рассмотрении"), cssClass: "btn btn-primary btn-sm mr-1 mb-1");
                        var tbApps = new TbLandApplications();
                        tbApps.flRejectReason.RenderCustom(re.Form, re);
                        GetAppView(new TbLandApplications().GetAppModel(re.Args.AppId, re.QueryExecuter), re);
                    })
                    .OnValidating(ve => {
                        var tbApps = new TbLandApplications();
                        tbApps.flRejectReason
                            .Required()
                            .Validate(ve);

                        if (ve.IsValid)
                        {
                            if (string.IsNullOrEmpty(tbApps.flRejectReason.GetVal(ve)))
                            {
                                ve.AddError(tbApps.flRejectReason.FieldName, ve.T("Поле обязательно для заполнения"));
                            }
                        }
                    })
                    .OnProcessing(pe => {
                        var tbApps = new TbLandApplications();
                        tbApps.AddFilter(t => t.flId, pe.Args.AppId);

                        var reason = tbApps.flRejectReason.GetVal(pe);

                        tbApps.Update()
                        .SetT(t => t.flRejectDate, pe.QueryExecuter.GetDateTime("dbAgreements"))
                        .SetT(t => t.flRejectReason, reason)
                        .SetT(t => t.flStatus, LandSource.References.Applications.ApplicationStatuses.Отклонена)
                        .Execute(pe.QueryExecuter);

                        pe.Redirect.SetRedirect(ModuleName, MenuName, new LandApplicationsArgs() { AppId = pe.Args.AppId, MenuAction = "view" });
                        pe.SetPostbackMessage("Заявление отклонено");
                    })
                    ;
                })
                .OnAction(Actions.Accept, c => {
                    return c
                    .IsValid(env =>
                    {
                        var isInternal = (!env.User.IsExternalUser() && !env.User.IsGuest());
                        var isUserRegistrator = env.User.HasRole("TRADERESOURCES-Земельные ресурсы-Создание приказов", env.QueryExecuter)/*env.User.HasCustomRole("landobjects", "appLandEdit", env.QueryExecuter) || env.User.HasCustomRole("landobjects", "appLandView", env.QueryExecuter)*/;
                        var xin = env.User.GetUserXin(env.QueryExecuter);

                        var tbLandApplications = new TbLandApplications().GetAppModel(env.Args.AppId, env.QueryExecuter);

                        if (tbLandApplications.flStatus == ApplicationStatuses.Сохранена && ((isUserRegistrator && tbLandApplications.flCompetentOrgBin == xin) || isInternal))
                        {
                            return new OkResult();
                        }
                        return new RedirectResult(new AccessDeniedException("Подавать заявления могут только юридические и физические лица!"));
                    })
                    .OnRendering(re => {
                        re.Form.AddSubmitButton("accept", re.T("Принять в работу"), cssClass: "btn btn-primary btn-sm mr-1 mb-1");
                        GetAppView(new TbLandApplications().GetAppModel(re.Args.AppId, re.QueryExecuter), re);
                    })
                    .OnProcessing(pe => {
                        pe.Redirect.SetRedirect(ModuleName, nameof(MnuLandObjectOrderBase), new LandObjectOrderQueryArgs() { AppId = pe.Args.AppId, RevisionId = -1, MenuAction = "create-new" });
                        pe.SetPostbackMessage("Для того, чтобы принять заявление, создайте приказ на создание объекта земельных ресурсов");
                    })
                    ;
                })
                ;
        }



        public class Actions {
            public const string
            Create = "create",
            View = "view",
            Download = "download",
            Accept = "accept",
            Reject = "reject";
        }

        public void GetAppView(LandApplicationModel model, RenderActionEnv<LandApplicationsArgs> re)
        {
            var tbApps = new TbLandApplications();
            var card = new Card("Заявление");
            tbApps.flId.RenderCustomT(card, re, model.flId, readOnly: true);
            tbApps.flGuid.RenderCustomT(card, re, model.flGuid, readOnly: true);
            if (model.flLandObjectId.HasValue)
            {
                tbApps.flLandObjectId.RenderCustomT(card, re, model.flLandObjectId, readOnly: true);
            }
            tbApps.flStatus.RenderCustom(card, re, model.flStatus, readOnly: true);
            tbApps.flCompetentOrgBin.RenderCustomT(card, re, model.flCompetentOrgBin, readOnly: true);
            tbApps.flRegDate.RenderCustomT(card, re, model.flRegDate, readOnly: true);
            tbApps.flSignSendDate.RenderCustomT(card, re, model.flSignSendDate, readOnly: true);
            if (model.flStatus == LandSource.References.Applications.ApplicationStatuses.Принята)
            {
                tbApps.flAcceptDate.RenderCustomT(card, re, model.flAcceptDate, readOnly: true);
            }
            if (model.flStatus == LandSource.References.Applications.ApplicationStatuses.Отклонена)
            {
                tbApps.flRejectDate.RenderCustomT(card, re, model.flRejectDate, readOnly: true);
                tbApps.flRejectReason.RenderCustomT(card, re, model.flRejectReason, readOnly: true);
            }
            tbApps.flRegByUserId.RenderCustomT(card, re, model.flRegByUserId, readOnly: true);
            tbApps.flApplicantType.RenderCustom(card, re, model.flApplicantType, readOnly: true);
            tbApps.flApplicantXin.RenderCustomT(card, re, model.flApplicantXin, readOnly: true);
            tbApps.flApplicantName.RenderCustomT(card, re, model.flApplicantName, readOnly: true);
            tbApps.flApplicantAddress.RenderCustomT(card, re, model.flApplicantAddress, readOnly: true);
            tbApps.flApplicantPhoneNumber.RenderCustomT(card, re, model.flApplicantPhoneNumber, readOnly: true);
            tbApps.flCountry.RenderCustomT(card, re, model.flCountry, readOnly: true);
            tbApps.flRegion.RenderCustomT(card, re, model.flRegion, readOnly: true);
            tbApps.flDistrict.RenderCustomT(card, re, model.flDistrict, readOnly: true);
            tbApps.flAddress.RenderCustomT(card, re, model.flAddress, readOnly: true);
            tbApps.flArea.RenderCustomT(card, re, model.flArea, readOnly: true);

            var groupbox = new Accordion(re.T("Геометрия"));
            groupbox.CssClass += " container-fluid";
            card.AddComponent(groupbox);
            var row = new GridRow();
            groupbox.AddComponent(row);
            row.AddComponent(new ObjectGeometryViewerComponent(model.flWKT, "col-md-6"));
            row.AddComponent(new ObjectCoordsViewerComponent(model.flCoords, "col-md-6"));

            tbApps.flUsageAim.RenderCustomT(card, re, model.flUsageAim, readOnly: true);
            tbApps.flUsageAimText.RenderCustomT(card, re, model.flUsageAimText, readOnly: true);

            re.Form.AddComponent(card);

            if (!re.User.IsExternalUser() || model.flApplicantXin == re.User.GetUserXin(re.QueryExecuter))
            {
                var contentCard = new Accordion("Содержание");
                contentCard.AddComponent(new ApplicationDoc(model, re.RequestContext).GetPanelHtml());
                re.Form.AddComponent(contentCard);
            }
        }

        public class ApplicationDoc : DefaultDocTemplate {
            public LandApplicationModel ApplicationData { get; set; }
            public IYodaRequestContext RequestContext { get; set; }

            public override string[] GetUIPackages()
            {
                return new[] { "default-doc-package", "yoda-geom-renderer" };
            }

            public ApplicationDoc(LandApplicationModel applicationData, IYodaRequestContext requestContext)
            {
                ApplicationData = applicationData;
                RequestContext = requestContext;
            }
            public override string GetContent()
            {
                var template = string.Empty;

                string signatureInfo = string.Empty;

                if (!string.IsNullOrEmpty(ApplicationData.flCertInfo))
                {
                    signatureInfo = "Дата и время подписи: " + ApplicationData.flSignSendDate.ToString("dd.MM.yyyy HH:mm") + "; ";
                    var certParts = CertificateInfoExtractor.GetCertificateInfo(Convert.FromBase64String(ApplicationData.flCertInfo));
                    signatureInfo = certParts.Except(certParts.Where(x => (x.Code.In("SERIALNUMBER", "E")))).Aggregate(signatureInfo, (current, part) => current + (part.Title + ": " + part.Value + "; "));

                    template += new DocumentBuilder()
                            .Doc(doc =>
                            {
                                var qrUrl = RequestContext.GetUrlHelper().YodaAction(nameof(RegistersModule), nameof(MnuCheckLandApplication), new MnuCheckLandApplicationArgs()
                                {
                                    flApplicantXin = ApplicationData.flApplicantXin,
                                    flApplicationId = ApplicationData.flId,
                                    MenuAction = MnuCheckLandApplication.Actions.Check
                                }, urlWithSchema: true);
                                var qrCodeImageBase64 = Convert.ToBase64String(QrGenerator.GenerateQr(qrUrl));
                                var linkUrl = RequestContext.GetUrlHelper().YodaAction(nameof(RegistersModule), nameof(MnuCheckLandApplication), null, urlWithSchema: true);
                                doc.AddSection(s => s.Body(b => {
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
"
)
                                    .BrTag()
                                    .Table(new[] { new DocTableCol("attr", null, 30, cellCssClass: "align-top border text-nowrap", headerCellCssClass: "d-none"), new DocTableCol("value", null, 0, cellCssClass: "align-top border", headerCellCssClass: "d-none") }, new[] {
                                        new { attr = "Номер документа", value = $"№-{ApplicationData.flId:0000000000}" },
                                        new { attr = "Id заявки", value = ApplicationData.flId.ToString() },
                                        new { attr = "Статус", value = "Подписан" },
                                        new { attr = "Дата создания", value = ApplicationData.flRegDate.ToString("dd.MM.yyyy HH:mm") },
                                        new { attr = "Подпись заявителя", value = signatureInfo },
                                    }, tableCssClass: "wide-table table-td-p-10");



                                }));
                            }).Build();
                }

                template += new DocumentBuilder()
                            .Doc(doc =>
                            {
                                doc.AddSection(s => s.Body(b => {
                                    b
                                    .Div($"ПРЕДЛОЖЕНИЕ №-{ApplicationData.flId:0000000000} О ВЫНЕСЕНИИ СВОБОДНОГО ЗЕМЕЛЬНОГО УЧАСТКА НА ТОРГИ", new CssClass("text-center font-weight-bold"))
                                    .BrTag()
                                    .Paragraph($"В { new GrObjectSearchCollection().GetItem(ApplicationData.flCompetentOrgBin, RequestContext).ObjectData.NameRu }");

                                    switch (ApplicationData.flApplicantType)
                                    {
                                        case ApplicantType.Jur:
                                            b.Paragraph($@"от {ApplicationData.flApplicantInfo.JurInfo.CorpName}</b></p> ")
                                            .BrTag();
                                            break;
                                        case ApplicantType.Fiz:
                                            b.Paragraph($@"от {ApplicationData.flApplicantInfo.Name}</b></p>")
                                            .BrTag();
                                            break;
                                    }

                                    var refAr = RefAr.Instance;
                                    b
                                    .Paragraph($"Прошу рассмотреть свободный участок, расположенный по адресу: {refAr.Search(ApplicationData.flCountry).Text}, {refAr.Search(ApplicationData.flRegion).Text}, {refAr.Search(ApplicationData.flDistrict).Text}, {ApplicationData.flAddress}")
                                    .Paragraph($"В границах: ")
                                    .Table(new[] { new DocTableCol("x", "Широта", 50), new DocTableCol("y", "Долгота", 50) }, ApplicationData.flCoords, tableCssClass: "wide-table")
                                    .BrTag();

                                    var percent = 0;
                                    var strokeWidth = 1;
                                    var panel = new Panel()
                                    {
                                        Attributes = new Dictionary<string, object>() {
                                                { "style",
                                                    "margin: 0 auto; width: 80%"
                                                }
                                            }
                                    };
                                    for (var rows = 1; rows <= 3; rows++)
                                    {
                                        var row = new GridRow().AppendTo(panel);
                                        for (var cols = 1; cols <= 3; cols++)
                                        {
                                            percent = percent > 100 ? 100 : percent;
                                            if (rows == 1 && cols == 1)
                                            {
                                                new GridCol("col").AppendTo(row).Append(new GeomRenderer(ApplicationData.flWKT, GeomType.Geometry, new GeomRenderer.GeometryStyle(4326, background: GeomRenderer.MapBackgrounds.google, height: 200, fillColor: "#0acf9710", strokeColor: "#0acf97", strokeWidth: strokeWidth, scale: 2), readOnly: true, geomViewExtent: new GeomRenderer.ViewExtent(percent))).Append(new Br());
                                                cols++;
                                            }
                                            new GridCol("col").AppendTo(row).Append(new GeomRenderer(ApplicationData.flWKT, GeomType.Geometry, new GeomRenderer.GeometryStyle(4326, background: GeomRenderer.MapBackgrounds.osm, height: 200, fillColor: "#0acf9710", strokeColor: "#0acf97", strokeWidth: strokeWidth, scale: 2 ), readOnly: true, geomViewExtent: new GeomRenderer.ViewExtent(percent))).Append(new Br());
                                            percent *= 2;
                                            percent = percent == 0 ? 1 : percent;
                                            strokeWidth = 6;
                                        }
                                    }

                                    b.Html(panel.ToHtmlString(null).Value);

                                    var RefUpgsUsageAim = RefUpgsLandObjectsUsageAim.GetReference(RequestContext.QueryExecuter);
                                    b
                                    .Paragraph($"Планируемое целевое использование земельного участка: <b>{ RefUpgsUsageAim.Search(ApplicationData.flUsageAim).Text.Text }</b> ({ApplicationData.flUsageAimText})")
                                    .BrTag();

                                    switch (ApplicationData.flApplicantType)
                                    {
                                        case ApplicantType.Jur:
                                            b.Paragraph($@"Наименование:  <b>{ApplicationData.flApplicantInfo.JurInfo.CorpName}</b>");
                                            b.Paragraph($@"Бизнес-идентификационный номер: <b>{ApplicationData.flApplicantInfo.Xin}</b>");
                                            b.Paragraph($@"Фамилия, имя и отчество (при его наличии) руководителя: <b>{ApplicationData.flApplicantInfo.JurInfo.FirstPerson}</b>");
                                            b.Paragraph($@"Адрес: <b>{ApplicationData.flApplicantInfo.Address}</b>");
                                            b.Paragraph($@"Номер телефона: {ApplicationData.flApplicantInfo.PhoneNumber}</b>");
                                            b.BrTag();

                                            break;
                                        case ApplicantType.Fiz:
                                            b.Paragraph($@"Фамилия, имя и отчество (при его наличии): <b>{ApplicationData.flApplicantInfo.Name}</b>");
                                            b.Paragraph($@"Индивидуальный идентификационный номер: <b>{ApplicationData.flApplicantInfo.Xin}</b>");
                                            b.Paragraph($@"Документ, удостоверяющий личность:");
                                            b.Paragraph($@"Номер: <b>{ApplicationData.flApplicantInfo.IdentityDocInfo.Number}</b>");
                                            b.Paragraph($@"Кем выдано: <b>{ApplicationData.flApplicantInfo.IdentityDocInfo.IssuerOrg}</b>");
                                            b.Paragraph($@"Дата выдачи: <b>{(ApplicationData.flApplicantInfo.IdentityDocInfo.DocDate.HasValue ? ApplicationData.flApplicantInfo.IdentityDocInfo.DocDate.Value.ToString("dd.MM.yyyy HH:mm") : "-")}</b>");
                                            b.Paragraph($@"Адрес: <b>{ApplicationData.flApplicantInfo.Address}</b> ");
                                            b.Paragraph($@"Номер телефона: <b>{ApplicationData.flApplicantInfo.PhoneNumber}</b>");
                                            b.BrTag();
                                            break;
                                    }

                                    b
                                    .Paragraph($"Земельному участку присвоен уникальный идентификационный номер: <b>{ApplicationData.flGuid}</b>")
                                    .Paragraph($"Площадь: <b>{ApplicationData.flArea:N} га</b>")
                                    .Paragraph($"Даю согласие на использование сведений, указанных в данном предложении, в том числе, составляющих охраняемую законом тайну, а также на сбор, обработку персональных данных.");

                                    if (!string.IsNullOrEmpty(ApplicationData.flCertInfo))
                                    {
                                        b
                                        .Paragraph($"Подписано и отправлено пользователем в {ApplicationData.flSignSendDate.ToString("dd.MM.yyyy HH:mm")}")
                                        .BrTag()
                                        .Paragraph($"Данные из электронной цифровой подписи (далее – ЭЦП)")
                                        .Paragraph(signatureInfo);
                                    }

                                }));
                            }).Build();

                return template;
            }
        }

    }

}
