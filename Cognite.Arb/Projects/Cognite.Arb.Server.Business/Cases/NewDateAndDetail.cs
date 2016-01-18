using System;

namespace Cognite.Arb.Server.Business.Cases
{
    public class NewDateAndDetail
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }
        public NewDocument[] Documents { get; set; }
    }
}