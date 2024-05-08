using Microsoft.AspNetCore.Mvc;
using UIhub.Data;
using UIhub.Models;
using UIhub.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

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

        public IActionResult OpenPostById(int id)
        {
            if (id == 0) //ошибка если ссылка некорректна
                return View();
            var postModel = CreatePostViewModel(id);
            var model = new PostContentViewModel
            {
                Id = id,
                PostViewModel = postModel
            };
            return View(model);
        }
        public IActionResult CreateNewPost(NewPostViewModel model = null)
        {
            // if (User.Identity.IsAuthenticated)
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
            //var userId = _userManager.GetUserId(User);
            var userId = "8ff7a4c5-33b3-4332-a4d4-979ba86ec589";
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

            //Estimates = new List<Estimate>(),
            

            foreach (var content in model.InterfaceLayoutsSrc)
            {
                var layoutSrc = new InterfaceLayout() { SourceUrl = content };
                post.InterfaceLayouts.Add(layoutSrc);
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
            return new PostViewModel
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
                InterfaceLayouts = post.InterfaceLayouts
            };
        }
    }
}
