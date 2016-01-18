namespace Cognite.Arb.Server.Contract.Cases
{
    public enum CaseStateKind
    {
        New,
        PreliminaryComments,
        PriliminaryDecision,
        WaitingForPartiesComments,
        FinalDecision,
        Locked,
        Rejected,
        Closed,
    }
}