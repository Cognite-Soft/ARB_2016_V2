namespace Cognite.Arb.Server.Contract.Cases
{
    public class ReadonlyCaseData
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public CaseState State { get; set; }
    }
}