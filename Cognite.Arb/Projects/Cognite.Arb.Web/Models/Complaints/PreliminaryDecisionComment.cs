﻿using Cognite.Arb.Web.Models.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cognite.Arb.Web.Models.Complaints
{
    public class PreliminaryDecisionComment
    {
        public string Text { get; set; }
        public UserViewModel User { get; set; }

        public PreliminaryDecisionComment()
        {
            this.User = new UserViewModel();
        }
    }
}
