$(document).ready(function () {
    $(".activity-types-editor").each(function () {
        var $editor = $(this);
        var editorName = $editor.data("editorName");
        var items = $editor.data("value");
        var isReadonly = $editor.data("readonly") == "True";
        var customTitle = $editor.data("customTitle");
        if (!customTitle) {
            customTitle = "Тип субъекта";
        }
        function getMarkupForItem(activityTypeItem, depthLevel) {
            var $ret = $("<tr class='level-{0}'><td class='caption'><span>{1}</span></td><td class='from'></td><td class='to'></td></tr>".format(depthLevel, activityTypeItem.Text));
            var $from = $("<input class='activity-type-date-val activity-item-from-val' data-activity-type-name='{0}'></input>".format(activityTypeItem.Value));
            var $to = $("<input class='activity-type-date-val activity-item-to-val' data-activity-type-name='{0}'></input>".format(activityTypeItem.Value));
            $ret.find(".from").append($from);
            $ret.find(".to").append($to);

            if (isReadonly == true) {
                $from.prop("readonly", true);
                $to.prop("readonly", true);
            }

            syncActivityItemMarkup($ret, activityTypeItem);
            return $ret;
        }

        function syncActivityItemMarkup($itemMarkup, activityTypeItem) {
            $itemMarkup.data("activityTypeItem", activityTypeItem);
            $itemMarkup.data("val", activityTypeItem.Value);
            var $from = $itemMarkup.find(".activity-item-from-val");
            var $to = $itemMarkup.find(".activity-item-to-val");

            $from.val(activityTypeItem.From.RawValue);
            $to.val(activityTypeItem.To.RawValue);

            if (activityTypeItem.From.Error) {
                $from.addClass("input-validation-error");
                $from.after("<span class='error-text'>{0}</span>".format(activityTypeItem.From.Error));
            }
            if (activityTypeItem.To.Error) {
                $to.addClass("input-validation-error");
                $to.after("<span class='error-text'>{0}</span>".format(activityTypeItem.To.Error));
            }
        }

        function printItems(activityItems, $container, depthLevel) {
            for (var i = 0; i < activityItems.length; i++) {
                var item = activityItems[i];
                var $itemMarkup = getMarkupForItem(item, depthLevel);
                $container.append($itemMarkup);

                if (item.Items && item.Items.length > 0) {
                    $itemMarkup.find("input").prop("readonly", true);
                    $itemMarkup.addClass("disabled");
                    printItems(item.Items, $container, depthLevel + 1);
                }
            }
        }


        var $mainContainer = $("<table class='activity-items'><thead><tr><th>{0}</th><th>{1}</th><th>{2}</th></tr></thead><tbody></tbody></table>".format(customTitle, "с", "по"));
        printItems(items, $mainContainer.find("tbody"), 0);

        if (isReadonly == false) {
            $mainContainer.find(".activity-type-date-val:not([readonly])").datepicker({ dateFormat: 'dd.mm.yy', changeYear: true, changeMonth: true, yearRange: '1991:2030' });
        }
        $editor.append($mainContainer);
        var $form = $editor.closest("form");
        $form.submit(function () {
            var ret = [];
            $mainContainer.find("tbody tr").each(function () {
                var $item = $(this);
                var val = $item.data("val");
                var from = $item.find(".from input").val();
                var to = $item.find(".to input").val();

                ret.push({
                    Value: val,
                    From: from,
                    To: to
                });
            });

            $form.find("input[name='{0}']".format(editorName)).remove();

            var $input = $("<input type='hidden' name='{0}'/>".format(editorName));
            $input.val(JSON.stringify(ret));
            $form.append($input);
        });

    });


});