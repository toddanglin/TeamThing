using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.OpenAccess;

namespace Mercury.Dash.Mvc.Controllers
{
    public abstract class BaseController : Controller
    {
        public IObjectScope ObjectScope
        {
            get;
            private set;
        }

        /// <summary>
        /// This is fired right before the execue event
        /// </summary>
        protected virtual void PreExecute(IObjectScope objectScope)
        {

        }

        protected override void Execute(System.Web.Routing.RequestContext requestContext)
        {
            //grab scope
            ObjectScope = (IObjectScope)requestContext.HttpContext.Items[Mercury.Dash.Mvc.MvcApplication.SCOPE_KEY];

            PreExecute(ObjectScope);

            base.Execute(requestContext);
        }

    }
}
