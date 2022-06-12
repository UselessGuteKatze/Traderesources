function createMap(targetId, projectIconSvgUrl, layers, backGroundLayers, frontGroundLayers) {
    //TODO: mapConfig is necessary to do something with this config, and initialize it humanly. Now, somehow.
    var mapConfig = {
        Center: [7951137.118857781, 6651040.097882961],
        Zoom: 10
    };
    var _MUI_InitializedObject = MUI_Initialize(projectIconSvgUrl, targetId);
    var $this = $("#" + targetId);
    var baseClass = "agro-weather-map";
    $this.addClass(baseClass);
    var darkosmLayer = new ol.layer.Tile({
        source: new ol.source.OSM({ "url": "http://{a-c}.basemaps.cartocdn.com/dark_all/{z}/{x}/{y}.png" })
    });
    darkosmLayer.setOpacity(0.8);
    var kzGeometry = GetKzBorder('rgba(245, 245, 245, 0.85)');
    kzGeometry.setOpacity(0);

    var mapLayers = [];
    var backGroundLayersLegend = [];
    if (backGroundLayers.length == 0) {
        mapLayers.push(darkosmLayer);
        backGroundLayersLegend.push({
            text: 'Тёмная схема',
            layers: [darkosmLayer]
        });
    }
    var frontGroundLayersLegend = [];

    layers.forEach(function (layer) { 
        mapLayers.push(layer.layer);
    })

    backGroundLayers.forEach(function (layer) {

        layer.layers.forEach(function (itemLayer) {
            mapLayers.push(itemLayer);
        });
        backGroundLayersLegend.push({
            text: layer.text,
            layers: layer.layers
        });

    })

    frontGroundLayers.forEach(function (layer) {

        layer.layers.forEach(function (itemLayer) {
            mapLayers.push(itemLayer);
        });
        frontGroundLayersLegend.push({
            text: layer.text,
            layers: layer.layers
        });

    })

    mapLayers.push(kzGeometry);

    var controls = ol.control.defaults({ rotate: false });
    var interactions = ol.interaction.defaults({ altShiftDragRotate: false, pinchRotate: false })

    var map = new ol.Map({
        controls: controls.extend([
            new ol.control.FullScreen(),
            new gisApp.controls.ChooseLayersControl({
                legend: backGroundLayersLegend //[{ text: T("Тёмная схема"), layers: [darkosmLayer] }]
            }),
            new gisApp.controls.ChooseLayersControl({
                legend: frontGroundLayersLegend
            })
        ]),
        interactions: interactions,
        target: targetId,
        layers: mapLayers,
        view: new ol.View({
            center: mapConfig.Center,
            zoom: mapConfig.Zoom
        })
    });
    map.getView().fit(kzGeometry.getSource().getExtent(), map.getSize());

    frontGroundLayers.forEach(function (layer) {
        layer.layers.forEach(function (itemLayer) {
            if (itemLayer.isCenter === true) {
                map.getView().fit(itemLayer.getSource().getExtent(), map.getSize());
            }
        });
    })

    frontGroundLayers.forEach(function (layer, index) {
        if (index == 0) { map.currentLayerName = layer.code; };
    })
    

    $(".ol-zoom").append('<button type="button" class="btn-filter fa fa-filter" title="Фильтр слоя"></button>');
    $(".btn-choose-layer").appendTo(".ol-zoom");
    $(".choose-layers").hide();
    $(".ol-full-screen-false").appendTo(".ol-zoom");
    $(".ol-rotate").remove();
    $(".ol-full-screen").remove();
    $(".ol-attribution").remove();
    $(".ol-zoom").addClass("gis-map widget-base widget-defaults");
    $(".ol-zoom").addClass("gis-map");

    $(".btn-choose-layer").click();
    $(".btn-choose-layer").click();
    $(".btn-choose-layer").attr("layerType", "frontground-layers");
    $(".btn-choose-layer").first().attr("layerType", "background-layers");
    $(".gis-choose-layers-panel").addClass("frontground-layers");
    $(".gis-choose-layers-panel").first().addClass("background-layers");

    $(".btn-choose-layer").click(function () {
        MUI_HideMenus();
        $(`.${$(this).attr('layerType')}`).show(WIDGET_TOGGLE_TIMEOUT);
    });

    $(`#${targetId} .frontground-layers .gis-choose-layer-item`).click(function () {
        var thisText = $(this).children(":first").html();
        frontGroundLayers.forEach(function (layer) {
            if (layer.text == thisText) {
                map.currentLayerName = layer.code;
                $(".layer-filter").addClass("hidden-filter");
                $(`#${layer.code}-filter`).removeClass("hidden-filter");
            }
        });
        console.log(map.currentLayerName);
    });

    $(`#${targetId}`).append(`
            <div id="filter-widget" class="widget-base widget-defaults filter-widget ol-toggle-options ol-unselectable" style="">
                <div id="filter-widget-close" class="fa fa-times"></div>
            </div>
    `);
    $("#filter-widget").hide();

    frontGroundLayers.forEach(function (layer) {
        if (layer.filters != null) {
            var filterhtml = `
                <div id="${layer.code}-filter" layer-code="${layer.code}" class="layer-filter ${map.currentLayerName == layer.code ? "" : "hidden-filter"}">
                <p class="filter-name"><b>${layer.text}:</b><br>
            `;

            layer.filters.forEach(function (filter) {
                if (layer.raster) {
                    if (filter.inputtype != 'select') {
                        filterhtml += `
                        <p><b>${filter.label}:</b><br>
                           <input class="${layer.code}-field" field="${filter.field}" condition="${filter.condition}" type="${filter.inputtype}">
                        </p>
                    `;
                    } else {
                        filterhtml += `
                        <div style="position: relative; padding-bottom: 10px;"><b>${filter.label}:</b><br>
                           <textarea style="color: #333;" class="${layer.code}-field  ${layer.code}-${filter.field}-view" field="${filter.field}" list="${layer.code}-field-list" values="" condition="${filter.condition}" type="text" disabled></textarea>
                           <div class="field-hover ${layer.code}-${filter.field}-hover"></div>
                        </div>
                    `;

                        function getChildrenCount(item) {
                            var count = 0;
                            if (item.children != null) {
                                item.children.forEach(function (child) {
                                    count += getChildrenCount(child);
                                });
                            } else {
                                count++;
                            }
                            return count;
                        }

                        function renderList(list, parent) {
                            var html = '';
                            list.forEach(function (item) {
                                if (item.children != null) {
                                    html += `<span class="${layer.code}-${filter.field}-collapse" code="${item.code}"></span>`;
                                }
                                html += `<span id="${layer.code}-${filter.field}-${item.code}" class="select-item ${layer.code}-${filter.field}-item select-item-unchecked" layer="${layer.code}" parent="${parent}" code="${item.code}"><span>${item.text}</span></span>`;
                                html += `<div class="child-items ${layer.code}-${filter.field}-${item.code}-childs">`;
                                if (item.children != null) {
                                    html += renderList(item.children, item.code);
                                }
                                item.count = getChildrenCount(item);
                                html += '</div><div></div>';
                            });
                            return html;
                        }

                        filterhtml += `<div class="${layer.code}-${filter.field}-list items-list">`;
                        filterhtml += `<div class="${layer.code}-${filter.field}-list-close list-close fa fa-times"></div>`;
                        filterhtml += renderList(filter.selectList, null);
                        filterhtml += "</div>";
                    }
                } else {
                    if (filter.inputtype != 'select') {

                    } else if (filter.multiSelect) {
                        
                    } else {
                        filterhtml += `
                        <p><b>${filter.label}:</b><br>
                           <select  class="${layer.code}-field" field="${filter.field}" condition="${filter.condition}">`;

                        filter.selectList.forEach(function (item) {
                            filterhtml += `<option value="${item.code}" ${item.code == filter.defaultValue ? 'selected' : ''}>${item.text}</option>`;
                        });

                        filterhtml += `
                            </select>
                        </p>
                    `;
                    }
                }
            });
            filterhtml += `<div class="${layer.code}-filter-confirm btn-confirm">Применить</div>`;
            filterhtml += `</div>`;
            $("#filter-widget").append(filterhtml);
        } else {
            var filterhtml = `
                <div id="${layer.code}-filter" layer-code="${layer.code}" class="layer-filter ${map.currentLayerName == layer.code ? "" : "hidden-filter"}">
                    <p class="filter-name"><b>${layer.text} - фильтры отсутствуют.</b><br>
                </div>`;
            $("#filter-widget").append(filterhtml);
        }
    })

    frontGroundLayers.forEach(function (layer) {
        if (layer.filters != null) {
            $(`.${layer.code}-filter-confirm`).click(function () {
                var filterParams = {
                    'FILTER': null,
                    'CQL_FILTER': null,
                    'FEATUREID': null
                };
                var filter = "";

                var filterArray = [];

                $(`.${layer.code}-field`).each(function (index) {

                    var field = $(this).attr("field");
                    var value = $(this).val();
                    var values = $(this).attr("values");
                    var datatype = $(this).attr("datatype");
                    var inputtype = $(this).attr("inputtype");
                    var condition = $(this).attr("condition");

                    var filterObject = {};
                    filterObject.field = field;
                    filterObject.value = value;
                    filterArray.push(filterObject);

                    if (layer.raster) {

                        if (condition != 'in') {
                            console.log(`${layer.code} field: ${index}: ${field} ${condition} ${value}`);
                        } else {
                            console.log(`${layer.code} field: ${index}: ${field} ${condition} ${values}`);
                        }

                        if (value != "" && value != "none") {
                            filter += filter.length > 0 ? " AND " : " ";

                            filter += condition != 'in' ? ` ${field} ` : '';

                            switch (condition) {
                                case 'equals':
                                    filter += ` = `;
                                    break;
                                case 'greate':
                                    filter += ` > `;
                                    break;
                                case 'less':
                                    filter += ` < `;
                                    break;
                                case 'greate-or-equal':
                                    filter += ` >= `;
                                    break;
                                case 'less-or-equal':
                                    filter += ` <= `;
                                    break;
                                case 'in':
                                    filter += ``;
                                    break;
                                default:
                                    filter += ` = `;
                            }

                            switch (condition) {
                                case 'in':
                                    filter += ` ${values} `;
                                    break;
                                default:
                                    switch (datatype) {
                                        case 'int':
                                            filter += ` ${value} `;
                                            break;
                                        default:
                                            filter += ` '${value}' `;
                                    }
                            }
                        }

                    } else {

                        if (condition != 'in') {
                            console.log(`${layer.code} field: ${index}: ${field} ${condition} ${value}`);
                        } else {
                            console.log(`${layer.code} field: ${index}: ${field} ${condition} ${values}`);
                        }



                        if (value != "" && value != "none") {
                            filter += filter.length > 0 ? " AND " : " ";

                            filter += condition != 'in' ? ` ${field} ` : '';

                            switch (condition) {
                                case 'equals':
                                    filter += ` = `;
                                    break;
                                case 'greate':
                                    filter += ` > `;
                                    break;
                                case 'less':
                                    filter += ` < `;
                                    break;
                                case 'greate-or-equal':
                                    filter += ` >= `;
                                    break;
                                case 'less-or-equal':
                                    filter += ` <= `;
                                    break;
                                case 'in':
                                    filter += ``;
                                    break;
                                default:
                                    filter += ` = `;
                            }

                            switch (condition) {
                                case 'in':
                                    filter += ` ${values} `;
                                    break;
                                default:
                                    switch (datatype) {
                                        case 'int':
                                            filter += ` ${value} `;
                                            break;
                                        default:
                                            filter += ` '${value}' `;
                                    }
                            }
                        }

                    }
                });

                console.log(filter);
                console.log(filterArray);

                if (layer.raster) {
                    filterParams["CQL_FILTER"] = filter.length > 0 ? filter : " true = true ";
                    layer.layers[0].getSource().updateParams(filterParams);
                } else {
                            map.ShowInformationWidget(true, "Загрузка данных по фильтру..");
                            map.UiHide();

                            var url = layer.filterSource;
                            filterArray.forEach(function (filter) {
                                url = url.replace(filter.field, filter.value);
                            });

                            $.ajax({
                                url: `${rootDir}${url}`,
                                success: function (resultJson) {

                                    var result = JSON.parse(resultJson);
                                    console.log(result);

                                    layer.layers.forEach(function (subLayer) {
                                        layer.toFilter.forEach(function (toFilterItem) {
                                            if (subLayer.id == toFilterItem.layerId) {
                                                var source = subLayer.getSource();
                                                source.clear();
                                                result[toFilterItem.layerId].forEach(function (resultItem) {
                                                    if (toFilterItem.type == 'point') {
                                                        var regionPoint = createVectorPoint(resultItem, resultItem.group, resultItem.x, resultItem.y, resultItem.size, resultItem.border, resultItem.id, resultItem.label, "bold 10px Roboto", "#fff", resultItem.color);
                                                        source.addFeature(regionPoint);
                                                    } else if (toFilterItem.type == 'polygon') {
                                                        var regionPoint = createVectorPolygon(resultItem, resultItem.group, resultItem.wkt, resultItem.id, resultItem.color, resultItem.stroke, resultItem.strokeColor);
                                                        source.addFeature(regionPoint);
                                                    }
                                                });
                                                subLayer.setSource(source);

                                            };
                                        });
                                    });

                                    map.HideInformationWidget();
                                    map.UiShow();
                                },
                                error: function (xhr, status) {
                                    console.log(xhr, status);
                                    $("#widget-panel").hide(WIDGET_TOGGLE_TIMEOUT);
                                    map.UiShow();
                                    map.ShowInformationWidget(false, "Error.");
                                }
                            });

                }
            });

            layer.filters.forEach(function (filter) {
                if (filter.inputtype == 'select') {
                    $(`.${layer.code}-${filter.field}-item`).click(function () {

                        function interactItem(list, code) {
                            list.forEach(function (item) {
                                if (item.code == code) {
                                    toggleItem(item, null);
                                    checkItemParent(filter.selectList, item);
                                } else {
                                    if (item.children != null) {
                                        interactItem(item.children, code);
                                    }
                                }
                            });
                        }

                        function toggleItem(item, parentToggled) {
                            if (parentToggled == null) {
                                if (item.checkeds == 0) {
                                    parentToggled = true;
                                } else {
                                    parentToggled = false;
                                }
                            }   

                            if (parentToggled) {
                                if (item.children != null) {
                                    item.checkeds = item.count;
                                } else {
                                    item.checkeds = 1;
                                }
                            } else {
                                item.checkeds = 0;
                            }

                            if (item.children != null) {
                                item.children.forEach(function (child) {
                                    toggleItem(child, parentToggled);
                                });
                            }

                        }

                        function checkItemParent(list, item) {
                            var checked = 0;
                            if (item.children != null) {
                                item.children.forEach(function (child) {
                                    checked += child.checkeds;
                                });
                            } else {
                                checked = item.checkeds;
                            }
                            item.checkeds = checked;

                            function searchParent(list, child, parent) {
                                list.forEach(function (item) {
                                    if (item.code == child.parent) {
                                        parent = item;
                                    } else {
                                        if (item.children != null) {
                                            parent = searchParent(item.children, child, parent)
                                        }
                                    }
                                });
                                return parent;
                            }

                            if (item.parent != null) {
                                var parent = searchParent(list, item, {});
                                checkItemParent(list, parent);
                            }
                        }

                        var code = $(this).attr("code");
                        interactItem(filter.selectList, code);


                        function refreshList(list) {
                            list.forEach(function (item) {
                                $(`#${layer.code}-${filter.field}-${item.code}`).removeClass("select-item-checked");
                                $(`#${layer.code}-${filter.field}-${item.code}`).removeClass("select-item-unchecked");
                                $(`#${layer.code}-${filter.field}-${item.code}`).removeClass("select-item-intermediate");

                                if (item.checkeds == item.count) {
                                    $(`#${layer.code}-${filter.field}-${item.code}`).addClass("select-item-checked");
                                } else if (item.checkeds == 0) {
                                    $(`#${layer.code}-${filter.field}-${item.code}`).addClass("select-item-unchecked");
                                } else {
                                    $(`#${layer.code}-${filter.field}-${item.code}`).addClass("select-item-intermediate");
                                }

                                if (item.children != null) {
                                    refreshList(item.children);
                                }
                            });
                        }

                        refreshList(filter.selectList);

                        filter.selectLevels.forEach(function (level) {
                            level.codes = [];
                            level.texts = [];
                        });

                        function getValues(list) {
                            list.forEach(function (item) {

                                if (item.children != null) {
                                    if (item.count == item.checkeds) {
                                        if (filter.datatype == 'in-number') {
                                            filter.selectLevels[item.level].codes.push(item.code);
                                        } else {
                                            filter.selectLevels[item.level].codes.push(`'${item.code}'`);
                                        }
                                        filter.selectLevels[item.level].texts.push(item.text);
                                    } else if (item.checkeds != 0) {
                                        getValues(item.children);
                                    }
                                } else {
                                    if (item.checkeds > 0) {
                                        if (filter.datatype == 'in-number') {
                                            filter.selectLevels[item.level].codes.push(item.code);
                                        } else {
                                            filter.selectLevels[item.level].codes.push(`'${item.code}'`);
                                        }
                                        filter.selectLevels[item.level].texts.push(item.text);
                                    }
                                }
                            });
                        }

                        getValues(filter.selectList);

                        var filterString = "";
                        var textsArray = [];
                        var textsString = "";

                        filter.selectLevels.forEach(function (level) {
                            if (level.texts.length > 0) {
                                level.texts.forEach(function (text) {
                                    textsArray.push(text);
                                });

                                var codes = level.codes.join(", ");

                                filterString += filterString.length > 0 ? ` or ${level.field} in (${codes}) ` : ` ( ${level.field} in (${codes}) `;
                            }
                        });

                        filterString += filterString.length > 0 ? ` ) ` : ``;
                        textsString = textsArray.join(", ");

                        console.log(filterString);
                        console.log(textsString);

                        $(`.${layer.code}-${filter.field}-view`).val(textsString);
                        $(`.${layer.code}-${filter.field}-hover`).attr('title', textsString);
                        $(`.${layer.code}-${filter.field}-view`).attr('values', filterString);

                        $(`.${layer.code}-${filter.field}-view`).height('auto');
                        $(`.${layer.code}-${filter.field}-view`).height($(`.${layer.code}-${filter.field}-view`).prop('scrollHeight'));

                    });

                    $(`.${layer.code}-${filter.field}-collapse`).each(function () {
                        var childs = $(`.${layer.code}-${filter.field}-${$(this).attr('code')}-childs`);
                        childs.css('display', 'none');
                        $(this).addClass("fa-chevron-right");
                    });

                    $(`.${layer.code}-${filter.field}-collapse`).click(function () {
                        var childs = $(`.${layer.code}-${filter.field}-${$(this).attr('code')}-childs`);

                        if (childs.css('display') == 'none') {
                            childs.css('display', 'block');
                            $(this).removeClass("fa-chevron-right");
                            $(this).addClass("fa-chevron-down");
                        } else {
                            childs.css('display', 'none');
                            $(this).removeClass("fa-chevron-down");
                            $(this).addClass("fa-chevron-right");
                        }
                    });

                    $(`.${layer.code}-${filter.field}-hover`).click(function () {
                        $(`.${layer.code}-${filter.field}-list`).show(_MUI_InitializedObject.TimeOut);
                    });

                    $(`.${layer.code}-${filter.field}-list-close`).click(function () {
                        $(`.${layer.code}-${filter.field}-list`).hide(_MUI_InitializedObject.TimeOut);
                    });

                    $(`.${layer.code}-${filter.field}-list`).hide();

                }
            });

        }
    })

    $('.filter-widget').css('max-height', $(window).height() - 230);

    map.on('movestart', function () {
        $(".gis-choose-layers-panel").hide();
        $(".filter-widget").hide();
    });
    $(".btn-filter").click(function () {
        MUI_HideMenus();
        $(".filter-widget").show(_MUI_InitializedObject.TimeOut);
    });
    $("#filter-widget-close").click(function () {
        $(".filter-widget").hide(_MUI_InitializedObject.TimeOut);
    });

    MUI_ReBuild(_MUI_InitializedObject);
    $informationWidget = MUI_CreateWidget("widget-information", "widget-defaults widget-information");
    $informationWidget.hide();
    $this.append($informationWidget);

    map.on("click", function () {
        MUI_HideMenus();
    });
    map.on('movestart', function () {
        MUI_HideMenus();
    });

    map.HideInformationWidget = function (delay) {
        function close() {
            $informationWidget.removeClass('process');
            $informationWidget.hide(500);
            $informationWidget.html('');
        }
        setTimeout(function () {
            close();
        }, delay !== undefined ? delay : 1200);
    };
    map.ShowInformationWidget = function (isLoader, message) {
        if (isLoader) {
            if (!$informationWidget.hasClass('process')) {
                $informationWidget.addClass('process');
            }
            $informationWidget.html(`<span>${message}</span>`);
        } else {
            $informationWidget.html(`<span>${message}</span>`);
            map.HideInformationWidget(6000);
        }
        if (!$informationWidget.is(":visible")) {
            $informationWidget.show(500);
        }
    };
    $widgetCoordinates = createWidgetCoordinates();
    $this.append($widgetCoordinates);
    map.on('pointermove', function (evt) {
        var xy3857 = evt.coordinate;
        var xy4326 = ol.proj.transform(xy3857, 'EPSG:3857', 'EPSG:4326');
        $widgetCoordinates.show();
        $widgetCoordinates.find('.latitude').html(xy4326[1].toFixed(5));
        $widgetCoordinates.find('.longitude').html(xy4326[0].toFixed(5));
    });
    map.UiShow = function () {
        $('.ol-zoom').show(_MUI_InitializedObject.TimeOut);
        $widgetCoordinates.show(_MUI_InitializedObject.TimeOut);
        MUI_Show();
    };
    map.UiHide = function () {
        $('.ol-zoom').hide(_MUI_InitializedObject.TimeOut);
        $widgetCoordinates.hide(_MUI_InitializedObject.TimeOut);
        MUI_Hide();
    };
    map.RemoveLayerByName = function (name) {
        map.getLayers().forEach(function (layer) {
            if (layer.get('name') !== undefined && layer.get('name') === name) {
                map.removeLayer(layer);
            }
        });
    };

    $(window).resize(function () {
        waitLastCall(function () {
            if ($(":not(:root):fullscreen").length > 0 || ($(".interact-widget").length > 0 && $(".interact-widget").css("display") != 'none')) {
                MUI_Hide();
                $widgetCoordinates.hide(_MUI_InitializedObject.TimeOut);
            } else {
                MUI_Show();
                $widgetCoordinates.show(_MUI_InitializedObject.TimeOut);
            }
            $this.width($(window).width());
            $this.height($(window).height());
            map.updateSize();
        }, 150, "resize");
    });

    $(window).resize();
    return map;
}

