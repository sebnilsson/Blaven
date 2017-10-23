using System;
using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace Blaven.DataStorage.RavenDb.Indexes
{
    public class SearchBlogPostsIndex : AbstractMultiMapIndexCreationTask<SearchBlogPostsIndex.Result>
    {
        public SearchBlogPostsIndex()
        {
            AddMap<BlogPost>(
                blogPosts => from post in blogPosts
                             where post.PublishedAt > DateTime.MinValue
                             select new Result
                                    {
                                        BlogKey = post.BlogKey,
                                        Content = new object[]
                                                  {
                                                      post.Content, post.Summary, post.Title,
                                                      post.BlogAuthor.Name
                                                  }
                                    });

            AddMap<BlogPost>(
                blogPosts => from post in blogPosts
                             where post.PublishedAt > DateTime.MinValue
                             from tag in post.BlogPostTags
                             select new Result
                                    {
                                        BlogKey = post.BlogKey,
                                        Content = new object[] { tag.Text }
                                    });

            Index(x => x.Content, FieldIndexing.Analyzed);
        }

        public class Result : BlogKeyItemBase
        {
            public object[] Content { get; set; }
        }
    }
}