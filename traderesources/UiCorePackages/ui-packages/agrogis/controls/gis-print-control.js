window.gisApp.controls.PrintControl = function(opt_options) {
    var options = opt_options || {};

    var $btnPrint = $("<button type='button'>");
    $btnPrint.addClass('gis-button btn-gis-print');
    $btnPrint.attr("title", "Печать карты");
    $btnPrint.text("P");
    var that = this;
    var $this = $(this);

    var $printWrapper = $("<div class='print-map'>");
    var $printMap = $("<div style='width:100%; height:95%;background:aliceblue; border:1px solid #ccc;'/>");
    var $btnDiv = $("<div'></div>");
    
    var $goToPrint = document.createElement("input");
    $goToPrint.type = "button";
    $goToPrint.value = "Перейти к печати";
    
    $goToPrint.onclick = function() {
        modalMap.once('postcompose', function(event) {
            var canvas = event.context.canvas;
            var imageSource = canvas.toDataURL('image/png');
            showPrintModule(imageSource);
        });
        modalMap.renderSync();
    };

    $btnDiv.append($goToPrint);
    
    $printWrapper.append($printMap);
    $printWrapper.append($btnDiv);
    $this.after($printWrapper);
    
    
    var modalMap;
    $printWrapper.bind("dialogclose", function() {
        
    });
    var openCount = 0;
    $printWrapper.bind("dialogopen", function() {
        var map = that.getMap();

        var view = map.getView();
        var zoom = view.getZoom();
        var center = view.getCenter();

        var printView = new ol.View({
            center: center,
            zoom: zoom,
            projection: gisLayers.defaultSrsName,
            extent: gisLayers.mapBounds,
            minZoom: gisLayers.minZoom,
            maxZoom: gisLayers.maxZoom
        });
        if (openCount == 0) {
            modalMap.setTarget($printMap[0]);
        }
        modalMap.setView(printView);
        openCount += 1;
    });
    $printWrapper.ready(function() {
        var printView = new ol.View({
            center: options.center,
            zoom: options.zoom,
            projection: gisLayers.defaultSrsName,
            extent: gisLayers.mapBounds,
            minZoom: gisLayers.minZoom,
            maxZoom: gisLayers.maxZoom
        });

        modalMap = new ol.Map({
            controls: ol.control.defaults({
                attributionOptions: ({
                    collapsible: false
                })
            }),
            layers: options.layers,
            target: $printMap[0],
            view: printView,
            logo: false
        });
        var zoomslider = new ol.control.ZoomSlider();
        modalMap.addControl(zoomslider);
        modalMap.render();
        });
   
    $btnPrint.click(function() {
        $printWrapper.dialog({
            modal: true,
            resizable: true,
            width: 700,
            height: 700,
            title: "Экстент карты для печати",
        });
    });
    

    var showPrintModule = function(imageSource){
			var img = new Image();
			img.src = imageSource;
            var height = img.height;
            var modalHeight = height + 250;
            var width = img.width + 20;
			var strPage = '<html>'+
                '<head>'+
				'<style>'+
				'.map{'+
					'position:relative;'+
				'}'+
				'.others{'+
					'position:relative;'+
					'padding-top:10px;'+
				'}'+
                '@media print {'+
			        '.noprint { display: none; }'+
			        'textarea { border: none; }'+
                '}'+
				'</style>'+
			    '<script type="text/javascript">'+
			    '</script>'+
				'</head>'+
                    '<body>'+
			            '<div id="section-to-print" style="margin-bottom:10px;">'+
                            '<h1 id="title">Печать карты</h1>'+
                        '</div>'+
			            '<input type="button" class="noprint" value="Печать" style="margin-bottom:10px" onClick="window.print()">'+
						'<div id="section-to-print">'+
                             '<div class="map">'+
						         '<img src="'+imageSource+'" width="'+img.width+'" height="'+height+'">'+
						     '</div>'+
							 '<div class="others">'+
			                  'Комментарии:'+
	                              '<textarea id="comments" rows="2" cols="30" style="width:100%;padding:10px; margin-top:5px; margin-bottom:10px;"></textarea>'+
			                 '</div>'+
						'</div>'+
			            '<input type="button" class="noprint" value="Печать" onClick="window.print()">'+
                    '</body>'+
                  '</html>';
			var wnd = window.open("about:blank", "", "width="+width+",height="+modalHeight+",resizable,scrollbars=yes,status=1");
			wnd.document.write(strPage);
		};


    var $element = $('<div>');
    $element.addClass('btn-gis-print ol-selectable ol-control');
    $element.append($btnPrint);

    ol.control.Control.call(this, {
        element: $element[0],
        target: options.target
    });
};
ol.inherits(window.gisApp.controls.PrintControl, ol.control.Control);

