window.gisApp.controls.MeasureAreaControl = function (opt_options) {
    var options = opt_options || {};

    var $btnMeasureArea = $("<button type='button'>");
    $btnMeasureArea.attr("title", "Измерить площадь");
    $btnMeasureArea.addClass('gis-button btn-gis-measure-area');


    var sketch;
    var that = this;

    var measureLayer = new ol.layer.Vector({
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


    var measureTooltipElement = document.createElement('div');
    measureTooltipElement.className = 'tooltip tooltip-measure';
    measureTooltipElement.id = 'tooltip-measure';

    var measureTooltipOverlay = new ol.Overlay({
        element: measureTooltipElement,
        offset: [0, -15],
        positioning: 'bottom-center'
    });

    var $container = $("<div class='gis-measure-info-popup'>");
    var $closer = $("<div class='gis-ol-measure-info-closer'></div>");
    $closer.click(function () {
        if (sketch) {
            measureLayer.getSource().removeFeature(sketch);
            $container.hide();
            sketch = null;
        }
    });
    $container.append($closer);
    var $measureInfo = $("<div class='gis-measure-info'>" +
        "<div class='area-ha'><span class='title'>Площадь</span> <span class='value'></span> <span class='measure'>га</span></div>" +
        "<div class='area-km'><span class='title'>Площадь</span> <span class='value'></span> <span class='measure'></span></div>" +
        "<div class='perimeter-km'><span class='title'>Периметр</span> <span class='value'></span> <span class='measure'>км</span></div>" +
        "</div>");
    $container.append($measureInfo);

    var refreshMeasureInfo = function (polygon) {
        var area = ol.Sphere.getArea(polygon);
        var perimeter = ol.Sphere.getLength(polygon);
        
        $measureInfo.find(".area-ha .value").text((area / 10000).toFixed(2));
        $measureInfo.find(".area-km .value").text((area > 10000) ? (area / 1000000).toFixed(2) : area.toFixed(2));
        $measureInfo.find(".area-km .measure").html((area > 10000) ? "км<sup>2</sup>" : "м<sup>2</sup>");
        $measureInfo.find(".perimeter-km .value").text((perimeter / 1000).toFixed(2));
    };

    var draw = new ol.interaction.Draw({
        source: measureLayer.getSource(),
        type: ('Polygon'),
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
            refreshMeasureInfo(geom);
            tooltipCoord = geom.getLastCoordinate();
            $container.show();
            if (measureTooltipElement.style.visibility == 'visible') {
                measureTooltipElement.style.visibility = 'hidden';
            }
            measureTooltipOverlay.setElement($container[0]);
            measureTooltipOverlay.setPosition(tooltipCoord);
        }

    }, this);


    var measureArea = {
        init: function (map) {
            $btnMeasureArea.addClass("active");
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
                refreshMeasureInfo(geom);
                tooltipCoord = geom.getLastCoordinate();
                measureTooltipElement.innerHTML = $measureInfo[0].innerHTML;
                measureTooltipOverlay.setPosition(tooltipCoord);
            }
        }
    };

    $btnMeasureArea.click(function () {
        var isActive = $(this).hasClass("active");
        if (!isActive) {
            var map = that.getMap();
            map.deactivateControls(that);
            if (that._inited != true) {
                measureArea.init(map);
                measureArea.setActive(true);
                that._inited = true;
            }
        } else {
            that.onControlsDeactivate(null);
        }
    });

    var $element = $('<div>');
    $element.addClass('btn-gis-measure-area ol-selectable ol-control');
    $element.append($btnMeasureArea);

    this.onControlsDeactivate = function (sender) {
        if (sender == that) {
            return;
        }
        if (that._inited != true) {
            return;
        }
        $btnMeasureArea.removeClass("active");

        measureArea.setActive(false);

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
ol.inherits(window.gisApp.controls.MeasureAreaControl, ol.control.Control);