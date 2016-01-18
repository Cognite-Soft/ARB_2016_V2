using System;

namespace Cognite.Arb.Server.Contract.Cases
{
    public class CaseState
    {
        public CaseStateKind StateKind { get; set; }
        public DateTime DueByDate { get; set; }
        public int DueDaysLeft { get; set; }
    }
}