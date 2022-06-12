using FileStoreInterfaces;
using LandSource.QueryTables.LandObject;
using LandSource.QueryTables.Trades;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using PaymentsApi.Client;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using TradeResourcesPlugin.Helpers.Agreements;
using UsersResources;
using UsersResources.Refs;
using WalletApi.Client;
using Yoda;
using Yoda.Application;
using Yoda.Application.Helpers;
using Yoda.Interfaces;
using Yoda.Interfaces.Forms;
using Yoda.Interfaces.Forms.Components;
using Yoda.Interfaces.Helpers;
using YodaApp.UsersResources.QueryTables.V2;
using YodaHelpers;
using YodaHelpers.ActionMenus;
using YodaHelpers.Components;
using YodaHelpers.Fields;
using YodaQuery;
using static TradeResourcesPlugin.Helpers.Agreements.PaymentHelper;
using static TradeResourcesPlugin.Helpers.DefaultAgrTemplate;
using PaymentItemModel = TradeResourcesPlugin.Helpers.Agreements.PaymentItemModel;
using PaymentModel = TradeResourcesPlugin.Helpers.Agreements.PaymentModel;
using PaymentStatus = TradeResourcesPlugin.Helpers.Agreements.PaymentStatus;

namespace TradeResourcesPlugin.Helpers
{
    public class MnuDefaultAgrWizard : MnuActionsExt<DefaultAgrTemplateArgs>
    {
        public MnuDefaultAgrWizard(string moduleName, string menuName, string menuTitle) : base(moduleName, menuName, menuTitle)
        {
            AsCallback();
        }
        public class Actions
        {
            public const string
                GoToObject = "object",
                GoToTrade = "trade",
                GoToEtp = "etp",
                Create = "create",
                Edit = "edit",
                View = "view",
                Download = "download",
                DownloadWithSigns = "download-signs",
                Delete = "delete",
                SendToApproval = "send-to-approval",
                SendToCorrection = "send-to-correction",
                ApproveWinner = "approve-by-winner",
                SignSeller = "sign-by-seller",
                SignWinner = "sign-by-winner",
                //LinkPaymentItems = "link-payment-items",
                Extend = "extend"
                ;
        }

        protected override string GetDefaultActionName()
        {
            return Actions.View;
        }

