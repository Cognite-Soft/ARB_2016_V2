namespace Cognite.Arb.Server.Business.Database
{
    public class CaseHeader
    {
        public int Id { get; set; }
        public string Complainant { get; set; }
        public string Architect { get; set; }
        public int? RegistrationNumber { get; set; }
        public CaseData.CaseStatus Status { get; set; }
    }
}
