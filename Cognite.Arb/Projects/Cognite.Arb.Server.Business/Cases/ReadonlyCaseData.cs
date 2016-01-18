namespace Cognite.Arb.Server.Business.Cases
{
    public class ReadonlyCaseData
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public CaseState State { get; set; }
    }
}