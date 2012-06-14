using System.ComponentModel.DataAnnotations;

namespace TeamThing.Web.Models.API
{
    public class JoinTeamViewModel
    {
        [Required(ErrorMessage = "A user is required to join a team")]
        public int UserId { get; set; }
    }
}