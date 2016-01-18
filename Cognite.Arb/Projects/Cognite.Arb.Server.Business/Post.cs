using System;
using Cognite.Arb.Server.Business.Database;

namespace Cognite.Arb.Server.Business
{
    public class Post : Reply
    {
        public Reply Parent { get; set; }
    }

    public class Reply
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public UserHeader User { get; set; }
        public string Text { get; set; }
    }
}
