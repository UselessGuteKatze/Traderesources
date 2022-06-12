$(document).ready(function ($) {
    $(".check-boxes-box").each(function () {
        var $this = $(this);

        $this.closest("form").submit(function () {
            var checkeds = [];
            $this.find(".check-boxes-box-item").each(function (index, element) {
                var $element = $(element);
                if ($element.is(":checked")) {
                    checkeds.push($element.attr("id"));
                }
            });
            $this.find(".check-boxes-box-input").val(JSON.stringify(checkeds));
        });
    });
});