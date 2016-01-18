using Cognite.Arb.Web.Core.Mappers;
using Cognite.Arb.Server.Contract;
using Cognite.Arb.Web.Models.Complaints;
using Cognite.Arb.Web.Core;
using Cognite.Arb.Web.Core.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using AllegationCommentType = Cognite.Arb.Server.Contract.Cases.AllegationCommentType;
using Cognite.Arb.Web.Models.Additional;
using System.Web;
using System.IO;

namespace Cognite.Arb.Web.Controllers
{
    public partial class ComplaintsController : BaseController
    {
        [HttpGet]
        [Authorize(Roles = "CaseWorker")]
        public ActionResult Edit(int id)
        {
            if (IsUserCanEditComplaint(id))
            {
                return TryLoadComplaintToEdit(id);
            }

            return RedirectToAction("Overview", new { id = id });
        }

        private bool IsUserCanEditComplaint(int id)
        {
            try
            {
                var userCases = GetUserAssignment();
                if (userCases != null)
                    return userCases.ToList().Contains(id);

                return false;
            }
            catch { return false; }
        }

        private int[] GetUserAssignment()
        {
            return this.Service.GetAssignedCases(this.SecurityToken, this.Authentication.GetCurrentUser().Id);
        }

        private ActionResult TryLoadComplaintToEdit(int id)
        {
            ComplaintOverviewViewModel model = new ComplaintOverviewViewModel();

            try
            {
                model = GetComplaintToEdit(id);
                TryCreateUsersList();
                model.StartDate = DateTime.Now;
            }
            catch (UserDoesNotExistException) { AddModelStateError(GlobalStrings.UserDoesNotExists); }
            catch (ForbiddenException) { AddModelStateError(GlobalStrings.Forbidden); }
            catch (CaseDoesNotExistException) { AddModelStateError(GlobalStrings.CaseDoesNotExist); }
            catch (Exception) { AddModelStateError(GlobalStrings.SomethingWentWrong); }

            return View(model);
        }

        private ComplaintOverviewViewModel GetComplaintToEdit(int id)
        {
            var model = GetBaseComplaintEditModel(id);

            SetDatesAndDetailsFromSession(model);
            SetAllegationsFromSession(model);
            
            return model;
        }

        private ComplaintOverviewViewModel GetBaseComplaintEditModel(int id)
        {
            var sourceModel = TryGetDetails(id);
            var model = TempData[ArbConstants.ComplaintModelTempDataKey] as ComplaintOverviewViewModel;
            if (model == null)
                return sourceModel;

            FillModel(model, sourceModel);
            return model;
        }

        private void FillModel(ComplaintOverviewViewModel model, ComplaintOverviewViewModel sourceModel)
        {
            ResolveUsers(model);
            model.DatesAndDetails = sourceModel.DatesAndDetails;
            model.Allegations = sourceModel.Allegations;
        }

        private void ResolveUsers(ComplaintOverviewViewModel model)
        {
            if (!model.PanelMembers.Member1.Id.Equals(Guid.Empty))
                model.PanelMembers.Member1 = Mappers.MapUserToUserViewModel(GetUser(this.SecurityToken, model.PanelMembers.Member1.Id));
            if (!model.PanelMembers.Member2.Id.Equals(Guid.Empty))
                model.PanelMembers.Member2 = Mappers.MapUserToUserViewModel(GetUser(this.SecurityToken, model.PanelMembers.Member2.Id));
            if (!model.PanelMembers.Member3.Id.Equals(Guid.Empty))
                model.PanelMembers.Member3 = Mappers.MapUserToUserViewModel(GetUser(this.SecurityToken, model.PanelMembers.Member3.Id));
        }

        private User GetUser(string token, Guid userId)
        {
            return this.Service.GetUser(token, userId);
        }

        private void SetAllegationsFromSession(ComplaintOverviewViewModel model)
        {
            if (!IsSessionSynchronized(model.Id))
                return;

            var tempAlgList = Session[ArbConstants.AllegationSessionKey] as List<AllegationWithMyComment>;
            if (tempAlgList != null)
                model.Allegations.Items.AddRange(tempAlgList);
            CheckDeleteAllegationsCollection(model);
        }

