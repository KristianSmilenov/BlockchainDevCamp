$(document).ready(function () {
    Date.prototype.toJSON = function () { return this.toISOString(); }

    $('#buttonBack').click(function () {
        showView("sendCoins");
    });

    let curve = "secp256k1";
    let sigalg = "SHA256withECDSA";
    
    $('#buttonSendCoins').click(buttonSendCoins);
    $(document).on({
        ajaxStart: function () { $("#loadingBox").show() },
        ajaxStop: function () { $("#loadingBox").hide() }
    });

    function buttonSendCoins() {
        let recipient = $("#recipientAddress").val();
        let nodeUrl = $("#blockchainNodeUrl").val();
        let value = parseInt($("#requestedCoins").val());

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
                showView("coinsSend");

                $("#numerOfCoins").text(value);
                $("#addressSent").text(recipient);
                $("#transactionHash").text(data.transactionHash);
            },
            error: function (err) {
                alert(JSON.parse(err));
            }
        });
    }

    function showView(viewName) {
        // Hide all views and show the selected view only
        $('section').hide();
        $('#' + viewName).show();
    }
    showView("sendCoins");
});