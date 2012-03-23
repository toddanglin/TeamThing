using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DomainModel = TeamThing.Model;
using ServiceModel = TeamThing.Web.Models.API;

namespace TeamThing.Web.Core.Mappers
{
    public static class DomainToServiceModel
    {
        public static IQueryable<ServiceModel.Team> MapToServiceModel(this IQueryable<DomainModel.Team> modelTeams)
        {
            return modelTeams.Select(MapToServiceModel).AsQueryable();
        }

        public static IEnumerable<ServiceModel.Thing> MapToServiceModel(this IEnumerable<DomainModel.UserThing> userThings)
        {
            return userThings.Select(MapToServiceModel);
        }

        public static IEnumerable<ServiceModel.TeamMemberBasic> MapToServiceModel(this IQueryable<DomainModel.TeamUser> teamMembers)
        {
            return teamMembers.Select(MapToServiceModel);
        }

        public static IQueryable<ServiceModel.UserBasic> MapToServiceModel(this IQueryable<DomainModel.User> users)
        {
            return users.Select(MapToBasicServiceModel).AsQueryable();
        }
        public static IQueryable<ServiceModel.Thing> MapToServiceModel(this IQueryable<DomainModel.Thing> things)
        {
            return things.Select(MapToServiceModel).AsQueryable();
        }

        public static ServiceModel.Thing MapToServiceModel(this DomainModel.Thing t)
        {
            return new ServiceModel.Thing()
            {
                Id = t.Id,
                Description = t.Description,
                Status = t.Status.ToString()
            };
        }

        public static ServiceModel.Thing MapToServiceModel(this DomainModel.UserThing t)
        {
            return new ServiceModel.Thing()
            {
                Id = t.ThingId,
                Description = t.Thing.Description,
                Status = t.Thing.Status.ToString()
            };
        }

        public static ServiceModel.TeamMemberBasic MapToServiceModel(this DomainModel.TeamUser tm)
        {
            return new ServiceModel.TeamMemberBasic()
            {
                Id = tm.UserId,
                FullName = tm.User.FirstName + " " + tm.User.LastName,
                EmailAddress = tm.User.EmailAddress,
                Role = tm.Role.ToString()
            };
        }

        public static ServiceModel.Team MapToServiceModel(this DomainModel.Team t)
        {
            return new ServiceModel.Team()
            {
                Id = t.Id,
                Name = t.Name,
                IsPublic = t.IsOpen,
                TeamMembers = new List<ServiceModel.TeamMemberBasic>(t.TeamMembers.Where(tm=>tm.Status == DomainModel.TeamUserStatus.Approved).Select(MapToServiceModel)),
                PendingTeamMembers = new List<ServiceModel.TeamMemberBasic>(t.TeamMembers.Where(tm=>tm.Status == DomainModel.TeamUserStatus.Pending).Select(MapToServiceModel))
            };
        }

        public static ServiceModel.TeamBasic MapToBasicServiceModel(this DomainModel.Team t)
        {
            return new ServiceModel.TeamBasic()
            {
                Id = t.Id,
                Name = t.Name,
                OwnerId=t.OwnerId,
                IsPublic = t.IsOpen,
                Administrators = t.TeamMembers.Where(tm => tm.Role == DomainModel.TeamUserRole.Administrator).Select(tm => tm.UserId).ToArray()
            };
        }

        public static IEnumerable<ServiceModel.TeamBasic> MapToBasicServiceModel(this IEnumerable<DomainModel.Team> t)
        {
            return t.Select(MapToBasicServiceModel);
        }

        public static ServiceModel.User MapToServiceModel(this DomainModel.User user)
        {
            return new ServiceModel.User()
            {
                Id = user.Id,
                EmailAddress = user.EmailAddress,
                Teams = user.Teams.Where(t => t.Status == DomainModel.TeamUserStatus.Approved).Select(tu => tu.Team).MapToBasicServiceModel().ToList(),
                PendingTeams = user.Teams.Where(t => t.Status == DomainModel.TeamUserStatus.Pending).Select(tu => tu.Team).MapToBasicServiceModel().ToList(),
                Things = user.Things.MapToServiceModel().ToList()
            };
        }

        public static ServiceModel.UserBasic MapToBasicServiceModel(this DomainModel.User user)
        {
            return new ServiceModel.UserBasic()
            {
                Id = user.Id,
                EmailAddress = user.EmailAddress
            };
        }
    }
}