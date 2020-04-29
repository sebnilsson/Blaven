using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blaven.Storage
{
    public interface IStorageRepository
    {
        Task<BlogMeta> GetBlogMeta(BlogKey blogKey);

        Task<BlogPost> GetPost(string id, BlogKey blogKey);

        Task<BlogPost> GetPostBySlug(string slug, BlogKey blogKey);

        Task<BlogPost> GetPostBySourceId(string sourceId, BlogKey blogKey);

        Task<IReadOnlyList<BlogArchiveItem>> ListArchive(
            IEnumerable<BlogKey> blogKeys);

        Task<IReadOnlyList<BlogMeta>> ListBlogMetas(
            IEnumerable<BlogKey> blogKeys);

        Task<IReadOnlyList<BlogPostHeader>> ListPostHeaders(
            Paging paging,
            IEnumerable<BlogKey> blogKeys);

        Task<IReadOnlyList<BlogPost>> ListPostsByArchive(
            DateTime archiveDate,
            Paging paging,
            IEnumerable<BlogKey> blogKeys);

        Task<IReadOnlyList<BlogPost>> ListPostsByTag(
            string tag,
            Paging paging,
            IEnumerable<BlogKey> blogKeys);

        Task<IReadOnlyList<BlogTagItem>> ListTags(
            IEnumerable<BlogKey> blogKeys);

        Task<IReadOnlyList<BlogPostHeader>> SearchPostHeaders(
            string searchText,
            Paging paging,
            IEnumerable<BlogKey> blogKeys);
    }
}
