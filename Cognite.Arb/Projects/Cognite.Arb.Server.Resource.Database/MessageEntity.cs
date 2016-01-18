using Cognite.Arb.Server.Business;

namespace Cognite.Arb.Server.Resource.Database
{
    public class MessageEntity : Message
    {
        public virtual UserEntity FromUser { get; set; }
    }
}
