using System.Configuration;
using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http;
using Cognite.Arb.Server.WebApi.ExceptionHandling;

namespace Cognite.Arb.Server.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.Filters.Add(new MappedExceptionFilterAttribute(GetExceptionLogger()));
            config.EnableSystemDiagnosticsTracing();
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }

        private static FileExceptionLogger GetExceptionLogger()
        {
            var path = ConfigurationManager.AppSettings["FileExceptionLoggerPath"];
            return string.IsNullOrEmpty(path) ? null : new FileExceptionLogger(path);
        }
    }
}
