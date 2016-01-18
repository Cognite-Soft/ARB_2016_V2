using Cognite.Arb.Server.Contract;
using Cognite.Arb.Web.Core.Authorization.Interfaces;
using Cognite.Arb.Web.ServiceClient;
using System;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

namespace Cognite.Arb.Web.Core.Authorization
{
    public class ArbAuthentication : IAuthentication
    {
        private string _cookieName = "cognite.arb.cookies.auth";
        private IServiceClient _service = null;
        private IPrincipal _currentUser = null;

        private HttpContext _httpContext = null;
        public HttpContext HttpContext
        {
            get
            {
                if (_httpContext == null)
                    _httpContext = HttpContext.Current;

                return _httpContext;
            }
            set
            {
                _httpContext = value;
            }
        }

        private string _currentUserToken = null;
        public string CurrentUserToken
        {
            get
            {
                if (String.IsNullOrEmpty(_currentUserToken))
                    TryInitializeUser();
                return _currentUserToken;
            }
            private set { _currentUserToken = value; }
        }


        public IPrincipal CurrentUser
        {
            get
            {
                if (this._currentUser == null)
                    TryInitializeUser();

                return this._currentUser;
            }
        }

        private void TryInitializeUser()
        {
            try { InitializeUser(); }
            catch (Exception ex) { SetDefaultUser(); }
        }

        public ArbAuthentication(IServiceClient service)
        {
            this._service = service;
        }

        private void InitializeUser()
        {
            var authCookie = this.HttpContext.Request.Cookies.Get(this._cookieName);
            if (authCookie != null && !String.IsNullOrEmpty(authCookie.Value))
                InitializeUserWithToken(authCookie.Value);
            else
                SetDefaultUser();
        }

        private void InitializeUserWithToken(string securityToken)
        {
            var decryptedToken = FormsAuthentication.Decrypt(securityToken);
            this._currentUser = new UserProvider(decryptedToken.Name);
            this.CurrentUserToken = decryptedToken.Name;
        }
        
        private void SetDefaultUser()
        {
            _currentUser = new UserProvider(null);
        }

        private void RedirectToLogin()
        {
            if (!IsRedirectCookieExists())
                RedirectToLoginChecked();
        }

        private bool IsRedirectCookieExists()
        {
            var cookie = this.HttpContext.Request.Cookies[ArbConstants.RedirectCookieName];
            return cookie != null;
        }

        private void RedirectToLoginChecked()
        {
            var redirectUrl = String.Format("/Account/Login?returnUrl={0}", this.HttpContext.Request.Url);
            this.HttpContext.Response.Redirect(redirectUrl);
            CreateRedirectCookie();
        }

        private void CreateRedirectCookie()
        {
            this.HttpContext.Response.Cookies.Add(new HttpCookie(ArbConstants.RedirectCookieName));
        }

        public SecurePhraseQuestion Login(string email, string password)
        {
            return this._service.StartLoginAndGetSecurePhraseQuestion(email, password);
        }
        public User Login(string securityToken, SecurePhraseAnswer sequrePhraseAnswer)
        {
            var authResult = this._service.FinishLoginWithSecurePhraseAnswer(securityToken, sequrePhraseAnswer);
            return Login(authResult.SecurityToken);
        }

        public User Login(string securityToken)
        {
            var user = this._service.GetUserBySecurityToken(securityToken);
            CreateAuthorizationCookie(securityToken);
            return user;
        }

        private void CreateAuthorizationCookie(string securityToken)
        {
            var authTicket = CreateAuthorizationTicket(securityToken);
            var encryptedTicket = FormsAuthentication.Encrypt(authTicket);

            var authCookie = new HttpCookie(this._cookieName)
            {
                Value = encryptedTicket,
                Expires = DateTime.Now.Add(FormsAuthentication.Timeout),
            };
            this.HttpContext.Response.Cookies.Set(authCookie);
        }

        private FormsAuthenticationTicket CreateAuthorizationTicket(string securityToken)
        {
            var currentTime = DateTime.Now;
            return new FormsAuthenticationTicket(
                1,
                securityToken,
                currentTime,
                currentTime.Add(FormsAuthentication.Timeout),
                false,
                String.Empty,
                FormsAuthentication.FormsCookiePath);
        }

        public void LogOut()
        {
            var cookie = this.HttpContext.Response.Cookies.Get(this._cookieName);
            if (cookie != null)
                cookie.Value = String.Empty;
        }


        public User GetCurrentUser()
        {
            var identity = CurrentUser.Identity as UserIdentity;
            if (identity != null)
                return identity.User;

            return new User();
        }
    }
}