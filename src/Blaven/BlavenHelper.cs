using System;
using System.Text;

namespace Blaven
{
    public static class BlavenHelper
    {
        public static string GetBlavenHash(string id, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;

            var bytes = encoding.GetBytes(id);
            return GetBlavenHash(bytes);
        }

        public static string GetBlavenHash(ulong id)
        {
            var bytes = BitConverter.GetBytes(id);
            return GetBlavenHash(bytes);
        }

        private static string GetBlavenHash(byte[] bytes)
        {
            var sha1 = System.Security.Cryptography.SHA1.Create();
            var hash = sha1.ComputeHash(bytes);

            var hashInt = BitConverter.ToInt32(hash, 0);
            return hashInt.ToString("x8");
        }
    }
}