using System;

namespace Cognite.Arb.Server.Contract
{
    public class CreateNotificationBase
    {
        public string Message { get; set; }
        public DeliveryType Delivery { get; set; }

        public enum DeliveryType
        {
            Email,
            System,
            Both,
        }
    }
}
