using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cognite.Arb.Web.Models.Complaints
{
    public class ComplaintDiscussionsViewModel
    {
        public List<DiscussionComment> Replies { get; set; }

        public ComplaintDiscussionsViewModel()
        {
            this.Replies = new List<DiscussionComment>();
        }
    }
}
