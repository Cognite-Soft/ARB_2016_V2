using Cognite.Arb.Server.Contract;
using Cognite.Arb.Server.Contract.Cases;
using Cognite.Arb.Web.Models.Complaints;
using System;
using System.Collections.Generic;

namespace Cognite.Arb.Web.Core.Mappers
{
    internal static partial class Mappers
    {
        internal static ComplaintCommentsViewModel MapComments(ComplaintComments comments)
        {
            var result = new ComplaintCommentsViewModel();

            result.PreliminaryComments = Mappers.MapPreliminaryComments(comments.PreliminaryComments);
            result.PreliminaryDecision = Mappers.MapPreliminaryDecision(comments.PreliminaryDecisionComments);
            result.FinalDecision = Mappers.MapFinalDecision(comments.FinalDecisionApprovedByCurrentUser);
            result.FinalDecision.Document = Mappers.MapDocument(comments.FinalDecisionDocument);
            SetStatuses(comments.DueDate, comments.DueDaysLeft, comments.Status, result);

            return result;
        }

        private static ComplaintCommentsPreliminaryCommentsViewModel MapPreliminaryComments(ComplaintComments.PreliminaryCommentsData preliminaryComments)
        {
            var result = new ComplaintCommentsPreliminaryCommentsViewModel();

            result.AllegationsWithComments = Mappers.MapAllegationsWithComments(preliminaryComments.AllegationsWithComments);
            result.AllegationsWithMyComments = Mappers.MapAllegationsWithMyComments(preliminaryComments.AllegationsWithMyComments);

            return result;
        }

        private static List<AllegationWithComments> MapAllegationsWithComments(ComplaintComments.AllegationWithComments[] allegationWithComments)
        {
            var result = new List<AllegationWithComments>();

            foreach (var allegation in allegationWithComments)
                result.Add(Mappers.MapAllegationWithComments(allegation));

            return result;
        }

        private static AllegationWithComments MapAllegationWithComments(ComplaintComments.AllegationWithComments allegation)
        {
            var result = new AllegationWithComments();

            result.Id = allegation.Id;
            result.Text = allegation.Text;
            result.Documents = Mappers.MapDocuments(allegation.Documents);
            result.Comments = Mappers.MapMiniComments(allegation.Comments);

            return result;
        }

        private static List<AllegationMiniComment> MapMiniComments(ComplaintComments.AllegationComment[] allegationComment)
        {
            var result = new List<AllegationMiniComment>();

            foreach (var comment in allegationComment)
                result.Add(Mappers.MapMiniComment(comment));
            
            return result;
        }

        private static AllegationMiniComment MapMiniComment(ComplaintComments.AllegationComment comment)
        {
            var result = new AllegationMiniComment();

            result.Id = comment.Id;
            result.Text = comment.Text;
            result.User = Mappers.MapUserToUserViewModel(comment.User);

            return result;
        }

        private static List<AllegationWithMyComment> MapAllegationsWithMyComments(ComplaintComments.AllegationWithMyComment[] allegationWithMyComment)
        {
            var result = new List<AllegationWithMyComment>();

            foreach (var allegation in allegationWithMyComment)
                result.Add(Mappers.MapAllegationsWithMyComment(allegation));

            return result;
        }

        private static AllegationWithMyComment MapAllegationsWithMyComment(ComplaintComments.AllegationWithMyComment allegation)
        {
            var result = new AllegationWithMyComment();

            result.Id = allegation.Id;
            result.Text = allegation.Text;
            result.Documents = Mappers.MapDocuments(allegation.Documents);
            result.Comment = Mappers.MapMyAllegationComment(allegation.MyComment);
            if (allegation.OtherComments != null)
                result.OtherComments = Mappers.MapMiniComments(allegation.OtherComments);

            return result;
        }

        private static ComplaintCommentsPreliminaryDecisionViewModel MapPreliminaryDecision(ComplaintComments.PreliminaryDecisionCommentsData preliminaryDecision)
        {
            var result = new ComplaintCommentsPreliminaryDecisionViewModel();

            result.PreliminaryDecisionDocument = Mappers.MapDocument(preliminaryDecision.PreliminaryDecisionDocument);
            result.Comments = Mappers.MapPreliminaryDecisionComments(preliminaryDecision.Comments);
            result.CommentsFromParties = Mappers.MapPreliminaryDecisionCommentsFromParties(preliminaryDecision.CommentsFromParies);

            return result;
        }

