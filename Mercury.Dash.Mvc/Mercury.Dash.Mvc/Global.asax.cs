using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Telerik.OpenAccess;
using System.Configuration;

namespace Mercury.Dash.Mvc
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static string SCOPE_KEY;
        private const string DEFAULT_SCOPE_KEY = "####MERCURY_OBJECT_SCOPE_KEY####";
        private const string WEB_CONFIG_SCOPE_KEY = "OpenAccessScopeKey";

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Dashboard", action = "Index", id = "" } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            //OpenAccessWatcher.Listeners.WatchTraceListener.Initialize(Telerik.OpenAccess.Diagnostics.TraceAdapter.Instance);

            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);

            //Try to get the scope key out of web config, if it doesnt exist set it to the default scope key constant.
            string scopeKey = ConfigurationManager.AppSettings[WEB_CONFIG_SCOPE_KEY];
            SCOPE_KEY = scopeKey ?? DEFAULT_SCOPE_KEY;
        }

        protected void Application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            WireUpObjectScope();
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            KillObjectScope();
        }

        private void KillObjectScope()
        {
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Handler is MvcHandler)
                {
                    if (HttpContext.Current.Items[SCOPE_KEY] != null)
                    {
                        IObjectScope scope = (IObjectScope)HttpContext.Current.Items[SCOPE_KEY];
                        if (scope.Transaction != null && scope.Transaction.IsActive)
                        {
                            scope.Transaction.Commit();
                        }

                        scope.Dispose();
                    }
                }
            }
        }

        private void WireUpObjectScope()
        {
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Handler is MvcHandler)
                {
                    if (HttpContext.Current.Items[SCOPE_KEY] == null)
                    {
                        IObjectScope scope = ObjectScopeProvider1.GetNewObjectScope();

                        scope.TransactionProperties.AutomaticBegin = true;

                        HttpContext.Current.Items.Add(SCOPE_KEY, scope);
                    }
                }
            }
        }
    }
}