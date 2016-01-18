using System;

namespace Cognite.Arb.Server.Business
{
    public class CreateCaseInfo
    {
        public int Id { get; set; }
        public ContactData ClaimantContact { get; set; }
        public string CaseManagerEmail { get; set; }
        public DateTime StartDate { get; set; }

        public class ContactData
        {
            public string Name { get; set; }
            public int RegistractionNumber { get; set; }
            public string EMail { get; set; }
            public string Address { get; set; }
            public string Phone { get; set; }
        }
    }
}
