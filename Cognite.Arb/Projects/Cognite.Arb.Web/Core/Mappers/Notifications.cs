using Cognite.Arb.Server.Contract;
using Cognite.Arb.Web.Models.Account;
using Cognite.Arb.Web.Models.Complaints;
using System;
using System.Collections.Generic;
using Cognite.Arb.Web.ServiceClient;
using Cognite.Arb.Web.Models.Notifications;

namespace Cognite.Arb.Web.Core.Mappers
{
    internal static partial class Mappers
    {
        internal static List<NotificationModel> MapNotifications(Notification[] notifications)
        {
            var result = new List<NotificationModel>();

            foreach (var notification in notifications)
                result.Add(Mappers.MapNotification(notification));

            return result;
        }

        private static NotificationModel MapNotification(Notification notification)
        {
            return new NotificationModel()
            {
                Id = notification.Id,
                Message = notification.Message,
                DateTime = notification.DateTime.ToString(),
                From = notification.From.FullName(),
                FromMail = notification.From.Email,
            };
        }
    }
}