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
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string resetToken)
        {
            return LogOutForResetIfLoggedIn(TryResetPassword, resetToken, "ResetPassword");
        }

        private ActionResult TryResetPassword(string resetToken)
        {
            try
            {
                var tokenValidation = this.Service.ValidateResetToken(resetToken);
                if (tokenValidation.Type == ResetToken.ResetType.SecurePhrase)
                    return RedirectToAction("ResetSecurePhrase", new { resetToken = resetToken });
            }
            catch (InvalidResetTokenException)
            {
                AddModelStateError(GlobalStrings.ResetTokenIsInvalid);
            }
            catch (Exception)
            {
                AddModelStateError(GlobalStrings.SomethingWentWrong);
            }

            return ResetPasswordView(resetToken);
        }

        private ActionResult ResetPasswordView(string resetToken)
        {
            var model = new ResetPasswordViewModel()
            {
                ResetToken = resetToken,
            };

            return View(model);
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
                return TryResetPassword(model);

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private ActionResult TryResetPassword(ResetPasswordViewModel model)
        {
            try
            {
                var tokenValidation = this.Service.ValidateResetToken(model.ResetToken);
                if (tokenValidation.Type == ResetToken.ResetType.Both)
                {
                    if (!this.Service.IsPasswordStrengthPassed(model.Password))
                        throw new WeakPasswordException();

                    SavePasswordToSession(model.Password);
                    return RedirectToAction("ResetSecurePhrase", new { resetToken = model.ResetToken });
                }

                this.Service.FinishResetPassword(model.ResetToken, model.Password);
                return RedirectToAction("ResetPasswordConfirmation");
            }
            catch (InvalidResetTokenException)
            {
                AddModelStateError(GlobalStrings.ResetTokenIsInvalid);
            }
            catch (WeakPasswordException)
            {
                AddModelStateError(GlobalStrings.WeakPassword);
            }
            catch (Exception)
            {
                AddModelStateError(GlobalStrings.SomethingWentWrong);
            }

            return View(model);
        }

        private void SavePasswordToSession(string password)
        {
            Session.Add(ArbConstants.SessionPasswordForBothReset, password);
        }

        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }
    }
}