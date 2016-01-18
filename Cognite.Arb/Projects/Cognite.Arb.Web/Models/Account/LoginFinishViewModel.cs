using Cognite.Arb.Web.Core;
using System.ComponentModel.DataAnnotations;

namespace Cognite.Arb.Web.Models.Account
{
    public class LoginFinishViewModel
    {
        [DataType(DataType.Password)]
        [Display(Name = "Phrase First Character")]
        [StringLength(1, MinimumLength = 1, ErrorMessage = GlobalStrings.OneSymbolForFirstCharacter)]
        public string PhraseFirstChar { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Phrase Second Character")]
        [StringLength(1, MinimumLength = 1, ErrorMessage = GlobalStrings.OneSymbolForSecondCharacter)]
        public string PhraseSecondChar { get; set; }

        public string PhraseFirstCharNumber { get; set; }
        public string PhraseSecondCharNumber { get; set; }
        public string SecurityToken { get; set; }
    }
}