using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BlockchainCore.Utils
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

        public static string GetSha256Hex(string content)
        {
            return ByteArrayToHex(GetSha256Bytes(content));
        }
        
        public static String GetHash(String text, String key)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            Byte[] textBytes = encoding.GetBytes(text);
            Byte[] keyBytes = encoding.GetBytes(key);

            HMACSHA256 hash = new HMACSHA256(keyBytes);
            Byte[] hashBytes = hash.ComputeHash(textBytes);

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        #region Hex functions

        public static string ByteArrayToHex(byte[] data)
        {
            string hex = BitConverter.ToString(data);
            return hex.Replace("-", "");
        }

        public static byte[] HexToByteArray(String hex)
        {
            int numberChars = hex.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        #endregion

        #region BouncyCastle.NetCore

        static X9ECParameters curve = SecNamedCurves.GetByName("secp256k1");
        static ECDomainParameters domain = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H);
        static SecureRandom secureRandom = new SecureRandom();
        static BigInteger HALF_CURVE_ORDER = curve.N.ShiftRight(1);

        public static byte[] CreateNewPrivateKey()
        {
            ECKeyPairGenerator generator = new ECKeyPairGenerator();
            ECKeyGenerationParameters keygenParams = new ECKeyGenerationParameters(domain, secureRandom);
            generator.Init(keygenParams);
            AsymmetricCipherKeyPair keypair = generator.GenerateKeyPair();
            ECPrivateKeyParameters privParams = (ECPrivateKeyParameters)keypair.Private;
            return privParams.D.ToByteArray();
        }

        public static byte[] GetPublicFor(byte[] privateKey)
        {
            return curve.G.Multiply(new BigInteger(privateKey)).GetEncoded(true);
        }

        public static bool BouncyCastleVerify(byte[] hash, byte[] publicKey, BigInteger R, BigInteger S)
        {
            try
            {
                ECDsaSigner signer = new ECDsaSigner();
                signer.Init(false, new ECPublicKeyParameters(curve.Curve.DecodePoint(publicKey), domain));
                return signer.VerifySignature(hash, R, S);
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static bool BouncyCastleVerify(byte[] hash, byte[] signature, byte[] publicKey)
        {
            Asn1InputStream asn1 = new Asn1InputStream(signature);
            try
            {
                ECDsaSigner signer = new ECDsaSigner();
                signer.Init(false, new ECPublicKeyParameters(curve.Curve.DecodePoint(publicKey), domain));

                Asn1Sequence seq = (Asn1Sequence)asn1.ReadObject();
                DerInteger r = DerInteger.GetInstance(seq[0]);
                DerInteger s = DerInteger.GetInstance(seq[1]);
                return signer.VerifySignature(hash, r.PositiveValue, s.PositiveValue);
            }
            catch (Exception e)
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

        public static byte[] BouncyCastleSign(byte[] hash, byte[] privateKey)
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

        private static BigInteger ToCanonicalS(BigInteger s)
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