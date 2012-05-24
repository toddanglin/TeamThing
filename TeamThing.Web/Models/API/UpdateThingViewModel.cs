using System.ComponentModel.DataAnnotations;

namespace TeamThing.Web.Models.API
{
    public class UpdateThingViewModel
    {
        public string Description { get; set; }
        [Required(ErrorMessage = "A thing must have an editor")]
        public int EditedById { get; set; }
        [Required(ErrorMessage = "A thing must be assigned to 1 or more people")]
        public int[] AssignedTo { get; set; }
    }
}