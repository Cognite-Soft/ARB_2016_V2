using System;
using System.Web.Http;
using Cognite.Arb.Server.Business.Cases;
using Cognite.Arb.Server.Contract;
using Cognite.Arb.Server.WebApi.Security;

namespace Cognite.Arb.Server.WebApi.Controllers
{
    [AuthorizationRequired]
    [Security.Authorize(Role.Admin, Role.CaseWorker, Role.PanelMember, Role.Inquirer, Role.Solicitor, Role.ThirdPartyReviewer)]
    public class DocumentsController : ApiController
    {
        [Route("api/documents/{id}", Name = "GetDocument", Order = 90)]
        [HttpGet]
        public NewDocument GetDocument(Guid id)
        {
            var facade = Main.CreateFacade();
            var document = facade.GetDocument(id);
            return new NewDocument
            {
                Id = document.Id,
                Name = document.Name,
                Body = document.Body,
            };
        }
    }
}