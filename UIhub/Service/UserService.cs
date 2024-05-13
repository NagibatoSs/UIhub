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
                .FirstOrDefault();
        }
    }
}
