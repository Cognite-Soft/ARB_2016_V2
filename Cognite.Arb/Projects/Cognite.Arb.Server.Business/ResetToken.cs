using System;

namespace Cognite.Arb.Server.Business.Database
{
    public class ResetToken
    {
        public Guid UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpirationTime { get; set; }
        public ResetTokenType Type { get; set; }
    }
}
