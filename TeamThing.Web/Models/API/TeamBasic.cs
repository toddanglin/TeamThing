using System.Collections.Generic;

namespace TeamThing.Web.Models.API
{
    public class TeamBasic
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public int OwnerId { get; set; }
        //TODO: this should probably return a bool flag based on the current user's permissions, so that they don't know the admin's id!
        public IList<int> Administrators { get; set; }
        public bool IsPublic { get; set; }
        public string ImagePath { get; set; }
        public IList<TeamMemberBasic> TeamMembers { get; set; }
    }
 }