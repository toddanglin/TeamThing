using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TeamThing.Model.Helpers;
using TeamThing.Web.Core.Mappers;
using DomainModel = TeamThing.Model;
using ServiceModel = TeamThing.Web.Models.API;
using TeamThing.Web.Core.Helpers;

namespace TeamThing.Web.Controllers
{
    //[Authorize]
    public class TeamController : TeamThingApiController
    {

        public TeamController()
            : base(new DomainModel.TeamThingContext())
        {
        }

        [Queryable]
        public IQueryable<ServiceModel.TeamBasic> Get()
        {
            return context.GetAll<TeamThing.Model.Team>().MapToBasicServiceModel();
        }

        public HttpResponseMessage Get(int id)
        {
            var item = context.GetAll<TeamThing.Model.Team>()
                              .FirstOrDefault(t => t.Id == id);
            if (item == null)
            {
                ModelState.AddModelError("", "Invalid Team");
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.ToJson());
            }

            var sTeam = item.MapToBasicServiceModel();
            var response = Request.CreateResponse(HttpStatusCode.OK, sTeam);
            response.Headers.Location = new Uri(Request.RequestUri, "/api/team/" + sTeam.Id.ToString());
            return response;
        }

        // GET /api/team/5/things/{status}
        [HttpGet]
        [Queryable]
        public IQueryable<ServiceModel.ThingBasic> Things(int id, string status)
        {
            //get user
            var team = context.GetAll<DomainModel.Team>().FirstOrDefault(t => t.Id == id);
            if (team == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound, "Invalid Team"));

            if (status != null)
            {
                return team.Things
                           .WithStatus(status)
                           .MapToBasicServiceModel()
                           .AsQueryable();
            }

