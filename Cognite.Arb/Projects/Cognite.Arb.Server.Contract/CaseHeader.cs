namespace Cognite.Arb.Server.Contract
{
    public class CaseHeader
    {
        public int Id { get; set; }
        public string Complainant { get; set; }
        public string Architect { get; set; }
        public int? RegistrationNumber { get; set; }
        public CaseStatus Status { get; set; }
    }
}
