using Cognite.Arb.Web.Core;
using System.ComponentModel.DataAnnotations;

namespace Cognite.Arb.Web.Models.Account
{
    public class ResetSecurePhraseViewModel
    {
        [Required(ErrorMessage = GlobalStrings.SecurePhraseIsRequired)]
        [DataType(DataType.Password)]
        [Display(Name = "Secure phrase")]
        public string SecurePhrase { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm secure phrase")]
        [Compare("SecurePhrase", ErrorMessage = GlobalStrings.SecurePhraseConfirmation)]
        public string ConfirmSecurePhrase { get; set; }

        public string ResetToken { get; set; }
    }
}