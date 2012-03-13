using System.Web.Mvc;

namespace BloggerViewController.Website.Controllers {
    public class InfoController : BaseController {
        public ActionResult Archive() {
            var service = GetBlogService();

            var info = service.GetInfo();

            return PartialView("Archive", info);
        }

        public ActionResult Labels() {
            var service = GetBlogService();

            var info = service.GetInfo();

            return PartialView("Labels", info);
        }
    }
}