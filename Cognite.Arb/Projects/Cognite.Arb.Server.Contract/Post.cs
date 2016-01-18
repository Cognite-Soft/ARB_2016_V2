using System;

namespace Cognite.Arb.Server.Contract
{
    public class Post : Reply
    {
        public Reply Parent { get; set; }
    }

    public class Reply
    {
        public Guid Id { get; set; }
        public User User { get; set; }
        public string Text { get; set; }
    }
}
