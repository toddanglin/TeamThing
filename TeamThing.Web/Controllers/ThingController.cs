using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using TeamThing.Web.Core.Mappers;
using DomainModel = TeamThing.Model;
using ServiceModel = TeamThing.Web.Models.API;
using System.Net;
using System.Json;
using TeamThing.Web.Core.Helpers;

namespace TeamThing.Web.Controllers
{
    public class ThingController : ApiController
    {
        private DomainModel.TeamThingContext context;
        public ThingController()
        {
            this.context = new DomainModel.TeamThingContext();
        }

        // GET /api/thing/5
        public ServiceModel.Thing Get(int id)
        {
            //TODO: We chould grab the current user here probably?
            return context.GetAll<DomainModel.Thing>().FirstOrDefault(t => t.Id == id).MapToServiceModel();
        }

        // GET /api/thing
        public IQueryable<ServiceModel.Thing> Get()
        {
            //TODO: We chould grab the current user here probably?
            return context.GetAll<DomainModel.Thing>().MapToServiceModel();
        }

        // POST /api/thing
        public HttpResponseMessage Post(ServiceModel.AddThingViewModel newThing)
        {
            if (!ModelState.IsValid)
            {
                return new HttpResponseMessage<JsonValue>(ModelState.ToJson(), HttpStatusCode.BadRequest);
            }

            var thingCreator = context.GetAll<DomainModel.User>()
                                      .FirstOrDefault(u => u.Id == newThing.CreatedById);

            if (thingCreator == null)
            {
                throw new HttpResponseException("Invalid Creator", HttpStatusCode.NotFound);
            }

            
            var thing = new DomainModel.Thing(thingCreator);
            thing.Description = newThing.Description;

            foreach (var userId in newThing.AssignedTo)
            {
                var assignedTo = context.GetAll<DomainModel.User>()
                                          .FirstOrDefault(u => u.Id == userId);

                if (assignedTo == null)
                {
                    throw new HttpResponseException("Invalid User Assigned to Thing", HttpStatusCode.NotFound);
                }

                thing.AssignedTo.Add(new DomainModel.UserThing(thing, assignedTo, thingCreator));
            }

            context.Add(thing);
            context.SaveChanges();

            var sThing = thing.MapToServiceModel();
            var response = new HttpResponseMessage<ServiceModel.Thing>(sThing, HttpStatusCode.Created);
            response.Headers.Location = new Uri(Request.RequestUri, "/api/thing/" + thing.Id.ToString());
            return response;
        }

        // PUT /api/thing/5
        public void Put(int id, string value)
        {
        }

        // DELETE /api/thing/5
        public void Delete(int id)
        {
        }
    }
}
