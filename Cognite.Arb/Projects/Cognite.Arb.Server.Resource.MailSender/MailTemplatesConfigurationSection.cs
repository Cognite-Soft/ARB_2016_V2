using System.Configuration;

namespace Cognite.Arb.Server.Resource.MailSender
{
    public class MailTemplatesConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("sendFrom", IsRequired = true)]
        public string SendMailNotificationsFrom
        {
            get { return (string) this["sendFrom"]; }
            set { this["sendFrom"] = value; }
        }

        [ConfigurationProperty("newUser", IsRequired = true)]
        public MailTemplate NewUserNotification
        {
            get { return (MailTemplate) this["newUser"]; }
            set { this["newUser"] = value; }
        }

        [ConfigurationProperty("resetPassword", IsRequired = true)]
        public MailTemplate ResetPasswordNotification
        {
            get { return (MailTemplate) this["resetPassword"]; }
            set { this["resetPassword"] = value; }
        }

        [ConfigurationProperty("resetPhrase", IsRequired = true)]
        public MailTemplate ResetSecurePhraseNotification
        {
            get { return (MailTemplate) this["resetPhrase"]; }
            set { this["resetPhrase"] = value; }
        }

        [ConfigurationProperty("newUserComplete", IsRequired = true)]
        public MailTemplate NewUserCompleteNotification
        {
            get { return (MailTemplate) this["newUserComplete"]; }
            set { this["newUserComplete"] = value; }
        }

        [ConfigurationProperty("resetPasswordComplete", IsRequired = true)]
        public MailTemplate ResetPasswordCompleteNotification
        {
            get { return (MailTemplate) this["resetPasswordComplete"]; }
            set { this["resetPasswordComplete"] = value; }
        }

        [ConfigurationProperty("resetPhraseComplete", IsRequired = true)]
        public MailTemplate ResetSecurePhraseCompleteNotification
        {
            get { return (MailTemplate) this["resetPhraseComplete"]; }
            set { this["resetPhraseComplete"] = value; }
        }

        [ConfigurationProperty("caseWorkerAssigned", IsRequired = true)]
        public MailTemplate CaseWorkerAssignedNotification
        {
            get { return (MailTemplate) this["caseWorkerAssigned"]; }
            set { this["caseWorkerAssigned"] = value; }
        }

        [ConfigurationProperty("assignmentUpdate", IsRequired = true)]
        public MailTemplate AssignmentUpdateNotification
        {
            get { return (MailTemplate) this["assignmentUpdate"]; }
            set { this["assignmentUpdate"] = value; }
        }

        [ConfigurationProperty("assignmentUpdateCreate", IsRequired = true)]
        public MailTemplate AssignmentUpdateNotificationCreate
        {
            get { return (MailTemplate)this["assignmentUpdateCreate"]; }
            set { this["assignmentUpdateCreate"] = value; }
        }

        [ConfigurationProperty("assignmentUpdateDelete", IsRequired = true)]
        public MailTemplate AssignmentUpdateNotificationDelete
        {
            get { return (MailTemplate)this["assignmentUpdateDelete"]; }
            set { this["assignmentUpdateDelete"] = value; }
        }

        [ConfigurationProperty("baseAppUrl", IsRequired = true)]
        public string BaseApplicationUrl
        {
            get { return (string)this["baseAppUrl"]; }
            set { this["baseAppUrl"] = value; }
        }

        public class MailTemplate : ConfigurationElement
        {
            [ConfigurationProperty("subject", IsRequired = true)]
            public string Subject
            {
                get { return (string) this["subject"]; }
                set { this["subject"] = value; }
            }


            [ConfigurationProperty("path", IsRequired = true)]
            public string Path
            {
                get { return (string) this["path"]; }
                set { this["path"] = value; }
            }

            [ConfigurationProperty("isHtml", IsRequired = true)]
            public bool IsHtml
            {
                get { return (bool) this["isHtml"]; }
                set { this["isHtml"] = value; }
            }
        }
    }
}