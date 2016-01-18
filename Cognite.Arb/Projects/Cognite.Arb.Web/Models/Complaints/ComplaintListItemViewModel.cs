namespace Cognite.Arb.Web.Models.Complaints
{
    public class ComplaintListItemViewModel
    {
        public int Id { get; set; }
        public int CaseId { get; set; }
        public int RegistrationId { get; set; }
        public string Complainant { get; set; }
        public string Architect { get; set; }
    }
}