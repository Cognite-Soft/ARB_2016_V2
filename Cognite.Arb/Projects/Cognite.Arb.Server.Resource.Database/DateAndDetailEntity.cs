using System;
using System.Collections.Generic;

namespace Cognite.Arb.Server.Resource.Database
{
    public class DateAndDetailEntity
    {
        public DateAndDetailEntity()
        {
            Documents = new List<DocumentEntity>();
        }

        public Guid Id { get; set; }
        public int CaseId { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }
        public Authorship Authorship { get; set; }
        public int Order { get; set; }

        public virtual CaseEntity Case { get; set; }
        public virtual ICollection<DocumentEntity> Documents { get; set; }
    }
}
