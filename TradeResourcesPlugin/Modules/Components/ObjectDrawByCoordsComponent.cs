using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Yoda.Interfaces.Forms;
using Yoda.Interfaces.Forms.Components;

namespace TradeResourcesPlugin.Modules.Components {
    public class ObjectDrawByCoordsComponent: YodaFormElement {

        private string _wktInputName = "";
        private string _coordsInputName = "";
        private string _backgroundParentWKT = null;
        private string _backgroundOldVersionWKT = null;
        private string[]? _backgroundWKTs = null;

        public ObjectDrawByCoordsComponent(string wktInputName, string coordsInputName, string[] backgroundWKTs = null, string backgroundParentWKT = null, string backgroundOldVersionWKT = null)
        {
            _wktInputName = wktInputName;
            _coordsInputName = coordsInputName;
            _backgroundParentWKT = backgroundParentWKT;
            _backgroundOldVersionWKT = backgroundOldVersionWKT;
            _backgroundWKTs = backgroundWKTs;
        }

        public override string[] GetRequireUiPackages()
        {
            return new[] { "object-draw-by-coords" };
        }
        public override HtmlString ToHtmlString(IHtmlHelper html)
        {
            var panel = new Panel {
                Name = "object-draw-by-coords",
                CssClass = "object-draw-by-coords " + CssClass,
                Attributes = new Dictionary<string, object> {
                { "target-wkt",  _wktInputName },
                { "target-coords",  _coordsInputName },
                { "background-parent-wkt",  JsonConvert.SerializeObject(_backgroundParentWKT) },
                { "background-old-version-wkt",  JsonConvert.SerializeObject(_backgroundOldVersionWKT) },
                { "background-wkts",  JsonConvert.SerializeObject(_backgroundWKTs) },
            }
            }.ToHtmlString(html);
            return panel;
        }

    }
}
