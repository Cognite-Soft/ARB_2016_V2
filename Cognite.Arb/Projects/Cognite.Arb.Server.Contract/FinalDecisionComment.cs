using Cognite.Arb.Server.Contract.Cases;

namespace Cognite.Arb.Server.Contract
{
    public class FinalDecisionComment
    {
        public FinalDecisionType Type { get; set; }
        public string CommentForChange { get; set; }
    }
}
