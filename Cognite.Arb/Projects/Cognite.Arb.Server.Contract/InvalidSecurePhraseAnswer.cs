using System;
using System.Net;

namespace Cognite.Arb.Server.Contract
{
    public class InvalidSecurePhraseAnswer : Exception
    {
        public static HttpStatusCode HttpStatusCode = HttpStatusCode.Unauthorized;
    }
}