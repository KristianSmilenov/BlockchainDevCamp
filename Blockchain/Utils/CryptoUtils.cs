using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Blockchain.Utils
{
    public class CryptoUtils
    {
        public static byte[] GetSha256Bytes(string content)
        {
            var bytes = Encoding.ASCII.GetBytes(content);
            return GetSha256Bytes(bytes);
        }

        public static byte[] GetSha256Bytes(byte[] content)
        {
            var mySHA256 = SHA256Managed.Create();
            return mySHA256.ComputeHash(content);
        }

        public static string GetSha256String(string content)
        {
            return Encoding.ASCII.GetString(GetSha256Bytes(content));
        }

        public static string GetSha256String(byte[] content)
        {
            return Encoding.ASCII.GetString(GetSha256Bytes(content));
        }

        #region Cryptography.ECDSA.Secp256k1

        private string SignTransaction(string text, string privateKey)
        {
            var msg = Encoding.UTF8.GetBytes(text);
            var hex = Cryptography.ECDSA.Base58.GetBytes(privateKey);
            var sha256 = Cryptography.ECDSA.Proxy.GetMessageHash(msg);
            var signature = Cryptography.ECDSA.Proxy.SignCompressedCompact(sha256, hex);

            return Cryptography.ECDSA.Hex.ToString(signature);
        }

        private void VerifyTransaction(byte[] message, byte[] signature, byte[] publicKey, bool normalizeSignatureOnFailure)
        {
            var result = Cryptography.ECDSA.Proxy.Verify(message, signature, publicKey, false);
        }

        #endregion

        #region BouncyCastle.NetCore

        static X9ECParameters curve = SecNamedCurves.GetByName("secp256k1");
        static ECDomainParameters domain = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H);
        static SecureRandom secureRandom = new SecureRandom();
        static BigInteger HALF_CURVE_ORDER = curve.N.ShiftRight(1);

        public bool BouncyCastleVerify(byte[] hash, byte[] signature, byte[] publicKey)
        {
            Asn1InputStream asn1 = new Asn1InputStream(signature);
            try
            {
                ECDsaSigner signer = new ECDsaSigner();
                signer.Init(false, new ECPublicKeyParameters(curve.Curve.DecodePoint(publicKey), domain));

                Asn1Sequence seq = (Asn1Sequence)asn1.ReadObject();
                DerInteger r = DerInteger.GetInstance(seq[0]);
                DerInteger s = DerInteger.GetInstance(seq[1]);
                return signer.VerifySignature(hash, r.Value, s.Value);
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                try
                {
                    asn1.Close();
                }
                catch (IOException)
                {
                }
            }

        }

        public byte[] BouncyCastleSign(byte[] hash, byte[] privateKey)
        {
            ECDsaSigner signer = new ECDsaSigner(new HMacDsaKCalculator(new Sha256Digest()));
            signer.Init(true, new ECPrivateKeyParameters(new BigInteger(privateKey), domain));
            BigInteger[] signature = signer.GenerateSignature(hash);
            MemoryStream baos = new MemoryStream();
            try
            {
                DerSequenceGenerator seq = new DerSequenceGenerator(baos);
                seq.AddObject(new DerInteger(signature[0]));
                seq.AddObject(new DerInteger(ToCanonicalS(signature[1])));
                seq.Close();
                return baos.ToArray();
            }
            catch (IOException)
            {
                return new byte[0];
            }
        }

        private BigInteger ToCanonicalS(BigInteger s)
        {
            if (s.CompareTo(HALF_CURVE_ORDER) <= 0)
            {
                return s;
            }
            else
            {
                return curve.N.Subtract(s);
            }
        }

        #endregion
    }
}