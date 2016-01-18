using System;

namespace Cognite.Arb.Server.Business.Cases
{
    public class NewPartiesComment
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public NewDocument[] Documents { get; set; }
        public int CaseId { get; set; }
    }
}
