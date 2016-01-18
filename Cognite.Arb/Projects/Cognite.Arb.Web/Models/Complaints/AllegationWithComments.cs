using Cognite.Arb.Web.Models.UserManagement;
using System;
using System.Collections.Generic;

namespace Cognite.Arb.Web.Models.Complaints
{
    public class AllegationWithComments : Allegation
    {
        public List<AllegationMiniComment> Comments { get; set; }

        public AllegationWithComments()
        {
            this.Comments = new List<AllegationMiniComment>();
        }
    }
}