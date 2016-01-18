using System;

namespace Cognite.Arb.Server.WebApi.Security
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute
    {
        public AuthorizeAttribute(params object[] roles)
        {
            Roles = roles;
        }

        public object[] Roles { get; set; }
    }
}