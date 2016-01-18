using System;

namespace Cognite.Arb.Server.Contract.Cases
{
    public class NewPartiesComment
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public NewDocument[] Documents { get; set; }
    }
}
