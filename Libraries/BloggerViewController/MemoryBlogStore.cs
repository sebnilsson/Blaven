using System;
using System.Linq;
using System.Xml.Linq;

namespace BloggerViewController {
    public class MemoryBlogStore : IBlogStore {
        private static DateTime? _lastUpdate;
        private static BlogData _blogData;

        private static BlogData BlogData {
            get {
                return _blogData;
            }
        }

        public void Update(XDocument bloggerDocument) {
            lock(_lock) {
                _lastUpdate = DateTime.Now;

                if(bloggerDocument == null) {
                    return;
                }

                var newBlogData = BloggerHelper.ParseBlogData(bloggerDocument);

                _blogData = newBlogData;
            }
        }

        public BlogInfo GetBlogInfo() {
            return BlogData.Info;
        }

        public BlogSelection GetBlogSelection(int pageIndex, int? pageSize) {
            int take = pageSize.GetValueOrDefault(BlogConfigurationHelper.PageSize);
            int skip = (pageIndex * take);

            var selectionPosts = BlogData.Posts.Skip(skip).Take(take);
            return new BlogSelection(BlogData.Posts, selectionPosts, pageIndex, pageSize);
        }

        public BlogPost GetBlogPost(string permaLink) {
            return _blogData.Posts.FirstOrDefault(post => post.FriendlyPermaLink == permaLink);
        }

        private static readonly object _lock = new object();

        public DateTime? LastUpdate {
            get {
                lock(_lock) {
                    return _lastUpdate;
                }
            }
        }

        public bool HasData {
            get {
                return BlogData != null && !string.IsNullOrWhiteSpace(BlogData.Info.Title);
            }
        }
    }
}
