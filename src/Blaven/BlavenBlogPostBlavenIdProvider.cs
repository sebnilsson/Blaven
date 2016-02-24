using System;
using System.Text;

namespace Blaven
{
    public class BlavenBlogPostBlavenIdProvider : IBlogPostBlavenIdProvider
    {
        private static readonly Lazy<BlavenBlogPostBlavenIdProvider> InstanceLazy =
            new Lazy<BlavenBlogPostBlavenIdProvider>(() => new BlavenBlogPostBlavenIdProvider());

        public static BlavenBlogPostBlavenIdProvider Instance => InstanceLazy.Value;

        private BlavenBlogPostBlavenIdProvider()
        {
        }

        public string GetId(BlogPost blogPost)
        {
            if (blogPost == null)
            {
                throw new ArgumentNullException(nameof(blogPost));
            }

            string id = GetId(blogPost.SourceId);
            return id;
        }

        public static string GetId(string blogPostSourceId)
        {
            if (blogPostSourceId == null)
            {
                throw new ArgumentNullException(nameof(blogPostSourceId));
            }

            var bytes = Encoding.UTF8.GetBytes(blogPostSourceId);
            var sha1 = System.Security.Cryptography.SHA1.Create();
            var hash = sha1.ComputeHash(bytes);

            var hashInt = BitConverter.ToInt32(hash, 0);

            string id = hashInt.ToString("x8");
            return id;
        }
    }
}