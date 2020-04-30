using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blaven
{
    public interface IBlogService
    {
        Task<BlogMeta> GetMeta(BlogKey blogKey = default);

        Task<BlogPost> GetPost(string id, BlogKey blogKey = default);

        Task<BlogPost> GetPostBySlug(string slug, BlogKey blogKey = default);

        Task<BlogPost> GetPostBySourceId(
            string sourceId,
            BlogKey blogKey = default);

        Task<IReadOnlyList<BlogArchiveItem>> ListArchive(
            params BlogKey[] blogKeys);

        Task<IReadOnlyList<BlogMeta>> ListMetas(params BlogKey[] blogKeys);

        Task<IReadOnlyList<BlogPostHeader>> ListPostHeaders(
            Paging paging = default,
            params BlogKey[] blogKeys);

        Task<IReadOnlyList<BlogPostHeader>> ListPostsByArchive(
            DateTimeOffset archiveDate,
            Paging paging = default,
            params BlogKey[] blogKeys);

        Task<IReadOnlyList<BlogPostHeader>> ListPostsByTag(
            string tag,
            Paging paging = default,
            params BlogKey[] blogKeys);

        Task<IReadOnlyList<BlogTagItem>> ListTags(params BlogKey[] blogKeys);

        Task<IReadOnlyList<BlogPostHeader>> SearchPostHeaders(
            string searchText,
            Paging paging = default,
            params BlogKey[] blogKeys);
    }
}
