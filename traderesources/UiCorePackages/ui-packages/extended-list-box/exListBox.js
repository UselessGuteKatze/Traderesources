(function ($) {
    $.widget("yoda.exListbox", {
        _create: function () {
            var value = this.options.value;
            if (!value) {
                value = {
                    editorName: null,
                    editorId: null,
                    isFiltered: false,
                    isRequestedItems: false,
                    urlRequestItems: null
                };
            }
            this._config = {
                editorName: value.editorName,
                editorId: value.editorId,
                isFiltered: value.isFiltered,
                isRequestedItems: value.isRequestedItems,
                urlRequestItems: value.urlRequestItems
            }
            this._initMarkup();
            this._subscribeToEvents();
            if (this._config.isRequestedItems) {
                this._requestItems("%");
            } else {
                this._items = new Array();
                this._setItems(value.items);
            }
            this._value = undefined;
            //this.setValue(value.selectedValue);
        },
        _initMarkup: function () {
            var $this = this.element;
            var $markup = $(
                "<table id='" + this._config.editorId + "' name='" + this._config.editorName + "' class='exListbox-container'>" +
                    "<tr>" +
                        "<td id='exListbox-items' class='exListbox-items'></td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td id='exListbox-filter' class='exListbox-filter' " + (this._config.isFiltered ? "" : "style='display: none;'") + ">" +
                            "<input type='text' id='exListbox-filter-box'>" +
                        "</td>" +
                    "</tr>" +
                "</table>"
            );

            $this.append($markup);
            this._$markup = $markup;
        },
        clear: function () {
            this._$markup.find("#exListbox-items").empty();
        },
        setValue: function (value) {
            this._disableEvents(true);
            if (value === undefined) {
                this._value = value;
                return;
            }
            for (var i = 0; i < this._items.length; i++)
                if (this._items[i].Key === value) {
                    this._value = this._items[i];
                    break;
                }
            this._$markup.find(".exListbox-item").removeClass("exListbox-item-selected");
            this._$markup.find(".exListbox-item[item-code='" + this._value.Key + "']").addClass("exListbox-item-selected");
            this._disableEvents(false);
        },
        getValue: function () {
            return this._value;
        },
        _requestItems: function () {
            var $this = this;
            $.ajax({
                url: $this._config.urlRequestItems,
                dataType: "JSON",
                success: function (data) {
                    $this._setItems(JSON.parse(data));
                },
                error: function (xhr, b, c) {
                    alert("Не удалось загрузить данные: " + xhr.responseText);
                }
            });
        },
        _setItems: function (items) {
            var $this = this;
            if (!items)
                return;
            $this.clear();

            $this._items = items;
            $this._setFilteredItems();
        },
        _renderItems: function(items) {
            var $this = this;
            $this._$markup.find("#exListbox-items").empty();
            if (!items)
                return;
            for (var i = 0; i < items.length; i++)
                $this._$markup.find("#exListbox-items").append("<span class='exListbox-item" + (items[i].Key === $this.getValue() ? " exListbox-item-selected" : "") + "' item-code='" + items[i].Key + "'>" + items[i].Text + "</span>");
            $this._$markup.find(".exListbox-container").scrollTop();
        },
        _setFilteredItems: function (filter) {
            var $this = this;
            var filteredItems = [];
            if (filter === undefined || filter === null)
                filteredItems = $this._items;
            else 
                for (var i = 0; i < $this._items.length; i++)
                    if ($this._items[i].Text.toLowerCase().indexOf(filter.toLowerCase()) > -1)
                        filteredItems.push($this._items[i]);
            $this._renderItems(filteredItems);
        },
        _subscribeToEvents: function () {
            var $this = this;
            var $element = this.element;
            $element.delegate(".exListbox-item", "click", function () {
                $this.setValue($(this).attr("item-code"));
            });
            $element.delegate("#exListbox-filter-box", "keyup", function () {
                if (!$this._config.isFiltered)
                    return;
                $this.setValue(undefined);
                if ($this.isRequestedItems)
                    $this._requestItems(($element.find("#exListbox-filter-box").val() === "" ? "%" : $element.find("#exListbox-filter-box").val()));
                else
                    $this._setFilteredItems(($element.find("#exListbox-filter-box").val() === "" ? undefined : $element.find("#exListbox-filter-box").val()));
            });
        },
        _disableEvents: function (disable) {
            this._eventsDisabled = disable;
        },
        destroy: function () {
            $.Widget.prototype.destroy.call(this);
        }
    });
}(jQuery));

//$(document).ready(function ($) {
//    $(".exListbox").each(function () {
//        var $this = $(this);
//        var editorId = $this.attr("editorId");
//        $this.exListbox({
//            editorName: $this.attr("editorName"),
//            editorId: editorId,
//            isFiltered: $this.attr("isFiltered") === "True",
//            isRequestedItems: $this.attr("isRequestedItems") === "True",
//            urlRequestItems: $this.attr("isRequestedItems") === "True" ? $this.attr("urlRequestItems"): null,
//            items: $this.attr("isRequestedItems") === "True" ? null : JSON.parse($this.attr("items"))
//            //selectedValue: $this.attr("selectedValue")
//        });
        
//        $this.closest("form").submit(function () {
//            var $form = $(this);
//            $form.find("#" + editorId).remove();
//            var $input = $("<input name='{0}' id='{0}' type='hidden'>".format(editorId));
//            $input.val(JSON.stringify($this.exListbox("getValue")));
//            $form.append($input);
//        });
//    });
//});