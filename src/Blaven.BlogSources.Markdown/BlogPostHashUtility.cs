using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Blaven.BlogSources.Markdown
{
    internal static class BlogPostHashUtility
    {
        public static string GetHash(BlogPost post)
        {
            if (post is null)
                throw new ArgumentNullException(nameof(post));

            var json = JsonSerializer.Serialize(post);

            var jsonBytes = Encoding.UTF8.GetBytes(json);

            using var sha256 = new SHA256Managed();

            var hashBytes = sha256.ComputeHash(jsonBytes);

            var stringBuilder = new StringBuilder();

            foreach (var b in hashBytes)
            {
                stringBuilder.Append(b.ToString("x2"));
            }

            return stringBuilder.ToString();
        }
    }
}
