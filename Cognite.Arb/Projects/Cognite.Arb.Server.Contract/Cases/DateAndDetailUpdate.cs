using System;

namespace Cognite.Arb.Server.Contract.Cases
{
    public class DateAndDetailUpdate
    {
        public NewDateAndDetail[] NewDatesAndDetails { get; set; }
        public Guid[] DeletedDatesAndDetails { get; set; }
    }
}
