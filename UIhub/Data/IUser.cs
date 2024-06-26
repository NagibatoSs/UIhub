﻿using UIhub.Models;

namespace UIhub.Data
{
    public interface IUser
    {
        User GetUserById(string id);
        IEnumerable<Post> GetAllUserPostsById(string id);
        IEnumerable<PostReply> GetAllUserPostsRepliesById(string id);
        IEnumerable<User> GetAllUsers();
        Task Update(User user);
        User GetUserByEmail(string email);
        void IncreaseReputationForEstimate(string id);
        void IncreaseReputationForLike(string id);
        public bool BuyTop(string userId);
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