        private static List<PreliminaryDecisionComment> MapPreliminaryDecisionComments(ComplaintComments.AllegationComment[] allegationComment)
        {
            var result = new List<PreliminaryDecisionComment>();

            foreach (var comment in allegationComment)
                result.Add(Mappers.MapAllegationPreliminaryDecisionComment(comment));

            return result;
        }

        private static PreliminaryDecisionComment MapAllegationPreliminaryDecisionComment(ComplaintComments.AllegationComment comment)
        {
            var result = new PreliminaryDecisionComment();

            result.Text = comment.Text;
            result.User = Mappers.MapUserToUserViewModel(comment.User);

            return result;
        }

        private static List<CommentFromParties> MapPreliminaryDecisionCommentsFromParties(ComplaintComments.CommentFromParties[] commentFromParties)
        {
            var result = new List<CommentFromParties>();

            foreach (var comment in commentFromParties)
                result.Add(Mappers.MapPreliminaryDecisionCommentFromParties(comment));

            return result;
        }

        private static CommentFromParties MapPreliminaryDecisionCommentFromParties(ComplaintComments.CommentFromParties comment)
        {
            var result = new CommentFromParties();

            result.Title = comment.Text;
            result.Documents = Mappers.MapDocuments(comment.Documents);

            return result;
        }

        private static ComplaintCommentsFinalDecisionViewModel MapFinalDecision(bool? approved)
        {
            var result = new ComplaintCommentsFinalDecisionViewModel();

            result.ApprovedByCurrentUser = approved ?? false;

            return result;
        }

        private static void SetStatuses(DateTime? dueDate, int? dueDays, ComplaintComments.StatusKind status, ComplaintCommentsViewModel result)
        {
            var date = dueDate ?? DateTime.Now;
            var days = dueDays ?? 0;

            result.PreliminaryComments.DueDate = date;
            result.PreliminaryComments.DueDays = days;
            result.PreliminaryDecision.DueDate = date;
            result.PreliminaryDecision.DueDays = days;
            result.FinalDecision.DueDate = date;
            result.FinalDecision.DueDays = days;

            switch (status)
            {
                case ComplaintComments.StatusKind.PreliminaryComments:
                    SetRender(result, true, false, false);
                    SetState(result, ComplaintCommentsStepState.InProcess, ComplaintCommentsStepState.InProcess, ComplaintCommentsStepState.InProcess);
                    break;
                case ComplaintComments.StatusKind.PreliminaryDecisionComments:
                    SetRender(result, true, true, false);
                    SetState(result, ComplaintCommentsStepState.Complete, ComplaintCommentsStepState.InProcess, ComplaintCommentsStepState.InProcess);
                    break;
                case ComplaintComments.StatusKind.PreliminaryDecisionWaiting:
                    SetRender(result, true, true, false);
                    SetState(result, ComplaintCommentsStepState.Complete, ComplaintCommentsStepState.WaitingForPartiesComments, ComplaintCommentsStepState.InProcess);
                    break;
                case ComplaintComments.StatusKind.FinalDecisionComments:
                    SetRender(result, true, true, true);
                    SetState(result, ComplaintCommentsStepState.Complete, ComplaintCommentsStepState.Complete, ComplaintCommentsStepState.InProcess);
                    break;
                case ComplaintComments.StatusKind.FinalDecisionLocked:
                    SetRender(result, true, true, true);
                    SetState(result, ComplaintCommentsStepState.Complete, ComplaintCommentsStepState.Complete, ComplaintCommentsStepState.Locked);
                    break;
                case ComplaintComments.StatusKind.Complete:
                    SetRender(result, true, true, true);
                    SetState(result, ComplaintCommentsStepState.Complete, ComplaintCommentsStepState.Complete, ComplaintCommentsStepState.Complete);
                    break;
            }
        }

        private static void SetRender(ComplaintCommentsViewModel result, bool renderPrelimComments, bool renderPrelimDecision, bool renderFinalDecision)
        {
            result.RenderPreliminaryComments = renderPrelimComments;
            result.RenderPreliminaryDecision = renderPrelimDecision;
            result.RenderFinalDecision = renderFinalDecision;
        }

        private static void SetState(ComplaintCommentsViewModel result, ComplaintCommentsStepState prelimCommentsState, ComplaintCommentsStepState prelimDecisionState, ComplaintCommentsStepState finalDecisionState)
        {
            result.PreliminaryComments.State = prelimCommentsState;
            result.PreliminaryDecision.State = prelimDecisionState;
            result.FinalDecision.State = finalDecisionState;
        }
    }
}