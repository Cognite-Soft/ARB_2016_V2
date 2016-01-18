using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Cognite.Arb.Server.WebApi.Security
{
    public class LocalCallAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!IsLocalCall(actionContext))
                SetNotAuthorizedResponse(actionContext);
            else
                base.OnActionExecuting(actionContext);
        }

        private bool IsLocalCall(HttpActionContext actionContext)
        {
            return actionContext.Request.IsLocal();
        }

        private static void SetNotAuthorizedResponse(HttpActionContext actionContext)
        {
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden);
        }
    }
}
