namespace TeamThing.Model
{
    public class UserThing
    {
        private UserThing()
        {
        }

        public UserThing(Thing thing, User assignTo, User assignedBy)
        {
            this.Thing = thing;
            this.ThingId = thing.Id;

            this.AssignedByUser = assignedBy;
            this.AssignedByUserId = assignedBy.Id;

            this.AssignedToUser = assignTo;
            this.AssignedToUserId = assignTo.Id;
        }

        public User AssignedToUser { get; set; }
        public int AssignedToUserId { get; set; }
        public User AssignedByUser { get; set; }
        public int AssignedByUserId { get; set; }
        public Thing Thing { get; set; }
        public int ThingId { get; set; }
    }
}