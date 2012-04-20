using System.Linq;

using Raven.Client.Indexes;

namespace BloggerViewController.Data.Indexes {
    public class BlogPostsOrderedByCreated : AbstractIndexCreationTask<BlogPost, BlogPost> {
        public BlogPostsOrderedByCreated() {
            Map = posts => from post in posts
                           orderby post.Published descending
                           select new { BlogKey = post.BlogKey, Tags = post.Tags, Post = post,
                               Published = post.Published, Published_Year = post.Published.Year, Published_Month = post.Published.Month, };
        }
    }
}
