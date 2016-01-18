using Cognite.Arb.Web.Core;
using Cognite.Arb.Web.Models;
using Cognite.Arb.Web.Core.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cognite.Arb.Web.Models.Additional;
using Cognite.Arb.Web.Models.Complaints;
using Cognite.Arb.Server.Contract;

namespace Cognite.Arb.Web.Controllers
{
    public partial class ComplaintsController : BaseController
    {
        // POST: /Complaints/Close/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Close(int id)
        {
            var result = new JsonOperationModel(false, String.Empty);

            try
            {
                this.Service.CloseCase(this.SecurityToken, id);
                result.Result = "Close";
                result.IsSucceeded = true;
            }
            catch (ForbiddenException) { result.Result = GlobalStrings.Forbidden; }
            catch (CaseDoesNotExistException) { result.Result = GlobalStrings.CaseDoesNotExist; }
            catch (Exception) { result.Result = GlobalStrings.SomethingWentWrong; }

            return Json(result);
        }

        // POST: /Complaints/Clone/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Clone(int id)
        {
            var result = new JsonOperationModel(false, String.Empty);

            try
            {
                this.Service.CloneCase(this.SecurityToken, id);
                result.Result = "Clone";
                result.IsSucceeded = true;
            }
            catch (ForbiddenException) { result.Result = GlobalStrings.Forbidden; }
            catch (CaseDoesNotExistException) { result.Result = GlobalStrings.CaseDoesNotExist; }
            catch (Exception) { result.Result = GlobalStrings.SomethingWentWrong; }

            return Json(result);
        }

        // POST: /Complaints/Reopen/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Reopen(int id)
        {
            var result = new JsonOperationModel(false, String.Empty);

            try
            {
                this.Service.ReopenCase(this.SecurityToken, id);
                result.Result = "Reopen";
                result.IsSucceeded = true;
            }
            catch (ForbiddenException) { result.Result = GlobalStrings.Forbidden; }
            catch (CaseDoesNotExistException) { result.Result = GlobalStrings.CaseDoesNotExist; }
            catch (Exception) { result.Result = GlobalStrings.SomethingWentWrong; }

            return Json(result);
        }
    }
}
