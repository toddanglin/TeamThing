using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamThing.Model.Helpers
{
    public static class ModelExtensions
    {
        public static IQueryable<Thing> Active(this IQueryable<Thing> things)
        {
            return things.Where(t => t.Status != ThingStatus.Deleted && t.Status != ThingStatus.Completed);
        }

        public static IQueryable<Thing> WithStatus(this IQueryable<Thing> things, string status)
        {
            ThingStatus realStatus;
            if (Enum.TryParse(status, true, out realStatus))
            {
                return things.Where(t => t.Status == realStatus);
            }

            return new List<Thing>().AsQueryable();
        }

        public static IEnumerable<Thing> Active(this IEnumerable<Thing> things)
        {
            return things.Where(t => t.Status != ThingStatus.Deleted && t.Status != ThingStatus.Completed);
        }

        public static IEnumerable<Thing> WithStatus(this IEnumerable<Thing> things, string status)
        {
            ThingStatus realStatus;
            if (Enum.TryParse(status, true, out realStatus))
            {
                return things.Where(t => t.Status == realStatus);
            }

            return new List<Thing>();
        }

        public static IQueryable<Thing> Active(this IQueryable<UserThing> userThings)
        {
            return userThings.Select(t => t.Thing).Active();
        }

        public static IQueryable<Thing> WithStatus(this IQueryable<UserThing> userThings, string status)
        {
            return userThings.Select(t => t.Thing).WithStatus(status);
        }

        public static IEnumerable<Thing> Active(this IEnumerable<UserThing> userThings)
        {
            return userThings.Select(t => t.Thing).Active();
        }

        public static IEnumerable<Thing> WithStatus(this IEnumerable<UserThing> userThings, string status)
        {
            return userThings.Select(t => t.Thing).WithStatus(status);
        }

        public static IEnumerable<Team> ActiveTeams(this IEnumerable<TeamUser> userTeams)
        {
            return userTeams.Where(tu => tu.Status == TeamUserStatus.Approved).Select(t => t.Team);
        }

        public static IEnumerable<Team> TeamsWithStatus(this IEnumerable<TeamUser> userTeams, string status)
        {
            TeamUserStatus realStatus;
            if (Enum.TryParse(status, true, out realStatus))
            {
                return userTeams.Where(tu => tu.Status == realStatus).Select(t => t.Team);
            }

            return new List<Team>();
        }

        public static IEnumerable<User> ActiveUsers(this IEnumerable<TeamUser> userTeams)
        {
            return userTeams.Where(tu => tu.Status == TeamUserStatus.Approved).Select(t => t.User);
        }

        public static IEnumerable<User> UsersWithStatus(this IEnumerable<TeamUser> userTeams, string status)
        {
            TeamUserStatus realStatus;
            if (Enum.TryParse(status, true, out realStatus))
            {
                return userTeams.Where(tu => tu.Status == realStatus).Select(t => t.User);
            }

            return new List<User>();
        }
    }
}
