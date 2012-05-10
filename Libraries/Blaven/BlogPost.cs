﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Blaven {
    /// <summary>
    /// Represents a blog-post on a blog.
    /// </summary>
    public class BlogPost {
        public BlogPost(string blogKey) {
            this.BlogKey = blogKey;

            this.Author = new BlogAuthor();
            this.Tags = Enumerable.Empty<string>();
        }

        /// <summary>
        /// The text-content of a blog-post. Contains HTML-code.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// The unique identifier for the blog-post.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// The tags that are set on the blog-post.
        /// </summary>
        public IEnumerable<string> Tags { get; set; }

        /// <summary>
        /// The date and time that the blog-post was posted.
        /// </summary>
        public DateTime Published { get; set; }

        /// <summary>
        /// The title of the blog-post.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The date and time that the blog-post was updated.
        /// </summary>
        public DateTime Updated { get; set; }

        /// <summary>
        /// The absolute perma-link to the blog-post.
        /// </summary>
        public string PermaLinkAbsolute { get; set; }

        /// <summary>
        /// The relative perma-link to the blog-post.
        /// </summary>
        public string PermaLinkRelative { get; set; }

        /// <summary>
        /// The author-information of the blog-post.
        /// </summary>
        public BlogAuthor Author { get; set; }

        /// <summary>
        /// The blog key the post belongs to..
        /// </summary>
        public string BlogKey { get; set; }
    }
}