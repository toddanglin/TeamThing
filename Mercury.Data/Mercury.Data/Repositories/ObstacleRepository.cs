using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mercury.Model;
using Telerik.OpenAccess;

namespace Mercury.Data
{
    public class ObstacleRepository : BaseRepository<Obstacle>
    {
        public ObstacleRepository(IObjectScope scope) : base(scope) { }

        public override Obstacle GetByID(int id)
        {
            throw new NotImplementedException();
        }

        public override void Add(Obstacle item)
        {
            //Make sure there are no apparent duplicates before adding
            var dupe = scope.Extent<Obstacle>().Where(o => o.Description.ToLower() == item.Description.ToLower()
                && o.DateCreated == item.DateCreated
                && o.TeamMember == item.TeamMember)
                .FirstOrDefault();

            if(dupe == null) //Dupe not found, safe to add
                base.Add(item);
        }

        public List<Obstacle> GetTodayObstaclesByTeam(Team team)
        {
            var yesterday = DateTime.Now.AddDays(-1).Date;
            return scope.Extent<Obstacle>().Where(t => team.TeamMembers.Contains(t.TeamMember)
                && (t.DateCreated.Date == DateTime.Now.Date || t.DateCreated.Date == yesterday)
                && t.IsCleared != true)
                .ToList();
        }
    }
}
