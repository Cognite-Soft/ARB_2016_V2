using System.Collections.Generic;
using System.ServiceModel;
using Cognite.Arb.Sts.Business;
using Cognite.Arb.Sts.Contract;

namespace Cognite.Arb.Server.Sts.Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)] 
    public class SecurityTokenService : ISecurityTokenService
    {
        public string Add(KeyValuePair<string, string>[] user)
        {
            return SecurityTokenServiceManager.Add(user);
        }

        public KeyValuePair<string, string>[] Get(string token)
        {
            return SecurityTokenServiceManager.Get(token);
        }

        public void Remove(string token)
        {
            SecurityTokenServiceManager.Remove(token);
        }
    }
}
