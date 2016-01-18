using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cognite.Arb.Web.Core
{
    public static class GlobalStrings
    {
        public const string EmailIsRequired = "Email is required";
        public const string EmailIsNotValid = "Email is not a valid e-mail address";

        public const string PasswordIsRequired = "Password is required";
        public const string PasswordLength = "The {0} must be at least {2} characters long";
        public const string PasswordConfirmation = "Password confirmation must be equal to password";

        public const string MessageIsRequired = "Notification message is required";
        public const string NotificationTypeIsRequired = "Select notification type please";

        public const string OneSymbolForFirstCharacter = "One symbol is required for the First character";
        public const string OneSymbolForSecondCharacter = "One symbol is required for the Second character";

        public const string SecurePhraseIsRequired = "Secure phrase is required";
        public const string SecurePhraseConfirmation = "Secure phrase confirmation must be equal to secure phrase";
        public const string SecurePhraseSymbolsAreInvalid = "Secure phrase symbols are invalid";

        public const string UserNameOrPasswordIsInvalid = "User name or password is invalid";

        public const string FirstLoginStepMustBeDoneBeforeResetSecurePhrase = "First Login step must be done before you could request Secure Phrase reset";

        public const string ResetTokenIsInvalid = "Your reset token has expired or invalid";

        public const string SomethingWentWrong = "Something went wrong. Please try again";

        public const string FirstNameIsRequired = "First Name is required";
        public const string LastNameIsRequired = "Last Name is required";
        public const string RoleIsRequired = "User Role is required";
        public const string CaseIsRequired = "Assign to case is required";

        public const string UserDeletedSuccessfully = "User deleted successfully";
        public const string UserUpdatedSuccessfully = "User updated successfully";
        public const string UserResetSecurePhraseRequested = "{0} will reseive a mail with password reset instructions";
        public const string UserResetPasswordRequested = "{0} will reseive a mail with secure phrase reset instructions";
        public const string DuplicatedUser = "User with the same email already exists";

        public const string Forbidden = "Forbidden";
        public const string UserDoesNotExists = "User does not found";
        public const string WeakPassword = "Your password does not meet policy requirements";
        public const string WeakSecurePhrase = "Your secure phrase does not meet policy requirements";
        public const string YouNeedToEnterMailAndPasswordFirst = "You need to enter mail and password first";

        public const string SuccessfullyUpdatedSchedule = "Successfully updated to '{0}'";
        public const string AssignmentSuccessfullyChanged = "Assignment Successfully Changed";
        public const string IncorrectData = "Incorrect data was sent";

        public const string AssigningWhileCreatingErrorTemplate = "Error while assigning case: {0}";
        public const string CaseDoesNotExist = "Requested case does not exist";

        public const string MessageNoBackground = "No background";
        public const string MessageNoIdealOutcome = "No outcome";
        public const string MessageNoRelationship = "No relationship";

        public const string BooleanTrueText = "Yes";
        public const string BooleanFalseText = "No";

        public const string AllegationDeleteError = "Sory, the allegation can not be deleted at the moment";
        public const string DateAndDetailDeleteError = "Sory, the date and detail can not be deleted at the moment";
        public const string CanNotAddAllegationComment = "Cannot add allegation comment at the moment, please try again later";
        public const string CanNotAddPreliminaryDecisionComment = "Cannot add a comment at the moment, please try again later";
        public const string CanNotAddDecision = "Cannot add a decision, please try again later";
        public const string CanNotAddComment = "Cannot add a comment, please try again later";
        public const string CanNotCreatePost = "Cannot create a new post, please try again later";
        public const string CanNotAddReplyOnPost = "Cannot add your reply, please try again later";
        public const string NoSuchFile = "File does not exists or not uploaded yet";
    }
}