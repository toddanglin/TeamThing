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
            this.IsDeleted = true;
            this.Status = ThingStatus.Deleted;
            LogChange(user, ThingAction.Deleted);
        }

        //public void Delete(int userId)
        //{
        //    this.IsDeleted = true;
        //    this.Status = ThingStatus.Deleted;
        //    LogChange(userId, ThingAction.Deleted);
        //}

        public void Complete(User user)
        {
            this.Status = ThingStatus.Completed;
            LogStatusChange(user);
        }

        //public void Complete(int userId)
        //{
        //    this.Status = ThingStatus.Completed;
        //    LogStatusChange(userId);
        //}

        //public void UpdateStatus(int userId, ThingStatus status)
        //{
        //    this.Status = status;
        //    LogStatusChange(userId);
        //}

        public void UpdateStatus(User user, ThingStatus status)
        {
            this.Status = status;
            LogStatusChange(user);
        }

        //private void LogStatusChange(int userId)
        //{
        //    LogChange(userId, (this.Status == ThingStatus.Completed) ? ThingAction.Completed : ThingAction.StatusChanged);
        //}
        private void LogStatusChange(User user)
        {
            LogChange(user, (this.Status == ThingStatus.Completed) ? ThingAction.Completed : ThingAction.StatusChanged);
        }

        //private void LogChange(int userId, ThingAction action)
        //{
        //    var log = new ThingLog(userId, this.Id);
        //    log.Action = action;
        //    this.History.Add(log);
        //}
        
        private void LogChange(User user, ThingAction action)
        {
            var log = new ThingLog(user, this);
            log.Action = action;
            this.History.Add(log);
        }

        public void ChangeOwner(User user, User newOwner)
        {
            SetOwner(newOwner);
            LogChange(user, ThingAction.OwnerChanged);
        }

        private void SetOwner(User newOwner)
        {
            this.Owner = newOwner;
            this.OwnerId = newOwner.Id;
        }
    }
}