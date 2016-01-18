using System;
using System.Collections.Generic;

namespace Cognite.Arb.Server.Resource.Database
{
    public class DocumentEntity
    {
        public DocumentEntity()
        {
            PreliminaryDecisionCases = new List<CaseEntity>();
            FinalDecisionCases = new List<CaseEntity>();
            Allegations = new List<AllegationEntity>();
            DateAndDetails = new List<DateAndDetailEntity>();
            PartiesComments = new List<PartiesCommentEntity>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<CaseEntity> PreliminaryDecisionCases { get; set; }
        public virtual ICollection<CaseEntity> FinalDecisionCases { get; set; }
        public virtual ICollection<AllegationEntity> Allegations { get; set; }
        public virtual ICollection<DateAndDetailEntity> DateAndDetails { get; set; }
        public virtual ICollection<PartiesCommentEntity> PartiesComments { get; set; } 
    }
}
