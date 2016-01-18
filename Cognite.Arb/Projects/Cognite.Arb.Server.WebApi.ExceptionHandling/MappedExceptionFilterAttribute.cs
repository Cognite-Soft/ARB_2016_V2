using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace Cognite.Arb.Server.WebApi.ExceptionHandling
{
    public class MappedExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly IExceptionLogger _exceptionLogger;

        public MappedExceptionFilterAttribute(IExceptionLogger exceptionLogger = null, bool returnUnhandledException = false)
        {
            _exceptionLogger = exceptionLogger ?? new NullExceptionLogger();
            DefaultMapping = new MapException(null, HttpStatusCode.InternalServerError, returnUnhandledException);
        }

        public override void OnException(HttpActionExecutedContext context)
        {
            _exceptionLogger.Log(context);
            var mappings = GetMappings(context);
            var exception = context.Exception;
            var exceptionType = exception.GetType();
            var mapping = mappings.FirstOrDefault(item => item.ExceptionType.IsAssignableFrom(exceptionType)) ?? DefaultMapping;
            SetResponse(context, mapping.HttpStatusCode, mapping.ReturnException ? exception : null);
        }

        private MapException DefaultMapping { get; set; }

        private static void SetResponse(HttpActionExecutedContext context, HttpStatusCode httpStatusCode, Exception exception)
        {
            context.Response = exception != null
                ? context.Request.CreateResponse(httpStatusCode, exception)
                : context.Request.CreateResponse(httpStatusCode);
        }

        private static IEnumerable<MapException> GetMappings(HttpActionExecutedContext context)
        {
            return context.ActionContext.ActionDescriptor.GetCustomAttributes<MapException>();
        }

        private class NullExceptionLogger : IExceptionLogger
        {
            public void Log(HttpActionExecutedContext context)
            {
            }
        }
    }
}