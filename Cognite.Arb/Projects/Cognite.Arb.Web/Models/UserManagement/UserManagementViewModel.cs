using System.Collections.Generic;

namespace Cognite.Arb.Web.Models.UserManagement
{
    public class UserManagementViewModel
    {
        public List<UserViewModel> UserList { get; set; }
        public UserViewModel NewUser { get; set; }

        public UserManagementViewModel()
        {
            this.UserList = new List<UserViewModel>();
            this.NewUser = new UserViewModel();
        }
    }
}