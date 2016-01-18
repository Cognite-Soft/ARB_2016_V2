using System;

namespace Cognite.Arb.Server.WebApi.Security
{
    public class CurrentUser
    {
        [ThreadStatic] private static object _user;

        public static void Set<T>(T user) where T : class
        {
            _user = user;
        }

        public static T Get<T>() where T : class
        {
            return (T) _user;
        }
    }
}