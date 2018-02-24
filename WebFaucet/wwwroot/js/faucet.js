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
    function createFaucetWallet() {
        let ec = new KJUR.crypto.ECDSA({ "curve": curve });
        let keypair = ec.generateKeyPairHex();
        let ripemd160 = new Hashes.RMD160();
        let walletAddress = ripemd160.hex(keypair.ecpubhex);

        sessionStorage.setItem("privateKey", keypair.ecprvhex);
        sessionStorage.setItem("publicKey", keypair.ecpubhex);
        sessionStorage.setItem("address", walletAddress);

        $("#faucetAddress").text(walletAddress);
    }
    createFaucetWallet();

    function buttonSendCoins() {
        let faucetAddress = $("#faucetAddress").text();
        let recipient = $("#recipientAddress").val();
        let nodeUrl = $("#blockchainNodeUrl").val();
        let value = parseInt($("#requestedCoins").val());

        privateKey = sessionStorage.getItem("privateKey");
        if (privateKey) {
            let dateCreated = new Date();
            let dataToSign = {
                from: faucetAddress,
                to: recipient,
                value: parseInt(value),
                fee: 2,
                senderPubKey: sessionStorage.getItem("publicKey"),
                dateCreated: dateCreated
            }
            let dataHex = JSON.stringify(dataToSign);
            
            // Sign option 2
            var sig = new KJUR.crypto.Signature({ "alg": sigalg });
            sig.init({ d: privateKey, curve: curve });
            sig.updateString(dataHex);
            var signature = sig.sign();
            console.log(signature);
            dataToSign.senderSignature = signature;

            $.ajax({
                url: nodeUrl + '/transactions',
                method: 'post',
                dataType: 'json',
                data: JSON.stringify(dataToSign),
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
            })
        }
    }

    function showView(viewName) {
        // Hide all views and show the selected view only
        $('section').hide();
        $('#' + viewName).show();
    }
    showView("sendCoins");
});