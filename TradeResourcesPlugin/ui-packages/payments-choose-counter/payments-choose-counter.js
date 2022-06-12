$(document).ready(function ($) {
    $(".check-boxes-box").each(function () {
        var $this = $(this);

        $this.click(function () {

            var sum = 0;

            $this.find(".check-boxes-box-item").each(function (index, element) {
                    var $element = $(element);
                    if ($element.is(":checked")) {
                        sum += +$element.next().find(".payment-item").attr("amount").replace(/,/g, ".");
                    }
            });

            var value = $this.find(".check-boxes-box-head").html();

            var array = value.split("-");

            var amountValues = Array.from(array[1].split(" / "), item => +(item.replace("тг.", "").replace(/&nbsp;/g, "").replace(/\s/g, "").replace(/,/g, ".")) );
            amountValues[2] = sum;
            amountValues[3] = amountValues[1] - sum;
            if (amountValues[3] < 0) {
                amountValues[3] = 0;
            }

            amountValues = Array.from(amountValues, amountValue => amountValue.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$& '));

            array[1] = ` ${amountValues.join(" / ").replace(/\./g, ",")} тг.`;

            $this.find(".check-boxes-box-head").html(array.join("-"))

            console.log(sum);

        });

        $this.click();

    });
});