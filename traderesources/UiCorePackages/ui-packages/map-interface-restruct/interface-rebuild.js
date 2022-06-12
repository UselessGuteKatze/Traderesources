WIDGET_TOGGLE_TIMEOUT = 500;
function MUI_Initialize(logotypeUrl, targetId) {
    /*
        Very important!
            - logotypeUrl - full URL to logotypeImage
            - targetId - id map for target content than you want let on body
    */
    let $map = $("#" + targetId).clone();
    let $body = $("body").clone();
    $("body").addClass("body-load");
    $("body").html("");
    $("body").height($(window).height());
    let $widgetPartLogo = $(`<div class='widget-part-logo'><img src='${logotypeUrl}'></div>`);
    let $widgetPartLanguage = $("<div class='widget-part-language'></div>");
    //let langs = $body.find(".header-nav a").filter(function () { return $(this).attr("class").indexOf("lang") > -1; });
    //let $select = $("<select></select>");
    //for (let i = 0; i < langs.length; i++) {
    //    $select.append(`<option ${$(langs[i]).attr("class").indexOf("current-lang") > -1 ? "selected" : ""} value='${$(langs[i]).attr('href')}'>${$(langs[i]).text()}</option>`);
    //}
    //$widgetPartLanguage.append($select);
    let $langs = $body.find(".lang-menu");
    $widgetPartLanguage.append($langs);

    let $widgetMain = MUI_CreateWidget("widget-main", "widget-main");
    $widgetMain.append($widgetPartLogo);
    $widgetMain.append($widgetPartLanguage);

    let isUserDefined = $body.find(".header-sign-out-text").length > 0;
    var $widgetPartAccount = $("<div class='widget-part-account'></div>");

    let $widgetPartMenu = $("<div class='widget-part-menu'><i class='fa fa-bars'></i></div>");
    $widgetPartAccount.append($widgetPartMenu);
    if (isUserDefined) {
        let $menuUserUl = $("<ul class='sidebar-menu'></ul>");
        $menuUserUl.append(`<li><a href='${$body.find(".header-user-info").attr("href")}'><i class="fa fa-circle-thin"></i>${T('Аккаунт')}</a></li>`);
        $menuUserUl.append(`<li><a href='${$body.find(".header-sign-out").attr("href")}'><i class="fa fa-circle-thin"></i>${T('Выход')}</a></li>`);
        var $widgetMenuUser = MUI_CreateWidget("widget-menu-user", "widget-menu-user");
        $widgetMenuUser.append($menuUserUl);
        $widgetMenuUser.hide();
        $widgetPartAccount.append(`<div class='widget-part-account-info'><span>${$body.find(".header-user-text").text()}</span></div>`);
    } else {
        $widgetPartAccount.append(`<div class='widget-part-account-info'><span>${T("Гость")}</span></div>`);
    }
    $widgetMain.append($widgetPartAccount);

    let $widgetMenuMain = MUI_CreateWidget("widget-menu-main", "widget-menu-main");
    let $sideBarMenu = $body.find(".sidebar-menu").clone();
    $widgetMenuMain.append($sideBarMenu);
    $widgetMenuMain.hide();
    
    let $widgetCopyRight = $("<div class='widget-copyright ol-toggle-options ol-unselectable' id='copyright-logo'><a href='https://www.qoldau.kz' target='_blank'></a></div>");
    $.removeData($body);
    $("body").append($map);
    $("body").removeClass("body-load");
    return {
        Id: targetId,
        TimeOut: WIDGET_TOGGLE_TIMEOUT,
        IsUserDefined: isUserDefined,
        WidgetMain: $widgetMain,
        WidgetMenuMain: $widgetMenuMain,
        WidgetMenuUser: $widgetMenuUser,
        WidgetCopyRight: $widgetCopyRight
    };
}
function MUI_ReBuild(MUI_InitializeConfig) {
    /*
        Very important!
            Before using the function, registration of the MAP in the target must be successful.
            As well as the following notes:
                - MUI_InitializeConfig - Object that was received during initialization

    */
    var $target = $("." + MUI_InitializeConfig.Id);
    $target.append(MUI_InitializeConfig.WidgetMain);
    $(".widget-part-language select").change(function () { $(location).attr("href", $(this).val()); });
    $(".widget-part-menu").click(function () {
        //if (MUI_InitializeConfig.IsUserDefined) {
        //    if ($(".widget-menu-user").is(":visible")) {
        //        $(".widget-menu-user").hide(WIDGET_TOGGLE_TIMEOUT);
        //    }
        //}
        //$(".widget-menu-main").toggle(WIDGET_TOGGLE_TIMEOUT);
        if ($(".widget-menu-main").is(":visible")) {
            $(".widget-menu-main").hide(WIDGET_TOGGLE_TIMEOUT);
        } else {
            MUI_HideMenus();
            $(".widget-menu-main").show(WIDGET_TOGGLE_TIMEOUT);
        }
    });
    $target.append(MUI_InitializeConfig.WidgetMenuMain);
    if (MUI_InitializeConfig.IsUserDefined) {
        $target.append(MUI_InitializeConfig.WidgetMenuUser);
        $(".widget-part-account-info span").css("cursor", "pointer");
        $(".widget-part-account-info span").click(function () {
            //if ($(".widget-menu-main").is(":visible")) {
            //    $(".widget-menu-main").hide(WIDGET_TOGGLE_TIMEOUT);
            //}
            //$(".widget-menu-user").toggle(WIDGET_TOGGLE_TIMEOUT);
            if ($(".widget-menu-user").is(":visible")) {
                $(".widget-menu-user").hide(WIDGET_TOGGLE_TIMEOUT);
            } else {
                MUI_HideMenus();
                $(".widget-menu-user").show(WIDGET_TOGGLE_TIMEOUT);
            }
        });
    }
    $.sidebarMenu($(".sidebar-menu"));
    $target.append(MUI_InitializeConfig.CopyRight);
}
function MUI_HideMenus() {
    $(".widget-menu-main").hide(WIDGET_TOGGLE_TIMEOUT);
    $(".widget-menu-user").hide(WIDGET_TOGGLE_TIMEOUT);
    $(".gis-choose-layers-panel").hide(WIDGET_TOGGLE_TIMEOUT);
    $(".filter-widget").hide(WIDGET_TOGGLE_TIMEOUT);
}
function MUI_Hide() {
    $(".widget-main").hide(WIDGET_TOGGLE_TIMEOUT);
    MUI_HideMenus();
    $(".widget-copyright").hide(WIDGET_TOGGLE_TIMEOUT);
}
function MUI_Show() {
    $(".widget-main").show(WIDGET_TOGGLE_TIMEOUT);
    $(".widget-copyright").show(WIDGET_TOGGLE_TIMEOUT);
}
function MUI_CreateWidget(id, cssClass) {
    return $(`<div id='${id}' class='widget-base ${cssClass !== undefined ? cssClass : ""} ol-toggle-options ol-unselectable'></div>`);
}