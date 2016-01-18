using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Mvc;
using Cognite.Arb.Server.Business;
using Cognite.Arb.Server.Business.Database;
using Cognite.Arb.Server.Contract;
using Cognite.Arb.Server.WebApi.ExceptionHandling;
using Cognite.Arb.Server.WebApi.Security;
using Role = Cognite.Arb.Server.Contract.Role;

namespace Cognite.Arb.Server.WebApi.Controllers
{
    [AuthorizationRequired]
    [Security.Authorize(Role.Admin, Role.CaseWorker, Role.PanelMember, Role.Inquirer, Role.Solicitor, Role.ThirdPartyReviewer)]
    public class NotificationsController : ApiController
    {
        [System.Web.Http.Route("api/notifications", Name = "CreateNotification", Order = 100)]
        [System.Web.Http.HttpPost]
        [MapException(typeof(Facade.ForbiddenException), HttpStatusCode.Forbidden)]
        public void CreateNotification([FromBody] CreateNotification request)
        {
            var fromUser = CurrentUser.Get<UserHeader>();
            var facade = Main.CreateFacade();
            facade.CreateMessage(Guid.NewGuid(), fromUser.Id, request.ToUserId, request.Message, request.Delivery);
        }

        [System.Web.Http.Route("api/notifications", Name = "GetNotifications", Order = 100)]
        [System.Web.Http.HttpGet]
        [MapException(typeof(Facade.ForbiddenException), HttpStatusCode.Forbidden)]
        public Notification[] GetNotifications()
        {
            var toUser = CurrentUser.Get<UserHeader>();
            var facade = Main.CreateFacade();
            var messages = facade.GetMessages(toUser.Id);
            var result = messages.Select(x =>
                new Notification
                {
                    Id = x.Id,
                    Message = x.Text,
                    DateTime = x.Created,
                    From = new User
                    {
                        Id = x.FromUser.Id,
                        Email = x.FromUser.Email,
                        Role = (Role)x.FromUser.Role,
                        FirstName = x.FromUser.FirstName,
                        LastName = x.FromUser.LastName,
                    },
                }).ToArray();
            return result;
        }

        [System.Web.Http.Route("api/notifications/{id}", Name = "DimissNotification", Order = 100)]
        [System.Web.Http.HttpDelete]
        public void DimissNotification(Guid id)
        {
            var facade = Main.CreateFacade();
            facade.AcceptMessage(CurrentUser.Get<UserHeader>().Id, id);
        }
    }
}