using System.Collections.Generic;

namespace TeamThing.Web.Models.API
{
    public class User
    {
        public int Id { get; set; }
        public string EmailAddress { get; set; }
        public string ImagePath { get; set; }
        public IList<TeamBasic> Teams { get; set; }
        public IList<TeamBasic> PendingTeams { get; set; }
        public IList<Thing> Things { get; set; }
    }
}