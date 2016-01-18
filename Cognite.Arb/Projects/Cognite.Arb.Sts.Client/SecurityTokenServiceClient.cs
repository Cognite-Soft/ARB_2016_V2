using System.Collections.Generic;
using System.ServiceModel;
using Cognite.Arb.Sts.Contract;

namespace Cognite.Arb.Sts.Client
{
    public class SecurityTokenServiceClient : ClientBase<ISecurityTokenService>, ISecurityTokenService
    {
        public string Add(KeyValuePair<string, string>[] user)
        {
            return Channel.Add(user);
        }

        public KeyValuePair<string, string>[] Get(string token)
        {
            return Channel.Get(token);
        }

        public void Remove(string token)
        {
            Channel.Remove(token);
        }
    }
}
