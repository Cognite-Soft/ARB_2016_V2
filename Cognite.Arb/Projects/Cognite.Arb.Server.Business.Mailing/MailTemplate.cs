using System;

namespace Cognite.Arb.Server.Business.Mailing
{
    public class MailTemplate
    {
        public string Subject { get; set; }
        public bool IsHtml { get; set; }
        public string BodyTemplate { get; set; }

        public string ApplyTemplate(Guid id, string mail, string firstName, string lastName, DateTime expiration, string token, string baseUrl)
        {
            return string.Format(BodyTemplate, id, mail, firstName, lastName, expiration, token, baseUrl);
        }

        public string ApplyTemplate(Guid id, string mail, string firstName, string lastName)
        {
            return string.Format(BodyTemplate, id, mail, firstName, lastName);
        }
    }
}
