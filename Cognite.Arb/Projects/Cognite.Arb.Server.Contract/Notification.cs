using System;

namespace Cognite.Arb.Server.Contract
{
    public class Notification
    {
        public Guid Id { get; set; }
        public User From { get; set; }
        public string Message { get; set; }
        public DateTime DateTime { get; set; }
    }
}
