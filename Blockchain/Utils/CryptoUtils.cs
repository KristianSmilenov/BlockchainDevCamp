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
    }
}