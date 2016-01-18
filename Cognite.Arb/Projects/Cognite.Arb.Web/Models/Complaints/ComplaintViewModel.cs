using Cognite.Arb.Web.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cognite.Arb.Web.Models.Complaints
{
    public class ComplaintOverviewViewModel
    {
        #region Readonly data

        public int Id { get; set; }
        public int ParentId { get; set; }
        public ComplaintState State { get; set; }
        
        #endregion

        #region Initial data

        public ComplaintPanelMembers PanelMembers { get; set; }
        
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime StartDate { get; set; }
        public string Background { get; set; }
        public string IdealOutcome { get; set; }
        public Question IssueRaisedWithArchitect { get; set; }
        public Question SubjectOfLegalProceedings { get; set; }
        public bool ComplaintCanBeEdited { get; set; }
        public bool IsReady { get; set; }

        #endregion

        #region Dates & details
        
        public List<DateAndDetail> DatesAndDetails { get; set; }
        
        #endregion

        #region Allegations

        public AllegationsCollection Allegations { get; set; }

        #endregion

        #region Contacts

        public string Relationship { get; set; }
        public bool ContactAgreement { get; set; }
        public string ContactAgreementText { get { return this.ContactAgreement.GetBooleanYesNoText(); } }
        public Contact ClaimantContact { get; set; }
        public ArchitectContact ArchitectContact { get; set; }
        
        #endregion

        #region New elements

        public DateAndDetail NewDateAndDetail { get; set; }
        public AllegationWithMyComment NewAllegation { get; set; }

        #endregion
        
        public ComplaintOverviewViewModel()
        {
            this.State = new ComplaintState();
            this.PanelMembers = new ComplaintPanelMembers();
            this.IssueRaisedWithArchitect = new Question();
            this.SubjectOfLegalProceedings = new Question();
            this.DatesAndDetails = new List<DateAndDetail>();
            this.Allegations = new AllegationsCollection();
            this.ClaimantContact = new Contact();
            this.ArchitectContact = new ArchitectContact();

            this.NewDateAndDetail = new DateAndDetail();
            this.NewAllegation = new AllegationWithMyComment();
        }
    }
}