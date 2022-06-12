using HydrocarbonSource.QueryTables.Application;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yoda.Application.Helpers;
using Yoda.Interfaces;
using Yoda.Interfaces.Helpers;
using YodaHelpers;
using YodaHelpers.HtmlDocumentBuilder;
using YodaQuery;

namespace TradeResourcesPlugin.Helpers {
    public class PdfBuilder {
        private string _mainContent;
        private readonly StringBuilder _sb = new StringBuilder();
        private readonly StringBuilder _sbHeader = new StringBuilder();
        private readonly StringBuilder _sbFooter = new StringBuilder();


        public PdfBuilder(string content) {
            _mainContent = content ?? throw new ArgumentNullException("content");
        }

        private TextValue[] _sysItems;
        public PdfBuilder SysHeader(params TextValue[] items) {
            _sysItems = items;
            return this;
        }

        private SignItem[] _signs;
        public PdfBuilder Signs(params SignItem[] signs) {
            _signs = signs;
            return this;
        }

        private AppendixData[] _appendices = new AppendixData[] { };
        public PdfBuilder Appendices(params AppendixData[] appendices) {
            _appendices = appendices ?? new AppendixData[] { };
            return this;
        }

        private string[] _uiPackages = new string[] { };
        public PdfBuilder UiPackages(params string[] uiPackages) {
            _uiPackages = uiPackages ?? new string[] { };
            return this;
        }


        private bool tryGetSignsInfo(out string signsHtml) {
            if (_signs == null) {
                signsHtml = null;
                return false;
            }
            var ret = new StringBuilder();
            ret.Append("<div style='page-break-before: always'></div>");
            foreach (var sign in _signs) {
                var signText = $@"             
            <div class='app-content' style='color: #000; text-align: left; padding-left: 0px; '>
<table>
    <tr>
        <td>
            <div style='float: left; width: 144px;'>
                        <img src='data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAJcAAABjCAYAAACWnJJ2AAAACXBIWXMAAAsSAAALEgHS3X78AAADRElEQVR4nO3dTY7TQBCG4RrEjg0H8Q2QEHfhXNwFIXEDH2Q2rIOChJREbveP6yuX7feRWEUOM/Y77U577Hm73W4GKHxgr0KFuCBDXJAhLsgQF2Q+9rzxPM98tLy4aZreWvdA01IEUeFVS2TV0yJhYUlLF6txERbW1PpgQg+ZYlyMWtiKkQubrA1CXUsR2f389bv4FX77+oWKgrnGtXZwlywd8Jb3GNnu/jqBxUp1WuyNs3e70ffHGLeRiwNXN01T9i/xyTzPm7Z3G7mOcMrhtBgr1Wlx9OATTU7Fa4usc/k762mxdJ2RdS7IEBdkiAsyxAUZ4rqQT9/f//2LQlwX8RhVVGDEdQFLMUUE5nb552hrOL22XgrZy1pE99f+/Pgs+8oYuU6sZXRSjmDEdVI90agCI64TGolFEdipfhP1LB4PdO+caDQSxdyLkSuZ1zgiTm+qST1xJVKKQzkx59PiBdTiqC0pjFCGZcSVQ2scnouh6rCMuPbXG4fHZZyIsIxPi8e0ZdkgKizzjOuol0f2dj/YUReSI8MyTos5RBz06LCMuPJQHvw9wjLiykURwV5hGXHl4xnDnmEZceXkEcXeYRlx5bUljgxhGXHlNhJJlrBMtYg68sSb2vMeau951udF9KyDZQrL9n7429K2r5H0PnvrjJG1BJYtLPM8LfJ8Lq21eDKGZcy5jmUpoqxh2dUe/nYGjzFlDsuO8PC3nmivEvg9quxhmfeEXnVwGRWPiTkXZIgLMsQFGfcV+tG/gNGy/eh2tW2h4fY0Z49LPq3v4bVdNJ7mDDjh7p9AV7uJhZELMrtd/hldjS9tV9uWCX08/jwLNmNCj3DEBRniggxxQYa4IENckCEuyKS5tQw5eC42c2sZnngeR06LkCEuyBAXZLgpFk88j+Mh7lvEMXFahAxxQYbfob+Y0Vv3RqRcoV/6Jo9y+1hN7z4a3Rcj291fP/0KPav9ZaP7pvcJjR6YcwWK/KHJ8ANKXIEiT9mv/9ce04WUi6isl5Wp943n+3NrGTbj1jKEIy5sUhq1bC2utY2AFoxcGFYbgFbjYvRCSUsbxU+Lr/j0iP9aB53muIBezLkgQ1yQIS7IEBdkiAsyxAUNM/sL5ShFEMLPHOUAAAAASUVORK5CYII=' width='150px'/>
             </div>
        </td>
        <td>
            <div style='font-size: 11pt;padding-top: 5px;padding-bottom: 5px;padding-left: 20px;font-weight: bold;'>
                {sign.SignTypeText.ToHtml()}
            </div>
            <div style='font-size: 11pt; width: 600px; padding-left: 20px;'>
                Дата и время подписи: {sign.SignDate:dd.MM.yyyy HH:mm:ss}
            </div>
            <div style='font-size: 11pt; width: 600px; padding-left: 20px;'>
                {new DocumentBuilder.SignInfo(sign.CertInfo).DisplayHtmlText}
            </div>
        </td>
    </tr>
</table>
            
           

            
        </div>
</br></br>";
                ret.Append(signText);
            }
            signsHtml = ret.ToString();
            return true;

        }

