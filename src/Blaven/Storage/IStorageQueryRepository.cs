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

        Task<IPagedReadOnlyList<BlogPostHeader>> ListPosts(
            Paging paging,
            IEnumerable<BlogKey> blogKeys);

        Task<IPagedReadOnlyList<BlogPost>> ListPostsFull(
            Paging paging,
            IEnumerable<BlogKey> blogKeys);

        Task<IPagedReadOnlyList<BlogPostHeader>> ListPostsByArchive(
            DateTimeOffset archiveDate,
            Paging paging,
            IEnumerable<BlogKey> blogKeys);

        Task<IPagedReadOnlyList<BlogPostHeader>> ListPostsByTag(
            string tagName,
            Paging paging,
            IEnumerable<BlogKey> blogKeys);

        Task<IPagedReadOnlyList<BlogPostHeader>> SearchPosts(
            string searchText,
            Paging paging,
            IEnumerable<BlogKey> blogKeys);
    }
}
