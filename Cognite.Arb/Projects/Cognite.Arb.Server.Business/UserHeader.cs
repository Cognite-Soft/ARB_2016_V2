using System;

namespace Cognite.Arb.Server.Business.Database
{
    public class UserHeader
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Role Role { get; set; }
    }
}
