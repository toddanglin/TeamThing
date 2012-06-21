using System.ComponentModel.DataAnnotations;

namespace TeamThing.Web.Models.API
{
    public class StarThingViewModel
    {
        [Required(ErrorMessage="A user is required to star a thing")]
        public int UserId { get; set; }
    }
}