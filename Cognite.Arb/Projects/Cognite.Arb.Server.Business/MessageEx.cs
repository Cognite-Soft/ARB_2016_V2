using Cognite.Arb.Server.Business.Database;

namespace Cognite.Arb.Server.Business
{
    public class MessageEx : Message
    {
        public UserHeader FromUser { get; set; }
    }
}
