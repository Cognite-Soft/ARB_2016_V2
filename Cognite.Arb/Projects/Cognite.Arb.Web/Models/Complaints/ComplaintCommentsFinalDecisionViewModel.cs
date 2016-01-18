using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cognite.Arb.Web.Models.Complaints
{
    public class ComplaintCommentsFinalDecisionViewModel : ComplaintCommentsBase
    {
        public bool ApprovedByCurrentUser { get; set; }
        public Document Document { get; set; }
    }
}
