using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using UIhub.Data;
using UIhub.Models;
using UIhub.Models.ViewModels;
using UIhub.Service;

namespace UIhub.Controllers
{
    public class PostReplyController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IPostReply _replyService;
        private readonly IPost _postService;
        private readonly IUser _userService;
        public PostReplyController(UserManager<User> userManager, IPostReply replyService, IPost postService, IUser userService)
        {
            _userManager = userManager;
            _replyService = replyService;
            _postService = postService;
            _userService = userService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SetLike(int id, bool isLike)
        {
            var reply = _replyService.GetPostReplyById(id);
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("OpenPostById", "Post", new { id = reply.Post.Id });
            var user = _userManager.FindByIdAsync(_userManager.GetUserId(User)).Result;
            if (user.Reputation < 2)
            {
                return RedirectToAction("OpenPostById", "Post", new { id = reply.Post.Id });
            }
            if (reply.PostReplyLikes == null)
                reply.PostReplyLikes = new List<PostReplyLike>();
            if (isLike)
            {
                _userService.IncreaseReputationForLike(reply.Author.Id);
                reply.LikesCount += 1;
                reply.PostReplyLikes.Add(new PostReplyLike { PostReply = reply, User = user });
                _replyService.Update(reply).Wait();
            }
            //else
            //{
            //    reply.LikesCount -= 1;
            //    reply.PostReplyLikes.Remove(_replyService.GetPostReplyLikeById(reply.Id,user.Id));
            //}
            return RedirectToAction("OpenPostById", "Post", new { id = reply.Post.Id });
        }
        [HttpPost]
        public IActionResult PlaceReply(PostContentViewModel model)
        {
            var modelReply = model.NewReplyModel;
            var userId = _userManager.GetUserId(User);
            var post = _postService.GetPostById(model.Id);
            //var userId = "8ff7a4c5-33b3-4332-a4d4-979ba86ec589";
            //var user = _userManager.FindByIdAsync(userId).Result;
            var user = _userService.GetUserById(userId);
            modelReply.Post = post;
            var reply = BuildReply(modelReply, user);
            _replyService.Create(reply).Wait();
            //сюда юзер рейтинг манипуляции
            return RedirectToAction("OpenPostById", "Post", new { id = post.Id });
        }
        private PostReply BuildReply(NewReplyViewModel model, User user)
        {
            var reply = new PostReply
            {
                Content = model.Content,
                Author = user,
                Post = model.Post,
                Created = DateTime.Now,
                LikesCount = 0,
                PostReplyLikes = new List<PostReplyLike>()
            };
            return reply;
        }
    }
}
