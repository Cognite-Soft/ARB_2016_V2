using System;
using System.Linq;
using Cognite.Arb.Server.Contract;

namespace Cognite.Arb.Web.ServiceClient
{
    public static class UserExtension
    {
        public static string FullName(this User user) { return user.FirstName + " " + user.LastName; }

        public static bool InRoles(this User user, string roles)
        {
            if (String.IsNullOrEmpty(roles))
                return false;

            return CheckRoles(user, roles);
        }

        private static bool CheckRoles(User user, string roles)
        {
            var rolesArray = roles.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            return rolesArray.Any(role => user.Role.ToString().Equals(role));
        }
    }
}