        private void CheckDeleteAllegationsCollection(ComplaintOverviewViewModel model)
        {
            var allegationsToDelete = GetAllegationsIdsToDelete();
            foreach (var allegationId in allegationsToDelete)
                Mappers.CheckAllegationToDelete(model.Allegations.Items, allegationId);
        }

        private List<Guid> GetAllegationsIdsToDelete()
        {
            return InitializeSessionTempList<Guid>(ArbConstants.AllegationsToDeleteSessionKey);
        }

        private void SetDatesAndDetailsFromSession(ComplaintOverviewViewModel model)
        {
            if (!IsSessionSynchronized(model.Id))
                return;

            var tempDaDList = Session[ArbConstants.DatesAndDetailsSessionKey] as List<DateAndDetail>;
            if (tempDaDList != null)
                model.DatesAndDetails.AddRange(tempDaDList);
            CheckDeleteDateAndDetailsCollection(model);
        }

        private void CheckDeleteDateAndDetailsCollection(ComplaintOverviewViewModel model)
        {
            var dateAndDetailsToDelete = GetDateAndDetailsIdsToDelete();
            foreach (var dateAndDetailId in dateAndDetailsToDelete)
                Mappers.CheckDateAndDetailsToDelete(model.DatesAndDetails, dateAndDetailId);
        }

        private List<Guid> GetDateAndDetailsIdsToDelete()
        {
            return InitializeSessionTempList<Guid>(ArbConstants.DateAndDetailsToDeleteSessionKey);
        }

        private bool IsSessionSynchronized(int syncId)
        {
            var sessionSyncId = Session[ArbConstants.SessionSynchronizationId];
            if (sessionSyncId != null && (int)sessionSyncId == syncId)
                return true;
            return false;
        }

        // POST: /Complaints/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "CaseWorker")]
        [HttpParamAction]
        public ActionResult Save(ComplaintOverviewViewModel model)
        {
            return TrySaveComplaint(model);
        }

        private ActionResult TrySaveComplaint(ComplaintOverviewViewModel model)
        {
            try
            {
                ExtendModelWithSessionData(model);

                var allegationsToDelete = GetAllegationsIdsToDelete();
                var dateAndDetailsToDelete = GetDateAndDetailsIdsToDelete();
                var updateCase = Mappers.MapCaseToUpdateCase(model, allegationsToDelete, dateAndDetailsToDelete);
                this.Service.UpdateCase(this.SecurityToken, model.Id, updateCase);
                ClearSession();

                return RedirectToAction("Overview", new { id = model.Id });
            }
            catch (ForbiddenException) { AddModelStateError(GlobalStrings.Forbidden); }
            catch (CaseDoesNotExistException) { AddModelStateError(GlobalStrings.CaseDoesNotExist); }
            catch (Exception) { AddModelStateError(GlobalStrings.SomethingWentWrong); }

            SetModelToTempData(model);
            return RedirectToAction("Edit", new { id = model.Id });
        }

        private void ExtendModelWithSessionData(ComplaintOverviewViewModel model)
        {
            var allegations = InitializeSessionTempList<AllegationWithMyComment>(ArbConstants.AllegationSessionKey);
            var datesAndDetails = InitializeSessionTempList<DateAndDetail>(ArbConstants.DatesAndDetailsSessionKey);
            model.Allegations.Items = allegations;
            model.DatesAndDetails = datesAndDetails;
        }

        private void ClearSession()
        {
            Session.Remove(ArbConstants.AllegationSessionKey);
            Session.Remove(ArbConstants.DatesAndDetailsSessionKey);
            Session.Remove(ArbConstants.AllegationsToDeleteSessionKey);
            Session.Remove(ArbConstants.DateAndDetailsToDeleteSessionKey);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "CaseWorker")]
        [HttpParamAction]
        public ActionResult AddDateAndDetail(ComplaintOverviewViewModel model, HttpPostedFileBase dateanddetailsfile)
        {
            TryAddDateAndDetail(model, dateanddetailsfile);
            return RedirectToAction("Edit", new { id = model.Id });
        }

