using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cognite.Arb.Web.Models.Complaints
{
    public enum ComplaintCommentsStepState
    {
        Complete,
        InProcess,
        WaitingForPartiesComments,
        Locked,
    }
}
