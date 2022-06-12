window.gisApp.controls.FieldColorType = function (opt_options) {
    var options = opt_options || {};
    var areaByCultureArr = options.areaByCultureArr || undefined;
    var vectorLayer = options.vectorLayer || undefined;
    var getNdviStyle = options.getNdviStyle || undefined;

    if (areaByCultureArr == undefined) {
        throw "err. ctrl=FieldColorType. areaByCultureArr can't be undefined";
    }
    if (vectorLayer == undefined) {
        throw "err. ctrl=FieldColorType. vectorLayer can't be undefined";
    }
    if (getNdviStyle == undefined) {
        throw "err. ctrl=FieldColorType. getNdviStyle can't be undefined";
    }
    
    var $button = $("<button type='button' class='gis-btn-icon btn-color-type-layer' title='Выбор подсветки участка'>");
    $button.text('LY');

    var $element = $("<div class='color-type-layers ol-unselectable ol-control'>");
    $element.append($button);
    
    var legend = [
        { text: "NDVI", legendType: 1 },
        { text: "В разрезе культур", legendType: 2 }
    ];
    var that = this;
    var $layersPanel = $("<div class='gis-color-type-panel'>");
    for (var i = 0; i < legend.length; i++) {
        var item = legend[i];
        var $legendItem = $("<div class='gis-color-type-item' legend-index='{0}'><span class=''>{1}</span></div>".format(i, item.text));
        $layersPanel.append($legendItem);
    }

    var currentFieldColorType = 1;
    $layersPanel.delegate(".gis-color-type-item", "click", function () {
        var $this = $(this);
        if ($this.hasClass("selected-item")) {
            return;
        }

        $layersPanel.find(".gis-color-type-item").removeClass("selected-item");
        $this.addClass("selected-item");

        var index = $this.attr("legend-index");
        var legendType = legend[index].legendType;
        if (legendType == 1) {
            if (currentFieldColorType != 1) {
                currentFieldColorType = 1;
                $.each(vectorLayer.getSource().getFeatures(), function (key, feature) {
                    var fieldInfo = feature.getProperties().fieldInfo;
                    var ndviStyle = new ol.style.Style({
                        fill: new ol.style.Fill({
                            color: 'rgba(255,255,255,0.4)'
                        }),
                        stroke: new ol.style.Stroke({
                            color: '#000',  //'#B8873D',
                            width: 1.25     //3
                        })
                    });
                    if (!fieldInfo.Ndvi) {
                        ndviStyle.getFill().setColor([255, 255, 255, 0.4]);
                    } else {
                        var color = window.gisApp.palettes.Ndvi.getValColor(fieldInfo.Ndvi) + '88';
                        ndviStyle.getFill().setColor(color);
                    }
                    feature.setStyle(ndviStyle);
                });
            }
        }
        else if (legendType == 2) {
            if (currentFieldColorType != 2) {
                currentFieldColorType = 2;
                $.each(vectorLayer.getSource().getFeatures(), function (key, feature) {
                    var fieldInfo = feature.getProperties().fieldInfo;
                    var cultureCode = fieldInfo.CultureCode || "NONE";
                    var color = 'rgba(255,255,255,0.4)';
                    $.each(areaByCultureArr, function (k, v) {
                        if (v.cultureCode == cultureCode) {
                            color = v.color;
                        }
                    });
                    var style = new ol.style.Style({
                        fill: new ol.style.Fill({
                            color: color
                        }),
                        stroke: new ol.style.Stroke({
                            color: '#000',
                            width: 1.25
                        })
                    });
                    feature.setStyle(style);
                });
            }
        }

        $layersPanel.hide();
    });

    $($layersPanel.find(".gis-color-type-item")[0]).click();

    $('body').click(function (evt) {
        if ($layersPanel.is(':visible') == false) {
            return;
        }
        if ($(evt.target).parents('.gis-color-type-panel').length == 0) {
            $layersPanel.hide();
        }
    });
    
    $button.click(function () {
        $element.after($layersPanel);
        $layersPanel.show();
        that._inited = true;

        $(".gis-choose-layers-panel").hide();
    });

    this.onControlsDeactivate = function (sender) {
        if (sender == that) {
            return;
        }
        if (that._inited != true) {
            return;
        }
        $layersPanel.hide();
    };

    ol.control.Control.call(this, {
        element: $element[0],
        target: options.target
    });
};
ol.inherits(window.gisApp.controls.FieldColorType, ol.control.Control);