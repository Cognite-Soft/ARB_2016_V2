using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using Cognite.Arb.Server.Business.Mailing;

namespace Cognite.Arb.Server.Resource.MailSender
{
    public class SmtpMailSender : IMailSender
    {
        public bool SendMail(IndividualMail mail)
        {
            var mailClient = new SmtpClient();

            var message = new MailMessage
            {
                From = new MailAddress(mail.From),
                Body = mail.Body,
                IsBodyHtml = mail.IsHtml,
                Subject = mail.Subject
            };

            message.To.Add(mail.To);

            try
            {
                using (AddAttachments(mail, message))
                    mailClient.Send(message);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private static DisposableAttachments AddAttachments(IndividualMail mail, MailMessage message)
        {
            var result = new DisposableAttachments();
            if (mail.Attachments != null)
            {
                foreach (var attachment in mail.Attachments)
                {
                    var stream = new MemoryStream(attachment.Data);
                    result.AddStream(stream);
                    var mailAttachment = new Attachment(stream, attachment.Name);
                    message.Attachments.Add(mailAttachment);
                }
            }
            return result;
        }

        private class DisposableAttachments : IDisposable
        {
            private readonly List<MemoryStream> _streams = new List<MemoryStream>();

            public void AddStream(MemoryStream stream)
            {
                _streams.Add(stream);
            }

            public void Dispose()
            {
                foreach (var stream in _streams)
                    stream.Dispose();
            }
        }
    }
}
