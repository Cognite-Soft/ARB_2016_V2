using Cognite.Arb.Server.Contract;
using Cognite.Arb.Web.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Cognite.Arb.Web.Models.Notifications
{
    public class SendNotificationModel
    {
        [Required(ErrorMessage = GlobalStrings.MessageIsRequired)]
        [Display(Name = "Notification message")]
        public string Message { get; set; }

        [Required(ErrorMessage = GlobalStrings.NotificationTypeIsRequired)]
        [Display(Name = "Notification message type")]
        public CreateNotification.DeliveryType Type { get; set; }
        
        public Guid UserId { get; set; }
    }
}