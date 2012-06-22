using System.Collections.Generic;

namespace TeamThing.Web.Models.API
{
    public class User : IServiceResource
    {
        public int Id { get; set; }
        public string EmailAddress { get; set; }
        public string ImagePath { get; set; }
        public IList<TeamBasic> Teams { get; set; }
        public IList<TeamBasic> PendingTeams { get; set; }
        public IList<Thing> Things { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Nickname { get; set; }
    }
}