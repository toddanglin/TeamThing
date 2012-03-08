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
        }

        public Team(string name, User owner, bool isOpen = false)
        {
            this.Name = name;
            this.IsOpen = isOpen;
            this.DateCreated = DateTime.Now;

            this.TeamMembers = new List<TeamUser>();

            ChangeOwner(owner);
        }

        public string ImagePath { get; set; }
        public int Id { get;  private set; }
        public IList<TeamUser> TeamMembers { get; private set; }
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
        //                          .SelectMany(tm => tm.User.Things)
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
            this.TeamMembers.Add(teamUser);
        }
    }
}