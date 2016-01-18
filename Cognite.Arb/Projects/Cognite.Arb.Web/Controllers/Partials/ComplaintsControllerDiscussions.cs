using Cognite.Arb.Web.Core;
using System;
using System.Web.Mvc;
using Cognite.Arb.Server.Contract;
using Cognite.Arb.Server.Contract.Cases;
using Cognite.Arb.Web.Core.Mappers;
using Cognite.Arb.Web.Models.Complaints;
using Cognite.Arb.Web.Models.Additional;

namespace Cognite.Arb.Web.Controllers
{
    [Authorize]
    public partial class ComplaintsController : BaseController
    {
        // GET: /Compaints/Discussions/{id}
        public ActionResult Discussions(int id)
        {
            ViewBag.Title = "Activity";
            ViewBag.PageDescription = "Case Activity:";

            var model = new ComplaintDetailsViewModel()
            {
                Id = id,
                Discussions = TryGetDiscussions(id),
            };
            return View("Details", model);
        }

        private ComplaintDiscussionsViewModel TryGetDiscussions(int id)
        {
            var result = new ComplaintDiscussionsViewModel();

            try
            {
                var discussions = this.Service.GetDiscussions(this.SecurityToken, id);
                return Mappers.MapDiscussions(discussions);
            }
            catch (ForbiddenException) { AddModelStateError(GlobalStrings.Forbidden); }
            catch (CaseDoesNotExistException) { AddModelStateError(GlobalStrings.CaseDoesNotExist); }
            catch (Exception) { AddModelStateError(GlobalStrings.SomethingWentWrong); }

            return result;
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public JsonResult AddNewPost(int id, string text)
        {
            return TryAddNewPost(id, text);
        }

        private JsonResult TryAddNewPost(int id, string text)
        {
            var result = new JsonOperationModel(false, GlobalStrings.CanNotCreatePost);

            try
            {
                this.Service.CreatePost(this.SecurityToken, id, Guid.NewGuid(), text);
                result.IsSucceeded = true;
                return Json(result);
            }
            catch (Exception)
            {
                return Json(result);
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public JsonResult AddReplyToPost(int id, Guid postId, string text)
        {
            return TryAddReplyToPost(id, postId, text);
        }

        private JsonResult TryAddReplyToPost(int id, Guid postId, string text)
        {
            var result = new JsonOperationModel(false, GlobalStrings.CanNotAddReplyOnPost);

            try
            {
                this.Service.ReplyOnPost(this.SecurityToken, id, postId, Guid.NewGuid(), text);
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
