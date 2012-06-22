using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TeamThing.Model;
using TeamThing.Web.Models.API;
using DomainModel = TeamThing.Model;
using ServiceModel = TeamThing.Web.Models.API;

namespace TeamThing.Web.Controllers
{
    public class TeamThingApiController : ApiController
    {
        protected readonly TeamThing.Model.TeamThingContext context;
        public TeamThingApiController(TeamThingContext context)
        {
            this.context = context;
        }

        [NonAction]
        protected DomainModel.User GetCurrentUser()
        {
            try
            {
                var userName = User.Identity.Name;
                if (userName != null)
                {
                    return context.GetAll<DomainModel.User>().FirstOrDefault(u => u.EmailAddress == userName);
                }
                return null;
            }
            catch (Exception ex)
            {
                //TODO: log error?
                return null;
            }
        }

        public HttpResponseMessage ResourceOkResponse<T>(T item) where T : IServiceResource
        {
            var controller = this.Request.GetRouteData().Values["controller"];
            var response = Request.CreateResponse(HttpStatusCode.OK, item);
            response.Headers.Location = new Uri(Request.RequestUri, string.Format("/api/{0}/{1}", controller, item.Id.ToString()));

            return response;
        }
    }
}
