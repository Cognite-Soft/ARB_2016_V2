using System.Collections.Generic;

namespace Cognite.Arb.Web.Models.Complaints
{
    public class AllegationsCollection
    {
        public List<AllegationWithMyComment> Items { get; set; }
        public bool CanAddAlligation { get; set; }

        public AllegationsCollection()
        {
            this.Items = new List<AllegationWithMyComment>();
        }
    }
}