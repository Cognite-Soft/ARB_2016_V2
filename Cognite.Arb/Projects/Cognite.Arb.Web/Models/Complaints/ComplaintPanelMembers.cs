using Cognite.Arb.Web.Models.UserManagement;
namespace Cognite.Arb.Web.Models.Complaints
{
    public class ComplaintPanelMembers
    {
        public UserViewModel Member1 { get; set; }
        public UserViewModel Member2 { get; set; }
        public UserViewModel Member3 { get; set; }

        public ComplaintPanelMembers()
        {
            this.Member1 = new UserViewModel();
            this.Member2 = new UserViewModel();
            this.Member3 = new UserViewModel();
        }
    }
}