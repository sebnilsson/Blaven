using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blaven.BlogSources.Blogger
{
    public interface IBloggerApiProvider
    {
        Task<BloggerBlogData> GetBlog(string blogId);

        Task<IReadOnlyList<BloggerPostData>> GetPosts(string blogId, DateTime? lastUpdatedAt);
    }
}