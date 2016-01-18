using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cognite.Arb.Web.Models.Complaints
{
    public class ComplaintCommentsPreliminaryDecisionViewModel : ComplaintCommentsBase
    {
        public List<CommentFromParties> CommentsFromParties { get; set; }
        public List<PreliminaryDecisionComment> Comments { get; set; }
        public Document PreliminaryDecisionDocument { get; set; }

        public ComplaintCommentsPreliminaryDecisionViewModel()
        {
            this.CommentsFromParties = new List<CommentFromParties>();
            this.Comments = new List<PreliminaryDecisionComment>();
        }
    }
}
