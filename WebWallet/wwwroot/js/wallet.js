const curve = "secp256k1";
const sigalg = "SHA256withECDSA";
const salt = "Some random salt";

async function openPasswordPrompt() {
    return new Promise((resolve, reject) => {
        $('#passwordModalOkButton').off();

        $('#passwordModalOkButton').on('click', function () {
            var p1 = $('#passwordModalInput1').val();
            var p2 = $('#passwordModalInput2').val();

            if (p1.length == 0 || p2.length == 0) {
                return;
            }

            if (p1 != p2) {
                $("#passwordsDoNotMatchLabel").fadeTo(2000, 500).slideUp(500, function () {
                    $("#passwordsDoNotMatchLabel").slideUp(500);
                });
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
            var pass = $('#passInput').val();;

            $('#openWalletModal').modal('hide');
            resolve(pass);
        });
        $('#openWalletModal').modal();
    });
}

async function createNewWallet(pass) {
    var pass = await openPasswordPrompt();
    let ec = new elliptic.ec('secp256k1');
    let keyPair = ec.genKeyPair();
    var walletData = await saveWalletData(keyPair, pass);

    $("#mnemonicWordsTxt").text(walletData.words);
    $("#publicKeyTxt").text(walletData.publicKey);
    $("#addressTxt").text(walletData.address);
    updateWalletAddressFields(walletData.address);

    $("#createWalletHiddenSection").show();
}

async function openWallet(pass) {
    var mnemonic = $('#inputMnemonicWordsTxt').val();
    if (mnemonic.split(' ').length != 24) {
        $("errorAlertNotEnoughWordsMsg").text("You need to enter 24 recovery words for your private key!")
        $("#errorAlertNotEnoughWords").fadeTo(2000, 500).slideUp(500, function () {
            $("#errorAlertNotEnoughWords").slideUp(500);
        });
        return;
    }

    var pass = await openWalletPrompt();
    var res = fromMnemonic(mnemonic);
    if (res.error) {
        $("#errorAlertNotEnoughWordsMsg").text(res.error);
        $("#errorAlertNotEnoughWords").fadeTo(2000, 500).slideUp(500, function () {
            $("#errorAlertNotEnoughWords").slideUp(500);
        });
        return;
    }

    var encryptedHex = res.hex;
    var privateKey = await decryptPK(pass, encryptedHex);

    let ec = new elliptic.ec(curve);
    let keyPair = ec.keyFromPrivate(privateKey);
    let walletData = await saveWalletData(keyPair, pass);

    $("#restoredPrivateKeyTxt").text("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
    $("#restoredPublicKeyTxt").text(walletData.publicKey);
    $("#restoredAddressTxt").text(walletData.address);
    $("#openWalletHiddenData").show();
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

            $("#viewAccountHiddenSection").show();
        });
    }
}

