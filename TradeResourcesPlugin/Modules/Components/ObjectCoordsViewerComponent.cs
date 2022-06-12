using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Yoda.Interfaces.Forms;
using Yoda.Interfaces.Forms.Components;

namespace TradeResourcesPlugin.Modules.Components {
    public class ObjectCoordsViewerComponent: YodaFormElement {

        private string _coords = "";
        private string _typeClass = "";

        public ObjectCoordsViewerComponent(List<LandSource.QueryTables.LandObject.TbLandObjectsBase.Coords> coords, string typeClass = "")
        {
            _coords = JsonConvert.SerializeObject(coords);
            _typeClass = typeClass;
        }

        public ObjectCoordsViewerComponent(List<FishingSource.QueryTables.Object.TbObjectsBase.Coords> coords, string typeClass = "")
        {
            _coords = JsonConvert.SerializeObject(coords);
            _typeClass = typeClass;
        }

        public ObjectCoordsViewerComponent(List<ForestSource.QueryTables.Object.TbForestriesBase.Coords> coords, string typeClass = "")
        {
            _coords = JsonConvert.SerializeObject(coords);
            _typeClass = typeClass;
        }

        public ObjectCoordsViewerComponent(List<ForestSource.QueryTables.Object.TbForestryPiecesBase.Coords> coords, string typeClass = "")
        {
            _coords = JsonConvert.SerializeObject(coords);
            _typeClass = typeClass;
        }

        public ObjectCoordsViewerComponent(List<ForestSource.QueryTables.Object.TbQuartersBase.Coords> coords, string typeClass = "")
        {
            _coords = JsonConvert.SerializeObject(coords);
            _typeClass = typeClass;
        }

        public ObjectCoordsViewerComponent(List<FishingSource.QueryTables.Reservoir.TbReservoirsBase.Coords> coords, string typeClass = "")
        {
            _coords = JsonConvert.SerializeObject(coords);
            _typeClass = typeClass;
        }

        public ObjectCoordsViewerComponent(List<HuntingSource.QueryTables.Object.TbObjectsBase.Coords> coords, string typeClass = "")
        {
            _coords = JsonConvert.SerializeObject(coords);
            _typeClass = typeClass;
        }

        public ObjectCoordsViewerComponent(List<TelecomOperatorsSource.QueryTables.Object.TbObjectsBase.Coords> coords, string typeClass = "")
        {
            _coords = JsonConvert.SerializeObject(coords);
            _typeClass = typeClass;
        }

        public override string[] GetRequireUiPackages()
        {
            return new[] { "object-coords-viewer" };
        }
        public override HtmlString ToHtmlString(IHtmlHelper html)
        {
            var panel = new Panel {
                Name = "object-coords-viewer",
                CssClass = $"object-coords-viewer {_typeClass} " + CssClass,
                Attributes = new Dictionary<string, object> {
                { "coords",  _coords },
            }
            }.ToHtmlString(html);
            return panel;
        }

    }
}
