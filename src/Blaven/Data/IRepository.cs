using System;
using System.Collections.Generic;
using System.Linq;

namespace Blaven.Data
{
    public interface IRepository
    {
        IQueryable<BlogMeta> GetBlogMetas();

        BlogMeta GetBlogMeta(string blogKey);

        BlogPost GetPost(string blogKey, string blavenId);

        BlogPost GetPostBySourceId(string blogKey, string sourceId);

        IQueryable<BlogArchiveItem> ListArchive(IEnumerable<string> blogKeys);

        IQueryable<BlogTagItem> ListTags(IEnumerable<string> blogKeys);

        IQueryable<BlogPostHead> ListPostHeads(IEnumerable<string> blogKeys);

        IQueryable<BlogPost> ListPosts(IEnumerable<string> blogKeys);

        IQueryable<BlogPost> ListPostsByArchive(IEnumerable<string> blogKeys, DateTime archiveDate);

        IQueryable<BlogPost> ListPostsByTag(IEnumerable<string> blogKeys, string tagName);

        IQueryable<BlogPost> SearchPosts(IEnumerable<string> blogKeys, string search);
    }
}