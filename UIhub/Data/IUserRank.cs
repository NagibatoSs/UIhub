using UIhub.Models;

namespace UIhub.Data
{
    public interface IUserRank
    {
        IEnumerable<UserRank> GetAllRanks();
        UserRank GetDefaultRank();
        //  UserRank GetUserRankByUserId(string userId);
        UserRank GetRank(int id);
    }
}
