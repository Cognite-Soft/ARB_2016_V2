using System;
using Cognite.Arb.Server.Business.Cases;

namespace Cognite.Arb.Server.Business.Database
{
    public interface IDatabase
    {
        #region User Management, Authentication and Authorization

        UserData GetUserById(Guid id);
        UserData GetUserByEmail(string email);
        UserHeader[] GetActiveUsers();
        UserHeader[] GetAllUsers();
        void CreateUser(UserData user);
        void UpdateUser(UserData user);
        void DeleteUser(Guid email);
        void AddResetToken(ResetToken token);
        ResetToken GetResetToken(string token);
        void DeleteResetTokenIfExists(Guid userId, ResetTokenType type);
        void DeleteExpiredResetTokens();

        #endregion

        #region Cases

        CaseData GetCaseById(int id);
        void CreateCase(CaseData caseData);
        void UpdateCase(CaseData caseData);
        CaseHeader[] GetAllCaseHeaders();
        CaseHeader[] GetCaseHeadersByAssignedUserId(Guid id);
        UserHeader[] GetAssignedUsers(int caseId);

        #endregion

        void AssignUser(int caseId, Guid userId);
        bool DisassignUser(int caseId, Guid userId);
        int[] GetUserAssignments(Guid id);
        Schedule[] GetSchedules();
        Schedule GetSchedule(int row);
        void UpdateSchedule(Schedule schedule);
        int GetMaxDateAndDetailOrder(int caseId, DateTime date);
        void CreateDateAndDetail(int caseId, NewDateAndDetail item, Guid userId, int order);
        void DeleteAllegation(Guid id);
        void CreateAllegation(int caseId, NewAllegation item, Guid userId, int order);
        int? GetCaseIdByAllegationId(Guid allegationId);
        void CreateAllegationComment(Guid allegationId, string comment, AllegationCommentType type, string additionalComment, Guid id);
        ActivityFeed.ActivityFeedItem[] GetPreliminaryCommentsActivityFeedItems(int caseId);
        ActivityFeed.ActivityFeedItem[] GetPreliminaryDecisionActivityFeedItems(int caseId);
        ActivityFeed.ActivityFeedItem[] GetFinalDecisionActivityFeedItems(int caseId);
        void CreatePreliminaryDecisionComment(PreliminaryDecisionComment comment);
        void CreateFinalDecisionComment(FinalDecisionComment comment);
        PreliminaryDecisionComment[] GetPreliminaryDecisionComments(int caseId);
        FinalDecisionComment[] GetFinalDecisionComments(int caseId);
        AllegationEx[] GetAllegationComments(int caseId, Guid userId);
        void CreatePost(Guid postId, int caseId, DateTime date, Guid userId, string text);
        void ReplyPost(Guid postId, int caseId, Guid replyOnId, DateTime date, Guid userId, string text);
        Post[] GetPosts(int caseId);
        void CreateMessage(Message message);
        Message GetMessageById(Guid id);
        void SetMessageAcceptedDate(Guid messageId, DateTime accepted);
        MessageEx[] GetNotAcceptedMessagesByToUserId(Guid toUserId);
        Document[] GetAllegationDocuments(Guid allegationId);
        void AddPartyComment(NewPartiesComment comment, DateTime now);
        PartiesComment[] GetPartiesComments(int caseId);
        void AddPreliminaryDecisionDocument(int caseId, Guid id, string name);
        void AddFinalDecisionDocument(int caseId, Guid id, string name);
        Document[] GetAllDocuments(int caseId);
        Document GetDocumentById(Guid documentId);
        int[] GetOpenCasesOlderThan(DateTime minimumStartDate);
        void SetClosedState(int[] caseIds);
        void SetCaseStartDate(int caseId, DateTime date);
        Document GetFinalDecisionDocument(int caseId);
        Document[] GetDateAndDetailDocument(Guid dateAndDetailId);
        void DeleteDateAndDetail(Guid id);
        Document GetPreliminaryDecisionDocument(int caseId);
        bool GetIsLooseUser(Guid id);
        void AddDocumentActivity(DocumentActivity documentActivity);
        DocumentActivity[] GetDocumentActivities(int caseId);
    }
}
