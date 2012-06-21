using System;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Management;
using System.Web.Optimization;
using System.Web.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace TeamThing.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class WebApiApplication : System.Web.HttpApplication
    {
        public const string FACEBOOK_APP_ID = "384951088223271";
        public const string FACEBOOK_APP_SECRET = "6f53e4d257507ee2ba6acc2690f09ce3";

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("mobile/{*pathInfo}");

            //routes.MapHttpRoute(
            //    name: "TeamThings",
            //    routeTemplate: "api/team/{id}/things",
            //    defaults: new { controller = "Team", action = "GetThings" }
            //);

            routes.MapHttpRoute(
                name: "PutApi",
                routeTemplate: "api/{controller}/{id}/{action}",
                defaults: new { Action = "Put" },
                constraints: new { httpMethod = new HttpMethodConstraint(new string[] { "PUT" }) });
            routes.MapHttpRoute(
                name: "SingleResourceApi",
                routeTemplate: "api/{controller}/{id}/{action}/{status}",
                defaults: new { status = RouteParameter.Optional },
                constraints: new { id = @"\d+" });

            routes.MapHttpRoute(
                name: "HeaderBasedApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { },
                constraints: new { id = @"\d+" });

            routes.MapHttpRoute(
                name: "ResourceApi",
                routeTemplate: "api/{controller}/{action}");

            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}");

            ////routes.MapHttpRoute(
            ////    name: "TeamThings",
            ////    routeTemplate: "api/team/{id}/things",
            ////    defaults: new { controller = "Team", action = "GetThings" }
            ////);
            //routes.MapHttpRoute(
            //    name: "SingleResourceApi",
            //    routeTemplate: "api/{controller}/{id}/{action}",
            //    defaults: new { },
            //    constraints: new { id = @"\d+" }
            //);

            //routes.MapHttpRoute(
            //    name: "HttpMethodBasedResourceApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { },
            //    constraints: new { id = @"\d+" }
            //);

            //routes.MapHttpRoute(
            //    name: "ResourceApi",
            //    routeTemplate: "api/{controller}/{action}",
            //    defaults: new { action = "get" }
            //);

            ////routes.MapHttpRoute(
            ////    name: "DefaultApi",
            ////    routeTemplate: "api/{controller}/{id}",
            ////    defaults: new { id = RouteParameter.Optional }
            ////);

            //routes.MapRoute(
            //    name: "Default",
            //    url: "index.html",
            //    defaults: new { controller="Home", action="Index", id=UrlParameter.Optional });

            RegisterApis(GlobalConfiguration.Configuration);

            //Set error reporting policy for Web API (which doesn't use Web.Config settings)
            GlobalConfiguration.Configuration.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (Context.Request.FilePath == "/")
            {
                Response.Redirect("/mobile/index.html", true);
            }
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            BundleTable.Bundles.EnableDefaultBundles();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();

            //Log errors in a way that is visible on AppHarbor
            HttpException httpException = exception as HttpException;
            if (httpException != null)
            {
                var errLog = new LogEvent(httpException);
                errLog.Raise();
            }
            else
            {
                var errLog = new LogEvent(exception);
                errLog.Raise();
            }

            // clear error on server
            Server.ClearError();
        }

        public static void RegisterApis(HttpConfiguration config)
        {
            // Add JavaScriptSerializer  formatter instead - add at top to make default
            //config.Formatters.Insert(0, new JavaScriptSerializerFormatter());
            // Add Json.net formatter - add at the top so it fires first!
            // This leaves the old one in place so JsonValue/JsonObject/JsonArray still are handled
            config.Formatters.JsonFormatter.SerializerSettings =
            new JsonSerializerSettings
            {
                ContractResolver =
                new CamelCasePropertyNamesContractResolver()
            };
            //config.Formatters[0] = new JsonNetFormatter();
        }
    }

    public class LogEvent : WebRequestErrorEvent
    {
        public LogEvent(Exception ex)
            : base(null, null, 100001, ex)
        {
        }
    }
}