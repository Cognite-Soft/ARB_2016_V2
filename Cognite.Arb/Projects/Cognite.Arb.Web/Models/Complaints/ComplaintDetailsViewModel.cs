using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cognite.Arb.Web.Models.Complaints
{
    public class ComplaintDetailsViewModel
    {
        public int Id { get; set; }
        public ComplaintOverviewViewModel Overview { get; set; }
        public ComplaintActivityFeedViewModel ActivityFeed { get; set; }
        public ComplaintCommentsViewModel Comments { get; set; }
        public ComplaintDiscussionsViewModel Discussions { get; set; }

        public ComplaintDetailsViewModel()
        {
            this.Overview = new ComplaintOverviewViewModel();
            this.ActivityFeed = new ComplaintActivityFeedViewModel();
            this.Comments = new ComplaintCommentsViewModel();
            this.Discussions = new ComplaintDiscussionsViewModel();
        }
    }
}