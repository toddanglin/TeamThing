using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TeamThing.Web.Core.Mappers;
using DomainModel = TeamThing.Model;
using ServiceModel = TeamThing.Web.Models.API;
using System.Json;
using TeamThing.Web.Core.Helpers;

namespace TeamThing.Web.Controllers
{
    public class TeamController : ApiController
    {
        private readonly DomainModel.TeamThingContext context;
        public TeamController()
        {
            this.context = new DomainModel.TeamThingContext();
        }

        [HttpGet]
        public IQueryable<ServiceModel.TeamBasic> Get()
        {
            return context.GetAll<TeamThing.Model.Team>().MapToBasicServiceModel();
        }

        [HttpGet]
        public HttpResponseMessage Get(int id)
        {
            var item = context.GetAll<TeamThing.Model.Team>()
                              .FirstOrDefault(t => t.Id == id);
            if (item == null)
            {
                ModelState.AddModelError("", "Invalid Team");
                return new HttpResponseMessage<JsonValue>(ModelState.ToJson(), HttpStatusCode.BadRequest);
            }

            var sTeam = item.MapToServiceModel();
            var response = new HttpResponseMessage<ServiceModel.Team>(sTeam, HttpStatusCode.OK);
            response.Headers.Location = new Uri(Request.RequestUri, "/api/team/" + sTeam.Id.ToString());
            return response;
        }

        [HttpPost]
        public HttpResponseMessage Post(ServiceModel.AddTeamViewModel addTeamViewModel)
        {
            if (!ModelState.IsValid)
            {
                return new HttpResponseMessage<JsonValue>(ModelState.ToJson(), HttpStatusCode.BadRequest);
            }

            var existingTeam = context.GetAll<DomainModel.Team>()
                                    .FirstOrDefault(u => u.Name.Equals(addTeamViewModel.Name));

            if (existingTeam != null)
            {
                ModelState.AddModelError("", "Team name already in use");
                return new HttpResponseMessage<JsonValue>(ModelState.ToJson(), HttpStatusCode.BadRequest);
            }

            var teamCreator = context.GetAll<DomainModel.User>()
                                     .FirstOrDefault(u => u.Id == addTeamViewModel.CreatedById);

            if (teamCreator == null)
            {
                ModelState.AddModelError("", "Invalid Team Owner Specified");
                return new HttpResponseMessage<JsonValue>(ModelState.ToJson(), HttpStatusCode.BadRequest);
            }

            var team = new DomainModel.Team(addTeamViewModel.Name, teamCreator);
            team.IsOpen = addTeamViewModel.IsPublic;
            context.Add(team);
            context.SaveChanges();

            var sTeam = team.MapToBasicServiceModel();
            var response = new HttpResponseMessage<ServiceModel.TeamBasic>(sTeam, HttpStatusCode.Created);
            response.Headers.Location = new Uri(Request.RequestUri, "/api/team/" + sTeam.Id.ToString());
            return response;
        }

        [HttpPut]
        public HttpResponseMessage Put(ServiceModel.UpdateTeamViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(ModelState.ToJson().ToString(), HttpStatusCode.BadRequest);
            }

            var existingTeam = context.GetAll<DomainModel.Team>()
                                  .FirstOrDefault(u => u.Name.Equals(viewModel.Name) && u.Id != viewModel.Id);

            if (existingTeam != null)
            {
                ModelState.AddModelError("", "Team name already in use");
                return new HttpResponseMessage<JsonValue>(ModelState.ToJson(), HttpStatusCode.BadRequest);
            }

            var team = context.GetAll<DomainModel.Team>()
                              .FirstOrDefault(u => u.Id == viewModel.Id);

            if (team == null)
            {
                ModelState.AddModelError("", "Invalid team edited");
                return new HttpResponseMessage<JsonValue>(ModelState.ToJson(), HttpStatusCode.BadRequest);
            }

            var editor = team.TeamMembers
                             .FirstOrDefault(tm => tm.Role == DomainModel.TeamUserRole.Administrator && tm.UserId == viewModel.UpdatedById);

            if (editor == null)
            {
                ModelState.AddModelError("", "User does not have permissions to edit team");
                return new HttpResponseMessage<JsonValue>(ModelState.ToJson(), HttpStatusCode.BadRequest);
            }

            team.Name = viewModel.Name;
            team.IsOpen = viewModel.IsPublic;

            context.SaveChanges();

