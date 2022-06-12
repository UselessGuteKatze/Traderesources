$(document).ready(function () {

    window.layersLib = {};
    window.layersLib;
    layersLib["osm"] = function () {
        var ret = new ol.layer.Tile({
            preload: Infinity,
            source: new ol.source.OSM()
        });
        ret.text = "OSM светлый";
        ret.name = "osm";
        return (ret);
    };
    layersLib["osmGray"] = function () {
        var ret = new ol.layer.Tile({
            preload: Infinity,
            source: new ol.source.XYZ({
                url: 'https://tiles.wmflabs.org/bw-mapnik/{z}/{x}/{y}.png',
                attributions: 'Map Tiles &copy; ' + new Date().getFullYear() + ' ' +
                    '<a href="http://developer.here.com">HERE</a>'
            })
        });
        ret.text = "OSM серый";
        ret.name = "osmGray";
        return (ret);
    };
    layersLib["osmDark"] = function () {
        var ret = new ol.layer.Tile({
            className: "bg-white",
            preload: Infinity,
            source: new ol.source.OSM({ "url": "http://{a-c}.basemaps.cartocdn.com/dark_all/{z}/{x}/{y}.png" }),
            opacity: 0.8
        });
        ret.text = "OSM тёмный";
        ret.name = "osmDark";
        return (ret);
    };
    layersLib["google"] = function () {
        var ret = new ol.layer.Tile({
            preload: Infinity,
            source: new ol.source.XYZ({ url: 'https://mt{0-3}.google.com/vt/lyrs=s&x={x}&y={y}&z={z}' })
        });
        ret.text = "Google";
        ret.name = "google";
        return (ret);
    };
    layersLib["airbus"] = function () {
        var apiKey = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJzdWIiOiI2OGUwM2RiYy1jNmU1LTQzYmQtYjU2YS1lNmFlNjM0OTI2ZjMiLCJhdWQiOiJodHRwczovL29uZWF0bGFzLmRhdGFkb29ycy5uZXQvaWQvcmVzb3VyY2VzIiwiaWRwIjoiVUNBX0FERlMiLCJhdXRoX3RpbWUiOjE2MDYzOTgwMjIsImFtciI6ImV4dGVybmFsIiwic2NvcGUiOlsib3BlbmlkIiwicHJvZmlsZSIsImVtYWlsIiwicm9sZXMiLCJyZWFkIiwid3JpdGUiXSwiaXNzIjoiaHR0cHM6Ly9vbmVhdGxhcy5kYXRhZG9vcnMubmV0L2lkIiwiZXhwIjoxNjM4MTA2ODIyLCJjbGllbnRfaWQiOiJwdGVzdF9leGFtcGxlXzEifQ.cAZLoWDeVT3Su2c3vrk4wWCEYUjzY4-S7U13sf7QFLZbozByvUC7-1eZS6oH7LM6BqAURMsw2TjOskuNfYnBWXjYF5sWgTZ7jvKwlp-oEliqBTOwOkhinihuFwPxOLtS7bnCb3ASmrnHpMBYm2xnqyTJLT8KuB6FWPbkcSJYpJI7JYL_3DsbSLd5JJTtQKLt73bjAL4v1J6pNjBSGkv1yrHuuudqyW93wxyI0X4E3zFMgYd_v-73Uza1lTE8AksR1AIK5oE8luHP3g7eNhwx4IRUkkqcwwuz1l3mbws8x6ujo63k-5Esk25nuaCNzEKdmPEwSzOaVbEuobBWmJQFOA";
        var WMTSUrl = "https://view.geoapi-airbusds.com/api/v1/map/imagery.wmts";
        var projection = ol.proj.get('EPSG:3857');
        var projectionExtent = projection.getExtent();
        var size = (ol.extent.getWidth(projectionExtent)) / 256;
        var resolutions = Array.from(Array(18), (item, index) => size / Math.pow(2, index));
        var matrixIds = Array.from(Array(18), (item, index) => index);

        var ret = new ol.layer.Tile({
            preload: Infinity,
            source: new ol.source.OneAtlas({
                url: WMTSUrl,
                format: 'image/png',
                matrixSet: "3857",
                tileGrid: new ol.tilegrid.WMTS({
                    origin: ol.extent.getTopLeft(projectionExtent),
                    resolutions: resolutions,
                    matrixIds: matrixIds
                }),
                apiKey: apiKey
            })
        });
        ret.text = "Airbus Basemap";
        ret.name = "airbus";
        return ret;
    };

    InitYodaGeomRenderers();

});

