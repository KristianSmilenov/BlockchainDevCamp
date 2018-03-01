﻿$(document).ready(function () {
    Date.prototype.toJSON = function () { return this.toISOString(); }

    $('#passwordModal').on('shown.bs.modal', function () {
        $('#myInput').focus()
    })

    $('#passwordModal').on('shown.bs.modal', function () {
        $('#myInput').focus()
    })

    let curve = "secp256k1";
    let sigalg = "SHA256withECDSA";
    const salt = "Some random salt";

    $('#buttonCreateNewWallet').click(createNewWallet);
    $('#buttonOpenWallet').click(openWallet);
    $('#buttonDisplayBalance').click(displayBalance);
    $('#buttonSignTransaction').click(signTransaction);
    $('#buttonSetNodeUrl').click(updateBlockchainNodeUrl);

    //$("#openWalletForm").validator();
    $("#blockchainNodeForm").validator();
    $("#accountBalanceForm").validator();
    $("#sendTransactionForm").validator();

    async function openPasswordPrompt() {
        return new Promise((resolve, reject) => {
            $('#passwordModalOkButton').off();

            $('#passwordModalOkButton').on('click', function () {
                var p1 = $('#passwordModalInput1').val();
                var p2 = $('#passwordModalInput2').val();

                if (p1.length == 0 || p2.length == 0) {
                    alert('Password cannot be empty!');
                    return;
                }

                if (p1 != p2) {
                    alert('Passwords do not match!');
                    return;
                }

                $('#passwordModal').modal('hide');
                resolve(p1);
            });
            $('#passwordModal').modal();
        });
    }

    async function openWalletPrompt() {
        return new Promise((resolve, reject) => {
            $('#openWalletOkButton').off();

            $('#openWalletOkButton').on('click', function () {
                var mnemonic = $('#mnemonicModalInput').val();
                var p1 = $('#passInput1').val();
                var p2 = $('#passInput2').val();

                if (mnemonic.split(' ').length != 24) {
                    alert('You need to enter 24 recovery words for your private key!');
                    return;
                }

                if (p1.length == 0 || p2.length == 0) {
                    alert('Password cannot be empty!');
                    return;
                }

                if (p1 != p2) {
                    alert('Passwords do not match!');
                    return;
                }

                $('#openWalletModal').modal('hide');
                resolve({ mnemonic: mnemonic, password: p1 });
            });
            $('#openWalletModal').modal();
        });
    }

    async function createNewWallet(pass) {
        var pass = await openPasswordPrompt();
        let ec = new elliptic.ec('secp256k1');
        let keyPair = ec.genKeyPair();
        var walletData = saveWalletData(keyPair, pass);

        $("#privateKeyTxt").text("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
        $("#publicKeyTxt").text(walletData.publicKey);
        $("#addressTxt").text(walletData.address);
        updateWalletAddressFields(walletData.address);
    }

    function openWallet(pass) {
        var data = openWalletPrompt();

        //TODO:  restore private key from mnemonic
        let privateKeyInput = $("#existingWalletKey").val();

        let ec = new elliptic.ec(curve);
        let keyPair = ec.keyFromPrivate(privateKeyInput);
        let walletData = saveWalletData(keyPair, data.pass);
        $("#restoredPrivateKeyTxt").text(walletData.privateKey);
        $("#restoredPublicKeyTxt").text(walletData.publicKey);
        $("#restoredAddressTxt").text(walletData.address);
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

    $("#successAlert").hide();
    $("#errorAlert").hide();
    function signTransaction() {
        var validator = $("#sendTransactionForm").data("bs.validator");
        validator.validate();
        if (!validator.hasErrors()) {
            let address = $("#sendTransactionAddress").val();
            let nodeUrl = getBlockchainNodeUrl();
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
                    dateCreated: new Date().toISOString(),
                    senderPubKey: sessionStorage.getItem("publicKey")
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
                        if (data.isValid) {
                            $("#successAlertMessage").text("Transaction send successfully.");
                            $("#successAlert").fadeTo(2000, 500).slideUp(500, function () {
                                $("#successAlert").slideUp(500);
                            });
                        } else {
                            showSignTransactionError(data);
                        }
                    },
                    error: function (data) {
                        showSignTransactionError(data);
                    }
                })
            }
        }
    }

    function showSignTransactionError(data) {
        if (data.errorMessage) {
            $("#errorAlertMessage").text(data.errorMessage);
        }
        $("#errorAlert").fadeTo(2000, 500).slideUp(500, function () {
            $("#errorAlert").slideUp(500);
        });
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

    function saveWalletData(keyPair, pass) {
        encryptPK(pass, keyPair.priv.toArray())
            .then(encryptedHex => {
                sessionStorage.setItem("privateKey", encryptedHex);
                var words = [];
                for (var i in encryptedHex)
                {
                    words.push(mnemonic[i]);
                }
            });

        //TODO: replace this hex with 12 mnemonic words.


        let pubKey = compressPublicKey(keyPair.getPublic());
        sessionStorage.setItem("publicKey", pubKey);

        let ripemd160 = new Hashes.RMD160();
        let walletAddress = ripemd160.hex(pubKey);
        sessionStorage.setItem("address", walletAddress);

        updateWalletAddressFields(walletAddress);

        return { publicKey: pubKey, address: walletAddress, words: []};
    }

    function deriveKey(pass, callback) {
        scrypt(pass, salt, {
            N: 16384,
            r: 8,
            p: 1,
            dkLen: 32,
            encoding: 'hex'
        }, callback);
    }

    async function encryptPK(pass, privateKey) {
        return new Promise((resolve, reject) => {
            deriveKey(pass, (derivedKey) => {
                var key = aesjs.utils.hex.toBytes(derivedKey);
                var aesCtr = new aesjs.ModeOfOperation.ctr(key, new aesjs.Counter(5));
                var encryptedBytes = aesCtr.encrypt(privateKey);
                var encrpytedHex = aesjs.utils.hex.fromBytes(encryptedBytes);
                resolve(encrpytedHex);
            });
        });
    }

    async function decryptPK(pass, encryptedHex) {
        return new Promise((resolve, reject) => {
            deriveKey(pass, (derivedKey) => {
                var key = aesjs.utils.hex.toBytes(derivedKey);
                var encryptedBytes = aesjs.utils.hex.toBytes(encryptedHex);
                var aesCtr = new aesjs.ModeOfOperation.ctr(key, new aesjs.Counter(5));
                var decryptedBytes = aesCtr.decrypt(encryptedBytes);
                var decryptedText = aesjs.utils.hex.fromBytes(decryptedBytes);
                resolve(decryptedText);
            });
        });
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