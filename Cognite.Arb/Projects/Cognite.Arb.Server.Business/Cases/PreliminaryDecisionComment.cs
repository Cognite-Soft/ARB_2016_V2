using System;

namespace Cognite.Arb.Server.Business.Cases
{
    public class PreliminaryDecisionComment
    {
        public Guid Id { get; set; }
        public int CaseId { get; set; }
        public Guid PanelMemberId { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }
    }
}
