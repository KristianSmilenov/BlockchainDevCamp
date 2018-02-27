$(document).ready(function () {
    Date.prototype.toJSON = function () { return this.toISOString(); }

    let curve = "secp256k1";
    let sigalg = "SHA256withECDSA";

    $('#buttonCreateNewWallet').click(createNewWallet);
    $('#buttonOpenWallet').click(openWallet);
    $('#buttonDisplayBalance').click(displayBalance);
    $('#buttonSignTransaction').click(signTransaction);
    $('#buttonSetNodeUrl').click(updateBlockchainNodeUrl);

    $("#openWalletForm").validator();
    $("#blockchainNodeForm").validator();
    $("#accountBalanceForm").validator();
    $("#sendTransactionForm").validator();

    function createNewWallet() {
        let ec = new elliptic.ec('secp256k1');
        let keyPair = ec.genKeyPair();
        let walletData = saveWalletData(keyPair);
        sessionStorage.setItem("privateKey", walletData.privateKey);
        sessionStorage.setItem("publicKey", walletData.publicKey);
        sessionStorage.setItem("address", walletData.address);
        $("#privateKeyTxt").text(walletData.privateKey);
        $("#publicKeyTxt").text(walletData.publicKey);
        $("#addressTxt").text(walletData.address);
        updateWalletAddressFields(walletData.address);

        //let publicKeyCompressed = compressPublicKey(keyPair.getPublic());
    }

    function openWallet() {
        var validator = $("#openWalletForm").data("bs.validator");
        validator.validate();
        if (!validator.hasErrors()) {
            let privateKeyInput = $("#existingWalletKey").val();
            let ec = new elliptic.ec(curve);
            let keyPair = ec.keyFromPrivate(privateKeyInput);
            let walletData = saveWalletData(keyPair);
            $("#restoredPrivateKeyTxt").text(walletData.privateKey);
            $("#restoredPublicKeyTxt").text(walletData.publicKey);
            $("#restoredAddressTxt").text(walletData.address);
        }
    }

    function updateBlockchainNodeUrl() {
        var validator = $("#blockchainNodeForm").data("bs.validator");
        validator.validate();
        if (!validator.hasErrors()) {
            $("#walletConfiguration").collapse('hide');
        }
    }

    function displayBalance() {
        var validator = $("#accountBalanceForm").data("bs.validator");
        validator.validate();
        if (!validator.hasErrors()) {
            let address = $("#accountBalanceAddress").val();
            let nodeUrl = getBlockchainNodeUrl();
            let balanceConfirmations = $("#accountBalanceConfirmations").val();

            var requestUrl = nodeUrl + '/balance/' + address + '/' + balanceConfirmations;
            $.get(requestUrl, function (addressData) {
                var confirmed = `${addressData.confirmedBalance.balance} coins confirmed with ${addressData.confirmedBalance.confirmations} confirmations`;
                var lastMined = `${addressData.lastMinedBalance.balance} coins last mined with ${addressData.lastMinedBalance.confirmations} confirmations`;
                var pending = `${addressData.pendingBalance.balance} coins pending ${addressData.pendingBalance.confirmations} confirmations`;

                $("#balanceConfirmed").text(confirmed);
                $("#balanceLastMined").text(lastMined);
                $("#balancePending").text(pending);
            });
        }
    }

    function signTransaction() {
        var validator = $("#sendTransactionForm").data("bs.validator");
        validator.validate();
        if (!validator.hasErrors()) {
            let address = $("#sendTransactionAddress").val();
            let nodeUrl = getBlockchainNodeUrl(); //$("#transactionNodeUrl").val();
            let recipient = $("#transactionRecipient").val();
            let value = $('#transactionValue').val();

            //let privateKey = window.prompt("To sign a transaction you have to write down your private key:")
            let privateKey = sessionStorage.getItem("privateKey");
            if (privateKey) {

                let ec = new elliptic.ec('secp256k1');
                let keyPair = ec.keyFromPrivate(privateKey);

                let dataToSign = {
                    from: address,
                    to: recipient,
                    value: parseInt(value),
                    fee: 2,
                    senderPubKey: sessionStorage.getItem("publicKey"),
                    dateCreated: new Date()
                }

                let sha256 = new Hashes.SHA256();
                let dataHex = sha256.hex(JSON.stringify(dataToSign));
                var signature = ec.sign(dataHex, privateKey);
                console.log(signature);
                keyPair.verify(dataHex, signature)
                var signatureHex = toHexString(signature.toDER());
                console.log(signatureHex);
                dataToSign.senderSignature = signatureHex;

                $.ajax({
                    url: nodeUrl + '/transactions',
                    method: 'post',
                    dataType: 'json',
                    data: JSON.stringify(dataToSign),
                    contentType: "application/json",
                    success: function (data) {
                        alert(data.isValid);
                    },
                    error: function (err) {
                        alert(JSON.parse(err));
                    }
                })
            }
        }
    }

    function toHexString(byteArray) {
        return Array.from(byteArray, function (byte) {
            return ('0' + (byte & 0xFF).toString(16)).slice(-2);
        }).join('')
    }

    function compressPublicKey(publicKey) {
        var prefix = publicKey.getY().isOdd() ? "03" : "02";
        var x = publicKey.getX().toString(16);

        return prefix + x;
    }

    function saveWalletData(keyPair) {
        let privateKey = keyPair.getPrivate().toString(16);
        sessionStorage.setItem("privateKey", privateKey);

        let pubKey = compressPublicKey(keyPair.getPublic());
        sessionStorage.setItem("publicKey", pubKey);

        let ripemd160 = new Hashes.RMD160();
        let walletAddress = ripemd160.hex(pubKey);
        sessionStorage.setItem("address", walletAddress);

        updateWalletAddressFields(walletAddress);

        return { privateKey: privateKey, publicKey: pubKey, address: walletAddress };
    }

    function getBlockchainNodeUrl() {
        return $("#blockchainNodeUrl").val() + '/api';
    }

    function updateWalletAddressFields(address) {
        $("#sendTransactionAddress").val(address);
        $("#accountBalanceAddress").val(address);
    }

    $('#buttonCreateWalletView').click(showCreateWalletView);
    function showCreateWalletView() {
        showView("createWalletSection");
        $(this).parent().addClass("active");
    }

    $('#buttonOpenWalletView').click(showExistingWalletView);
    function showExistingWalletView() {
        showView("openWalletSection");
        $(this).parent().addClass("active");
    }

    $('#buttonAccountBalanceView').click(showAccountView);
    function showAccountView() {
        showView("viewAccountSection");
        $(this).parent().addClass("active");
    }

    $('#buttonSendTransactionView').click(showSendTransactionView);
    function showSendTransactionView() {
        showView("sendTransactionSection");
        $(this).parent().addClass("active");
    }

    function showView(viewName) {
        activeView = viewName;
        $('li.nav-item').removeClass("active");
        $('section').hide();
        $('#' + viewName).show();
    }
    $('#buttonCreateWalletView').click();
});