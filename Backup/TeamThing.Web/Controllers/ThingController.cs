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
        public HttpResponseMessage Get(int id)
        {
            //TODO: We chould grab the current user here probably?
            var thing = context.GetAll<DomainModel.Thing>().FirstOrDefault(t => t.Id == id);

            if (thing == null)
            {
                ModelState.AddModelError("", "Invalid Thing");
                return new HttpResponseMessage<JsonValue>(ModelState.ToJson(), HttpStatusCode.BadRequest);
            }

            var sThing = thing.MapToServiceModel();
            var response = new HttpResponseMessage<ServiceModel.Thing>(sThing, HttpStatusCode.OK);
            response.Headers.Location = new Uri(Request.RequestUri, "/api/thing/" + thing.Id.ToString());
            return response;
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

            var team = context.GetAll<DomainModel.Team>()
                                      .FirstOrDefault(u => u.Id == newThing.TeamId);

            if (team == null)
            {
                throw new HttpResponseException("Invalid Team", HttpStatusCode.NotFound);
            }

            
            var thing = new DomainModel.Thing(team, thingCreator);
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

        [HttpPut]
        public HttpResponseMessage Complete(int id, int userId)
        {

            var thing = context.GetAll<DomainModel.Thing>()
                               .FirstOrDefault(u => u.Id == id);

            //rest spec says we should not throw an error in this case ( delete requests should be idempotent)
            if (thing == null)
            {
                throw new HttpResponseException("Invalid Thing", HttpStatusCode.BadRequest);
            }

            if (!thing.AssignedTo.Any(at=>at.AssignedToUserId == userId))
            {
                throw new HttpResponseException("A thing can only be removed by its owner.", HttpStatusCode.BadRequest);
            }

            thing.Complete(userId);
            context.SaveChanges();

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

        // PUT /api/thing/5
        [HttpPut]
        public void Put(ServiceModel.UpdateThingViewModel viewModel)
        {
        }

        // DELETE /api/thing/5
        [HttpDelete]
        public HttpResponseMessage Delete(ServiceModel.DeleteThingViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return new HttpResponseMessage<JsonValue>(ModelState.ToJson(), HttpStatusCode.BadRequest);
            }

            var thing = context.GetAll<DomainModel.Thing>()
                               .FirstOrDefault(u => u.Id == viewModel.Id);

            //rest spec says we should not throw an error in this case ( delete requests should be idempotent)
            if (thing == null)
            {
                throw new HttpResponseException("Invalid Thing", HttpStatusCode.BadRequest);
            }

            if (thing.OwnerId != viewModel.DeletedById)
            {
                throw new HttpResponseException("A thing can only be removed by its owner.", HttpStatusCode.BadRequest);
            }

            thing.Delete(viewModel.DeletedById);
            context.SaveChanges();

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }
    }
}
