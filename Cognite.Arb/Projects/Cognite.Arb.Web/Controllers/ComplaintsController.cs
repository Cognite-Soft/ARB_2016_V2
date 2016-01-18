using Cognite.Arb.Web.Core;
using System;
using System.Web.Mvc;
using Cognite.Arb.Server.Contract;
using Cognite.Arb.Server.Contract.Cases;
using Cognite.Arb.Web.Core.Mappers;
using Cognite.Arb.Web.Models.Complaints;

namespace Cognite.Arb.Web.Controllers
{
    [Authorize]
    public partial class ComplaintsController : BaseController
    {
        // GET: Complaints
        public ActionResult Index()
        {
            return TryGetCasesList();
        }

        private ActionResult TryGetCasesList()
        {
            var model = new ComplaintsFullListViewModel();

            try { model = CreateTempModel(this.SecurityToken); }
            catch (Exception) { AddModelStateError(GlobalStrings.SomethingWentWrong); }

            return View(model);
        }

        private ComplaintsFullListViewModel CreateTempModel(string token)
        {
            var model = new ComplaintsFullListViewModel();

            model.Active.Complaints = Mappers.MapCasesToComplaintsListItem(this.Service.GetActiveCases(token));
            model.Closed.Complaints = Mappers.MapCasesToComplaintsListItem(this.Service.GetClosedCases(token));
            model.Rejected.Complaints = Mappers.MapCasesToComplaintsListItem(this.Service.GetRejectedCases(token));

            return model;
        }

        // GET: /Compaints/Overview/{id}
        public ActionResult Overview(int id)
        {
            ViewBag.Title = "Overview";
            ViewBag.PageDescription = "Case Overview:";

            CheckRempDataModelErrors();
            var model = new ComplaintDetailsViewModel()
            {
                Id = id,
                Overview = TryGetDetails(id),
            };
            SetCurrentCase(id);
            return View("Details", model);
        }

        private void CheckRempDataModelErrors()
        {
            var error = TempData[ArbConstants.TempDataErrorKey] as String;
            if (!String.IsNullOrEmpty(error))
                AddModelStateError(error);
        }

        private void SetCurrentCase(int id)
        {
            Session[ArbConstants.SessionCurrentCaseKey] = id;
        }

        private ComplaintOverviewViewModel TryGetDetails(int id)
        {
            var model = new ComplaintOverviewViewModel();

            try
            {
                var curCase = GetComplaint(this.SecurityToken, id);
                model = Mappers.MapCaseToComplaintDetails(curCase);
                model.ComplaintCanBeEdited = model.ComplaintCanBeEdited && IsUserCanEditComplaint(id);
            }
            catch (ForbiddenException) { AddModelStateError(GlobalStrings.Forbidden); }
            catch (CaseDoesNotExistException) { AddModelStateError(GlobalStrings.CaseDoesNotExist); }
            catch (Exception) { AddModelStateError(GlobalStrings.SomethingWentWrong); }

            return model;
        }

        private Case GetComplaint(string token, int id)
        {
            return this.Service.GetCase(token, id);
        }

        private void TryCreateUsersList()
        {
            try { ViewBag.PanelMembersToComplete = GetUsersJson(this.SecurityToken); }
            catch (ForbiddenException) { AddModelStateError(GlobalStrings.Forbidden); }
            catch (Exception) { AddModelStateError(GlobalStrings.SomethingWentWrong); }
        }

        private string GetUsersJson(string token)
        {
            var users = this.Service.GetPanelMembers(token);
            return Mappers.MapUsersToJsonIdValueModel(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "CaseWorker")]
        public ActionResult StartProcess(int id)
        {
            return TryStartProcess(id);
        }

        private ActionResult TryStartProcess(int id)
        {
            try { this.Service.StartCaseProcessing(this.SecurityToken, id); }
            catch (ForbiddenException) { AddErrorToTempData(GlobalStrings.Forbidden); }
            catch (CaseDoesNotExistException) { AddErrorToTempData(GlobalStrings.CaseDoesNotExist); }
            catch (Exception) { AddErrorToTempData(GlobalStrings.SomethingWentWrong); }
            return RedirectToAction("Overview", new { id = id });
        }

        private void AddErrorToTempData(string error)
        {
            TempData.Add(ArbConstants.TempDataErrorKey, error);
        }
    }
}
