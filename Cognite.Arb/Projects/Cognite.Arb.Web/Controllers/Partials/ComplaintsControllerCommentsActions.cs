using Cognite.Arb.Server.Contract.Cases;
using Cognite.Arb.Web.Core;
using Cognite.Arb.Web.Models;
using Cognite.Arb.Web.Core.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cognite.Arb.Web.Models.Additional;
using FinalDecisionType = Cognite.Arb.Web.Models.Complaints.FinalDecisionType;

namespace Cognite.Arb.Web.Controllers
{
    public partial class ComplaintsController : BaseController
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PanelMember")]
        public JsonResult AddPreliminaryDecisionComment(int id, string text)
        {
            var result = new JsonOperationModel(false, GlobalStrings.CanNotAddPreliminaryDecisionComment);

            var commented = TryAddPreliminaryDecisionComment(id, text);
            if (commented)
                result.IsSucceeded = true;

            return Json(result);
        }

        private bool TryAddPreliminaryDecisionComment(int id, string text)
        {
            try
            {
                this.Service.AddPreliminaryDecisionComment(this.SecurityToken, id, text);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PanelMember")]
        public JsonResult AddFinalDecisionAccept(int id, string text)
        {
            return AddFinalDecision(id, text, FinalDecisionType.Accept);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PanelMember")]
        public JsonResult AddFinalDecisionAmend(int id, string text)
        {
            return AddFinalDecision(id, text, FinalDecisionType.Amend);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PanelMember")]
        public JsonResult AddFinalDecisionChanged(int id, string text)
        {
            return AddFinalDecision(id, text, FinalDecisionType.DecisionChanged);
        }

        private JsonResult AddFinalDecision(int id, string text, FinalDecisionType finalDecisionType)
        {
            var result = new JsonOperationModel(false, GlobalStrings.CanNotAddDecision);

            var completed = TryAddFinalDecision(id, finalDecisionType, text);
            if (completed)
                result.IsSucceeded = true;

            return Json(result);
        }

        private bool TryAddFinalDecision(int id, FinalDecisionType finalDecisionType, string text)
        {
            try
            {
                this.Service.AddFinalDecisionComment(this.SecurityToken, id,
                    (Server.Contract.Cases.FinalDecisionType) finalDecisionType, text);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "CaseWorker")]
        public JsonResult AddPartiesComment(int id, string text)
        {
            return TryAddPartiesComment(id, text);
        }

        private JsonResult TryAddPartiesComment(int id, string text)
        {
            var result = new JsonOperationModel(false, GlobalStrings.CanNotAddComment);

            try
            {
                this.Service.AddPartiesComment(this.SecurityToken, id, text, new NewDocument[0]);
                result.IsSucceeded = true;
                return Json(result);
            }
            catch (Exception)
            {
                return Json(result);
            }
        }
    }
}