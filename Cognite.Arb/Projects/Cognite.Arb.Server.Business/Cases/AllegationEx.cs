using System;

namespace Cognite.Arb.Server.Business.Cases
{
    public class AllegationEx
    {
        public Allegation Allegation { get; set; }
        public AllegationComment[] Comments { get; set; }

        public class AllegationComment
        {
            public Guid Id { get; set; }
            public string Text { get; set; }
            public Guid PanelMemberId { get; set; }
        }
    }
}
