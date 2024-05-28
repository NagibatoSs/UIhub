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
        }
        public UserRank GetRank(int id)
        {
            return _context.UserRanks
                .Where(r => r.Id == id)
                .FirstOrDefault();
        }

        public UserRank GetDefaultRank()
        {
            return _context.UserRanks.FirstOrDefault();
        }
    }
}
