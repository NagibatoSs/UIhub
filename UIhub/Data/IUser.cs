using UIhub.Models;

namespace UIhub.Data
{
    public interface IUser
    {
        User GetUserById(string id);
        IEnumerable<Post> GetAllUserPostsById(string id);
        IEnumerable<PostReply> GetAllUserPostsRepliesById(string id);
        IEnumerable<User> GetAllUsers();
        //Post GetPostById(int id);
        //IEnumerable<Post> GetAllPosts();
        //IEnumerable<PostReply> GetAllPostsReplies();
        //User GetAuthor();

        //Task Create(Post post);
        //Task Delete(int postId);
        //Task UpdatePostTitle(int postId, string newTitle);
        //Task UpdatePostDescription(int postId, string newDescription);
    }
}
