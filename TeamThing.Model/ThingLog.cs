using System;

namespace TeamThing.Model
{
    public class ThingLog
    {
        public ThingLog(User user, Thing thing)
        {
            this.Thing = thing;
            this.EditedBy = user;
            this.ThingId = thing.Id;
            this.EditedByUserId = user.Id;
            this.DateOccured = DateTime.Now;
        }

        private ThingLog()
        {
        }

        public ThingAction Action { get; set; }
        public DateTime DateOccured { get; private set; }
        public User EditedBy { get;  set; }
        public int EditedByUserId { get;  set; }
        public int Id { get; private set; }
        public Thing Thing { get; set; }
        public int ThingId { get; set; }
    }
}