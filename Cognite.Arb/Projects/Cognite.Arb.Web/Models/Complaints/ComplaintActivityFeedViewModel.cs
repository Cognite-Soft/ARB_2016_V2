using Cognite.Arb.Server.Contract;
using Cognite.Arb.Web.Models.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CaseActivityFeedAction = Cognite.Arb.Server.Contract.ActivityFeed.ActivityFeedAction;

namespace Cognite.Arb.Web.Models.Complaints
{
    public class ComplaintActivityFeedViewModel
    {
        public DateTime? StartDate { get; set; }
        public List<ComplaintActivityFeedSectionViewModel> Sections { get; set; }

        public ComplaintActivityFeedViewModel()
        {
            this.Sections = new List<ComplaintActivityFeedSectionViewModel>();
        }
    }

    public class ComplaintActivityFeedSectionViewModel
    {
        public ActivityFeed.ActivityFeedSectionStatus SectionStatus { get; set; }
        public ActivityFeed.ActivityFeedSectionType SectionType { get; set; }
        public ComplaintActivityFeedHeaderViewModel Header { get; set; }
        public List<ComplaintActivityFeedItemViewModel> Items { get; set; }

        public ComplaintActivityFeedSectionViewModel()
        {
            this.Header = new ComplaintActivityFeedHeaderViewModel();
            this.Items = new List<ComplaintActivityFeedItemViewModel>();
        }
    }

    public class ComplaintActivityFeedHeaderViewModel
    {
        public DateTime DueDate { get; set; }
        public string Title { get; set; }
        public string Class { get; set; }
    }

    public class ComplaintActivityFeedItemViewModel
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public UserViewModel User { get; set; }
        public CaseActivityFeedAction Action { get; set; }
        public string ActionText
        {
            get
            {
                switch (Action)
                {
                    case ActivityFeed.ActivityFeedAction.PreliminaryAllegationComment:
                    case ActivityFeed.ActivityFeedAction.PreliminaryDecisionComment:
                    case ActivityFeed.ActivityFeedAction.FinalDecisionComment:
                        return "said";
                    case ActivityFeed.ActivityFeedAction.CreateDocument:
                        return "added document";
                    case ActivityFeed.ActivityFeedAction.UpdateDocument:
                        return "modified document";
                    case ActivityFeed.ActivityFeedAction.DeleteDocument:
                        return "deleted document";
                    case ActivityFeed.ActivityFeedAction.Discussion:
                        return "created discussion";
                    case ActivityFeed.ActivityFeedAction.DiscussionComment:
                        return "said in discussion";
                }

                return String.Empty;
            }
        }

        public ComplaintActivityFeedItemViewModel()
        {
            this.User = new UserViewModel();
        }
    }
}
