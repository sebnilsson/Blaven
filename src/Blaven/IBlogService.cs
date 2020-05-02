using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blaven
{
    public interface IBlogService
    {
        Task<BlogMeta?> GetMeta(BlogKey blogKey = default);

        Task<BlogPost?> GetPost(string id, BlogKey blogKey = default);

        Task<BlogPost?> GetPostBySlug(string slug, BlogKey blogKey = default);

        Task<IReadOnlyList<BlogDateItem>> ListAllDates(
            params BlogKey[] blogKeys);

        Task<IReadOnlyList<BlogTagItem>> ListAllTags(params BlogKey[] blogKeys);

        Task<IReadOnlyList<BlogMeta>> ListMetas(params BlogKey[] blogKeys);

        Task<IReadOnlyList<BlogPostHeader>> ListPostHeaders(
            Paging paging = default,
            params BlogKey[] blogKeys);

        Task<IReadOnlyList<BlogPostHeader>> ListPostsByArchive(
            DateTimeOffset archiveDate,
            Paging paging = default,
            params BlogKey[] blogKeys);

        Task<IReadOnlyList<BlogPostHeader>> ListPostsByTag(
            string tagName,
            Paging paging = default,
            params BlogKey[] blogKeys);

        Task<IReadOnlyList<BlogPostHeader>> SearchPostHeaders(
            string searchText,
            Paging paging = default,
            params BlogKey[] blogKeys);
    }
}
