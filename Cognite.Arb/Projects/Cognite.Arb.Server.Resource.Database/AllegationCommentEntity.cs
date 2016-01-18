using System;
using Cognite.Arb.Server.Business.Cases;

namespace Cognite.Arb.Server.Resource.Database
{
    public class AllegationCommentEntity
    {
        public Guid Id { get; set; }
        public Guid AllegationId { get; set; }
        public string Text { get; set; }
        public string AdditionalText { get; set; }
        public AllegationCommentType Type { get; set; }
        public Authorship Authorship { get; set; }

        public virtual AllegationEntity Allegation { get; set; }
    }
}
