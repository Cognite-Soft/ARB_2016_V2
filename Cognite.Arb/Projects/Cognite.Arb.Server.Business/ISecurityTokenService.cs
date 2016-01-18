using Cognite.Arb.Server.Business.Database;

namespace Cognite.Arb.Server.Business
{
    public interface ISecurityTokenService
    {
        string Add(UserHeader user);
        string AddLogin(UserHeader userData);
        UserHeader Get(string token);
        UserHeader GetLogin(string token);
        void Remove(string token);
        void RemoveLogin(string token);
    }
}
