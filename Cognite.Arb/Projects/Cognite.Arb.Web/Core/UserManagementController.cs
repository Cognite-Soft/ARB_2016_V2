using Cognite.Arb.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cognite.Arb.Web.Core.Authorization;
using Cognite.Arb.Web.Core;
using Cognite.Arb.Server.Contract;
using Cognite.Arb.Web.ServiceClient;
using Cognite.Arb.Web.Models.Additional;

namespace Cognite.Arb.Web.Controllers
{
    [Authorize(Roles = "Admin,CaseWorker")]
    public partial class UserManagementController : BaseController
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ResetPassword(Guid identifier)
        {
            var opResult = new JsonOperationModel(false, String.Empty);

            try
            {
                var user = this.Service.GetUser(this.SecurityToken, identifier);
                this.Service.InitiateResetPassword(user.Email);

                opResult.Result = String.Format(GlobalStrings.UserResetPasswordRequested, user.FullName());
                opResult.IsSucceeded = true;
            }
            catch (UserDoesNotExistException)
            {
                opResult.Result = GlobalStrings.UserDoesNotExists;
            }
            catch (Exception)
            {
                opResult.Result = GlobalStrings.SomethingWentWrong;
            }

            return Json(opResult);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ResetSecurePhrase(Guid identifier)
        {
            var opResult = new JsonOperationModel(false, String.Empty);

            try
            {
                var user = this.Service.GetUser(this.SecurityToken, identifier);
                this.Service.InitiateResetSecurePhrase(this.SecurityToken, user.Email);

                opResult.Result = String.Format(GlobalStrings.UserResetSecurePhraseRequested, user.FullName());
                opResult.IsSucceeded = true;
            }
            catch (UserDoesNotExistException)
            {
                opResult.Result = GlobalStrings.UserDoesNotExists;
            }
            catch (Exception)
            {
                opResult.Result = GlobalStrings.SomethingWentWrong;
            }

            return Json(opResult);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteUser(Guid identifier)
        {
            var opResult = new JsonOperationModel(false, String.Empty);

            try
            {
                this.Service.DeleteUser(this.SecurityToken, identifier);

                opResult.Result = GlobalStrings.UserDeletedSuccessfully;
                opResult.IsSucceeded = true;
            }
            catch (UserDoesNotExistException)
            {
                opResult.Result = GlobalStrings.UserDoesNotExists;
            }
            catch (Exception)
            {
                opResult.Result = GlobalStrings.SomethingWentWrong;
            }

            return Json(opResult);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UpdateUserCaseAssignment(Guid userId, string value)
        {
            var opResult = new JsonOperationModel(false, String.Empty);

            try
            {
                var caseIds = GetCasesIds(value);
                this.Service.UpdateUserAssigments(this.SecurityToken, userId, caseIds);

                opResult.Result = GlobalStrings.AssignmentSuccessfullyChanged;
                opResult.IsSucceeded = true;
            }
            catch (ForbiddenException)
            {
                opResult.Result = GlobalStrings.Forbidden;
            }
            catch (UserDoesNotExistException)
            {
                opResult.Result = GlobalStrings.UserDoesNotExists;
            }
            catch (Exception)
            {
                opResult.Result = GlobalStrings.SomethingWentWrong;
            }

            return Json(opResult);
        }

        private int[] GetCasesIds(string value)
        {
            var caseIds = value.Trim('[', ']').Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            return caseIds.Select(cs => Int32.Parse(cs)).ToArray();
        }
    }
}
