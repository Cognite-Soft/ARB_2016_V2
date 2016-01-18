using Cognite.Arb.Web.Core.Mappers;
using Cognite.Arb.Server.Contract;
using Cognite.Arb.Web.Models.Complaints;
using Cognite.Arb.Web.Core;
using Cognite.Arb.Web.Core.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using AllegationCommentType = Cognite.Arb.Server.Contract.Cases.AllegationCommentType;
using Cognite.Arb.Web.Models.Additional;

namespace Cognite.Arb.Web.Controllers
{
    public partial class ComplaintsController : BaseController
    {
        public ComplaintsController()
        {
            ViewBag.CasesOptions = CreateComplaintsOptions();
        }

        private List<PageInfoComplaintOption> CreateComplaintsOptions()
        {
            var result = new List<PageInfoComplaintOption>();

            var complaints = GetComplaintsForPageInfoSection();
            foreach (var complaint in complaints)
                result.Add(GetPageInfoOption(complaint));

            return result;
        }

        private List<CaseHeader> GetComplaintsForPageInfoSection()
        {
            List<CaseHeader> result = new List<CaseHeader>();
            try { result.AddRange(this.Service.GetActiveCases(this.SecurityToken)); } catch { }
            return result;
        }

        private PageInfoComplaintOption GetPageInfoOption(CaseHeader complaint)
        {
            return new PageInfoComplaintOption()
            {
                Id = complaint.Id,
                Title = String.Format("#{0}", complaint.Id),
                Value = String.Format("/Complaints/Overview/{0}", complaint.Id),
            };
        }
    }
}
