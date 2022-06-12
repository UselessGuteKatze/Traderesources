$(document).ready(function () {
    var $exceptionsField = $("#flExceptions").closest(".form-group");
    $exceptionsField.hide();

    $("#flAreaShown").each(toggleExceptionsField);
    $("#flAreaShown").change(toggleExceptionsField);
});


function toggleExceptionsField() {
    var $exceptionsField = $("#flExceptions").closest(".form-group");
    var $this = $(this);
    var selectedVal = $this.val();
    if (selectedVal === 'HasException') {
        $exceptionsField.show();
    } else {
        $exceptionsField.hide();
    }
}