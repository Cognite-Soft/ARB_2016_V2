using System;
using Cognite.Arb.Server.Contract;
using Cognite.Arb.Server.Contract.Cases;
using System.Collections.Generic;

namespace Cognite.Arb.Web.ServiceClient
{
    public interface IServiceClient
    {
        #region Login

        SecurePhraseQuestion StartLoginAndGetSecurePhraseQuestion(string email, string password);
        AuthenticationResult FinishLoginWithSecurePhraseAnswer(string token, SecurePhraseAnswer answer);
        User GetUserBySecurityToken(string securityToken);
        User GetUser(string securityToken, Guid id);
        void Logout(string securityToken);
        bool IsPasswordStrengthPassed(string password);

        #endregion

        #region Activation & Reset
        
        void InitiateResetPassword(string email);
        void InitiateResetSecurePhrase(string securityToken, string email);
        void FinishResetPassword(string resetToken, string password);
        void FinishResetSecurePhrase(string resetToken, string securePhrase);
        void FinishActivateUser(string resetToken, string password, string securePhrase);
        ResetToken ValidateResetToken(string resetToken);

        #endregion

        #region User Management

        User[] GetUsers(string securityToken);
        User[] GetPanelMembers(string securityToken);
        User CreateUser(string securityToken, User user);
        void UpdateUser(string securityToken, User user);
        void DeleteUser(string securityToken, Guid id);

        #endregion

        #region Cases

        CaseHeader[] GetCases(string securityToken);
        CaseHeader[] GetRejectedCases(string securityToken);
        CaseHeader[] GetClosedCases(string securityToken);
        CaseHeader[] GetActiveCases(string securityToken);
        Case GetCase(string securityToken, int id);
        void UpdateCase(string securityToken, int caseId, CaseUpdate caseUpdate);
        void StartCaseProcessing(string securityToken, int id); // only new case can be started

        #endregion

        #region Case assignment

        Schedule[] GetSchedule(string securityToken);
        void UpdateScheduleCell(string securityToken, int id, int colIndex, Guid userId);
        void UpdateUserAssigments(string securityToken, Guid userId, params int[] caseIdCollection);
        int[] GetAssignedCases(string securityToken, Guid userId);

        #endregion

        #region Notifications

        void SendNotification(string securityToken, CreateNotification notification);
        Notification[] GetNotifications(string securityToken);
        void DismissNotification(string securityToken, Guid notificationId);
        void SendNotificationAll(string securityToken, CreateNotificationAll notification);
                
        #endregion

        #region Case Workflow

        void CommentAllegation(string securityToken, Guid allegationId, string comment, AllegationCommentType type, string additionalComment);
        ActivityFeed GetActivityFeed(string securityToken, int caseId);

        #endregion


        void CloseCase(string securityToken, int caseId);
        void ReopenCase(string securityToken, int caseId);
        void CloneCase(string securityToken, int caseId);
        ComplaintComments GetComments(string securityToken, int caseId);
        void AddPreliminaryDecisionComment(string securityToken, int caseId, string text);
        void AddFinalDecisionComment(string securityToken, int caseId, FinalDecisionType finalDecisionType, string text);
        Post[] GetDiscussions(string securityToken, int caseId);
        void CreatePost(string securityToken, int caseId, Guid postId, string text);
        void ReplyOnPost(string securityToken, int caseId, Guid postId, Guid replyId, string text);

        void AddPartiesComment(string securityToken, int caseId, string text, NewDocument[] documents);

        void AddPreliminaryDecision(string securityToken, int caseId, NewDocument newDocument);

        void AddUpdatedPreliminaryDecision(string securityToken, int caseId, NewDocument newDocument);

        void AddFinalDecision(string p, int id, NewDocument newDocument);

        void AddUpdatedFinalDecision(string securityToken, int caseId, NewDocument newDocument);

        NewDocument GetDocument(string securityToken, Guid documentId);

        Document[] GetCaseDocuments(string securityToken, int caseId);

        //prabhakar
        List<User> GetCaseWorkerforaCase(string securityToken, int caseid);
    }
}
