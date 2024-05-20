using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UIhub.Data;
using UIhub.Models;
using UIhub.Service;

namespace UIhub.Rating
{
    public class RatingAccrual: Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IUser _userService;
        private readonly IUserRank _userRankService;
        public RatingAccrual(UserManager<User> userManager, IUser userService, IUserRank userRankService)
        {
            _userService = userService;
            _userManager = userManager;
            _userRankService = userRankService;
        }
        public void AddReputationOfEstimate(string userId)
        {
            var user = _userManager.FindByIdAsync(userId).Result;
            user.Reputation++;
            user.Points++;
            CheckUserRate(user);
            _userService.Update(user).Wait();
        }
        private void CheckUserRate(User user)
        {
            var ranks = _userRankService.GetAllRanks();
            foreach(var r in ranks)
            {
                if (user.Reputation == r.MinReputation)
                {
                    user.Rank = r;
                    break;
                }
            }
            _userService.Update(user).Wait();
        }
    }
}
