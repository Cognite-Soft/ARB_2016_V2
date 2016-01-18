using System;

namespace Cognite.Arb.Server.Contract
{
    public class ActivityFeed
    {
        public DateTime? StartDate { get; set; }
        public ActivityFeedSection[] Sections { get; set; }

        public class ActivityFeedSection
        {
            public ActivityFeedSectionStatus SectionStatus { get; set; }
            public ActivityFeedSectionType SectionType { get; set; }
            public ActivityFeedSectionHeader Header { get; set; }
            public ActivityFeedItem[] Items { get; set; }
        }

        public class ActivityFeedSectionHeader
        {
            public string Title { get; set; }
            public DateTime? DueBy { get; set; }
        }

        public class ActivityFeedItem
        {
            public User User { get; set; }
            public ActivityFeedAction Action { get; set; }
            public string Description { get; set; }
            public Guid Id { get; set; }
        }

        public enum ActivityFeedSectionType
        {
            PreliminaryComments,
            PreliminaryDecision,
            FinalDecision,
        }

        public enum ActivityFeedSectionStatus
        {
            Future,
            Current,
            Finished,
        }

        public enum ActivityFeedAction
        {
            PreliminaryAllegationComment, // navigate to overview
            PreliminaryDecisionComment,
            FinalDecisionComment,
            CreateDocument, // get document
            UpdateDocument, // get document
            DeleteDocument, // get document
            Discussion, // navigate to discussions
            DiscussionComment, // navigate to discussions
        }
    }
}
