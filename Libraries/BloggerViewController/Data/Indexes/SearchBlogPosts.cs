using System.Linq;

using Raven.Client.Indexes;

namespace BloggerViewController.Data.Indexes {
    public class SearchBlogPosts : AbstractMultiMapIndexCreationTask<SearchBlogPosts.Result> {
        public class Result {
            public object[] Content { get; set; }
        }

        public SearchBlogPosts() {
            AddMap<BlogPost>(blogPosts => from post in blogPosts
                                          select new Result { Content = new object[] { post.Title, post.Content, } });
            AddMap<BlogPost>(blogPosts => from post in blogPosts
                                          from tag in post.Tags
                                          select new Result { Content = new object[] { tag } });

            Index(x => x.Content, Raven.Abstractions.Indexing.FieldIndexing.Analyzed);
        }
    }
}
