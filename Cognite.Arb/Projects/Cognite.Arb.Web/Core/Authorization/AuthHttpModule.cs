using Cognite.Arb.Web.Core.Authorization.Interfaces;
using System;
using System.Web;
using System.Web.Mvc;

namespace Cognite.Arb.Web.Core.Authorization
{
    public class AuthHttpModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.AuthenticateRequest += new EventHandler(this.Authenticate);
        }

        private void Authenticate(object sender, EventArgs e)
        {
            var application = sender as HttpApplication;
            if (application != null)
                Authenticate(application);
        }

        private void Authenticate(HttpApplication application)
        {
            var context = application.Context;
            var auth = DependencyResolver.Current.GetService<IAuthentication>();

            auth.HttpContext = context;
            context.User = auth.CurrentUser;
        }
        
        public void Dispose() { }
    }
}