namespace TeamThing.Model
{
    public class TeamUser
    {
        private TeamUser()
        {
        }

        public TeamUser(Team team, User user)
        {
            this.Team = team;
            this.TeamId = team.Id;
            this.User = user;
            this.UserId = user.Id;
            this.Status = TeamUserStatus.Pending;
            this.Role = TeamUserRole.Viewer;
        }

        public Team Team { get; set; }
        public int TeamId { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        public TeamUserStatus Status { get; set; }
        public TeamUserRole Role { get; set; }
    }
}