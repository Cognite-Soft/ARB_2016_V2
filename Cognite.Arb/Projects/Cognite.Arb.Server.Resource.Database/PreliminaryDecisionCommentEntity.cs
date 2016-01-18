using Cognite.Arb.Server.Business.Cases;

namespace Cognite.Arb.Server.Resource.Database
{
    public class PreliminaryDecisionCommentEntity : PreliminaryDecisionComment
    {
        public virtual CaseEntity Case { get; set; }
        public virtual UserEntity PanelMember { get; set; }
    }
}