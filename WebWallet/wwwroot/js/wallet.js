﻿$(document).ready(function () {
    let curve = "secp256k1";
    let sigalg = "SHA256withECDSA";
    
    $('#buttonCreateNewWallet').click(createNewWallet);
    $('#buttonOpenWallet').click(openWallet);
    $('#buttonDisplayBalance').click(displayBalance);
    $('#buttonSignTransaction').click(signTransaction);

    function createNewWallet() {
        //let ec = new elliptic.ec('secp256k1');
        //let keyPair = ec.genKeyPair();
        //saveKeys(keyPair);

        let ec = new KJUR.crypto.ECDSA({ "curve": curve });
        let keypair = ec.generateKeyPairHex();
        let ripemd160 = new Hashes.RMD160();
        let walletAddress = ripemd160.hex(keypair.ecpubhex);

        sessionStorage.setItem("privateKey", keypair.ecprvhex);
        sessionStorage.setItem("publicKey", keypair.ecpubhex);
        sessionStorage.setItem("address", walletAddress);

        $("#privateKeyTxt").text(keypair.ecprvhex);
        $("#publicKeyTxt").text(keypair.ecpubhex);
        $("#addressTxt").text(walletAddress);
        updateWalletAddressFields(walletAddress);
    }

    function openWallet() {
        let privateKeyInput = $("#existingWalletKey").val();
        let ec = new elliptic.ec(curve);
        let keyPair = ec.keyFromPrivate(privateKeyInput);
        let walletData = saveWalletData(keyPair);
        $("#restoredPrivateKeyTxt").text(walletData.privateKey);
        $("#restoredPublicKeyTxt").text(walletData.publicKey);
        $("#restoredAddressTxt").text(walletData.address);
    }

    function displayBalance() {
        let address = $("#accountBalanceAddress").val();
        let nodeUrl = $("#blockchainNodeUrl").val();

        //TODO: call node endpoint

        $("#balanceConfirmed").text("0");
        $("#balance1Confirmation").text("0");
        $("#balancePending").text("0");
    }

    function signTransaction() {
        
        let address = $("#accountBalanceAddress").text();
        let nodeUrl = $("#transactionNodeUrl").val();
        let recipient = $("#transactionRecipient").val();
        let value = $('#transactionValue').val();

        //let privateKey = window.prompt("To sign a transaction you have to write down your private key:")

        privateKey = sessionStorage.getItem("privateKey");
        if (privateKey) {
            debugger;
            let ec = new elliptic.ec('secp256k1');
            let keyPair = ec.keyFromPrivate(privateKey);
            let publicKey = keyPair.getPublic().getX().toString(16) + (keyPair.getPublic().getY().isOdd() ? "1" : "0");

            let senderPubKey = publicKey
            let dateCreated = new Date().toString()

            let dataToSign = {
                from: address,
                to: recipient,
                value: value,
                fee: 2,
                senderPubKey: senderPubKey,
                dateCreated: dateCreated
            }
            let dataHex = toHex(JSON.stringify(dataToSign));

            // Sign option 1
            var signature = ec.sign(dataHex, privateKey);
            console.log(signature);
            var signatureHex = toHexString(signature.toDER());
            console.log(signatureHex);

            // Sign option 2
            var sig = new KJUR.crypto.Signature({ "alg": sigalg });
            sig.init({ d: privateKey, curve: curve });
            sig.updateString(dataHex);
            var signature2 = sig.sign();
            console.log(signature2);

            dataToSign.senderSignature = signature2;

            jsonRequest = JSON.stringify(dataToSign);

            $.ajax({
                url: nodeUrl + '/transactions/new',
                method: 'post',
                dataType: 'json',
                data: jsonRequest,
                contentType: "application/json",
                success: function (data) {
                    alert(JSON.parse(data));
                },
                error: function (err) {
                    alert(JSON.parse(err));
                }
            })
            //{
            //  "senderSignature": "string",
            //  "senderMessage": "string",
            //  "from": "string",
            //  "to": "string",
            //  "senderPubKey": "string",
            //  "value": 0,
            //  "fee": 0,
            //  "dateCreated": "2018-02-22T23:14:15.488Z"
            //}
        }
    }

    function toHexString(byteArray) {
        return Array.from(byteArray, function (byte) {
            return ('0' + (byte & 0xFF).toString(16)).slice(-2);
        }).join('')
    }

    function toHex(str) {
        var hex = '';
        for (var i = 0; i < str.length; i++) {
            hex += '' + str.charCodeAt(i).toString(16);
        }
        return hex;
    }

    function doSign() {
        debugger;
        var f1 = document.form1;
        var prvkey = f1.prvkey1.value;
        //var curve = "secp256k1";
        //var sigalg = "SHA256withECDSA";
        var msg1 = f1.msg1.value;

        var sig = new KJUR.crypto.Signature({ "alg": sigalg });
        sig.init({ d: prvkey, curve: curve });
        sig.updateString(msg1);
        var sigValueHex = sig.sign();

        f1.sigval1.value = sigValueHex;
    }

    function doVerify() {
        debugger;
        var f1 = document.form1;
        var pubkey = f1.pubkey1.value;
        var curve = "secp256k1"; //f1.curve1.value;
        var sigalg = "SHA256withECDSA"; //f1.sigalg1.value;
        var msg1 = f1.msg1.value;
        var sigval = f1.sigval1.value;

        var sig = new KJUR.crypto.Signature({ "alg": sigalg, "prov": "cryptojs/jsrsa" });
        sig.init({ xy: pubkey, curve: curve });
        sig.updateString(msg1);
        var result = sig.verify(sigval);
        if (result) {
            alert("valid ECDSA signature");
        } else {
            alert("invalid ECDSA signature");
        }
    }

    function saveWalletData(keyPair) {
        let privateKey = keyPair.getPrivate().toString(16);
        sessionStorage.setItem("privateKey", privateKey);

        let pubKey = keyPair.getPublic().getX().toString(16) +
            (keyPair.getPublic().getY().isOdd() ? "1" : "0");
        sessionStorage.setItem("publicKey", pubKey);

        let ripemd160 = new Hashes.RMD160();
        let walletAddress = ripemd160.hex(pubKey);
        sessionStorage.setItem("address", walletAddress);

        updateWalletAddressFields(walletAddress);

        return { privateKey: privateKey, publicKey: pubKey, address: walletAddress };
    }

    function updateWalletAddressFields(address) {
        $("#sendTransactionAddress").text(address);
        $("#accountBalanceAddress").text(address);
    }
});