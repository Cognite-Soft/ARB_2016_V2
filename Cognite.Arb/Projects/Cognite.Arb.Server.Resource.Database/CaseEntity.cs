using System;
using System.Collections.Generic;

namespace Cognite.Arb.Server.Resource.Database
{
    public class CaseEntity
    {
        public CaseEntity()
        {
            PartiesComments = new List<PartiesCommentEntity>();
            AssignedUsers = new List<UserEntity>();
            DatesAndDetails = new List<DateAndDetailEntity>();
            Allegations = new List<AllegationEntity>();
            PreliminaryDecisionComments = new List<PreliminaryDecisionCommentEntity>();
            FinalDecisionComments = new List<FinalDecisionCommentEntity>();
        }

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
        public Contact ClaimantContact { get; set; }
        public Contact ArchitectContact { get; set; }
        public Guid? PreliminaryDecisionDocumentId { get; set; }
        public Guid? FinalDecisionDocumentId { get; set; }
        //public DateTime? FinalDecisionSubmitDate { get; set; }

        public virtual DocumentEntity PreliminaryDecisionDocument { get; set; }
        public virtual DocumentEntity FinalDecisionDocument { get; set; }
        public virtual ICollection<PartiesCommentEntity> PartiesComments { get; protected set; }

        public class Contact
        {
            public string Name { get; set; }
            public int? RegistrationNumber { get; set; }
            public string EMail { get; set; }
            public string Address { get; set; }
            public string Phone { get; set; }
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

        public virtual ICollection<UserEntity> AssignedUsers { get; set; }
        public virtual ICollection<DateAndDetailEntity> DatesAndDetails { get; set; }
        public virtual ICollection<AllegationEntity> Allegations { get; set; }
        public virtual ICollection<PreliminaryDecisionCommentEntity> PreliminaryDecisionComments { get; set; }
        public virtual ICollection<FinalDecisionCommentEntity> FinalDecisionComments { get; set; }
        public DateTime? ProcessStartDate { get; set; }
    }
}