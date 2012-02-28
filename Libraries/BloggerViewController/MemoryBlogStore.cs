using System;
using System.Linq;
using System.Xml.Linq;

namespace BloggerViewController {
    public class MemoryBlogStore : IBlogStore {
        private static DateTime? _lastUpdate;
        private static BlogData _blogData;

        public void Update(XDocument bloggerDocument) {
            lock(_lock) {
                var newBlogData = BloggerHelper.ParseBlogData(bloggerDocument);
                
                _blogData = _blogData ?? new BlogData();
                _blogData.Posts = _blogData.Posts.Union(newBlogData.Posts);
                _blogData.Info = newBlogData.Info;
                
                _lastUpdate = DateTime.Now;
            }
        }

        public BlogInfo GetBlogInfo() {
            return _blogData.Info;
        }

        public BlogSelection GetBlogSelection(int pageIndex, int? pageSize) {
            int take = pageSize.GetValueOrDefault(BlogConfiguration.PageSize);
            int skip = (pageIndex * take);

            var selectionPosts = _blogData.Posts.Skip(skip).Take(take);
            return new BlogSelection(_blogData.Posts, selectionPosts, pageIndex, pageSize);
        }

        public BlogPost GetBlogPost(string link) {
            throw new NotImplementedException();
        }

        public BlogPost GetBlogPostById(string blogId) {
            return _blogData.Posts.FirstOrDefault(post => post.ID == blogId);
        }

        private static object _lock = new object();

        public DateTime? LastUpdate {
            get {
                lock(_lock) {
                    return _lastUpdate;
                }
            }
        }
    }
}
