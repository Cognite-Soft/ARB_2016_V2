using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cognite.Arb.Web.Models.Complaints
{
    public class ComplaintCommentsViewModel
    {
        public ComplaintCommentsPreliminaryCommentsViewModel PreliminaryComments { get; set; }
        public bool RenderPreliminaryComments { get; set; }

        public ComplaintCommentsPreliminaryDecisionViewModel PreliminaryDecision { get; set; }
        public bool RenderPreliminaryDecision { get; set; }

        public ComplaintCommentsFinalDecisionViewModel FinalDecision { get; set; }
        public bool RenderFinalDecision { get; set; }

        public List<Document> AllDocuments { get; set; }

        public ComplaintCommentsViewModel()
        {
            this.PreliminaryComments = new ComplaintCommentsPreliminaryCommentsViewModel();
            this.PreliminaryDecision = new ComplaintCommentsPreliminaryDecisionViewModel();
            this.FinalDecision = new ComplaintCommentsFinalDecisionViewModel();
            this.AllDocuments = new List<Document>();
        }
    }
}
