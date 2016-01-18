using System.Security.Principal;
using Cognite.Arb.Web.ServiceClient;

namespace Cognite.Arb.Web.Core.Authorization
{
    internal class UserProvider : IPrincipal
    {
        private UserIdentity _userIdentity { get; set; }

        public IIdentity Identity { get { return this._userIdentity; } }

        public UserProvider(string secureToken)
        {
            this._userIdentity = new UserIdentity();
            this._userIdentity.Init(secureToken);
        }

        public bool IsInRole(string roles)
        {
            if (this._userIdentity.User == null)
                return false;

            return this._userIdentity.User.InRoles(roles);
        }
        
        public override string ToString()
        {
            return this._userIdentity.Name;
        }
    }
}