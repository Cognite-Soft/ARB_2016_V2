namespace Cognite.Arb.Server.Contract
{
    public class ResetToken
    {
        public User User { get; set; }
        public ResetType Type { get; set; }

        public enum ResetType
        {
            Password,
            SecurePhrase,
            Both,
        }
    }
}
