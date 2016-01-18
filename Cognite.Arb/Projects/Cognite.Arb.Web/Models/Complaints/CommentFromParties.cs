using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cognite.Arb.Web.Models.Complaints
{
    public class CommentFromParties
    {
        public string Title { get; set; }
        public List<Document> Documents { get; set; }
    }
}
