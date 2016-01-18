using System;
using System.Collections.Generic;

namespace Cognite.Arb.Server.Business.Database
{
    public class CaseData
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public CaseStatus Status { get; set; }
        public DateTime? StartDate { get; set; }
        
        public string Background { get; set; }
        public string IdealOutcome { get; set; }
        public Question IssueRaisedWithArchitect { get; set; }
        public Question SubjectOfLegalProceedings { get; set; }
        public string Relationship { get; set; }
        public bool? ContactAgreement { get; set; }
        public ContactData ClaimantContact { get; set; }
        public ArchitectContactData ArchitectContact { get; set; }
        
        public DatesAndDetail[] DatesAndDetails { get; set; }
        public Allegation[] Allegations { get; set; }
        public UserHeader[] AssignedUsers { get; set; }

        public Document PreliminaryDecisionDocument { get; set; }
        public Document FinalDecisionDocument { get; set; }
        public DateTime? ProcessStartDate { get; set; }

        public class ContactData
        {
            public string Name { get; set; }
            public string EMail { get; set; }
            public string Address { get; set; }
            public string Phone { get; set; }
        }

        public class ArchitectContactData : ContactData
        {
            public int? RegistrationNumber { get; set; }
        }

        public class Question
        {
            public bool? Answer { get; set; }
            public string Comments { get; set; }
        }

        public enum CaseStatus
        {
            New = 0,
            Open = 1,
            Rejected = 2,
            Closed = 3,
        }

        public class DatesAndDetail
        {
            public Guid Id { get; set; }
            public DateTime Date { get; set; }
            public string Text { get; set; }
            public IEnumerable<Document> Documents { get; set; }
        }

        public class Allegation
        {
            public Guid Id { get; set; }
            public string Text { get; set; }
            public Document[] Documents { get; set; }
            public Comment[] Comments { get; set; }
        }

        public class Document
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        public class Comment
        {
            public Guid PanelMemberId { get; set; }
            public CommentType CommentType { get; set; }
            public string Text { get; set; }
            public string AdditionalText { get; set; }
        }

        public enum CommentType
        {
            Yes = 1,
            No = 2,
            Advise = 3,
        }
    }
}
