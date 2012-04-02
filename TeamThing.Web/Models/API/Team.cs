using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace TeamThing.Web.Models.API
{
    public class Team
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public IEnumerable<TeamMemberBasic> TeamMembers { get; set; }
        public IEnumerable<Thing> Things { get; set; }
        public IEnumerable<TeamMemberBasic> PendingTeamMembers { get; set; }
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
        public int[] Administrators { get; set; }
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
        [Required(ErrorMessage = "A team must have a name")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "A team name must be between 1 and 255 characters")]
        public string Name
        {
            get;
            set;
        }

        [Required(ErrorMessage = "A user is required to join a team")]
        public int UserId
        {
            get;
            set;
        }

        [Required(ErrorMessage = "Team Id required to join a team.")]
        public int Id 
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

        [Required(ErrorMessage = "To update a team, an id must be sent to the server")]
        public int Id
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

        [Required(ErrorMessage = "A valid id is required to update an existing thing")]
        public int Id
        {
            get;
            set;
        }

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

        public int Id
        {
            get;
            set;
        }

    }
}