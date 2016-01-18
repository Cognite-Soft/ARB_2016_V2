using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Cognite.Arb.Server.WebApi.Security;
using Cognite.Arb.Server.WebApi2;

namespace Cognite.Arb.Server.WebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthorizationRequiredAttribute.SecurityTokenService = new SecurityTokenService();
        }
    }
}
