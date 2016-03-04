using System;

using Raven.Client;

namespace Blaven.Data.RavenDb2
{
    public static class RavenDbIdConventions
    {
        public static string GetBlogMetaId(string blogKey)
        {
            if (blogKey == null)
            {
                throw new ArgumentNullException(nameof(blogKey));
            }

            string id = blogKey.ToLowerInvariant();
            return id;
        }

        public static string GetBlogPostId(string blogKey, string blavenId)
        {
            if (blogKey == null)
            {
                throw new ArgumentNullException(nameof(blogKey));
            }
            if (blavenId == null)
            {
                throw new ArgumentNullException(nameof(blavenId));
            }

            string id = $"{blogKey.ToLowerInvariant()}/{blavenId}";
            return id;
        }

        internal static void Init(IDocumentStore documentStore)
        {
            if (documentStore == null)
            {
                throw new ArgumentNullException(nameof(documentStore));
            }
            
            documentStore.Conventions.RegisterIdConventions<BlogMeta>(meta => GetBlogMetaId(meta.BlogKey));
            documentStore.Conventions.RegisterIdConventions<BlogPost>(
                post => GetBlogPostId(post.BlogKey, post.BlavenId));
        }
    }
}