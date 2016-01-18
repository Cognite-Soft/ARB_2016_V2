using System;

namespace Cognite.Arb.Server.Business
{
    public class Message
    {
        public Guid Id { get; set; }
        public Guid FromUserId { get; set; }
        public Guid ToUserId { get; set; }
        public string Text { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Accepted { get; set; }
    }
}
