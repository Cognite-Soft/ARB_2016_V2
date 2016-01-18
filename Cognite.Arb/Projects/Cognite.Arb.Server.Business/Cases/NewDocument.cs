using System;

namespace Cognite.Arb.Server.Business.Cases
{
    public class NewDocument
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public byte[] Body { get; set; }
    }
}