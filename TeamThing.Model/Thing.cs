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

        public Thing(Team team, User owner)
            : this()
        {
            this.DateCreated = DateTime.Now;
            this.Status = ThingStatus.InProgress;
            this.Team = team;
            this.TeamId = team.Id;

            this.SetOwner(owner);
        }

        public bool IsStarred { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsDeleted { get; private set; }
        public int Id { get; private set; }
        public IList<UserThing> AssignedTo { get; private set; }
        public User Owner { get; private set; }
        public int OwnerId { get; private set; }
        public ThingStatus Status { get; private set; }
        public IList<ThingLog> History { get; private set; }
        public int TeamId { get; private set; }
        public Team Team { get; private set; }

        public void Delete(User user)
        {
            Delete(new ThingLog(user, this));
        }

        public void Delete(int userId)
        {
            Delete(new ThingLog(userId, this.Id));
        }

        public void Delete(ThingLog log)
        {
            log.Action = ThingAction.Deleted;
            this.IsDeleted = true;
            this.Status = ThingStatus.Deleted;
            this.History.Add(log);
        }

        public void Complete(User user)
        {
            Complete(new ThingLog(user, this));
        }

        public void Complete(int userId)
        {
            Complete(new ThingLog(userId, this.Id));
        }

        private void Complete(ThingLog log)
        {
            log.Action = ThingAction.Completed;
            this.Status = ThingStatus.Completed;
            this.History.Add(log);
        }

        public void UpdateStatus(int userId, ThingStatus status)
        {
            var log = new ThingLog(userId, this.Id);
            log.Action = (status == ThingStatus.Completed) ? ThingAction.Completed : ThingAction.StatusChanged;
            this.Status = status;
            this.History.Add(log);
        }

        public void ChangeOwner(User user, User newOwner)
        {
            SetOwner(newOwner);

            this.History.Add(new ThingLog(user, this) { Action = ThingAction.OwnerChanged });
        }
  
        private void SetOwner(User newOwner)
        {
            this.Owner = newOwner;
            this.OwnerId = newOwner.Id;
        }
    }
}