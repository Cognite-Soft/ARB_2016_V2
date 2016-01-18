using Cognite.Arb.Web.Core;
using System.ComponentModel.DataAnnotations;

namespace Cognite.Arb.Web.Models.Account
{
    public class LoginStartViewModel
    {
        [Required(ErrorMessage = GlobalStrings.EmailIsRequired)]
        [EmailAddress(ErrorMessage = GlobalStrings.EmailIsNotValid)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = GlobalStrings.PasswordIsRequired)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }    
}
