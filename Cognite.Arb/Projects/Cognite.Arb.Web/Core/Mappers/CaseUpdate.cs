using Cognite.Arb.Web.Models.Complaints;
using System;
using System.Collections.Generic;
using System.Linq;

using CaseQuestion = Cognite.Arb.Server.Contract.Cases.Question;
using CaseNewDateAndDetail = Cognite.Arb.Server.Contract.Cases.NewDateAndDetail;
using CaseContact = Cognite.Arb.Server.Contract.Cases.Contact;
using CaseArchitectContact = Cognite.Arb.Server.Contract.Cases.ArchitectContact;
using CaseUpdate = Cognite.Arb.Server.Contract.Cases.CaseUpdate;
using CaseInitialCaseDataUpdate = Cognite.Arb.Server.Contract.Cases.InitialCaseDataUpdate;
using CaseContacts = Cognite.Arb.Server.Contract.Cases.CaseContacts;
using CaseAllegationsUpdate = Cognite.Arb.Server.Contract.Cases.AllegationsUpdate;
using CaseDateAndDetailUpdate = Cognite.Arb.Server.Contract.Cases.DateAndDetailUpdate;
using CasePanelMembersUpdate = Cognite.Arb.Server.Contract.Cases.PanelMembersUpdate;
using CaseNewAllegation = Cognite.Arb.Server.Contract.Cases.NewAllegation;
using CaseNewDocument = Cognite.Arb.Server.Contract.Cases.NewDocument;

namespace Cognite.Arb.Web.Core.Mappers
{
    internal static partial class Mappers
    {
        internal static CaseUpdate MapCaseToUpdateCase(ComplaintOverviewViewModel model, List<Guid> allegationsToDelete, List<Guid> dateAndDetailsToDelete)
        {
            var result = new CaseUpdate();

            result.Initial = GetInitialCaseDataUpdate(model);
            result.Contacts = GetConstactsCaseDataUpdate(model);
            //result.NewDatesAndDetails = GetNewDatesAndDetails(model.DatesAndDetails);
            result.AllegationsUpdate = GetAllegationsUpdate(model.Allegations.Items, allegationsToDelete);
            result.DatesAndDetailsUpdate = GetDateAndDetailsUpdate(model.DatesAndDetails, dateAndDetailsToDelete);

            return result;
        }

        private static CaseInitialCaseDataUpdate GetInitialCaseDataUpdate(ComplaintOverviewViewModel model)
        {
            var result = new CaseInitialCaseDataUpdate();

            result.StartDate = model.StartDate;
            result.Background = model.Background;
            result.IdealOutcome = model.IdealOutcome;
            result.PanelMembers = GetPanelMemebers(model.PanelMembers);
            result.IssueRaisedWithArchitect = GetQuestion(model.IssueRaisedWithArchitect);
            result.SubjectOfLegalProceedings = GetQuestion(model.SubjectOfLegalProceedings);

            return result;
        }

        private static CaseQuestion GetQuestion(Question question)
        {
            return new CaseQuestion()
            {
                Answer = question.Answer,
                Comments = question.Comments,
            };
        }

        private static CasePanelMembersUpdate GetPanelMemebers(ComplaintPanelMembers complaintPanelMembers)
        {
            return new CasePanelMembersUpdate()
            {
                PanelMember1 = complaintPanelMembers.Member1.Id,
                PanelMember2 = complaintPanelMembers.Member2.Id,
                PanelMember3 = complaintPanelMembers.Member3.Id,
            };
        }

        private static CaseContacts GetConstactsCaseDataUpdate(ComplaintOverviewViewModel model)
        {
            var result = new CaseContacts();

            result.Relationship = model.Relationship;
            result.ContactAgreement = model.ContactAgreement;
            result.ClaimantContact = GetClaimantContact(model.ClaimantContact);
            result.ArchitectContact = GetArchitectContact(model.ArchitectContact);

            return result;
        }

        private static CaseContact GetClaimantContact(Contact contact)
        {
            return new CaseContact()
            {
                Address = contact.Address,
                EMail = contact.EMail,
                Name = contact.Name,
                Phone = contact.Phone,
            };
        }

        private static CaseArchitectContact GetArchitectContact(ArchitectContact contact)
        {
            return new CaseArchitectContact()
            {
                Address = contact.Address,
                EMail = contact.EMail,
                Name = contact.Name,
                Phone = contact.Phone,
                RegistrationNumber = contact.RegistrationNumber,
            };
        }

        //private static CaseNewDateAndDetail[] GetNewDatesAndDetails(List<DateAndDetail> datesAndDetails)
        //{
        //    var result = new List<CaseNewDateAndDetail>();

        //    foreach (var dateAndDetail in datesAndDetails)
        //        result.Add(Mappers.GetNewDateAndDetail(dateAndDetail));

        //    return result.ToArray();
        //}

        private static CaseNewDateAndDetail GetNewDateAndDetail(DateAndDetail dateAndDetail)
        {
            var documents = Mappers.GetCaseNewDocuments(dateAndDetail.Documents);

            return new CaseNewDateAndDetail()
            {
                Id = Guid.NewGuid(),
                Documents = documents.ToArray(),
                Date = dateAndDetail.Date,
                Text = dateAndDetail.Text,
            };
        }

        public static List<CaseNewDocument> GetCaseNewDocuments(List<Document> documents)
        {
            var result = new List<CaseNewDocument>();

            foreach (var document in documents)
                result.Add(Mappers.GetCaseNewDocument(document));

            return result;
        }

        public static CaseNewDocument GetCaseNewDocument(Document document)
        {
            var newDocument = new CaseNewDocument();

            newDocument.Id = document.Id;
            newDocument.Name = document.Name;
            newDocument.Body = document.File;

            return newDocument;
        }
        
