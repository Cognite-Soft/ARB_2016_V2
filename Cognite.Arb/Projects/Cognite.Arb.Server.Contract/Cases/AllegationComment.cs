namespace Cognite.Arb.Server.Contract.Cases
{
    public class AllegationComment
    {
        public string Text { get; set; }
        public AllegationCommentType Type { get; set; }
        public string AdditionalText { get; set; }
    }
}
