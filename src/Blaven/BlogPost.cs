using System;
using System.Collections.Generic;
using System.Linq;

using Blaven.RavenDb;

namespace Blaven
{
    /// <summary>
    /// Represents a blog-post on a blog.
    /// </summary>
    public class BlogPost
    {
        public BlogPost(string blogKey, ulong dataSourceId)
            : this(blogKey)
        {
            this.DataSourceId = dataSourceId.ToString("#");

            this.BlavenId = BlavenHelper.GetBlavenHash(dataSourceId);
            this.Id = RavenDbHelper.GetEntityId<BlogPost>(this.BlavenId);
        }

        public BlogPost(string blogKey, string dataSourceId)
            : this(blogKey)
        {
            this.DataSourceId = dataSourceId;

            this.BlavenId = BlavenHelper.GetBlavenHash(dataSourceId);
            this.Id = RavenDbHelper.GetEntityId<BlogPost>(this.BlavenId);
        }

        internal BlogPost(string blogKey)
            : this()
        {
            this.BlogKey = blogKey;
        }

        internal BlogPost()
        {
            this.Author = new BlogAuthor();
            this.Tags = Enumerable.Empty<string>();
        }

        /// <summary>
        /// The author-information of the blog-post.
        /// </summary>
        public BlogAuthor Author { get; set; }

        /// <summary>
        /// The automatic incremented unique ID given by RavenDB.
        /// </summary>
        public string BlavenId { get; internal set; }

        /// <summary>
        /// The blog key the post belongs to.
        /// </summary>
        public string BlogKey { get; internal set; }

        /// <summary>
        /// The text-content of a blog-post. Contains HTML-code.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// The original ID of the blog-post at the data-source.
        /// </summary>
        public string DataSourceId { get; set; }

        /// <summary>
        /// The absolute original URL to the blog-post at the data-source.
        /// </summary>
        public string DataSourceUrl { get; set; }

        /// <summary>
        /// The unique identifier for the blog-post in RavenDB.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets if the post is deleted.
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Gets or sets the hash of the blog-post.
        /// </summary>
        public string Checksum { get; set; }

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
        public DateTime Updated { get; set; }

        /// <summary>
        /// The URL-friendly slug, based on the title of the blog post.
        /// </summary>
        public string UrlSlug { get; set; }
    }
}