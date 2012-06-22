using System.Collections.Generic;
namespace TeamThing.Web.Models.API
{
    public class ThingBasic : IServiceResource
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool IsStarred{ get; set; }
        public string Status { get; set; }
        public int TeamId { get; set; }
        public UserBasic Owner { get; set; }
        public IEnumerable<UserBasic> AssignedTo { get; set; }
    }
}