using System;
using System.Net;

namespace Cognite.Arb.Server.Contract
{
    public class InvalidUserOrPasswordException : Exception
    {
        public static HttpStatusCode HttpStatusCode = HttpStatusCode.Unauthorized;
    }
}