using System;
using Cognite.Arb.Server.Business.Mailing;
using Cognite.Arb.Server.Resource.MailSender;
using NUnit.Framework;

namespace Cognite.Arb.Server.Resouce.MailSender.Tests
{
    [TestFixture]
    public class MailSenderTests
    {
        [Test, Explicit]
        public void SendMail()
        {
            var sender = new SmtpMailSender();
            var mail = new IndividualMail
            {
                From = "ict-8047@scnsoft.com",
                To = "kisialiou@scnsoft.com",
                Subject = "mail sender test",
                Body = "mail sender test " + DateTime.Now,
                IsHtml = false,
            };
            sender.SendMail(mail);
        }
    }
}