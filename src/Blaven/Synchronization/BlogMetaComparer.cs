using System;

namespace Blaven.Synchronization
{
    internal static class BlogMetaComparer
    {
        public static bool AreMetasEqual(
            BlogMeta blogSourceMeta,
            BlogMeta? storageMeta)
        {
            if (blogSourceMeta is null)
                throw new ArgumentNullException(nameof(blogSourceMeta));

            if (storageMeta == null)
            {
                return false;
            }

            return
                blogSourceMeta.Description == storageMeta.Description
                && blogSourceMeta.Name == storageMeta.Name
                && blogSourceMeta.PublishedAt == storageMeta.PublishedAt
                && blogSourceMeta.UpdatedAt == storageMeta.UpdatedAt
                && blogSourceMeta.Url == storageMeta.Url;
        }
    }
}
