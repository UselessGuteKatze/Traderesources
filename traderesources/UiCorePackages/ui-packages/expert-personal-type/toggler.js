$(document).ready(function ($) {
	$("#flPersonalType").each(flAgreementToggler);
	$("#flPersonalType").change(flAgreementToggler);
});

function flAgreementToggler() {
	if ($("#flPersonalType").val() === "Внештатный") {
		$("#flAgreementNumber").closest("div").show();
		$("#flAgreementNumber").closest("label").show();
		$('label[for="flAgreementNumber"]').show();

		$("#flAgreementDate").closest("div").show();
		$("#flAgreementDate").closest("label").show();
		$('label[for="flAgreementDate"]').show();
	} else {
		$("#flAgreementNumber").closest("div").hide();
		$("#flAgreementNumber").closest("label").hide();
		$('label[for="flAgreementNumber"]').hide();
		$("#flAgreementNumber").val('');

		$("#flAgreementDate").closest("div").hide();
		$("#flAgreementDate").closest("label").hide();
		$('label[for="flAgreementDate"]').hide();
		$("#flAgreementDate").val('');
	}
}