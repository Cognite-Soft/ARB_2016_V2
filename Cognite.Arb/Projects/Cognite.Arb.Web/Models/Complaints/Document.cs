using System;
using System.Web;

namespace Cognite.Arb.Web.Models.Complaints
{
    public class Document
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public byte[] File { get; set; }
    }
}