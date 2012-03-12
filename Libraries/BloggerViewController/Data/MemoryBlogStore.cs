using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace BloggerViewController.Data {
    public class MemoryBlogStore : IBlogStore {
        private static Dictionary<string, BlogData> _blogData = new Dictionary<string, BlogData>();
        private static Dictionary<string, DateTime> _blogUpdates = new Dictionary<string, DateTime>();

        private static BlogData GetBlogData(string blogKey) {
            BlogData data;
            if(!_blogData.TryGetValue(blogKey, out data)) {
                return null;
            }
            return data;
        }

        public BlogInfo GetBlogInfo(string blogKey) {
            return _blogData[blogKey].Info;
        }

        public BlogPost GetBlogPost(string permaLink, string blogKey) {
            var data = GetBlogData(blogKey);
            return data.Posts.FirstOrDefault(post => post.FriendlyPermaLink == permaLink);
        }

        public BlogSelection GetBlogSelection(int pageIndex, int? pageSize, string blogKey) {
            int take = pageSize.GetValueOrDefault(ConfigurationService.PageSize);
            int skip = (pageIndex * take);

            var data = GetBlogData(blogKey);

            var selectionPosts = data.Posts.Skip(skip).Take(take);
            return new BlogSelection(data.Posts, selectionPosts, pageIndex, pageSize);
        }

        public bool GetIsBlogUpdated(string blogKey) {
            if(!_blogUpdates.ContainsKey(blogKey)) {
                return false;
            }

            var updated = _blogUpdates[blogKey];
            return updated.AddMinutes(ConfigurationService.CacheTime) > DateTime.Now;
        }

        public void Update(XDocument bloggerDocument, string blogKey) {
            _blogUpdates[blogKey] = DateTime.Now;

            if(bloggerDocument == null) {
                return;
            }

            var newBlogData = BloggerHelper.ParseBlogData(bloggerDocument, blogKey);

            _blogData[blogKey] = newBlogData;
        }
    }
}
