window.gisApp.controls.MeasureLengthControl = function (opt_options) {
    var options = opt_options || {};

    var $btnMeasureLength = $("<button type='button'>");
    $btnMeasureLength.attr("title", "Измерить расстояние");
    $btnMeasureLength.addClass('gis-button btn-gis-measure-length');


    var sketch;
    var measureTooltipElement;
    var measureTooltipOverlay;
    var measureLayer;
    var that = this;

    measureLayer = new ol.layer.Vector({
        source: new ol.source.Vector(),
        style: new ol.style.Style({
            fill: new ol.style.Fill({
                color: 'rgba(255, 255, 255, 0.2)'
            }),
            stroke: new ol.style.Stroke({
                color: '#ff5433',
                width: 4
            }),
            image: new ol.style.Circle({
                radius: 7,
                fill: new ol.style.Fill({
                    color: '#ff5433'
                })
            })
        })
    });


    measureTooltipElement = document.createElement('div');
    measureTooltipElement.className = 'tooltip tooltip-measure';
    measureTooltipElement.id = 'tooltip-measure';

    measureTooltipOverlay = new ol.Overlay({
        element: measureTooltipElement,
        offset: [0, -15],
        positioning: 'bottom-center'
    });

    var $container = $("<div id='gis-measure-length-info-popup' class='gis-measure-info-popup'>");
    var $closer = $("<div class='gis-ol-measure-info-closer'></div>");
    $closer.click(function() {
        if (sketch) {
            measureLayer.getSource().removeFeature(sketch);
            $container.hide();
            sketch = null;
        }
    });
    $container.append($closer);
    var $measureInfo = $("<div class='gis-measure-info'>");
    $container.append($measureInfo);

    var calcCrow = function calcCrow(lat1, lon1, lat2, lon2) {
        var R = 6371; // km
        var dLat = toRad(lat2 - lat1);
        var dLon = toRad(lon2 - lon1);
        var lat1 = toRad(lat1);
        var lat2 = toRad(lat2);

        var a = Math.sin(dLat / 2) * Math.sin(dLat / 2) +
            Math.sin(dLon / 2) * Math.sin(dLon / 2) * Math.cos(lat1) * Math.cos(lat2);
        var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
        var d = R * c;
        return d;
    };

    var toRad = function toRad(Value) {
        return Value * Math.PI / 180;
    };

    var formatLength = function (line) {
        var lineCoords = line.getCoordinates();
        var point1_4326 = proj4('EPSG:3857', 'EPSG:4326', [lineCoords[0][0], lineCoords[0][1]]);
        var point2_4326 = proj4('EPSG:3857', 'EPSG:4326', [lineCoords[1][0], lineCoords[1][1]]);
        var sphereLen = calcCrow(point1_4326[1], point1_4326[0], point2_4326[1], point2_4326[0]) * 1000;
        var length = Math.round(sphereLen * 100) / 100;
        var output;

        if (length > 100) {
            output = (Math.round(length / 1000 * 100) / 100) + ' км';
        } else {
            output = (Math.round(length * 100) / 100) + ' м';
        }
        return output;
    };

    var draw = new ol.interaction.Draw({
        source: measureLayer.getSource(),
        type: ('LineString'),
        style: new ol.style.Style({
            fill: new ol.style.Fill({
                color: 'rgba(255, 255, 255, 0.2)'
            }),
            stroke: new ol.style.Stroke({
                color: 'rgba(0, 0, 0, 0.5)',
                lineDash: [10, 10],
                width: 2
            }),
            image: new ol.style.Circle({
                radius: 5,
                stroke: new ol.style.Stroke({
                    color: 'rgba(0, 0, 0, 0.7)'
                }),
                fill: new ol.style.Fill({
                    color: 'rgba(255, 255, 255, 0.2)'
                })
            })
        })
    });

    var drawing = false;
    draw.on('drawstart', function (evt) {
        drawing = true;
        if (measureTooltipElement.style.visibility == 'hidden') {
            measureTooltipElement.style.visibility = 'visible';
        }
        $container.hide();
        measureTooltipOverlay.setElement(measureTooltipElement);
        measureTooltipOverlay.setPosition(undefined);
        var measureSource = measureLayer.getSource();
        measureSource.forEachFeature(function (feature) { measureSource.removeFeature(feature) });
        sketch = evt.feature;
    }, this);

    draw.on('drawend', function (evt) {
        var tooltipCoord = evt.coordinate;
        drawing = false;

        if (sketch) {
            var output;
            var geom = (sketch.getGeometry());
            output = formatLength(geom);
            tooltipCoord = geom.getLastCoordinate();

            $container.show();
            if (measureTooltipElement.style.visibility == 'visible') {
                measureTooltipElement.style.visibility = 'hidden';
            }
            $measureInfo.empty();
            $measureInfo.append(output);
            measureTooltipOverlay.setElement($container[0]);
            measureTooltipOverlay.setPosition(tooltipCoord);
        }

    }, this);


    var measureLength = {
        init: function (map) {
            $btnMeasureLength.addClass("active");
            this.map = map;
            this.map.addLayer(measureLayer);
            this.map.addOverlay(measureTooltipOverlay);
            measureTooltipOverlay.setPosition(undefined);
            this.map.addInteraction(draw);
        },
        setActive: function (active) {
            if (active) {
                this.map.on('pointermove', this.onPointerMove);
            } else {
                this.map.un('pointermove', this.onPointerMove);
            }
        },
        onPointerMove: function (evt) {
            if (evt.dragging) {
                return;
            }
            if (sketch && drawing) {
                var tooltipCoord = evt.coordinate;
                var output;
                var geom = (sketch.getGeometry());
                output = formatLength(geom);
                tooltipCoord = geom.getLastCoordinate();
                measureTooltipElement.innerHTML = output;
                measureTooltipOverlay.setPosition(tooltipCoord);
            }
        }
    };

    $btnMeasureLength.click(function () {
        var isActive = $(this).hasClass("active");
        if (!isActive) {
            var map = that.getMap();
            map.deactivateControls(that);
            if (that._inited != true) {
                measureLength.init(map);
                measureLength.setActive(true);
                that._inited = true;
            }
        } else {
            that.onControlsDeactivate(null);
        }
    });

    var $element = $('<div>');
    $element.addClass('btn-gis-measure-length ol-selectable ol-control');
    $element.append($btnMeasureLength);

    this.onControlsDeactivate = function (sender) {
        if (sender == that) {
            return;
        }
        if (that._inited != true) {
            return;
        }
        $btnMeasureLength.removeClass("active");
        
        measureLength.setActive(false);
        measureTooltipElement.style.visibility = 'hidden';
        $container.hide();
        that.getMap().removeLayer(measureLayer);
        that.getMap().removeOverlay(measureTooltipOverlay);
        that.getMap().removeInteraction(draw);
        that._inited = false;
        var measureSource = measureLayer.getSource();
        measureSource.forEachFeature(function (feature) { measureSource.removeFeature(feature) });
    };

    ol.control.Control.call(this, {
        element: $element[0],
        target: options.target
    });
};
ol.inherits(window.gisApp.controls.MeasureLengthControl, ol.control.Control);