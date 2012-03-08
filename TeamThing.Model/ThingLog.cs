using System;

namespace TeamThing.Model
{
    public class ThingLog
    {
        public ThingLog(User user, Thing thing)
        {
            Thing = thing;
            this.EditedBy = user;
            this.DateOccured = DateTime.Now;
        }

        private ThingLog()
        {
        }

        public ThingAction Action { get; private set; }
        public DateTime DateOccured { get; private set; }
        public User EditedBy { get; private set; }
        public int EditedByUserId { get; private set; }
        public int Id { get; private set; }
        public Thing Thing { get; private set; }
    }
}