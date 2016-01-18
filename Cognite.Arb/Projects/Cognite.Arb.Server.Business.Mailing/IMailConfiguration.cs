namespace Cognite.Arb.Server.Business.Mailing
{
    public interface IMailConfiguration
    {
        string SendMailNotificationsFrom { get; }
        string BaseApplicationUrl { get; }
        MailTemplate NewUserNotificationTemplate { get; }
        MailTemplate ResetPasswordNotificationTemplate { get; }
        MailTemplate ResetSecurePhraseNotificationTemplate { get; }
        MailTemplate NewUserCompleteNotificationTemplate { get; }
        MailTemplate ResetPasswordCompleteNotificationTemplate { get; }
        MailTemplate ResetSecurePhraseCompleteNotificationTemplate { get; }
        MailTemplate CaseWorkerAssignedNotificationTemplate { get; set; }
        MailTemplate AssignmentUpdateNotificationTemplate { get; set; }
        MailTemplate AssignmentUpdateCreateNotificationTemplate { get; set; }
        MailTemplate AssignmentUpdateDeleteNotificationTemplate { get; set; }
    }
}
