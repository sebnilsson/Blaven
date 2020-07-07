using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blaven
{
    public interface IBlogQueryService
    {
        Task<BlogMeta?> GetMeta(BlogKey blogKey = default);

        Task<BlogPost?> GetPost(string id, BlogKey blogKey = default);

        Task<BlogPost?> GetPostBySlug(string slug, BlogKey blogKey = default);

        Task<IReadOnlyList<BlogDateItem>> ListAllDates(
            params BlogKey[] blogKeys);

        Task<IReadOnlyList<BlogMeta>> ListAllMetas();

        Task<IReadOnlyList<BlogTagItem>> ListAllTags(params BlogKey[] blogKeys);

        Task<IPagedReadOnlyList<BlogPostHeader>> ListPosts(
            Paging paging = default,
            params BlogKey[] blogKeys);

        Task<IPagedReadOnlyList<BlogPost>> ListPostsFull(
            Paging paging = default,
            params BlogKey[] blogKeys);

        Task<IPagedReadOnlyList<BlogPostHeader>> ListPostsByArchive(
            DateTimeOffset archiveDate,
            Paging paging = default,
            params BlogKey[] blogKeys);

        Task<IPagedReadOnlyList<BlogPostHeader>> ListPostsByTag(
            string tagName,
            Paging paging = default,
            params BlogKey[] blogKeys);

        Task<IReadOnlyList<BlogPostSeriesEpisode>> ListSeriesEpisodes(
            string seriesName,
            params BlogKey[] blogKeys);

        Task<IPagedReadOnlyList<BlogPostHeader>> SearchPosts(
            string searchText,
            Paging paging = default,
            params BlogKey[] blogKeys);
    }
}
