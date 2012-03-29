using System;

namespace TeamThing.Model
{
    public class ThingLog
    {
        public ThingLog(User user, Thing thing)
        {
            Thing = thing;
            ThingId = thing.Id;
            this.EditedBy = user;
            this.EditedByUserId = user.Id;
            this.DateOccured = DateTime.Now;
        }

        public ThingLog(int userId, int thingId)
        {
            ThingId = thingId;
            this.EditedByUserId = userId;
            this.DateOccured = DateTime.Now;
        }

        private ThingLog()
        {
        }

        public ThingAction Action { get; set; }
        public DateTime DateOccured { get; private set; }
        public User EditedBy { get; private set; }
        public int EditedByUserId { get; private set; }
        public int Id { get; private set; }
        public Thing Thing { get; private set; }
        public int ThingId { get; private set; }
    }
}