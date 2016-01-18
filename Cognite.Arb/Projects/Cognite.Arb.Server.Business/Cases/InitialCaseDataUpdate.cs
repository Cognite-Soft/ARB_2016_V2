using System;

namespace Cognite.Arb.Server.Business.Cases
{
    public class InitialCaseDataUpdate
    {
        public PanelMembersUpdate PanelMembers { get; set; }
        public DateTime StartDate { get; set; }
        public string Background { get; set; }  // can be locked
        public string IdealOutcome { get; set; }  // can be locked
        public Question IssueRaisedWithArchitect { get; set; } // can be locked
        public Question SubjectOfLegalProceedings { get; set; } // can be locked
    }
}