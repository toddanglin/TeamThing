using System;
using System.Linq;
using System.Web.Http;
using TeamThing.Model;

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
        protected User GetCurrentUser()
        {
            try
            {
                var userName = User.Identity.Name;
                if (userName != null)
                {
                    return context.GetAll<User>().FirstOrDefault(u => u.EmailAddress == userName);
                }
                return null;
            }
            catch (Exception ex)
            {
                //TODO: log error?
                return null;
            }
        }
    }
}
