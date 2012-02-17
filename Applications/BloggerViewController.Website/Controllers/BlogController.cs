using System;
using System.Collections.Generic;
using System.Web.Caching;
using System.Web.Mvc;

using BloggerViewController.Website.Models;

namespace BloggerViewController.Website.Controllers {
    public class BlogController : Controller {
        public ActionResult Index(int? page = 1) {
            var allPosts = GetAllPosts();
            var data = GetBlogData(page.GetValueOrDefault(1) - 1);

            var paging = new BlogPagingHelper(allPosts, data.Posts);

            var model = new BlogListViewModel { Data = data, Paging = paging, PageIndex = page.GetValueOrDefault(1) };

            return View(model);
        }

        public ActionResult Post(string id) {
            var service = GetBlogService();

            var data = service.GetBlogData(entryId: id);
            var model = new BlogSingleViewModel { Data = data };

            return View(model);
        }

        private BlogData GetBlogData(int currentPageIndex) {
            string cacheKey = string.Format("BlogData_currentPageIndex={0}", currentPageIndex);

            var data = this.HttpContext.Cache[cacheKey] as BlogData;
            if(data == null) {
                var service = GetBlogService();

                data = service.GetBlogData(BlogConfiguration.PageSize, currentPageIndex);

                this.HttpContext.Cache.Add(cacheKey, data, null, DateTime.Now.AddMinutes(5), Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
            }

            return data;
        }

        private BlogService GetBlogService() {
            string blogId = BlogConfiguration.BlogId;
            string username = BlogConfiguration.Username;
            string password = BlogConfiguration.Password;

            var service = new BlogService(blogId, username, password);
            return service;
        }

        private IEnumerable<BlogPost> GetAllPosts() {
            string cacheKey = "AllBlogPosts";

            var allPosts = this.HttpContext.Cache[cacheKey] as IEnumerable<BlogPost>;
            if(allPosts == null) {
                var service = GetBlogService();

                allPosts = service.GetAllPosts();

                this.HttpContext.Cache.Add(cacheKey, allPosts, null, DateTime.Now.AddMinutes(5), Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
            }

            return allPosts;
        }
    }
}