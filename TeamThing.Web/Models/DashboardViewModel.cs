using System;
using System.Collections.Generic;
using System.Linq;
using TeamThing.Model;

namespace TeamThing.Web.Models
{
    public class DashboardViewModel
    {
        public IList<UserThing> UserAssignedThings { get; set; }

        //public ICollection<Obstacle> Obstacles { get; set; }

        public IList<TeamUser> MissingPeople { get; set; }

        public IList<TeamUser> Team { get; set; }
    }
}