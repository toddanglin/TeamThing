using System;
using System.ComponentModel.DataAnnotations;

namespace TeamThing.Web.Models.API
{
    [Obsolete]
    public class AddUserModel
    {
        [Required(ErrorMessage = "Email Address is required!")]
        [StringLength(255, MinimumLength = 5, ErrorMessage = "Email address must be between 5 and 255 characters")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email Address")]
        public string EmailAddress { get; set; }
        //    set;
        //}
        //    get;
        //{
        //public string Password
        //[StringLength(255, MinimumLength = 5, ErrorMessage = "Passwords must be between 5 and 255 characters")]
//[Required(ErrorMessage = "Password required!")]
      }
 }