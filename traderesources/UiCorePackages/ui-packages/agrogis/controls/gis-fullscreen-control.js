window.gisApp.controls.FullScreenControl = function (opt_options) {
    var options = opt_options || {};
    if (!options.tipLabel) {
        options.tipLabel = "На весь экран";
    }
    if (!options.className) {
        options.className = "gis-full-screen-control";
    }
    ol.control.FullScreen.call(this, options);
};
ol.inherits(window.gisApp.controls.FullScreenControl, ol.control.FullScreen);