            var sTeam = team.MapToServiceModel();
            var response = new HttpResponseMessage<ServiceModel.Team>(sTeam, HttpStatusCode.OK);
            response.Headers.Location = new Uri(Request.RequestUri, "/api/team/" + sTeam.Id.ToString());
            return response;
        }

        [HttpDelete]
        public HttpResponseMessage Delete(int id, int userId)
        {
            if (!ModelState.IsValid)
            {
                return new HttpResponseMessage<JsonValue>(ModelState.ToJson(), HttpStatusCode.BadRequest);
            }

            var team = context.GetAll<DomainModel.Team>()
                              .FirstOrDefault(u => u.Id == id);

            //rest spec says we should not throw an error in this case ( delete requests should be idempotent)
            //if (team == null)
            //{
            //    throw new HttpResponseException("Invalid Team", HttpStatusCode.BadRequest);
            //}

            var editor = team.TeamMembers
                             .FirstOrDefault(tm => tm.Role == DomainModel.TeamUserRole.Administrator && tm.UserId == userId);

            if (editor == null)
            {
                ModelState.AddModelError("", "User does not have permissions to edit team");
                return new HttpResponseMessage<JsonValue>(ModelState.ToJson(), HttpStatusCode.BadRequest);
            }

            context.Delete(team);
            context.SaveChanges();

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

        [HttpPut]
        public HttpResponseMessage Join(int id, ServiceModel.JoinTeamViewModel joinTeamViewModel)
        {
            if (!ModelState.IsValid)
            {
                return new HttpResponseMessage<JsonValue>(ModelState.ToJson(), HttpStatusCode.BadRequest);
            }

            var team = context.GetAll<DomainModel.Team>()
                              .FirstOrDefault(u => u.Id == joinTeamViewModel.Id);

            if (team == null)
            {
                ModelState.AddModelError("", "Invalid Team");
                return new HttpResponseMessage<JsonValue>(ModelState.ToJson(), HttpStatusCode.BadRequest);
            }

            var user = context.GetAll<DomainModel.User>()
                              .FirstOrDefault(u => u.Id == joinTeamViewModel.UserId);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid User");
                return new HttpResponseMessage<JsonValue>(ModelState.ToJson(), HttpStatusCode.BadRequest);
            }

            if (user.Teams.Any(ut => ut.TeamId == team.Id))
            {
                ModelState.AddModelError("", "User already added to team");
                return new HttpResponseMessage<JsonValue>(ModelState.ToJson(), HttpStatusCode.BadRequest);
            }

            var newTeamMember = new DomainModel.TeamUser(team, user);
            if (team.IsOpen)
            {
                newTeamMember.Status = DomainModel.TeamUserStatus.Approved;
            }
            team.TeamMembers.Add(newTeamMember);
            context.SaveChanges();

            var sTeam = team.MapToServiceModel();
            var response = new HttpResponseMessage<ServiceModel.Team>(sTeam, HttpStatusCode.OK);
            response.Headers.Location = new Uri(Request.RequestUri, "/api/team/" + sTeam.Id.ToString());
            return response;
        }

        [HttpPut]
        public void ApproveMember(int teamId, int userId)
        {
            var team = context.GetAll<DomainModel.Team>()
                              .FirstOrDefault(u => u.Id == teamId);

            if (team == null)
            {
                throw new HttpResponseException("Invalid Team", HttpStatusCode.NotFound);
            }

            var teamMember = team.TeamMembers.FirstOrDefault(t => t.UserId == userId);

            if (teamMember == null)
            {
                throw new HttpResponseException("Invalid Team Member", HttpStatusCode.NotFound);
            }

            teamMember.Status = DomainModel.TeamUserStatus.Approved;
            context.SaveChanges();
        }

        [HttpPut]
        public void DenyMember(int id, int userId)
        {
            var team = context.GetAll<DomainModel.Team>()
                              .FirstOrDefault(u => u.Id == id);

            if (team == null)
            {
                throw new HttpResponseException("Invalid Team", HttpStatusCode.NotFound);
            }

            var teamMember = team.TeamMembers.FirstOrDefault(t => t.UserId == userId);

            if (teamMember == null)
            {
                throw new HttpResponseException("Invalid Team Member", HttpStatusCode.NotFound);
            }

            if (team.OwnerId == teamMember.UserId)
            {
                throw new HttpResponseException("Can not deny access to the team owner", HttpStatusCode.BadRequest);
            }

            teamMember.Status = DomainModel.TeamUserStatus.Denyed;
            context.SaveChanges();
        }
    }
}