using Cognite.Arb.Web.Core;

namespace Cognite.Arb.Web.Models.Complaints
{
    public class ComplaintsFullListViewModel
    {
        public ComplaintsListViewModel Active { get; set; }
        public ComplaintsListViewModel Closed { get; set; }
        public ComplaintsListViewModel Rejected { get; set; }

        public ComplaintsFullListViewModel()
        {
            this.Active = new ComplaintsListViewModel("active") { Type = ComplaintsListType.Active };
            this.Closed = new ComplaintsListViewModel("closed") { Type = ComplaintsListType.Closed };
            this.Rejected = new ComplaintsListViewModel("rejected") { Type = ComplaintsListType.Rejected };
        }
    }
}