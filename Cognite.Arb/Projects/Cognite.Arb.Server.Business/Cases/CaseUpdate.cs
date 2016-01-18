using System;

namespace Cognite.Arb.Server.Business.Cases
{
    public class CaseUpdate
    {
        public InitialCaseDataUpdate Initial { get; set; }
        public CaseContacts Contacts { get; set; }
        public AllegationsUpdate AllegationsUpdate { get; set; }
        public DateAndDetailUpdate DatesAndDetailsUpdate { get; set; }
    }

    public class DateAndDetailUpdate
    {
        public NewDateAndDetail[] NewDatesAndDetails { get; set; }
        public Guid[] DeletedDatesAndDetails { get; set; }
    }
}
