using System;

namespace Cognite.Arb.Server.Business.Mailing
{
    public class MailGenerator
    {
        private readonly IMailConfiguration _mailConfiguration;

        public MailGenerator(IMailConfiguration mailConfiguration)
        {
            _mailConfiguration = mailConfiguration;
        }

        public IndividualMail CreateNewUserNotification(ResetNotification notification)
        {
            return CreateResetNotification(notification, () => _mailConfiguration.NewUserNotificationTemplate);
        }

        public IndividualMail CreateResetPasswordNotification(ResetNotification notification)
        {
            return CreateResetNotification(notification, () => _mailConfiguration.ResetPasswordNotificationTemplate);
        }

        public IndividualMail CreateResetSecurePhraseNotification(ResetNotification notification)
        {
            return CreateResetNotification(notification, () => _mailConfiguration.ResetSecurePhraseNotificationTemplate);
        }

        public IndividualMail CreateNewUserCompleteNotification(User user)
        {
            return CreateResetCompleteNotification(user, () => _mailConfiguration.NewUserCompleteNotificationTemplate);
        }

        public IndividualMail CreateResetPasswordCompleteNotification(User user)
        {
            return CreateResetCompleteNotification(user, () => _mailConfiguration.ResetPasswordCompleteNotificationTemplate);
        }

        public IndividualMail CreateResetSecurePhraseCompleteNotification(User user)
        {
            return CreateResetCompleteNotification(user, () => _mailConfiguration.ResetSecurePhraseCompleteNotificationTemplate);
        }

        public IndividualMail CreateAssignedCaseWorkerNotification(User caseWorker, int caseId)
        {
            var template = _mailConfiguration.CaseWorkerAssignedNotificationTemplate;
            return new IndividualMail
            {
                From = _mailConfiguration.SendMailNotificationsFrom,
                To = caseWorker.Email,
                Subject = template.Subject,
                IsHtml = template.IsHtml,
                Body = string.Format(template.BodyTemplate, caseWorker.FirstName, caseWorker.LastName, caseId),
            };
        }

        public IndividualMail CreateAssignmentUpdateNotification(User user, int[] deleted, int[] created)
        {
            var deletedList = string.Join(", ", deleted);
            var createdList = string.Join(", ", created);

            if (deleted.Length == 0)
                return CreateAssignmentUpdateNotificationCreated(user, createdList);

            if (created.Length == 0)
                return CreateAssignmentUpdateNotificationDeleted(user, deletedList);

            return CreateAssignmentUpdateNotificationBoth(user, deletedList, createdList);
        }

        private IndividualMail CreateAssignmentUpdateNotificationCreated(User user, string createdList)
        {
            var template = _mailConfiguration.AssignmentUpdateCreateNotificationTemplate;
            return new IndividualMail
            {
                From = _mailConfiguration.SendMailNotificationsFrom,
                To = user.Email,
                Subject = template.Subject,
                IsHtml = template.IsHtml,
                Body = string.Format(template.BodyTemplate, user.FirstName, user.LastName, createdList),
            };
        }

        private IndividualMail CreateAssignmentUpdateNotificationDeleted(User user, string deletedList)
        {
            var template = _mailConfiguration.AssignmentUpdateDeleteNotificationTemplate;
            return new IndividualMail
            {
                From = _mailConfiguration.SendMailNotificationsFrom,
                To = user.Email,
                Subject = template.Subject,
                IsHtml = template.IsHtml,
                Body = string.Format(template.BodyTemplate, user.FirstName, user.LastName, deletedList),
            };
        }

        private IndividualMail CreateAssignmentUpdateNotificationBoth(User user, string deletedList, string createdList)
        {
            var template = _mailConfiguration.AssignmentUpdateNotificationTemplate;
            return new IndividualMail
            {
                From = _mailConfiguration.SendMailNotificationsFrom,
                To = user.Email,
                Subject = template.Subject,
                IsHtml = template.IsHtml,
                Body = string.Format(template.BodyTemplate, user.FirstName, user.LastName, deletedList, createdList),
            };
        }
        
        private IndividualMail CreateResetNotification(ResetNotification notification, Func<MailTemplate> getTemplate)
        {
            var template = getTemplate();
            return new IndividualMail
            {
                From = _mailConfiguration.SendMailNotificationsFrom,
                To = notification.UserEmail,
                Subject = template.Subject,
                Body = template.ApplyTemplate(notification.UserId, notification.UserEmail, notification.UserFirstName,
                    notification.UserLastName, notification.ExpirationTime, notification.ResetToken,
                    _mailConfiguration.BaseApplicationUrl),
                IsHtml = template.IsHtml,
            };
        }

        private IndividualMail CreateResetCompleteNotification(User user, Func<MailTemplate> getTemplate)
        {
            var template = getTemplate();
            return new IndividualMail
            {
                From = _mailConfiguration.SendMailNotificationsFrom,
                To = user.Email,
                Subject = template.Subject,
                Body = template.ApplyTemplate(user.UserId, user.Email, user.FirstName, user.LastName),
                IsHtml = template.IsHtml,
            };
        }

        public IndividualMail CreateCommentNotification(User caseWorker, User commenter, int caseId, string comment)
        {
            // TODO: implement
            return new IndividualMail
            {
                From = _mailConfiguration.SendMailNotificationsFrom,
                To = caseWorker.Email,
                Subject = "Case Commented",
                Body = string.Format("Case {0} has been commented by {1}: {2}",
                    caseId, commenter.FirstName + " " + commenter.LastName, comment),
                IsHtml = false,
            };
        }
    }
}