function createMiniMap(targetId, xy, geojson, color, width, geoserverLayer, filter) {

    var darkosmLayer = new ol.layer.Tile({
        source: new ol.source.OSM({ "url": "http://{a-c}.basemaps.cartocdn.com/dark_all/{z}/{x}/{y}.png" })
    });
    darkosmLayer.setOpacity(0.8);

    var minimaplayers = [darkosmLayer];

    if (geoserverLayer != null) {
        minimaplayers.push(createRasterLayer(geoserverLayer, filter));
    }

    $(`#${targetId}`).html("");
    var minimap = new ol.Map({
        controls: ol.control.defaults({
            attribution: false,
            zoom: true,
        }),
        target: targetId,
        layers: minimaplayers,
        view: new ol.View({
            center: xy,
            zoom: 12
        })
    });

    if (geojson != null) {
        var vectorLayer = createVectorLayerByGeoJson(geojson, color, width);
        minimap.addLayer(vectorLayer);
        var extent = vectorLayer.getSource().getExtent();
        minimap.getView().fit(extent, minimap.getSize());
    }

    $(window).resize(function () {
        waitLastCall(function () {
            minimap.updateSize();
        }, 150, `resize-minimap-${targetId}`);
    });
    $(window).resize();

    return minimap;

}

function createRasterLayer(geoserverLayer, filter) {
    var rasterLayer = new ol.layer.Tile({
        visible: true,
        source: new ol.source.TileWMS({
            url: 'https://maps.qoldau.kz/gs5',
            params: {
                FORMAT: 'image/png',
                VERSION: '1.1.1',
                tiled: true,
                LAYERS: `service:${geoserverLayer}`,
                exceptions: "application/vnd.ogc.se_inimage",
                tilesOrigin: 5564899.8761452 + "," + 4949441.58291756
            }
        })
    });

    if (filter != null) {
        var filterParams = {
            'FILTER': null,
            'CQL_FILTER': null,
            'FEATUREID': null
        };
        filterParams["CQL_FILTER"] = filter;
        rasterLayer.getSource().updateParams(filterParams);
    }

    return rasterLayer;

}

