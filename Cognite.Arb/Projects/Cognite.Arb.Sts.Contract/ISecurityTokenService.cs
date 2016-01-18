using System.Collections.Generic;
using System.ServiceModel;

namespace Cognite.Arb.Sts.Contract
{
    [ServiceContract]
    public interface ISecurityTokenService
    {
        [OperationContract]
        string Add(KeyValuePair<string, string>[] user);

        [OperationContract]
        KeyValuePair<string, string>[] Get(string token);

        [OperationContract]
        void Remove(string token);
    }
}
