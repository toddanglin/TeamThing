using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Http;

namespace TeamThing.Web.Models.API
{
    public class Team
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public IList<TeamMemberBasic> TeamMembers { get; set; }
        public IList<Thing> Things { get; set; }
        public IList<TeamMemberBasic> PendingTeamMembers { get; set; }
        public bool IsPublic { get; set; }

        public UserBasic Owner { get; set; }
        //TODO: this should probably return a bool flag based on the current user's permissions, so that they don't know the admin's id!
        public int[] Administrators { get; set; }
    }

    public class TeamBasic
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public int OwnerId { get; set; }

        //TODO: this should probably return a bool flag based on the current user's permissions, so that they don't know the admin's id!
        public IList<int> Administrators { get; set; }
        public bool IsPublic { get; set; }
        public string ImagePath { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string EmailAddress { get; set; }
        public string ImagePath { get; set; }
        public IList<TeamBasic> Teams { get; set; }
        public IList<TeamBasic> PendingTeams { get; set; }
        public IList<Thing> Things { get; set; }
    }

    public class UserBasic
    {
        public int Id { get; set; }
        public string ImagePath { get; set; }
        public string EmailAddress { get; set; }
    }

    public class JoinTeamViewModel
    {
        [Required(ErrorMessage = "A user is required to join a team")]
        public int UserId
        {
            get;
            set;
        }
    }

    public class AddTeamViewModel
    {
        [Required(ErrorMessage = "A team must have a name")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "A team name must be between 1 and 255 characters")]
        public string Name
        {
            get;
            set;
        }

        public bool IsPublic
        {
            get;
            set;
        }

        [Required(ErrorMessage = "A team must have a creator")]
        public int CreatedById
        {
            get;
            set;
        }
    }

    public class UpdateTeamViewModel
    {
        [Required(ErrorMessage = "A team must have a name")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "A team name must be between 1 and 255 characters")]
        public string Name
        {
            get;
            set;
        }

        public bool IsPublic
        {
            get;
            set;
        }

        [Required(ErrorMessage = "To update a team, a user id must be sent to the server")]
        public int UpdatedById
        {
            get;
            set;
        }
    }

    public class DeleteTeamViewModel
    {
        public int UserId
        {
            get;
            set;
        }
    }

    public class UpdateThingStatusViewModel
    {
        public int UserId
        {
            get;
            set;
        }
        public string Status
        {
            get;
            set;
        }
    }

    public class MemberApprovalViewModel
    {
        public int UserId
        {
            get;
            set;
        }
    }

    public class AddThingViewModel
    {
        public string Description
        {
            get;
            set;
        }

        [Required(ErrorMessage = "A thing must have a creator")]
        public int CreatedById
        {
            get;
            set;
        }

        [Required(ErrorMessage = "A thing must have a an associated team")]
        public int TeamId
        {
            get;
            set;
        }

        [Required(ErrorMessage = "A thing must be assigned to 1 or more people")]
        public int[] AssignedTo
        {
            get;
            set;
        }
    }

    public class UpdateThingViewModel
    {
        public string Description
        {
            get;
            set;
        }

        [Required(ErrorMessage = "A thing must have an editor")]
        public int EditedById
        {
            get;
            set;
        }

        [Required(ErrorMessage = "A thing must be assigned to 1 or more people")]
        public int[] AssignedTo
        {
            get;
            set;
        }
    }

    public class DeleteThingViewModel
    {

        [Required(ErrorMessage = "A thing can only be deleted by a valid user.")]
        public int DeletedById
        {
            get;
            set;
        }
    }

    public class CompleteThingViewModel
    {
        public int UserId
        {
            get;
            set;
        }
    }
}