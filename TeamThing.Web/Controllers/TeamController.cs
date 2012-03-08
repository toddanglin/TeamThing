using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TeamThing.Model;

namespace TeamThing.Web.Controllers
{
    public class TeamController : ApiController
    {
        private TeamThingContext context;

        public TeamController()
        {
            this.context = new TeamThingContext();
        }

        public IQueryable<Team> Get()
        {
            return context.GetAll<Team>();
        }

        // GET /api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST /api/values
        public void Post(string value)
        {
        }

        // PUT /api/values/5
        public void Put(int id, string value)
        {
        }

        // DELETE /api/values/5
        public void Delete(int id)
        {
        }
    }
}