function createEmptyVectorLayer(id) {
    var vectorSource = new ol.source.Vector({
        features: []
    });
    var vectorLayer = new ol.layer.Vector({
        source: vectorSource
    });

    vectorLayer.id = id;

    return vectorLayer;
}

function createVectorLayerByGeoJson(geojson, color, width) {
    var format = new ol.format.GeoJSON({
        featureProjection: "EPSG:3857"
    });
    var vectorSource = new ol.source.Vector({
        features: format.readFeatures(geojson)
    });
    var vectorLayer = new ol.layer.Vector({
        source: vectorSource,
        style: new ol.style.Style({
            stroke: new ol.style.Stroke({
                color: color,
                width: width
            })
        })
    });

    return vectorLayer;

}

function createVectorLayerByFeatures(id, features) {
    var vectorSource = new ol.source.Vector({
        features: features
    });
    var vectorLayer = new ol.layer.Vector({
        source: vectorSource
    });

    vectorLayer.id = id;

    return vectorLayer;
}

function createFeatureArray(featureArray) {
    var features = []

    var format = new ol.format.WKT();

    featureArray.forEach(function (featuteItem) {

        var style = new ol.style.Style({
            fill: new ol.style.Fill({
                color: featuteItem.fillColor
            }),
            stroke: new ol.style.Stroke({
                color: featuteItem.strokeColor,
                width: featuteItem.strokeWidth
            })
        });

        var feature = new ol.Feature({
            geometry: format.readGeometry(featuteItem.wkt,
                {
                    dataProjection: 'EPSG:4326',
                    featureProjection: 'EPSG:3857'
                }
            ),
            data: featuteItem.data,
            id: featuteItem.id
        });

        feature.setStyle([style]);

        features.push(feature);
    });

    return features;
}

