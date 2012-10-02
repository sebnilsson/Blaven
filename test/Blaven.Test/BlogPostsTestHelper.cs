using System;
using System.Collections.Generic;
using System.Linq;

using Blaven.RavenDb;

namespace Blaven.Test {
    public static class BlogPostsTestHelper {
        public static IEnumerable<BlogPost> GetBlogPosts(string blogKey, int count) {
            return GetBlogPosts(blogKey, 1, count);
        }

        public static IEnumerable<BlogPost> GetBlogPosts(string blogKey, int start, int count) {
            var numbers = Enumerable.Range(start, count);
            foreach(var number in numbers) {
                yield return new BlogPost(blogKey, number); //RavenDbBlogStore.GetKey<BlogPost>(Convert.ToString(number)));
            }
        }
    }
}
