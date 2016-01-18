using System;

namespace Cognite.Arb.Server.Business.Mailing
{
    public class ResetNotification
    {
        public Guid UserId { get; set; }
        public string UserEmail { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string ResetToken { get; set; }
        public DateTime ExpirationTime { get; set; }
    }
}
