using System;

namespace Cognite.Arb.Server.Contract.Cases
{
    public class ComplaintComments
    {
        public DateTime? DueDate { get; set; }
        public int? DueDaysLeft { get; set; }
        public StatusKind Status { get; set; }
        public PreliminaryCommentsData PreliminaryComments { get; set; }
        public PreliminaryDecisionCommentsData PreliminaryDecisionComments { get; set; }
        public Document FinalDecisionDocument { get; set; }
        public bool? FinalDecisionApprovedByCurrentUser { get; set; }

        public class PreliminaryCommentsData
        {
            public AllegationWithMyComment[] AllegationsWithMyComments { get; set; }
            public AllegationWithComments[] AllegationsWithComments { get; set; }
        }

        public class Allegation
        {
            public Guid Id { get; set; }
            public string Text { get; set; }
            public Document[] Documents { get; set; }
        }

        public class AllegationWithMyComment : Allegation
        {
            public MyAllegationComment MyComment { get; set; }
            public AllegationComment[] OtherComments { get; set; }
        }

        public class AllegationWithComments : Allegation
        {
            public AllegationComment[] Comments { get; set; }
        }

        public class AllegationComment
        {
            public Guid Id { get; set; }
            public string Text { get; set; }
            public User User { get; set; }
        }

        public class PreliminaryDecisionCommentsData
        {
            public CommentFromParties[] CommentsFromParies { get; set; }
            public AllegationComment[] Comments { get; set; }
            public Document PreliminaryDecisionDocument { get; set; }
        }

        public class CommentFromParties
        {
            public string Text { get; set; }
            public Document[] Documents { get; set; }
        }

        public enum StatusKind
        {
            PreliminaryComments,
            PreliminaryDecisionComments,
            PreliminaryDecisionWaiting,
            FinalDecisionComments,
            FinalDecisionLocked,
            Complete,
        }
    }
}
