function MUI_CreateStylizedWidget(widgetId, widgetCssClass, widgetTitle, controls) {
    var $widget = $(`<div id='${widgetId}' class='widget-base ${widgetCssClass !== undefined ? widgetCssClass : ""} ol-toggle-options ol-unselectable'></div>`);
    $widget.hide();
    var $widgetContainer = $(`<div id='${widgetId + "-container"}' class='stylized-widget-container'></div>`);
    var $widgetPartControls = $(`<div id='${widgetId + "-controls"}' class='stylized-widget-controls'></div>`);
    $widgetPartControls.append(`<i id='${widgetId + "-close"}' class='stylized-widget-control fa fa-times' title='${T('Закрыть')}'></i>`);
    if (controls !== undefined) {
        for (let i = 0; i < controls.length; i++) {
            $widgetPartControls.append(`<i id='${controls[i].Id}' class='stylized-widget-control fa ${controls[i].CssClass}' title='${controls[i].Title}'></i>`);
            //$widgetPartControls.find(`#${controls[i].Id}`).hide();
        }
    }
    var $widgetTaskBar = $("<div class='stylized-widget-task-bar'></div>");
    var $table = $("<table></table>");
    var $row = $("<tr></tr>");
    var $cellLeft = $("<td></td>");
    $cellLeft.css("padding-left","5px");
    $cellLeft.append($widgetPartControls);
    var $cellRight = $(`<td>${widgetTitle}</td>`);
    $row.append($cellLeft);
    $row.append($cellRight);
    $table.append($row);
    $widgetTaskBar.append($table);
    $widget.append($widgetContainer);
    $widget.append($widgetTaskBar);
    $widget.find(`#${widgetId + "-close"}`).click(function () {
        $widgetContainer.html("");
        $widget.hide(WIDGET_TOGGLE_TIMEOUT);
    });
    return {
        WidgetId: widgetId,
        WidgetCssClass: widgetCssClass,
        Widget: $widget,
        WidgetContainer: $widgetContainer,
        WidgetTitle: $cellRight
    };
}