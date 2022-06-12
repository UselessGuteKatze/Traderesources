function createWidgetCoordinates() {
    let $widgetCoordinates = MUI_CreateWidget("widget-coordinates", "widget-defaults widget-coordinates");
    $widgetCoordinates.hide();
    $widgetCoordinates.append(`<div class='latitude' title='${T('Широта')}'></div>`);
    $widgetCoordinates.append(`<div class='longitude' title='${T('Долгота')}'></div>`);
    return $widgetCoordinates;
}