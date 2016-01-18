using Cognite.Arb.Server.Contract;
using Cognite.Arb.Web.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Web;
using System.Web.Mvc;

namespace Cognite.Arb.Web.Core
{
    public static class RolesHelper
    {
        public static List<SelectListItem> GetRolesSelectListItems()
        {
            var result = new List<SelectListItem>();

            result.Add(new SelectListItem()
            {
                Value = String.Empty,
                Text = "Choose a user Role",
            });

            foreach (var value in Enum.GetValues(typeof(Role)))
            {
                if ((Role)value == Role.System)
                    continue;

                var str = value.ToString();
                result.Add(new SelectListItem()
                {
                    Value = str,
                    Text = Resources.ResourceManager.GetString(str),
                });
            }

            return result;
        }
    }
}