using System;

namespace Cognite.Arb.Server.Contract
{
    public class Schedule
    {
        public int Id { get; set; }
        public User First { get; set; }
        public User Second { get; set; }
        public User Third { get; set; }
        public DateTime LastUsed { get; set; }
    }
}
