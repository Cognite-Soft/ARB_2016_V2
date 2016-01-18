using System;

namespace Cognite.Arb.Server.Resource.Database
{
    public class DecisionDocumentEntity
    {
        public int CaseId { get; set; }
        public bool Final { get; set; }
        public Guid DocumentId { get; set; }
    }
}
