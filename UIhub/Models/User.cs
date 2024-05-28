using Microsoft.AspNetCore.Identity;

namespace UIhub.Models
{
    public class User: IdentityUser
    {
        public int Reputation { get; set; }
        public int Points { get; set; }
        public virtual UserRank Rank { get; set; }
        public virtual List<Post>? Post { get; set; }
        public virtual List<PostReply>? PostReplies { get; set; }
        public virtual List<UserPostEstimate>? UserPostEstimates { get; set; }
        public virtual List<PostReplyLike>? PostReplyLikes { get; set; }
    }
}
