namespace Cognite.Arb.Server.Business.Database
{
    public class UserData : UserHeader
    {
        public UserData()
        {
        }

        public UserData(UserHeader user)
        {
            Id = user.Id;
            Email = user.Email;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Role = user.Role;
        }

        public string PasswordSalt { get; set; }
        public string HashedPassword { get; set; }
        public string EncryptedSecurePhrase { get; set; }
        public int? FirstSecurePhraseQuestionCharacterIndex { get; set; }
        public int? SecondSecurePhraseQuestionCharacterIndex { get; set; }
        public UserState UserState { get; set; }
    }
}
