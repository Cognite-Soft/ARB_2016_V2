namespace Cognite.Arb.Server.Business.Cases
{
    public enum CaseStateKind
    {
        New,
        PriliminaryComments,
        PriliminaryDecision,
        WaitingForPartyComments,
        FinalDecision,
        Locked,
        Rejected,
        Closed,
    }
}