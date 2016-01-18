using Cognite.Arb.Web.Models.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cognite.Arb.Web.Models.Notifications
{
    public class NotificationModel
    {
        public Guid Id { get; set; }
        public string Message { get; set; }
        public string DateTime { get; set; }
        public string From { get; set; }
        public string FromMail { get; set; }
    }
}