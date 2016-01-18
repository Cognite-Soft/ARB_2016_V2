using System;

namespace Cognite.Arb.Server.Business
{
    public class DocumentActivity
    {
        public Guid Id { get; set; }
        public int CaseId { get; set; }
        public Guid DocumentId { get; set; }
        public string DocumentName { get; set; }
        public DocumentType DocumentType { get; set; }
        public ActionType ActionType { get; set; }
        public Guid UserId { get; set; }
        public DateTime Date { get; set; }
    }

    public enum DocumentType
    {
        DateAndDetail,
        PreliminaryDecision,
        PartyComment,
        FinalDecision,
    }

    public enum ActionType
    {
        Create,
        Update,
        Delete,
    }
}