            return team.Things
                       .Active()
                       .MapToBasicServiceModel()
                       .AsQueryable();
        }

        // GET /api/team/5/members/{status}
        [HttpGet]
        [Queryable]
        public IQueryable<ServiceModel.UserBasic> Members(int id, string status)
        {
            var team = context.GetAll<DomainModel.Team>().FirstOrDefault(t => t.Id == id);
            if (team == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound, "Invalid Team"));

            if (status != null)
            {
                return team.Members
                           .UsersWithStatus(status)
                           .MapToBasicServiceModel()
                           .AsQueryable();
            }

            return team.Members
                       .ActiveUsers()
                       .MapToBasicServiceModel()
                       .AsQueryable();
        }

        // GET /api/team/5/stats/{status}
        [HttpGet]
        [Queryable]
        public IQueryable<ServiceModel.UserStat> Stats(int id, string status)
        {

            var team = context.GetAll<DomainModel.Team>().FirstOrDefault(t => t.Id == id);
            if (team == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound, "Invalid Team"));

            var realStatus = DomainModel.ThingAction.Completed;

            return context.GetAll<DomainModel.ThingLog>()
                          .Where(t => t.Thing.TeamId == id && t.Action == realStatus)
                          .GroupBy(t => t.EditedBy)
                          .Select(t => new ServiceModel.UserStat() { User = t.Key.MapToBasicServiceModel(), ThingCount = t.Count() });

        }

        public HttpResponseMessage Post(ServiceModel.AddTeamViewModel addTeamViewModel)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.ToJson());
            }

            var existingTeam = context.GetAll<DomainModel.Team>()
                                    .FirstOrDefault(u => u.Name.Equals(addTeamViewModel.Name));

            if (existingTeam != null)
            {
                ModelState.AddModelError("", "Team name already in use");
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.ToJson());
            }

            var teamCreator = context.GetAll<DomainModel.User>()
                                     .FirstOrDefault(u => u.Id == addTeamViewModel.CreatedById);

            if (teamCreator == null)
            {
                ModelState.AddModelError("", "Invalid Team Owner Specified");
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.ToJson());
            }

            var team = new DomainModel.Team(addTeamViewModel.Name, teamCreator);
            team.IsOpen = addTeamViewModel.IsPublic;
            context.Add(team);
            context.SaveChanges();

            var sTeam = team.MapToBasicServiceModel();
            var response = Request.CreateResponse(HttpStatusCode.Created, sTeam);
            response.Headers.Location = new Uri(Request.RequestUri, "/api/team/" + sTeam.Id.ToString());
            return response;
        }

        [HttpPut]
        public HttpResponseMessage AddMember(int id, ServiceModel.AddMemberViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.ToJson());
            }

            var team = context.GetAll<DomainModel.Team>()
                              .FirstOrDefault(u => u.Id == id);

            if (team == null)
            {
                ModelState.AddModelError("", "Invalid Team");
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.ToJson());
            }

            var user = context.GetAll<DomainModel.User>()
                              .FirstOrDefault(u => u.EmailAddress == viewModel.EmailAddress);

            if (user == null)
            {
                user = new DomainModel.User(viewModel.EmailAddress);
                context.Add(user);
            }

            if (user.Teams.Any(ut => ut.TeamId == team.Id))
            {
                ModelState.AddModelError("", "User already added to team");
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.ToJson());
            }

            var newTeamMember = new DomainModel.TeamUser(team, user);
            if (team.IsOpen || team.Members.Admins().Any(x => x.Id == viewModel.AddedByUserId))
            {
                newTeamMember.Status = DomainModel.TeamUserStatus.Approved;
            }

            team.Members.Add(newTeamMember);
            context.SaveChanges();

            var sTeam = team.MapToBasicServiceModel();
            var response = Request.CreateResponse(HttpStatusCode.OK, sTeam);
            response.Headers.Location = new Uri(Request.RequestUri, "/api/team/" + sTeam.Id.ToString());
            return response;
        }

        [HttpPut]
        public HttpResponseMessage Join(int id, ServiceModel.JoinTeamViewModel joinTeamViewModel)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.ToJson());
            }

            var team = context.GetAll<DomainModel.Team>()
                              .FirstOrDefault(u => u.Id == id);

            if (team == null)
            {
                ModelState.AddModelError("", "Invalid Team");
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.ToJson());
            }

            var user = context.GetAll<DomainModel.User>()
                              .FirstOrDefault(u => u.Id == joinTeamViewModel.UserId);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid User");
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.ToJson());
            }

            if (user.Teams.Any(ut => ut.TeamId == team.Id))
            {
                ModelState.AddModelError("", "User already added to team");
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.ToJson());
            }

            var newTeamMember = new DomainModel.TeamUser(team, user);
            if (team.IsOpen)
            {
                newTeamMember.Status = DomainModel.TeamUserStatus.Approved;
            }
            team.Members.Add(newTeamMember);
            context.SaveChanges();

            var sTeam = team.MapToBasicServiceModel();
            var response = Request.CreateResponse(HttpStatusCode.OK, sTeam);
            response.Headers.Location = new Uri(Request.RequestUri, "/api/team/" + sTeam.Id.ToString());
            return response;
        }

        [HttpPut]
        public void ApproveMember(int id, ServiceModel.MemberApprovalViewModel viewModel)
        {
            var team = context.GetAll<DomainModel.Team>()
                           .FirstOrDefault(u => u.Id == id);

            if (team == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound, "Invalid Team"));
            }

            var teamMember = team.Members.FirstOrDefault(t => t.UserId == viewModel.UserId);

            if (teamMember == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound, "Invalid Team Member"));
            }

            teamMember.Status = DomainModel.TeamUserStatus.Approved;
            context.SaveChanges();
        }

        [HttpPut]
        public void DenyMember(int id, ServiceModel.MemberApprovalViewModel viewModel)
        {
            var team = context.GetAll<DomainModel.Team>()
                           .FirstOrDefault(u => u.Id == id);

            if (team == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound, "Invalid Team"));
            }

            var teamMember = team.Members.FirstOrDefault(t => t.UserId == viewModel.UserId);

            if (teamMember == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound, "Invalid Team Member"));
            }

            if (team.OwnerId == teamMember.UserId)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, "Can not deny access to the team owner"));
            }

            teamMember.Status = DomainModel.TeamUserStatus.Denied;
            context.SaveChanges();
        }

        [HttpPut]
        public HttpResponseMessage Put(int id, ServiceModel.UpdateTeamViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.ToJson().ToString()));
            }

            var existingTeam = context.GetAll<DomainModel.Team>()
                                  .FirstOrDefault(u => u.Name.Equals(viewModel.Name) && u.Id != id);

            if (existingTeam != null)
            {
                ModelState.AddModelError("", "Team name already in use");
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.ToJson());
            }

            var team = context.GetAll<DomainModel.Team>()
                              .FirstOrDefault(u => u.Id == id);

            if (team == null)
            {
                ModelState.AddModelError("", "Invalid team edited");
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.ToJson());
            }

            var editor = team.Members
                             .FirstOrDefault(tm => tm.Role == DomainModel.TeamUserRole.Administrator && tm.UserId == viewModel.UpdatedById);

            if (editor == null)
            {
                ModelState.AddModelError("", "User does not have permissions to edit team");
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.ToJson());
            }

            team.Name = viewModel.Name;
            team.IsOpen = viewModel.IsPublic;

            context.SaveChanges();

            var sTeam = team.MapToServiceModel();
            var response = Request.CreateResponse(HttpStatusCode.OK, sTeam);
            response.Headers.Location = new Uri(Request.RequestUri, "/api/team/" + sTeam.Id.ToString());
            return response;
        }

        [HttpDelete]
        public HttpResponseMessage Delete(int id, ServiceModel.DeleteTeamViewModel deleteParameters)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.ToJson());
            }

            var team = context.GetAll<DomainModel.Team>()
                              .FirstOrDefault(u => u.Id == id);

            //rest spec says we should not throw an error in this case ( delete requests should be idempotent)
            if (team == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Team"));
            }

            var editor = team.Members
                             .FirstOrDefault(tm => tm.Role == DomainModel.TeamUserRole.Administrator && tm.UserId == deleteParameters.UserId);

            if (editor == null)
            {
                ModelState.AddModelError("", "User does not have permissions to edit team");
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.ToJson());
            }

            context.Delete(team);
            context.SaveChanges();

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }
    }
}