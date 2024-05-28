using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UIhub.Data;
using UIhub.Models;

namespace UIhub.Service
{
    public class UserService: IUser
    {
        ApplicationDbContext _context;
        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task Update(User user)
        {
            _context.Update(user);
            await _context.SaveChangesAsync();
        }
        public IEnumerable<Post> GetAllUserPostsById(string id)
        {
            return _context.Posts.Where(p => p.Author.Id == id);
        }

        public IEnumerable<PostReply> GetAllUserPostsRepliesById(string id)
        {
            return _context.PostTextReplies.Where(p => p.Author.Id == id);
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _context.Users;
        }

        public User GetUserById(string id)
        {
            return _context.Users
                .Where(u => u.Id == id)
                .Include(u => u.Rank)
                .Include(u => u.UserPostEstimates)
                .Include(u=>u.PostReplyLikes)
                .FirstOrDefault();
        }

        public User GetUserByEmail(string email)
        {
            return _context.Users.Where(u => u.Email.Equals(email)).FirstOrDefault();
        }
        public void IncreaseReputationForEstimate(string userId)
        {
            var user = GetUserById(userId);
            user.Reputation++;
            user.Points++;
            CheckUserRate(user);
            Update(user).Wait();
        }
        public void IncreaseReputationForLike(string userId)
        {
            var user = GetUserById(userId);
            user.Reputation+=5;
            user.Points+=5;
            CheckUserRate(user);
            Update(user).Wait();
        }
        private void CheckUserRate(User user)
        {
            var ranks = _context.UserRanks;
            foreach (var r in ranks)
            {
                if (user.Reputation == r.MinReputation)
                {
                    user.Rank = r;
                    break;
                }
            }
            Update(user).Wait();
        }
    }
}
