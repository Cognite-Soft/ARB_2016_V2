namespace Cognite.Arb.Server.Business.Mailing
{
    public interface IMailNotifier
    {
        void SendNewUserInstruction(ResetNotification notification);
        void SendResetPasswordInstruction(ResetNotification notification);
        void SendResetSecurePhraseInstruction(ResetNotification notification);
        void SendNewUserComplete(User user);
        void SendResetPasswordComplete(User user);
        void SendResetSecurePhraseComplete(User user);
        void NotifyAssignedCaseWorker(int caseId, User caseWorker);
        void NotifyAssignmentChange(User user, int[] deleted, int[] created);
        void NotifyCaseWorkerAboutPreliminaryDecisionComment(User caseWorker, User commenter, int caseId, string comment);
        void NotifyCaseWorkerAboutFinalDecisionComment(User caseWorker, User commenter, int caseId, string comment);
    }
}
