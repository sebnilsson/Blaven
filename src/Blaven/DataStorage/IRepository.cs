using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blaven.DataStorage
{
    public interface IRepository
    {
        Task<BlogMeta> GetBlogMeta(string blogKey);

        IQueryable<BlogMeta> GetBlogMetas();

        Task<BlogPost> GetPost(string blogKey, string blavenId);

        Task<BlogPost> GetPostBySourceId(string blogKey, string sourceId);

        IQueryable<BlogArchiveItem> ListArchive(IEnumerable<string> blogKeys);

        IQueryable<BlogPostHead> ListPostHeads(IEnumerable<string> blogKeys);

        IQueryable<BlogPost> ListPosts(IEnumerable<string> blogKeys);

        IQueryable<BlogPost> ListPostsByArchive(IEnumerable<string> blogKeys, DateTime archiveDate);

        IQueryable<BlogPost> ListPostsByTag(IEnumerable<string> blogKeys, string tagName);

        IQueryable<BlogTagItem> ListTags(IEnumerable<string> blogKeys);

        IQueryable<BlogPost> SearchPosts(IEnumerable<string> blogKeys, string search);
    }
}