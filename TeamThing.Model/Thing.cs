using System;
using System.Collections.Generic;

namespace TeamThing.Model
{
    public class Thing
    {
        private Thing()
        {
            this.AssignedTo = new List<UserThing>();
            this.History = new List<ThingLog>();
        }

        public Thing(User owner)
            : this()
        {
            this.DateCreated = DateTime.Now;
            this.Status = ThingStatus.InProgress;

            this.ChangeOwner(owner);
        }

        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsDeleted { get; private set; }
        public int Id { get; private set; }
        public IList<UserThing> AssignedTo { get; private set; }
        public User Owner { get; private set; }
        public int OwnerId { get; private set; }
        public ThingStatus Status { get; private set; }
        public IList<ThingLog> History { get; private set; }
        public void Delete()
        {
            this.IsDeleted = true;
            this.Status = ThingStatus.Deleted;
        }

        public void Complete()
        {
            this.Status = ThingStatus.Completed;
        }

        public void ChangeOwner(User newOwner)
        {
            this.Owner = newOwner;
            this.OwnerId = newOwner.Id;
        }
    }
}