using Cognite.Arb.Web.Core;
using System.ComponentModel.DataAnnotations;

namespace Cognite.Arb.Web.Models.Account
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = GlobalStrings.PasswordIsRequired)]
        [StringLength(100, ErrorMessage = GlobalStrings.PasswordLength, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = GlobalStrings.PasswordConfirmation)]
        public string ConfirmPassword { get; set; }

        public string ResetToken { get; set; }
    }
}