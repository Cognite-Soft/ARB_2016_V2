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
        // GET: UserManagement/Create
        public ActionResult Create()
        {
            return RedirectToAction("Index");
        }

        // POST: UserManagement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserViewModel model)
        {
            if (ModelState.IsValid)
                return TryCreateUser(model);

            return IndexErrorCreate(model);
        }

        private ActionResult TryCreateUser(UserViewModel model)
        {
            try
            {
                var newUser = CreateUser(this.SecurityToken, model);
                TryAssignCase(this.SecurityToken, newUser.Id, model.SelectedCase);
                return RedirectToAction("Index");
            }
            catch (DuplicateUserException)
            {
                AddModelStateError(GlobalStrings.DuplicatedUser);
            }
            catch (Exception)
            {
                AddModelStateError(GlobalStrings.SomethingWentWrong);
            }

            return IndexErrorCreate(model);
        }

        private User CreateUser(string token, UserViewModel model)
        {
            var user = Mappers.MapUserManagementModelToUser(model);
            return this.Service.CreateUser(token, user);
        }

        private void TryAssignCase(string token, Guid userId, string caseId)
        {
            try
            {
                AssignCase(token, userId, Int32.Parse(caseId));
            }
            catch (UserDoesNotExistException)
            {
                AddTempData(GetAssigninError(GlobalStrings.UserDoesNotExists));
            }
            catch (ForbiddenException)
            {
                AddTempData(GetAssigninError(GlobalStrings.Forbidden));
            }
            catch (Exception)
            {
                AddTempData(GetAssigninError(GlobalStrings.SomethingWentWrong));
            }
        }

        private void AddTempData(string errorMessage)
        {
            TempData[ArbConstants.AssignCaseErrorTempDataKey] = errorMessage;
        }

        private string GetAssigninError(string baseString)
        {
            return String.Format(GlobalStrings.AssigningWhileCreatingErrorTemplate, baseString);
        }

        private void AssignCase(string token, Guid userId, int caseId)
        {
            if (caseId == -1)
                return;

            this.Service.UpdateUserAssigments(token, userId, caseId);
        }

        private ActionResult IndexErrorCreate(UserViewModel model)
        {
            var modelWithError = CreateBaseUserManagementVewModel();
            MapExistingUserModel(modelWithError, model);
            ViewBag.ShowUserCreation = true;
            return View("Index", modelWithError);
        }

        private void MapExistingUserModel(UserManagementViewModel modelWithError, UserViewModel model)
        {
            modelWithError.NewUser.FirstName = model.FirstName;
            modelWithError.NewUser.LastName = model.LastName;
            modelWithError.NewUser.Email = model.Email;
            modelWithError.NewUser.SelectedCase = model.SelectedCase;
            modelWithError.NewUser.SelectedRole = model.SelectedRole;
        }
    }
}