function createVectorPoint(pointObject, pointGroup, pointX, pointY, pointR, pointBorder, pointId, pointText, pointTextStyle, pointTextColor, pointColor) {
    var xy3857 = ol.proj.transform([pointX, pointY], 'EPSG:4326', 'EPSG:3857');
    var point = new ol.Feature({
        geometry: new ol.geom.Point(xy3857),
        data: pointObject,
        id: pointId,
        group: pointGroup
    });

    var pointInner = new ol.style.Style({
        text: new ol.style.Text({
            text: pointText,
            scale: 1.1,
            font: pointTextStyle == "icon" ? 'normal 12px FontAwesome' : pointTextStyle,
            offsety: 0,
            offsetx: 0,
            fill: new ol.style.Fill({ color: pointTextColor })
        })
    });

    var pointBackground = new ol.style.Style({
        text: new ol.style.Text({
            text: '\uf111',
            scale: 1.1,
            font: `normal ${pointR}px FontAwesome`,
            offsety: 0,
            offsetx: 0,
            fill: new ol.style.Fill({ color: 'rgba(49, 52, 58, 0.85)' })
        })
    });

    var pointBorder = new ol.style.Style({
        text: new ol.style.Text({
            text: '\uf111',
            scale: 1.1,
            font: `normal ${(pointR + pointBorder)}px FontAwesome`,
            offsety: 0,
            offsetx: 0,
            fill: new ol.style.Fill({ color: pointColor })
        })
    });

    point.setStyle([pointBorder, pointBackground, pointInner]);
    return point;
}

