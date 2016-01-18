using Cognite.Arb.Web.Models.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cognite.Arb.Web.Models.Complaints
{
    public class AllegationMiniComment
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public UserViewModel User { get; set; }

        public AllegationMiniComment()
        {
            this.User = new UserViewModel();
        }
    }
}