        public PdfBuilder SetHeader() {
            return this;
        }

        public async Task<byte[]> Build(IYodaRequestContext context) {
            var pdfConfigList = new List<PrintPdfConfig>();

            var sbMainPdf = new StringBuilder();
            sbMainPdf.Append(_sbHeader);
            sbMainPdf.Append(_mainContent + _sbFooter);

            if (tryGetSignsInfo(out var signsHtml)) {
                sbMainPdf.Append(signsHtml);
            }

            pdfConfigList.Add(new PrintPdfConfig(sbMainPdf.ToString(), PageOrientation.Portrait));

            int appendixNum = 0;
            foreach (var appendix in _appendices) {
                if (appendix.HtmlContent != null) {
                    appendixNum++;
                    pdfConfigList.Add(new PrintPdfConfig(appendix.HtmlContent, appendix.Orientation, $"P{appendixNum} - "));
                }
                else {
                    pdfConfigList.Add(new PrintPdfConfig(appendix.Pdf));
                }
            }

            var pdfList = new List<byte[]>();

            foreach (var pdfConfig in pdfConfigList) {
                var module = context.ModulesProvider.GetModule("RegistersModule");
                var path = context.ModulesProvider.GetModuleLoadedDirectory(module);
                var absolutePath = context.ServerMapPath(path);
                var pathToFooter = Path.Combine(absolutePath, "ui-packages", "agreement-style", "footer.html");

                //var wkCustomArgs = $"-L 10mm -R 10mm -T 10mm -B 15mm --footer-html {pathToFooter} --header-font-size 7 --header-right \"{pdfConfig.PagePrefix}[page] / [topage]\"";
                var wkCustomArgs = $"-L 5mm -R 5mm -T 5mm -B 10mm --header-font-size 7 --header-right \"{pdfConfig.PagePrefix}[page] / [topage]\"";
                if (pdfConfig.Orientation == PageOrientation.Landscape) {
                    wkCustomArgs += $" -O landscape";
                }


                var uiPackages = new List<string>();
                //uiPackages.Add(DocumentBuilder.DocumentBuilderStylesUiPackages);
                //uiPackages.Add("");
                //uiPackages.AddRange(new[] {
                //"subsidies-fertilizer-application-pdf-table-fix",
                //"esf-items-css"
                //});

                uiPackages.AddRange(_uiPackages);

                //var wkhtmlPath = HtmlToPdfPrinter.GetWkthmlToPdfExePath(context.Configuration);

                //if (!HtmlToPdfPrinter.GeneratePdf(
                //        context,
                //        pdfConfig.HtmlContent,
                //        out var pdf,
                //        HtmlContentType.Body,
                //        uiPackages: uiPackages.ToArray(),
                //        wkCustomArgs: wkCustomArgs
                //    )
                //) {
                //    throw new Exception("Не удалось сгенерировать pdf");
                //}

                var args = new {
                    ContentCache = $"pdf-builder-{context.User.Id}-{DateTime.Now.ToString("dd-MM-yyyy-hh-mm")}",
                    ContentUiPackagesCache = $"UI-Packages-pdf-builder-{context.User.Id}-{DateTime.Now.ToString("dd-MM-yyyy-hh-mm")}"
                };

                context.Cache.Set(args.ContentCache, pdfConfig.HtmlContent);
                context.Cache.Set(args.ContentUiPackagesCache, uiPackages.ToArray());

                using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { ExecutablePath = context.Configuration["ChromiumPath"], Headless = true });
                using var page = await browser.NewPageAsync();
                await page.GoToAsync(context.GetUrlHelper().YodaAction("RegistersModule", "DefaultDocPdfMnu", args, urlWithSchema: true));
                Thread.Sleep(2000);
                var pdf = await page.PdfDataAsync(new PdfOptions() {
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

                context.Cache.Remove(args.ContentCache);
                context.Cache.Remove(args.ContentUiPackagesCache);

                pdfList.Add(pdf);
            }

            if (pdfList.Count == 1) {
                return pdfList[0];
            }

            return PdfMerger.MergePdfFiles(pdfList.ToArray());
        }

        public static byte[] ImageToByte(Bitmap img) {
            using (var ms = new MemoryStream()) {
                img.Save(ms, img.RawFormat);
                return ms.ToArray();
            }
        }

        class PrintPdfConfig {
            public PrintPdfConfig(string htmlContent, PageOrientation orientation = PageOrientation.Portrait, string pagePrefix = null) {
                HtmlContent = htmlContent;
                Orientation = orientation;
                PagePrefix = pagePrefix;
            }

            public PrintPdfConfig(byte[] pdf) {
                Pdf = pdf;
            }

            public byte[] Pdf { get; }

            public string PagePrefix { get; }
            public PageOrientation Orientation { get; }
            public string HtmlContent { get; }
        }
    }


