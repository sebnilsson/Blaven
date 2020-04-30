using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blaven
{
    public interface IBlogServiceRepository
    {
        Task<BlogMeta> GetMeta(BlogKey blogKey);

        Task<BlogPost> GetPost(string id, BlogKey blogKey);

        Task<BlogPost> GetPostBySlug(string slug, BlogKey blogKey);

        Task<BlogPost> GetPostBySourceId(string sourceId, BlogKey blogKey);

        Task<IReadOnlyList<BlogArchiveItem>> ListArchive(
            IEnumerable<BlogKey> blogKeys);

        Task<IReadOnlyList<BlogMeta>> ListMetas(IEnumerable<BlogKey> blogKeys);

        Task<IReadOnlyList<BlogPostHeader>> ListPostHeaders(
            Paging paging,
            IEnumerable<BlogKey> blogKeys);

        Task<IReadOnlyList<BlogPost>> ListPostsByArchive(
            DateTimeOffset archiveDate,
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
