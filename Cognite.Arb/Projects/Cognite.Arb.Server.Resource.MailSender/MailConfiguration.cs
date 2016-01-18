using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using Cognite.Arb.Server.Business.Mailing;

namespace Cognite.Arb.Server.Resource.MailSender
{
    public class MailConfiguration : IMailConfiguration
    {
        public MailConfiguration()
        {
            var mailTemplates = (MailTemplatesConfigurationSection) ConfigurationManager.GetSection("mailTemplates");
            SendMailNotificationsFrom = mailTemplates.SendMailNotificationsFrom;
            BaseApplicationUrl = mailTemplates.BaseApplicationUrl;

            NewUserNotificationTemplate = CreateTemplate(mailTemplates.NewUserNotification);
            ResetPasswordNotificationTemplate = CreateTemplate(mailTemplates.ResetPasswordNotification);
            ResetSecurePhraseNotificationTemplate = CreateTemplate(mailTemplates.ResetSecurePhraseNotification);
            NewUserCompleteNotificationTemplate = CreateTemplate(mailTemplates.NewUserCompleteNotification);
            ResetPasswordCompleteNotificationTemplate = CreateTemplate(mailTemplates.ResetPasswordCompleteNotification);
            ResetSecurePhraseCompleteNotificationTemplate = CreateTemplate(mailTemplates.ResetSecurePhraseCompleteNotification);
            CaseWorkerAssignedNotificationTemplate = CreateTemplate(mailTemplates.CaseWorkerAssignedNotification);
            AssignmentUpdateNotificationTemplate = CreateTemplate(mailTemplates.AssignmentUpdateNotification);
            AssignmentUpdateCreateNotificationTemplate = CreateTemplate(mailTemplates.AssignmentUpdateNotificationCreate);
            AssignmentUpdateDeleteNotificationTemplate = CreateTemplate(mailTemplates.AssignmentUpdateNotificationDelete);
        }

        private MailTemplate CreateTemplate(MailTemplatesConfigurationSection.MailTemplate template)
        {
            return new MailTemplate
            {
                Subject = template.Subject,
                BodyTemplate = Load(template.Path),
                IsHtml = template.IsHtml,
            };
        }

        private string Load(string path)
        {
            var fullPath = GetFullPath(path);
            return File.ReadAllText(fullPath);
        }

        private static string GetFullPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;
            if (Path.IsPathRooted(path)) return path;
            return MakeRelativePathFull(path);
        }

        private static string MakeRelativePathFull(string path)
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            string unescapedUri = Uri.UnescapeDataString(uri.Path);
            string directoryName = Path.GetDirectoryName(unescapedUri);
            var result = Path.Combine(directoryName, path);
            return result;
        }

        public string SendMailNotificationsFrom { get; private set; }
        public string BaseApplicationUrl { get; private set; }
        public MailTemplate NewUserNotificationTemplate { get; private set; }
        public MailTemplate ResetPasswordNotificationTemplate { get; private set; }
        public MailTemplate ResetSecurePhraseNotificationTemplate { get; private set; }
        public MailTemplate NewUserCompleteNotificationTemplate { get; private set; }
        public MailTemplate ResetPasswordCompleteNotificationTemplate { get; private set; }
        public MailTemplate ResetSecurePhraseCompleteNotificationTemplate { get; private set; }
        public MailTemplate CaseWorkerAssignedNotificationTemplate { get; set; }
        public MailTemplate AssignmentUpdateNotificationTemplate { get; set; }
        public MailTemplate AssignmentUpdateCreateNotificationTemplate { get; set; }
        public MailTemplate AssignmentUpdateDeleteNotificationTemplate { get; set; }
    }
}