function InitYodaGeomRenderers() {
    $(".yoda-geom-renderer").each(function () {
        var $this = $(this);
        var geomRendererInputId = `${$this.attr("id-input")}`;
        var wkt = $this.attr("wkt");
        var hasValue = wkt != null && wkt.length > 0;
        $this.html(`<input type="hidden" id="${geomRendererInputId}" name="${geomRendererInputId}" class="d-none" style="display: none;"></input>`);
        $(`#${geomRendererInputId}`).val(wkt);
        var geomRendererId = `${$this.attr("id")}`;
        var srid = $this.attr("srid");
        var strokeWidth = parseInt($this.attr("strokeWidth"));
        var strokeColor = $this.attr("strokeColor");
        var fillColor = $this.attr("fillColor");
        var radius = parseInt($this.attr("radius"));
        var background = $this.attr("background");
        var rounded = $this.attr("rounded") == "True";
        //var setZoom = parseInt($this.attr("setZoom"));
        //var setMinZoom = parseInt($this.attr("setMinZoom"));
        //var setMaxZoom = parseInt($this.attr("setMaxZoom"));
        var canMove = $this.attr("canMove") == "True";
        var readOnly = $this.attr("geomReadOnly") == "True";
        var geomType = $this.attr("geomType");
        var scale = parseFloat($this.attr("scale"));
        var viewExtentSrid = $this.attr("viewExtentSrid");
        var viewExtentWkt = $this.attr("viewExtentWkt");
        viewExtentWkt = viewExtentWkt == "kazakhstan" ? GetKzGeom() : viewExtentWkt;
        var viewExtentPercent = parseInt($this.attr("viewExtentPercent"));

        var styles = createStyles(strokeWidth, strokeColor, fillColor, radius);
        var layers = [];
        if (background != "none") {
            var backgroundLayer = window.layersLib[background]();
            layers.push(backgroundLayer);
        }
        var vectorSource = new ol.source.Vector({
            features: hasValue ? [createWktFeature(wkt, srid)] : [],
        });
        var vectorLayer = new ol.layer.Vector({
            source: vectorSource,
            style: styles
        });
        layers.push(vectorLayer);


        var map = new ol.Map({
            controls: [],
            target: geomRendererId,
            layers: layers
        });

        //if (hasValue) {
        //    var extentGeom = ol.geom.Polygon.fromExtent(vectorLayer.getSource().getExtent());
        //    extentGeom.scale(scale);
        //    map.getView().fit(extentGeom, map.getSize());

        //} else {
        //    map.getView().fit(new ol.geom.Point([7951137.118857781, 6651040.097882961]), map.getSize());
        //}

        var viewExtent = createWktFeature(viewExtentWkt, viewExtentSrid).getGeometry().getExtent();
        var layerExtent = vectorLayer.getSource().getExtent();
        if (hasValue) {
            if (wkt == viewExtentWkt) {
                var viewExtentGeom = ol.geom.Polygon.fromExtent(viewExtent);
                viewExtentGeom.scale(scale);
                viewExtent = viewExtentGeom.getExtent();
            } else {
                var layerExtentGeom = ol.geom.Polygon.fromExtent(layerExtent);
                layerExtentGeom.scale(scale);
                layerExtent = layerExtentGeom.getExtent();
            }
            viewExtent = [
                layerExtent[0] + (((viewExtent[0] - layerExtent[0]) / 100) * viewExtentPercent),
                layerExtent[1] + (((viewExtent[1] - layerExtent[1]) / 100) * viewExtentPercent),
                layerExtent[2] + (((viewExtent[2] - layerExtent[2]) / 100) * viewExtentPercent),
                layerExtent[3] + (((viewExtent[3] - layerExtent[3]) / 100) * viewExtentPercent)
            ]
        }
        map.getView().fit(ol.geom.Polygon.fromExtent(viewExtent), map.getSize());

        if (!canMove) {
            map.getInteractions().getArray().forEach(function (interaction) {
                interaction.setActive(false);
            });
        }

        if (rounded) {
            $this.children().addClass('rounded');
        }

        if (!readOnly) {
            $(`#${geomRendererId}`).append(`
                <div class="ol-toggle-options ol-unselectable ${geomRendererId}-geom-type" style="position: absolute; top: 10px; right: 10px;">
                    <select class="form-control ${geomRendererId}-geom-type-input" id="example-select">
                        <option value="Polygon" selected>Полигон</option>
                        <option value="Point">Точка</option>
                    </select>
                </div>
            `);
            if (geomType != "Geometry") {
                $(`.${geomRendererId}-geom-type`).hide();
            }

            //жёлтый +
            var polygonInteraction = new ol.interaction.Draw({
                type: 'Polygon',
                source: vectorLayer.getSource(),
                style: createStyles(strokeWidth, '#ffbc00', '#ffbc00' + '50', radius)
            });
            polygonInteraction.setActive(false);
            polygonInteraction.on('drawend', function (e) {
                setTimeout(function () {

                    var selected_collection = selectSingleInteraction.getFeatures();
                    selected_collection.clear();
                    selected_collection.push(e.feature);

                    polygonInteraction.setActive(false);
                    pointInteraction.setActive(false);
                    modifyInteraction.setActive(false);
                    translateInteraction.setActive(true);
                    selectSingleInteraction.setActive(true);
                    selectDoubleInteraction.setActive(true);
                }, 100);
            });
            map.addInteraction(polygonInteraction);

            //жёлтый +
            var pointInteraction = new ol.interaction.Draw({
                type: 'Point',
                source: vectorLayer.getSource(),
                style: createStyles(strokeWidth, '#ffbc00', '#ffbc00' + '50', radius)
            });
            pointInteraction.setActive(false);
            pointInteraction.on('drawend', function (e) {
                setTimeout(function () {

                    var selected_collection = selectSingleInteraction.getFeatures();
                    selected_collection.clear();
                    selected_collection.push(e.feature);

                    polygonInteraction.setActive(false);
                    pointInteraction.setActive(false);
                    modifyInteraction.setActive(false);
                    translateInteraction.setActive(true);
                    selectSingleInteraction.setActive(true);
                    selectDoubleInteraction.setActive(true);
                }, 100);
            });
            map.addInteraction(pointInteraction);

            //зелёный +
            var selectSingleInteraction = new ol.interaction.Select({
                condition: ol.events.condition.singleClick,
                //features: vectorLayer.getSource().getFeatures(),
                wrapX: false,
                style: createStyles(strokeWidth, '#0acf97', '#0acf97' + '50', radius)
            });
            selectSingleInteraction.setActive(true);
            map.addInteraction(selectSingleInteraction);

            //розовый +
            var selectDoubleInteraction = new ol.interaction.Select({
                condition: ol.events.condition.doubleClick,
                //features: vectorLayer.getSource().getFeatures(),
                wrapX: false,
                style: createStyles(strokeWidth, '#ff679b', '#ff679b' + '50', radius)
            });
            selectDoubleInteraction.setActive(true);
            map.addInteraction(selectDoubleInteraction);

            var selected_collection = selectSingleInteraction.getFeatures();
            selected_collection.clear();
            selected_collection.push(new ol.Feature());
            var selected_double_collection = selectDoubleInteraction.getFeatures();
            selected_double_collection.clear();
            selected_double_collection.push(new ol.Feature());

            selectSingleInteraction.on('select', function (e) {
                //alert("change single");
                if (e.target.getFeatures().getLength() > 0) {
                    modifyInteraction.setActive(false);
                    translateInteraction.setActive(true);
                    selectSingleInteraction.setActive(true);
                    selectDoubleInteraction.setActive(true);
                    polygonInteraction.setActive(false);
                    pointInteraction.setActive(false);
                    var selected_double_collection = selectDoubleInteraction.getFeatures();
                    selected_double_collection.clear();
                    selected_double_collection.push(new ol.Feature());
                } else {
                    modifyInteraction.setActive(false);
                    translateInteraction.setActive(false);
                    selectSingleInteraction.setActive(true);
                    selectDoubleInteraction.setActive(true);
                    polygonInteraction.setActive(false);
                    pointInteraction.setActive(false);
                    var selected_collection = selectSingleInteraction.getFeatures();
                    selected_collection.clear();
                    selected_collection.push(new ol.Feature());
                    var selected_double_collection = selectDoubleInteraction.getFeatures();
                    selected_double_collection.clear();
                    selected_double_collection.push(new ol.Feature());
                }
            });
            selectDoubleInteraction.on('select', function (e) {
                //alert("change double");
                if (e.target.getFeatures().getLength() == 1) {
                    modifyInteraction.setActive(true);
                    translateInteraction.setActive(false);
                    selectSingleInteraction.setActive(true);
                    selectDoubleInteraction.setActive(true);
                    polygonInteraction.setActive(false);
                    pointInteraction.setActive(false);
                    var selected_collection = selectSingleInteraction.getFeatures();
                    selected_collection.clear();
                    selected_collection.push(new ol.Feature());
                } else {
                    if (
                        geomType == "MultiPolygon"
                        ||
                        (geomType == "Geometry" && $(`.${geomRendererId}-geom-type-input`).val() == "Polygon")
                        ||
                        (geomType == "Polygon" && vectorLayer.getSource().getFeatures().length == 0)
                    ) {
                        modifyInteraction.setActive(false);
                        translateInteraction.setActive(false);
                        selectSingleInteraction.setActive(false);
                        selectDoubleInteraction.setActive(false);
                        polygonInteraction.setActive(true);
                        var selected_collection = selectSingleInteraction.getFeatures();
                        selected_collection.clear();
                        selected_collection.push(new ol.Feature());
                        var selected_double_collection = selectDoubleInteraction.getFeatures();
                        selected_double_collection.clear();
                        selected_double_collection.push(new ol.Feature());
                    } else if (
                        (geomType == "Geometry" && $(`.${geomRendererId}-geom-type-input`).val() == "Point")
                        ||
                        (geomType == "Point" && vectorLayer.getSource().getFeatures().length == 0)
                    ) {
                        modifyInteraction.setActive(false);
                        translateInteraction.setActive(false);
                        selectSingleInteraction.setActive(false);
                        selectDoubleInteraction.setActive(false);
                        pointInteraction.setActive(true);
                        var selected_collection = selectSingleInteraction.getFeatures();
                        selected_collection.clear();
                        selected_collection.push(new ol.Feature());
                        var selected_double_collection = selectDoubleInteraction.getFeatures();
                        selected_double_collection.clear();
                        selected_double_collection.push(new ol.Feature());
                    } else {
                        modifyInteraction.setActive(false);
                        translateInteraction.setActive(false);
                        selectSingleInteraction.setActive(true);
                        selectDoubleInteraction.setActive(true);
                        polygonInteraction.setActive(false);
                        pointInteraction.setActive(false);
                        var selected_collection = selectSingleInteraction.getFeatures();
                        selected_collection.clear();
                        selected_collection.push(new ol.Feature());
                        var selected_double_collection = selectDoubleInteraction.getFeatures();
                        selected_double_collection.clear();
                        selected_double_collection.push(new ol.Feature());
                    }
                }
            });

            //синий
            var modifyInteraction = new ol.interaction.Modify({
                features: selectDoubleInteraction.getFeatures(),
                style: createStyles(strokeWidth, '#2c8ef8', '#2c8ef8' + '50', radius)
            });
            modifyInteraction.setActive(false);
            map.addInteraction(modifyInteraction);

            //фиолетовый
            var translateInteraction = new ol.interaction.Translate({
                features: selectSingleInteraction.getFeatures(),
                style: createStyles(strokeWidth, '#727cf5', '#727cf5' + '50', radius)
            });
            translateInteraction.setActive(false);
            map.addInteraction(translateInteraction);



            var snapInteraction = new ol.interaction.Snap({
                source: vectorLayer.getSource(),
                style: styles
            });
            snapInteraction.setActive(true);
            map.addInteraction(snapInteraction);

            map.getViewport().addEventListener('contextmenu', function (e) {
                e.preventDefault();
                modifyInteraction.setActive(false);
                translateInteraction.setActive(false);
                selectSingleInteraction.setActive(true);
                selectDoubleInteraction.setActive(true);
                polygonInteraction.setActive(false);
                pointInteraction.setActive(false);
                var selected_collection = selectSingleInteraction.getFeatures();
                selected_collection.clear();
                selected_collection.push(new ol.Feature());
                var selected_double_collection = selectDoubleInteraction.getFeatures();
                selected_double_collection.clear();
                selected_double_collection.push(new ol.Feature());
            });
            map.getViewport().addEventListener('mousedown', function (e) {
                if (e.which == 2) {
                    e.preventDefault();

                    var selected = selectSingleInteraction.getFeatures();
                    selected.forEach(function (feature) {
                        if (feature.getGeometry()) {
                            vectorLayer.getSource().removeFeature(feature);
                        }
                    });

                    modifyInteraction.setActive(false);
                    translateInteraction.setActive(false);
                    selectSingleInteraction.setActive(true);
                    selectDoubleInteraction.setActive(true);
                    polygonInteraction.setActive(false);
                    pointInteraction.setActive(false);
                    var selected_collection = selectSingleInteraction.getFeatures();
                    selected_collection.clear();
                    selected_collection.push(new ol.Feature());
                    var selected_double_collection = selectDoubleInteraction.getFeatures();
                    selected_double_collection.clear();
                    selected_double_collection.push(new ol.Feature());
                }
            });

            $this.closest("form").submit(function () {
                var selected_collection = selectSingleInteraction.getFeatures();
                selected_collection.clear();
                var selected_double_collection = selectDoubleInteraction.getFeatures();
                selected_double_collection.clear();

                var geometry =
                    geomType == "Geometry" ? new ol.geom.GeometryCollection(Array.from(vectorLayer.getSource().getFeatures(), feature => feature.getGeometry())) :
                        geomType == "MultiPolygon" ? new ol.geom.MultiPolygon(Array.from(vectorLayer.getSource().getFeatures(), feature => feature.getGeometry())) :
                            vectorLayer.getSource().getFeatures().length > 0 ? vectorLayer.getSource().getFeatures()[0].getGeometry() : new ol.geom.GeometryCollection([]);

                var format = new ol.format.WKT({
                });
                var featureGeometry = geometry.transform('EPSG:3857', 'EPSG:' + srid)
                var newWKT = format.writeGeometry(featureGeometry);
                var featureGeometry = geometry.transform('EPSG:' + srid, 'EPSG:3857')

                $(`#${geomRendererInputId}`).val(newWKT);
            });
        }

    });
}

