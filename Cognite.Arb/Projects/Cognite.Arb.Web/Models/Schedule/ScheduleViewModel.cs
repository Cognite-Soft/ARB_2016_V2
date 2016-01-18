using Cognite.Arb.Web.Core;
using System.Collections.Generic;

namespace Cognite.Arb.Web.Models.Schedule
{
    public class ScheduleViewModel
    {
        public List<ScheduleRowViewModel> ScheduleRows { get; set; }

        public ScheduleViewModel()
        {
            this.ScheduleRows = new List<ScheduleRowViewModel>(ArbConstants.ScheduleRowsCount);
        }
    }
}