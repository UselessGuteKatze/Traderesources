window.gisApp.controls.CustomLegend = function (opt_options) {
    var options = opt_options || {};
    var legendData = options.legendData || [];
    
    var legendStr = "<div class='custom-legend-container'><table><tbody>";
    legendStr += "</tbody></table></div>";
    var $legend = $(legendStr);
    var $tbody = $legend.find("tbody");

    for (var i = legendData.length - 1; i >= 0; i--) {
        var legendItem = legendData[i];
        var tr = "<tr><td class='color-mark'><div style='background-color:{0}; border: 1px solid {1};'></div></td><td class='color-text'>{2}</td></tr>"
            .format(legendItem.fillColor, legendItem.borderColor || "gray", (legendItem.text === null) ? "&nbsp;" : legendData[i].text);
        var $tr = $(tr);
        $tbody.append($tr);
        if (legendItem.legendCss) {
            $tr.find(".color-mark div").css(legendItem.legendCss);
        }
    }

    var $element = $("<div class='custom-legend ol-unselectable ol-control'>");
    $element.append($legend);

    ol.control.Control.call(this, {
        element: $element[0],
        target: options.target
    });
};
ol.inherits(window.gisApp.controls.CustomLegend, ol.control.Control);
