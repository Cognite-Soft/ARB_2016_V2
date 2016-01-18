namespace Cognite.Arb.Server.Contract.Cases
{
    public class CaseUpdate
    {
        public InitialCaseDataUpdate Initial { get; set; }
        public CaseContacts Contacts { get; set; }
        public AllegationsUpdate AllegationsUpdate { get; set; }
        public DateAndDetailUpdate DatesAndDetailsUpdate { get; set; }
    }
}
