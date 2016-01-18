using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cognite.Arb.Web.Core.Errors
{
    internal static class AccountErrors
    {
        public const string EmailIsRequired = "The Email is required";
        public const string EmailIsNotValid = "The Email is not a valid e-mail address";
        public const string PasswordIsRequired = "The Password is required";
        public const string OneSymbolForFirstCharacter = "One symbol is required for the First character";
        public const string OneSymbolForSecondCharacter = "One symbol is required for the Second character";
        public const string PasswordLength = "The {0} must be at least {2} characters long";
        public const string PasswordConfirmation = "Password confirmation must be equal to password";
        public const string SecurePhraseIsRequired = "The Secure phrase is required";
        public const string SecurePhraseConfirmation = "Secure phrase confirmation must be equal to secure phrase";
        public const string SecurePhraseSymbolsAreInvalid = "Secure phrase symbols are invalid";
        public const string UserNameOrPasswordIsInvalid = "The user name or password is invalid";
        public const string FirstLoginStepMustBeDoneBeforeResetSecurePhrase = "First Login step must be done before you could request Secure Phrase reset";
        public const string ResetTokenIsInvalid = "Your request has expired or invalid";
    }
}