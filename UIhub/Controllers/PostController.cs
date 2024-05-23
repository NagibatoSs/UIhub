using Microsoft.AspNetCore.Mvc;
using UIhub.Data;
using UIhub.Models;
using UIhub.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Reflection.PortableExecutable;
using System.Security.Claims;
using UIhub.Service;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace UIhub.Controllers
{
    public class PostController : Controller
    {
        private readonly IPost _postService;
        private readonly UserManager<User> _userManager;

        public PostController(IPost postService, UserManager<User> userManager)
        {
            _postService = postService;
            _userManager = userManager;
        }
        public IActionResult MainPage()
        {
            var posts = _postService
               .GetAllPosts()
               .Select(post => new PostsListViewModel()
               {
                   Id = post.Id,
                   Title = post.Title,
                   AuthorName = post.Author.UserName,
                   Created = post.Created.ToString("U"),
                   Views = post.Views,
                   EstimateCount = post.EstimateCount,
                   isTop = post.IsTop
               })
               .ToList();
            var model = new MainPageViewModel
            {
                Posts = posts
            };
            return View(model);
        }
        //var user = _userManager.FindByIdAsync(userId).Result;
        public IActionResult OpenPostById(int id)
        {
            if (id == 0)
                return View();
            var postModel = CreatePostViewModel(id);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = new PostContentViewModel
            {
                Id = id,
                PostViewModel = postModel,
                CurrentUserId = userId
            };
            return View(model);
        }
        public IActionResult CreateNewPost(NewPostViewModel model = null)
        {
            if (model == null)
            {
                model = CreateNewPostViewModel();
            }
            if (model.InterfaceLayoutsSrc == null)
                model.InterfaceLayoutsSrc = new List<string>();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> PlacePost(NewPostViewModel model, List<IFormFile> FormFiles)
        {
            var userId = _userManager.GetUserId(User);
            var user = _userManager.FindByIdAsync(userId).Result;
            if (model.FormFiles != null)
            {
                var assessment = new AutoAssessmentController();
                var results = assessment.AssessFilesAsync(model.FormFiles).Result;
                AutoAssessmentResult autoAssessment = new AutoAssessmentResult() 
                { ResultValue = results.Item1, ResultText = results.Item2 };
                model.AutoAssessment = autoAssessment;
            };
            var post = BuildPost(model, user);
            _postService.Create(post).Wait();
            return RedirectToAction("OpenPostById", "Post", new { id = post.Id });
        }
        private void BuildEstimateScale(Post post, NewPostViewModel model)
        {
            foreach (var estimate in model.EstimatesScale)
            {
                if (estimate.Characteristic == null) continue;
                var scale = new EstimateScale { Characteristic = estimate.Characteristic, Count_1 = 0, Count_2 = 0, Count_3 = 0, Count_4 = 0, Count_5 = 0 };
                post.Estimates.Add(scale);
            }
        }
        private void BuildEstimateVoting(Post post, NewPostViewModel model)
        {
            foreach (var estimate in model.EstimatesVoting)
            {
                if (estimate.VotingObjects == null) continue;
                var voting = new EstimateVoting
                {
                    Characteristic = estimate.Characteristic,
                };
                voting.VotingObjects = new List<VotingObject>();
                foreach (var obj in estimate.VotingObjects)
                {
                    obj.VoteCount = 0;
                    voting.VotingObjects.Add(obj);
                }
                post.Estimates.Add(voting);
            }
        }
        private void BuildEstimateRanging(Post post, NewPostViewModel model)
        {
            foreach (var estimate in model.EstimatesRanging)
            {
                if (estimate.RangingObjects == null) continue;
                var ranging = new EstimateRanging
                {
                    Characteristic = estimate.Characteristic,
                };
                ranging.RangingObjects = new List<RangingObject>();
                foreach (var obj in estimate.RangingObjects)
                {
                    obj.NumberInSequence = "0";
                    ranging.RangingObjects.Add(obj);
                }
                post.Estimates.Add(ranging);
            }
        }
        private Post BuildPost(NewPostViewModel model, User user)
        {
            var post = new Post
            {
                Title = model.Title,
                Description = model.Description,
                Author = user,
                Created = DateTime.Now,
                Views = 0,
                EstimateCount = 0,
                IsTop = model.IsTop,
                InterfaceLayouts = new List<InterfaceLayout>(),
                Replies = new List<PostReply>(),
                AutoAssessment = model.AutoAssessment,
                Estimates = new List<Estimate>()
            };
            if (model.InterfaceLayoutsSrc != null)
            {
                foreach (var content in model.InterfaceLayoutsSrc)
                {
                    var layoutSrc = new InterfaceLayout() { SourceUrl = content };
                    post.InterfaceLayouts.Add(layoutSrc);
                }
            }
            switch(model.EstimateFormat)
            {
                case "scale":
                    BuildEstimateScale(post, model);
                    break;
                case "voting":
                    BuildEstimateVoting(post, model);
                    break;
                case "ranging":
                    BuildEstimateRanging(post, model);
                    break;
            }
            return post;
        }


        private NewPostViewModel CreateNewPostViewModel()
        {
            return new NewPostViewModel();
        }

        private PostViewModel CreatePostViewModel(int id)
        {
            var post = _postService.GetPostById(id);
            var model = new PostViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Description = post.Description,
                Views = post.Views,
                EstimateCount = post.EstimateCount,
                Created = post.Created.ToString("U"),
                Author = post.Author,
                AutoAssessment = post.AutoAssessment,
                Replies = post.Replies,
                Estimates = post.Estimates,
                EstimatesScale = new List<EstimateScale>(),
                EstimatesVoting = new List<EstimateVoting>(),
                EstimatesRanging = new List<EstimateRanging>(),
                EstimatesResult = new EstimatesResultViewModel(),
                InterfaceLayouts = post.InterfaceLayouts
            };
            if (post.Estimates[0].Discriminator != null)
            {
                switch (post.Estimates[0].Discriminator)
                {
                    case "EstimateScale":
                        model.EstimatesResult.ScaleAverages = new List<double>();
                        for (int i = 0; i < post.Estimates.Count; i++)
                        {
                            model.EstimatesScale.Add(post.Estimates[i] as EstimateScale);
                            model.EstimatesResult.ScaleAverages.Add(CalculateScaleAvg(model.EstimatesScale[i]));
                        }
                        break;
                    case "EstimateVoting":
                        foreach (var item in post.Estimates)
                            model.EstimatesVoting.Add(item as EstimateVoting);
                        string json = JsonConvert.SerializeObject(model.EstimatesVoting);
                        ViewBag.Voting = json;
                        break;
                    case "EstimateRanging":
                        model.RangingEstimatesPresenter = new List<RangingEstimatePresenterViewModel>();
                        foreach (var item in post.Estimates)
                        {
                            var estRange = item as EstimateRanging;
                            model.EstimatesRanging.Add(estRange);
                        }
                        for (int i = 0; i < model.EstimatesRanging.Count; i++)
                        {
                            List<string> mostCommon = new List<string>();
                            mostCommon.AddRange(
                                model.EstimatesRanging[i].Sequences
                                .GroupBy(s => s.NumbersOrder)
                                .OrderByDescending(grp => grp.Count())
                                .Select(grp => grp.Key)
                                .Take(3));

                            model.RangingEstimatesPresenter.Add(new RangingEstimatePresenterViewModel { Contents = mostCommon });

                        }
                        var rg = 23;
                        break;
                }
            }
            return model;
        }
        private double CalculateScaleAvg(EstimateScale scale)
        {
            double avg = 0;
            int count = scale.Count_1 + scale.Count_2 + scale.Count_3 + scale.Count_4 + scale.Count_5;
            avg += scale.Count_1 * 1;
            avg += scale.Count_2 * 2;
            avg += scale.Count_3 * 3;
            avg += scale.Count_4 * 4;
            avg += scale.Count_5 * 5;
            return (Math.Round(avg / count, 2));
        }

    }
}
