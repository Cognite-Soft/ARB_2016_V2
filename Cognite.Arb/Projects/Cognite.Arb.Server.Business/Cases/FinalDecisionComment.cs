using System;

namespace Cognite.Arb.Server.Business.Cases
{
    public class FinalDecisionComment
    {
        public Guid Id { get; set; }
        public int CaseId { get; set; }
        public Guid PanelMemberId { get; set; }
        public DateTime Date { get; set; }
        public FinalDecisionCommentKind Decision { get; set; }
        public string CommentForChange { get; set; }
    }

    public enum FinalDecisionCommentKind
    {
        Accept,
        Ammend,
        Changed,
    }
}
