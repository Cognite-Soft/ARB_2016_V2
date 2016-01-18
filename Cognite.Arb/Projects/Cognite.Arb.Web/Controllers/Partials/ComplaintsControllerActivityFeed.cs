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

namespace Cognite.Arb.Web.Controllers
{
    public partial class ComplaintsController : BaseController
    {
        // GET: /Compaints/ActivityFeed/{id}
        public ActionResult ActivityFeed(int id)
        {
            ViewBag.Title = "Activity";
            ViewBag.PageDescription = "Case Activity:";

            var model = new ComplaintDetailsViewModel()
            {
                Id = id,
                ActivityFeed = TryGetActivityFeed(id),
            };
            return View("Details", model);
        }

        private ComplaintActivityFeedViewModel TryGetActivityFeed(int id)
        {
            var model = new ComplaintActivityFeedViewModel();

            try
            {
                model = GetActivityFeed(id);
            }
            catch (ForbiddenException) { AddModelStateError(GlobalStrings.Forbidden); }
            catch (CaseDoesNotExistException) { AddModelStateError(GlobalStrings.CaseDoesNotExist); }
            catch (Exception) { AddModelStateError(GlobalStrings.SomethingWentWrong); }

            return model;
        }

        private ComplaintActivityFeedViewModel GetActivityFeed(int id)
        {
            var activityFeed = this.Service.GetActivityFeed(this.SecurityToken, id);
            return Mappers.MapActivityFeed(activityFeed);
        }
    }
}
