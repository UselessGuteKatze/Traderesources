var mapConfig = {
    defaultSrs: "EPSG:900913",
    mapBounds: [5175907, 4921291, 9719922, 7470000],
    defaultCenter: [7447914, 6167051],
    minZoom: 4,
    maxZoom: 19,
    mapGwcWmsUrl: "https://maps.minagro.kz/geoserver/gwc/service/wms",
    mapGwcWmsUrl2: "https://maps2.minagro.kz/geoserver/gwc/service/wms",
    //mapGwcWmsUrl: "https://maps.minagro.kz/geoserver/wms",
    mapGwcWmtsUrl: "https://maps.minagro.kz/geoserver/gwc/service/wmts",
    //mapGwcWmsUrl: "https://maps.minagro.kz/geoserver/wms"
};
window.mapConfig = mapConfig;

window.gisApp = {
    mapConfig: mapConfig,
    controls: {},
    layers: {},
    views: {},
    palettes: {},
    constants: {},
};
var gisApp = window.gisApp;

gisApp.constants.longLine = "—";

gisApp.views.getDefaultView = function() {
    return new ol.View({
        center: mapConfig.defaultCenter,
        zoom: mapConfig.minZoom,
        projection: mapConfig.defaultSrs,
        extent: mapConfig.mapBounds,
        minZoom: mapConfig.minZoom,
        maxZoom: mapConfig.maxZoom
    });
}

gisApp.layers.osm = function () {
    return new ol.layer.Tile({ source: new ol.source.OSM() });
};
gisApp.layers["osm-gray"] = function () {
    var ret = new ol.layer.Tile({
        preload: Infinity,
        source: new ol.source.XYZ({
            url: 'https://tiles.wmflabs.org/bw-mapnik/{z}/{x}/{y}.png',
            attributions: 'Map Tiles &copy; ' + new Date().getFullYear() + ' ' +
            '<a href="http://developer.here.com">HERE</a>'
        })
    });
    return (ret);
};
gisApp.layers["esri-wi"] = function () {
    var ret = new ol.layer.Tile({
        preload: Infinity,
        source: new ol.source.XYZ({
            url: 'https://services.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer/tile/{z}/{y}/{x}',
            maxZoom: 17
        })
    });
    return (ret);
};
gisApp.layers.topomap = function () {
    var ret = new ol.layer.Tile({
        preload: Infinity,
        source: new ol.source.XYZ({
            url: 'https://{a-c}.tile.opentopomap.org/{z}/{x}/{y}.png',
            attributions: 'Opentopomap'
        })
    });
    return (ret);
};
gisApp.layers.here = function () {
    var ret = new ol.layer.Tile({
        preload: Infinity,
        source: new ol.source.XYZ({                                                       //iuc_kz key
            url: 'https://{1-4}.aerial.maps.cit.api.here.com/maptile/2.1/maptile/newest/satellite.day/{z}/{x}/{y}/256/png?app_id=BIRn5bTDwNRWr5kiDIHZ&app_code=_9lEVNApJi5rmfF-jpFxdg',
            attributions: 'Map Tiles &copy; ' + new Date().getFullYear() + ' ' +
            '<a href="http://developer.here.com">HERE</a>'
        })
    });
    return (ret);
};
gisApp.layers["here-hyb"] = function () {
    var ret = new ol.layer.Tile({
        preload: Infinity,
        source: new ol.source.XYZ({                                                       //iuc_kz key
            url: 'https://{1-4}.aerial.maps.cit.api.here.com/maptile/2.1/maptile/newest/hybrid.day/{z}/{x}/{y}/256/png?app_id=BIRn5bTDwNRWr5kiDIHZ&app_code=_9lEVNApJi5rmfF-jpFxdg&lg=rus',
            attributions: 'Map Tiles &copy; ' + new Date().getFullYear() + ' ' +
            '<a href="http://developer.here.com">HERE</a>'
        })
    });
    return (ret);
};
gisApp.layers["bing"] = function () {
    var ret = new ol.layer.Tile({
        preload: Infinity,
        source: new ol.source.BingMaps({
            key: 'Ar9AE3biIFnvn0bMBN6TSgXCZ67hgQJniric_O-0QwHYqZ4j7z4daPAIn8WI8onq',        //iuc_kz key
            imagerySet: "Aerial"
        })
    });
    return (ret);
};
gisApp.layers["bing-hyb"] = function () {
    var ret = new ol.layer.Tile({
        preload: Infinity,
        source: new ol.source.BingMaps({
            key: 'Ar9AE3biIFnvn0bMBN6TSgXCZ67hgQJniric_O-0QwHYqZ4j7z4daPAIn8WI8onq',        //iuc_kz key
            imagerySet: "AerialWithLabels"
        })
    });
    return (ret);
};
gisApp.layers["modis-surf-temp-day"] = function () {
    var ret = new ol.layer.Tile({
        preload: Infinity,
        source: new ol.source.XYZ({
            url:'https://gibs-a.earthdata.nasa.gov/wmts/epsg3857/best/wmts.cgi?TIME=2017-07-03&layer=MODIS_Terra_Land_Surface_Temp_Day&style=default&tilematrixset=GoogleMapsCompatible_Level7&Service=WMTS&Request=GetTile&Version=1.0.0&Format=image%2Fpng&TileMatrix={z}&TileCol={x}&TileRow={y}'
        })
    });
    return (ret);
};
gisApp.layers["google-sat"] = function () {
    return new ol.layer.Tile({
        preload: Infinity,
        source: new ol.source.XYZ({url: 'https://mt{0-3}.google.com/vt/lyrs=s&x={x}&y={y}&z={z}'})
    });
};
gisApp.layers["google-ter"] = function () {
    return new ol.layer.Tile({
        preload: Infinity,
        source: new ol.source.XYZ({ url: 'https://mt{0-3}.google.com/vt/lyrs=p&x={x}&y={y}&z={z}' })
    });
};
gisApp.layers["google-hyb"] = function () {
    return new ol.layer.Tile({
        preload: Infinity,
        source: new ol.source.XYZ({ url: 'https://mt{0-3}.google.com/vt/lyrs=s,h&x={x}&y={y}&z={z}' })
    });
};

