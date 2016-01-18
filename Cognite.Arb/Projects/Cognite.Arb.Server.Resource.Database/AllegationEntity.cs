using System;
using System.Collections.Generic;

namespace Cognite.Arb.Server.Resource.Database
{
    public class AllegationEntity
    {
        public AllegationEntity()
        {
            Documents = new List<DocumentEntity>();
            Comments = new List<AllegationCommentEntity>();
        }

        public Guid Id { get; set; }
        public int CaseId { get; set; }
        public string Text { get; set; }
        public Authorship Authorship { get; set; }
        public int Order { get; set; }

        public virtual CaseEntity Case { get; set; }
        public virtual ICollection<AllegationCommentEntity> Comments { get; protected set; }
        public virtual ICollection<DocumentEntity> Documents { get; protected set; } 
    }
}
