namespace Cognite.Arb.Server.Business.Cases
{
    public class Contact
    {
        public string Name { get; set; } // InvertedDisplayName
        public string EMail { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; } // phone, business, fax
    }
}