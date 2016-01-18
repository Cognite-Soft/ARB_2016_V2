using Cognite.Arb.Server.Contract;
using Cognite.Arb.Web.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Cognite.Arb.Web.Models.Notifications
{
    public class SendNotificationAllModel : SendNotification
    {
        public int CaseId { get; set; }
    }
}