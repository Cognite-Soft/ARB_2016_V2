using System;
using System.Collections.Generic;

namespace Cognite.Arb.Web.Models.Complaints
{
    public class Allegation
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public List<Document> Documents { get; set; }
        public bool CanBeDeleted { get; set; }

        public Allegation()
        {
            this.Documents = new List<Document>();
        }
    }
}