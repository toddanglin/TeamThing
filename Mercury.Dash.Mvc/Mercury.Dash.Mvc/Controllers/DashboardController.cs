using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mercury.Data;
using Mercury.Model;

namespace Mercury.Dash.Mvc.Controllers
{
    public class DashboardController : BaseController
    {
        //
        // GET: /Dashboard/
        public ActionResult Index()
        {
            var context = new DashboardViewContext();

            var teamRepo = new TeamRepository(ObjectScope);
            var team = teamRepo.GetByID(1);

            ViewData["teamName"] = team.TeamName;
            context.Team = team.TeamMembers;

            var tr = new TaskRepository(this.ObjectScope);
            var or = new ObstacleRepository(this.ObjectScope);

            var things = tr.GetTodayTasksByTeam(team);
            var obstacles = or.GetTodayObstaclesByTeam(team);

            context.Tasks = things;
            context.Obstacles = obstacles;

            var activePeople = things.Select(t => t.TeamMember).Distinct();
            var missingPeople = team.TeamMembers.Where(t => !activePeople.Contains(t)).Select(t => t).ToList();

            context.MissingPeople = missingPeople;

            return View(context);
        }
    }

    
}
