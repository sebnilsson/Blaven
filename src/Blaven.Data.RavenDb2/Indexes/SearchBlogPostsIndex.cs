using System;
using System.Linq;

using Raven.Client.Indexes;

namespace Blaven.Data.RavenDb2.Indexes
{
    public class SearchBlogPostsIndex : AbstractMultiMapIndexCreationTask<SearchBlogPostsIndex.Result>
    {
        public SearchBlogPostsIndex()
        {
            this.AddMap<BlogPost>(
                blogPosts => from post in blogPosts
                             where post.PublishedAt > DateTime.MinValue
                             select
                                 new Result
                                     {
                                         BlogKey = post.BlogKey,
                                         Content =
                                             new object[]
                                                 { post.Content, post.Summary, post.Title, post.Author.Name }
                                     });

            this.AddMap<BlogPost>(
                blogPosts => from post in blogPosts
                             where post.PublishedAt > DateTime.MinValue
                             from tag in post.Tags
                             select new Result { BlogKey = post.BlogKey, Content = new object[] { tag } });

            this.Index(x => x.Content, Raven.Abstractions.Indexing.FieldIndexing.Analyzed);
        }

        public class Result
        {
            public string BlogKey { get; set; }

            public object[] Content { get; set; }
        }
    }
}