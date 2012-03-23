using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace TeamThing.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            //routes.MapHttpRoute(
            //    name: "TeamThings",
            //    routeTemplate: "api/team/{id}/things",
            //    defaults: new { controller = "Team", action = "GetThings" }
            //);
            routes.MapHttpRoute(
                name: "SingleResourceApi",
                routeTemplate: "api/{controller}/{id}/{action}",
                defaults: new { action="get" },
                constraints: new { id = @"\d+" }
            );

            routes.MapHttpRoute(
                name: "ResourceApi",
                routeTemplate: "api/{controller}/{action}"
            );

            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}"
            );

            routes.MapRoute(
                name: "Default",
                url: "index.html",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            BundleTable.Bundles.RegisterTemplateBundles();
        }
    }
}