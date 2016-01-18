using System;

namespace Cognite.Arb.Server.Business.Cases
{
    public class MyAllegationComment
    {
        public AllegationCommentType AllegationCommentType { get; set; }
        public Guid Id { get; set; }
        public string Text { get; set; }
        public string AdditionalText { get; set; }
    }
}