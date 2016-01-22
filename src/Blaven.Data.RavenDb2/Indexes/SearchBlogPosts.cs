using System;
using System.Linq;

using Raven.Client.Indexes;

namespace Blaven.Data.RavenDb2.Indexes
{
    public class SearchBlogPosts : AbstractMultiMapIndexCreationTask<SearchBlogPosts.Result>
    {
        public SearchBlogPosts()
        {
            this.AddMap<BlogPost>(
                blogPosts => from post in blogPosts
                             where !post.IsDeleted && post.Published > DateTime.MinValue
                             select new Result { Content = new object[] { post.Title, post.Content } });

            this.AddMap<BlogPost>(
                blogPosts => from post in blogPosts
                             where !post.IsDeleted && post.Published > DateTime.MinValue
                             from tag in post.Tags
                             select new Result { Content = new object[] { tag } });

            this.Index(x => x.Content, Raven.Abstractions.Indexing.FieldIndexing.Analyzed);
        }

        public class Result
        {
            public object[] Content { get; set; }
        }
    }
}