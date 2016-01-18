using Cognite.Arb.Server.Contract;
using Cognite.Arb.Web.Core.Authorization.Interfaces;
using Cognite.Arb.Web.ServiceClient;
using System;
using System.Security.Principal;
using System.Web.Mvc;

namespace Cognite.Arb.Web.Core.Authorization
{
    internal class UserIdentity : IIdentity, IUserProvider
    {
        public User User { get; set; }

        public string AuthenticationType
        {
            get { return typeof(User).ToString(); }
        }

        public bool IsAuthenticated
        {
            get { return this.User != null; }
        }

        public string Name
        {
            get 
            {
                if (this.User != null)
                    return String.Format("{0} {1}", this.User.FirstName, this.User.LastName);

                return "anonymous";
            }
        }
        
        public void Init(string securityToken)
        {
            if (!String.IsNullOrEmpty(securityToken))
                this.User = GetUser(securityToken);
        }

        private User GetUser(string securityToken)
        {
            var service = DependencyResolver.Current.GetService<IServiceClient>();
            return service.GetUserBySecurityToken(securityToken);
        }
    }
}
