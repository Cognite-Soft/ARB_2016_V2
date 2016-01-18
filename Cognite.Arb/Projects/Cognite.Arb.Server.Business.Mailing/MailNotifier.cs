namespace Cognite.Arb.Server.Business.Mailing
{
    public class MailNotifier : IMailNotifier
    {
        private readonly IMailSender _mailSender;
        private readonly MailGenerator _mailGenerator;

        public MailNotifier(IMailSender mailSender, IMailConfiguration mailConfiguration)
        {
            _mailSender = mailSender;
            _mailGenerator = new MailGenerator(mailConfiguration);
        }

        public void SendNewUserInstruction(ResetNotification notification)
        {
            var mail = _mailGenerator.CreateNewUserNotification(notification);
            _mailSender.SendMail(mail);
        }

        public void SendResetPasswordInstruction(ResetNotification notification)
        {
            var mail = _mailGenerator.CreateResetPasswordNotification(notification);
            _mailSender.SendMail(mail);
        }

        public void SendResetSecurePhraseInstruction(ResetNotification notification)
        {
            var mail = _mailGenerator.CreateResetSecurePhraseNotification(notification);
            _mailSender.SendMail(mail);
        }

        public void SendNewUserComplete(User user)
        {
            var mail = _mailGenerator.CreateNewUserCompleteNotification(user);
            _mailSender.SendMail(mail);
        }

        public void SendResetPasswordComplete(User user)
        {
            var mail = _mailGenerator.CreateResetPasswordCompleteNotification(user);
            _mailSender.SendMail(mail);
        }

        public void SendResetSecurePhraseComplete(User user)
        {
            var mail = _mailGenerator.CreateResetSecurePhraseCompleteNotification(user);
            _mailSender.SendMail(mail);
        }

        public void NotifyAssignedCaseWorker(int caseId, User caseWorker)
        {
            var mail = _mailGenerator.CreateAssignedCaseWorkerNotification(caseWorker, caseId);
            _mailSender.SendMail(mail);
        }

        public void NotifyAssignmentChange(User user, int[] deleted, int[] created)
        {
            var mail = _mailGenerator.CreateAssignmentUpdateNotification(user, deleted, created);
            _mailSender.SendMail(mail);
        }

        public void NotifyCaseWorkerAboutPreliminaryDecisionComment(User caseWorker, User commenter, int caseId, string comment)
        {
            var mail = _mailGenerator.CreateCommentNotification(caseWorker, commenter, caseId, comment);
            _mailSender.SendMail(mail);
        }

        public void NotifyCaseWorkerAboutFinalDecisionComment(User caseWorker, User commenter, int caseId, string comment)
        {
            var mail = _mailGenerator.CreateCommentNotification(caseWorker, commenter, caseId, comment);
            _mailSender.SendMail(mail);
        }
    }
}
