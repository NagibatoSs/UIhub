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
//    var user = _userManager.FindByIdAsync(_userManager.GetUserId(User)).Result;
//    estimateScale.UserEstimates.Add(new UserEstimate { Estimate = estimateScale, User = user });
public class EstimateController : Controller
    {
        private readonly IEstimate _estimateService;
        private readonly IPost _postService;
        private readonly UserManager<User> _userManager;
        private readonly IUser _userService;
        
        public EstimateController(IEstimate estimateService, UserManager<User> userManager, IPost postService, IUser userService)
        {
            _estimateService = estimateService;
            _userManager = userManager;
            _postService = postService;
            _userService = userService;
        }
        [HttpPost]
        public async Task<IActionResult> SetScaleEstimate(PostContentViewModel model)
        {
            for (int i = 0; i < model.NewEstimateViewModel.Count; i++)
            {
                var modelScale = model.NewEstimateViewModel[i];
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
            AddUserPostEstimate(model.Id);
            AddPostEstimateCount(model.Id);
            _userService.IncreaseReputationForEstimate(_userManager.GetUserId(User));
            return RedirectToAction("OpenPostById", "Post", new { id = model.Id });
        }
        private void AddUserPostEstimate(int postId)
        {
            var user = _userManager.FindByIdAsync(_userManager.GetUserId(User)).Result;
            var post = _postService.GetPostById(postId);
            post.UserPostEstimates.Add(new UserPostEstimate { Post = post, User = user });
            _postService.Update(post).Wait();
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
            for (int i = 0; i < model.NewEstimateViewModel.Count; i++)
            {
                var modelVoting = model.NewEstimateViewModel[i];
                var estimateVote = _estimateService.GetEstimateVoting(model.PostViewModel.EstimatesVoting[i].Id);
                estimateVote.VotingObjects[int.Parse(modelVoting.SelectedValue)].VoteCount++;
                _estimateService.UpdateEstimate(estimateVote).Wait();
            }
            AddUserPostEstimate(model.Id);
            AddPostEstimateCount(model.Id);
            _userService.IncreaseReputationForEstimate(_userManager.GetUserId(User));
            return RedirectToAction("OpenPostById", "Post", new { id = model.Id });
        }

        [HttpPost]
        public async Task<IActionResult> SetRangingEstimate(PostContentViewModel model)
        {
            for (int i = 0; i < model.NewEstimateViewModel.Count; i++)
            {
                var modelRanging = model.NewEstimateViewModel[i];
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
            AddUserPostEstimate(model.Id);
            AddPostEstimateCount(model.Id);
            _userService.IncreaseReputationForEstimate(_userManager.GetUserId(User));
            return RedirectToAction("OpenPostById", "Post", new { id = model.Id });
        }
    }
}
