function GetStyle(drawColor, drawWidth) {
    var drawStrokeColor = `${drawColor}`;
    var drawFillColor = `${drawColor}25`;

    var radius = 7;

    var fill = new ol.style.Fill({
        color: drawFillColor
    });
    var stroke = new ol.style.Stroke({
        color: drawStrokeColor,
        width: drawWidth
    });

    var styles = [
        new ol.style.Style({
            fill: fill,
            stroke: stroke,
            image: new ol.style.Circle({
                fill: fill,
                stroke: stroke,
                radius: radius
            }),
        })
    ];


    styles[ol.geom.Polygon] = [
        new ol.style.Style({
            fill: fill
        })
    ];
    styles[ol.geom.MultiPolygon] =
        styles[ol.geom.Polygon];
    styles[ol.geom.LineString] = [
        new ol.style.Style({
            stroke: stroke
        }),
        new ol.style.Style({
            stroke: stroke
        })
    ];
    styles[ol.geom.MultiLineString] =
        styles[ol.geom.LineString];
    styles[ol.geom.Point] = [
        new ol.style.Style({
            image: new ol.style.Circle({
                radius: radius,
                fill: fill,
                stroke: stroke
            }),
            zIndex: Infinity
        })
    ];
    styles[ol.geom.MultiPoint] =
        styles[ol.geom.Point];
    styles[ol.geom.GeometryCollection] =
        styles[ol.geom.Polygon].concat(
            styles[ol.geom.LineString],
            styles[ol.geom.Point]
        );

    return styles;
}

$(document).ready(function () {

    var targetId = 'object-geometry-viewer';
    var wkts = JSON.parse($(`#${targetId}`).attr("wkts"));
    var wktsNeighbours = JSON.parse($(`#${targetId}`).attr("wktsNeighbours"));


    var defaultBackGroundLayers = [];
    var layers = [];


    var googleLayer = new ol.layer.Tile({
        preload: Infinity,
        source: new ol.source.XYZ({ url: 'https://mt{0-3}.google.com/vt/lyrs=s,h&x={x}&y={y}&z={z}' })
    });
    defaultBackGroundLayers.push({ subLayers: [googleLayer], text: "Google", name: "google" });
    layers.push(googleLayer);

    var esriWi = new ol.layer.Tile({
        preload: Infinity,
        source: new ol.source.XYZ({
            url: 'https://services.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer/tile/{z}/{y}/{x}',
            maxZoom: 17
        })
    });
    defaultBackGroundLayers.push({ subLayers: [esriWi], text: "ESRI", name: "esri" });
    layers.push(esriWi);

    if (wkts.length > 0) {

        var drawColor = '#0acf97';

        var styles = GetStyle(drawColor, 5);

        var polygons = [];

        wkts.forEach(function (wkt) {
            var polygon =
                new ol.Feature({
                    geometry: new ol.format.WKT().readGeometry(
                        wkt,
                        {
                            dataProjection: 'EPSG:4326',
                            featureProjection: 'EPSG:3857'
                        }
                    )
                });
            polygons.push(polygon);
        });

        var vectorLayer = new ol.layer.Vector({
            source: new ol.source.Vector({
                features: polygons
            }),
            style: styles
        });
        layers.push(vectorLayer);
    }

    if (wktsNeighbours.length > 0) {

        var drawColorNeighbours = '#ffbc00';

        var stylesNeighbours = GetStyle(drawColorNeighbours, 2);

        var polygonsNeighbours = [];

        wktsNeighbours.forEach(function (wktNeighbour) {
            var polygonNeighbours =
                new ol.Feature({
                    geometry: new ol.format.WKT().readGeometry(
                        wktNeighbour,
                        {
                            dataProjection: 'EPSG:4326',
                            featureProjection: 'EPSG:3857'
                        }
                    )
                });
            polygonsNeighbours.push(polygonNeighbours);
        });

        var vectorLayerNeighbours = new ol.layer.Vector({
            source: new ol.source.Vector({
                features: polygonsNeighbours
            }),
            style: stylesNeighbours
        });
        layers.push(vectorLayerNeighbours);
    }

    var map = new ol.Map({
        controls: [
            new controlsLib.ChooseLayersControl({
                mapId: targetId,
                iconClass: "mdi mdi-layers-triple",
                legendName: "BackGroundLayersLegend",
                iconTitle: "Выбор подложек",
                legend: defaultBackGroundLayers
            })
        ],
        interactions: ol.interaction.defaults(),
        target: targetId,
        layers: layers,
        view: new ol.View({
            center: [7951137.118857781, 6651040.097882961],
            zoom: 5.5
        })
    });

    $(`#${targetId}`).append(`
        <div class="object-area ol-unselectable ol-control" id="object-area"></div>
    `);
    var area = 0;
    if (vectorLayer.getSource().getFeatures().length == 1) {
        if (vectorLayer.getSource().getFeatures()[0].getGeometry().getType() == "MultiPolygon") {
            area = vectorLayer.getSource().getFeatures()[0].getGeometry().getArea() / 20000;
        } else {
            new ol.geom.MultiPolygon([vectorLayer.getSource().getFeatures()[0].getGeometry()]).getArea() / 20000;
        }
    } else {
        area = new ol.geom.MultiPolygon(Array.from(vectorLayer.getSource().getFeatures(), feature => feature.getGeometry())).getArea() / 20000;
    }
    var areaText = `~ ${(area).toFixed(4)} га`;
    $(`#object-area`).html(areaText);

    var extentGeom = ol.geom.Polygon.fromExtent(vectorLayer.getSource().getExtent());
    extentGeom.scale(3);
    map.getView().fit(extentGeom, map.getSize());

});