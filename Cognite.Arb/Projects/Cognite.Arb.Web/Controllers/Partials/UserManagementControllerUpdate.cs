using Cognite.Arb.Web.Core;
using Cognite.Arb.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cognite.Arb.Web.Core.Authorization;
using Cognite.Arb.Server.Contract;
using Cognite.Arb.Web.Models.UserManagement;
using Cognite.Arb.Web.Core.Mappers;

namespace Cognite.Arb.Web.Controllers
{
    public partial class UserManagementController : BaseController
    {
        // GET: UserManagement/Update
        public ActionResult Update()
        {
            return RedirectToAction("Index");
        }

        // POST: UserManagement/Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(UserViewModel model)
        {
            if (ModelState.IsValid)
                return TryUpdateUser(model);

            return IndexUpdate(model);
        }

        private ActionResult TryUpdateUser(UserViewModel model)
        {
            try
            {
                var user = Mappers.MapUserManagementModelToUser(model);
                this.Service.UpdateUser(this.SecurityToken, user);
                return IndexUpdate(model);
            }
            catch (UserDoesNotExistException)
            {
                AddModelStateError(GlobalStrings.UserDoesNotExists);
            }
            catch (Exception)
            {
                AddModelStateError(GlobalStrings.SomethingWentWrong);
            }

            return IndexUpdate(model);
        }

        private ActionResult IndexUpdate(UserViewModel model)
        {
            return PartialView("Details", model);
        }

        private void UpdateUserModel(UserManagementViewModel indexModel, UserViewModel userModel)
        {
            var user = (from u in indexModel.UserList
                        where u.Id.Equals(userModel.Id)
                        select u).FirstOrDefault();

            if (user != null)
                UpdateUserModel(user, userModel);
        }

        private void UpdateUserModel(UserViewModel targetUser, UserViewModel sourceUser)
        {
            targetUser.Email = sourceUser.Email;
            targetUser.FirstName = sourceUser.FirstName;
            targetUser.LastName = sourceUser.LastName;
            targetUser.SelectedRole = sourceUser.SelectedRole;
        }
    }
}