function createVectorPolygon(polygonObject, polygonGroup, polygonWKT, polygonId, polygonColor, polygonStroke, polygonStrokeColor) {
    var format = new ol.format.WKT();

    var style = new ol.style.Style({
        fill: new ol.style.Fill({
            color: polygonColor
        }),
        stroke: new ol.style.Stroke({
            color: polygonStrokeColor,
            width: polygonStroke
        })
    });

    var feature = new ol.Feature({
        geometry: format.readGeometry(polygonWKT,
            {
                dataProjection: 'EPSG:4326',
                featureProjection: 'EPSG:3857'
            }
        ),
        data: polygonObject,
        id: polygonId,
        group: polygonGroup
    });

    feature.setStyle([style]);

    return feature;

}

function numberToBeautyNumber(number) {

    var newnumber = String(number);

    var spacecount = String(String(number).length / 3).split('.')[0];

    for (var i = 1; i <= spacecount; i++) {

        var position = (String(number).length) - (i * 3);

        newnumber = [newnumber.slice(0, position), " ", newnumber.slice(position)].join('');
    }

    return newnumber;

}

var waitLastCall = (function () {
    var funcs = {};
    return function (newfunc, ms, newfuncid) {
        if (!newfuncid) {
            funcid = "Don't call this twice without a uniqueId";
        }
        if (funcs[newfuncid]) {
            clearTimeout(funcs[newfuncid]);
        }
        funcs[newfuncid] = setTimeout(newfunc, ms);
    };
})();