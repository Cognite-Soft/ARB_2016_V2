using System;

namespace Cognite.Arb.Server.Contract.Cases
{
    public class DateAndDetail
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }
        public Document[] Documents { get; set; }
    }
}