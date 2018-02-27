var verifyCallback = function (data) {
    if (data != "") {
        $("#recaptchaError").text("");
    }
}

var onloadCallback = function () {
    grecaptcha.render('recaptcha', {
        'sitekey': '6LeVZ0kUAAAAADk5pLtf6IAv72plFVAnvD-ZxcAB',
        'callback': verifyCallback
    });
};

$(document).ready(function () {
    Date.prototype.toJSON = function () { return this.toISOString(); }

    $("#blockchainNodeForm").validator();
    $("#sendTransactionForm").validator();
    $('#buttonSetNodeUrl').click(updateBlockchainNodeUrl);
    $('#buttonSendCoins').click(sendCoins);

    function updateBlockchainNodeUrl() {
        var validator = $("#blockchainNodeForm").data("bs.validator");
        validator.validate();
        if (!validator.hasErrors()) {
            $("#faucetConfiguration").collapse('hide');
        }
    }

    function getNodeUrl() {
        return $("#blockchainNodeUrl").val();
    }

    $(document).on({
        ajaxStart: function () { $("#loadingBox").show() },
        ajaxStop: function () { $("#loadingBox").hide() }
    });

    function validate(event) {
        event.preventDefault();
        if (!document.getElementById('field').value) {
            alert("You must add text to the required field");
        } else {
            grecaptcha.execute();
        }
    }

    function sendCoins() {
        var validator = $("#sendTransactionForm").data("bs.validator");
        validator.validate();
        if (!validator.hasErrors()) {
            $("#recaptchaError").text("");
            var captcha = grecaptcha.getResponse();
            if (!captcha) {
                $("#recaptchaError").text("Please validate captcha");
                return;
            }
            let value = parseInt($("#transactionValue").val());
            let recipient = $("#transactionRecipient").val();
            let nodeUrl = getNodeUrl();
            $.ajax({
                url: 'api/transactions',
                method: 'post',
                dataType: 'json',
                data: JSON.stringify({
                    nodeUrl: nodeUrl,
                    recipientAddress: recipient,
                    value: value
                }),
                contentType: "application/json",
                success: function (data) {
                    if (!data.isValid || data.errorMessage || !data.transactionHash) {
                        showView("coinsNotSent");
                        $("#errorMessage").text(data.errorMessage);
                    }
                    else {
                        showView("coinsSend");
                        $("#numerOfCoins").text(value);
                        $("#transactionHash").text(data.transactionHash);
                    }
                    $("#addressSent").text(recipient);
                },
                error: function (err) {
                    alert(JSON.parse(err));
                }
            });
        }
    }

    function showView(viewName) {
        // Hide all views and show the selected view only
        $('section').hide();
        $('#' + viewName).show();
    }
    showView("sendCoins");

    $('#buttonBack').click(function () {
        showView("sendCoins");
    });

    $('#buttonBackError').click(function () {
        showView("sendCoins");
    });
});