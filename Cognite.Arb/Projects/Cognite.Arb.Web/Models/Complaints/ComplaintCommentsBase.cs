using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cognite.Arb.Web.Models.Complaints
{
    public class ComplaintCommentsBase
    {
        public ComplaintCommentsStepState State { get; set; }
        public int DueDays { get; set; }
        public DateTime DueDate { get; set; }
        public string DueDateText { get { return this.DueDate.ToShortDateString(); } }
    }
}
