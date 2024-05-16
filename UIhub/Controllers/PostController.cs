using Microsoft.AspNetCore.Mvc;
using UIhub.Data;
using UIhub.Models;
using UIhub.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Reflection.PortableExecutable;
using System.Security.Claims;
using UIhub.Service;
using System.Text;

namespace UIhub.Controllers
{
    public class PostController : Controller
    {
        private readonly IPost _postService;
        private readonly UserManager<User> _userManager;
        private int scaleObjectsCount = 5;

        public PostController(IPost postService, UserManager<User> userManager, IEstimate estimateService)
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

        public IActionResult OpenPostById(int id)
        {
            if (id == 0) //ошибка если ссылка некорректна
                return View();
            var postModel = CreatePostViewModel(id);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //var user = _userManager.FindByIdAsync(userId).Result;
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
        public async Task<IActionResult> PlacePost(NewPostViewModel model)
        {
            var userId = _userManager.GetUserId(User);
            //var userId = "8ff7a4c5-33b3-4332-a4d4-979ba86ec589";
            var user = _userManager.FindByIdAsync(userId).Result;
            var post = BuildPost(model, user);
            _postService.Create(post).Wait();
            //сюда юзер рейтинг манипуляции
            return RedirectToAction("OpenPostById", "Post", new { id = post.Id });
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
            if (model.EstimateFormat == "scale")
            {
                foreach (var estimate in model.EstimatesScale)
                {
                    if (estimate.Characteristic == null) continue;
                    var scale = new EstimateScale { Characteristic = estimate.Characteristic, Count_1=0, Count_2=0, Count_3 =0, Count_4=0, Count_5=0 };
                    post.Estimates.Add(scale);
                }
            }
            if (model.EstimateFormat == "voting")
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
            if (model.EstimateFormat == "ranging")
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
                InterfaceLayouts = post.InterfaceLayouts
            };
            if (post.Estimates[0].Discriminator != null)
            {
                switch (post.Estimates[0].Discriminator)
                {
                    case "EstimateScale":
                        foreach (var item in post.Estimates)
                            model.EstimatesScale.Add(item as EstimateScale);
                        break;
                    case "EstimateVoting":
                        foreach (var item in post.Estimates)
                            model.EstimatesVoting.Add(item as EstimateVoting);
                        break;
                    case "EstimateRanging":
                        foreach (var item in post.Estimates)
                            model.EstimatesRanging.Add(item as EstimateRanging);
                        break;
                }
            }
            return model;
        }


       

    }
}
