$(document).ready(function () {

    $("#btnNext_formWizard").hide();

    var targetId = 'object-draw-by-coords';
    var $target = $(`#${targetId}`);

    $target.html(`
        <div id="coords-result-label" class="alert alert-success" role="alert" style="display: none;"></div>
        <div class="object-area ol-unselectable ol-control" id="object-area" style="display: none;"></div>
        <div id="coords-widget" class="rounded">
          <div id="coords-panel">
            <table>
              <thead>
                <tr>
                  <td class="h6 text-center text-white">Долгота</td>
                  <td class="h6 text-center text-white">Широта</td>
                </tr>
              </thead>
              <tbody id="coords-table-body">
              </tbody>
            </table>
          </div>
          <input type="button" id="confirm-wkt-button" class="btn btn-sm btn-primary" value="Принять"/>
          <input type="button" id="back-to-wkt-button" class="btn btn-sm btn-secondary" value="Назад" style="display: none;"/>
          <input type="button" id="confirm-coords-button" class="btn btn-sm btn-primary" value="Принять" style="display: none;"/>
          <input type="button" id="back-to-coords-button" class="btn btn-sm btn-secondary" value="Назад" style="display: none;"/>
        </div>
    `);


    var $wkt = $(`#${$(`#${targetId}`).attr("target-wkt")}`);
    var $coords = $(`#${$(`#${targetId}`).attr("target-coords")}`);
    var $background = JSON.parse($(`#${targetId}`).attr("background-wkts"));
    var $backgroundParent = JSON.parse($(`#${targetId}`).attr("background-parent-wkt"));
    var $backgroundOldVersion = JSON.parse($(`#${targetId}`).attr("background-old-version-wkt"));



    var drawColor = '#0acf97';
    var drawStrokeColor = `${drawColor}`;
    var drawFillColor = `${drawColor}25`;
    var drawWidth = 5;

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


    var backgroundLayer = new ol.layer.Vector({
        source: new ol.source.Vector(),
        id: "background"
    });
    if ($background != null) {
        if ($background.length > 0) {
            backgroundLayer = createVectorLayerByFeatures("background", createFeatureArray($background, "#727cf5", "#727cf5"));
        }
    }
    if ($backgroundParent != null) {
        if ($backgroundParent.length > 0) {
            var backgroundLayerSource = backgroundLayer.getSource();
            backgroundLayerSource.addFeature(createFeature($backgroundParent, "#ffbc00", "#ffbc00"));
            backgroundLayer.setSource(backgroundLayerSource);
        }
    }
    if ($backgroundOldVersion != null) {
        if ($backgroundOldVersion.length > 0) {
            var backgroundLayerSource = backgroundLayer.getSource();
            backgroundLayerSource.addFeature(createFeature($backgroundOldVersion, "#98a6ad", "#98a6ad"));
            backgroundLayer.setSource(backgroundLayerSource);
        }
    }


    var $resultLabel = $(`#coords-result-label`);

    var $confirmWkt = $(`#confirm-wkt-button`);
    var $backToWkt = $(`#back-to-wkt-button`);
    var $confirmCoords = $(`#confirm-coords-button`);
    var $backToCoords = $(`#back-to-coords-button`);

    var googleLayer = new ol.layer.Tile({
        preload: Infinity,
        source: new ol.source.XYZ({ url: 'https://mt{0-3}.google.com/vt/lyrs=s,h&x={x}&y={y}&z={z}' })
    });

    var vectorLayer = new ol.layer.Vector({
        source: new ol.source.Vector(),
        style: styles
    });

    var pointLayer = new ol.layer.Vector({
        source: new ol.source.Vector()
    });

    var polygonInteraction = new ol.interaction.Draw({
        type: 'Polygon',
        source: vectorLayer.getSource(),
        style: styles
    });
    polygonInteraction.setActive(true);
    polygonInteraction.on('drawend', function (e) {
        setTimeout(function () {
            polygonInteraction.setActive(false);
            selectInteraction.setActive(true);
            modifyInteraction.setActive(true);
            translateInteraction.setActive(true);
            snapInteraction.setActive(true);

            var feature = e.feature;
            var selected_collection = selectInteraction.getFeatures();
            selected_collection.clear();
            selected_collection.push(e.feature);
        }, 100);
    });

    var selectInteraction = new ol.interaction.Select({
        condition: ol.events.condition.click,
        wrapX: false,
        style: styles
    });
    selectInteraction.getFeatures().clear();
    selectInteraction.setActive(false);

    var modifyInteraction = new ol.interaction.Modify({
        features: selectInteraction.getFeatures(),
        style: styles
    });
    modifyInteraction.setActive(false);

    var translateInteraction = new ol.interaction.Translate({
        features: selectInteraction.getFeatures(),
        style: styles
    });
    translateInteraction.setActive(false);

    var snapInteraction = new ol.interaction.Snap({
        source: vectorLayer.getSource(),
        style: styles
    });
    snapInteraction.setActive(false);

    var map = new ol.Map({
        controls: [],
        interactions: ol.interaction.defaults().extend([
            polygonInteraction,
            selectInteraction,
            modifyInteraction,
            translateInteraction,
            snapInteraction
        ]),
        target: targetId,
        layers: [googleLayer, backgroundLayer, vectorLayer, pointLayer],
        view: new ol.View({
            center: [7951137.118857781, 6651040.097882961],
            zoom: 5.5
        })
    });

    $(`#${targetId} canvas`).addClass("rounded");

    if ($backgroundParent != null) {
        if ($backgroundParent.length > 0) {
            var zoomToFeature = createFeature($backgroundParent, null, null);
            map.getView().fit(zoomToFeature.getGeometry().getExtent(), map.getSize());
        }
    }
    if ($backgroundOldVersion != null) {
        if ($backgroundOldVersion.length > 0) {
            var zoomToFeature = createFeature($backgroundOldVersion, null, null);
            map.getView().fit(zoomToFeature.getGeometry().getExtent(), map.getSize());
        }
    }

    $confirmWkt.click(function () {
        try {
            var feature = GetFeature();

            var wkt = GetFeatureWkt(feature);
            $wkt.val(wkt);

            var textCoordsArray = GetFeatureCoords(feature);
            $coords.val(JSON.stringify(textCoordsArray));

            FillCoordsTable(textCoordsArray);

            $('.coord-row').hover(function () {

                var x = +$(this).attr("x");
                var y = +$(this).attr("y");

                var point = new ol.Feature({
                    geometry: new ol.geom.Point(ol.proj.transform([x, y], 'EPSG:4326', 'EPSG:3857')),
                    id: 'CURRENT COORD'
                });

                //var pointBorder = new ol.style.Style({
                //    text: new ol.style.Text({
                //        text: '⦿',
                //        scale: 1.1,
                //        font: `normal 50px FontAwesome`,
                //        offsety: 0,
                //        offsetx: 0,
                //        fill: new ol.style.Fill({ color: `${coordColor}75` })//'#0099ff' })
                //    })
                //});

                point.setStyle(styles);

                var pointSource = new ol.source.Vector({
                    features: [point]
                });

                pointLayer.setSource(pointSource);


            });

            $resultLabel.html(`Геометрия обработана успешно.`);
            $resultLabel.removeClass(`alert-warning`);
            $resultLabel.addClass(`alert-success`);
            $resultLabel.show();

            var polygon = new ol.format.WKT().readGeometry(
                wkt,
                {
                    dataProjection: 'EPSG:4326',
                    featureProjection: 'EPSG:3857'
                }
            );
            var area = polygon.getArea() / 20000;
            var areaText = `~ ${(area).toFixed(4)} га`;
            $(`#object-area`).html(areaText);
            $(`#object-area`).show();

            $confirmWkt.hide();
            $backToWkt.show();
            $confirmCoords.show();
            $backToCoords.hide();

            map.getView().fit(vectorLayer.getSource().getExtent(), map.getSize());

            //map.getInteractions().clear();

            polygonInteraction.setActive(false);
	        selectInteraction.setActive(false);
	        selectInteraction.getFeatures().clear();
	        modifyInteraction.setActive(false);
	        translateInteraction.setActive(false);
	        snapInteraction.setActive(false);

        }
        catch(e) {

            $wkt.val("ERROR");
            $coords.val("ERROR");

            $resultLabel.html(`Ошибка обработки геометрии`);
            $resultLabel.removeClass(`alert-success`);
            $resultLabel.addClass(`alert-warning`);
            $resultLabel.show();
            $('#coords-table-body').html(``);

            throw new Error(e);

        }

    });

    $backToWkt.click(function () {

        var feature = GetFeature();

        pointLayer.setSource(new ol.source.Vector());

        selectInteraction.setActive(true);
        var selected_collection = selectInteraction.getFeatures();
        selected_collection.clear();
        selected_collection.push(feature);

        snapInteraction.setActive(true);
        translateInteraction.setActive(true);
        modifyInteraction.setActive(true);

        $(`#object-area`).html("");
        $(`#object-area`).hide();

        $confirmWkt.show();
        $backToWkt.hide();
        $confirmCoords.hide();
        $backToCoords.hide();

        $('#coords-table-body').html(``);
        $("#btnNext_formWizard").hide();

    });

    $confirmCoords.click(function () {
        ConfirmCoords();

        $('#coords-table-body').hide();

        $confirmWkt.hide();
        $backToWkt.hide();
        $confirmCoords.hide();
        $backToCoords.show();

        $("#btnNext_formWizard").show();
    });

    $backToCoords.click(function () {
        ConfirmCoords();

        $('#coords-table-body').show();

        $confirmWkt.hide();
        $backToWkt.show();
        $confirmCoords.show();
        $backToCoords.hide();

        $("#btnNext_formWizard").hide();
    });

    function GetFeature() {
        var features = vectorLayer.getSource().getFeatures().slice();
        return features[0];
    }

    function GetFeatureWkt(feature) {
        var format = new ol.format.WKT({
        });
        var featureGeometry = feature.getGeometry().transform('EPSG:3857', 'EPSG:4326');
        var wkt = format.writeGeometry(featureGeometry);
        featureGeometry.transform('EPSG:4326', 'EPSG:3857')
        return wkt;
    }

    function GetFeatureCoords(feature) {
        var featureGeometry = feature.getGeometry().transform('EPSG:3857', 'EPSG:4326');
        var coordsArray = featureGeometry.getCoordinates()[0].slice();
        var textCoordsArray = [];
        coordsArray.forEach(function (coord) {
            var x = coord[0];
            var y = coord[1];
            textCoordsArray.push({
                x: x,
                y: y,
                appropriateX: null,
                appropriateY: null
            });
        });
        featureGeometry.transform('EPSG:4326', 'EPSG:3857')
        return textCoordsArray.slice(0, textCoordsArray.length - 1);
    }

    function FillCoordsTable(textCoordsArray) {
        textCoordsArray.forEach(function (coord) {
            $('#coords-table-body').append(`
	      	<tr class="coord-row" x="${coord.x}" y="${coord.y}">
		        <td><input type="text" class="form-control text-white rounded coord-input" value="${coord.x.toFixed(7)}"/></td>
		        <td><input type="text" class="form-control text-white rounded coord-input" value="${coord.y.toFixed(7)}"/></td>
	    	</tr>
		`);
        });
    }

    function GetCoordTableBody(targetId) {

        var textCoordsArray = [];

        $(`#${targetId} tr`).each(function (i, tr) {

            var x = +$(tr).attr("x");
            var y = +$(tr).attr("y");

            var row = [];
            $(tr).children().each(function (j, td) {
                var $input = $(td).children().first();
                if (!$input.attr("not-for-table")) {
                    row.push($input.val());
                    if ($input.val() == "" || $input.val() == null) {
                        throw new Error('Нужно заполнить все ячейки.');
                    }
                }
            });

            var appropriateX = row[0];
            var appropriateY = row[1];

            textCoordsArray.push({
                x: x,
                y: y,
                appropriateX: appropriateX,
                appropriateY: appropriateY
            });
        });

        return textCoordsArray;

    }

    $(document).submit(function (event) {
        ConfirmCoords();
    });

    function ConfirmCoords() {
        try {

            var feature = GetFeature();

            var wkt = GetFeatureWkt(feature);
            $wkt.val(wkt);

            var textCoordsArray = GetCoordTableBody("coords-table-body");
            if (textCoordsArray.length >= 3) {
                $coords.val(JSON.stringify(textCoordsArray));

                $resultLabel.html(`Координаты обработаны успешно.`);
                $resultLabel.removeClass(`alert-warning`);
                $resultLabel.addClass(`alert-success`);
                $resultLabel.show();
            } else {
                throw new Error("Проверьте, введены ли все координаты");
            }
        }
        catch (e) {

            $wkt.val("ERROR");
            $coords.val("ERROR");

            $resultLabel.html(`Ошибка обработки координат`);
            $resultLabel.removeClass(`alert-success`);
            $resultLabel.addClass(`alert-warning`);
            $resultLabel.show();

            throw new Error(e);

        }
    }

});


function createVectorLayerByFeatures(id, features) {
    var vectorSource = new ol.source.Vector({
        features: features
    });
    var vectorLayer = new ol.layer.Vector({
        source: vectorSource
    });

    vectorLayer.id = id;

    return vectorLayer;
}

function createFeature(featureWKT, strokeColor, fillColor) {

    var drawColor = fillColor ?? strokeColor;
    var drawStrokeColor = `${strokeColor}`;
    var drawFillColor = `${drawColor}10`;
    var drawWidth = 5;

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

    var feature = new ol.Feature({
        geometry: new ol.format.WKT().readGeometry(featureWKT,
            {
                dataProjection: 'EPSG:4326',
                featureProjection: 'EPSG:3857'
            }
        )
    });

    feature.setStyle(styles);

    return feature;
}

function createFeatureArray(featureWKTs, strokeColor, fillColor) {
    var features = []

    featureWKTs.forEach(function (featureWKT) {
        features.push(createFeature(featureWKT, strokeColor, fillColor));
    });

    return features;
}
