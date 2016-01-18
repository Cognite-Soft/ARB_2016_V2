using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Cognite.Arb.Server.WebApi.Security
{
    public class AuthorizationRequiredAttribute : ActionFilterAttribute
    {
        public static ISecurityTokenService SecurityTokenService { get; set; }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!Authorize(actionContext))
                SetNotAuthorizedResponse(actionContext);
            else
                base.OnActionExecuting(actionContext);
        }

        #region Private

        private static bool Authorize(HttpActionContext actionContext)
        {
            var allowedRoles = GetAllowedRolesFromAction(actionContext);
            if (!allowedRoles.Any()) return true;
            var securityToken = GetSecurityTokenFromRequest(actionContext.Request);
            if (securityToken == null) return false;
            var role = GetRoleBySecurityToken(securityToken);
            if (role == null) return false;
            var allowed = allowedRoles.Any(item=>item.ToString() == role.ToString());
            return allowed;
        }

        private static object[] GetAllowedRolesFromAction(HttpActionContext actionContext)
        {
            var attributes = GetApplicableAuthorizeAttributes(actionContext);
            var allowedRoles = attributes.SelectMany(item => item.Roles).ToArray();
            return allowedRoles;
        }

        private static IEnumerable<AuthorizeAttribute> GetApplicableAuthorizeAttributes(HttpActionContext actionContext)
        {
            var actionAttributes = GetActionAuthorizeAttributes(actionContext);
            if (actionAttributes.Count != 0) return actionAttributes;
            return GetControllerAuthorizeAttributes(actionContext);
        }

        private static IEnumerable<AuthorizeAttribute> GetControllerAuthorizeAttributes(HttpActionContext actionContext)
        {
            return actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AuthorizeAttribute>();
        }

        private static Collection<AuthorizeAttribute> GetActionAuthorizeAttributes(HttpActionContext actionContext)
        {
            return actionContext.ActionDescriptor.GetCustomAttributes<AuthorizeAttribute>();
        }

        private static string GetSecurityTokenFromRequest(HttpRequestMessage request)
        {
            const string securityTokenHeaderName = "SecurityToken";
            if (!request.Headers.Contains(securityTokenHeaderName)) return null;
            var securityToken = request.Headers.GetValues(securityTokenHeaderName).FirstOrDefault();
            return securityToken;
        }

        private static object GetRoleBySecurityToken(string securityToken)
        {
            var user = SecurityTokenService.GetUser(securityToken);
            CurrentUser.Set(user);
            if (user == null) return null;
            var role = SecurityTokenService.GetRole(user);
            return role;
        }

        private static void SetNotAuthorizedResponse(HttpActionContext actionContext)
        {
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden);
        }

        #endregion
    }
}