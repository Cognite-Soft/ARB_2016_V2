using System;

namespace Cognite.Arb.Server.Contract
{
    public class CreateNotification : CreateNotificationBase
    {
        public Guid ToUserId { get; set; }
    }
}
