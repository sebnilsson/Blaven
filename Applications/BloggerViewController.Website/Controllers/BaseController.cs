using System.Web;
using System.Web.Mvc;

namespace BloggerViewController.Website.Controllers {
    public abstract class BaseController : Controller {
        protected override void Initialize(System.Web.Routing.RequestContext requestContext) {
            base.Initialize(requestContext);

            string requestUrl = HttpContext.Request.AppRelativeCurrentExecutionFilePath;
            string requestUrlLower = requestUrl.ToLowerInvariant();
            if(requestUrl != requestUrlLower) {
                HttpContext.Response.Redirect(requestUrlLower, true);
            }
        }

        public ViewResult ErrorView(object model) {
            return View("Error", model);
        }

        internal BlogService GetBlogService() {
            var service = new BlogService(new BloggerViewController.Data.MemoryBlogStore());
            return service;
        }
    }
}
