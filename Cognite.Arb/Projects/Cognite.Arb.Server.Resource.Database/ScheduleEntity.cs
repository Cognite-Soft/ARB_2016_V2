using System;

namespace Cognite.Arb.Server.Resource.Database
{
    public class ScheduleEntity
    {
        public int Id { get; set; }

        public Guid? User1Id { get; set; }
        public Guid? User2Id { get; set; }
        public Guid? User3Id { get; set; }
        public DateTime? LastUsed { get; set; }

        public virtual UserEntity User1 { get; set; }
        public virtual UserEntity User2 { get; set; }
        public virtual UserEntity User3 { get; set; }
    }
}
