using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Blaven
{
    public static class HashUtility
    {
        private static readonly HashAlgorithm Hash = SHA256.Create();

        public static string GetBase64(params object[] args)
        {
            var hashBytes = GetBytes(args);

            string hashBase64 = Convert.ToBase64String(hashBytes);
            return hashBase64;
        }

        public static byte[] GetBytes(params object[] args)
        {
            args = args ?? new object[0];

            var argsStrings = args.Select(Convert.ToString);
            var argsValue = string.Join("|", argsStrings);

            var argsBuffer = Encoding.UTF8.GetBytes(argsValue);

            var hashBytes = Hash.ComputeHash(argsBuffer);
            return hashBytes;
        }
    }
}