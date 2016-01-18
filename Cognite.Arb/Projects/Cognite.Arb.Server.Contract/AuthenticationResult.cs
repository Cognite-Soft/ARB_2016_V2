namespace Cognite.Arb.Server.Contract
{
    public class AuthenticationResult
    {
        public string SecurityToken { get; set; }
        public Role Role { get; set; }
    }
}
