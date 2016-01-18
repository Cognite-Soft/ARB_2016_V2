using Cognite.Arb.Server.Contract.Cases;
using System;

namespace Cognite.Arb.Web.Models.Complaints
{
    public class ComplaintState
    {
        public CaseStateKind Type { get; set; }
        public string TypeText { get { return GetTypeText(); } }
        public DateTime DueDate { get; set; }
        public int DaysLeft { get; set; }

        private string GetTypeText()
        {
            switch (Type)
            {
                case CaseStateKind.New:
                    return "New";
                case CaseStateKind.PreliminaryComments:
                    return "Preliminary Comments";
                case CaseStateKind.PriliminaryDecision:
                    return "Preliminary Decision";
                case CaseStateKind.WaitingForPartiesComments:
                    return "Waiting for parties comments";
                case CaseStateKind.FinalDecision:
                    return "Final Decision";
                case CaseStateKind.Locked:
                    return "Locked";
                case CaseStateKind.Rejected:
                    return "Rejected";
                case CaseStateKind.Closed:
                    return "Closed";
            }

            return String.Empty;
        }
    }
}