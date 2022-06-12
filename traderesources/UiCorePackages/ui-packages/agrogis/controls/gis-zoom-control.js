window.gisApp.controls.ZoomControl = function (opt_options) {
    var options = opt_options || {};
    if (!options.zoomInTipLabel) {
        options.zoomInTipLabel = "Приблизить";
    }
    if (!options.zoomOutTipLabel) {
        options.zoomOutTipLabel = "Отдалить";
    }
    if (!options.units) {
        options.units = "degrees";
    }
    ol.control.Zoom.call(this, options);
};
ol.inherits(window.gisApp.controls.ZoomControl, ol.control.Zoom);