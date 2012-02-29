using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mercury.Data;
using Mercury.Model;

namespace Mercury.Dash.Mvc.Controllers
{
    public class DashboardViewContext
    {
        public ICollection<Task> Tasks { get; set; }

        public ICollection<Obstacle> Obstacles { get; set; }

        public ICollection<TeamMember> MissingPeople { get; set; }

        public ICollection<TeamMember> Team { get; set; }
    }
}
