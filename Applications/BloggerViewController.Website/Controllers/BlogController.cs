using System;
using System.Linq;
using System.Web.Mvc;

using BloggerViewController.Website.Models;

namespace BloggerViewController.Website.Controllers {
    public class BlogController : BaseController {
        public ActionResult Archive(int year, int month, int? page = 1) {
            int pageIndex = page.GetValueOrDefault(1) - 1; // Given pageIndex is user-friendly, not 0-based

            var service = GetBlogService();

            var info = service.GetInfo();

            Func<BlogPost, bool> predicate = (post) => post.Published.Year == year && post.Published.Month == month;
            var selection = service.GetSelection(pageIndex, predicate: predicate);

            var model = new BlogListViewModel { Info = info, Selection = selection, PageIndex = page.GetValueOrDefault(1), };

            return View("List", model);
        }

        public ActionResult Index(int? page = 1) {
            int pageIndex = page.GetValueOrDefault(1) - 1; // Given pageIndex is user-friendly, not 0-based

            var service = GetBlogService();

            var info = service.GetInfo();
            var selection = service.GetSelection(pageIndex);

            var model = new BlogListViewModel { Info = info, Selection = selection, PageIndex = page.GetValueOrDefault(1), };

            return View("List", model);
        }

        public ActionResult Label(string labelName, int? page = 1) {
            int pageIndex = page.GetValueOrDefault(1) - 1; // Given pageIndex is user-friendly, not 0-based

            var service = GetBlogService();

            var info = service.GetInfo();

            Func<BlogPost, bool> predicate = (post) => post.Labels.Any(tag => tag.Equals(labelName, StringComparison.OrdinalIgnoreCase));
            var selection = service.GetSelection(pageIndex, predicate: predicate);

            var model = new BlogListViewModel { Info = info, Selection = selection, PageIndex = page.GetValueOrDefault(1), };

            return View("List", model);
        }

        public ActionResult Post(int year, int month, string title) {
            var service = GetBlogService();

            var info = service.GetInfo();

            string friendlyLink = string.Format("/{0}/{1}/{2}", year, month.ToString("00"), title);
            var post = service.GetPost(friendlyLink);

            if(post == null) {
                return View("Error");
            }
            
            var model = new BlogSingleViewModel { Info = info, Post = post, };

            return View(model);
        }
    }
}