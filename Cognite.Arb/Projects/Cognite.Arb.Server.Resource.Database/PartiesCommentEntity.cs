using System;
using System.Collections.Generic;

namespace Cognite.Arb.Server.Resource.Database
{
    public class PartiesCommentEntity
    {
        public PartiesCommentEntity()
        {
            Documents = new List<DocumentEntity>();
        }

        public Guid Id { get; set; }
        public int CaseId { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }

        public virtual CaseEntity Case { get; set; }
        public virtual ICollection<DocumentEntity> Documents { get; protected set; } 
    }
}
