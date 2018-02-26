$(document).ready(function () {
    Date.prototype.toJSON = function () { return this.toISOString(); }

    let curve = "secp256k1";
    let sigalg = "SHA256withECDSA";
    
    $('#buttonCreateNewWallet').click(createNewWallet);
    $('#buttonOpenWallet').click(openWallet);
    $('#buttonDisplayBalance').click(displayBalance);
    $('#buttonSignTransaction').click(signTransaction);

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

    function updateWalletAddressFields(address) {
        $("#sendTransactionAddress").text(address);
        $("#accountBalanceAddress").text(address);
    }
});