using System.Web.Http.Filters;

namespace Cognite.Arb.Server.WebApi.ExceptionHandling
{
    public interface IExceptionLogger
    {
        void Log(HttpActionExecutedContext context);
    }
}
