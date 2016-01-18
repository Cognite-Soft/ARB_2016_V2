using System;
using System.Collections.Generic;
using System.Linq;
using Cognite.Arb.Server.Business;
using Cognite.Arb.Server.Business.Database;
using Cognite.Arb.Sts.Client;

namespace Cognite.Arb.WebApi.Resource.Sts
{
    public class SecurityTokenService : ISecurityTokenService
    {
        private const string IdProperty = "Id";
        private const string EMailProperty = "Email";
        private const string RoleProperty = "Role";
        private const string FirstNameProperty = "FirstName";
        private const string LastNameProperty = "LastName";
        private const string IsLoginProperty = "IsLogin";

        public string Add(UserHeader user)
        {
            using (var client = new SecurityTokenServiceClient())
            {
                var token = client.Add(SerializeUser(user));
                client.Close();
                return token;
            }
        }

        public string AddLogin(UserHeader user)
        {
            using (var client = new SecurityTokenServiceClient())
            {
                var token = client.Add(
                    SerializeUser(user).Concat(
                        new[] { new KeyValuePair<string, string>(IsLoginProperty, "true") }).ToArray());
                client.Close();
                return token;
            }
        }

        public UserHeader Get(string token)
        {
            return GetImpl(token, false);
        }

        private UserHeader GetImpl(string token, bool isLogin)
        {
            using (var client = new SecurityTokenServiceClient())
            {
                var propertyBag = client.Get(token);
                client.Close();
                if (propertyBag == null) return null;
                if (propertyBag.Any(item => item.Key == IsLoginProperty) == !isLogin) return null;
                return DeserializeUser(propertyBag);
            }
        }

        public UserHeader GetLogin(string token)
        {
            return GetImpl(token, true);
        }

        public void Remove(string token)
        {
            using (var client = new SecurityTokenServiceClient())
            {
                client.Remove(token);
                client.Close();
            }
        }

        public void RemoveLogin(string token)
        {
            using (var client = new SecurityTokenServiceClient())
            {
                client.Remove(token);
                client.Close();
            }
        }

        private static Role ParseRole(string value)
        {
            Role role;
            if (!Enum.TryParse(value, out role)) throw new ArgumentException();
            return role;
        }

        private static KeyValuePair<string, string>[] SerializeUser(UserHeader user)
        {
            return new[]
            {
                new KeyValuePair<string, string>(IdProperty, user.Id.ToString()),
                new KeyValuePair<string, string>(EMailProperty, user.Email),
                new KeyValuePair<string, string>(RoleProperty, user.Role.ToString()),
                new KeyValuePair<string, string>(FirstNameProperty, user.FirstName),
                new KeyValuePair<string, string>(LastNameProperty, user.LastName),
            };
        }

        private static UserHeader DeserializeUser(KeyValuePair<string, string>[] propertyBag)
        {
            return new UserHeader
            {
                Id = new Guid(propertyBag.First(item => item.Key == IdProperty).Value),
                Email = propertyBag.First(item => item.Key == EMailProperty).Value,
                Role = ParseRole(propertyBag.First(item => item.Key == RoleProperty).Value),
                FirstName = propertyBag.First(item => item.Key == FirstNameProperty).Value,
                LastName = propertyBag.First(item => item.Key == LastNameProperty).Value,
            };
        }
    }
}