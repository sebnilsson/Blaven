using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Blaven.Transformation
{
    public class BlogPostSha256HashStorageTransform
        : IBlogPostStorageTransform
    {
        public void Transform(BlogPost post)
        {
            if (post == null || post.Hash != null)
            {
                return;
            }

            post.Hash = GetHash(post);
        }

        private static string GetHash(BlogPost post)
        {
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
