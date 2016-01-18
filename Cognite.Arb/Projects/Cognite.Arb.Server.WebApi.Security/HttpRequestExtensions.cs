using System;
using System.Net.Http;

namespace Cognite.Arb.Server.WebApi.Security
{
    public static class HttpRequestExtensions
    {
        public static bool IsLocal(this HttpRequestMessage request)
        {
            var flag = request.Properties["MS_IsLocal"] as Lazy<bool>;
            return flag != null && flag.Value;
        }
    }
}
