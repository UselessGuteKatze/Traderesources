$(document).ready(function ($) {
    $(".base-list-editor").each(function () {
        var $this = $(this);
        $this.baseListEditor();

        $this.closest("form").submit(function () {
            $this.find("[name='{0}']".format($this.attr("editor-name"))).remove();
            var editedItems = $this.baseListEditor("getItems");
            var $input = $("<input type='hidden' name='{0}'>".format($this.attr("editor-name")));
            $input.val(JSON.stringify(editedItems));
            $this.append($input);
        });
    });
});


(function ($) {
    $.widget("yodaCommon.baseListEditor", {
        _create: function () {
            var $this = this.element;
            var items = JSON.parse($this.attr("items"));
            var readonly = $this.attr("isReadonly") == "True";
            var objectSchema = JSON.parse($this.attr("object-schema"));
            var editor = getListEditorWidget(objectSchema, {
                readonly: readonly,
                buttons: { addMulti: false },
                editorDialogWidth: 600,
                uiCss: {
                    editBtnClass: "text-list-editor-op-edit",
                    removeBtnClass: "text-list-editor-op-remove"
                },
                callbacks: {
                    onValidate: function (value) {
                        var errors = {};
                        Object.keys(objectSchema).forEach(function (field) {
                            if (IsNullOrEmpty(value[field])) {
                                errors[field] = "Поле обязательно для заполнения";
                            }
                            var type = objectSchema[field].type;
                            if (type == "decimal") {
                                var isNumber = /^\d+\,\d+$/.test(value[field]) || /^\d+$/.test(value[field]);
                                if (!isNumber) {
                                    errors[field] = "Неверный формат поля";
                                }
                            }
                            if (type == "int") {
                                var isNumber = /^\d+$/.test(value[field]);
                                if (!isNumber) {
                                    errors[field] = "Неверный формат поля";
                                }
                            }
                        });
                        //if (IsNullOrEmpty(value.MemberId)) {
                        //    errors.MemberId = "Поле обязательно для заполнения";
                        //}
                        //if (IsNullOrEmpty(value.Role)) {
                        //    errors.Role = "Поле обязательно для заполнения";
                        //}
                        return errors;
                    }
                }
            });
            this._editor = editor;
            $this.append(editor.$widget);
            this.setItems(items);
        },
        getItems: function () {
            var editedItems = this._editor.getItems();
            var editedItemsPrepared = new Array();
            for (var j = 0; j < editedItems.length; j++) {
                editedItemsPrepared.push(editedItems[j]);
            }

            return editedItemsPrepared;
        },
        setItems: function (items) {
            //var itemsPrepared = new Array();
            //for (var i = 0; i < items.length; i++) {
            //    itemsPrepared.push({ MemberId: items[i].MemberId, Role: items[i].Role });
            //}
            //
            //return this._editor.setItems(itemsPrepared);
            return this._editor.setItems(items);
        },
        destroy: function () {
            this.element.empty();
            $.Widget.prototype.destroy.call(this);
        }
    });
}(jQuery));

function IsNullOrEmpty(val) {
    if (val == undefined || val == null || val == "")
        return true;
    return false;
};