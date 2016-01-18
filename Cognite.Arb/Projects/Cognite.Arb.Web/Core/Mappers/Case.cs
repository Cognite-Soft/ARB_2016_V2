using Cognite.Arb.Server.Contract;
using Cognite.Arb.Web.Models.Additional;
using Cognite.Arb.Web.Models.Complaints;
using System;
using System.Collections.Generic;

using Case = Cognite.Arb.Server.Contract.Cases.Case;
using CaseState = Cognite.Arb.Server.Contract.Cases.CaseState;
using CasePanelMembers = Cognite.Arb.Server.Contract.Cases.CasePanelMembers;
using CaseQuestion = Cognite.Arb.Server.Contract.Cases.Question;
using CaseDateAndDetail = Cognite.Arb.Server.Contract.Cases.DateAndDetail;
using CaseDocument = Cognite.Arb.Server.Contract.Cases.Document;
using CaseAllegationCollection = Cognite.Arb.Server.Contract.Cases.AllegationCollection;
using CaseAllegation = Cognite.Arb.Server.Contract.Cases.Allegation;
using CaseMyAllegationComment = Cognite.Arb.Server.Contract.Cases.MyAllegationComment;
using CaseContact = Cognite.Arb.Server.Contract.Cases.Contact;
using CaseArchitectContact = Cognite.Arb.Server.Contract.Cases.ArchitectContact;

namespace Cognite.Arb.Web.Core.Mappers
{
    internal static partial class Mappers
    {
        internal static List<ComplaintListItemViewModel> MapCasesToComplaintsListItem(CaseHeader[] cases)
        {
            var result = new List<ComplaintListItemViewModel>();

            foreach (var cs in cases)
                result.Add(Mappers.MapCaseToComplaintsListItem(cs));

            return result;
        }

        internal static ComplaintListItemViewModel MapCaseToComplaintsListItem(CaseHeader currentCase)
        {
            var result = new ComplaintListItemViewModel()
            {
                Id = currentCase.Id,
                CaseId = currentCase.Id,
                RegistrationId = currentCase.RegistrationNumber ?? 0,
                Complainant = currentCase.Complainant,
                Architect = currentCase.Architect,
            };

            return result;
        }

        internal static ComplaintOverviewViewModel MapCaseToComplaintDetails(Case curCase)
        {
            var result = new ComplaintOverviewViewModel();

            result.Id = curCase.ReadonlyData.Id;
            result.ParentId = curCase.ReadonlyData.ParentId ?? 0;
            result.State = Mappers.MapComplaintState(curCase.ReadonlyData.State);
            result.IsReady = curCase.InitialData.IsReady;
            result.PanelMembers = Mappers.MapPanelMembers(curCase.InitialData.CasePanelMembers);
            result.StartDate = curCase.InitialData.StartDate;
            result.Background = curCase.InitialData.Background;
            result.IdealOutcome = curCase.InitialData.IdealOutcome;
            result.IssueRaisedWithArchitect = Mappers.MapQuestion(curCase.InitialData.IssueRaisedWithArchitect);
            result.SubjectOfLegalProceedings = Mappers.MapQuestion(curCase.InitialData.SubjectOfLegalProceedings);
            result.ComplaintCanBeEdited = curCase.InitialData.CanBeEdited;
            result.DatesAndDetails = Mappers.MapDatesAndDetails(curCase.DatesAndDetails);
            result.Allegations = Mappers.MapAllegations(curCase.Allegations);
            result.Relationship = curCase.Contacts.Relationship;
            result.ContactAgreement = curCase.Contacts.ContactAgreement ?? false;
            result.ClaimantContact = Mappers.MapClaimantContact(curCase.Contacts.ClaimantContact);
            result.ArchitectContact = Mappers.MapArchitectContact(curCase.Contacts.ArchitectContact);

            return result;
        }

        private static ComplaintState MapComplaintState(CaseState caseState)
        {
            var result = new ComplaintState();

            if (caseState != null)
            {
                result.Type = caseState.StateKind;
                result.DueDate = caseState.DueByDate;
                result.DaysLeft = caseState.DueDaysLeft;
            }

            return result;
        }

