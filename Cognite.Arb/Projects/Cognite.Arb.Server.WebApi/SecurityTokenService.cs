using Cognite.Arb.Server.Business.Database;
using Cognite.Arb.Server.WebApi.Security;

namespace Cognite.Arb.Server.WebApi
{
    public class SecurityTokenService : ISecurityTokenService
    {
        private static readonly UserHeader _systemUser = new UserHeader { Role = Role.System };

        public object GetUser(string securityToken)
        {
            if (securityToken == "arb1234567890system")
                return _systemUser;

            var client = new Arb.WebApi.Resource.Sts.SecurityTokenService();
            var userHeader = client.Get(securityToken);
            if (userHeader != null) return userHeader;
            var loginUserHeader = client.GetLogin(securityToken);
            return loginUserHeader == null ? null : new LoginUserHeaderProxy(loginUserHeader);
        }

        public object GetRole(object user)
        {
            if (user == null) return null;
            var typedUser = (UserHeader) user;
            return typedUser.Role;
        }

        private class LoginUserHeaderProxy : UserHeader
        {
            public LoginUserHeaderProxy(UserHeader user)
            {
                Id = user.Id;
                Email = user.Email;
                FirstName = user.FirstName;
                LastName = user.LastName;

                Role = Role.None;
            }
        }
    }
}