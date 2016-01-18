using System;
using System.Collections.Generic;
using System.Web;

namespace Cognite.Arb.Web.Models.Complaints
{
    public class AllegationWithMyComment : Allegation
    {
        public MyAllegationComment Comment { get; set; }
        public List<AllegationMiniComment> OtherComments { get; set; }

        public AllegationWithMyComment()
        {
            this.Comment = new MyAllegationComment();
            this.OtherComments = new List<AllegationMiniComment>();
        }
    }
}