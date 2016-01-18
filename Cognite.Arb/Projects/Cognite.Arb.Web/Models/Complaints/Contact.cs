using Cognite.Arb.Web.Core;
using System.ComponentModel.DataAnnotations;

namespace Cognite.Arb.Web.Models.Complaints
{
    public class Contact
    {
        [EmailAddress(ErrorMessage = GlobalStrings.EmailIsNotValid)]
        [Display(Name = "Email")]
        public string EMail { get; set; }
        public string Name { get; set; }        
        public string Address { get; set; }
        public string Phone { get; set; }
    }
}