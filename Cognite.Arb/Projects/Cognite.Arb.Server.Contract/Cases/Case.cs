namespace Cognite.Arb.Server.Contract.Cases
{
    public class Case
    {
        public ReadonlyCaseData ReadonlyData { get; set; } 
        public InitialCaseData InitialData { get; set; }
        public CaseContacts Contacts { get; set; }
        public DateAndDetail[] DatesAndDetails { get; set; }
        public AllegationCollection Allegations { get; set; }
        public PartiesComment[] PartiesComments { get; set; }
        public Document PreliminaryDecisionDocument { get; set; }
        public Document FinalDecisionDocument { get; set; }
    }

    public class PartiesComment
    {
        public string Text { get; set; }
        public Document[] Documents { get; set; }
    }
}
