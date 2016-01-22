using System;
using System.Collections.Generic;
using System.Linq;

namespace Blaven.Data
{
    public interface IRepository
    {
        BlogMeta GetBlogMeta(string blogKey);

        BlogPost GetPost(string blogKey, string blavenId);

        BlogPost GetPostBySourceId(string blogKey, string sourceId);

        IQueryable<BlogArchiveItem> ListArchive(IEnumerable<string> blogKeys);

        IQueryable<BlogTagItem> ListTags(IEnumerable<string> blogKeys);

        IQueryable<BlogPostHead> ListPostHeads(IEnumerable<string> blogKeys);

        IQueryable<BlogPost> ListPosts(IEnumerable<string> blogKeys);

        IQueryable<BlogPost> ListPostsByArchive(IEnumerable<string> blogKeys, DateTime date);

        IQueryable<BlogPost> ListPostsByTag(IEnumerable<string> blogKeys, string tagName);
    }
}