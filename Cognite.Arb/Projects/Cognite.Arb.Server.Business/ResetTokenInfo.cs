using Cognite.Arb.Server.Business.Database;

namespace Cognite.Arb.Server.Business
{
    public class ResetTokenInfo
    {
        public UserHeader User { get; set; }
        public ResetTokenType Type { get; set; }
    }
}
