namespace TeamThing.Web.Models.API
{
    public class UserBasic : IServiceResource
    {
        public int Id { get; set; }
        public string ImagePath { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Nickname { get; set; }
    }

    public class UserStat
    {
        public UserBasic User { get; set; }
        public int ThingCount { get; set; }
    }
}