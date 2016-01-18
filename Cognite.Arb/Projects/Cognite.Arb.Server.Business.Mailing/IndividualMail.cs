namespace Cognite.Arb.Server.Business.Mailing
{
    public class IndividualMail
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsHtml { get; set; }
        public Attachment[] Attachments { get; set; }

        public class Attachment
        {
            public string Name { get; set; }
            public byte[] Data { get; set; }
        }
    }
}