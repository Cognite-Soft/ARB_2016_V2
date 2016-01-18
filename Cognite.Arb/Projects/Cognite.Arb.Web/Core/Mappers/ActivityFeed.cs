using Cognite.Arb.Server.Contract;
using Cognite.Arb.Web.Models.Complaints;
using System;
using System.Collections.Generic;

namespace Cognite.Arb.Web.Core.Mappers
{
    internal static partial class Mappers
    {
        internal static ComplaintActivityFeedViewModel MapActivityFeed(ActivityFeed activityFeed)
        {
            var result = new ComplaintActivityFeedViewModel();
            result.StartDate = activityFeed.StartDate;

            foreach (var section in activityFeed.Sections)
                result.Sections.Add(Mappers.MapActivityFeedSection(section));

            return result;
        }

        private static ComplaintActivityFeedSectionViewModel MapActivityFeedSection(ActivityFeed.ActivityFeedSection section)
        {
            return new ComplaintActivityFeedSectionViewModel()
            {
                SectionStatus = section.SectionStatus,
                SectionType = section.SectionType,
                Header = MapActivityFeedHeader(section.Header, section.SectionStatus),
                Items = MapActivityFeedItems(section.Items),
            };
        }

        private static ComplaintActivityFeedHeaderViewModel MapActivityFeedHeader(ActivityFeed.ActivityFeedSectionHeader header, ActivityFeed.ActivityFeedSectionStatus status)
        {
            var @class = String.Empty;
            var title = header.Title;
            if (header.DueBy.HasValue)
            {
                title = String.Format("{0} are due by {1}", title, header.DueBy.Value.ToShortDateString());
                @class = "warning";
            }
            else
            {
                if (status == ActivityFeed.ActivityFeedSectionStatus.Finished)
                    title = String.Format("{0}. Completed!", title);
                @class = status == ActivityFeed.ActivityFeedSectionStatus.Future ? "secondary" : "success";
            }

            return new ComplaintActivityFeedHeaderViewModel() { DueDate = header.DueBy ?? DateTime.Now, Title = title, Class = @class, };
        }

        private static List<ComplaintActivityFeedItemViewModel> MapActivityFeedItems(ActivityFeed.ActivityFeedItem[] items)
        {
            var result = new List<ComplaintActivityFeedItemViewModel>();

            foreach (var item in items)
                result.Add(MapActivityFeedItem(item));

            return result;
        }

        private static ComplaintActivityFeedItemViewModel MapActivityFeedItem(ActivityFeed.ActivityFeedItem item)
        {
            return new ComplaintActivityFeedItemViewModel()
            {
                Id = item.Id,
                Description = item.Description,
                User = MapUserToUserViewModel(item.User),
                Action = item.Action,
            };
        }
    }
}