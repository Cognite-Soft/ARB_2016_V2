using Cognite.Arb.Web.Core;

namespace Cognite.Arb.Web.Models.Complaints
{
    public class Question
    {
        public bool Answer { get; set; }
        public string AnswerText { get { return this.Answer.GetBooleanYesNoText(); } }
        public string Comments { get; set; }
    }
}