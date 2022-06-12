WIDGET_TOGGLE_TIMEOUT = 500;
function UIRestructure_Initialize() {
    let $widgetLogo = $("<div class='widget-logo ol-toggle-options ol-unselectable'></div>");
    let $widgetMenu = $("<div class='widget-menu toggle-options ol-unselectable'></div>");
    let $sideBarMenu = $('.sidebar-menu').clone();
    $widgetMenu.append($sideBarMenu);
    $widgetMenu.hide();
    let $widgetCopyRight = $("<div class='widget-copyright ol-toggle-options ol-unselectable' id='copyright-logo'><a href='https://www.qoldau.kz' target='_blank'></a></div>");
    let isUserDefined = $('.header-sign-out-text').length > 0;
    if (isUserDefined) {
        var $widgetAccount = $(`<div class='widget-account toggle-options ol-unselectable'><a href='${$('.header-user-info').attr('href')}' title='${$('.header-user-text').text()}'><i class="fa fa-user-circle"></i></a></div>`);
        var $widgetLogOut = $(`<div class='widget-logout toggle-options ol-unselectable'><a href='${$('.header-sign-out').attr('href')}' title='Выйти'><i class="fa fa-sign-out"></i></a></div>`);
    }
    let $map = $("#forecast-map");
    let $body = $('.project-weather').addClass("body-load");
    $body.css("background-color", "black");
    $body.css("height", "auto");
    $body.html('');
    $body.append($map);
    return {
        Logo: $widgetLogo,
        Menu: $widgetMenu,
        CopyRight: $widgetCopyRight,
        IsUserDefined: isUserDefined,
        Account: isUserDefined ? $widgetAccount : undefined,
        LogOut: isUserDefined ? $widgetLogOut : undefined,
        ToggleTimeOut: WIDGET_TOGGLE_TIMEOUT
    };
}
function UIRestructure_ReBuild(targetCssName, logoCssName, uIRestructureConfig) {
    /*
        Very important!
            Before using the function, registration of the MAP in the target must be successful.
            As well as the following notes:
                - targetCssName - Class name of map container
                - logoCssName - Class name of logotype image (!with! background-image)
                - uIRestructureConfig - Object that was received during initialization

    */
    var $target = $("." + targetCssName);
    $target.append(uIRestructureConfig.Logo);
    $(".widget-logo").addClass(logoCssName);
    $target.append(uIRestructureConfig.Menu);
    var $widgetMenuButton = $("<div class='widget-menu-button root-menu-item toggle-options ol-unselectable'><span class='icon-bar'></span><span class='icon-bar'></span><span class='icon-bar'></span></div>");
    $widgetMenuButton.click(function () { $('.widget-menu').toggle(500); });
    $.sidebarMenu($(".sidebar-menu"));
    $(".sidebar-menu").css("padding-bottom", "0px !important");
    $target.append($widgetMenuButton);
    $target.append(uIRestructureConfig.CopyRight);
    if (uIRestructureConfig.IsUserDefined) {
        $target.append(uIRestructureConfig.Account);
        $target.append(uIRestructureConfig.LogOut);
    }
}
function UIRestructure_HideInterface() {
    $('.widget-logo').hide(WIDGET_TOGGLE_TIMEOUT);
    $('.widget-menu-button').hide(WIDGET_TOGGLE_TIMEOUT);
    $('.widget-menu').hide(WIDGET_TOGGLE_TIMEOUT);
    $('.widget-copyright').hide(WIDGET_TOGGLE_TIMEOUT);
    $('.widget-account').hide(WIDGET_TOGGLE_TIMEOUT);
    $('.widget-logout').hide(WIDGET_TOGGLE_TIMEOUT);
}
function UIRestructure_ShowInterface() {
    $('.widget-logo').show(timeOut);
    $('.widget-menu-button').show(WIDGET_TOGGLE_TIMEOUT);
    $('.widget-copyright').show(WIDGET_TOGGLE_TIMEOUT);
    $('.widget-account').show(WIDGET_TOGGLE_TIMEOUT);
    $('.widget-logout').show(WIDGET_TOGGLE_TIMEOUT);
}