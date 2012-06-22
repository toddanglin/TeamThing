using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TeamThing.Model.Helpers;
using TeamThing.Web.Core.Helpers;
using TeamThing.Web.Core.Mappers;
using TeamThing.Web.Core.Security;
using TeamThing.Web.Mailers;
using DomainModel = TeamThing.Model;
using ServiceModel = TeamThing.Web.Models.API;
using Mvc.Mailer;

namespace TeamThing.Web.Controllers
{
    //[RequireOAuthAuthorization]
    public class TeamController : TeamThingApiController
    {
        private IUserMailer emailService = new UserMailer();

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
            //get team
            var team = GetTeam(id);

            return ResourceOkResponse(team.MapToBasicServiceModel());
        }

        // GET /api/team/5/things/{status}
        [HttpGet]
        [Queryable]
        public IQueryable<ServiceModel.ThingBasic> Things(int id, string status)
        {
            //get team
            var team = GetTeam(id);

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

        //TODO: Move to service
        private DomainModel.Team GetTeam(int id)
        {
            //get user
            var team = context.GetAll<DomainModel.Team>()
                         .FirstOrDefault(t => t.Id == id);
            if (team == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound, "Invalid Team"));
            }
            return team;
        }

        // GET /api/team/5/members/{status}
        [HttpGet]
        [Queryable]
        public IQueryable<ServiceModel.UserBasic> Members(int id, string status)
        {
            //get team
            var team = GetTeam(id);

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
            //get team
            var team = GetTeam(id);

            var realStatus = DomainModel.ThingAction.Completed;

            return context.GetAll<DomainModel.ThingLog>()
                          .Where(t => t.Thing.TeamId == id && t.Action == realStatus)
                          .GroupBy(t => t.EditedBy)
                          .Select(t => new ServiceModel.UserStat() { User = t.Key.MapToBasicServiceModel(), ThingCount = t.Count() });

        }

        // POST /api/team/5
        public HttpResponseMessage Post(ServiceModel.AddTeamViewModel addTeamViewModel)
        {
            if (!ModelState.IsValid) { throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.ToJson().ToString())); }

            var existingTeam = context.GetAll<DomainModel.Team>()
                                      .FirstOrDefault(u => u.Name.Equals(addTeamViewModel.Name));

            if (existingTeam != null) { throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, "Team name already in use")); }


            var teamCreator = context.GetAll<DomainModel.User>()
                                     .FirstOrDefault(u => u.Id == addTeamViewModel.CreatedById);

            if (teamCreator == null) { throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Team Owner Specified")); }

            var team = new DomainModel.Team(addTeamViewModel.Name, teamCreator);
            team.IsOpen = addTeamViewModel.IsPublic;
            context.Add(team);
            context.SaveChanges();
            
            return ResourceOkResponse(team.MapToBasicServiceModel());
        }

        // PUT /api/team/5/AddMember
        [HttpPut]
        public HttpResponseMessage AddMember(int id, ServiceModel.AddMemberViewModel viewModel)
        {
            if (!ModelState.IsValid) { throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.ToJson())); }

            //get team
            var team = GetTeam(id);

            var user = context.GetAll<DomainModel.User>()
                              .FirstOrDefault(u => u.EmailAddress == viewModel.EmailAddress);

            if (user == null)
            {
                user = new DomainModel.User(viewModel.EmailAddress);
                context.Add(user);
            }

            if (user.Teams.Any(ut => ut.TeamId == team.Id)) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, "User already added to team"));

            var newTeamMember = new DomainModel.TeamUser(team, user);
            var inviter = team.Members.FirstOrDefault(x => x.UserId == viewModel.AddedByUserId);
            
            if (inviter == null) { throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, "User Not Allowed to Invite Members to this Team")); }

            if (team.IsOpen || (inviter != null && inviter.Role== DomainModel.TeamUserRole.Administrator))
            {
                newTeamMember.Status = DomainModel.TeamUserStatus.Approved;
            }

            emailService.InvitedToTeam(user, inviter.User, team).Send();

            team.Members.Add(newTeamMember);
            context.SaveChanges();

            return ResourceOkResponse(team.MapToBasicServiceModel());
        }


        // PUT /api/team/5/Join
        [HttpPut]
        public HttpResponseMessage Join(int id, ServiceModel.JoinTeamViewModel joinTeamViewModel)
        {
            if (!ModelState.IsValid) { throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.ToJson())); }

            //get team
            var team = GetTeam(id);

            var user = context.GetAll<DomainModel.User>()
                              .FirstOrDefault(u => u.Id == joinTeamViewModel.UserId);

            if (user == null) { throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid User")); }
            if (user.Teams.Any(ut => ut.TeamId == team.Id)) { throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, "User already added to team")); }

            var newTeamMember = new DomainModel.TeamUser(team, user);
            if (team.IsOpen)
            {
                newTeamMember.Status = DomainModel.TeamUserStatus.Approved;
            }
            team.Members.Add(newTeamMember);
            context.SaveChanges();

            return ResourceOkResponse(team.MapToBasicServiceModel());
        }


        // PUT /api/team/5/Leave
        [HttpPut]
        public HttpResponseMessage Leave(int id, ServiceModel.JoinTeamViewModel joinTeamViewModel)
        {
            if (!ModelState.IsValid) { throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.ToJson().ToString())); }

            //get team
            var team = GetTeam(id);

            var user = context.GetAll<DomainModel.User>()
                              .FirstOrDefault(u => u.Id == joinTeamViewModel.UserId);

            if (user == null) { throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid User")); }
            if (user.Id == team.OwnerId) { throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, "Owner can not leave team")); }

            var teamUser = user.Teams.FirstOrDefault(ut => ut.TeamId == team.Id);

            if (teamUser != null)
            {
                team.Members.Remove(teamUser);
                context.SaveChanges();
            }

            return ResourceOkResponse(team.MapToBasicServiceModel());
        }


        // PUT /api/team/5/ApproveMember
        [HttpPut]
        public void ApproveMember(int id, ServiceModel.MemberApprovalViewModel viewModel)
        {
            if (!ModelState.IsValid) { throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.ToJson().ToString())); }

            //get team
            var team = GetTeam(id);

            var authorizer = team.Members.FirstOrDefault(tm => tm.UserId == viewModel.StatusChangedByUserId);

            if (authorizer == null || (authorizer.Role != DomainModel.TeamUserRole.Administrator && team.OwnerId != authorizer.UserId))
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.Forbidden, "Only team owners, and admins can approve members."));
            }

            var teamMember = team.Members.FirstOrDefault(t => t.UserId == viewModel.UserId);

            if (teamMember == null) { throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound, "Invalid Team Member")); }

            teamMember.Status = DomainModel.TeamUserStatus.Approved;
            context.SaveChanges();

            emailService.ApprovedForTeam(teamMember.User, team).Send();
        }


        // PUT /api/team/5/DenyMember
        [HttpPut]
        public void DenyMember(int id, ServiceModel.MemberApprovalViewModel viewModel)
        {
            if (!ModelState.IsValid) { throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.ToJson().ToString())); }

            //get team
            var team = GetTeam(id);

            var teamMember = team.Members.FirstOrDefault(t => t.UserId == viewModel.UserId);

            if (teamMember == null) { throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound, "Invalid Team Member")); }
            if (team.OwnerId == teamMember.UserId) { throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, "Can not deny access to the team owner")); }

            var authorizer = team.Members.FirstOrDefault(tm => tm.UserId == viewModel.StatusChangedByUserId);

            if (authorizer == null || (authorizer.Role != DomainModel.TeamUserRole.Administrator && team.OwnerId != authorizer.UserId))
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.Forbidden, "Only team owners, and admins can approve members."));
            }

            teamMember.Status = DomainModel.TeamUserStatus.Denied;
            context.SaveChanges();

            emailService.DeniedTeam(teamMember.User, team).Send();
        }


        // PUT /api/team/5
        [HttpPut]
        public HttpResponseMessage Put(int id, ServiceModel.UpdateTeamViewModel viewModel)
        {
            if (!ModelState.IsValid) { throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.ToJson().ToString())); }

            var existingTeam = context.GetAll<DomainModel.Team>()
                                      .FirstOrDefault(u => u.Name.Equals(viewModel.Name) && u.Id != id);

            if (existingTeam != null) { throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, "Team Name Already in Use")); }

            //get team
            var team = GetTeam(id);

            var editor = team.Members
                             .FirstOrDefault(tm => tm.Role == DomainModel.TeamUserRole.Administrator && tm.UserId == viewModel.UpdatedById);

            if (editor == null) { throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, "User does not have permissions to edit team")); }

            team.Name = viewModel.Name;
            team.IsOpen = viewModel.IsPublic;

            context.SaveChanges();

            return ResourceOkResponse(team.MapToServiceModel());
        }

        // DELETE /api/team/5
        [HttpDelete]
        public HttpResponseMessage Delete(int id, ServiceModel.DeleteTeamViewModel viewModel)
        {
            if (!ModelState.IsValid) { throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.ToJson().ToString())); }

            //get team
            var team = GetTeam(id);

            var editor = team.Members
                             .FirstOrDefault(tm => tm.Role == DomainModel.TeamUserRole.Administrator && tm.UserId == viewModel.UserId);

            if (editor == null) { throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, "User does not have permissions to delete team")); }

            context.Delete(team);
            context.SaveChanges();

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }
    }
}