        private void TryAddDateAndDetail(ComplaintOverviewViewModel model, HttpPostedFileBase dateanddetailsfile)
        {
            try
            {
                if (!String.IsNullOrEmpty(model.NewDateAndDetail.Text) &&
                    !String.IsNullOrEmpty(model.NewDateAndDetail.Text.Trim()))
                    AddDateAndTimeToSession(model, dateanddetailsfile);
                SetModelToTempData(model);
            }
            catch (Exception)
            {
                AddModelStateError(GlobalStrings.SomethingWentWrong);
            }
        }

        private void AddDateAndTimeToSession(ComplaintOverviewViewModel model, HttpPostedFileBase dateanddetailsfile)
        {
            var tempList = InitializeSessionTempList<DateAndDetail>(ArbConstants.DatesAndDetailsSessionKey);
            model.NewDateAndDetail.Id = Guid.NewGuid();
            model.NewDateAndDetail.CanBeDeleted = true;
            if (dateanddetailsfile != null)
            {
                model.NewDateAndDetail.Documents.Add(new Document()
                {
                    Id = Guid.NewGuid(),
                    Name = dateanddetailsfile.FileName,
                    File = GetFileContent(dateanddetailsfile),
                });
            }
            tempList.Add(model.NewDateAndDetail);
            model.NewDateAndDetail = new DateAndDetail();
            Session[ArbConstants.DatesAndDetailsSessionKey] = tempList;
            SetSessionSynchronyzation(model.Id);
        }

