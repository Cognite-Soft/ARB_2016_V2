using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Cognite.Arb.Web.Models.Complaints
{
    public class DateAndDetail
    {
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Date { get; set; }

        public Guid Id { get; set; }
        public string Text { get; set; }
        public List<Document> Documents { get; set; }
        public bool CanBeDeleted { get; set; }

        public DateAndDetail()
        {
            this.Date = DateTime.Now;
            this.Documents = new List<Document>();
            this.CanBeDeleted = true;
        }
    }
}