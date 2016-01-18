using Cognite.Arb.Web.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cognite.Arb.Web.Controllers
{
    public class DocumentsController : BaseController
    {
        [Authorize]
        public ActionResult Download(Guid id)
        {
            return TryDownloadDocument(id);
        }

        private ActionResult TryDownloadDocument(Guid id)
        {
            try
            {
                var document = this.Service.GetDocument(this.SecurityToken, id);
                if (document == null)
                    return View("Shared/NoFile");

                return File(document.Body, "application/x-binary", document.Name);
            }
            catch (Exception)
            {
                return View("Shared/NoFile");
            }
        }
    }
}