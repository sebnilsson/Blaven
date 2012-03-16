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

        public BlogPost GetBlogPost(string permaLinkRelative, string blogKey) {
            var data = GetBlogData(blogKey);
            return data.Posts.FirstOrDefault(post => post.PermaLinkRelative == permaLinkRelative);
        }

        public BlogSelection GetBlogSelection(int pageIndex, int pageSize, IEnumerable<string> blogKeys, Func<BlogPost, bool> predicate = null) {
            var blogDatas = _blogData.Values.Where(data => blogKeys.Contains(data.Info.BlogKey));

            var selectedPosts = Enumerable.Empty<BlogPost>();
            foreach(var data in blogDatas) {
                selectedPosts = selectedPosts.Union(data.Posts);
            }

            if(predicate != null) {
                selectedPosts = selectedPosts.Where(predicate);
            }

            selectedPosts = selectedPosts.OrderByDescending(post => post.Published);

            return new BlogSelection(selectedPosts, pageIndex, pageSize);
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
