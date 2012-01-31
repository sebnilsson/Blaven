using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BloggerViewController.Website.Controllers {
    public class BlogController : Controller {
        //
        // GET: /Main/

        public ActionResult Index(int? page) {
            ViewBag.PageIndex = page.GetValueOrDefault(1);

            var service = new BlogService("", "", "");
            var entries = service.GetBlog();
            
            return View(entries);
        }
    }
}