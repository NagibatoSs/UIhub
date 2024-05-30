using Microsoft.AspNetCore.Mvc;
using UIhub.Data;
using UIhub.Models;
using UIhub.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace UIhub.Controllers
{
    public class PostController : Controller
    {
        private readonly IPost _postService;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _webHost;
        private readonly IUser _userService;
        private readonly ViewModelsBuilder _modelBuilder;

        public PostController(IPost postService, UserManager<User> userManager, IWebHostEnvironment webHost, IUser userService)
        {
            _postService = postService;
            _userManager = userManager;
            _webHost = webHost;
            _userService = userService;
            _modelBuilder = new ViewModelsBuilder(_userManager,_webHost);
        }
        public IActionResult MainPage()
        {
            var allPosts = _postService.GetAllPosts();
            if (allPosts == null)
                return View();
            var posts = allPosts
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
            var post = _postService.GetPostById(id);
            var postModel = _modelBuilder.BuildPostViewModel(post);
            if (postModel.EstimatesVoting != null && postModel.EstimatesVoting.Any())
            {
                string json = JsonConvert.SerializeObject(postModel.EstimatesVoting, Formatting.Indented,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                }
                );
                ViewBag.Voting = json;
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _userManager.FindByIdAsync(userId).Result;
            var model = new PostContentViewModel
            {
                Id = id,
                PostViewModel = postModel,
                CurrentUserId = userId,
                IsPostEstimatedByUser = _postService.GetPostById(id).UserPostEstimates.Select(e => e.User).Contains(user)
        };
            return View(model);
        }
      
        [HttpPost]
        public IActionResult EditPost(int id)
        {
            var post = _postService.GetPostById(id);
            var model = _modelBuilder.BuildNewPostViewModel(post);
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> PlacePost(NewPostViewModel model)
        {
            var userId = _userManager.GetUserId(User);
            var user = _userManager.FindByIdAsync(userId).Result;
            if (model.IsTop)
            {
                model.IsTop = _userService.BuyTop(user.Id);
            }
            var post = _modelBuilder.BuildPost(model, user);
            _postService.Create(post).Wait();
            return RedirectToAction("OpenPostById", "Post", new { id = post.Id });
        }
      
        [HttpPost]
        public IActionResult UpdatePost(NewPostViewModel model)
        {
            var post = _postService.GetPostById(model.Id);
            post.Title = model.Title;
            post.Description = model.Description;
            var interf = post.InterfaceLayouts;
            post.InterfaceLayouts = new List<InterfaceLayout>();
            if (model.InterfaceLayoutsSrc != null)
            {
                for (int i = 0; i < model.InterfaceLayoutsSrc.Count(); i++)
                {
                    post.InterfaceLayouts[i] = new InterfaceLayout { SourceUrl = model.InterfaceLayoutsSrc[i], SourceType = interf[i].SourceType };
                }
            }
            string postFormat = post.Estimates[0].Discriminator;
            switch (postFormat)
            {
                case "EstimateScale":
                    FillEstimateCharacteristicToPost(post,model.EstimatesScale);
                    break;
                case "EstimateVoting":
                    FillEstimateCharacteristicToPost(post, model.EstimatesVoting);
                    break;
                case "EstimateRanging":
                    FillEstimateCharacteristicToPost(post, model.EstimatesRanging);
                    break;
            }
            _postService.Update(post).Wait();
            return RedirectToAction("OpenPostById", "Post", new { id = post.Id });
        }
        private void FillEstimateCharacteristicToPost<TEstimate>(Post post, List<TEstimate> estimates) where TEstimate:Estimate
        {
            for (int i = 0; i < post.Estimates.Count; i++)
            {
                post.Estimates[i].Characteristic = estimates[i].Characteristic;
            }
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
    }
}
