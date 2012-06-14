using System.ComponentModel.DataAnnotations;

namespace TeamThing.Web.Models.API
{
    public class UpdateTeamViewModel
    {
        [Required(ErrorMessage = "A team must have a name")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "A team name must be between 1 and 255 characters")]
        public string Name { get; set; }
        public bool IsPublic { get; set; }
        [Required(ErrorMessage = "To update a team, a user id must be sent to the server")]
        public int UpdatedById { get; set; }
    }
}