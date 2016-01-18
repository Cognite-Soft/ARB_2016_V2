using Cognite.Arb.Web.Core;
using System.ComponentModel.DataAnnotations;

namespace Cognite.Arb.Web.Models.Account
{
    public class ForgotPasswordPhraseViewModel
    {
        [Required(ErrorMessage = GlobalStrings.EmailIsRequired)]
        [EmailAddress(ErrorMessage = GlobalStrings.EmailIsNotValid)]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}