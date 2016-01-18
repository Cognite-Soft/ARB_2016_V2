using System;

namespace Cognite.Arb.Server.Business.Cases
{
    public class NewAllegation
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public NewDocument[] Documents { get; set; }
    }
}