        private byte[] GetFileContent(HttpPostedFileBase dateanddetailsfile)
        {
            byte[] data;
            using (Stream inputStream = dateanddetailsfile.InputStream)
            {
                MemoryStream memoryStream = inputStream as MemoryStream;
                if (memoryStream == null)
                {
                    memoryStream = new MemoryStream();
                    inputStream.CopyTo(memoryStream);
                }
                data = memoryStream.ToArray();
            }
            return data;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "CaseWorker")]
        [HttpParamAction]
        public ActionResult AddAllegation(ComplaintOverviewViewModel model, HttpPostedFileBase allegationfile)
        {
            TryAddAllegation(model, allegationfile);
            return RedirectToAction("Edit", new { id = model.Id });
        }

        private void TryAddAllegation(ComplaintOverviewViewModel model, HttpPostedFileBase allegationfile)
        {
            if (!String.IsNullOrEmpty(model.NewAllegation.Text) &&
                !String.IsNullOrEmpty(model.NewAllegation.Text.Trim()))
                AddAllegationToSession(model, allegationfile);
            SetModelToTempData(model);
        }

        private void AddAllegationToSession(ComplaintOverviewViewModel model, HttpPostedFileBase allegationfile)
        {
            var tempList = InitializeSessionTempList<AllegationWithMyComment>(ArbConstants.AllegationSessionKey);
            model.NewAllegation.CanBeDeleted = true;
            model.NewAllegation.Id = Guid.NewGuid();
            if (allegationfile != null)
            {
                model.NewAllegation.Documents.Add(new Document
                {
                    Id = Guid.NewGuid(),
                    Name = allegationfile.FileName,
                    File = GetFileContent(allegationfile),
                });
            }
            tempList.Add(model.NewAllegation);
            model.NewAllegation = new AllegationWithMyComment();
            Session[ArbConstants.AllegationSessionKey] = tempList;
            SetSessionSynchronyzation(model.Id);
        }


        private List<T> InitializeSessionTempList<T>(string sessionKey)
        {
            var tempList = Session[sessionKey] as List<T>;
            if (tempList == null)
                tempList = new List<T>();

            return tempList;
        }

        private void SetSessionSynchronyzation(int synchronizationId)
        {
            Session[ArbConstants.SessionSynchronizationId] = synchronizationId;
        }

        private void SetModelToTempData(ComplaintOverviewViewModel model)
        {
            TempData.Add(ArbConstants.ComplaintModelTempDataKey, model);
        }

        // GET: /Complaints/Cancel/{id}
        [HttpGet]
        [Authorize(Roles = "CaseWorker")]
        public ActionResult Cancel(int id)
        {
            ClearSession();
            return RedirectToAction("Overview", new { id = id });
        }

        // POST: /Complaints/DeleteAllegation/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "CaseWorker")]
        public JsonResult DeleteAllegation(Guid id, int syncId)
        {
            var result = new JsonOperationModel(true, GlobalStrings.AllegationDeleteError);
            try { TryAddAllegationDeleteId(id, syncId); }
            catch { result.IsSucceeded = false; }
            return Json(result);
        }

        private void TryAddAllegationDeleteId(Guid id, int syncId)
        {
            var deletingList = InitializeSessionTempList<Guid>(ArbConstants.AllegationsToDeleteSessionKey);
            deletingList.Add(id);
            SetDeleteAllegationsIdsToSession(deletingList);
            SetSessionSynchronyzation(syncId);
        }

        private void SetDeleteAllegationsIdsToSession(List<Guid> deletingList)
        {
            Session[ArbConstants.AllegationsToDeleteSessionKey] = deletingList;
        }

        // POST: /Complaints/DeleteDateAndDetail/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "CaseWorker")]
        public JsonResult DeleteDateAndDetail(Guid id, int syncId)
        {
            var result = new JsonOperationModel(true, GlobalStrings.DateAndDetailDeleteError);
            try { TryAddDateAndDetailDeleteId(id, syncId); }
            catch { result.IsSucceeded = false; }
            return Json(result);
        }

        private void TryAddDateAndDetailDeleteId(Guid id, int syncId)
        {
            var deletingList = InitializeSessionTempList<Guid>(ArbConstants.DateAndDetailsToDeleteSessionKey);
            deletingList.Add(id);
            SetDeleteDateAndDetailsIdsToSession(deletingList);
            SetSessionSynchronyzation(syncId);
        }

        private void SetDeleteDateAndDetailsIdsToSession(List<Guid> deletingList)
        {
            Session[ArbConstants.DateAndDetailsToDeleteSessionKey] = deletingList;
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PanelMember")]
        [HttpParamAction]
        public JsonResult AddAllegationCommentYes(Guid id, string text, string addText)
        {
            var result = new JsonOperationModel(false, GlobalStrings.CanNotAddAllegationComment);

            var commented = TryAddAllegationComment(id, text, AllegationCommentType.Yes);
            if (commented)
                result.IsSucceeded = true;

            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PanelMember")]
        [HttpParamAction]
        public JsonResult AddAllegationCommentNo(Guid id, string text, string addText)
        {
            var result = new JsonOperationModel(false, GlobalStrings.CanNotAddAllegationComment);
            
            var commented = TryAddAllegationComment(id, text, AllegationCommentType.No);
            if (commented)
                result.IsSucceeded = true;
            
            return Json(result);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PanelMember")]
        [HttpParamAction]
        public JsonResult AddAllegationCommentAdvice(Guid id, string text, string addText)
        {
            var result = new JsonOperationModel(false, GlobalStrings.CanNotAddAllegationComment);
            
            var commented = TryAddAllegationComment(id, text, AllegationCommentType.Advise, addText);
            if (commented)
                result.IsSucceeded = true;
            
            return Json(result);
        }

        private bool TryAddAllegationComment(Guid allegationId, string commentText, AllegationCommentType allegationCommentType, string addText = "")
        {
            try
            {
                this.Service.CommentAllegation(this.SecurityToken, allegationId, commentText, allegationCommentType, addText);
                return true;
            }
            catch (CaseDoesNotExistException) { AddErrorToTempData(GlobalStrings.CaseDoesNotExist); }
            catch (ForbiddenException) { AddErrorToTempData(GlobalStrings.Forbidden); }
            catch (Exception) { AddErrorToTempData(GlobalStrings.SomethingWentWrong); }

            return false;
        }
    }
}
