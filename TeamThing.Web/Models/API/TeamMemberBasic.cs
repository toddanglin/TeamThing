namespace TeamThing.Web.Models.API
{
    public class TeamMemberBasic : IServiceResource
    {
        public int Id { get;set; }
        public string EmailAddress { get;set; }
        public string FullName { get;set; }
        public string Role { get; set; }
        public string ImagePath { get; set; }
    }
}