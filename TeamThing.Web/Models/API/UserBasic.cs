namespace TeamThing.Web.Models.API
{
    public class UserBasic
    {
        public int Id { get; set; }
        public string ImagePath { get; set; }
        public string EmailAddress { get; set; }
    }

    public class UserStat
    {
        public UserBasic User { get; set; }
        public int ThingCount { get; set; }
    }
}