// Write your Javascript code.
function doGenerate() {
    var f1 = document.form1;
    var curve = "secp256k1"; //f1.curve1.value;
    var ec = new KJUR.crypto.ECDSA({ "curve": curve });
    var keypair = ec.generateKeyPairHex();

    f1.prvkey1.value = keypair.ecprvhex;
    f1.pubkey1.value = keypair.ecpubhex;
}

function doSign() {
    var f1 = document.form1;
    var prvkey = f1.prvkey1.value;
    var curve = "secp256k1"; //f1.curve1.value;
    var sigalg = "SHA256withECDSA"; //f1.sigalg1.value;
    var msg1 = f1.msg1.value;

    var sig = new KJUR.crypto.Signature({ "alg": sigalg });
    sig.init({ d: prvkey, curve: curve });
    sig.updateString(msg1);
    var sigValueHex = sig.sign();

    f1.sigval1.value = sigValueHex;
}

function doVerify() {
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