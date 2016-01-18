using Cognite.Arb.Server.Business.Database;

namespace Cognite.Arb.Server.Business
{
    public class AuthenticationResult
    {
        public string SecurityToken { get; set; }
        public Role Role { get; set; }
    }
}
