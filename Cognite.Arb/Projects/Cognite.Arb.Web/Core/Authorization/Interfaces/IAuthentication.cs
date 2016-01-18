using Cognite.Arb.Server.Contract;
using System.Security.Principal;
using System.Web;

namespace Cognite.Arb.Web.Core.Authorization.Interfaces
{
    public interface IAuthentication
    {
        HttpContext HttpContext { get; set; }
        SecurePhraseQuestion Login(string email, string password);
        User Login(string securityToken, SecurePhraseAnswer sequrePhraseAnswer);
        User Login(string securityToken);
        void LogOut();
        IPrincipal CurrentUser { get; }
        User GetCurrentUser();
    }
}