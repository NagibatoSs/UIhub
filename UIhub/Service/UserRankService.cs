using UIhub.Data;
using UIhub.Models;

namespace UIhub.Service
{
    public class UserRankService : IUserRank
    {
        private readonly ApplicationDbContext _context;
        public UserRankService(ApplicationDbContext context)
        {
            _context = context;
        }
        public IEnumerable<UserRank> GetAllRanks()
        {
            return _context.UserRanks;
            //return _context.PostTextReplies
            //  .Where(reply => reply.Post.Id == postId)
            //  .Include(reply => reply.Post)
            //  .Include(reply => reply.Author);
        }

        public UserRank GetDefaultRank()
        {
            return _context.UserRanks.FirstOrDefault();
        }
    }
}
