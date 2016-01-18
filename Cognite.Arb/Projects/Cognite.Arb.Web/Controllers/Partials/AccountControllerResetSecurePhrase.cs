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
        // GET: /Account/ResetSecurePhrase
        [AllowAnonymous]
        public ActionResult ResetSecurePhrase(string resetToken)
        {
            return LogOutForResetIfLoggedIn(TryResetSecurePhrase, resetToken, "ResetSecurePhrase");
        }

        private ActionResult TryResetSecurePhrase(string resetToken)
        {
            try
            {
                var tokenValidation = this.Service.ValidateResetToken(resetToken);
                if (tokenValidation.Type == ResetToken.ResetType.Password ||
                    (tokenValidation.Type == ResetToken.ResetType.Both && !IsPasswordAlreadyReseted()))
                    return RedirectToAction("ResetPassword", new { resetToken = resetToken });
            }
            catch (InvalidResetTokenException)
            {
                AddModelStateError(GlobalStrings.ResetTokenIsInvalid);
            }
            catch (Exception)
            {
                AddModelStateError(GlobalStrings.SomethingWentWrong);
            }

            return ResetSecurePhraseView(resetToken);
        }

        private bool IsPasswordAlreadyReseted()
        {
            return Session[ArbConstants.SessionPasswordForBothReset] != null;
        }

        private ActionResult ResetSecurePhraseView(string resetToken)
        {
            var model = new ResetSecurePhraseViewModel()
            {
                ResetToken = resetToken,
            };

            return View(model);
        }
        
        // POST: /Account/ResetSecurePhrase
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ResetSecurePhrase(ResetSecurePhraseViewModel model)
        {
            if (ModelState.IsValid)
                return TryResetSecurePhrase(model);
            
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private ActionResult TryResetSecurePhrase(ResetSecurePhraseViewModel model)
        {
            try
            {
                var tokenValidation = this.Service.ValidateResetToken(model.ResetToken);
                if (tokenValidation.Type == ResetToken.ResetType.SecurePhrase)
                    return ResetSecurePhraseOnly(model);

                if (tokenValidation.Type == ResetToken.ResetType.Both)
                    return ResetBoth(model);

                throw new Exception();
            }
            catch (InvalidResetTokenException)
            {
                AddModelStateError(GlobalStrings.ResetTokenIsInvalid);
            }
            catch (WeakSecurePhraseException)
            {
                AddModelStateError(GlobalStrings.WeakSecurePhrase);
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
        
        private ActionResult ResetSecurePhraseOnly(ResetSecurePhraseViewModel model)
        {
            this.Service.FinishResetSecurePhrase(model.ResetToken, model.SecurePhrase);
            return RedirectToAction("ResetSecurePhraseConfirmation");
        }

        private ActionResult ResetBoth(ResetSecurePhraseViewModel model)
        {
            var password = GetPasswordFromSession();
            this.Service.FinishActivateUser(model.ResetToken, password, model.SecurePhrase);
            RemovePasswordFromSession();            
            return RedirectToAction("ResetBothConfirmation");
        }
        
        private string GetPasswordFromSession()
        {
            var pwdObj = Session[ArbConstants.SessionPasswordForBothReset];
            if (pwdObj == null)
                throw new Exception();

            return pwdObj.ToString();
        }

        private void RemovePasswordFromSession()
        {
            Session[ArbConstants.SessionPasswordForBothReset] = null;
        }

        // GET: /Account/ResetSecurePhraseConfirmation
        [AllowAnonymous]
        public ActionResult ResetSecurePhraseConfirmation()
        {
            return View();
        }

        // GET: /Account/ResetBothConfirmation
        [AllowAnonymous]
        public ActionResult ResetBothConfirmation()
        {
            return View();
        }
    }
}