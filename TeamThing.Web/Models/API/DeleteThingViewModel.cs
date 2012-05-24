using System.ComponentModel.DataAnnotations;

namespace TeamThing.Web.Models.API
{
    public class DeleteThingViewModel
    {
        [Required(ErrorMessage = "A thing can only be deleted by a valid user.")]
        public int DeletedById { get; set; }
    }
}