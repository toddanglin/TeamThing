using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq;

namespace TeamThing.Model
{
    public class Team
    {
        private Team()
        {

            this.TeamMembers = new List<TeamUser>();
        }

        public Team(string name, User owner, bool isOpen = false)
            : this()
        {
            this.Name = name;
            this.IsOpen = isOpen;
            this.DateCreated = DateTime.Now;

            ChangeOwner(owner);
        }

        public string ImagePath { get; set; }
        public int Id { get; private set; }
        public IList<TeamUser> TeamMembers { get; private set; }
        //public IList<TeamUser> ApprovedTeamMembers
        //{
        //    get
        //    {
        //        return TeamMembers.Where(tm => tm.Status == TeamUserStatus.Approved).ToList();
        //    }
        //}
        //public IList<TeamUser> PendingTeamMembers
        //{
        //    get
        //    {
        //        return TeamMembers.Where(tm => tm.Status == TeamUserStatus.Pending).ToList();
        //    }
        //}
        public string Name { get; set; }
        public User Owner { get; private set; }
        public int OwnerId { get; private set; }
        public bool IsOpen { get; set; }
        public DateTime DateCreated { get; private set; }
        //public IList<UserThing> TeamThings
        //{
        //    get
        //    {
        //        return TeamMembers.Where(tm => tm.Status == TeamUserStatus.Approved)
        //                          .Select(tm => tm.User)
        //                          .SelectMany(u => u.Things)
        //                          .Distinct()
        //                          .ToList();
        //    }
        //}

        public void ChangeOwner(User newOwner)
        {
            this.Owner = newOwner;
            this.OwnerId = newOwner.Id;
            var teamUser = new TeamUser(this, newOwner);
            teamUser.Status = TeamUserStatus.Approved;
            teamUser.Role = TeamUserRole.Administrator;
            this.TeamMembers.Add(teamUser);
        }
    }
}