function createWktFeature(wkt, srid) {
    var format = new ol.format.WKT();

    var feature = format.readFeature(wkt, {
        dataProjection: 'EPSG:' + srid,
        featureProjection: 'EPSG:3857',
    });

    return feature;

}

function createStyles(strokeWidth, strokeColor, fillColor, radius) {
    var fill = new ol.style.Fill({
        color: fillColor
    });
    var stroke = new ol.style.Stroke({
        color: strokeColor,
        width: strokeWidth
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
    styles[ol.geom.MultiPolygon] = styles[ol.geom.Polygon];
    styles[ol.geom.LineString] = [
        new ol.style.Style({
            stroke: stroke
        }),
        new ol.style.Style({
            stroke: stroke
        })
    ];
    styles[ol.geom.MultiLineString] = styles[ol.geom.LineString];
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
    styles[ol.geom.MultiPoint] = styles[ol.geom.Point];
    styles[ol.geom.GeometryCollection] = styles[ol.geom.Polygon].concat(
        styles[ol.geom.LineString],
        styles[ol.geom.Point]
    );
    return styles;
}

function GetKzGeom() {
    return "POLYGON ((77.087482 42.980256,76.343846 42.860756,75.926266 42.955718,75.711571 42.7971894,74.7499278 42.9897261,74.2042832 43.2667971,73.5470466 43.0169127,73.4871648 42.4175902,71.8636388 42.8328097,71.2670287 42.7777244,70.637992 42.016815,70.3735146 42.0693249,69.4571559 41.4646997,69.023696 41.357124,68.736804 40.9738109,68.513458 41.009906,68.6631211 40.5995527,68.4927366 40.572429,67.9765522 40.832093,68.1277035 41.0384968,67.9655608 41.1500772,66.7093293 41.1426147,66.5323928 41.8774368,65.99871 41.937179,66.0966931 42.9381375,65.8441558 42.8582546,65.0021563 43.7153968,64.5348607 43.5706872,63.3490764 43.6504846,62.004055 43.505401,61.1065656 44.3604334,58.587577 45.590118,55.9987426 45.0003137,56.0017921 41.3242798,55.430472 41.279495,54.928925 41.928325,54.2178099 42.3790865,52.984788 42.121175,52.259144 41.724447,52.3928472 42.6017583,51.1326417 43.05145,51.0968111 43.4507833,50.5487667 44.1438056,50.1032333 44.2835944,49.8104972 44.9956222,50.0740944 45.1936083,50.8716306 45.1156222,51.4064694 45.5232056,52.4719222 45.607825,52.8757694 46.0208778,52.7984917 46.6053889,52.6520722 46.7066833,51.6042667 46.6026194,51.2384528 46.8500139,50.1468194 46.4773194,49.886117 46.045769,48.5479411 46.5572664,48.5352722 46.7062855,48.9959671 46.7430837,48.2093704 47.6916271,47.1944439 47.76084,47.0499372 47.9977513,47.2111172 48.0822157,47.1229738 48.2685031,46.4932179 48.432558,46.7791649 48.9514137,47.028725 49.089209,46.7822421 49.3398769,46.837222 49.6065712,46.9025552 49.8637806,47.3535466 50.0943422,47.3065711 50.3090775,47.6125408 50.4639626,48.2261922 49.868056,48.7456239 49.9213212,48.9034293 50.0183874,48.5706275 50.632365,49.4229079 50.8486852,49.4546664 51.1255932,50.3731352 51.332611,50.5851824 51.6458348,50.8120335 51.5850431,50.7681628 51.7730366,51.3801834 51.6412244,51.2404462 51.5631264,51.3801758 51.4761548,51.738729 51.4651675,51.856608 51.6738058,52.3586696 51.7758986,52.5606107 51.4530308,53.4264155 51.482231,53.6683819 51.2254202,54.5181701 50.854636,54.4136108 50.6206947,54.5448205 50.5277123,54.7313548 50.6193289,54.6976065 50.8926752,54.5264226 50.9306227,54.6923411 51.0400799,55.7080302 50.5496304,56.5137287 51.0858606,56.7126274 50.9706772,56.704428 51.077521,57.1847713 51.1149613,57.5070278 50.8759514,57.7618164 50.9202119,57.7719418 51.1384157,58.3210316 51.1622791,58.6141735 51.0474024,58.6564001 50.8086365,59.4792952 50.6414793,59.509517 50.4953129,59.8131772 50.535508,60.0149026 50.8221238,60.8090003 50.6566054,61.4490006 50.8061229,61.6897852 51.2559632,61.505234 51.406996,60.41554 51.6470009,60.5174884 51.7977253,59.9941197 51.9646457,61.0667983 52.3470908,60.714553 52.6630268,61.0086368 52.9781089,62.1304402 52.99006,61.6739255 53.2656589,61.1770356 53.3117561,61.2586516 53.5080373,61.5932608 53.5261239,60.9002873 53.6286665,61.2271763 53.795613,61.0293566 53.9551112,61.2798674 53.9285743,61.400587 54.0868636,61.9323322 53.9499079,62.4187937 54.046982,62.5100236 53.9128576,62.5947674 54.0738762,63.1705991 54.1936302,64.974791 54.424178,65.221956 54.334286,65.2144536 54.541393,65.489554 54.655537,68.2347429 54.9689587,68.3151931 55.0766291,68.1771478 55.1987932,68.6197954 55.198213,68.7486541 55.3788423,69.001168 55.2884639,68.9326858 55.4420271,70.2043533 55.1463726,70.8074641 55.2926304,71.0188383 55.085095,71.0304616 54.7784824,71.2932122 54.6677509,71.1986884 54.3117026,70.9985715 54.3360786,71.1789715 54.0949458,71.7204594 54.1064617,71.7512172 54.2541499,72.1843064 54.120593,72.0326976 54.3664699,72.5070029 54.1399711,72.3695826 54.0833483,72.4866879 53.9007542,72.7144825 53.9508897,72.5327246 54.0435594,72.6083603 54.1370506,73.275814 53.942662,73.7676751 54.0540553,73.6909568 53.8562287,73.4522778 53.8761856,73.2528958 53.6742283,73.4382659 53.4353948,73.9007753 53.6511372,74.3829131 53.4559241,74.4571137 53.6911004,75.0462878 53.7926662,75.4410767 53.9723507,75.3751078 54.0699691,76.9289614 54.4577679,76.7564658 54.161826,76.44044 54.1673306,76.5040997 54.0317861,77.9065061 53.29141,80.0758658 50.7382076,80.0679769 50.8528895,80.4822429 50.9686871,80.4470866 51.2082857,80.6776732 51.3167498,81.1463635 51.2118797,81.0597635 50.9479034,81.4125874 50.9757418,81.4493382 50.755926,82.5674696 50.7468424,82.7328173 50.9272716,83.4324488 51.0093789,83.9726623 50.7953988,84.3625226 50.2054649,85.0355435 50.0524943,84.9854889 49.9067722,85.2575191 49.5885235,86.2020375 49.4661128,86.6883746 49.8108988,86.7789236 49.6960165,86.6182488 49.5703775,86.8271741 49.5454254,87.0307157 49.2528804,87.3156316 49.2310833,86.749645 49.015031,86.819222 48.833126,86.586633 48.540539,85.7124996 48.3607546,85.5265731 48.0258791,85.7029578 47.3800725,85.5339395 47.0488716,84.944232 46.8628509,84.7362061 47.0140686,83.9069039 46.9747689,83.031327 47.2232667,82.2579086 45.6165769,82.5428316 45.4209581,82.5601012 45.2034839,81.9220346 45.228887,81.7717552 45.3860334,79.8819854 44.9106776,80.4104826 44.610393,80.3942647 44.1120308,80.8072807 43.1772719,80.3841734 43.0459032,80.6006155 42.8925359,80.2599508 42.827599,80.1619837 42.6304903,80.2734724 42.2278629,79.964108 42.436598,79.504877 42.459887,79.1993719 42.8042231,77.087482 42.980256))";
}