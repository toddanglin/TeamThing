using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Mvc.Mailer;
using TeamThing.Model.Helpers;
using TeamThing.Web.Core.Helpers;
using TeamThing.Web.Core.Mappers;
using TeamThing.Web.Mailers;
using DomainModel = TeamThing.Model;
using ServiceModel = TeamThing.Web.Models.API;

namespace TeamThing.Web.Controllers
{
    //[Authorize]
    //[RequireOAuthAuthorization]
    public class ThingController : TeamThingApiController
    {
        private IUserMailer emailService = new UserMailer();
        public ThingController()
            : base(new DomainModel.TeamThingContext())
        {
        }

        [Queryable]
        public IQueryable<ServiceModel.Thing> Get()
        {
            //TODO: We chould grab the current user here probably?
            return context.GetAll<DomainModel.Thing>().MapToServiceModel();
        }

        public HttpResponseMessage Get(int id)
        {
            //TODO: We chould grab the current user here probably?
            var thing = context.GetAll<DomainModel.Thing>().FirstOrDefault(t => t.Id == id);

            if (thing == null)
            {
                ModelState.AddModelError("", "Invalid Thing");
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.ToJson());
            }

            var sThing = thing.MapToServiceModel();
            var response = Request.CreateResponse(HttpStatusCode.OK, sThing);
            response.Headers.Location = new Uri(Request.RequestUri, "/api/thing/" + thing.Id.ToString());
            return response;
        }

