using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mercury.Model;
using Telerik.OpenAccess;

namespace Mercury.Data
{
    public class TaskRepository : BaseRepository<Task>
    {
        public TaskRepository(IObjectScope scope) : base(scope) { }

        public override Task GetByID(int id)
        {
            return scope.Extent<Task>().Where(t => t.ThingId == id).FirstOrDefault();
        }

        public bool IsDuplicate(Task item)
        {
            var dupe = scope.Extent<Task>().Where(o => o.Description.ToLower().Trim() == item.Description.ToLower().Trim()
                                                       && o.DateCreated.Date == item.DateCreated.Date
                                                       && o.TeamMember == item.TeamMember)
            .FirstOrDefault();

            return dupe != null;
        }

        public List<Task> GetTodayTasksByTeam(Team team)
        {
            var yesterday = DateTime.Now.AddDays(-1).Date;
            var tomorrow = DateTime.Now.AddDays(1).Date;
            return scope.Extent<Task>().Where(t => team.TeamMembers.Contains(t.TeamMember)
                && (t.DateCreated.Date == DateTime.Now.Date || t.DateCreated.Date == yesterday || t.DateCreated.Date == tomorrow)
                && t.Status != TaskStatus.Removed)
                .ToList();
        }

        public List<Task> GetTodayTasksByPerson(TeamMember teamMember)
        {
            var yesterday = DateTime.Now.AddDays(-1).Date;
            var tomorrow = DateTime.Now.AddDays(1).Date;
            return scope.Extent<Task>().Where(t => t.TeamMember == teamMember
                && (t.DateCreated.Date == DateTime.Now.Date || t.DateCreated.Date == yesterday || t.DateCreated.Date == tomorrow)
                && (t.Status != TaskStatus.Cancelled || t.Status != TaskStatus.Removed))
                .ToList();
        }

        public List<Task> GetTasksByStatus(Team team, TaskStatus status)
        {
            return scope.Extent<Task>().Where(t => team.TeamMembers.Contains(t.TeamMember) 
                && t.Status == status)
                .ToList();
        }

        public Task ChangeStatus(TeamMember tm, string thingText, TaskStatus taskStatus)
        {
            //Try to find the original task
            var original = FindOriginalTask(thingText, tm);

            if (original == null)
                return null;

            //Update status
            original.Status = taskStatus;

            return original;
        }

        public Task FindOriginalTask(string thingText, TeamMember tm)
        {
            //Try to find the original task
            //A match must:
            //1- Have the same text
            //2- Have a Date of today or yesterday or tomorrow (to handle overnight completion and "delayed" tasks)
            //3- Be created by the current team member
            var yesterday = DateTime.Now.AddDays(-1).Date;
            var tomorrow = DateTime.Now.AddDays(1).Date;
            thingText = thingText.ToLower().Trim();
            
            var original = scope.Extent<Task>()
            .Where(t => t.Description.ToLower().Trim() == thingText
                        && (t.DateCreated.Date == DateTime.Now.Date || t.DateCreated.Date == yesterday || t.DateCreated.Date == tomorrow)
                        && t.TeamMember == tm)
            .FirstOrDefault();

            return original;
        }

        public Task Delay(TeamMember tm, string thingText)
        {
            return Delay(tm, thingText, 1);
        }

        public Task Delay(TeamMember tm, string thingText, int days)
        {
            //Try to find the original task
            var original = FindOriginalTask(thingText, tm);

            //Match couldn't be found
            if (original == null)
                return null;

            //Change date on task to tomorrow
            original.DateCreated = original.DateCreated.AddDays(days);
            original.Status = TaskStatus.Delayed;

            return original;
        }
    }
}
