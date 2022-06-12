(function ($) {
    $.widget("yoda.keyInfoListEditor", {
        _create: function () {
            var value = this.options;
            if (!value) {
                value = {
                    editorName: null,
                    editorId: null,
                    selectedItems: null,
                    urlRequestItems: null,
                    readonly: true
                };
            }
            this._config = {
                editorName: value.editorName,
                editorId: value.editorId,
                urlRequestItems: value.urlRequestItems,
                readonly: value.readonly
            }

            this._initMarkup();
            if (value.selectedItems !== undefined)
                for (var i = 0; i < value.selectedItems.length; i++)
                    this._setItem(value.selectedItems[i]);
            this._subscribeToEvents();
            this._refreshNoneRow();
        },
        _initMarkup: function () {
            var $markup = $(
                "<table name='" + this._config.editorName + "' id='" + this._config.editorId + "' class='kInfoList-editor'>" +
                    (this._config.readonly === true ?
                        "":
                        "<tr style='padding:5px;'>" +
                            "<td>" +
                                "<span id='kInfoList-add-button' class='kInfoList-add-button btn btn-success'>Добавить</span>" +
                            "</td>" +
                        "</tr>"
                    ) +
                    "<tr>" +
                        "<td> " +
                            "<table style='width: 100%;'>" +
                                "<thead>" +
                                    "<tr>" +
                                        "<td class='kInfoList-column-actions'> </td>" +
                                        "<td class='kInfoList-title-cell'>Объекты</td>" +
                                    "<tr>" +
                                "</thead>" +
                                "<tbody class='kInfoList-container'></tbody>" +
                            "</table>" +
                        "</td>" +
                    "</tr>" +
                "</table>"
            );

            this.element.append($markup);
            this._$markup = $markup;
            this._$container = this._$markup.find('.kInfoList-container');
        },
        _setItem: function (item) {
            if (item === undefined || item === null)
                return;
            if (this._config.selectedItems === undefined)
                this._config.selectedItems = new Array();
            var exists = false;
            for (var i = 0; i < this._config.selectedItems.length; i++)
                if (item.Key === this._config.selectedItems[i].Key) {
                    exists = true;
                    break;
                }
            if (exists)
                return;
            this._config.selectedItems.push(item);
            this._$container.append(
                "<tr row-id='" + item.Key + "'>" +
                    "<td class='kInfoList-action-cell'>" +
                        (this._config.editable === true ? "" : "<span class='kInfoList-remove-button' title='Удалить'>Удалить</span>") +
                    "</td>" +
                    "<td class='kInfoList-content-cell'>" + item.Text + "</td>" +
                "</tr>"
            );
            this._refreshNoneRow();
        },
        _refreshNoneRow: function () {
            if (this._config.selectedItems === undefined)
                this._config.selectedItems = new Array();
            if (this._config.selectedItems.length === 0)
                this._$container.append("<tr row-id='none'><td class='kInfoList-content-cell' style='text-align: center; font- size: 10pt; color: darkgrey;' colspan=2><i>Список пуст</i></td></tr>");
            else
                this._$container.find("tr[row-id='none']").remove();
        },
        _removeItem(key) {
            this._config.selectedItems = $.grep(this._config.selectedItems, function (value) { return value.Key !== key; });
            this._$container.find("tr[row-id='" + key + "']").remove();
            this._refreshNoneRow();
        },
        _subscribeToEvents: function () {
            var $this = this;

            this.element.delegate(".kInfoList-add-button", "click", function () {
                var $list = $("<div>");
                var $listContent = $("<div class='elb'>");
                $listContent.exListbox({
                    value: {
                        editorName: "elbKeyInfoEditor",
                        editorId: "elbKeyInfoEditor",
                        isFiltered: true,
                        isRequestedItems: true,
                        urlRequestItems: $this._config.urlRequestItems,
                        items: null
                    }
                });
                $list.append($listContent);
                $list.dialog({
                    modal: true,
                    width: 640,
                    height: 300,
                    title: T("Выбор"),
                    close: function () {
                        $list.dialog("destroy");
                        $list.empty();
                    },
                    buttons: [
                        {
                            text: "Ok",
                            click: function () {
                                var selectedValue = $listContent.exListbox("getValue");
                                if (!selectedValue)
                                    return;
                                $this._setItem(selectedValue);
                                $list.dialog("close");
                            }
                        }, {
                            text: "Отмена",
                            click: function () {
                                $list.dialog("close");
                            }
                        }
                    ]
                });
            });

            this.element.delegate(".kInfoList-remove-button", "click", function () {
                var $element = $(this);
                var $div = $("<div>Удалить запись из списка?</div>");
                $div.dialog({
                    title: "Удаление",
                    buttons: [
                        {
                            text: "Ok",
                            click: function () {
                                $this._removeItem($element.closest("tr").attr("row-id"));
                                $div.dialog("close");
                            }
                        }, {
                            text: "Отмена",
                            click: function () {
                                $div.dialog("close");
                            }
                        }
                    ]
                });
            });

        },
        _disableEvents: function (disable) {
            this._eventsDisabled = disable;
        },
        destroy: function () {
            $.Widget.prototype.destroy.call(this);
        },
        getSelectedItems: function () {
            return this._config.selectedItems;
        }
    });
}(jQuery));

$(document).ready(function ($) {
    $(".kInfoList-element").each(function () {
        var $this = $(this);
        var editorId = $this.attr("editorId");
        $this.keyInfoListEditor({
            editorName: $this.attr("editorName"),
            editorId: editorId,
            selectedItems: $this.attr("selectedItems") === undefined ? undefined : JSON.parse($this.attr("selectedItems")),
            urlRequestItems: $this.attr("urlRequestItems"),
            readonly: $this.attr("readonly")
        });
        $this.closest("form").submit(function () {
            var $form = $(this);
            $form.find("#" + editorId).remove();
            var $input = $("<input name='{0}' id='{0}' type='hidden'>".format(editorId));
            $input.val(JSON.stringify($this.keyInfoListEditor("getSelectedItems")));
            $form.append($input);
        });
    });
});