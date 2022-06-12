$(document).ready(function () {

    var value = $(".reg-number-mask-input").val();

    if (value != null && value != undefined && value != '' && value != 'SYSTEM-NULL-VALUE') {
        SetRegNumberMaskInputValue(value);
        SetRegNumberMaskInput(value);
    } else {
        $(".reg-number-mask-input").val("Выберите тип и год");
        $(".reg-number-mask-input").prop('disabled', true);
    }

    var typeCodeId = $(".reg-number-mask-input").attr("type-code-id");
    var yearId = $(".reg-number-mask-input").attr("year-id");

    $(`#${typeCodeId}`).change(function () {
        SetRegNumberMaskInput(null);
    });

    $(`#${yearId}`).change(function () {
        SetRegNumberMaskInput(null);
    });

});

function SetRegNumberMaskInput(value) {
    var typeCodeId = $(".reg-number-mask-input").attr("type-code-id");
    var yearId = $(".reg-number-mask-input").attr("year-id");

    var typeCodeVal = $(`#${typeCodeId}`).val();
    var yearVal = $(`#${yearId}`).val();

    if (
        (typeCodeVal != null && typeCodeVal != undefined && typeCodeVal != '' && typeCodeVal != 'SYSTEM-NULL-VALUE') &&
        (yearVal != null && yearVal != undefined && yearVal != '' && yearVal != 'SYSTEM-NULL-VALUE')
    ) {
        typeCodeVal = typeCodeVal.replaceAll("0", ".0");
        yearVal = yearVal.replaceAll("Y", "");

        $.mask.definitions['9'] = '';
        $.mask.definitions['n'] = '[0-9]';
        var hasCurValue = $(".reg-number-mask-input").val().indexOf("KZ") > -1;
        var curValue = $(".reg-number-mask-input").val();
        if (hasCurValue) {
            curValue = curValue.split(".")[3].split("-")[0];
        }
        $(".reg-number-mask-input").mask(`${typeCodeVal}.nnnnn-${yearVal}`);
        $(".reg-number-mask-input").prop("placeholder", `${typeCodeVal}._____-${yearVal}`);
        $(".reg-number-mask-input").prop('disabled', false);
        if (hasCurValue) {
            $(".reg-number-mask-input").val(`${typeCodeVal}.${curValue}-${yearVal}`);
        }

    } else {
        if (value == null) {
            $(".reg-number-mask-input").val("Выберите тип и год");
            $(".reg-number-mask-input").prop('disabled', true);
        } else {
            $(".reg-number-mask-input").val(value);
            $(".reg-number-mask-input").prop('disabled', true);
        }
    }
}

function SetRegNumberMaskInputValue(value) {
    var typeCodeId = $(".reg-number-mask-input").attr("type-code-id");
    var yearId = $(".reg-number-mask-input").attr("year-id");

    var typeCodeVal = $(`#${typeCodeId}`).val();
    var yearVal = $(`#${yearId}`).val();

    if (
        (typeCodeVal != null && typeCodeVal != undefined && typeCodeVal != '' && typeCodeVal != 'SYSTEM-NULL-VALUE') &&
        (yearVal != null && yearVal != undefined && yearVal != '' && yearVal != 'SYSTEM-NULL-VALUE')
    ) {
        typeCodeVal = typeCodeVal.replaceAll("0", ".0");
        yearVal = yearVal.replaceAll("Y", "");

        $(".reg-number-mask-input").val(`${typeCodeVal}.${value}-${yearVal}`);
    } else {
        $(".reg-number-mask-input").val("Ошибка получения значения");
    }
}