async function signTransaction() {
    var validator = $("#sendTransactionForm").data("bs.validator");
    validator.validate();
    if (!validator.hasErrors()) {
        let address = $("#sendTransactionAddress").val();
        let nodeUrl = getBlockchainNodeUrl();
        let recipient = $("#transactionRecipient").val();
        let value = $('#transactionValue').val();

        var pass = await openWalletPrompt();
        let encryptedHex = sessionStorage.getItem("privateKey");
        if (pass && encryptedHex) {
            // decrypt private key from storage
            var privateKey = await decryptPK(pass, encryptedHex);

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

async function saveWalletData(keyPair, pass) {
    return new Promise(async function (resolve, reject) {
        var encryptedHex = await encryptPK(pass, keyPair.priv.toArray());
        sessionStorage.setItem("privateKey", encryptedHex);
        var words = toMnemonic(encryptedHex);

        let pubKey = compressPublicKey(keyPair.getPublic());
        sessionStorage.setItem("publicKey", pubKey);

        let ripemd160 = new Hashes.RMD160();
        let walletAddress = ripemd160.hex(pubKey);
        sessionStorage.setItem("address", walletAddress);

        updateWalletAddressFields(walletAddress);

        resolve({ publicKey: pubKey, address: walletAddress, words: words });
    });
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

function toMnemonic(hex) {
    var bits = bytesToBinary(aesjs.utils.hex.toBytes(hex));
    //1 byte checksum
    var chck = calc_cksum8(hex);

    bits += lpad(chck.toString(2), 8, '0');

    var chunks = bits.match(/(.{1,11})/g);
    var words = chunks.map(function (binary) {
        return mnemonic[binaryToByte(binary)];
    });

    return words.join(' ');
}

function fromMnemonic(words) {
    words = words.split(' ');
    var bits = '';

    for (var i in words) {
        var index = mnemonic.indexOf(words[i]);
        if (index == -1) {
            return { error: 'Invalid word "' + words[i] + '"'};
        }
        bits += lpad(index.toString(2), 11, '0');
    }

    var chck = binaryToByte(bits.substr(256));
    bits = bits.substr(0, 256);

    //when we map to words, we split into chunks of 11 bits,
    //that leaves us with the last word being only 3 bits 
    //so we "unpad" it to get to the original 256 bits
    bits[bits.length - 1] = bits[bits.length - 1].substr(8); 

    var bytes = bits
        .match(/(.{1,8})/g)
        .map(binaryToByte)
        .slice(0, 32);

    var hex = aesjs.utils.hex.fromBytes(bytes);

    if (chck != calc_cksum8(hex)) {
        return { error: "Invalid checksum" };
    }

    return { hex: hex };
}

function binaryToByte(bin) {
    return parseInt(bin, 2)
}

function bytesToBinary(bytes) {
    return bytes.map(function (x) {
        return lpad(x.toString(2), 8, '0')
    }).join('')
}

function lpad(n, width, z) {
    z = z || '0';
    n = n + '';
    return n.length >= width ? n : new Array(width - n.length + 1).join(z) + n;
}

function calc_cksum8(hex) {
    var strN = hex.toUpperCase();
    var strHex = new String("0123456789ABCDEF");
    var result = 0;
    var fctr = 16;

    for (i = 0; i < strN.length; i++) {
        var v = strHex.indexOf(strN.charAt(i));
        result += v * fctr;

        fctr = (fctr == 16 ? 1 : 16);
    }

    // Calculate 2's complement
    return (~(result & 0xff) + 1) & 0xFF;
}

function getBlockchainNodeUrl() {
    return $("#blockchainNodeUrl").val() + '/api';
}

function updateWalletAddressFields(address) {
    $("#sendTransactionAddress").val(address);
    $("#accountBalanceAddress").val(address);
}

function showView(viewName) {
    activeView = viewName;
    $('li.nav-item').removeClass("active");
    $('section').hide();
    $('#' + viewName).show();
}

$(document).ready(function () {
    Date.prototype.toJSON = function () { return this.toISOString(); }

    $("#createWalletHiddenSection").hide();
    $("#openWalletHiddenData").hide();
    $("#viewAccountHiddenSection").hide();
    $("#errorAlertNotEnoughWords").hide();
    $("#successAlert").hide();
    $("#errorAlert").hide();
    $("#passwordsDoNotMatchLabel").hide();

    $('#passwordModal').on('shown.bs.modal', function () {
        $('#passwordModalInput1').focus()
    })

    $('#passwordModal').on('hidden.bs.modal', function () {
        $('#passwordModalInput1').val('');
        $('#passwordModalInput2').val('');
    })

    $('#openWalletModal').on('shown.bs.modal', function () {
        $('#passInput').focus()
    })

    $('#openWalletModal').on('hidden.bs.modal', function () {
        $('#passInput').val('')
    })

    $('#buttonCreateNewWallet').click(createNewWallet);
    $('#buttonOpenWallet').click(openWallet);
    $('#buttonDisplayBalance').click(displayBalance);
    $('#buttonSignTransaction').click(signTransaction);
    $('#buttonSetNodeUrl').click(updateBlockchainNodeUrl);

    //$("#openWalletForm").validator();
    $("#blockchainNodeForm").validator();
    $("#accountBalanceForm").validator();
    $("#sendTransactionForm").validator();

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

    $('#buttonCreateWalletView').click();
});