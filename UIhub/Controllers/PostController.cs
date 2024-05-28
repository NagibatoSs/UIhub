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
using System;
using UIhub.AutomatedAssessment.ControlElementsAssessment;
using UIhub.AutomatedAssessment.LongParagraphsAssessment;
using UIhub.AutomatedAssessment;
using System.IO;
using Microsoft.Extensions.Hosting;

namespace UIhub.Controllers
{
    public class PostController : Controller
    {
        private readonly IPost _postService;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _webHost;

        public PostController(IPost postService, UserManager<User> userManager, IWebHostEnvironment webHost)
        {
            _postService = postService;
            _userManager = userManager;
            _webHost = webHost;
        }
        public IActionResult MainPage()
        {
            if (_postService.GetAllPosts() == null)
                return View();
            var posts = _postService
               .GetAllPosts()
               .Select(post => new PostsListViewModel()
               {
                   Id = post.Id,
                   Title = post.Title,
                   AuthorName = post.Author.UserName,
                   Created = post.Created.ToString("U"),
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
            var user = _userManager.FindByIdAsync(_userManager.GetUserId(User)).Result;
            model.IsPostEstimatedByUser = _postService.GetPostById(id)
                .UserPostEstimates
                .Select(e => e.User)
                .Contains(user);
            return View(model);
        }
        [HttpPost]
        public IActionResult EditPost(int id)
        {
            var post = _postService.GetPostById(id);
            var model = new NewPostViewModel
            {
                Id = id,
                Title = post.Title,
                Description = post.Description,
                EstimateCount = post.EstimateCount,
                Author = post.Author,
                Created = post.Created,
                EstimateFormat = post.Estimates[0].Discriminator,
                EstimatesRanging = new List<EstimateRanging>(),
                EstimatesScale = new List<EstimateScale>(),
                EstimatesVoting = new List<EstimateVoting>()
            };

            model.InterfaceLayoutsSrc = post.InterfaceLayouts.Select(l => l.SourceUrl).ToList();
            switch (model.EstimateFormat)
            {
                case "EstimateScale":
                    foreach (var estimate in post.Estimates)
                    {
                        if (estimate.Characteristic == null) continue;
                        var scale = new EstimateScale { Characteristic = estimate.Characteristic };
                        model.EstimatesScale.Add(scale);
                    }
                    break;
                case "EstimateVoting":
                    foreach (var estimate in post.Estimates)
                    {
                        if (estimate.Characteristic == null) continue;
                        var voting = new EstimateVoting { Characteristic = estimate.Characteristic};
                        model.EstimatesVoting.Add(voting);
                    }
                    break;
                case "EstimateRanging":
                    foreach (var estimate in post.Estimates)
                    {
                        if (estimate.Characteristic == null) continue;
                        var ranging = new EstimateRanging { Characteristic = estimate.Characteristic };
                        model.EstimatesRanging.Add(ranging);
                    }
                    break;
            }
            return View(model);
        }
        [HttpPost]
        public IActionResult UpdatePost(NewPostViewModel model)
        {
            var post = _postService.GetPostById(model.Id);
            post.Title = model.Title;
            post.Description = model.Description;
            post.InterfaceLayouts = new List<InterfaceLayout>();
            string postFormat = post.Estimates[0].Discriminator;
            foreach (var l in model.InterfaceLayoutsSrc)
            {
                post.InterfaceLayouts.Add(new InterfaceLayout { SourceUrl = l });
            }
            switch (postFormat)
            {
                case "EstimateScale":
                    for (int i=0;i<post.Estimates.Count;i++)
                    {
                        post.Estimates[i].Characteristic = model.EstimatesScale[i].Characteristic;
                    }
                    break;
                case "EstimateVoting":
                    for (int i = 0; i < post.Estimates.Count; i++)
                    {
                        post.Estimates[i].Characteristic = model.EstimatesVoting[i].Characteristic;
                    }
                    break;
                case "EstimateRanging":
                    for (int i = 0; i < post.Estimates.Count; i++)
                    {
                        post.Estimates[i].Characteristic = model.EstimatesRanging[i].Characteristic;
                    }
                    break;
            }
            _postService.Update(post).Wait();
            return RedirectToAction("OpenPostById", "Post", new { id = post.Id });
        }
        public IActionResult DeletePost(bool confirm, int id)
        {
            if (User.IsInRole("admin"))
            {
                if (confirm)
                {
                    var post = _postService.GetPostById(id);
                    string folder = Path.Combine(_webHost.WebRootPath, "Interfaces");
                    DirectoryInfo directoryInfo = new DirectoryInfo(folder);
                    var files = directoryInfo.GetFiles();
                    var removingFiles = post.InterfaceLayouts.Select(l => l.SourceUrl).Where(url => files.Select(f=>f.Name).Contains(url));
                    FileInfo fileInf;
                    foreach (var file in removingFiles)
                    {
                        fileInf = new FileInfo(Path.Combine(folder,file));
                        if (fileInf.Exists)
                            fileInf.Delete();
                    }
                    _postService.Delete(post);
                }
            }
            return RedirectToAction("MainPage");
        }
        public IActionResult CreateNewPost(NewPostViewModel model = null)
        {
            if (model == null)
            {
                model = new NewPostViewModel();
            }
            if (model.InterfaceLayoutsSrc == null)
                model.InterfaceLayoutsSrc = new List<string>();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> PlacePost(NewPostViewModel model)
        {
            var userId = _userManager.GetUserId(User);
            var user = _userManager.FindByIdAsync(userId).Result;
            if (model.FormFiles != null)
            {
                var assessment = new AutoAssessmentController(_userManager);
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
            switch (model.EstimateFormat)
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


        private PostViewModel CreatePostViewModel(int id)
        {
            var post = _postService.GetPostById(id);
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
                Estimates = post.Estimates,
                EstimatesScale = new List<EstimateScale>(),
                EstimatesVoting = new List<EstimateVoting>(),
                EstimatesRanging = new List<EstimateRanging>(),
                EstimatesResult = new EstimatesResultViewModel(),
                InterfaceLayouts = post.InterfaceLayouts
            };
            if (post.Estimates.Any() && post.Estimates[0].Discriminator != null)
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
                        string json = JsonConvert.SerializeObject(model.EstimatesVoting, Formatting.Indented,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                        }
                        );
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
            if (count == 0) return 0;
            avg += scale.Count_1 * 1;
            avg += scale.Count_2 * 2;
            avg += scale.Count_3 * 3;
            avg += scale.Count_4 * 4;
            avg += scale.Count_5 * 5;
            return Math.Round(avg / count, 1);
        }

    }
}
