using System;
using System.Net;

namespace Cognite.Arb.Server.WebApi.ExceptionHandling
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class MapException : Attribute
    {
        public MapException(Type exceptionType, HttpStatusCode httpStatusCode, bool returnException = false)
        {
            ExceptionType = exceptionType;
            HttpStatusCode = httpStatusCode;
            ReturnException = returnException;
        }

        public Type ExceptionType { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
        public bool ReturnException { get; set; }
    }
}