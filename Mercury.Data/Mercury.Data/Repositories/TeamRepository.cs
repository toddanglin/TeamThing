using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mercury.Model;
using Telerik.OpenAccess;
using Mercury.Data.Util;

namespace Mercury.Data
{
    public class TeamRepository : BaseRepository<Team>
    {
        public TeamRepository(IObjectScope scope) : base(scope) { }
        
        public override Team GetByID(int id)
        {
            return scope.Extent<Team>().Where(t => t.TeamId == id).FirstOrDefault();
        }

        public void AddTeamMember(int teamId, string firstName, string lastName, string email)
        {
            var teamMember = new TeamMember() 
            { 
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                ProfileColor = StringConstants.DEFAULT_PROFILE_HEX_COLOR
            };

            AddTeamMember(teamId, teamMember);
        }

        public void AddTeamMember(int teamId, TeamMember teamMember)
        {
            //Adds team member to team
            var team = GetByID(teamId);

            AddTeamMember(team, teamMember);
        }

        public void AddTeamMember(Team team, TeamMember teamMember)
        {
            //Creates teamMember
            scope.Add(teamMember);

            //Adds team member to team
            team.TeamMembers.Add(teamMember);
        }

        public Guid GenerateNewApiKey(int teamMemberId)
        {
            var tm = scope.Extent<TeamMember>().Where(t => t.TeamMemberId == teamMemberId).FirstOrDefault();

            if (tm == null) return Guid.Empty;

            var newKey = Guid.NewGuid();
            tm.ApiKey = newKey;

            return newKey;
        }

        public TeamMember GetTeamMemberByKey(Guid key)
        {
            var tm = scope.Extent<TeamMember>().Where(t => t.ApiKey == key).FirstOrDefault();
            return tm;
        }

        public List<TeamMember> GetMembersWithNoCurrentTasks(int teamId)
        {
            //Find team members who DO have tasks for today
            var tmWithTasks = scope.Extent<Task>().Where(t => t.TeamMember.Team.TeamId == teamId
                    && t.Status != TaskStatus.Delayed 
                    && t.DateCreated.Date == DateTime.Now.Date)
                    .Select(t => t.TeamMember.TeamMemberId).ToList();

            //Now return the inverse list of team members that do not appear on first list
            //(in other words, team members that don't have tasks for today)
            if (tmWithTasks == null || tmWithTasks.Count == 0) //No team members have submitted tasks - select everyone
                return scope.Extent<TeamMember>().Where(t => t.Team.TeamId == teamId).ToList();
            else
                return scope.Extent<TeamMember>().Where(t => t.Team.TeamId == teamId && !tmWithTasks.Contains(t.TeamMemberId)).ToList();
        }

        public List<TeamMember> GetMembersCurrentTasks(int teamId)
        {
            //Find team members who DO have tasks for today
            return scope.Extent<Task>().Where(t => t.TeamMember.Team.TeamId == teamId
                    && t.Status == TaskStatus.InProgress
                    && t.DateCreated.Date == DateTime.Now.Date)
                    .Select(t => t.TeamMember).ToList();
        }
    }
}
