using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace BloggerViewController {
    public class DiskBlogStore : IBlogStore {
        private static DateTime? _lastUpdate;

        private static string DiskPath {
            get {
                string path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "BloggerViewControllerStore.txt");
                if(!System.IO.File.Exists(path)) {
                    lock(_diskLock) {
                        if(!System.IO.File.Exists(path)) {
                            using(System.IO.File.Create(path)) { }
                        }
                    }
                }
                return path;
            }
        }

        private BlogData _blogData;
        private BlogData BlogData {
            get {
                if(_blogData == null) {
                    _blogData = GetBlogData();
                }
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

                StoreBlogData(_blogData);
            }
        }

        private BlogData GetBlogData() {
            string fileContent;
            lock(_diskLock) {
                fileContent = File.ReadAllText(DiskPath);
            }

            if(string.IsNullOrWhiteSpace(fileContent)) {
                return null;
            }

            var deserialized = SerializationHelper.GetDeserializedObject<BlogData>(fileContent);
            return deserialized;
        }

        private void StoreBlogData(BlogData blogData) {
            string serialized = SerializationHelper.GetSerializedString<BlogData>(blogData);

            lock(_diskLock) {
                System.IO.File.WriteAllText(DiskPath, serialized);
            }

            _blogData = blogData;
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

        private static object _lock = new object();
        private static object _diskLock = new object();

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
