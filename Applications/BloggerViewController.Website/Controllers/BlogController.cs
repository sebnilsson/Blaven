using System.Web.Mvc;

using BloggerViewController.Website.Models;

namespace BloggerViewController.Website.Controllers {
    public class BlogController : Controller {
        public ActionResult Index(int? page = 1) {
            int pageIndex = page.GetValueOrDefault(1) - 1; // Given pageIndex is user-friendly, not 0-based

            var service = new BlogService();

            var info = service.GetInfo();
            var selection = service.GetSelection(pageIndex, BlogConfigurationHelper.PageSize);

            var model = new BlogListViewModel { Info = info, Selection = selection, PageIndex = page.GetValueOrDefault(1), };

            return View(model);
        }

        public ActionResult Post(string id) {
            var service = new BlogService();

            var info = service.GetInfo();
            var post = service.GetPost(id);

            var model = new BlogSingleViewModel { Info = info, Post = post, };

            return View(model);
        }
    }
}