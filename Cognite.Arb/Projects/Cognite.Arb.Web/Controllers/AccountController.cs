using System;
using System.Web.Mvc;
using Cognite.Arb.Web.Core;
using Cognite.Arb.Server.Contract;

namespace Cognite.Arb.Web.Controllers
{
    [Authorize]
#if !DEBUG
    [RequireHttps]
#endif
    public partial class AccountController : BaseController
    {
        // GET: /Account/Logout
        [HttpGet]
        public ActionResult Logout()
        {
            this.Authentication.LogOut();
            return RedirectToLocal("/");
        }

        // GET: /Account/Reset
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Reset(string resetToken)
        {            
            return LogOutForResetIfLoggedIn(TryReset, resetToken, "Reset");
        }

        private ActionResult LogOutForResetIfLoggedIn(Func<string, ActionResult> action, string resetToken, string senderAction)
        {
            if (Request.IsAuthenticated)
            {
                this.Authentication.LogOut();
                return RedirectToAction(senderAction, new { resetToken = resetToken });
            }

            return action.Invoke(resetToken);
        }
        
        private ActionResult TryReset(string resetToken)
        {
            try
            {
                var resetTokenValidation = this.Service.ValidateResetToken(resetToken);

                if (resetTokenValidation.Type == ResetToken.ResetType.Password || resetTokenValidation.Type == ResetToken.ResetType.Both)
                    return RedirectToAction("ResetPassword", new { resetToken = resetToken });

                return RedirectToAction("ResetSecurePhrase", new { resetToken = resetToken });
            }
            catch (InvalidResetTokenException)
            {
                AddModelStateError(GlobalStrings.ResetTokenIsInvalid);
            }
            catch (UserDoesNotExistException)
            {
                AddModelStateError(GlobalStrings.UserDoesNotExists);
            }
            catch (Exception)
            {
                AddModelStateError(GlobalStrings.SomethingWentWrong);
            }

            return View();
        }
    }
}