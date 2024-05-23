using Microsoft.AspNetCore.Mvc;
using System.Text;
using UIhub.Models.ViewModels;
using UIhub.Models;
using UIhub.Service;
using UIhub.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using UIhub.Rating;

namespace UIhub.Controllers
{
    public class EstimateController : Controller
    {
        private readonly IEstimate _estimateService;
        private readonly IPost _postService;
        private readonly UserManager<User> _userManager;
        private readonly IUser _userService;
        private readonly IUserRank _userRankService;
        private readonly RatingAccrual _rating;
        public EstimateController(IEstimate estimateService, UserManager<User> userManager, IPost postService, IUser userService, IUserRank userRankService)
        {
            _estimateService = estimateService;
            _userManager = userManager;
            _postService = postService;
            _userService = userService;
            _userRankService = userRankService;
            _rating = new RatingAccrual(_userManager, _userService, _userRankService);
        }
        [HttpPost]
        public async Task<IActionResult> SetScaleEstimate(PostContentViewModel model)
        {
            for (int i = 0; i < model.NewScaleEstimateViewModel.Count; i++)
            {
                var modelScale = model.NewScaleEstimateViewModel[i];
                var estimateScale = (_estimateService.GetEstimate(model.PostViewModel.EstimatesScale[i].Id)) as EstimateScale;
                switch (modelScale.SelectedValue)
                {
                    case "1":
                        estimateScale.Count_1++;
                        break;
                    case "2":
                        estimateScale.Count_2++;
                        break;
                    case "3":
                        estimateScale.Count_3++;
                        break;
                    case "4":
                        estimateScale.Count_4++;
                        break;
                    case "5":
                        estimateScale.Count_5++;
                        break;
                }
                _estimateService.UpdateEstimate(estimateScale).Wait();
            }
            AddPostEstimateCount(model.Id);
            _rating.AddReputationOfEstimate(_userManager.GetUserId(User));
            return RedirectToAction("OpenPostById", "Post", new { id = model.Id });
        }
        private void AddPostEstimateCount(int id)
        {
            var post = _postService.GetPostById(id);
            post.EstimateCount++;
            _postService.Update(post).Wait();
        }

        [HttpPost]
        public async Task<IActionResult> SetVotingEstimate(PostContentViewModel model)
        {
            for (int i = 0; i < model.NewVotingEstimateViewModel.Count; i++)
            {
                var modelVoting = model.NewVotingEstimateViewModel[i];
                var estimateVote = _estimateService.GetEstimateVoting(model.PostViewModel.EstimatesVoting[i].Id);
                estimateVote.VotingObjects[int.Parse(modelVoting.SelectedValue)].VoteCount++;
                _estimateService.UpdateEstimate(estimateVote).Wait();
            }
            AddPostEstimateCount(model.Id);
            _rating.AddReputationOfEstimate(_userManager.GetUserId(User));
            return RedirectToAction("OpenPostById", "Post", new { id = model.Id });
        }

        [HttpPost]
        public async Task<IActionResult> SetRangingEstimate(PostContentViewModel model)
        {
            for (int i = 0; i < model.NewRangingEstimateViewModel.Count; i++)
            {
                var modelRanging = model.NewRangingEstimateViewModel[i];
                var estimateRange = _estimateService.GetEstimateRanging(model.PostViewModel.EstimatesRanging[i].Id);
                StringBuilder order = new StringBuilder();
                for (int j = 0; j < estimateRange.RangingObjects.Count; j++)
                {
                    var numberByIndex = int.Parse(model.PostViewModel.EstimatesRanging[i].RangingObjects[j].NumberInSequence) - 1;
                    //estimateRange.RangingObjects[j].NumberInSequence = j.ToString();
                    order.Append(numberByIndex);
                }
                var sequence = new RangingSequence() { NumbersOrder = order.ToString() };
                estimateRange.Sequences.Add(sequence);
                _estimateService.UpdateEstimate(estimateRange).Wait();
            }
            AddPostEstimateCount(model.Id);
            _rating.AddReputationOfEstimate(_userManager.GetUserId(User));
            return RedirectToAction("OpenPostById", "Post", new { id = model.Id });
        }
    }
}
