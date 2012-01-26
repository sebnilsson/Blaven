using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BloggerViewController.Website.Controllers
{
    public class BlogController : Controller
    {
        //
        // GET: /Main/

        public ActionResult Index()
        {
            return View();
        }

    }
}
