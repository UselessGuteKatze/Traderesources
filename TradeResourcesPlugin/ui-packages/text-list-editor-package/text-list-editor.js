$(document).ready(function ($) {
    $(".text-list-editor").each(function () {
        var $this = $(this);
        var textItems = JSON.parse($this.attr("text-items"));
        var textItemsReadonly = JSON.parse($this.attr("readonly-items-list"));
        var readonly = $this.attr("isReadonly") == "True";
        var captionText = $this.attr("captionText");
        if (captionText == "" || captionText == undefined) {
            captionText = "Текст условия";
        }
        var objectSchema = {
            Id: { type: "text", text: "Id", hidden: true },
            Text: { type: "text", text: captionText }
        };

        for (var i = 0; i < textItems.length; i++) {
            for (var j = 0; j < textItemsReadonly.length; j++) {
                if (textItems[i].Id === textItemsReadonly[j]) {
                    textItems[i].Readonly = true;
                }
            }
        }

        $this.listEditor({
            schema: objectSchema,
            items: textItems,
            readonly: readonly,
            editorDialogWidth: 600,
            uiCss: {
                editBtnClass: "text-list-editor-op-edit",
                removeBtnClass: "text-list-editor-op-remove"
            },
            callbacks: {
                onActionCellPrinted: function (value, $actionTd) {
                    if (value.Readonly == true) {
                        $actionTd.empty();
                    }
                }
            }
        });

        $this.closest("form").submit(function () {
            var items = $this.listEditor("getItems");
            var $input = $("<input type='hidden' name='{0}'>".format($this.attr("editor-name")));
            $input.val(JSON.stringify(items));
            $this.append($input);
        });
    });

    $(".commission-list-editor").each(function () {
        var $this = $(this);
        $this.commissionListEditor();

        $this.closest("form").submit(function () {
            $this.find("[name='{0}']".format($this.attr("editor-name"))).remove();
            var editedItems = $this.commissionListEditor("getItems");
            var $input = $("<input type='hidden' name='{0}'>".format($this.attr("editor-name")));
            $input.val(JSON.stringify(editedItems));
            $this.append($input);
        });
    });
});


(function ($) {
    $.widget("yodaCommon.commissionListEditor", {
        _create: function () {
            var $this = this.element;
            var items = JSON.parse($this.attr("items"));
            var commissionItems = JSON.parse($this.attr("reference"));
            var refRoles = JSON.parse($this.attr("ref-commission-roles"));
            var readonly = $this.attr("isReadonly") == "True";
            var objectSchema = {
                MemberId: { type: "reference", text: "Члены комиссии", reference: commissionItems },
                Role: { type: "reference", text: "Роль в комиссии", reference: refRoles }
            };
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
                        if (IsNullOrEmpty(value.MemberId)) {
                            errors.MemberId = "Поле обязательно для заполнения";
                        }
                        if (IsNullOrEmpty(value.Role)) {
                            errors.Role = "Поле обязательно для заполнения";
                        }
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
            var itemsPrepared = new Array();
            for (var i = 0; i < items.length; i++) {
                itemsPrepared.push({ MemberId: items[i].MemberId, Role: items[i].Role });
            }

            return this._editor.setItems(itemsPrepared);
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