using System;
using System.Collections.Generic;
using System.Linq;

using Google.GData.Blogger;

namespace BloggerViewController {
    public static class BlogService {
        public static IEnumerable<BlogPostPreview> ListBlogs(DateTime? minPublicationDate = null) {
            var service = new BloggerService("BloggerViewController");
            var query = new BloggerQuery();
            if(minPublicationDate.HasValue) {
                query.MinPublication = minPublicationDate.Value;
            }

            var result = service.Query(query);

            return Enumerable.Empty<BlogPostPreview>();
        }
    }
}