        public override void Configure(ActionConfig<DefaultAgrTemplateArgs> config)
        {
            config
                .OnAction(Actions.View, action => { return action
                    .IsValid(env => {
                        if (env.Args.AgreementId == 0)
                            throw new NotImplementedException("AgreementId is Null");

                        var isInternal = (!env.User.IsExternalUser() && !env.User.IsGuest());

                        var canView = AgreementHelper.GetEmptyAgreementModel(env).HasAccessToView(env);

                        if (!canView && !isInternal)
                            return new RedirectResult(new AccessDeniedException("No access"));

                        return new OkResult();
                    })
                    .Tasks(t => t
                        .AddIfValid(Actions.GoToObject, "Перейти к объекту")
                        .AddIfValid(Actions.GoToTrade, "Перейти к торгу")
                        .AddIfValid(Actions.GoToEtp, "Перейти к ЭТП")
                        .AddIfValid(Actions.Edit, "Редактировать")
                        .AddIfValid(Actions.SendToApproval, "Отправить на согласование (этим вы подтвердите согласие)")
                        .AddIfValid(Actions.SendToCorrection, "Отправить на доработку")
                        .AddIfValid(Actions.ApproveWinner, "Подтвердить согласие")
                        .AddIfValid(Actions.SignSeller, "Подписать (продавец)")
                        .AddIfValid(Actions.SignWinner, "Подписать (победитель)")
                        .AddIfValid(Actions.Download, "Скачать")
                        .AddIfValid(Actions.DownloadWithSigns, "Скачать (с подписями)")
                        .AddIfValid(Actions.Delete, "Удалить")
                        .AddIfValid(Actions.Extend, "Продлить период подписания")
                    )
                    .OnRendering(async re => {
                        re.Form.AddComponent(AgreementHelper.GetDataPanelHtml(re));
                        //var isSeller = AgreementHelper.GetEmptyAgreementModel(re).IsSeller(re);
                        //var isInternal = !re.User.IsGuest() && !re.User.IsExternalUser();
                        var agrStatus = AgreementHelper.GetAgreementStatus(re.Args.AgreementId, re.QueryExecuter);
                        if (agrStatus == AgreementStatuses.Signed.ToString())
                        {
                            AgreementHelper.GetIfNeedPaymentPanelHtml(re);
                        }
                        re.Form.AddComponent(AgreementHelper.GetStoryPanelHtml(re));
                        re.Form.AddComponent(new Accordion("Содержание").Append(AgreementHelper.GetAgreementHtmlContent(re.Args.AgreementId, re.QueryExecuter)));
                    });
                })
                .OnAction(Actions.GoToObject, action => {
                    return action
                    .IsValid(env => {
                        if (env.Args.AgreementId == 0)
                            throw new NotImplementedException("AgreementId is Null");

                        var isInternal = (!env.User.IsExternalUser() && !env.User.IsGuest());

                        var canView = AgreementHelper.GetEmptyAgreementModel(env).HasAccessToView(env);

                        if (!canView && !isInternal)
                            return new RedirectResult(new AccessDeniedException("No access"));

                        return new OkResult();
                    })
                    .OnRendering(re => {
                        var redirect = AgreementHelper.GetEmptyAgreementModel(re).GetLinkToObject(re);
                        re.Redirect.SetRedirect(redirect.module, redirect.action, redirect.routeValues, redirect.project);
                    });
                })
                .OnAction(Actions.GoToTrade, action => {
                    return action
                    .IsValid(env => {
                        if (env.Args.AgreementId == 0)
                            throw new NotImplementedException("AgreementId is Null");

                        var isInternal = (!env.User.IsExternalUser() && !env.User.IsGuest());

                        var canView = AgreementHelper.GetEmptyAgreementModel(env).HasAccessToView(env);

                        if (!canView && !isInternal)
                            return new RedirectResult(new AccessDeniedException("No access"));

                        return new OkResult();
                    })
                    .OnRendering(re => {
                        var redirect = AgreementHelper.GetEmptyAgreementModel(re).GetLinkToTrade(re);
                        re.Redirect.SetRedirect(redirect.module, redirect.action, redirect.routeValues, redirect.project);
                    });
                })
                .OnAction(Actions.GoToEtp, action => {
                    return action
                    .IsValid(env => {
                        if (env.Args.AgreementId == 0)
                            throw new NotImplementedException("AgreementId is Null");

                        var isInternal = (!env.User.IsExternalUser() && !env.User.IsGuest());

                        var canView = AgreementHelper.GetEmptyAgreementModel(env).HasAccessToView(env);

                        if (!canView && !isInternal)
                            return new RedirectResult(new AccessDeniedException("No access"));

                        return new OkResult();
                    })
                    .OnRendering(re => {
                        var redirect = AgreementHelper.GetEmptyAgreementModel(re).GetLinkToEtp(re);
                        re.Redirect.SetRedirectToUrl(redirect);
                    });
                })
                .OnAction(Actions.Download, action => {
                    return action
                    .IsValid(env => {
                        if (env.Args.AgreementId == 0)
                            throw new NotImplementedException("AgreementId is Null");

                        var isInternal = (!env.User.IsExternalUser() && !env.User.IsGuest());

                        var canView = AgreementHelper.GetEmptyAgreementModel(env).HasAccessToView(env);

                        if (!canView && !isInternal)
                            return new RedirectResult(new AccessDeniedException("No access"));

                        return new OkResult();
                    })
                    .OnRendering(re => {

                        var pdfData = new TbAgreementPdfs()
                            .AddFilter(t => t.flAgreementId, re.Args.AgreementId)
                            .SelectScalar(t => t.flPdf, re.QueryExecuter);
                        re.Redirect.SetRedirectToFile(pdfData, $"№{re.Args.AgreementId}.pdf", "application/pdf");

                    });
                })
                .OnAction(Actions.DownloadWithSigns, action => {
                    return action
                    .IsValid(env => {
                        if (env.Args.AgreementId == 0)
                            throw new NotImplementedException("AgreementId is Null");

                        var isInternal = (!env.User.IsExternalUser() && !env.User.IsGuest());

                        var canView = AgreementHelper.GetEmptyAgreementModel(env).HasAccessToView(env);
                        var agrStatus = AgreementHelper.GetAgreementStatus(env.Args.AgreementId, env.QueryExecuter);

                        if (!((canView || isInternal) && agrStatus == AgreementStatuses.Signed.ToString()))
                            return new RedirectResult(new AccessDeniedException("No access"));

                        return new OkResult();
                    })
                    .OnRendering(re => {

                        var pdfData = new TbAgreementPdfs()
                            .AddFilter(t => t.flAgreementId, re.Args.AgreementId)
                            .SelectScalar(t => t.flPdfWithSigns, re.QueryExecuter);
                        re.Redirect.SetRedirectToFile(pdfData, $"№{re.Args.AgreementId} с подписями.pdf", "application/pdf");

                    });
                })
                .OnAction(Actions.Delete, action =>
                {
                    return action
                    .IsValid(env =>
                    {
                        if (env.Args.AgreementId == 0)
                            throw new NotImplementedException("AgreementId is Null");

                        var canDelete = AgreementHelper.GetEmptyAgreementModel(env).HasAccessToCreate(env);
                        var isAgreementCreator = AgreementHelper.GetEmptyAgreementModel(env).IsAgreementCreator(env);
                        var agrStatus = AgreementHelper.GetAgreementStatus(env.Args.AgreementId, env.QueryExecuter);

                        if (!(canDelete && isAgreementCreator && !agrStatus.In( AgreementStatuses.Signed.ToString(), AgreementStatuses.Deleted.ToString(), AgreementStatuses.Canceled.ToString(), AgreementStatuses.SignedSeller.ToString(), AgreementStatuses.SignedWinner.ToString() )))
                            return new RedirectResult(new AccessDeniedException("No access"));

                        return new OkResult();
                    })
                    .OnRendering(re =>
                    {
                        re.Form.AddComponent(AgreementHelper.GetDataPanelHtml(re));
                        re.Form.AddSubmitButton("Удалить");
                    })
                    .OnProcessing(pe =>
                    {
                        var tbAgreements = new TbAgreements();
                        tbAgreements.AddFilter(t => t.flAgreementId, pe.Args.AgreementId);
                        var agreementRevisionId = AgreementHelper.GetAgreementActiveRevisionId(pe.Args.AgreementId, pe.QueryExecuter);
                        var tbAgreementModels = new TbAgreementModels();
                        tbAgreementModels.AddFilter(t => t.flAgreementRevisionId, agreementRevisionId);

                        var tbAgreementsUpdate = tbAgreements.Update()
                            .SetT(t => t.flAgreementStatus, AgreementStatuses.Deleted)
                        ;
                        var tbAgreementModelsUpdate = tbAgreementModels.Update()
                            .SetT(t => t.flAgreementStatus, AgreementStatuses.Deleted)
                        ;

                        using (var trans = pe.QueryExecuter.BeginTransaction("dbAgreements"))
                        {
                            tbAgreementsUpdate.Execute(pe.QueryExecuter, trans);
                            tbAgreementModelsUpdate.Execute(pe.QueryExecuter, trans);
                            trans.Commit();
                        }

                        pe.Args.MenuAction = Actions.View;
                        pe.Redirect.SetRedirect(ModuleName, MenuName, pe.Args);
                    });
                })
                .OnAction(Actions.Create, action =>
                {
                    return action
                    .IsValid(env =>
                    {
                        if (env.Args.AgreementType == null)
                            throw new NotImplementedException("AgreementType is Null");
                        if (string.IsNullOrEmpty(env.Args.WinnerXin))
                            throw new NotImplementedException("WinnerXin is Null");

                        var canCreate = AgreementHelper.GetEmptyAgreementModel(env).HasAccessToCreate(env);
                        var agreementExists = AgreementHelper.AgreementExists(env);

                        if (!canCreate)
                            return new RedirectResult(new AccessDeniedException("No access"));
                        if (agreementExists)
                            return new RedirectResult(new AccessDeniedException("Agreement exists"));

                        return new OkResult();
                    })
                    .Wizard(wizard => wizard
                        .Args(env => env.Args)
                        .Model(env =>
                        {
                            return new AgreementWizardModel(JsonConvert.SerializeObject(AgreementHelper.GetAgreementModels(env)), env.Args.AgreementType);
                        })
                        .CancelBtn("Отмена", env => new ActionRedirectData(ModuleName, MenuName, new DefaultAgrTemplateArgs { AgreementId = env.Args.AgreementId, MenuAction = Actions.View }))
                        .FinishBtn("Сохранить")
                        .Step("Создать договор", step => step
                            .OnRendering(re =>
                            {
                                var Models = AgreementHelper.GetAgreementTypeModel(re.Model.AgreementType).DeserializeModels(re.Model.AgreementModelsJson);
                                re.Panel.AddComponent(Models.RenderInputs());
                            })
                            .OnValidating(ve =>
                            {
                                var Models = AgreementHelper.GetAgreementTypeModel(ve.Model.AgreementType).DeserializeModels(ve.Model.AgreementModelsJson);
                                Models.ValidateInputs(ve.Env.FormCollection, out var errors);
                                errors.Each(x => ve.Env.AddError(x.Key, x.Value));
                            })
                            .OnProcessing(pe =>
                            {
                                var Models = AgreementHelper.GetAgreementTypeModel(pe.Model.AgreementType).DeserializeModels(pe.Model.AgreementModelsJson);
                                Models.SetInputsValues(pe.Env.FormCollection);
                                pe.Model.AgreementModelsJson = JsonConvert.SerializeObject(Models);
                            })
                        )
                        .Step("Просмотр", step => step
                            .OnRendering(re =>
                            {
                                var Models = AgreementHelper.GetAgreementTypeModel(re.Model.AgreementType).DeserializeModels(re.Model.AgreementModelsJson).SetAgreementNumber(0);
                                re.Panel.AddComponent(Models.GetPanelHtml());
                            })
                            .OnProcessing(async pe =>
                            {
                                var tbAgreements = new TbAgreements();
                                var newAgreementId = tbAgreements.flAgreementId.GetNextId(pe.Env.QueryExecuter);
                                var createDate = pe.Env.QueryExecuter.GetDateTime("dbAgreements");

                                var Models = AgreementHelper.GetAgreementTypeModel(pe.Model.AgreementType).DeserializeModels(pe.Model.AgreementModelsJson).SetAgreementNumber(newAgreementId);

                                var tbAgreementsInsert = tbAgreements.Insert()
                                    .Set(t => t.flAgreementId, newAgreementId)
                                    .Set(t => t.flAgreementNumber, Models.GetAgreementNumber())
                                    .Set(t => t.flAgreementRevisionId, newAgreementId)
                                    .Set(t => t.flAgreementType, pe.Args.AgreementType)
                                    .SetT(t => t.flAgreementStatus, AgreementStatuses.Saved)
                                    .Set(t => t.flAgreementCreateDate, createDate)
                                    .Set(t => t.flObjectId, pe.Args.ObjectId)
                                    .Set(t => t.flObjectType, pe.Args.ObjectType)
                                    .Set(t => t.flTradeId, pe.Args.TradeId)
                                    .Set(t => t.flTradeType, pe.Args.TradeType)
                                    .Set(t => t.flAuctionId, pe.Args.AuctionId)
                                    .Set(t => t.flSellerBin, string.IsNullOrEmpty(pe.Args.SellerBin) ? pe.Env.User.GetUserBin(pe.Env.QueryExecuter) : pe.Args.SellerBin)
                                    .Set(t => t.flAgreementCreatorBin, pe.Env.User.GetUserBin(pe.Env.QueryExecuter))
                                    .Set(t => t.flWinnerXin, pe.Args.WinnerXin)
                                    ;
                                var tbAgreementModelsInsert = new TbAgreementModels().Insert()
                                    .Set(t => t.flAgreementId, newAgreementId)
                                    .Set(t => t.flAgreementRevisionId, newAgreementId)
                                    .SetT(t => t.flAgreementStatus, AgreementStatuses.Saved)
                                    .Set(t => t.flDateTime, createDate)
                                    .SetT(t => t.flModels, JsonConvert.SerializeObject(Models))
                                    .SetT(t => t.flContent, Models.GetPanelHtml().ToHtmlString(null).Value)
                                    ;

                                var args = new DefaultDocPdfMnuArgs()
                                {
                                    ContentCache = $"{MenuName}-{newAgreementId}-{createDate.ToString("dd-MM-yyyy-hh-mm")}",
                                    ContentUiPackagesCache = $"UI-Packages-{MenuName}-{newAgreementId}-{createDate.ToString("dd-MM-yyyy-hh-mm")}"
                                };

                                pe.Env.RequestContext.Cache.Set(args.ContentCache, Models.GetPdfContent());
                                pe.Env.RequestContext.Cache.Set(args.ContentUiPackagesCache, Models.First().GetUIPackages());

                                using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { ExecutablePath = pe.Env.RequestContext.Configuration["ChromiumPath"], Headless = true });
                                using var page = await browser.NewPageAsync();
                                await page.GoToAsync(pe.Env.RequestContext.GetUrlHelper().YodaAction(ModuleName, nameof(DefaultDocPdfMnu), args, urlWithSchema: true));
                                Thread.Sleep(3000);
                                var pdfData = await page.PdfDataAsync(new PdfOptions()
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
                                browser.CloseAsync();

                                pe.Env.RequestContext.Cache.Remove(args.ContentCache);
                                pe.Env.RequestContext.Cache.Remove(args.ContentUiPackagesCache);

                                var tbAgreementPdfsInsert = new TbAgreementPdfs().Insert()
                                    .SetT(t => t.flAgreementId, newAgreementId)
                                    .Set(t => t.flPdf, pdfData)
                                    ;

                                using (var trans = pe.Env.QueryExecuter.BeginTransaction("dbAgreements"))
                                {
                                    tbAgreementsInsert.Execute(pe.Env.QueryExecuter, trans);
                                    tbAgreementModelsInsert.Execute(pe.Env.QueryExecuter, trans);
                                    tbAgreementPdfsInsert.Execute(pe.Env.QueryExecuter, trans);

                                    trans.Commit();
                                }

                                pe.Args.AgreementId = newAgreementId;

                                pe.Args.MenuAction = Actions.View;

                                pe.Env.Redirect.SetRedirect(ModuleName, MenuName, pe.Args);

                            })
                        ).Build()
                    );
                })
                .OnAction(Actions.Edit, action =>
                {
                    return action
                    .IsValid(env =>
                    {
                        if (env.Args.AgreementId == 0)
                            throw new NotImplementedException("AgreementId is Null");

                        var canEdit = AgreementHelper.GetEmptyAgreementModel(env).HasAccessToCreate(env);
                        var isAgreementCreator = AgreementHelper.GetEmptyAgreementModel(env).IsAgreementCreator(env);
                        var agrStatus = AgreementHelper.GetAgreementStatus(env.Args.AgreementId, env.QueryExecuter);

                        if (!(canEdit && isAgreementCreator && agrStatus.In(/*AgreementStatuses.Saved.ToString(),*/ AgreementStatuses.OnCorrection.ToString())))
                            return new RedirectResult(new AccessDeniedException("No access"));

                        return new OkResult();
                    })
                    .Wizard(wizard => wizard
                        .Args(env => env.Args)
                        .Model(env =>
                        {
                            return new AgreementWizardModel(JsonConvert.SerializeObject(AgreementHelper.GetAgreementModels(env)), env.Args.AgreementType);
                        })
                        .CancelBtn("Отмена", env => new ActionRedirectData(ModuleName, MenuName, new DefaultAgrTemplateArgs { AgreementId = env.Args.AgreementId, MenuAction = Actions.View }))
                        .FinishBtn("Сохранить")
                        .Step("Редактировать договор", step => step
                            .OnRendering(re =>
                            {
                                var Models = AgreementHelper.GetAgreementTypeModel(re.Model.AgreementType).DeserializeModels(re.Model.AgreementModelsJson);
                                re.Panel.AddComponent(Models.RenderInputs());
                            })
                            .OnValidating(ve =>
                            {
                                var Models = AgreementHelper.GetAgreementTypeModel(ve.Model.AgreementType).DeserializeModels(ve.Model.AgreementModelsJson);
                                Models.ValidateInputs(ve.Env.FormCollection, out var errors);
                                errors.Each(x => ve.Env.AddError(x.Key, x.Value));
                            })
                            .OnProcessing(pe =>
                            {
                                var Models = AgreementHelper.GetAgreementTypeModel(pe.Model.AgreementType).DeserializeModels(pe.Model.AgreementModelsJson);
                                Models.ValidateInputs(pe.Env.FormCollection, out var errors);
                                Models.SetInputsValues(pe.Env.FormCollection);
                                pe.Model.AgreementModelsJson = JsonConvert.SerializeObject(Models);
                            })
                        )
                        .Step("Просмотр", step => step
                            .OnRendering(re =>
                            {
                                var Models = AgreementHelper.GetAgreementTypeModel(re.Model.AgreementType).DeserializeModels(re.Model.AgreementModelsJson);
                                re.Panel.AddComponent(Models.GetPanelHtml());
                            })
                            .OnProcessing(async pe =>
                            {
                                var Models = AgreementHelper.GetAgreementTypeModel(pe.Model.AgreementType).DeserializeModels(pe.Model.AgreementModelsJson);

                                var tbAgreements = new TbAgreements();
                                tbAgreements.AddFilter(t => t.flAgreementId, pe.Args.AgreementId);
                                var newAgreementRevisionId = tbAgreements.flAgreementId.GetNextId(pe.Env.QueryExecuter);
                                var createDate = pe.Env.QueryExecuter.GetDateTime("dbAgreements");

                                var tbAgreementsUpdate = tbAgreements.Update()
                                    .Set(t => t.flAgreementRevisionId, newAgreementRevisionId)
                                    .SetT(t => t.flAgreementStatus, AgreementStatuses.Saved)
                                ;
                                var tbAgreementModelsInsert = new TbAgreementModels().Insert()
                                    .Set(t => t.flAgreementId, pe.Args.AgreementId)
                                    .Set(t => t.flAgreementRevisionId, newAgreementRevisionId)
                                    .SetT(t => t.flAgreementStatus, AgreementStatuses.Saved)
                                    .Set(t => t.flDateTime, createDate)
                                    .SetT(t => t.flModels, Models)
                                    .SetT(t => t.flContent, Models.GetPanelHtml().ToHtmlString(null).Value)
                                ;



                                var args = new DefaultDocPdfMnuArgs()
                                {
                                    ContentCache = $"{MenuName}-{pe.Args.AgreementId}-{createDate.ToString("dd-MM-yyyy-hh-mm")}",
                                    ContentUiPackagesCache = $"UI-Packages-{MenuName}-{pe.Args.AgreementId}-{createDate.ToString("dd-MM-yyyy-hh-mm")}"
                                };

                                pe.Env.RequestContext.Cache.Set(args.ContentCache, Models.GetPdfContent());
                                pe.Env.RequestContext.Cache.Set(args.ContentUiPackagesCache, Models.First().GetUIPackages());

                                using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { ExecutablePath = pe.Env.RequestContext.Configuration["ChromiumPath"], Headless = true });
                                using var page = await browser.NewPageAsync();
                                await page.GoToAsync(pe.Env.RequestContext.GetUrlHelper().YodaAction(ModuleName, nameof(DefaultDocPdfMnu), args, urlWithSchema: true));
                                Thread.Sleep(3000);
                                var pdfData = await page.PdfDataAsync(new PdfOptions()
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
                                browser.CloseAsync();

                                pe.Env.RequestContext.Cache.Remove(args.ContentCache);
                                pe.Env.RequestContext.Cache.Remove(args.ContentUiPackagesCache);

                                var tbAgreementPdfsUpdate = new TbAgreementPdfs()
                                    .AddFilter(t => t.flAgreementId, pe.Args.AgreementId).Update()
                                    .Set(t => t.flPdf, pdfData)
                                    ;


                                using (var trans = pe.Env.QueryExecuter.BeginTransaction("dbAgreements"))
                                {
                                    tbAgreementsUpdate.Execute(pe.Env.QueryExecuter, trans);
                                    tbAgreementModelsInsert.Execute(pe.Env.QueryExecuter, trans);
                                    tbAgreementPdfsUpdate.Execute(pe.Env.QueryExecuter, trans);
                                    trans.Commit();
                                }

                                pe.Args.MenuAction = Actions.View;
                                pe.Env.Redirect.SetRedirect(ModuleName, MenuName, pe.Args);
                            })
                        ).Build()
                    );
                })
                .OnAction(Actions.SendToApproval, action => {
                    return action
                    .IsValid(env =>
                    {
                        if (env.Args.AgreementId == 0)
                            throw new NotImplementedException("AgreementId is Null");

                        var canCreate = AgreementHelper.GetEmptyAgreementModel(env).HasAccessToCreate(env);
                        var isAgreementCreator = AgreementHelper.GetEmptyAgreementModel(env).IsAgreementCreator(env);
                        var agrStatus = AgreementHelper.GetAgreementStatus(env.Args.AgreementId, env.QueryExecuter);

                        if (!(canCreate && isAgreementCreator && agrStatus == AgreementStatuses.Saved.ToString()))
                            return new RedirectResult(new AccessDeniedException("No access"));

                        return new OkResult();
                    })
                    .OnRendering(re =>
                    {
                        re.Form.AddComponent(AgreementHelper.GetDataPanelHtml(re));
                        re.Form.AddSubmitButton("Отправить на согласование");
                    })
                    .OnProcessing(async pe => {
                        var tbAgreements = new TbAgreements();
                        tbAgreements.AddFilter(t => t.flAgreementId, pe.Args.AgreementId);
                        var agreementRevisionId = AgreementHelper.GetAgreementActiveRevisionId(pe.Args.AgreementId, pe.QueryExecuter);
                        var tbAgreementModels = new TbAgreementModels();
                        tbAgreementModels.AddFilter(t => t.flAgreementRevisionId, agreementRevisionId);

                        var tbAgreementsUpdate = tbAgreements.Update()
                            .SetT(t => t.flAgreementStatus, AgreementStatuses.OnApproval)
                        ;
                        var tbAgreementModelsUpdate = tbAgreementModels.Update()
                            .SetT(t => t.flAgreementStatus, AgreementStatuses.OnApproval)
                        ;

                        using (var trans = pe.QueryExecuter.BeginTransaction("dbAgreements"))
                        {
                            tbAgreementsUpdate.Execute(pe.QueryExecuter, trans);
                            tbAgreementModelsUpdate.Execute(pe.QueryExecuter, trans);
                            trans.Commit();
                        }

                        if (new[] { "ДоговорКпЗемельногоУчастка", "ДоговорКпПраваАрендыЗемельногоУчастка" }.Contains(AgreementHelper.GetAgreementType(pe.Args.AgreementId, pe.QueryExecuter))) {
                            var sidesData = AgreementHelper.GetEmptyAgreementModel(pe).GetSidesAccountData(pe);

                            var emails = new string[] { };

                            var selfUsersEmail = new TbUsers().AddFilter(t => t.flLogin, ConditionOperator.ContainsWord, sidesData.flWinner.flXin).Select(t => new FieldAlias[] { t.flEmail }, pe.QueryExecuter).Select(r => r.GetVal(t => t.flEmail)).Distinct().ToArray();

                            if (selfUsersEmail.Length == 0) {
                                var grUsersEmail = new TbUsers() { DbKey = "dbYodaUsersGr" }.AddFilter(t => t.flLogin, ConditionOperator.ContainsWord, sidesData.flWinner.flXin).Select(t => new FieldAlias[] { t.flEmail }, pe.QueryExecuter).Select(r => r.GetVal(t => t.flEmail)).Distinct().ToArray();
                                emails = grUsersEmail;
                            }
                            else {
                                emails = selfUsersEmail;
                            }

                            var unsuccessSends = new List<string>();

                            emails.Each(async email => {
                                var emailSender = pe.RequestContext.AppEnv.ServiceProvider.GetRequiredService<IEmailSender>();
                                try {
                                    await emailSender.SendAsync(email, "О согласовании договора купли-продажи прав на земельный участок",
@$"Продавец {sidesData.flSeller.flName} инициировал в отношении Победителя {sidesData.flWinner.flName} процесс согласования договора купли-продажи
(https://traderesources.gosreestr.kz/ru/traderesources/agreements/register/agreement?AgreementId={pe.Args.AgreementId}&MenuAction=view)
права частной собственности/аренды земельного участка и ожидает от Вас согласования текста документа.
Просим до {DateTime.Now.AddDays(1):hh:mm:ss dd:MM:yyyy} проверить правильность внесенных Продавцом данных и согласовать текст документа. После указанной даты текст документа согласуется автоматически.
"
                                    );
                                }
                                catch (Exception ex) {
                                    unsuccessSends.Add(email);
                                    return;
                                }
                            });

                            if (unsuccessSends.Count > 0) {
                                if (unsuccessSends.Count == emails.Length) {
                                    pe.SetPostbackMessage("Не удалось отправить уведомление победителю.");
                                }
                            }
                            else if (emails.Length > 0) {
                                pe.SetPostbackMessage($"Уведомление победителю отправлено.");
                            }
                        }

                        pe.Args.MenuAction = Actions.View;
                        pe.Redirect.SetRedirect(ModuleName, MenuName, pe.Args);
                    });
                })
                .OnAction(Actions.SendToCorrection, action => {
                    return action
                    .IsValid(env =>
                    {
                        if (env.Args.AgreementId == 0)
                            throw new NotImplementedException("AgreementId is Null");

                        var isWinner = AgreementHelper.GetEmptyAgreementModel(env).IsWinner(env);
                        var canCreate = AgreementHelper.GetEmptyAgreementModel(env).HasAccessToCreate(env);
                        var isAgreementCreator = AgreementHelper.GetEmptyAgreementModel(env).IsAgreementCreator(env);
                        var agrStatus = AgreementHelper.GetAgreementStatus(env.Args.AgreementId, env.QueryExecuter);

                        if (!(isWinner && agrStatus == AgreementStatuses.OnApproval.ToString()) && !(canCreate && isAgreementCreator && agrStatus.In(AgreementStatuses.SignedSeller.ToString(), AgreementStatuses.SignedWinner.ToString(), AgreementStatuses.Agreed.ToString(), AgreementStatuses.Saved.ToString())))
                            return new RedirectResult(new AccessDeniedException("No access"));
                        return new OkResult();
                    })
                    .OnRendering(re =>
                    {
                        var card = AgreementHelper.GetDataPanelHtml(re);
                        var tbAgreementModels = new TbAgreementModels();
                        tbAgreementModels.flComment.RenderCustom(card, re, null);
                        re.Form.AddComponent(card);
                        re.Form.AddSubmitButton("Отправить на доработку");
                    })
                    .OnValidating(ve =>
                    {
                        var tbAgreementModels = new TbAgreementModels();
                        tbAgreementModels.flComment.Required().Validate(ve);
                    })
                    .OnProcessing(pe =>
                    {
                        var tbAgreements = new TbAgreements();
                        tbAgreements.AddFilter(t => t.flAgreementId, pe.Args.AgreementId);
                        var agreementRevisionId = AgreementHelper.GetAgreementActiveRevisionId(pe.Args.AgreementId, pe.QueryExecuter);
                        var tbAgreementModels = new TbAgreementModels();
                        tbAgreementModels.AddFilter(t => t.flAgreementRevisionId, agreementRevisionId);

                        var tbAgreementsUpdate = tbAgreements.Update()
                            .SetT(t => t.flAgreementStatus, AgreementStatuses.OnCorrection)
                        ;
                        var tbAgreementModelsUpdate = tbAgreementModels.Update()
                            .Set(t => t.flComment, tbAgreementModels.flComment.GetVal(pe))
                            .Set(t => t.flCommentDateTime, pe.QueryExecuter.GetDateTime("dbAgreements"))
                            .SetT(t => t.flAgreementStatus, AgreementStatuses.OnCorrection)
                        ;

                        var tbAgreementSignsRemove = new TbAgreementSigns()
                            .AddFilter(t => t.flAgreementId, pe.Args.AgreementId)
                            .Remove();

                        using (var trans = pe.QueryExecuter.BeginTransaction("dbAgreements"))
                        {
                            tbAgreementsUpdate.Execute(pe.QueryExecuter, trans);
                            tbAgreementModelsUpdate.Execute(pe.QueryExecuter, trans);
                            tbAgreementSignsRemove.Execute(pe.QueryExecuter, trans);
                            trans.Commit();
                        }

                        pe.Args.MenuAction = Actions.View;
                        pe.Redirect.SetRedirect(ModuleName, MenuName, pe.Args);
                    });
                })
                .OnAction(Actions.ApproveWinner, action => {
                    return action
                    .IsValid(env =>
                    {
                        if (env.Args.AgreementId == 0)
                            throw new NotImplementedException("AgreementId is Null");

                        var isWinner = AgreementHelper.GetEmptyAgreementModel(env).IsWinner(env);
                        var agrStatus = AgreementHelper.GetAgreementStatus(env.Args.AgreementId, env.QueryExecuter);

                        if (!(isWinner && agrStatus == AgreementStatuses.OnApproval.ToString()))
                            return new RedirectResult(new AccessDeniedException("No access"));
                        return new OkResult();
                    })
                    .OnRendering(re =>
                    {
                        re.Form.AddComponent(AgreementHelper.GetDataPanelHtml(re));
                        re.Form.AddSubmitButton("Согласовать");
                    })
                    .OnProcessing(pe =>
                    {
                        var tbAgreements = new TbAgreements();
                        tbAgreements.AddFilter(t => t.flAgreementId, pe.Args.AgreementId);
                        var agreementRevisionId = AgreementHelper.GetAgreementActiveRevisionId(pe.Args.AgreementId, pe.QueryExecuter);
                        var tbAgreementModels = new TbAgreementModels();
                        tbAgreementModels.AddFilter(t => t.flAgreementRevisionId, agreementRevisionId);

                        var tbAgreementsUpdate = tbAgreements.Update()
                            .SetT(t => t.flAgreementStatus, AgreementStatuses.Agreed)
                        ;
                        var tbAgreementModelsUpdate = tbAgreementModels.Update()
                            .SetT(t => t.flAgreementStatus, AgreementStatuses.Agreed)
                        ;

                        using (var trans = pe.QueryExecuter.BeginTransaction("dbAgreements"))
                        {
                            tbAgreementsUpdate.Execute(pe.QueryExecuter, trans);
                            tbAgreementModelsUpdate.Execute(pe.QueryExecuter, trans);
                            trans.Commit();
                        }

                        pe.Args.MenuAction = Actions.View;
                        pe.Redirect.SetRedirect(ModuleName, MenuName, pe.Args);
                    });
                })
                .OnAction(Actions.SignSeller, action => {
                    return action
                    .IsValid(env =>
                    {
                        if (env.Args.AgreementId == 0)
                            throw new NotImplementedException("AgreementId is Null");

                        var canSign = AgreementHelper.GetEmptyAgreementModel(env).HasAccessToSign(env);
                        var isAgreementCreator = AgreementHelper.GetEmptyAgreementModel(env).IsAgreementCreator(env);
                        var isSignAvailableDate = AgreementHelper.GetEmptyAgreementModel(env).IsSignAvailableDate(env);
                        var agrStatus = AgreementHelper.GetAgreementStatus(env.Args.AgreementId, env.QueryExecuter);
                        var agrStatusDate = AgreementHelper.GetAgreementStatusDateTime(env.Args.AgreementId, env.QueryExecuter);
                        var hasSign = AgreementHelper.AgreementHasSign(env.Args.AgreementId, AgreementSignerRoles.Seller, env.QueryExecuter);

                        if (!((!hasSign) && canSign && isAgreementCreator /*&& isSignAvailableDate*/ && (new[] { AgreementStatuses.Agreed.ToString(), AgreementStatuses.OnCorrection.ToString(), AgreementStatuses.Extended.ToString(), AgreementStatuses.SignedWinner.ToString() }.Contains(agrStatus) || (agrStatusDate.AddDays(1) <= DateTime.Now && agrStatus == AgreementStatuses.OnApproval.ToString()))))
                            return new RedirectResult(new AccessDeniedException("No access"));
                        return new OkResult();
                    })
                    .OnRendering(re =>
                    {
                        re.Form.AddComponent(AgreementHelper.GetDataPanelHtml(re));
                        var appContent = AgreementHelper.GetAgreementHtmlContent(re.Args.AgreementId, re.QueryExecuter);
                        var appContentHtml = Convert.ToBase64String(Encoding.UTF8.GetBytes(appContent.ToHtmlString(null).Value));
                        re.Form.AddComponent(re.GetSignBox(appContentHtml));
                        re.Form.AddSubmitButton("submit", "Подтвердить и отправить на подписание", cssClass: "btn btn-primary my-2");
                        re.Form.AddComponent(appContent);
                    })
                    .OnValidating(ve => {
                        var appContentHtml = Convert.ToBase64String(Encoding.UTF8.GetBytes(AgreementHelper.GetAgreementHtmlContent(ve.Args.AgreementId, ve.QueryExecuter).ToHtmlString(null).Value));
                        if (!ve.ValidateSign(appContentHtml, out var certData, out var error))
                        {
                            ve.AddError("certError", error);
                            return;
                        }
                    })
                    .OnProcessing(async pe =>
                    {
                        var Models = AgreementHelper.GetAgreementModels(pe);
                        var appContentHtml = Convert.ToBase64String(Encoding.UTF8.GetBytes(AgreementHelper.GetAgreementHtmlContent(pe.Args.AgreementId, pe.QueryExecuter).ToHtmlString(null).Value));
                        if (!pe.ValidateSign(appContentHtml, out var certData, out var error))
                        {
                            pe.AddError("certError", error);
                            return;
                        }

                        var tbAgreements = new TbAgreements();
                        tbAgreements.AddFilter(t => t.flAgreementId, pe.Args.AgreementId);
                        var agreementRevisionId = AgreementHelper.GetAgreementActiveRevisionId(pe.Args.AgreementId, pe.QueryExecuter);
                        var tbAgreementModels = new TbAgreementModels();
                        tbAgreementModels.AddFilter(t => t.flAgreementRevisionId, agreementRevisionId);
                        var curDate = pe.QueryExecuter.GetDateTime("dbAgreements");

                        var newStatus = AgreementStatuses.SignedSeller;
                        if (AgreementHelper.AgreementHasSign(pe.Args.AgreementId, AgreementSignerRoles.Winner, pe.QueryExecuter)) {
                            newStatus = AgreementStatuses.Signed;
                        }

                        var tbAgreementsUpdate = tbAgreements.Update()
                            .SetT(t => t.flAgreementStatus, newStatus)
                        ;
                        var tbAgreementModelsUpdate = tbAgreementModels.Update()
                            .SetT(t => t.flAgreementStatus, newStatus)
                        ;

                        var tbAgreementSignsRemove = new TbAgreementSigns()
                            .AddFilter(t => t.flAgreementId, pe.Args.AgreementId)
                            .AddFilter(t => t.flSignerRole, AgreementSignerRoles.Seller)
                            .Remove();

                        var tbAgreementSignsInsert = new TbAgreementSigns().Insert()
                            .SetT(t => t.flAgreementId, pe.Args.AgreementId)
                            .SetCertData(certData, curDate, pe.User.Id)
                            .SetT(t => t.flSignerRole, AgreementSignerRoles.Seller);


                        using (var trans = pe.QueryExecuter.BeginTransaction("dbAgreements"))
                        {
                            tbAgreementsUpdate.Execute(pe.QueryExecuter, trans);
                            tbAgreementModelsUpdate.Execute(pe.QueryExecuter, trans);
                            tbAgreementSignsRemove.Execute(pe.QueryExecuter, trans);
                            tbAgreementSignsInsert.Execute(pe.QueryExecuter, trans);

                            if (newStatus == AgreementStatuses.Signed) {
                                var args = new DefaultDocPdfMnuArgs() {
                                    ContentCache = $"{MenuName}-{pe.Args.AgreementId}-{curDate.ToString("dd-MM-yyyy-hh-mm")}",
                                    ContentUiPackagesCache = $"UI-Packages-{MenuName}-{pe.Args.AgreementId}-{curDate.ToString("dd-MM-yyyy-hh-mm")}"
                                };

                                pe.RequestContext.Cache.Set(args.ContentCache, Models.GetPdfContentWithSigns(pe.Args.AgreementId, curDate, pe.RequestContext, pe.QueryExecuter, trans));
                                pe.RequestContext.Cache.Set(args.ContentUiPackagesCache, Models.First().GetUIPackages());

                                using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { ExecutablePath = pe.RequestContext.Configuration["ChromiumPath"], Headless = true });
                                using var page = await browser.NewPageAsync();
                                await page.GoToAsync(pe.RequestContext.GetUrlHelper().YodaAction(ModuleName, nameof(DefaultDocPdfMnu), args, urlWithSchema: true));
                                Thread.Sleep(3000);
                                var pdfDataWithSigns = await page.PdfDataAsync(new PdfOptions() {
                                    Format = PaperFormat.A4,
                                    MarginOptions = new MarginOptions() {
                                        Bottom = "1.3cm",
                                        Left = "2.6cm",
                                        Right = "1.3cm",
                                        Top = "1.3cm"
                                    },
                                    PrintBackground = true
                                });
                                browser.CloseAsync();

                                pe.RequestContext.Cache.Remove(args.ContentCache);
                                pe.RequestContext.Cache.Remove(args.ContentUiPackagesCache);

                                var tbAgreementPdfsUpdate = new TbAgreementPdfs()
                                    .AddFilter(t => t.flAgreementId, pe.Args.AgreementId).Update()
                                    .Set(t => t.flPdfWithSigns, pdfDataWithSigns)
                                    ;
                                tbAgreementPdfsUpdate.Execute(pe.QueryExecuter, trans);
                            }

                            trans.Commit();

                        }

                        if (newStatus == AgreementStatuses.SignedSeller) {
                            if (new[] { "ДоговорКпЗемельногоУчастка", "ДоговорКпПраваАрендыЗемельногоУчастка" }.Contains(AgreementHelper.GetAgreementType(pe.Args.AgreementId, pe.QueryExecuter))) {
                                var sidesData = AgreementHelper.GetEmptyAgreementModel(pe).GetSidesAccountData(pe);

                                var emails = new string[] { };

                                var selfUsersEmail = new TbUsers().AddFilter(t => t.flLogin, ConditionOperator.ContainsWord, sidesData.flWinner.flXin).Select(t => new FieldAlias[] { t.flEmail }, pe.QueryExecuter).Select(r => r.GetVal(t => t.flEmail)).Distinct().ToArray();

                                if (selfUsersEmail.Length == 0) {
                                    var grUsersEmail = new TbUsers() { DbKey = "dbYodaUsersGr" }.AddFilter(t => t.flLogin, ConditionOperator.ContainsWord, sidesData.flWinner.flXin).Select(t => new FieldAlias[] { t.flEmail }, pe.QueryExecuter).Select(r => r.GetVal(t => t.flEmail)).Distinct().ToArray();
                                    emails = grUsersEmail;
                                }
                                else {
                                    emails = selfUsersEmail;
                                }

                                var unsuccessSends = new List<string>();

                                emails.Each(async email => {
                                    var emailSender = pe.RequestContext.AppEnv.ServiceProvider.GetRequiredService<IEmailSender>();
                                    try {
                                        await emailSender.SendAsync(email, "О подписании договора купли-продажи права на земельный участок",
@$"Продавец {sidesData.flSeller.flName} подписал с ЭЦП договор купли-продажи
(https://traderesources.gosreestr.kz/ru/traderesources/agreements/register/agreement?AgreementId={pe.Args.AgreementId}&MenuAction=view)
права частной собственности/аренды земельного участка и ожидает от Победителя {sidesData.flWinner.flName} подписания договора с использованием ЭЦП, для его введения в действие.
Просим до {AgreementHelper.GetEmptyAgreementModel(pe).SignAvailableDate(pe):hh:mm:ss dd:MM:yyyy} подписать документ. После указанной даты в соответствии с требованиями законодательства доступ победителю для подписания документа будет закрыт автоматически.Продавцом будет подписан с ЭЦП Акт об отмененных торгах, что приведет к потере Вами гарантийного взноса, внесенного по объекту продажи.
"
                                        );
                                    }
                                    catch (Exception ex) {
                                        unsuccessSends.Add(email);
                                        return;
                                    }
                                });

                                if (unsuccessSends.Count > 0) {
                                    if (unsuccessSends.Count == emails.Length) {
                                        pe.SetPostbackMessage("Не удалось отправить уведомление победителю.");
                                    }
                                }
                                else if (emails.Length > 0) {
                                    pe.SetPostbackMessage($"Уведомление победителю отправлено.");
                                }
                            }
                        }

                        pe.Args.MenuAction = Actions.View;
                        pe.Redirect.SetRedirect(ModuleName, MenuName, pe.Args);
                    });
                })
                .OnAction(Actions.SignWinner, action => {
                    return action
                    .IsValid(env =>
                    {
                        if (env.Args.AgreementId == 0)
                            throw new NotImplementedException("AgreementId is Null");

                        var isWinner = AgreementHelper.GetEmptyAgreementModel(env).IsWinner(env);
                        var isSignAvailableDate = AgreementHelper.GetEmptyAgreementModel(env).IsSignAvailableDate(env);
                        var agrStatus = AgreementHelper.GetAgreementStatus(env.Args.AgreementId, env.QueryExecuter);
                        var hasSign = AgreementHelper.AgreementHasSign(env.Args.AgreementId, AgreementSignerRoles.Winner, env.QueryExecuter);

                        if (!((!hasSign) && isWinner && isSignAvailableDate && new[] { AgreementStatuses.Agreed.ToString(), AgreementStatuses.SignedSeller.ToString(), AgreementStatuses.Extended.ToString() }.Contains(agrStatus)))
                            return new RedirectResult(new AccessDeniedException("No access"));
                        return new OkResult();
                    })
                    .OnRendering(re =>
                    {
                        re.Form.AddComponent(AgreementHelper.GetDataPanelHtml(re));
                        var appContent = AgreementHelper.GetAgreementHtmlContent(re.Args.AgreementId, re.QueryExecuter);
                        var appContentHtml = Convert.ToBase64String(Encoding.UTF8.GetBytes(appContent.ToHtmlString(null).Value));
                        re.Form.AddComponent(re.GetSignBox(appContentHtml));
                        re.Form.AddSubmitButton("submit", "Подтвердить", cssClass: "btn btn-primary my-2");
                        re.Form.AddComponent(appContent);
                    })
                    .OnValidating(ve => {
                        var appContentHtml = Convert.ToBase64String(Encoding.UTF8.GetBytes(AgreementHelper.GetAgreementHtmlContent(ve.Args.AgreementId, ve.QueryExecuter).ToHtmlString(null).Value));
                        if (!ve.ValidateSign(appContentHtml, out var certData, out var error))
                        {
                            ve.AddError("certError", error);
                            return;
                        }
                    })
                    .OnProcessing(async pe =>
                    {
                        var Models = AgreementHelper.GetAgreementModels(pe);
                        var appContentHtml = Convert.ToBase64String(Encoding.UTF8.GetBytes(AgreementHelper.GetAgreementHtmlContent(pe.Args.AgreementId, pe.QueryExecuter).ToHtmlString(null).Value));
                        if (!pe.ValidateSign(appContentHtml, out var certData, out var error))
                        {
                            pe.AddError("certError", error);
                            return;
                        }

                        var tbAgreements = new TbAgreements();
                        tbAgreements.AddFilter(t => t.flAgreementId, pe.Args.AgreementId);
                        var agreementRevisionId = AgreementHelper.GetAgreementActiveRevisionId(pe.Args.AgreementId, pe.QueryExecuter);
                        var tbAgreementModels = new TbAgreementModels();
                        tbAgreementModels.AddFilter(t => t.flAgreementRevisionId, agreementRevisionId);
                        var curDate = pe.QueryExecuter.GetDateTime("dbAgreements");

                        var newStatus = AgreementStatuses.SignedWinner;
                        if (AgreementHelper.AgreementHasSign(pe.Args.AgreementId, AgreementSignerRoles.Seller, pe.QueryExecuter)) {
                            newStatus = AgreementStatuses.Signed;
                        }

                        var tbAgreementsUpdate = tbAgreements.Update()
                            .SetT(t => t.flAgreementStatus, newStatus)
                            .SetT(t => t.flAgreementSignDate, pe.QueryExecuter.GetDateTime("dbAgreements"))
                        ;
                        var tbAgreementModelsUpdate = tbAgreementModels.Update()
                            .SetT(t => t.flAgreementStatus, newStatus)
                        ;

                        var tbAgreementSignsRemove = new TbAgreementSigns()
                            .AddFilter(t => t.flAgreementId, pe.Args.AgreementId)
                            .AddFilter(t => t.flSignerRole, AgreementSignerRoles.Winner)
                            .Remove();

                        var tbAgreementSignsInsert = new TbAgreementSigns().Insert()
                            .SetT(t => t.flAgreementId, pe.Args.AgreementId)
                            .SetCertData(certData, curDate, pe.User.Id)
                            .SetT(t => t.flSignerRole, AgreementSignerRoles.Winner);


                        using (var trans = pe.QueryExecuter.BeginTransaction("dbAgreements"))
                        {
                            Models.OnSignEnd(pe, trans);
                            tbAgreementsUpdate.Execute(pe.QueryExecuter, trans);
                            tbAgreementModelsUpdate.Execute(pe.QueryExecuter, trans);
                            tbAgreementSignsRemove.Execute(pe.QueryExecuter, trans);
                            tbAgreementSignsInsert.Execute(pe.QueryExecuter, trans);

                            if (newStatus == AgreementStatuses.Signed) {
                                var args = new DefaultDocPdfMnuArgs() {
                                    ContentCache = $"{MenuName}-{pe.Args.AgreementId}-{curDate.ToString("dd-MM-yyyy-hh-mm")}",
                                    ContentUiPackagesCache = $"UI-Packages-{MenuName}-{pe.Args.AgreementId}-{curDate.ToString("dd-MM-yyyy-hh-mm")}"
                                };

                                pe.RequestContext.Cache.Set(args.ContentCache, Models.GetPdfContentWithSigns(pe.Args.AgreementId, curDate, pe.RequestContext, pe.QueryExecuter, trans));
                                pe.RequestContext.Cache.Set(args.ContentUiPackagesCache, Models.First().GetUIPackages());

                                using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { ExecutablePath = pe.RequestContext.Configuration["ChromiumPath"], Headless = true });
                                using var page = await browser.NewPageAsync();
                                await page.GoToAsync(pe.RequestContext.GetUrlHelper().YodaAction(ModuleName, nameof(DefaultDocPdfMnu), args, urlWithSchema: true));
                                Thread.Sleep(3000);
                                var pdfDataWithSigns = await page.PdfDataAsync(new PdfOptions() {
                                    Format = PaperFormat.A4,
                                    MarginOptions = new MarginOptions() {
                                        Bottom = "1.3cm",
                                        Left = "2.6cm",
                                        Right = "1.3cm",
                                        Top = "1.3cm"
                                    },
                                    PrintBackground = true
                                });
                                browser.CloseAsync();

                                pe.RequestContext.Cache.Remove(args.ContentCache);
                                pe.RequestContext.Cache.Remove(args.ContentUiPackagesCache);

                                var tbAgreementPdfsUpdate = new TbAgreementPdfs()
                                    .AddFilter(t => t.flAgreementId, pe.Args.AgreementId).Update()
                                    .Set(t => t.flPdfWithSigns, pdfDataWithSigns)
                                    ;
                                tbAgreementPdfsUpdate.Execute(pe.QueryExecuter, trans);
                            }

                            trans.Commit();
                        }

                        pe.Args.MenuAction = Actions.View;
                        pe.Redirect.SetRedirect(ModuleName, MenuName, pe.Args);
                    });
                })
                .OnAction(Actions.Extend, action => {
                    return action
                    .IsValid(env =>
                    {
                        if (env.Args.AgreementId == 0)
                            throw new NotImplementedException("AgreementId is Null");

                        var isInternal = (!env.User.IsExternalUser() && !env.User.IsGuest());
                        var isSignAvailableDate = AgreementHelper.GetEmptyAgreementModel(env).IsSignAvailableDate(env);
                        var agrStatus = AgreementHelper.GetAgreementStatus(env.Args.AgreementId, env.QueryExecuter);

                        if (!(isInternal && agrStatus != AgreementStatuses.Signed.ToString() && !isSignAvailableDate))
                            return new RedirectResult(new AccessDeniedException("No access"));
                        return new OkResult();
                    })
                    .OnRendering(re =>
                    {
                        var card = AgreementHelper.GetDataPanelHtml(re);
                        var tbAgreementModels = new TbAgreementModels();
                        tbAgreementModels.flComment.RenderCustom(card, re, null);
                        re.Form.AddComponent(card);
                        re.Form.AddSubmitButton("Продлить период подписания");
                    })
                    .OnValidating(ve =>
                    {
                        var tbAgreementModels = new TbAgreementModels();
                        tbAgreementModels.flComment.Required().Validate(ve);
                    })
                    .OnProcessing(pe =>
                    {
                        var tbAgreements = new TbAgreements();
                        tbAgreements.AddFilter(t => t.flAgreementId, pe.Args.AgreementId);
                        var agreementRevisionId = AgreementHelper.GetAgreementActiveRevisionId(pe.Args.AgreementId, pe.QueryExecuter);
                        var tbAgreementModels = new TbAgreementModels();
                        tbAgreementModels.AddFilter(t => t.flAgreementRevisionId, agreementRevisionId);

                        var tbAgreementsUpdate = tbAgreements.Update()
                            .SetT(t => t.flAgreementStatus, AgreementStatuses.Extended)
                        ;
                        var tbAgreementModelsUpdate = tbAgreementModels.Update()
                            .Set(t => t.flComment, tbAgreementModels.flComment.GetVal(pe))
                            .Set(t => t.flCommentDateTime, pe.QueryExecuter.GetDateTime("dbAgreements"))
                            .SetT(t => t.flAgreementStatus, AgreementStatuses.Extended)
                        ;

                        //var tbAgreementSignsRemove = new TbAgreementSigns()
                        //    .AddFilter(t => t.flAgreementId, pe.Args.AgreementId)
                        //    .Remove();

                        using (var trans = pe.QueryExecuter.BeginTransaction("dbAgreements"))
                        {
                            tbAgreementsUpdate.Execute(pe.QueryExecuter, trans);
                            tbAgreementModelsUpdate.Execute(pe.QueryExecuter, trans);
                            //tbAgreementSignsRemove.Execute(pe.QueryExecuter, trans);
                            trans.Commit();
                        }

                        pe.Args.MenuAction = Actions.View;
                        pe.Redirect.SetRedirect(ModuleName, MenuName, pe.Args);
                    });
                })
                //.OnAction(Actions.LinkPaymentItems, action => {
                //    return action
                //    .IsValid(env =>
                //    {
                //        if (env.Args.AgreementId == 0)
                //            throw new NotImplementedException("AgreementId is Null");

                //        var isSeller = AgreementHelper.GetEmptyAgreementModel(env).IsSeller(env);
                //        var agrStatus = AgreementHelper.GetAgreementStatus(env.Args.AgreementId, env.QueryExecuter);

                //        if (!(isSeller && agrStatus == AgreementStatuses.Signed.ToString()))
                //            return new RedirectResult(new AccessDeniedException("No access"));
                //        return new OkResult();
                //    })
                //    .Wizard(wizard => wizard
                //        .Args(env => env.Args)
                //        .Model(env =>
                //        {
                //            env.Args.AgreementType = AgreementHelper.GetAgreementType(env.Args.AgreementId, env.QueryExecuter);
                //            var agrTempl = AgreementHelper.GetAgreementTypeModel(env.Args.AgreementType);
                //            var requisites = agrTempl.GetPaymentAndOverpaymentRequisites(env);
                //            var guaranteePaymentItems = agrTempl.GetGuaranteePayments(env);

                //            return new PaymentMatchWizardModel()
                //            {
                //                flGuaranteePaymentItems = guaranteePaymentItems,

                //                flName = requisites.flPayment.flName,
                //                flXin = requisites.flPayment.flXin,
                //                flBik = requisites.flPayment.flBik,
                //                flIban = requisites.flPayment.flIban,
                //                flKbe = requisites.flPayment.flKbe,
                //                flKnp = requisites.flPayment.flKnp,
                //                flKbk = requisites.flPayment.flKbk,

                //                flOverpaymentName = requisites.flOverPayment.flName,
                //                flOverpaymentXin = requisites.flOverPayment.flXin,
                //                flOverpaymentBik = requisites.flOverPayment.flBik,
                //                flOverpaymentIban = requisites.flOverPayment.flIban,
                //                flOverpaymentKbe = requisites.flOverPayment.flKbe,
                //                flOverpaymentKnp = requisites.flOverPayment.flKnp,
                //                flOverpaymentKbk = requisites.flOverPayment.flKbk,
                //                flOverpaymentContacts = requisites.flOverPayment.flContacts,
                //            };
                //        })
                //        .CancelBtn("Отмена", env => new ActionRedirectData(ModuleName, MenuName, new DefaultAgrTemplateArgs { AgreementId = env.Args.AgreementId, MenuAction = Actions.View }))
                //        .FinishBtn("Отправить деньги")
                //        .Step("Привязка платежей", step => step
                //            .OnRendering(re =>
                //            {
                //                var paymentsProvider = new TraderesourcesPaymentsProvider(re.Model.flOverpaymentXin, re.Model.flOverpaymentName, re.Env.RequestContext);
                //                var guaranteePaymentsList = paymentsProvider.GuaranteePayments.GetAllPayments().Where(x => re.Model.flGuaranteePaymentItems.Any(y => y.flId.Id == x.Id.Id));
                //                var freePaymentsList = paymentsProvider.SellPayments.GetFreePayments();
                //                var freePayments = freePaymentsList.OrderByDescending(item => item.Date).ToArray();

                //                re.Args.AgreementType = AgreementHelper.GetAgreementType(re.Args.AgreementId, re.Env.QueryExecuter);
                //                var agrTempl = AgreementHelper.GetAgreementTypeModel(re.Args.AgreementType);

                //                PaymentModel paymentModel = null;
                //                var tbPayments = new TbPayments()
                //                        .AddFilter(t => t.flAgreementId, re.Args.AgreementId);
                //                if (tbPayments.Count(re.Env.QueryExecuter) > 0)
                //                {
                //                    paymentModel = tbPayments.GetPaymentModelFirstOrDefault(re.Env.QueryExecuter, null);
                //                }

                //                var paidAmount = paymentModel == null ? 0 : paymentModel.flPaidAmount;

                //                re.Model.flPayAmount = (paymentModel == null ? agrTempl.GetSellPrice(new ActionEnv<DefaultAgrTemplateArgs>(null, re.Args, ActionFlowStep.Rendering, re.Env.RequestContext, re.Env.FormCollection, null, null, null)).Value : paymentModel.flPayAmount);
                //                var needToPay = re.Model.flPayAmount - paidAmount;

                //                var checkBoxesBox = new CheckBoxesBox("payments", CheckBoxesBox.ColumnsCount.col1, $"Цена продажи / Требуемая / Выбранная / Оставшаяся сумма - {re.Model.flPayAmount:#,##0.00} / {needToPay:#,##0.00} / {0:#,##0.00} / {needToPay:#,##0.00} тг.").AppendTo(re.Panel);

                //                var tbPaymentItems = new TbRenderPaymentItems();

                //                guaranteePaymentsList.Each(guaranteePaymentItem => {
                //                    var card = new Card($"Платеж №{guaranteePaymentItem.Id.Id} - {guaranteePaymentItem.Amount:#,##0.00} тг. от {guaranteePaymentItem.Date:dd.MM.yyyy hh:mm}") {
                //                        CssClass = "payment-item",
                //                        Attributes = new Dictionary<string, object>() {
                //                            { "amount", guaranteePaymentItem.Amount }
                //                        }
                //                    };
                //                    //var cardRow = new GridRow().AppendTo(card);
                //                    //tbPaymentItems.flPaymentItemId.RenderCustom(cardRow, re.Env, freePayment.Id.Id, readOnly: true, /*orientation: FormOrientation.Basic,*/ readOnlyCssClass: "d-block h5", cssClass: "col-sm-4");
                //                    //tbPaymentItems.flAmount.RenderCustom(cardRow, re.Env, freePayment.Amount, readOnly: true, /*orientation: FormOrientation.Basic,*/ readOnlyCssClass: "d-block h5", cssClass: "col-sm");
                //                    //tbPaymentItems.flDateTime.RenderCustom(cardRow, re.Env, freePayment.Date, readOnly: true, /*orientation: FormOrientation.Basic,*/ readOnlyCssClass: "d-block h5", cssClass: "col-sm");
                //                    tbPaymentItems.flPurpose.RenderCustom(card, re.Env, guaranteePaymentItem.Purpose, readOnly: true, hideLabel: true, /*orientation: FormOrientation.Basic,*/ readOnlyCssClass: "d-block h5 my-0");
                //                    checkBoxesBox.AppendCheckbox($"{guaranteePaymentItem.Id.Id}", card, true, "w-100", false);
                //                });

                //                re.Panel.AddComponent(new UiPackages("payments-choose-counter"));

                //                foreach (var freePayment in freePayments)
                //                {
                //                    var card = new Card($"Платеж №{freePayment.Id.Id} - {freePayment.Amount:#,##0.00} тг. от {freePayment.Date:dd.MM.yyyy hh:mm}")
                //                    {
                //                        CssClass = "payment-item",
                //                        Attributes = new Dictionary<string, object>() {
                //                            { "amount", freePayment.Amount }
                //                        }
                //                    };
                //                    //var cardRow = new GridRow().AppendTo(card);
                //                    //tbPaymentItems.flPaymentItemId.RenderCustom(cardRow, re.Env, freePayment.Id.Id, readOnly: true, /*orientation: FormOrientation.Basic,*/ readOnlyCssClass: "d-block h5", cssClass: "col-sm-4");
                //                    //tbPaymentItems.flAmount.RenderCustom(cardRow, re.Env, freePayment.Amount, readOnly: true, /*orientation: FormOrientation.Basic,*/ readOnlyCssClass: "d-block h5", cssClass: "col-sm");
                //                    //tbPaymentItems.flDateTime.RenderCustom(cardRow, re.Env, freePayment.Date, readOnly: true, /*orientation: FormOrientation.Basic,*/ readOnlyCssClass: "d-block h5", cssClass: "col-sm");
                //                    tbPaymentItems.flPurpose.RenderCustom(card, re.Env, freePayment.Purpose, readOnly: true, hideLabel: true, /*orientation: FormOrientation.Basic,*/ readOnlyCssClass: "d-block h5 my-0");
                //                    checkBoxesBox.AppendCheckbox($"{freePayment.Id.Id}", card, false, "w-100");
                //                }

                //            })
                //            .OnValidating(ve =>
                //            {
                //                var paymentsProvider = new TraderesourcesPaymentsProvider(ve.Model.flOverpaymentXin, ve.Model.flOverpaymentName, ve.Env.RequestContext);
                //                var guaranteePaymentsList = paymentsProvider.GuaranteePayments.GetAllPayments().Where(x => ve.Model.flGuaranteePaymentItems.Any(y => y.flId.Id == x.Id.Id));
                //                var freePaymentsList = paymentsProvider.SellPayments.GetAllPayments().ToList();
                //                freePaymentsList.AddRange(guaranteePaymentsList);
                //                var freePayments = freePaymentsList.ToArray();

                //                var choosenPaymentIds = new CheckBoxesBox("payments", CheckBoxesBox.ColumnsCount.col4).GetPostedValue(ve.Env).ToList();
                //                choosenPaymentIds.AddRange(ve.Model.flGuaranteePaymentItems.Select(y => y.flId.Id.ToString()));
                //                //if (choosenPaymentIds.Count == 0)
                //                //{
                //                //    ve.Env.AddError("payments", ve.Env.T("Нужно выбрать хотя-бы один платеж!"));
                //                //}
                //                if (ve.Env.IsValid)
                //                {
                //                    var choosenPayments = freePayments.Where(freePayment => choosenPaymentIds.Contains(freePayment.Id.Id.ToString())).ToList();

                //                    ve.Args.AgreementType = AgreementHelper.GetAgreementType(ve.Args.AgreementId, ve.Env.QueryExecuter);
                //                    var agrTempl = AgreementHelper.GetAgreementTypeModel(ve.Args.AgreementType);

                //                    if (!agrTempl.HasPayment())
                //                    {
                //                        ve.Env.AddError("payments", ve.Env.T("Для данного типа договора не предусматривается оплата"));
                //                    }
                //                    if (ve.Env.IsValid)
                //                    {
                //                        var commitPayments = choosenPayments.Select(x => new PaymentItemModel()
                //                        {
                //                            flId = x.Id,
                //                            flDateTime = x.Date,
                //                            flAmount = x.Amount,
                //                            flPurpose = x.Purpose,
                //                            flIsGuarantee = guaranteePaymentsList.Any(gp => gp.Id.Id == x.Id.Id)
                //                        }).ToArray();

                //                        PaymentModel paymentModel = null;
                //                        var tbPayments = new TbPayments()
                //                                .AddFilter(t => t.flAgreementId, ve.Args.AgreementId);
                //                        if (tbPayments.Count(ve.Env.QueryExecuter) > 0)
                //                        {
                //                            paymentModel = tbPayments.GetPaymentModelFirstOrDefault(ve.Env.QueryExecuter, null);
                //                        }

                //                        if ((!commitPayments.Any(x => x.flIsGuarantee)) && paymentModel == null)
                //                        {
                //                            ve.Env.AddError("payments", ve.Env.T("В первой привязке обязательно нужно выбрать гарантийный взнос!"));
                //                        }

                //                        if (ve.Env.IsValid)
                //                        {
                //                            var choosenAmount = choosenPayments.Sum(x => x.Amount);
                //                            var sellPrice = ve.Model.flPayAmount;

                //                            if (choosenAmount < sellPrice && !agrTempl.IsInstallment())
                //                            {
                //                                ve.Env.AddError("payments", ve.Env.T("Если догвор без рассрочки, нужно выбрать платежи, полностью покрывающие требуемую сумму") + $" ({choosenAmount:#,##0.00} тг. < {sellPrice:#,##0.00} тг.)");
                //                            }
                //                            if (paymentModel != null && !agrTempl.IsInstallment())
                //                            {
                //                                ve.Env.AddError("payments", ve.Env.T("К договору уже приаязана оплата!") + $" ({choosenAmount:#,##0.00} тг. < {sellPrice:#,##0.00} тг.)");
                //                            }
                //                        }
                //                    }
                //                }
                //            })
                //            .OnProcessing(pe =>
                //            {
                //                var paymentsProvider = new TraderesourcesPaymentsProvider(pe.Model.flOverpaymentXin, pe.Model.flOverpaymentName, pe.Env.RequestContext);
                //                var guaranteePaymentsList = paymentsProvider.GuaranteePayments.GetAllPayments().Where(x => pe.Model.flGuaranteePaymentItems.Any(y => y.flId.Id == x.Id.Id));
                //                var freePaymentsList = paymentsProvider.SellPayments.GetAllPayments().ToList();
                //                freePaymentsList.AddRange(guaranteePaymentsList);
                //                var freePayments = freePaymentsList.ToArray();
                //                var choosenPaymentIds = new CheckBoxesBox("payments", CheckBoxesBox.ColumnsCount.col4).GetPostedValue(pe.Env).ToList();
                //                choosenPaymentIds.AddRange(pe.Model.flGuaranteePaymentItems.Select(y => y.flId.Id.ToString()));
                //                var choosenPayments = freePayments.Where(freePayment => choosenPaymentIds.Contains(freePayment.Id.Id.ToString())).ToArray();
                //                pe.Model.flPaymentItemIds = choosenPayments.Select(x => x.Id.Id.ToString()).ToArray();


                //                pe.Args.AgreementType = AgreementHelper.GetAgreementType(pe.Args.AgreementId, pe.Env.QueryExecuter);
                //                var agrTempl = AgreementHelper.GetAgreementTypeModel(pe.Args.AgreementType);

                //                PaymentModel paymentModel = null;
                //                var tbPayments = new TbPayments()
                //                        .AddFilter(t => t.flAgreementId, pe.Args.AgreementId);
                //                if (tbPayments.Count(pe.Env.QueryExecuter) > 0)
                //                {
                //                    paymentModel = tbPayments.GetPaymentModelFirstOrDefault(pe.Env.QueryExecuter, null);
                //                }

                //                pe.Model.flIsUpdate = paymentModel != null;
                //                pe.Model.flPaymentId = paymentModel != null ? paymentModel.flPaymentId : tbPayments.flPaymentId.GetNextId(pe.Env.QueryExecuter);
                //                pe.Model.flAmount = choosenPayments.Sum(x => x.Amount);
                //                pe.Model.flPaidAmount = paymentModel == null ? 0 : paymentModel.flPaidAmount;
                //                pe.Model.flPayAmount = paymentModel == null ? agrTempl.GetSellPrice(new ActionEnv<DefaultAgrTemplateArgs>(null, pe.Args, ActionFlowStep.Processing, pe.Env.RequestContext, pe.Env.FormCollection, null, null, null)).Value : paymentModel.flPayAmount;

                //                pe.Model.flNeedForFullPayAmount = pe.Model.flPayAmount - pe.Model.flPaidAmount;
                //                pe.Model.flPaidAmount += pe.Model.flAmount;

                //                pe.Model.flHasOverpayment = pe.Model.flPaidAmount > pe.Model.flPayAmount;
                //                pe.Model.flHasSendOverpayment = pe.Model.flHasOverpayment;
                //                if (pe.Model.flHasOverpayment)
                //                {
                //                    pe.Model.flOverpaymentAmount = pe.Model.flPaidAmount - pe.Model.flPayAmount;
                //                    pe.Model.flOverpaymentSendAmount = pe.Model.flOverpaymentAmount;
                //                    pe.Model.flPaidAmount = pe.Model.flPayAmount;
                //                }

                //                var commitPayments = choosenPayments.Select(x => new PaymentItemModel()
                //                {
                //                    flId = x.Id,
                //                    flDateTime = x.Date,
                //                    flAmount = x.Amount,
                //                    flPurpose = x.Purpose,
                //                    flIsGuarantee = guaranteePaymentsList.Any(gp => gp.Id.Id == x.Id.Id)
                //                }).ToArray();

                //                pe.Model.flRealAmount = commitPayments.Where(x => !x.flIsGuarantee).Sum(x => x.flAmount);
                //                pe.Model.flGuaranteeAmount = commitPayments.Where(x => x.flIsGuarantee).Sum(x => x.flAmount);
                //                pe.Model.flSendAmount = pe.Model.flRealAmount - pe.Model.flOverpaymentAmount;
                //                pe.Model.flHasSendAmount = true;
                //                if (pe.Model.flSendAmount <= 0)
                //                {
                //                    pe.Model.flHasSendAmount = false;
                //                    pe.Model.flSendAmount = 0;
                //                }
                //                if (pe.Model.flGuaranteeAmount >= pe.Model.flNeedForFullPayAmount)
                //                {
                //                    pe.Model.flOverpaymentSendAmount -= pe.Model.flGuaranteeAmount - pe.Model.flNeedForFullPayAmount;
                //                    if (pe.Model.flOverpaymentSendAmount <= 0)
                //                    {
                //                        pe.Model.flHasSendOverpayment = false;
                //                        pe.Model.flOverpaymentSendAmount = 0;
                //                    }
                //                }
                //            })
                //        )
                //        .Step("Реквизиты для возврата переплаты победителю", step => step
                //            .Enabled(env => {
                //                return env.Model.flHasSendOverpayment;
                //            })
                //            .OnRendering(re => {
                //                if (re.Model.flHasOverpayment) {
                //                    new Panel("alert alert-warning")
                //                        .Append(new HtmlText(re.Env.T("Обнаружена переплата!")))
                //                        .AppendTo(re.Panel);
                //                }
                //                var tbRenderPaymentMatches = new TbRenderPaymentMatches();
                //                re.Panel.AddLabel($"Контакты победителя: {re.Model.flOverpaymentContacts}", "d-block alert alert-secondary p-2 mb-3");
                //                tbRenderPaymentMatches.flOverpaymentXin.RenderCustomT(re.Panel, re.Env, re.Model.flOverpaymentXin, readOnly: true);
                //                tbRenderPaymentMatches.flOverpaymentName.RenderCustomT(re.Panel, re.Env, re.Model.flOverpaymentName, readOnly: true);
                //                tbRenderPaymentMatches.flOverpaymentBik.RenderCustomT(re.Panel, re.Env, re.Model.flOverpaymentBik);
                //                tbRenderPaymentMatches.flOverpaymentIban.RenderCustomT(re.Panel, re.Env, re.Model.flOverpaymentIban);
                //                tbRenderPaymentMatches.flOverpaymentKbe.RenderCustomT(re.Panel, re.Env, re.Model.flOverpaymentKbe);
                //                tbRenderPaymentMatches.flOverpaymentKnp.RenderCustomT(re.Panel, re.Env, re.Model.flOverpaymentKnp);
                //                //tbRenderPaymentMatches.flOverpaymentKbk.RenderCustomT(re.Panel, re.Env, re.Model.flOverpaymentKbk);

                //            })
                //            .OnValidating(ve =>
                //            {
                //                var tbRenderPaymentMatches = new TbRenderPaymentMatches();
                //                new Field[] {
                //                    tbRenderPaymentMatches.flOverpaymentBik,
                //                    tbRenderPaymentMatches.flOverpaymentIban,
                //                    tbRenderPaymentMatches.flOverpaymentKbe,
                //                    tbRenderPaymentMatches.flOverpaymentKnp,
                //                    //tbRenderPaymentMatches.flOverpaymentKbk
                //                }.Each(x => x.Validate(ve.Env));
                //            })
                //            .OnProcessing(pe =>
                //            {
                //                var tbRenderPaymentMatches = new TbRenderPaymentMatches();
                //                pe.Model.flOverpaymentBik = tbRenderPaymentMatches.flOverpaymentBik.GetVal(pe.Env);
                //                pe.Model.flOverpaymentIban = tbRenderPaymentMatches.flOverpaymentIban.GetVal(pe.Env);
                //                pe.Model.flOverpaymentKbe = tbRenderPaymentMatches.flOverpaymentKbe.GetVal(pe.Env);
                //                pe.Model.flOverpaymentKnp = tbRenderPaymentMatches.flOverpaymentKnp.GetVal(pe.Env);
                //                //pe.Model.flOverpaymentKbk = tbRenderPaymentMatches.flOverpaymentKbk.GetVal(pe.Env);
                //            })
                //        )
                //        .Step("Просмотр", step => step
                //            .OnRendering(re =>
                //            {
                //                if (re.Model.flHasOverpayment) {
                //                    new Panel("alert alert-warning")
                //                        .Append(new HtmlText(re.Env.T("Обнаружена переплата! Внимательно изучите итоговые вычисления!")))
                //                        .AppendTo(re.Panel);
                //                }
                //                var tbPaymentMatches = new TbPaymentMatches();
                //                tbPaymentMatches.flAmount.RenderCustomT(re.Panel, re.Env, re.Model.flAmount, readOnly: true);
                //                tbPaymentMatches.flGuaranteeAmount.RenderCustomT(re.Panel, re.Env, re.Model.flGuaranteeAmount, readOnly: true);
                //                tbPaymentMatches.flRealAmount.RenderCustomT(re.Panel, re.Env, re.Model.flRealAmount, readOnly: true);
                //                tbPaymentMatches.flHasSendAmount.RenderCustomT(re.Panel, re.Env, re.Model.flHasSendAmount, readOnly: true);
                //                if (re.Model.flHasSendAmount)
                //                {
                //                    tbPaymentMatches.flSendAmount.RenderCustomT(re.Panel, re.Env, re.Model.flSendAmount, readOnly: true);
                //                }

                //                var tbPayments = new TbPayments();
                //                tbPayments.flPayAmount.RenderCustomT(re.Panel, re.Env, re.Model.flPayAmount, readOnly: true);
                //                tbPayments.flPaidAmount.RenderCustomT(re.Panel, re.Env, re.Model.flPaidAmount, readOnly: true);

                //                var tbRenderPaymentMatches = new TbRenderPaymentMatches();
                //                var budgetCard = new Card("Реквизиты для отправки денег в бюджет").AppendTo(re.Panel);
                //                tbRenderPaymentMatches.flXin.RenderCustomT(budgetCard, re.Env, re.Model.flXin, readOnly: true);
                //                tbRenderPaymentMatches.flName.RenderCustomT(budgetCard, re.Env, re.Model.flName, readOnly: true);
                //                tbRenderPaymentMatches.flBik.RenderCustomT(budgetCard, re.Env, re.Model.flBik, readOnly: true);
                //                tbRenderPaymentMatches.flIban.RenderCustomT(budgetCard, re.Env, re.Model.flIban, readOnly: true);
                //                tbRenderPaymentMatches.flKnp.RenderCustomT(budgetCard, re.Env, re.Model.flKnp, readOnly: true);
                //                tbRenderPaymentMatches.flKbe.RenderCustomT(budgetCard, re.Env, re.Model.flKbe, readOnly: true);
                //                tbRenderPaymentMatches.flKbk.RenderCustomT(budgetCard, re.Env, re.Model.flKbk, readOnly: true);


                //                tbPaymentMatches.flOverpayment.RenderCustomT(re.Panel, re.Env, re.Model.flHasOverpayment, readOnly: true);

                //                if (re.Model.flHasSendOverpayment)
                //                {
                //                    tbPaymentMatches.flOverpaymentAmount.RenderCustomT(re.Panel, re.Env, re.Model.flOverpaymentAmount, readOnly: true);
                //                }

                //                tbPaymentMatches.flSendOverpayment.RenderCustomT(re.Panel, re.Env, re.Model.flHasSendOverpayment, readOnly: true);

                //                if (re.Model.flHasSendOverpayment)
                //                {
                //                    re.Panel.Append(new Br());
                //                    tbPaymentMatches.flOverpaymentSendAmount.RenderCustomT(re.Panel, re.Env, re.Model.flOverpaymentSendAmount, readOnly: true);
                //                    var returnCard = new Card("Реквизиты для возврата переплаты победителю").AppendTo(re.Panel);

                //                    tbRenderPaymentMatches.flOverpaymentXin.RenderCustomT(returnCard, re.Env, re.Model.flOverpaymentXin, readOnly: true);
                //                    tbRenderPaymentMatches.flOverpaymentName.RenderCustomT(returnCard, re.Env, re.Model.flOverpaymentName, readOnly: true);
                //                    tbRenderPaymentMatches.flOverpaymentBik.RenderCustomT(returnCard, re.Env, re.Model.flOverpaymentBik, readOnly: true);
                //                    tbRenderPaymentMatches.flOverpaymentIban.RenderCustomT(returnCard, re.Env, re.Model.flOverpaymentIban, readOnly: true);
                //                    tbRenderPaymentMatches.flOverpaymentKnp.RenderCustomT(returnCard, re.Env, re.Model.flOverpaymentKnp, readOnly: true);
                //                    tbRenderPaymentMatches.flOverpaymentKbe.RenderCustomT(returnCard, re.Env, re.Model.flOverpaymentKbe, readOnly: true);
                //                    //tbRenderPaymentMatches.flOverpaymentKbk.RenderCustomT(returnCard, re.Env, re.Model.flOverpaymentKbk, readOnly: true);
                //                }
                //            })
                //            .OnProcessing(pe =>
                //            {
                //                var paymentsProvider = new TraderesourcesPaymentsProvider(pe.Model.flOverpaymentXin, pe.Model.flOverpaymentName, pe.Env.RequestContext);
                //                var guaranteePaymentsList = paymentsProvider.GuaranteePayments.GetAllPayments().Where(x => pe.Model.flGuaranteePaymentItems.Any(y => y.flId.Id == x.Id.Id));
                //                var freePaymentsList = paymentsProvider.SellPayments.GetAllPayments().ToList();
                //                freePaymentsList.AddRange(guaranteePaymentsList);
                //                var choosenPaymentIds = pe.Model.flPaymentItemIds;
                //                var choosenPayments = freePaymentsList.Where(freePayment => choosenPaymentIds.Contains(freePayment.Id.Id.ToString())).ToArray();

                //                var commitPayments = choosenPayments.Select(x => new PaymentItemModel()
                //                {
                //                    flId = x.Id,
                //                    flDateTime = x.Date,
                //                    flAmount = x.Amount,
                //                    flPurpose = x.Purpose,
                //                    flIsGuarantee = guaranteePaymentsList.Any(gp => gp.Id.Id == x.Id.Id)
                //                }).ToArray();

                //                var tbPayments = new TbPayments()
                //                        .AddFilter(t => t.flAgreementId, pe.Args.AgreementId);
                                
                //                var tbPaymentsUpdateOrInsert = pe.Model.flIsUpdate ? tbPayments.Update() : tbPayments.Insert();
                //                tbPaymentsUpdateOrInsert
                //                    .SetT(t => t.flAgreementId, pe.Args.AgreementId)
                //                    .SetT(t => t.flPaymentId, pe.Model.flPaymentId)
                //                    .SetT(t => t.flPaymentStatus, pe.Model.flPaidAmount == pe.Model.flPayAmount ? PaymentStatus.Paid : PaymentStatus.Paying)
                //                    .SetT(t => t.flPayAmount, pe.Model.flPayAmount)
                //                    .SetT(t => t.flPaidAmount, pe.Model.flPaidAmount)
                //                    ;

                //                var tbPaymentMatches = new TbPaymentMatches();
                //                var paymentMatchId = tbPaymentMatches.flId.GetNextId(pe.Env.QueryExecuter);
                //                var tbPaymentMatchesInsert = tbPaymentMatches.Insert()
                //                    .SetT(t => t.flId, paymentMatchId)
                //                    .SetT(t => t.flPaymentId, pe.Model.flPaymentId)
                //                    .SetT(t => t.flDateTime, DateTime.Now)
                //                    .SetT(t => t.flPaymentItems, commitPayments)
                //                    .SetT(t => t.flStatus, PaymentMatchStatus.Linked)
                //                    .SetT(t => t.flAmount, pe.Model.flAmount)
                //                    .SetT(t => t.flGuaranteeAmount, pe.Model.flGuaranteeAmount)
                //                    .SetT(t => t.flRealAmount, pe.Model.flRealAmount)
                //                    .SetT(t => t.flHasSendAmount, pe.Model.flHasSendAmount)
                //                    .SetT(t => t.flSendAmount, pe.Model.flSendAmount)
                //                    .SetT(t => t.flRequisites, new RequisitesModel()
                //                    {
                //                        flName = pe.Model.flName,
                //                        flXin = pe.Model.flXin,
                //                        flBik = pe.Model.flBik,
                //                        flIban = pe.Model.flIban,
                //                        flKbe = pe.Model.flKbe,
                //                        flKnp = pe.Model.flKnp,
                //                        flKbk = pe.Model.flKbk,
                //                    })
                //                    .SetT(t => t.flOverpayment, pe.Model.flHasOverpayment)
                //                    .SetT(t => t.flSendOverpayment, pe.Model.flHasSendOverpayment)
                //                    ;

                //                if (pe.Model.flHasOverpayment)
                //                {
                //                    tbPaymentMatchesInsert
                //                        .SetT(t => t.flOverpaymentAmount, pe.Model.flOverpaymentAmount);
                //                } else
                //                {
                //                    tbPaymentMatchesInsert
                //                        .SetT(t => t.flOverpaymentAmount, 0);
                //                }

                //                if (pe.Model.flHasSendOverpayment)
                //                {
                //                    tbPaymentMatchesInsert
                //                        .SetT(t => t.flOverpaymentSendAmount, pe.Model.flOverpaymentSendAmount)
                //                        .SetT(t => t.flOverpaymentRequisites, new RequisitesModel()
                //                        {
                //                            flName = pe.Model.flOverpaymentName,
                //                            flXin = pe.Model.flOverpaymentXin,
                //                            flBik = pe.Model.flOverpaymentBik,
                //                            flIban = pe.Model.flOverpaymentIban,
                //                            flKbe = pe.Model.flOverpaymentKbe,
                //                            flKnp = pe.Model.flOverpaymentKnp,
                //                            flKbk = pe.Model.flOverpaymentKbk,
                //                            flContacts = pe.Model.flOverpaymentContacts,
                //                        })
                //                        ;
                //                } else
                //                {
                //                    tbPaymentMatchesInsert
                //                        .SetT(t => t.flOverpaymentSendAmount, 0);
                //                }

                //                var paymentIds = commitPayments.Where(x => !x.flIsGuarantee).Select(x => x.flId).ToArray();

                //                YodaHelpers.Payments.MatchResult matchResult = null;
                //                if (paymentIds.Length > 0)
                //                {
                //                    matchResult = paymentsProvider.SellPayments.MatchPayments(paymentIds, paymentMatchId.ToString(), "traderesources-agreements-payments-match");
                //                    tbPaymentMatchesInsert.SetT(t => t.flMatchResult, matchResult);
                //                }

                //                using (var trans = pe.Env.QueryExecuter.BeginTransaction("dbAgreements"))
                //                {
                //                    tbPaymentsUpdateOrInsert.Execute(pe.Env.QueryExecuter, trans);
                //                    tbPaymentMatchesInsert.Execute(pe.Env.QueryExecuter, trans);

                //                    trans.Commit();
                //                }

                //                if (paymentIds.Length > 0)
                //                {
                //                    paymentsProvider.SellPayments.SettlePayments(paymentIds, matchResult);
                //                }

                //                pe.Env.Redirect.SetRedirect(ModuleName, MenuName, new DefaultAgrTemplateArgs() { AgreementId = pe.Args.AgreementId, AgreementType = Actions.View });
                //            })
                //        ).Build()
                //    );
                //})
                ;
        }


        public class AgreementWizardModel {
            public AgreementWizardModel(string agreementModelsJson, string agreementType)
            {
                AgreementModelsJson = agreementModelsJson;
                AgreementType = agreementType;
            }
            public string AgreementModelsJson { get; set; }
            public string AgreementType { get; set; }
        }
    }
}