        public HttpResponseMessage Post(ServiceModel.AddThingViewModel newThing)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.ToJson());
            }

            var thingCreator = context.GetAll<DomainModel.User>()
                                      .FirstOrDefault(u => u.Id == newThing.CreatedById);

            if (thingCreator == null)
            {
                ModelState.AddModelError("", "Invalid Creator");
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.ToJson());
            }

            var team = context.GetAll<DomainModel.Team>()
                              .FirstOrDefault(u => u.Id == newThing.TeamId);

            if (team == null)
            {
                ModelState.AddModelError("", "Invalid Team");
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.ToJson());
            }

            var thing = new DomainModel.Thing(team, thingCreator);
            thing.Description = newThing.Description;

            foreach (var userId in newThing.AssignedTo)
            {
                var assignedTo = context.GetAll<DomainModel.User>()
                                        .FirstOrDefault(u => u.Id == userId);

                if (assignedTo == null)
                {
                    ModelState.AddModelError("", "Invalid User Assigned to Thing");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.ToJson());
                }

                thing.AssignedTo.Add(new DomainModel.UserThing(thing, assignedTo, thingCreator));
            }

            emailService.ThingAssigned(thing.AssignedTo.Select(x => x.AssignedToUser).ToArray(), thing, thingCreator).Send();

            context.Add(thing);
            context.SaveChanges();

            var sThing = thing.MapToServiceModel();
            var response = Request.CreateResponse(HttpStatusCode.Created, sThing);
            response.Headers.Location = new Uri(Request.RequestUri, "/api/thing/" + thing.Id.ToString());
            return response;
        }

        [HttpPut]
        public HttpResponseMessage UpdateStatus(int id, ServiceModel.UpdateThingStatusViewModel viewModel)
        {
            var thing = context.GetAll<DomainModel.Thing>()
                            .FirstOrDefault(u => u.Id == id);

            DomainModel.ThingStatus realStatus;

            if (!Enum.TryParse<DomainModel.ThingStatus>(viewModel.Status, true, out realStatus))
            {
                ModelState.AddModelError("", "Invalid Status");
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.ToJson());
            }

            if (thing == null)
            {
                ModelState.AddModelError("", "Invalid Thing");
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.ToJson());
            }

            var user = context.GetAll<DomainModel.User>()
                             .FirstOrDefault(u => u.Id == viewModel.UserId);

            if (user == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound, "Invalid user"));
            }

            if (thing.OwnerId != user.Id && !thing.AssignedTo.Any(at => at.AssignedToUserId == user.Id) && !thing.Team.Members.Admins().Any(a => a.Id == user.Id))
            {
                ModelState.AddModelError("", "A thing's status can only be completed by someone assigned to it, the thing's owner, or a team administrator.");
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.ToJson());
            }

            thing.UpdateStatus(user, realStatus);
            context.SaveChanges();

            if (thing.Status == DomainModel.ThingStatus.Completed)
            {
                emailService.ThingCompleted(thing.AssignedTo.Select(x => x.AssignedToUser).ToArray(), user, thing).Send();
            }

            var sThing = thing.MapToServiceModel();
            var response = Request.CreateResponse(HttpStatusCode.OK, sThing);
            response.Headers.Location = new Uri(Request.RequestUri, "/api/thing/" + thing.Id.ToString());
            return response;
        }

        [HttpPut]
        public HttpResponseMessage Complete(int id, ServiceModel.CompleteThingViewModel viewModel)
        {

            var thing = context.GetAll<DomainModel.Thing>()
                               .FirstOrDefault(u => u.Id == id);

            if (thing == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound, "Invalid thing"));
            }

            var user = context.GetAll<DomainModel.User>()
                              .FirstOrDefault(u => u.Id == viewModel.UserId);

            if (user == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound, "Invalid user"));
            }

            if (thing.OwnerId != user.Id && !thing.AssignedTo.Any(at => at.AssignedToUserId == user.Id) && !thing.Team.Members.Admins().Any(a => a.Id == user.Id))
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.Forbidden, "A thing can only be completed by someone assigned to it, the thing's owner, or a team administrator."));
            }

            thing.Complete(user);
            context.SaveChanges();

            emailService.ThingCompleted(thing.AssignedTo.Select(x => x.AssignedToUser).ToArray(), user, thing).Send();

            var sThing = thing.MapToServiceModel();
            var response = Request.CreateResponse(HttpStatusCode.OK, sThing);
            response.Headers.Location = new Uri(Request.RequestUri, "/api/thing/" + thing.Id.ToString());
            return response;
        }

        [HttpPut]
        public HttpResponseMessage Star(int id, ServiceModel.StarThingViewModel viewModel)
        {
            var thing = context.GetAll<DomainModel.Thing>()
                               .FirstOrDefault(u => u.Id == id);

            if (thing == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound, "Invalid thing"));
            }

            var user = context.GetAll<DomainModel.User>()
                              .FirstOrDefault(u => u.Id == viewModel.UserId);

            if (user == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound, "Invalid user"));
            }

            user.StarredThings.Add(thing);

            context.SaveChanges();

            var sThing = thing.MapToServiceModel();
            var response = Request.CreateResponse(HttpStatusCode.OK, sThing);
            response.Headers.Location = new Uri(Request.RequestUri, "/api/thing/" + thing.Id.ToString());
            return response;
        }

        [HttpPut]
        public HttpResponseMessage Unstar(int id, ServiceModel.StarThingViewModel viewModel)
        {
            var thing = context.GetAll<DomainModel.Thing>()
                               .FirstOrDefault(u => u.Id == id);

            if (thing == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound, "Invalid thing"));
            }

            var user = context.GetAll<DomainModel.User>()
                              .FirstOrDefault(u => u.Id == viewModel.UserId);

            if (user == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound, "Invalid user"));
            }

            user.StarredThings.Remove(thing);

            context.SaveChanges();

            var sThing = thing.MapToServiceModel();
            var response = Request.CreateResponse(HttpStatusCode.OK, sThing);
            response.Headers.Location = new Uri(Request.RequestUri, "/api/thing/" + thing.Id.ToString());
            return response;
        }



        [HttpPut]
        public HttpResponseMessage Put(int id, ServiceModel.UpdateThingViewModel viewModel)
        {
            var thingEditor = context.GetAll<DomainModel.User>()
                                     .FirstOrDefault(u => u.Id == viewModel.EditedById);

            var thing = context.GetAll<DomainModel.Thing>()
                               .FirstOrDefault(u => u.Id == id);

            if (thingEditor == null)
            {
                ModelState.AddModelError("", "Invalid Editor");
            }
            if (thing == null)
            {
                ModelState.AddModelError("", "Invalid Thing");
            }
            if (thing != null && thing.OwnerId != thingEditor.Id)
            {
                ModelState.AddModelError("", "A Thing can only be edited by its owner");
            }

            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.ToJson());
            }

            foreach (var userId in viewModel.AssignedTo)
            {
                //already assigned
                if (thing.AssignedTo.Any(at => at.AssignedToUserId == userId)) continue;


                var assignedTo = context.GetAll<DomainModel.User>()
                                          .FirstOrDefault(u => u.Id == userId);

                if (assignedTo == null)
                {
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound, "Invalid User Assigned to Thing"));
                }

                thing.AssignedTo.Add(new DomainModel.UserThing(thing, assignedTo, thingEditor));
            }

            //removed users
            var removedUserIds = thing.AssignedTo.Select(at => at.AssignedToUserId).Except(viewModel.AssignedTo);
            var addedUserIds = viewModel.AssignedTo.Except(thing.AssignedTo.Select(at => at.AssignedToUserId));

            var removedUserThings = thing.AssignedTo.Where(at => removedUserIds.Contains(at.AssignedToUserId)).ToList();
            var newUserThings = thing.AssignedTo.Where(at => addedUserIds.Contains(at.AssignedToUserId)).ToList();

            context.Delete(removedUserThings);

            thing.Description = viewModel.Description;

            context.SaveChanges();

            emailService.ThingAssigned(newUserThings.Select(x => x.AssignedToUser).ToArray(), thing, thingEditor).Send();
            emailService.ThingUnassigned(newUserThings.Select(x => x.AssignedToUser).ToArray(), thing, thingEditor).Send();

            var sThing = thing.MapToServiceModel();
            var response = Request.CreateResponse(HttpStatusCode.OK, sThing);
            response.Headers.Location = new Uri(Request.RequestUri, "/api/thing/" + thing.Id.ToString());
            return response;
        }

        [HttpDelete]
        public HttpResponseMessage Delete(int id, ServiceModel.DeleteThingViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.ToJson());
            }

            var thing = context.GetAll<DomainModel.Thing>()
                               .FirstOrDefault(u => u.Id == id);

            //rest spec says we should not throw an error in this case ( delete requests should be idempotent)
            if (thing == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Thing"));
            }


            var user = context.GetAll<DomainModel.User>()
                             .FirstOrDefault(u => u.Id == viewModel.DeletedById);

            if (user == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound, "Invalid user"));
            }


            if (thing.OwnerId != viewModel.DeletedById)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, "A thing can only be removed by its owner."));
            }

            thing.Delete(user);
            context.SaveChanges();

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }
    }
}
