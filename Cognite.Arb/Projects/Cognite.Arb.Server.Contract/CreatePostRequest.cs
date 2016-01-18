using System;

namespace Cognite.Arb.Server.Contract
{
    public class CreatePostRequest
    {
        public Guid PostId { get; set; }
        public string Text { get; set; }
    }
}
