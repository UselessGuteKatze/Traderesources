$(document).ready(function () {
    var mapId = "acc-stat-map";
    var project = 'https://demo-techreg.qoldau.kz/Theme/ui-packages/project-theme/logo/qazcert.svg';
    var mapData = JSON.parse($(`#${mapId}`).attr("map-data"));
    var layers = [];

    var locationLayer = createVectorLayerByFeatures("locationsBorders", createFeatureArray(mapData.locations));
    locationLayer.isCenter = true;
    locationLayer.setOpacity(0.8);

    var locationDataLayer = createEmptyVectorLayer("locationsData");
    mapData.locationsData.forEach(function (location) {
        var locationPoint = createVectorPoint(location, location.group, location.x, location.y, location.size, location.border, location.id, location.label, "bold 10px Roboto", "#fff", location.color);
        var source = locationDataLayer.getSource();
        source.addFeature(locationPoint)
        locationDataLayer.setSource(source);
    });
    locationDataLayer.setOpacity(0.8);

    var filters = [
        {
            label: 'Тип',
            field: 'type',
            inputtype: 'select',
            datatype: 'string',
            condition: 'equal',
            selectList: [
                { text: "Все", code: "All" },
                { text: "ОПС ОП", code: "OPSOP" },
                { text: "ОВ", code: "OV" },
                { text: "ПЛ", code: "PL" },
                { text: "ПК", code: "PK" },
                { text: "МВИ", code: "MVI" },
                { text: "ИО", code: "IO" },
                { text: "ОПС СМ", code: "OPSSM" },
                { text: "МЛ", code: "ML" },
                { text: "КЛ", code: "KL" },
                { text: "ОПС П / ОПС ПиУ", code: "OPSP/OPSPIU" },
                { text: "ИЛ", code: "IL" }
            ],
            defaultValue: "All",
            multiSelect: false
        },
        {
            label: 'Статус',
            field: 'status',
            inputtype: 'select',
            datatype: 'string',
            condition: 'equal',
            selectList: [
                { text: "Все", code: "All" },
                { text: "Прекращение действия", code: "Terminated" },
                { text: "Действует", code: "Acting" },
                { text: "Приостановлен на 6 мес.", code: "Suspended" },
                { text: "Прекращение деятельности", code: "ActivityTerminated" },
                { text: "Отозван", code: "Recalled" }
            ],
            defaultValue: "All",
            multiSelect: false
        }
    ];

    layers.push({ code: 'accSubjects', text: 'Субъекты', layers: [locationLayer, locationDataLayer], raster: false, filters: filters, filterSource: "map/getfeatures/type/status", toFilter: [{ layerId: "locationsData", type: "point" }] });

    var map = createMap(mapId, project, [], [], layers);


    $(`#${mapId}`).append(`
        <div id="right-mini-widget-panel" class="widget-base widget-defaults right-mini-widget-panel ol-toggle-options ol-unselectable">
            <div class="widget-content"></div><div class="widget-panel-close fa fa-times"></div>
        </div>
    `);
    $("#right-mini-widget-panel").hide();

    $(".widget-panel-close").click(function () {
        $("#right-mini-widget-panel").hide(WIDGET_TOGGLE_TIMEOUT);
    });


    $(`#${mapId}`).append('<div id="widget-panel" class="widget-base widget-defaults widget-panel interact-widget ol-toggle-options ol-unselectable" style=""></div>');
    $("#widget-panel").hide();

    map.on('pointermove', function (e) {
        onInteract(map, e, "pointermove");
    });
    map.on('click', function (e) {
        onInteract(map, e, "click");
    });

});

function onInteract(map, e, eventName) {
    $("#right-mini-widget-panel").hide();
    map.forEachFeatureAtPixel(e.pixel, function (feature) {
        if (feature && feature.get('group') === "accSubj") {
            drawlocationTable("right-mini-widget-panel", feature.get('data'));
            var feature = feature.get('data');
            if (eventName == "click") {
                drawMainPanel(map, feature);
            }
        }
    });
}

function drawlocationTable(targetId, locationData) {
    var html = `
        <table>
            <tbody>
                <tr><td colspan="2"  class="table-header">${locationData.locationName}</td></tr>
                <tr><td class="table-title">Количество</td><td class="table-value">${locationData.label}</td></tr>
            </tbody>
        </table>
    `;
    $(`#${targetId} .widget-content`).html(html);
    $(`#${targetId}`).css('border-color', locationData.color.replace("0.8","0.6"));
    $(`#${targetId}`).show();
}

function drawMainPanel(map, feature) {
    map.ShowInformationWidget(true, "Загрузка данных по выбранной точке..");
    map.UiHide();

    var html = `
            <div id="widget-panel-head">
                Информация
                <div id="widget-panel-close" class="fa fa-times"></div>
            </div>
            <div id="widget-panel-body">
            </div>
        `;
    $("#widget-panel").html(html);

    $("#widget-panel-close").click(function () {
        $("#widget-panel").hide(WIDGET_TOGGLE_TIMEOUT);
        map.UiShow();
    });

    
    map.HideInformationWidget();

    $(".widget-panel-close").click();

    var html = `
        <div class="data-widget" id="animal-data-widget">
        </div>
    `;
    $("#widget-panel-body").html(html);


    var html = `
        <div id="animal-data-widget-body">
            <table>
                <tbody>
                    <tr><td colspan="6"  class="table-header">Субъекты</td></tr>
                    <tr>
                        <td class="table-title">Наименование</td>
                        <td class="table-title">Тип</td>
                        <td class="table-title">Номер</td>
                        <td class="table-title">Статус</td>
                        <td class="table-title">С</td>
                        <td class="table-value">По</td>
                    <tr><td colspan="6"><hr></td></tr>
    `;

    feature.data["subjects"].forEach(function (data) {
        html += `<tr class="table-row">
                        <td class="table-title">${data.name}</td>
                        <td class="table-title">${data.type}</td>
                        <td class="table-title">${data.number}</td>
                        <td class="table-title">${data.status}</td>
                        <td class="table-title">${new Date(data.datebegin).format("dd.mm.yy")}</td>
                        <td class="table-value">${new Date(data.dateend).format("dd.mm.yy")}</td>
                </tr>`;
    });

    html += `       </tbody>
            </table>
        </div>
    `;
    $("#animal-data-widget").html(html);

            

    $("#widget-panel").show(WIDGET_TOGGLE_TIMEOUT);
    MUI_Hide();

}



