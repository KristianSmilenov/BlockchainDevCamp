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
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BlockchainCore.Utils
{
    public class CryptoUtils
    {
        public static byte[] GetSha256Bytes(string content)
        {
            return SHA256Managed.Create()
                .ComputeHash(Encoding.ASCII.GetBytes(content));
        }

        public static string GetSha256Hex(string content)
        {
            return ByteArrayToHex(GetSha256Bytes(content));
        }
        
        public static string ByteArrayToHex(byte[] data)
        {
            return BitConverter
                .ToString(data)
                .Replace("-", "")
                .ToLower();
        }

        public static byte[] HexToByteArray(String hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

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
    }
}