gisApp.layers["airbus-pavlodar"] = function () {
    var imageFormat = 'image/png';
    var baseUrl = "https://maps.qoldau.kz/airbus-pavlodar";
    var layerName = "0";
    var srs = "EPSG:3857";
    var mapSource = new ol.source.TileWMS({
        url: baseUrl,
        params: { 'LAYERS': layerName, 'SRS': srs, 'FORMAT': imageFormat, 'VERSION':'' },
        projection: srs
    });
    var ret = new ol.layer.Tile({
        visible: true,
        source: mapSource
    });
    return ret;
};

gisApp.layers.getLayer = function (name) {
    var layers = gisApp.layers;
    return layers[name]();
};

gisApp.views.shared = {};
gisApp.views.getSharedView = function (viewName) {
    if (!gisApp.views.shared[viewName]) {
        gisApp.views.shared[viewName] = gisApp.views.getDefaultView();
    }
    return gisApp.views.shared[viewName];
};

ol.Map.prototype.deactivateControls = function (sender) {
    var map = this;
    var controls = map.getControls();
    for (var i = 0; i < controls.getLength(); i++) {
        var curControl = controls.item(i);
        if (curControl.onControlsDeactivate) {
            curControl.onControlsDeactivate(sender);
        }
    }
};


gisApp.layerStores = {
    defaultStoreCode: "geoserver1",

    geoserver1: {
        type: "geoserver",
        baseUrl: "https://maps.qoldau.kz/gs1"
    },
    geoserver2: {
        type: "geoserver",
        baseUrl: "https://maps.qoldau.kz/gs2"
    },
    geoserver4: {
        type: "geoserver",
        baseUrl: "https://maps.qoldau.kz/gs4"
    },
    geoserver8: {
        type: "geoserver",
        baseUrl: "https://maps.qoldau.kz/gs3"
    },
    geoserver9: {
        type: "geoserver",
        baseUrl: "https://maps.qoldau.kz/gs4"
    },
    filestore1: {
        type: "filestore",
        baseUrl: "https://maps.qoldau.kz/fs1/"
    },
    airbus1: {
        type: "airbus",
        baseUrl: "https://maps.qoldau.kz/airbus1/"
    }
}

gisApp.layerStores.getLayer = function (layerNameWithStoreCode, imageFormat, extent3857, zIndex, crossOrigin) {
    var defaultStoreCode = gisApp.layerStores.defaultStoreCode;
    var layerStores = gisApp.layerStores;
    var layerName = layerNameWithStoreCode;
    var nameParts = (layerName || "").split("@");
    zIndex = zIndex === undefined ? -100 : zIndex;
    var storeCode = null;
    if (nameParts.length === 2) {
        layerName = nameParts[1];
        storeCode = nameParts[0];
    }

    var layerStore = layerStores[storeCode || defaultStoreCode];
    if (!layerStore) {
        throw "Layer Store not found: " + storeCode;
    }
    var srs = window.gisApp.mapConfig.defaultSrs;
    var baseUrl = "";
    switch (layerStore.type) {
    case "geoserver": {
        imageFormat = imageFormat || 'image/jpeg';
        baseUrl = layerStore.baseUrl;
        var mapSource = new ol.source.TileWMS({
            url: baseUrl,
            params: { 'LAYERS': layerName, 'srs': srs, 'format': imageFormat, 'BGCOLOR': '0x000000' },
            projection: srs,
            crossOrigin: crossOrigin
        });
        var ret = new ol.layer.Tile({
            visible: true,
            zIndex: zIndex,
            source: mapSource
        });
        return ret;
    }
    case "filestore": {
        baseUrl = layerStore.baseUrl;
        if (!baseUrl.endsWith("/")) {
            baseUrl += "/";
        }
        return new ol.layer.Image({
            source: new ol.source.ImageStatic({
                url: baseUrl + layerName,
                projection: srs,
                imageExtent: extent3857,
                crossOrigin: crossOrigin
            })
        });
    }
    }
    throw "Unknown layer store type: " + layerStore.type;
}