    public class SignItem {
        public SignItem(string signTypeText, DateTime signDate, string certInfo) {
            SignTypeText = signTypeText;
            SignDate = signDate;
            CertInfo = certInfo;
        }

        public string SignTypeText { get; }
        public DateTime SignDate { get; }
        public string CertInfo { get; }
    }
    public class AppendixData {
        public AppendixData(string htmlContent, PageOrientation pageOrientation = PageOrientation.Portrait) {
            HtmlContent = htmlContent;
            Orientation = pageOrientation;
        }

        public AppendixData(byte[] pdf) {
            Pdf = pdf;
        }

        public PageOrientation Orientation { get; } = PageOrientation.Portrait;
        public byte[] Pdf { get; }
        public string HtmlContent { get; }
    }
    public enum PageOrientation {
        Portrait,
        Landscape
    }
    public class TextValue {
        public TextValue(string text, string value) {
            Text = text;
            Value = value;
        }
        public string Text { get; }
        public string Value { get; }
        public TextValue HideOnEmpty(bool hide = true) {
            HideEmpty = hide;
            return this;
        }
        public bool HideEmpty { get; private set; }
    }

    public static class PdfBuilderHelpers {
        public static PdfBuilder SysHeader<TQuery>(this PdfBuilder pdfBuilder, SelectFirstResultProxy<TQuery> r, Func<TQuery, Field[]> sysFields, IYodaRequestContext context) where TQuery : IQueryItem {
            var fields = sysFields(r.Query);

            TextValue getTextField(Field field) {
                var valText = field.GetDisplayText(field.GetRowVal<object>(r.FirstRow), context);
                return new TextValue(field.Text.Text, valText).HideOnEmpty(field.IsHideOnEmpty());
            }

            var sysItems = fields.Select(f => getTextField(f)).ToArray();
            return pdfBuilder.SysHeader(sysItems);
        }

        public static PdfBuilder Signs<TQuery>(this PdfBuilder pdfBuilder, SelectResultProxy<TQuery> signsResult, IYodaRequestContext context) where TQuery : IQueryItem, ITbAppSigns {
            var signs = signsResult.DataTable.Rows.Cast<DataRow>().Select(r =>
                new SignItem(signsResult.Query.flSignerType.GetDisplayText(signsResult.Query.flSignerType.GetVal(r), context),
                    signsResult.Query.flSignDate.GetVal(r),
                    signsResult.Query.flCertInfo.GetVal(r))
                    ).ToArray();
            return pdfBuilder.Signs(signs);
        }


        public static bool IsHideOnEmpty(this Field field) {
            return field.GetMeta("HideOnEmpty", false);
        }

        private static TV GetMeta<T, TV>(this T field, string key, TV defVal) where T : Field {
            if (!field.MetaData.ContainsKey(key)) {
                return defVal;
            }
            try {
                return (TV)Convert.ChangeType(field.MetaData[key], typeof(TV));
            }
            catch {
                return defVal;
            }
        }
    }
}