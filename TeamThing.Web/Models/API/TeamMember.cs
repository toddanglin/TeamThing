using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeamThing.Web.Models.API
{
    public class TeamMember
    {
        public int Id { get;set; }
        public string EmailAddress { get;set; }
        public string FullName { get;set; }
        public IList<Thing> Things { get;set; }
    }

    public class AddUserModel
    {
        [Required(ErrorMessage = "Email Address is required!")]
        [StringLength(255, MinimumLength = 5, ErrorMessage = "Email address must be between 5 and 255 characters")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email Address")]
        public string EmailAddress
        {
            get;
            set;
        }
        //[Required(ErrorMessage = "Password required!")]
        //[StringLength(255, MinimumLength = 5, ErrorMessage = "Passwords must be between 5 and 255 characters")]
        //public string Password
        //{
        //    get;
        //    set;
        //}
    }

    public class SignInViewModel
    {
        [Required(ErrorMessage = "Email Address is required!")]
        [StringLength(255, MinimumLength = 5, ErrorMessage = "Email address must be between 5 and 255 characters")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email Address")]
        public string EmailAddress
        {
            get;
            set;
        }
        //[Required(ErrorMessage = "Password required!")]
        //[StringLength(255, MinimumLength = 5, ErrorMessage = "Passwords must be between 5 and 255 characters")]
        //public string Password
        //{
        //    get;
        //    set;
        //}
    }
}