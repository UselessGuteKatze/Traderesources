using UsersResources;
using Yoda.Interfaces.Forms;
using Yoda.Interfaces.Forms.Components;
using Yoda.Interfaces.Helpers;
using Yoda.Interfaces.Menu;
using YodaQuery;

namespace TradeResourcesPlugin.Helpers {
    public class DefaultDocPdfMnuArgs {
        public string ContentUiPackagesCache { get; set; }
        public string ContentCache { get; set; }
    }
    public class DefaultDocPdfMnu : FrmMenu<DefaultDocPdfMnuArgs> {
        public DefaultDocPdfMnu(string moduleName) : base(nameof(DefaultDocPdfMnu), "Реестр")
        {
            AsCallback();
            ViewName("Doc");
            Enabled((rc) => {
                return true;
            });
            OnRendering(re => {

                re.Form.AddComponent(new UiPackages(re.RequestContext.Cache.Get<string[]>(re.Args.ContentUiPackagesCache)));
                re.Form.AddComponent(new HtmlText(re.RequestContext.Cache.Get<string>(re.Args.ContentCache)));

            });
        }
    }
}
