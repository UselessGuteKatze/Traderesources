$(function () {
    $(".multiselect").each(function () {
        var $this = $(this);
        var fieldText = $this.data("nonSelectedText");
        $this.multiselect({
            nonSelectedText: fieldText,
            selectAllText: "Выбрать всё",
            maxHeight: 300,
            includeSelectAllOption: $this.data("includeSelectAllOption"),
            enableClickableOptGroups: true,
            enableCollapsibleOptGroups: true,
            collapseOptGroupsByDefault: true,
            enableFiltering: $this.data("enableFiltering"),
            enableCaseInsensitiveFiltering: $this.data("enableFiltering"),
            filterPlaceholder: $this.data("filterPlaceholder"),
            buttonClass: 'form-control',
            buttonContainer: '<div></div>',
            buttonText: function (options, select) {
                if (options.length === 0) {
                    return fieldText;
                }
                else if (options.length > 3) {
                    return [fieldText, ': ', 'Выбрано (', options.length, ')'].join('');
                }
                else {
                    var labels = [];
                    options.each(function () {
                        if ($(this).attr('label') !== undefined) {
                            labels.push($(this).attr('label'));
                        }
                        else {
                            labels.push($(this).html());
                        }
                    });
                    return fieldText + ': ' + labels.join(', ') + '';
                }
            }
        });
    });
});