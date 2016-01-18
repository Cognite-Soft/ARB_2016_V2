using Cognite.Arb.Web.Core;
using System;
using System.Web.Mvc;
using Cognite.Arb.Server.Contract;
using Cognite.Arb.Server.Contract.Cases;
using Cognite.Arb.Web.Core.Mappers;
using Cognite.Arb.Web.Models.Complaints;
using System.Web;
using System.Web.Routing;
using System.Collections.Generic;

namespace Cognite.Arb.Web.Controllers
{
    [Authorize]
    public partial class ComplaintsController : BaseController
    {
        // GET: /Compaints/Comments/{id}
        public ActionResult Comments(int id)
        {
            ViewBag.Title = "Comments";
            ViewBag.PageDescription = "Case Comments:";

            var model = new ComplaintDetailsViewModel()
            {
                Id = id,
                Comments = TryGetComments(id),
            };
            return View("Details", model);
        }

        private ComplaintCommentsViewModel TryGetComments(int id)
        {
            var model = new ComplaintCommentsViewModel();

            try
            {
                model = GetComments(id);
                model.AllDocuments = GetAllDocuments(id);
            }
            catch (Exception) { AddModelStateError(GlobalStrings.SomethingWentWrong); }

            return model;
        }

        private List<Models.Complaints.Document> GetAllDocuments(int id)
        {
            var documents = this.Service.GetCaseDocuments(this.SecurityToken, id);
            return Mappers.MapDocuments(documents);
        }

        private ComplaintCommentsViewModel GetComments(int id)
        {
            var comments = this.Service.GetComments(this.SecurityToken, id);
            return Mappers.MapComments(comments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "CaseWorker")]
        [HttpParamAction]
        public ActionResult AddPartiesComment(int caseid, string partiescomment, HttpPostedFileBase partiescommentfile)
        {
            if (!String.IsNullOrEmpty(partiescomment) && partiescommentfile != null)
                TryAddPartiesComment(caseid, partiescomment, partiescommentfile);
            return RedirectToAction("Comments", new { id = caseid });
        }

        private void TryAddPartiesComment(int id, string partiescomment, HttpPostedFileBase partiescommentfile)
        {
            try
            {
                var document = GetDocument(partiescommentfile);
                var newDocuments = new List<NewDocument>() { Mappers.GetCaseNewDocument(document) };
                this.Service.AddPartiesComment(this.SecurityToken, id, partiescomment, newDocuments.ToArray());
            }
            catch (Exception)
            {
                AddModelStateError(GlobalStrings.SomethingWentWrong);
            }
        }

        private Cognite.Arb.Web.Models.Complaints.Document GetDocument(HttpPostedFileBase file)
        {
            return new Cognite.Arb.Web.Models.Complaints.Document()
            {
                Id = Guid.NewGuid(),
                Name = file.FileName,
                File = GetFileContent(file),
            };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "CaseWorker")]
        [HttpParamAction]
        //public ActionResult AddPreliminaryDecision(int caseid, HttpPostedFileBase preliminarydecisionfile)
        public ActionResult AddPreliminaryDecision(int Id, HttpPostedFileBase preliminarydecisionfile)
        {
            int caseid = Id;
            if (preliminarydecisionfile != null)
                TryAddPreliminaryDecision(caseid, preliminarydecisionfile);
            return RedirectToAction("Comments", new { id = caseid });
        }

        private void TryAddPreliminaryDecision(int id, HttpPostedFileBase preliminarydecisionfile)
        {
            try
            {
                var document = GetDocument(preliminarydecisionfile);
                var newDocument = Mappers.GetCaseNewDocument(document);
                this.Service.AddPreliminaryDecision(this.SecurityToken, id, newDocument);
            }
            catch (Exception)
            {
                AddModelStateError(GlobalStrings.SomethingWentWrong);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PanelMember")]
        [HttpParamAction]
        //public ActionResult AddUpdatedPreliminaryDecision(int caseid, HttpPostedFileBase preliminarydecisionfile)
        public ActionResult AddUpdatedPreliminaryDecision(int Id, HttpPostedFileBase preliminarydecisionfile)
        {
            int caseid = Id;
            if (preliminarydecisionfile != null)
                TryAddUpdatedPreliminaryDecision(caseid, preliminarydecisionfile);
            return RedirectToAction("Comments", new { id = caseid });
        }

        private void TryAddUpdatedPreliminaryDecision(int caseId, HttpPostedFileBase preliminarydecisionfile)
        {
            try
            {
                var document = GetDocument(preliminarydecisionfile);
                var targetCase = this.Service.GetCase(this.SecurityToken, caseId);
                var newDocument = Mappers.GetCaseNewDocument(document);
                if (targetCase.PreliminaryDecisionDocument != null)
                {
                    newDocument.Id = targetCase.PreliminaryDecisionDocument.Id;
                    newDocument.Name = targetCase.PreliminaryDecisionDocument.Name;
                    this.Service.AddUpdatedPreliminaryDecision(this.SecurityToken, caseId, newDocument);
                }
            }
            catch (Exception)
            {
                AddModelStateError(GlobalStrings.SomethingWentWrong);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "CaseWorker")]
        [HttpParamAction]
        //public ActionResult AddFinalDecision(int caseid, HttpPostedFileBase finaldecisionfile)
        public ActionResult AddFinalDecision(int Id, HttpPostedFileBase finaldecisionfile)
        {
            int caseid = Id;
            if (finaldecisionfile != null)
                TryAddFinalDecision(caseid, finaldecisionfile);
            return RedirectToAction("Comments", new { id = caseid });
        }

        private void TryAddFinalDecision(int id, HttpPostedFileBase finaldecisionfile)
        {
            try
            {
                var document = GetDocument(finaldecisionfile);
                var newDocument = Mappers.GetCaseNewDocument(document);
                this.Service.AddFinalDecision(this.SecurityToken, id, newDocument);
            }
            catch (Exception)
            {
                AddModelStateError(GlobalStrings.SomethingWentWrong);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PanelMember")]
        [HttpParamAction]
        //public ActionResult AddUpdatedFinalDecision(int caseid, HttpPostedFileBase finaldecisionfile)
        public ActionResult AddUpdatedFinalDecision(int Id, HttpPostedFileBase finaldecisionfile)
        {
            int caseid = Id;
            if (finaldecisionfile != null)
                TryAddUpdatedFinalDecision(caseid, finaldecisionfile);
            return RedirectToAction("Comments", new { id = caseid });
        }

        private void TryAddUpdatedFinalDecision(int caseId, HttpPostedFileBase finalDecisionFile)
        {
            try
            {
                var document = GetDocument(finalDecisionFile);
                var targetCase = this.Service.GetCase(this.SecurityToken, caseId);
                var newDocument = Mappers.GetCaseNewDocument(document);
                if (targetCase.FinalDecisionDocument != null)
                {
                    newDocument.Id = targetCase.FinalDecisionDocument.Id;
                    newDocument.Name = targetCase.FinalDecisionDocument.Name;
                    this.Service.AddUpdatedFinalDecision(this.SecurityToken, caseId, newDocument);
                }
            }
            catch (Exception)
            {
                AddModelStateError(GlobalStrings.SomethingWentWrong);
            }
        }
    }
}
