using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Http;

namespace TeamThing.Web.Models.API
{
    public class Team
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public IList<TeamMemberBasic> TeamMembers { get; set; }
        public IList<Thing> Things { get; set; }
        public IList<TeamMemberBasic> PendingTeamMembers { get; set; }
        public bool IsPublic { get; set; }

        public UserBasic Owner { get; set; }
        //TODO: this should probably return a bool flag based on the current user's permissions, so that they don't know the admin's id!
        public int[] Administrators { get; set; }
    }
}