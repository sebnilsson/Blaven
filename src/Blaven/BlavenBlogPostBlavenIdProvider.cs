using System;
using System.Text;

namespace Blaven
{
    public class BlavenBlogPostBlavenIdProvider : IBlogPostBlavenIdProvider
    {
        public string GetId(BlogPost blogPost)
        {
            if (blogPost == null)
            {
                throw new ArgumentNullException(nameof(blogPost));
            }
            if (string.IsNullOrWhiteSpace(blogPost.SourceId))
            {
                string message = $"{nameof(BlogPost)} cannot have an empty or null {nameof(BlogPost.SourceId)}.";
                throw new ArgumentOutOfRangeException(nameof(blogPost), message);
            }

            var bytes = Encoding.UTF8.GetBytes(blogPost.SourceId);
            var sha1 = System.Security.Cryptography.SHA1.Create();
            var hash = sha1.ComputeHash(bytes);

            var hashInt = BitConverter.ToInt32(hash, 0);
            return hashInt.ToString("x8");
        }
    }
}