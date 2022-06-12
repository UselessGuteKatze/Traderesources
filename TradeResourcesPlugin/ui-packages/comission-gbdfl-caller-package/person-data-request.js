$(document).ready(function () {
    var $callButton = $("#btn-fl-iin-caller");
    

    $callButton.click(loadPersonName);
});


function loadPersonName() {
    var iin = $("#flIin").val();
    $.ajax({
        url: rootDir + "references/commission-members/actions?Id=1&MenuAction=get-comission-data&com-member-iin=" + iin,
        dataType: "JSON",
        success: function (result) {
            $("#flFio").val(result.Name);
        }
    });
}