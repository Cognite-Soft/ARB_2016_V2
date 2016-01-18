using Cognite.Arb.Server.Contract;
using Cognite.Arb.Web.Core.Authorization;
using Cognite.Arb.Web.Core.Authorization.Interfaces;
using Cognite.Arb.Web.Core.Mappers;
using Cognite.Arb.Web.Models.Account;
using Cognite.Arb.Web.Models.Notifications;
using Cognite.Arb.Web.ServiceClient;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cognite.Arb.Web.Controllers
{
    public class BaseController : Controller
    {
        private IAuthentication _authentication = null;
        public IAuthentication Authentication
        {
            get
            {
                if (this._authentication == null)
                    ResolveAuthentication();

                return this._authentication;
            }
        }

        private IServiceClient _service = null;
        public IServiceClient Service
        {
            get
            {
                if (this._service == null)
                    ResolveService();

                return this._service;
            }
        }

        private string _securityToken = null;
        public string SecurityToken {
            get
            {
                if (String.IsNullOrEmpty(_securityToken))
                    _securityToken = this.Authentication.GetSecurityToken();
                return _securityToken;
            }
        }
        
        private void ResolveAuthentication()
        {
            this._authentication = DependencyResolver.Current.GetService<IAuthentication>();
        }

        public BaseController()
        {
            InitializeNotifications();
        }

        private void InitializeNotifications()
        {
            var json = "[]";

            if (this.Authentication.CurrentUser != null && this.Authentication.CurrentUser.IsInRole("PanelMember") || this.Authentication.CurrentUser.IsInRole("CaseWorker"))
            {
                var notification = TryGetNotifications();
                json = JsonConvert.SerializeObject(notification);
            }

            ViewBag.NotificationsJson = json;
        }

        private List<NotificationModel> TryGetNotifications()
        {
            try
            {
                var notifications = this.Service.GetNotifications(this.SecurityToken);
                return Mappers.MapNotifications(notifications);
            }
            catch { return new List<NotificationModel>(); }
        }

        private void ResolveService()
        {
            this._service = DependencyResolver.Current.GetService<IServiceClient>();
        }

        protected void AddModelStateError(string errorMessage)
        {
            ModelState.AddModelError("", errorMessage);
        }

        protected ActionResult RedirectToLocal(string returnUrl, int id=0)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else              
                //return RedirectToAction("Index", "Complaints");
            return RedirectToAction("Overview", "Complaints", routeValues: new { id });
        }
    }
}