namespace Cognite.Arb.Server.Business.Cases
{
    public class CaseContacts
    {
        public string Relationship { get; set; }
        public bool? ContactAgreement { get; set; }
        public Contact ClaimantContact { get; set; }
        public ArchitectContact ArchitectContact { get; set; }
    }
}