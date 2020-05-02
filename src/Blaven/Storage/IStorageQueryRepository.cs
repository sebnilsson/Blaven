using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blaven.Storage
{
    public interface IStorageQueryRepository
    {
        Task<BlogMeta?> GetMeta(BlogKey blogKey);

        Task<BlogPost?> GetPost(string id, BlogKey blogKey);

        Task<BlogPost?> GetPostBySlug(string slug, BlogKey blogKey);

        Task<IReadOnlyList<BlogDateItem>> ListAllDates(
            IEnumerable<BlogKey> blogKeys);

        Task<IReadOnlyList<BlogMeta>> ListAllMetas();

        Task<IReadOnlyList<BlogTagItem>> ListAllTags(
            IEnumerable<BlogKey> blogKeys);

        Task<IReadOnlyList<BlogPostHeader>> ListPostHeaders(
            Paging paging,
            IEnumerable<BlogKey> blogKeys);

        Task<IReadOnlyList<BlogPost>> ListPostsByArchive(
            DateTimeOffset archiveDate,
            Paging paging,
            IEnumerable<BlogKey> blogKeys);

        Task<IReadOnlyList<BlogPost>> ListPostsByTag(
            string tagName,
            Paging paging,
            IEnumerable<BlogKey> blogKeys);

        Task<IReadOnlyList<BlogPostHeader>> SearchPostHeaders(
            string searchText,
            Paging paging,
            IEnumerable<BlogKey> blogKeys);
    }
}
