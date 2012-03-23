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
        private DomainModel.TeamThingContext context;
        public TeamController()
        {
            this.context = new DomainModel.TeamThingContext();
        }

        [HttpGet]
        public IQueryable<ServiceModel.Team> Get()
        {
            return context.GetAll<TeamThing.Model.Team>().MapToServiceModel();
        }

        [HttpGet]
        public ServiceModel.Team Get(int id)
        {
            return context.GetAll<TeamThing.Model.Team>()
                          .First(t => t.Id == id)
                          .MapToServiceModel();
        }

        [HttpGet]
        public IEnumerable<ServiceModel.Thing> Things(int id)
        {
            var team = context.GetAll<TeamThing.Model.Team>()
                              .First(t => t.Id == id);

            var teamThings = team.TeamMembers
                                 .Where(tm=>tm.Status == DomainModel.TeamUserStatus.Approved)
                                 .SelectMany(tm => tm.User.Things)
                                 .Distinct()
                                 .ToList();

            return teamThings.MapToServiceModel();
        }

        [HttpPost]
        public HttpResponseMessage Post(ServiceModel.AddTeamViewModel addTeamViewModel)
        {
            if (!ModelState.IsValid)
            {
                return new HttpResponseMessage<JsonValue>(ModelState.ToJson(), HttpStatusCode.BadRequest);
            }

            var teamCreator = context.GetAll<DomainModel.User>()
                              .FirstOrDefault(u => u.Id == addTeamViewModel.CreatedById);

            if (teamCreator == null)
            {
                throw new HttpResponseException("Invalid Creator", HttpStatusCode.NotFound);
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

        
        public void PutTeam(int id, ServiceModel.Team viewModel)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(ModelState.ToJson().ToString(), HttpStatusCode.BadRequest);
            }

            var team = context.GetAll<DomainModel.Team>()
                              .FirstOrDefault(u => u.Id == viewModel.Id);

            if (team == null)
            {
                throw new HttpResponseException("Invalid Team", HttpStatusCode.BadRequest);
            }

            //var editor = team.TeamMembers
            //                 .Where(tm=>tm.Role== DomainModel.TeamUserRole.Administrator && tm.UserId == viewModel.UpdatedById);

            //if (editor == null)
            //{
            //    throw new HttpResponseException("User does not have permissions to edit team", HttpStatusCode.NotFound);
            //}

            team.Name = viewModel.Name;
            team.IsOpen = viewModel.IsPublic;
            
            context.SaveChanges();

            //var sTeam = team.MapToServiceModel();
            //var response = new HttpResponseMessage<ServiceModel.Team>(sTeam, HttpStatusCode.OK);
            //response.Headers.Location = new Uri(Request.RequestUri, "/api/team/" + sTeam.Id.ToString());
            //return response;
        }

        [HttpDelete]
        public HttpResponseMessage DeleteTeam(int id, ServiceModel.UpdateTeamViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return new HttpResponseMessage<JsonValue>(ModelState.ToJson(), HttpStatusCode.BadRequest);
            }

            var team = context.GetAll<DomainModel.Team>()
                              .FirstOrDefault(u => u.Id == viewModel.Id);

            //rest spec says we should not throw an error in this case ( delete requests should be idempotent)
            //if (team == null)
            //{
            //    throw new HttpResponseException("Invalid Team", HttpStatusCode.BadRequest);
            //}

            var editor = team.TeamMembers
                             .Where(tm => tm.Role == DomainModel.TeamUserRole.Administrator && tm.UserId == viewModel.UpdatedById);

            if (editor == null)
            {
                throw new HttpResponseException("User does not have permissions to edit team", HttpStatusCode.NotFound);
            }

            
            context.Delete(team);
            context.SaveChanges();

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

        [HttpPut]
        public HttpResponseMessage Join(ServiceModel.JoinTeamViewModel joinTeamViewModel)
        {
            if (!ModelState.IsValid)
            {
                return new HttpResponseMessage<JsonValue>(ModelState.ToJson(), HttpStatusCode.BadRequest);
            }

            var team = context.GetAll<DomainModel.Team>()
                              .FirstOrDefault(u => u.Name.Equals(joinTeamViewModel.Name, StringComparison.OrdinalIgnoreCase));

            if (team == null)
            {
                throw new HttpResponseException("Invalid Team", HttpStatusCode.BadRequest);
            }

            var user = context.GetAll<DomainModel.User>()
                              .FirstOrDefault(u => u.Id == joinTeamViewModel.UserId);

            if (user == null)
            {
                throw new HttpResponseException("Invalid User", HttpStatusCode.BadRequest);
            }

            if (user.Teams.Any(ut=>ut.TeamId== team.Id))
            {
                throw new HttpResponseException("User already added to team", HttpStatusCode.BadRequest);
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

            try
            {
                teamMember.Status = DomainModel.TeamUserStatus.Approved;
            }
            catch(Exception ex)
            {
                throw new HttpResponseException(ex.Message, HttpStatusCode.NotFound);
            }

            context.SaveChanges();
        }

        [HttpPut]
        public void DenyMember(int teamId, int userId)
        {
            var team = context.GetAll<DomainModel.Team>()
                              .FirstOrDefault(u => u.Id == teamId);

            if (team == null)
            {
                throw new HttpResponseException("Invalid Team", HttpStatusCode.BadRequest);
            }

            var teamMember = team.TeamMembers.FirstOrDefault(t => t.UserId == userId);

            if (teamMember == null)
            {
                throw new HttpResponseException("Invalid Team Member", HttpStatusCode.BadRequest);
            }

            if (team.OwnerId == teamMember.UserId)
            {
                throw new HttpResponseException("Can not deny access to the team owner", HttpStatusCode.BadRequest);
            }

            try
            {
                teamMember.Status = DomainModel.TeamUserStatus.Denyed;
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(ex.Message, HttpStatusCode.BadRequest);
            }

            context.SaveChanges();
        }
    }
}