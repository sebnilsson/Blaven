using System.Linq;

using Raven.Client.Indexes;

namespace Blaven.RavenDb.Indexes {
    public class SearchBlogPosts : AbstractMultiMapIndexCreationTask<SearchBlogPosts.Result> {
        public class Result {
            public object[] Content { get; set; }
        }

        public SearchBlogPosts() {
            AddMap<BlogPost>(blogPosts => from post in blogPosts
                                          where !post.IsDeleted
                                          select new Result { Content = new object[] { post.Title, post.Content, } });
            AddMap<BlogPost>(blogPosts => from post in blogPosts
                                          where !post.IsDeleted
                                          from tag in post.Tags
                                          select new Result { Content = new object[] { tag } });

            Index(x => x.Content, Raven.Abstractions.Indexing.FieldIndexing.Analyzed);
        }
    }
}
