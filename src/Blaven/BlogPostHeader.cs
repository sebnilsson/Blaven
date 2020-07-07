using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Blaven
{
    [DebuggerDisplay("BlogKey={BlogKey}, Id={Id}, Hash={Hash}, Title={Title}")]
    public class BlogPostHeader : BlogPostBase
    {
        public BlogPostHeader()
        {
        }

        public BlogPostHeader(BlogPostHeader blogPost)
        {
            if (blogPost == null)
            {
                return;
            }

            Author = blogPost.Author;
            BlogKey = blogPost.BlogKey;
            Hash = blogPost.Hash;
            Id = blogPost.Id;
            ImageUrl = blogPost.ImageUrl;
            IsDraft = blogPost.IsDraft;
            PublishedAt = blogPost.PublishedAt;
            Series = blogPost.Series;
            Slug = blogPost.Slug;
            SourceId = blogPost.SourceId;
            SourceUrl = blogPost.SourceUrl;
            Summary = blogPost.Summary;
            Tags = blogPost.Tags;
            Title = blogPost.Title;
            UpdatedAt = blogPost.UpdatedAt;
        }

        public BlogAuthor Author { get; set; } = new BlogAuthor();

        public string ImageUrl { get; set; } = string.Empty;

        public bool IsPublished
            => !IsDraft && PublishedAt <= DateTime.UtcNow;

        public bool IsScheduled
            => !IsDraft && PublishedAt > DateTime.UtcNow;

        public DateTimeOffset? PublishedAt { get; set; }

        public string Series { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;

        public string SourceUrl { get; set; } = string.Empty;

        public string Summary { get; set; } = string.Empty;

        public IReadOnlyCollection<string> Tags { get; set; }
            = new HashSet<string>();

        public string Title { get; set; } = string.Empty;
    }
}
