$(document).ready(function ($) {
	$("#flResident").each(flResidentToggler);
	$("#flResident").change(flResidentToggler);

	$("#flIin").closest("div").hide();
	$("#flIin").closest("label").hide();
	$('label[for="flIin"]').hide();

	$("#flPerson").closest("div").hide();
	$("#flPerson").closest("label").hide();
	$('label[for="flPerson"]').hide();
	$("#flPerson").val('');
});

function flResidentToggler() {
	if ($("#flResident").val() === "НеРезидент") {
		$("#flSurname").closest("div").show();
		$("#flSurname").closest("label").show();
		$('label[for="flSurname"]').show();

		$("#flFirstName").closest("div").show();
		$("#flFirstName").closest("label").show();
		$('label[for="flFirstName"]').show();

		$("#flSecondName").closest("div").show();
		$("#flSecondName").closest("label").show();
		$('label[for="flSecondName"]').show();

		$("#flPerson").closest("div").hide();
		$("#flPerson").closest("label").hide();
		$('label[for="flPerson"]').hide();
		$("#flPerson").val('');
	} else {
		$("#flSurname").closest("div").hide();
		$("#flSurname").closest("label").hide();
		$('label[for="flSurname"]').hide();
		$("#flSurname").val('');

		$("#flFirstName").closest("div").hide();
		$("#flFirstName").closest("label").hide();
		$('label[for="flFirstName"]').hide();
		$("#flFirstName").val('');

		$("#flSecondName").closest("div").hide();
		$("#flSecondName").closest("label").hide();
		$('label[for="flSecondName"]').hide();
		$("#flSecondName").val('');

		$("#flPerson").closest("div").show();
		$("#flPerson").closest("label").show();
		$('label[for="flPerson"]').show();
	}
}