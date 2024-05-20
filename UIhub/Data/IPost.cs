using System.Collections;
using UIhub.Models;

namespace UIhub.Data
{
    public interface IPost
    {
        Post GetPostById(int id);
        IEnumerable<Post> GetAllPosts();
        Task Create(Post post);
        Task Update(Post post);
        //IEnumerable<PostReply> GetAllPostReplies(int id);
        //User GetPostAuthorById(int id);
        //IEnumerable<Post> GetAllUserPosts(string userId);

        //Task Create(Post post);
        //Task Delete(int postId);
        //Task UpdatePostTitle(int postId, string newTitle);
        //Task UpdatePostDescription(int postId, string newDescription);
        //Task AddReply(PostReply reply);
    }
}
