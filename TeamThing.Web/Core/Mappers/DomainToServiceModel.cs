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

        public static IEnumerable<ServiceModel.ThingBasic> MapToBasicServiceModel(this IEnumerable<DomainModel.Thing> things)
        {
            return things.Select(MapToBasicServiceModel);
        } 
        
       

        public static IQueryable<ServiceModel.ThingBasic> MapToBasicServiceModel(this IQueryable<DomainModel.Thing> things)
        {
            return things.Select(MapToBasicServiceModel).AsQueryable();
        }

        public static IEnumerable<ServiceModel.TeamMemberBasic> MapToServiceModel(this IQueryable<DomainModel.TeamUser> teamMembers)
        {
            return teamMembers.Select(MapToBasicServiceModel);
        }

        public static IQueryable<ServiceModel.UserBasic> MapToServiceModel(this IQueryable<DomainModel.User> users)
        {
            return users.Select(MapToBasicServiceModel).AsQueryable();
        }

        public static IEnumerable<ServiceModel.UserBasic> MapToBasicServiceModel(this IEnumerable<DomainModel.User> users)
        {
            return users.Select(MapToBasicServiceModel);
        }

        public static IQueryable<ServiceModel.Thing> MapToServiceModel(this IQueryable<DomainModel.Thing> things)
        {
            return things.Select(MapToServiceModel).AsQueryable();
        }

        public static IEnumerable<ServiceModel.Thing> MapToServiceModel(this IEnumerable<DomainModel.Thing> things)
        {
            return things.Select(MapToServiceModel).AsQueryable();
        }

        public static IQueryable<ServiceModel.TeamBasic> MapToBasicServiceModel(this IEnumerable<DomainModel.Team> t)
        {
            return t.Select(MapToBasicServiceModel).AsQueryable();
        }

        public static ServiceModel.Thing MapToServiceModel(this DomainModel.Thing t)
        {
            if (t == null) return null;
            return new ServiceModel.Thing()
            {
                Id = t.Id,
                Description = t.Description,
                Status = t.Status.ToString(),
                DateCreated = t.DateCreated,
                IsStarred = t.IsStarred,
                AssignedTo = t.AssignedTo.Select(a => a.AssignedToUser).MapToBasicServiceModel(),
                Owner = t.Owner.MapToBasicServiceModel(),
                Team = t.Team.MapToBasicServiceModel()
            };
        }
        public static ServiceModel.ThingBasic MapToBasicServiceModel(this DomainModel.Thing t)
        {
            if (t == null) return null;
            return new ServiceModel.ThingBasic()
            {
                Id = t.Id,
                Description = t.Description,
                Status = t.Status.ToString(),
                IsStarred = t.IsStarred
            };
        }

        public static ServiceModel.Thing MapToServiceModel(this DomainModel.UserThing t)
        {
            if (t == null) return null;
            return new ServiceModel.Thing()
            {
                Id = t.ThingId,
                Description = t.Thing.Description,
                Status = t.Thing.Status.ToString(),
                AssignedTo = t.Thing.AssignedTo.Select(a => a.AssignedToUser).MapToBasicServiceModel(),
                Owner = t.Thing.Owner.MapToBasicServiceModel(),
                Team = t.Thing.Team.MapToBasicServiceModel()
            };
        }


        public static ServiceModel.TeamMemberBasic MapToBasicServiceModel(this DomainModel.TeamUser tm)
        {
            if (tm == null) return null;
            return new ServiceModel.TeamMemberBasic()
            {
                Id = tm.UserId,
                FullName = tm.User.FirstName + " " + tm.User.LastName,
                EmailAddress = tm.User.EmailAddress,
                ImagePath = tm.User.ImagePath ?? "/images/GenericUserImage.gif",
                Role = tm.Role.ToString()
            };
        }

        public static ServiceModel.Team MapToServiceModel(this DomainModel.Team t)
        {
            if (t == null) return null;
            return new ServiceModel.Team()
            {
                Id = t.Id,
                Name = t.Name,
                IsPublic = t.IsOpen,
                TeamMembers = t.Members.Where(tm => tm.Status == DomainModel.TeamUserStatus.Approved).Select(MapToBasicServiceModel).ToList(),
                PendingTeamMembers = t.Members.Where(tm => tm.Status == DomainModel.TeamUserStatus.Pending).Select(MapToBasicServiceModel).ToList(),
                Administrators = t.Members.Where(tm => tm.Role == DomainModel.TeamUserRole.Administrator).Select(tm => tm.UserId).ToArray(),
                Owner = t.Owner.MapToBasicServiceModel(),
                Things = t.Things.MapToServiceModel().ToList()
            };
        }

        public static ServiceModel.TeamBasic MapToBasicServiceModel(this DomainModel.Team t)
        {
            if (t == null) return null;
            return new ServiceModel.TeamBasic()
            {
                Id = t.Id,
                Name = t.Name,
                OwnerId = t.OwnerId,
                IsPublic = t.IsOpen,
                ImagePath = t.ImagePath ?? "/images/GenericUserImage.gif",
                Administrators = t.Members.Where(tm => tm.Role == DomainModel.TeamUserRole.Administrator).Select(tm => tm.UserId).ToArray()
            };
        }


        public static ServiceModel.User MapToServiceModel(this DomainModel.User user)
        {
            if (user == null) return null;
            return new ServiceModel.User()
            {
                Id = user.Id,
                ImagePath = user.ImagePath ?? "/images/GenericUserImage.gif",
                EmailAddress = user.EmailAddress,
                Teams = user.Teams.Where(t => t.Status == DomainModel.TeamUserStatus.Approved).Select(tu => tu.Team).MapToBasicServiceModel().ToList(),
                PendingTeams = user.Teams.Where(t => t.Status == DomainModel.TeamUserStatus.Pending).Select(tu => tu.Team).MapToBasicServiceModel().ToList(),
                Things = user.Things.MapToServiceModel().ToList()
            };
        }

        public static ServiceModel.UserBasic MapToBasicServiceModel(this DomainModel.User user)
        {
            if (user == null) return null;
            return new ServiceModel.UserBasic()
            {
                Id = user.Id,
                EmailAddress = user.EmailAddress,
                ImagePath = user.ImagePath ?? "/images/GenericUserImage.gif"
            };
        }
    }
}