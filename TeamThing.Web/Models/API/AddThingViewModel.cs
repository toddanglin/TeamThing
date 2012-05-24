using System.ComponentModel.DataAnnotations;

namespace TeamThing.Web.Models.API
{
    public class AddThingViewModel
    {
        public string Description { get; set; }
        [Required(ErrorMessage = "A thing must have a creator")]
        public int CreatedById { get; set; }
        [Required(ErrorMessage = "A thing must have a an associated team")]
        public int TeamId { get; set; }
        [Required(ErrorMessage = "A thing must be assigned to 1 or more people")]
        public int[] AssignedTo { get; set; }
    }
}