using Cognite.Arb.Web.Core.Authorization.Interfaces;
using System;

namespace Cognite.Arb.Web.Core.Authorization
{
    public static class ArbAuthenticationExtensions
    {
        public static string GetSecurityToken(this IAuthentication obj)
        {
            var arbAuthObject = obj as ArbAuthentication;
            if (arbAuthObject != null)
                return arbAuthObject.CurrentUserToken;

            return String.Empty;
        }
    }
}