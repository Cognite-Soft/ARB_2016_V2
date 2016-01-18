using System;
using System.Collections.Generic;

namespace Cognite.Arb.Server.Resource.Database
{
    public class PostEntity
    {
        public PostEntity()
        {
            Children = new List<PostEntity>();
        }

        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public int CaseId { get; set; }
        public DateTime Date { get; set; }
        public Guid UserId { get; set; }
        public string Text { get; set; }

        public virtual PostEntity Parent { get; set; }
        public virtual ICollection<PostEntity> Children { get; set; }
        public virtual UserEntity User { get; set; } 
    }
}
