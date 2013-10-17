using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Blaven
{
    public class BlogPostBase
    {
        public BlogPostBase(BlogPost blogPost)
        {
            BlavenId = blogPost.BlavenId;
            BlogKey = blogPost.BlogKey;
            Published = blogPost.Published;
            Tags = blogPost.Tags;
            Title = blogPost.Title;
            Updated = blogPost.Updated;
            UrlSlug = blogPost.UrlSlug;
        }

        [JsonConstructor]
        protected BlogPostBase()
        {
        }

        /// <summary>
        /// The automatic incremented unique ID given by RavenDB.
        /// </summary>
        public string BlavenId { get; internal set; }

        /// <summary>
        /// The blog key the post belongs to.
        /// </summary>
        public string BlogKey { get; internal set; }

        /// <summary>
        /// The date and time that the blog-post was posted.
        /// </summary>
        public DateTime Published { get; set; }

        /// <summary>
        /// The tags that are set on the blog-post.
        /// </summary>
        public IEnumerable<string> Tags { get; set; }

        /// <summary>
        /// The title of the blog-post.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The date and time that the blog-post was updated.
        /// </summary>
        public DateTime? Updated { get; set; }

        /// <summary>
        /// The URL-friendly slug, based on the title of the blog post.
        /// </summary>
        public string UrlSlug { get; set; }
    }
}