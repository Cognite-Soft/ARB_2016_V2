namespace Cognite.Arb.Server.WebApi.Security
{
    public interface ISecurityTokenService
    {
        object GetUser(string securityToken);
        object GetRole(object user);
    }
}
