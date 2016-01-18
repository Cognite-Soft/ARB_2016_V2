namespace Cognite.Arb.Server.Contract
{
    public class CreateCaseInfo
    {
        public int Id { get; set; }
        public ContactData ClaimantContact { get; set; }
        public string CaseManagerEmail { get; set; }

        public class ContactData
        {
            public string Name { get; set; }
            public string EMail { get; set; }
            public string Address { get; set; }
            public string Phone { get; set; }
        }
    }
}
