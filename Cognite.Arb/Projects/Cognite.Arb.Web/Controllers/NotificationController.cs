using Cognite.Arb.Server.Contract;
using Cognite.Arb.Web.Core;
using Cognite.Arb.Web.Models.Additional;
using Cognite.Arb.Web.Models.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cognite.Arb.Web.Controllers
{
    [Authorize]
    public class NotificationController : BaseController
    {
        // POST: /Notification/Dismiss/{id}
        [HttpPost]
        [Authorize(Roles = "PanelMember,CaseWorker")]
        public JsonResult Dismiss(Guid id)
        {
            return TryDismiss(id);
        }

        private JsonResult TryDismiss(Guid id)
        {
            var result = new JsonOperationModel(false, String.Empty);

            try
            {
                this.Service.DismissNotification(this.SecurityToken, id);
                result.IsSucceeded = true;
            }
            catch (Exception) { result.IsSucceeded = false; }

            return Json(result);
        }

        // Get: /Notification/Send/{id}
        [HttpGet]
        [Authorize(Roles = "CaseWorker")]
        public ActionResult Send(Guid id, string returnUrl)
        {
            var model = new SendNotificationModel()
            {
                UserId = id,
            };
            return View(model);
        }

        // POST: /Notification/Send
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "CaseWorker")]
        public ActionResult Send(SendNotificationModel model, string returnUrl)
        {
            if (ModelState.IsValid)
                return TrySend(model, returnUrl);

            return View(model);
        }

        private ActionResult TrySend(SendNotificationModel model, string returnUrl)
        {
            try
            {
                var notification = CreateNotification(model.Message, model.UserId, model.Type);
                this.Service.SendNotification(this.SecurityToken, notification);
            }
            catch (ForbiddenException)
            {
                AddModelStateError(GlobalStrings.Forbidden);
                return View(model);
            }
            catch (Exception)
            {
                AddModelStateError(GlobalStrings.SomethingWentWrong);
                return View(model);
            }
            string id = Session[ArbConstants.SessionCurrentCaseKey].ToString();
            

            return RedirectToLocal(returnUrl, Convert.ToInt32(id));
        }

        private CreateNotification CreateNotification(string message, Guid userId, CreateNotification.DeliveryType type)
        {
            var result = new CreateNotification()
            {
                Delivery = type,
                Message = message,
                ToUserId = userId,
            };
            return result;
        }

        // Get: /Notification/SendAll/{id}
        [HttpGet]
        [Authorize(Roles = "CaseWorker")]
        public ActionResult SendAll(int id, string returnUrl)
        {
            var model = new SendNotificationAllModel()
            {
                CaseId = id,
            };          
            return View(model);
        }

        // POST: /Notification/NotifyCaseWorker
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = "PanelMember")]
        //public ActionResult NotifyCaseWorker(SendNotificationAllModel model, string returnUrl)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var notification = CreateNotificationAll(model.Message, model.CaseId, model.Type);
        //            this.Service.CreateNotification(this.SecurityToken,"cw", notification);

                    
        //        }
        //        catch (ForbiddenException)
        //        {
        //            AddModelStateError(GlobalStrings.Forbidden);
        //            return View(model);
        //        }
        //        catch (Exception)
        //        {
        //            AddModelStateError(GlobalStrings.SomethingWentWrong);
        //            return View(model);
        //        }

        //    }
        //    return View(model);
        //}


        // POST: /Notification/SendAll
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "CaseWorker")]
        public ActionResult SendAll(SendNotificationAllModel model, string returnUrl)
        {           
            if (ModelState.IsValid)              
                return TrySendAll(model, returnUrl);

            return View(model);
        }

        private ActionResult TrySendAll(SendNotificationAllModel model, string returnUrl)
        {
            try
            {
                var notification = CreateNotificationAll(model.Message, model.CaseId, model.Type);
                this.Service.SendNotificationAll(this.SecurityToken, notification);
            }
            catch (ForbiddenException)
            {
                AddModelStateError(GlobalStrings.Forbidden);
                return View(model);
            }
            catch (Exception)
            {
                AddModelStateError(GlobalStrings.SomethingWentWrong);
                return View(model);
            }
            
            return RedirectToLocal(returnUrl,model.CaseId);
        }

        private CreateNotificationAll CreateNotificationAll(string message, int caseId, CreateNotification.DeliveryType type)
        {
            var result = new CreateNotificationAll()
            {
                Delivery = type,
                Message = message,
                CaseId = caseId,
            };

            return result;
        }

        //prabhakar
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PanelMember")]
       // public void NotifyCaseWorker(int Id)
        public ActionResult NotifyCaseWorker(int Id, string Message)
        {
            int caseid = Id;
            List<User> lstuser = this.Service.GetCaseWorkerforaCase(this.SecurityToken, caseid);
            foreach (var lst in lstuser)
            {
                if (lstuser.Count == 1)
                {
                    SendNotificationModel notObject = new SendNotificationModel();
                    notObject.Message = Message;
                    notObject.UserId = lst.Id;
                    notObject.Type = CreateNotificationBase.DeliveryType.Both;
                    TrySend(notObject, "Comments");
                }
            }
            return RedirectToLocal("Comments", caseid);
        }
    }
}