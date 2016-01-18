using Cognite.Arb.Server.Contract.Cases;
using System;

namespace Cognite.Arb.Web.Models.Complaints
{
    public class MyAllegationComment
    {
        public Guid Id { get; set; }
        public AllegationCommentType AllegationCommentType { get; set; }
        public string AllegationCommentTypeText { get { return GetAllegationCommentTypeText(); } }
        public string Text { get; set; }
        public string AdditionalText { get; set; }
        
        private string GetAllegationCommentTypeText()
        {
            switch (AllegationCommentType)
            {
                case AllegationCommentType.Yes:
                    return "Yes";
                case AllegationCommentType.No:
                    return "No";
                case AllegationCommentType.Advise:
                    return "Advise";
            }

            return String.Empty;
        }
    }
}
