using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Yoda.Interfaces.Forms;
using Yoda.Interfaces.Forms.Components;

namespace TradeResourcesPlugin.Modules.Components {
    public class ObjectGeometryViewerComponent: YodaFormElement {

        private string[] _wkts = new string[] { };
        private string[] _wktsNeighbours = new string[] { };

        public ObjectGeometryViewerComponent(string wkt, string cssClass = "", string[] wktsNeighbours = null)
        {
            _wkts = new string[] { wkt };
            if (wktsNeighbours != null) {
                _wktsNeighbours = wktsNeighbours;
            }
            CssClass = cssClass;
        }
        public ObjectGeometryViewerComponent(string[] wkts, string cssClass = "", string[] wktsNeighbours = null)
        {
            _wkts = wkts;
            if (wktsNeighbours != null) {
                _wktsNeighbours = wktsNeighbours;
            }
            CssClass = cssClass;
        }

        public override string[] GetRequireUiPackages()
        {
            return new[] { "object-geometry-viewer" };
        }
        public override HtmlString ToHtmlString(IHtmlHelper html)
        {
            var panel = new Panel {
                Name = "object-geometry-viewer",
                CssClass = $"object-geometry-viewer " + CssClass,
                Attributes = new Dictionary<string, object> {
                { "wkts",  JsonConvert.SerializeObject(_wkts) },
                { "wktsNeighbours",  JsonConvert.SerializeObject(_wktsNeighbours) },
            }
            }.ToHtmlString(html);
            return panel;
        }

    }

}
