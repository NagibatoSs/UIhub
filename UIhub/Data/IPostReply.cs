using UIhub.Models;

namespace UIhub.Data
{
    public interface IPostReply
    {
        IEnumerable<PostReply> GetAllRepliesByPostId(int postId);
        PostReply GetPostReplyById(int id);
        IEnumerable<PostReply> GetUserPostReplies(string userId);
        IEnumerable<PostReply> GetAllPostsReplies();

        Task Create(PostReply postReply);
        //Task Delete(int postReplyId);
    }
}