        private static CaseDateAndDetailUpdate GetDateAndDetailsUpdate(List<DateAndDetail> dateAndDetails, List<Guid> dateAndDetailsToDelete)
        {
            var deletedDateAndDetailsIds = CheckDateAndDetailsToDelete(dateAndDetails, dateAndDetailsToDelete);
            ClearDateAndDetailsDeleteCollection(dateAndDetailsToDelete, deletedDateAndDetailsIds);

            return new CaseDateAndDetailUpdate()
            {
                NewDatesAndDetails = GetNewDateAndDetails(dateAndDetails),
                DeletedDatesAndDetails = dateAndDetailsToDelete.ToArray(),
            };
        }

        private static List<Guid> CheckDateAndDetailsToDelete(List<DateAndDetail> dateAndDetails, List<Guid> dateAndDetailsToDelete)
        {
            var result = new List<Guid>();

            foreach (var dateAndDetailId in dateAndDetailsToDelete)
                CheckDateAndDetailsToDelete(dateAndDetails, dateAndDetailId, result);

            return result;
        }

        private static void CheckDateAndDetailsToDelete(List<DateAndDetail> dateAndDetails, Guid dateAndDetailId, List<Guid> dateAndDetailsToDelete)
        {
            if (CheckDateAndDetailsToDelete(dateAndDetails, dateAndDetailId))
                dateAndDetailsToDelete.Add(dateAndDetailId);
        }

        private static void ClearDateAndDetailsDeleteCollection(List<Guid> dateAndDetailsToDelete, List<Guid> deletedDateAndDetailsIds)
        {
            foreach (var allegationId in deletedDateAndDetailsIds)
                ClearDateAndDetailsDeleteCollection(dateAndDetailsToDelete, allegationId);
        }

        private static void ClearDateAndDetailsDeleteCollection(List<Guid> dateAndDetailsToDelete, Guid dateAndDetailId)
        {
            if (dateAndDetailsToDelete.Any(al => al.Equals(dateAndDetailId)))
                dateAndDetailsToDelete.Remove(dateAndDetailId);
        }

        private static CaseNewDateAndDetail[] GetNewDateAndDetails(List<DateAndDetail> dateAndDetails)
        {
            var newDateAndDetails = new List<CaseNewDateAndDetail>();
            foreach (var dateAndDetail in dateAndDetails)
                newDateAndDetails.Add(Mappers.GetNewDateAndDetail(dateAndDetail));
            return newDateAndDetails.ToArray();
        }

        private static CaseAllegationsUpdate GetAllegationsUpdate(List<AllegationWithMyComment> allegations, List<Guid> allegationsToDelete)
        {
            var deletedAllegationsIds = CheckAllegationsToDelete(allegations, allegationsToDelete);
            ClearAllegationsDeleteCollection(allegationsToDelete, deletedAllegationsIds);

            return new CaseAllegationsUpdate()
            {
                NewAllegations = GetNewAllegations(allegations),
                DeletedAllegations = allegationsToDelete.ToArray(),
            };
        }

        private static List<Guid> CheckAllegationsToDelete(List<AllegationWithMyComment> allegations, List<Guid> allegationIdsToDelete)
        {
            var result = new List<Guid>();

            foreach (var allegationId in allegationIdsToDelete)
                CheckAllegationsToDelete(allegations, allegationId, result);

            return result;
        }

        private static void CheckAllegationsToDelete(List<AllegationWithMyComment> allegations, Guid allegationId, List<Guid> allegationsToDelete)
        {
            if (CheckAllegationToDelete(allegations, allegationId))
                allegationsToDelete.Add(allegationId);
        }

        internal static bool CheckAllegationToDelete(List<AllegationWithMyComment> allegations, Guid allegationId)
        {
            var allegationToDelete = allegations.FirstOrDefault(al => al.Id.Equals(allegationId));
            if (allegationToDelete != null)
            {
                allegations.Remove(allegationToDelete);
                return true;
            }

            return false;
        }

        internal static bool CheckDateAndDetailsToDelete(List<DateAndDetail> dateAndDetails, Guid dateAndDetailId)
        {
            var dateAndDetailToDelete = dateAndDetails.FirstOrDefault(al => al.Id.Equals(dateAndDetailId));
            if (dateAndDetailToDelete != null)
            {
                dateAndDetails.Remove(dateAndDetailToDelete);
                return true;
            }

            return false;
        }

        private static void ClearAllegationsDeleteCollection(List<Guid> allegationsToDelete, List<Guid> deletedAllegationsIds)
        {
            foreach (var allegationId in deletedAllegationsIds)
                ClearAllegationsDeleteCollection(allegationsToDelete, allegationId);
        }

        private static void ClearAllegationsDeleteCollection(List<Guid> allegationsToDelete, Guid allegationId)
        {
            if (allegationsToDelete.Any(al => al.Equals(allegationId)))
                allegationsToDelete.Remove(allegationId);
        }

        private static CaseNewAllegation[] GetNewAllegations(List<AllegationWithMyComment> allegations)
        {
            var newAllegations = new List<CaseNewAllegation>();
            foreach (var allegation in allegations)
                newAllegations.Add(Mappers.GetNewAllegation(allegation));
            return newAllegations.ToArray();
        }

        private static CaseNewAllegation GetNewAllegation(AllegationWithMyComment allegation)
        {
            var documents = Mappers.GetCaseNewDocuments(allegation.Documents);

            return new CaseNewAllegation()
            {
                Id = Guid.NewGuid(),
                Text = allegation.Text,
                Documents = documents.ToArray(),
            };
        }
    }
}