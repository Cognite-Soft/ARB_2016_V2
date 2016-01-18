using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Owin;
using Cognite.Arb.Web.Models;
using Cognite.Arb.Web.ServiceClient;
using Cognite.Arb.Web.Core;
using Cognite.Arb.Server.Contract;
using Cognite.Arb.Web.Models.Account;

namespace Cognite.Arb.Web.Controllers
{
    public partial class AccountController : BaseController
    {
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordPhraseViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (RequestPasswordReset(model))
                    return RedirectToAction("ForgotPasswordConfirmation");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private bool RequestPasswordReset(ForgotPasswordPhraseViewModel model)
        {
            try
            {
                this.Service.InitiateResetPassword(model.Email);
                return true;
            }
            catch (UserDoesNotExistException)
            {
                AddModelStateError(GlobalStrings.UserDoesNotExists);
                return false;
            }
            catch (Exception)
            {
                AddModelStateError(GlobalStrings.SomethingWentWrong);
                return false;
            }
        }

        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // GET: /Account/ForgotSecurePhrase
        [AllowAnonymous]
        public ActionResult ForgotSecurePhrase()
        {
            return View();
        }

        // POST: /Account/ForgotSecurePhrase
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotSecurePhrase(ForgotPasswordPhraseViewModel model)
        {
            var step1Token = GetFirstStepSecurityToken();
            if (String.IsNullOrEmpty(step1Token))
            {
                AddModelStateError(GlobalStrings.FirstLoginStepMustBeDoneBeforeResetSecurePhrase);
                return View(model);
            }

            return ForgotSecurePhrase(model, step1Token);
        }

        private string GetFirstStepSecurityToken()
        {
            var stepCookie = Request.Cookies.Get(ArbConstants.AuthenticationStepCookieName);
            return stepCookie != null ? stepCookie.Value : String.Empty;
        }

        private ActionResult ForgotSecurePhrase(ForgotPasswordPhraseViewModel model, string token)
        {
            if (ModelState.IsValid)
            {
                if (RequestSecurePhraseReset(model, token))
                    return RedirectToAction("ForgotSecurePhraseConfirmation");
            }

            return View(model);
        }

        private bool RequestSecurePhraseReset(ForgotPasswordPhraseViewModel model, string token)
        {
            try
            {
                this.Service.InitiateResetSecurePhrase(token, model.Email);
                return true;
            }
            catch (UserDoesNotExistException)
            {
                AddModelStateError(GlobalStrings.UserDoesNotExists);
                return false;
            }
            catch (Exception)
            {
                AddModelStateError(GlobalStrings.SomethingWentWrong);
                return false;
            }
        }

        // GET: /Account/ForgotSecurePhraseConfirmation
        [AllowAnonymous]
        public ActionResult ForgotSecurePhraseConfirmation()
        {
            return View();
        }
    }
}