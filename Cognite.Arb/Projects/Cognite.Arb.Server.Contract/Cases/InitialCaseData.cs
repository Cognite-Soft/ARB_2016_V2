using System;

namespace Cognite.Arb.Server.Contract.Cases
{
    public class InitialCaseData
    {
        public CasePanelMembers CasePanelMembers { get; set; }
        public DateTime StartDate { get; set; }
        public string Background { get; set; }  // can be locked
        public string IdealOutcome { get; set; }  // can be locked
        public Question IssueRaisedWithArchitect { get; set; } // can be locked
        public Question SubjectOfLegalProceedings { get; set; } // can be locked
        public bool CanBeEdited { get; set; }
        public bool IsReady { get; set; }
    }
}