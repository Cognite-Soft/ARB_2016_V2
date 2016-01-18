using Cognite.Arb.Server.Contract;
using Cognite.Arb.Web.Models.UserManagement;
using Cognite.Arb.Web.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Cognite.Arb.Web.ServiceClient;
using Cognite.Arb.Web.Models.Additional;
using Newtonsoft.Json;

namespace Cognite.Arb.Web.Core.Mappers
{
    internal static partial class Mappers
    {
        internal static UserViewModel MapUserToUserViewModel(User user)
        {
            var role = user.Role.ToString();
            return new UserViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                SelectedRole = user.Role.ToString(),
                FullName = user.FullName().Trim(' '),
                // disabling role changing
                Roles = new List<SelectListItem>() { new SelectListItem() { Value = role, Text = Resources.ResourceManager.GetString(role) } },
            };
        }

        internal static User MapUserManagementModelToUser(UserViewModel model)
        {
            return new User()
            {
                Id = model.Id,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Role = model.SelectedRole.GetRole(),
            };
        }

        internal static List<UserViewModel> MapUsersToUserViewModel(User[] users)
        {
            var mappedUsers = new List<UserViewModel>();

            foreach (var user in users)
                mappedUsers.Add(Mappers.MapUserToUserViewModel(user));

            return mappedUsers;
        }
        
        internal static string MapUsersToJsonIdValueModel(User[] users)
        {
            var result = from user in users
                         select new GuidStringModel()
                         {
                             Id = user.Id,
                             Value = user.FullName(),
                         };

            return JsonConvert.SerializeObject(result);
        }

        private static Role GetRole(this string roleString)
        {
            var role = Role.ThirdPartyReviewer;
            Enum.TryParse(roleString, out role);
            return role;
        }
    }
}