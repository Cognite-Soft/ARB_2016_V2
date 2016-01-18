using System.Linq;
using Cognite.Arb.Server.Contract;
using Cognite.Arb.Web.Models.Complaints;
using System;
using System.Collections.Generic;

namespace Cognite.Arb.Web.Core.Mappers
{
    internal static partial class Mappers
    {
        internal static ComplaintDiscussionsViewModel MapDiscussions(Post[] posts)
        {
            var result = new ComplaintDiscussionsViewModel();

            result.Replies = Mappers.MapPosts(posts);

            return result;
        }

        private static List<DiscussionComment> MapPosts(Post[] posts)
        {
            var result = new List<DiscussionComment>();

            foreach (var post in posts)
                result.Add(Mappers.MapPost(post));

            return result;
        }

        private static DiscussionComment MapPost(Post post)
        {
            var result = new DiscussionComment();

            result.Id = post.Id;
            result.Text = post.Text;
            result.User = Mappers.MapUserToUserViewModel(post.User);

            if (post.Parent != null)
                result.RootComment = Mappers.MapParent(post.Parent);

            return result;
        }

        private static DiscussionComment MapParent(Reply reply)
        {
            var result = new DiscussionComment();

            result.Id = reply.Id;
            result.Text = reply.Text;
            result.User = Mappers.MapUserToUserViewModel(reply.User);

            return result;
        }
    }
}