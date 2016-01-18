using Cognite.Arb.Server.Contract;
using Cognite.Arb.Web.Core;
using System.ComponentModel.DataAnnotations;

namespace Cognite.Arb.Web.Models.Notifications
{
    public class SendNotification
    {
        [Required(ErrorMessage = GlobalStrings.MessageIsRequired)]
        [Display(Name = "Notification message")]
        public string Message { get; set; }

        [Required(ErrorMessage = GlobalStrings.NotificationTypeIsRequired)]
        [Display(Name = "Notification message type")]
        public CreateNotificationBase.DeliveryType Type { get; set; }
    }
}