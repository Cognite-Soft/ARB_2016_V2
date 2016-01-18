using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cognite.Arb.Web.Models.Complaints
{
    public class ComplaintCommentsPreliminaryCommentsViewModel : ComplaintCommentsBase
    {
        public List<AllegationWithMyComment> AllegationsWithMyComments { get; set; }
        public List<AllegationWithComments> AllegationsWithComments { get; set; }

        public ComplaintCommentsPreliminaryCommentsViewModel()
        {
            this.AllegationsWithMyComments = new List<AllegationWithMyComment>();
            this.AllegationsWithComments = new List<AllegationWithComments>();
        }
    }
}
