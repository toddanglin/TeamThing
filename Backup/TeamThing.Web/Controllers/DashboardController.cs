using System;
using System.Linq;
using System.Web.Mvc;
using TeamThing.Web.Models;
using Telerik.OpenAccess;
using TeamThing.Model;

namespace TeamThing.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly OpenAccessContext context;

        //TODO: pull out to repos, inject
        public DashboardController()
            : this(new TeamThing.Model.TeamThingContext())
        {
        }



        public DashboardController(OpenAccessContext context)
        {
            this.context = context;
        }
        public ActionResult Index()
        {
            var viewModel = new DashboardViewModel();

            ////var teamRepo = new TeamRepository(ObjectScope);
            ////var team = teamRepo.GetByID(1);
            //var team = context.GetAll<Team>().FirstOrDefault();
            //ViewData["teamName"] = team.Name;
            //viewModel.Team = team.TeamMembers;

            ////var tr = new TaskRepository(this.ObjectScope);
            ////var or = new ObstacleRepository(this.ObjectScope);

            //var things = team.Where(tm=>tm.Status == DomainModel.TeamUserStatus.Approved)
            //                 .SelectMany(tm => tm.User.Things)
            //                 .Distinct()
            //                 .ToList();
            //viewModel.UserAssignedThings = things;
            ////var things = tr.GetTodayTasksByTeam(team);
            ////var obstacles = or.GetTodayObstaclesByTeam(team);

            ////context.Tasks = things;
            ////context.Obstacles = obstacles;

            //var activePeople = things.Select(t => t.AssignedToUser)
            //                         .Distinct();

            //var missingPeople = team.TeamMembers
            //                        .Where(t => !activePeople.Contains(t.User))
            //                        .Select(t => t)
            //                        .ToList();

            //viewModel.MissingPeople = missingPeople;

            return View(viewModel);
        }

    }
}
