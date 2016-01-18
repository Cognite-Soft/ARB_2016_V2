using System;

namespace Cognite.Arb.Server.Contract.Cases
{
    public class AllegationsUpdate
    {
        public NewAllegation[] NewAllegations { get; set; }
        public Guid[] DeletedAllegations { get; set; }
    }
}