        private static ComplaintPanelMembers MapPanelMembers(CasePanelMembers casePanelMembers)
        {
            var result = new ComplaintPanelMembers();

            if (casePanelMembers != null)
            {
                result.Member1 = Mappers.MapUserToUserViewModel(casePanelMembers.PanelMember1 ?? new User());
                result.Member2 = Mappers.MapUserToUserViewModel(casePanelMembers.PanelMember2 ?? new User());
                result.Member3 = Mappers.MapUserToUserViewModel(casePanelMembers.PanelMember3 ?? new User());
            }

            return result;
        }

        private static Question MapQuestion(CaseQuestion question)
        {
            var result = new Question();

            if (question != null)
            {
                result.Answer = question.Answer ?? false;
                result.Comments = question.Comments;
            }

            return result;
        }

        private static List<DateAndDetail> MapDatesAndDetails(CaseDateAndDetail[] dateAndDetails)
        {
            var result = new List<DateAndDetail>();

            if (dateAndDetails != null)
            {
                foreach (var dateAndDetail in dateAndDetails)
                    result.Add(Mappers.MapDateAndDetail(dateAndDetail));
            }

            return result;
        }

        private static DateAndDetail MapDateAndDetail(CaseDateAndDetail dateAndDetail)
        {
            return new DateAndDetail()
            {
                Id = dateAndDetail.Id,
                Date = dateAndDetail.Date,
                Text = dateAndDetail.Text,
                Documents = Mappers.MapDocuments(dateAndDetail.Documents),
            };
        }

        public static List<Document> MapDocuments(CaseDocument[] documents)
        {
            var result = new List<Document>();

            if (documents != null)
            {
                foreach (var document in documents)
                    result.Add(Mappers.MapDocument(document));
            }

            return result;
        }

        private static Document MapDocument(CaseDocument document)
        {
            if (document == null)
                return null;

            return new Document()
            {
                Id = document.Id,
                Name = document.Name,
            };
        }

        private static AllegationsCollection MapAllegations(CaseAllegationCollection allegationCollection)
        {
            var result = new AllegationsCollection();

            if (allegationCollection != null)
            {
                result.CanAddAlligation = allegationCollection.CanAddAllegation;
                result.Items = Mappers.MapAllegations(allegationCollection.Items);
            }

            return result;
        }

        private static List<AllegationWithMyComment> MapAllegations(CaseAllegation[] allegations)
        {
            var result = new List<AllegationWithMyComment>();

            if (allegations != null)
            {
                foreach (var allegation in allegations)
                    result.Add(Mappers.MapAllegation(allegation));
            }

            return result;
        }

        private static AllegationWithMyComment MapAllegation(CaseAllegation allegation)
        {
            return new AllegationWithMyComment()
            {
                Id = allegation.Id,
                Text = allegation.Text,
                Documents = Mappers.MapDocuments(allegation.Documents),
                CanBeDeleted = allegation.CanBeDeleted,
                Comment = Mappers.MapMyAllegationComment(allegation.MyComment),
            };
        }

        private static MyAllegationComment MapMyAllegationComment(CaseMyAllegationComment myAllegationComment)
        {
            var result = new MyAllegationComment();

            if (myAllegationComment != null)
            {
                result.Id = myAllegationComment.Id;
                result.AllegationCommentType = myAllegationComment.AllegationCommentType;
                result.Text = myAllegationComment.Text;
                result.AdditionalText = myAllegationComment.AdditionalText;
            }

            return result;
        }

        private static Contact MapClaimantContact(CaseContact contact)
        {
            var result = new Contact();

            if (contact != null)
            {
                result.Address = contact.Address;
                result.EMail = contact.EMail;
                result.Name = contact.Name;
                result.Phone = contact.Phone;
            }

            return result;
        }

        private static ArchitectContact MapArchitectContact(CaseArchitectContact contact)
        {
            var result = new ArchitectContact();

            if (contact != null)
            {
                result.Address = contact.Address;
                result.EMail = contact.EMail;
                result.Name = contact.Name;
                result.Phone = contact.Phone;
                result.RegistrationNumber = contact.RegistrationNumber ?? 0;
            }

            return result;
        }

        internal static IEnumerable<IntStringModel> MapCasesToIntStringModel(CaseHeader[] cases)
        {
            var result = new List<IntStringModel>();

            foreach (var cs in cases)
                result.Add(Mappers.MapCase(cs));

            return result;
        }

        internal static IntStringModel MapCase(CaseHeader cs)
        {
            return new IntStringModel()
            {
                Id = cs.Id,
                Value = String.Format("#{0}", cs.Id),
            };
        }
    }
}