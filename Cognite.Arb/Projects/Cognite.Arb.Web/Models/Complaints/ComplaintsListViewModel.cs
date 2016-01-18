using Cognite.Arb.Web.Core;
using System.Collections.Generic;

namespace Cognite.Arb.Web.Models.Complaints
{
    public class ComplaintsListViewModel
    {
        public ComplaintsListType Type { get; set; }
        public string Info { get; set; }
        public List<ComplaintListItemViewModel> Complaints { get; set; }

        public ComplaintsListViewModel(string info)
        {
            this.Info = info;
            this.Complaints = new List<ComplaintListItemViewModel>();
        }
    }
}