$(document).ready(function () {
    var $callButton = $("#btn-xin-caller");
    

    $callButton.click(loadPersonName);
});


function loadPersonName() {
    var xin = $("#flXin").val();
    $.ajax({
        url: rootDir + "traderesources/res-subsoil/res-hydrocarbon/reestr/deprived-persons/get-comission-data/-1/" + xin,
        dataType: "JSON",
        success: function (result) {
            $("#flFio").val(result.Name);
        }
    });
}