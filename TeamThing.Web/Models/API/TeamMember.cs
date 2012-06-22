using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeamThing.Web.Models.API
{
    public class TeamMember : IServiceResource
    {
        public int Id { get;set; }
        public string EmailAddress { get;set; }
        public string FullName { get;set; }
        public IList<Thing> Things { get;set; }
    }
}