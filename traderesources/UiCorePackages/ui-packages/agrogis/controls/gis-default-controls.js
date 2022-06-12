window.gisApp.controls.defaults = function (opt_options) {
    opt_options = opt_options || {};
    var fullScreen = opt_options.fullScreen === undefined ? true : opt_options.fullScreen;

    var ret = new ol.Collection();
    ret.extend([
        new gisApp.controls.ZoomControl(),
        new ol.control.ScaleLine()
    ]);

    if (fullScreen) {
        ret.extend([new gisApp.controls.FullScreenControl()]);
    }
    if (opt_options.layersLegend) {
        ret.extend([new gisApp.controls.ChooseLayersControl({
            legend: opt_options.layersLegend
        })]);
    }

    return (ret);
};