using System.Web.Mvc;

using BloggerViewController.Website.Models;

namespace BloggerViewController.Website.Controllers {
    public class BlogController : Controller {
        public ActionResult Index(int? page = 1) {
            int pageIndex = page.GetValueOrDefault(1) - 1; // Given pageIndex is user-friendly, not 0-based

            var service = GetBlogService();

            var info = service.GetInfo();
            var selection = service.GetSelection(pageIndex, BlogConfiguration.PageSize);

            var model = new BlogListViewModel { Info = info, Selection = selection, PageIndex = page.GetValueOrDefault(1), };

            return View(model);
        }

        public ActionResult Post(string id) {
            var service = GetBlogService();

            var info = service.GetInfo();
            var post = service.GetPost(id);

            var model = new BlogSingleViewModel { Info = info, Post = post, };

            return View(model);
        }

        private BlogService GetBlogService() {
            string blogId = BlogConfiguration.BlogId;
            string username = BlogConfiguration.Username;
            string password = BlogConfiguration.Password;
            IBlogStore store = new MemoryBlogStore();

            return new BlogService(blogId, username, password, store);
        }

        // TODO: Handle code in BlogService-class
        /*private BlogData GetBlogData(int currentPageIndex) {
            string cacheKey = string.Format("BlogData_currentPageIndex={0}", currentPageIndex);

            var data = this.HttpContext.Cache[cacheKey] as BlogData;
            if(data == null) {
                var service = GetBlogService();

                data = service.GetBlogData(BlogConfiguration.PageSize, currentPageIndex);

                this.HttpContext.Cache.Add(cacheKey, data, null, DateTime.Now.AddMinutes(5), Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
            }

            return data;
        }*/

        /*private IEnumerable<BlogPost> GetAllPosts() {
            string cacheKey = "AllBlogPosts";

            var allPosts = this.HttpContext.Cache[cacheKey] as IEnumerable<BlogPost>;
            if(allPosts == null) {
                var service = GetBlogService();

                allPosts = service.GetAllPosts();

                this.HttpContext.Cache.Add(cacheKey, allPosts, null, DateTime.Now.AddMinutes(5), Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
            }

            return allPosts;
        }*/
    }
}