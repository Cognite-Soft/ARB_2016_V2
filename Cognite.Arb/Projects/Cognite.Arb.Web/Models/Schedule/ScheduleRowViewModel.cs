using Cognite.Arb.Web.Core;
using System.Collections.Generic;

namespace Cognite.Arb.Web.Models.Schedule
{
    public class ScheduleRowViewModel
    {
        public List<ScheduleCellViewModel> ScheduleRowItems { get; set; }
        public int Index { get; set; }

        public ScheduleRowViewModel()
        {
            this.ScheduleRowItems = new List<ScheduleCellViewModel>(ArbConstants.ScheduleRowCapacity);
        }
    }
}