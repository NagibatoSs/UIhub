using Newtonsoft.Json;
using UIhub.Models.ViewModels;
using UIhub.Models;
using UIhub.Service;
using Microsoft.AspNetCore.Identity;
using UIhub.Controllers;

namespace UIhub
{
    public class ViewModelsBuilder
    {
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _webHost;
        public ViewModelsBuilder(UserManager<User> userManager, IWebHostEnvironment webHost)
        {
            _userManager = userManager;
            _webHost = webHost;
        }
        public Post BuildPost(NewPostViewModel model, User user)
        {
            var post = new Post
            {
                Title = model.Title,
                Description = model.Description,
                Author = user,
                Created = DateTime.Now,
                EstimateCount = 0,
                IsTop = model.IsTop,
                InterfaceLayouts = new List<InterfaceLayout>(),
                Replies = new List<PostReply>(),
                Estimates = new List<Estimate>()
            };
            if (model.FormFiles != null)
            {
                post.AutoAssessment = GetAutoAssessmentResult(model.FormFiles);
            };
            BuildPostInterfaces(model, post);
            switch (model.EstimateFormat)
            {
                case "scale":
                    BuildPostEstimateScale(post, model);
                    break;
                case "voting":
                    BuildPostEstimateVoting(post, model);
                    break;
                case "ranging":
                    BuildPostEstimateRanging(post, model);
                    break;
            }
            
            return post;
        }
        private void BuildPostInterfaces(NewPostViewModel model, Post post)
        {
            if (model.InterfaceLayoutsSrc != null)
            {
                foreach (var content in model.InterfaceLayoutsSrc)
                {
                    var layoutSrc = new InterfaceLayout() { SourceUrl = content, SourceType = "figma" };
                    post.InterfaceLayouts.Add(layoutSrc);
                }
            }
            if (model.ImgFormFiles != null)
            {
                string uploadFolder = Path.Combine(_webHost.WebRootPath, "Interfaces");
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                foreach (var uploadedFile in model.ImgFormFiles)
                {
                    string fileName = Path.GetFileName(DateTime.Now.ToString("yyyyMMddHHmmss") + uploadedFile.FileName);
                    string fileSavePath = Path.Combine(uploadFolder, fileName);
                    using (FileStream stream = new FileStream(fileSavePath, FileMode.Create))
                    {
                        uploadedFile.CopyToAsync(stream).Wait();
                    }
                    post.InterfaceLayouts.Add(new InterfaceLayout { SourceUrl = fileName, SourceType = "img" });
                }
            };
        }
        private AutoAssessmentResult GetAutoAssessmentResult(List<IFormFile> formFiles)
        {
            var assessment = new AutoAssessmentController(_userManager);
            var results = assessment.AssessFilesAsync(formFiles).Result;
            AutoAssessmentResult autoAssessment = new AutoAssessmentResult()
            { ResultValue = results.Item1, ResultText = results.Item2 };
            return autoAssessment;
        }
        private void BuildPostEstimateScale(Post post, NewPostViewModel model)
        {
            foreach (var estimate in model.EstimatesScale)
            {
                if (estimate.Characteristic == null) continue;
                var scale = new EstimateScale { Characteristic = estimate.Characteristic, Count_1 = 0, Count_2 = 0, Count_3 = 0, Count_4 = 0, Count_5 = 0 };
                post.Estimates.Add(scale);
            }
        }
        private void BuildPostEstimateVoting(Post post, NewPostViewModel model)
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
        private void BuildPostEstimateRanging(Post post, NewPostViewModel model)
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
        public PostViewModel BuildPostViewModel(Post post)
        {
            var model = new PostViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Description = post.Description,
                EstimateCount = post.EstimateCount,
                Created = post.Created.ToString("U"),
                Author = post.Author,
                AutoAssessment = post.AutoAssessment,
                Replies = post.Replies,
                EstimatesScale = new List<EstimateScale>(),
                EstimatesVoting = new List<EstimateVoting>(),
                EstimatesRanging = new List<EstimateRanging>(),
                EstimatesResult = new EstimatesResultViewModel(),
                InterfaceLayouts = post.InterfaceLayouts
            };
            if (post.Estimates.Any())
            {
                switch (post.Estimates[0].Discriminator)
                {
                    case "EstimateScale":
                        BuildPostEstimateScaleViewModel(model, post);
                        break;
                    case "EstimateVoting":
                        BuildPostEstimateVotingViewModel(model, post);
                        break;
                    case "EstimateRanging":
                        BuildPostEstimateRangingViewModel(model, post);
                        break;
                }
            }
            return model;
        }
        private void BuildPostEstimateRangingViewModel(PostViewModel model, Post post)
        {
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
        }
        private void BuildPostEstimateScaleViewModel(PostViewModel model, Post post)
        {
            model.EstimatesResult.ScaleAverages = new List<double>();
            for (int i = 0; i < post.Estimates.Count; i++)
            {
                model.EstimatesScale.Add(post.Estimates[i] as EstimateScale);
                model.EstimatesResult.ScaleAverages.Add(CalculateScaleAvg(model.EstimatesScale[i]));
            }
        }
        private double CalculateScaleAvg(EstimateScale scale)
        {
            double avg = 0;
            int count = scale.Count_1 + scale.Count_2 + scale.Count_3 + scale.Count_4 + scale.Count_5;
            if (count == 0) return 0;
            avg += scale.Count_1 * 1;
            avg += scale.Count_2 * 2;
            avg += scale.Count_3 * 3;
            avg += scale.Count_4 * 4;
            avg += scale.Count_5 * 5;
            return Math.Round(avg / count, 1);
        }
        private void BuildPostEstimateVotingViewModel(PostViewModel model, Post post)
        {
            foreach (var item in post.Estimates)
                model.EstimatesVoting.Add(item as EstimateVoting);
        }

        public NewPostViewModel BuildNewPostViewModel(Post post)
        {
            var model = new NewPostViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Description = post.Description,
                EstimateCount = post.EstimateCount,
                Author = post.Author,
                Created = post.Created,
                EstimateFormat = post.Estimates[0].Discriminator,
                EstimatesRanging = new List<EstimateRanging>(),
                EstimatesScale = new List<EstimateScale>(),
                EstimatesVoting = new List<EstimateVoting>(),
                InterfaceLayoutsSrc = post.InterfaceLayouts.Select(l => l.SourceUrl).ToList()
            };
            switch (model.EstimateFormat)
            {
                case "EstimateScale":
                    model.EstimatesScale = BuildNewPostEstimateViewModel<EstimateScale>(post);
                    break;
                case "EstimateVoting":
                    model.EstimatesVoting = BuildNewPostEstimateViewModel<EstimateVoting>(post);
                    break;
                case "EstimateRanging":
                    model.EstimatesRanging = BuildNewPostEstimateViewModel<EstimateRanging>(post);
                    break;
            }
            return model;
        }
        private List<TEstimate> BuildNewPostEstimateViewModel<TEstimate>(Post post) where TEstimate : Estimate, new()
        {
            var estimates = new List<TEstimate>();
            foreach (var estimate in post.Estimates)
            {
                if (estimate.Characteristic == null) continue;
                estimates.Add(new TEstimate { Characteristic = estimate.Characteristic });
            }
            return estimates;
        }

    }
}
