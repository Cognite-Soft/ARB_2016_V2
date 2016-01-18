using System;

namespace Cognite.Arb.Server.Business.Database
{
    public class Schedule
    {
        public int Id { get; set; }
        public UserHeader First { get; set; }
        public UserHeader Second { get; set; }
        public UserHeader Third { get; set; }
        public DateTime? LastUsed { get; set; }
    }
}
