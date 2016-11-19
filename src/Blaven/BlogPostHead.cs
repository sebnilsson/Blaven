using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Blaven
{
    [DebuggerDisplay("BlogKey={BlogKey}, BlavenId={BlavenId}, SourceId={SourceId}, Hash={Hash}, Title={Title}")]
    public class BlogPostHead : BlogPostBase
    {
        public BlogAuthor BlogAuthor { get; set; } = new BlogAuthor();

        public long BlogAuthorId { get; set; }

        public string ImageUrl { get; set; }

        public DateTime? PublishedAt { get; set; }

        public string SourceUrl { get; set; }

        public string Summary { get; set; }

        public List<BlogPostTag> BlogPostTags { get; set; }

        public IEnumerable<string> TagTexts => this.BlogPostTags?.Select(x => x.Text) ?? Enumerable.Empty<string>();

        public string Title { get; set; }

        public string UrlSlug { get; set; }
    }
}