using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cognite.Arb.Server.Contract;
using Cognite.Arb.Web.Core;
using Cognite.Arb.Web.Models.Account;

namespace Cognite.Arb.Web.Controllers
{
    public partial class AccountController : BaseController
    {
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            HttpContext.Response.Cookies.Remove(ArbConstants.RedirectCookieName);
            return RedirectToAction("LoginStart", new { returnUrl = returnUrl });
        }

        // GET: /Account/LoginStart
        [AllowAnonymous]
        public ActionResult LoginStart(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            var model = new LoginStartViewModel();
            return View(model);
        }

        // POST: /Account/LoginStart
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult LoginStart(LoginStartViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                return TryLoginUserStart(model, returnUrl);
            }

            return View(model);
        }

        private ActionResult TryLoginUserStart(LoginStartViewModel model, string returnUrl)
        {
            try
            {
                return LoginUserStart(model, returnUrl);
            }
            catch (InvalidUserOrPasswordException ex)
            {
                AddModelStateError(GlobalStrings.UserNameOrPasswordIsInvalid);
            }
            catch (Exception)
            {
                AddModelStateError(GlobalStrings.SomethingWentWrong);
            }

            return View(model);
        }

        private ActionResult LoginUserStart(LoginStartViewModel model, string returnUrl)
        {
            var loginStepResult = LoginUserWithMailAndPassword(model);
            SetTempData(loginStepResult);
            SetStepCookie(loginStepResult.SecurityToken);
            return RedirectToAction("LoginFinish", "Account", new { returnUrl = returnUrl });
        }

        private SecurePhraseQuestion LoginUserWithMailAndPassword(LoginStartViewModel model)
        {
            return this.Authentication.Login(model.Email, model.Password);
        }

        private void SetTempData(SecurePhraseQuestion loginStep1Result)
        {
            TempData[ArbConstants.SecurePhraseQuestionTempDataKey] = loginStep1Result;
        }

        private void SetStepCookie(string securityToken)
        {
            var step1Cookie = new HttpCookie(ArbConstants.AuthenticationStepCookieName);
            step1Cookie.Value = securityToken;
            Response.Cookies.Add(step1Cookie);
        }

        // GET: /Account/LoginFinish
        [AllowAnonymous]
        public ActionResult LoginFinish(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            var sequrePhraseQuestion = GetSecurePhraseQuestionFromTempData();

            var model = new LoginFinishViewModel()
            {
                PhraseFirstCharNumber = GetCharacterArb(sequrePhraseQuestion.FirstCharacterIndex),
                PhraseSecondCharNumber = GetCharacterArb(sequrePhraseQuestion.SecondCharacterIndex),
                SecurityToken = sequrePhraseQuestion.SecurityToken,
            };

            return View(model);
        }

        private string GetCharacterArb(int index)
        {
            var number = index + 1;
            var abr = number > 20 ? GetAbr(number % 10) : GetAbr(number);
            return String.Format("{0}{1}", number, abr);
        }

        private string GetAbr(int number)
        {
            switch (number)
            {
                case 1:
                    return "st";
                case 2:
                    return "nd";
                case 3:
                    return "rd";
                default:
                    return "th";
            }
        }

        private SecurePhraseQuestion GetSecurePhraseQuestionFromTempData()
        {
            var result = TempData[ArbConstants.SecurePhraseQuestionTempDataKey] as SecurePhraseQuestion ??
                         new SecurePhraseQuestion();

            return result;
        }

        // POST: /Account/LoginFinish
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult LoginFinish(LoginFinishViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                return TryAuthenticateUser(model, returnUrl);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private ActionResult TryAuthenticateUser(LoginFinishViewModel model, string returnUrl)
        {
            try
            {
                return AuthenticateUser(model, returnUrl);
            }
            catch (InvalidSecurePhraseAnswer)
            {
                AddModelStateError(GlobalStrings.SecurePhraseSymbolsAreInvalid);
            }
            catch (NotAuthenticatedException)
            {
                AddModelStateError(GlobalStrings.YouNeedToEnterMailAndPasswordFirst);
            }
            catch (Exception ex)
            {
                AddModelStateError(GlobalStrings.SomethingWentWrong);
            }

            return View(model);
        }

        private ActionResult AuthenticateUser(LoginFinishViewModel model, string returnUrl)
        {
            var securePhraseAnswer = GetSecurePhraseAnswer(model);
            var authenticatedUser = FinishUserLogin(model.SecurityToken, securePhraseAnswer);
            RemoveStepCookie();

            if (authenticatedUser.Role == Role.Admin)
                return RedirectToAction("Index", "UserManagement");

            if (authenticatedUser.Role == Role.CaseWorker)
                return RedirectToAction("Index", "Complaints");

            return RedirectToLocal(returnUrl);
        }

        private void RemoveStepCookie()
        {
            Response.Cookies.Remove(ArbConstants.AuthenticationStepCookieName);
        }

        private SecurePhraseAnswer GetSecurePhraseAnswer(LoginFinishViewModel model)
        {
            return new SecurePhraseAnswer()
            {
                FirstCharacter = model.PhraseFirstChar.ToArray().FirstOrDefault(),
                SecondCharacter = model.PhraseSecondChar.ToArray().FirstOrDefault(),
            };
        }

        private User FinishUserLogin(string securityToken, SecurePhraseAnswer securePhraseAnswer)
        {
            return this.Authentication.Login(securityToken, securePhraseAnswer);
        }
    }
}