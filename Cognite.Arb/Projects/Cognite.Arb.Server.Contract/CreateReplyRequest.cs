using System;

namespace Cognite.Arb.Server.Contract
{
    public class CreateReplyRequest
    {
        public Guid ReplyToPostId { get; set; }
        public Guid ReplyId { get; set; }
        public string Text { get; set; }
    }
}
