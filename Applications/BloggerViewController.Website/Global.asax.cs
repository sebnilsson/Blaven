using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BloggerViewController.Website {
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Post",
                "{year}/{month}/{title}",
                new { controller = "Blog", action = "Post", },
                new { year = @"\d+", month = @"\d+", }
            );

            routes.MapRoute(
                "Archive",
                "{year}/{month}",
                new { controller = "Blog", action = "Archive", },
                new { year = @"\d+", month = @"\d+", }
            );

            routes.MapRoute(
                "Label",
                "label/{labelName}",
                new { controller = "Blog", action = "Label", labelName = string.Empty, }
            );

            routes.MapRoute(
                "Services", // Route name
                "services/{action}/{id}", // URL with parameters
                new { controller = "Services", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
            
            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Blog", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "Index",
                "",
                new { controller = "Blog", action = "Index", id = UrlParameter.Optional }
            );

            //routes.MapRoute(
            //    "Default", // Route name
            //    "{controller}/{action}/{id}", // URL with parameters
            //    new { controller = "Blog", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            //);
        }

        protected void Application_Start() {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            var service = new BlogService(new BloggerViewController.Data.MemoryBlogStore());            
            service.UpdateBlog();
        }
    }
}