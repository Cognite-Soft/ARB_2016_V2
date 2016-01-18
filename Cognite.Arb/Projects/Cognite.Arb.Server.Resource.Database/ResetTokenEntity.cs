using Cognite.Arb.Server.Business.Database;

namespace Cognite.Arb.Server.Resource.Database
{
    public class ResetTokenEntity : ResetToken
    {
        public virtual UserEntity User { get; set; }
    }
}
