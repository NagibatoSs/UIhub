using Microsoft.AspNetCore.Mvc;
using UIhub.Data;
using UIhub.Models.ViewModels;

namespace UIhub.Controllers
{
    public class UserController: Controller
    {
        private readonly IUser _userService;
        private readonly IPost _postService;
        private readonly IPostReply _replyService;
        private readonly IAnalysis _analysisService;
        public UserController(IUser userService, IPost postService, IPostReply replyService, IAnalysis analysisService)
        {
            _userService = userService;
            _postService = postService;
            _replyService = replyService;
            _analysisService = analysisService;
        }
        public IActionResult OpenUserProfile(string id)
        {
            var user = _userService.GetUserById(id);
            var userVM = new UserViewModel();
            userVM.UserName = user.UserName;
            userVM.Email = user.Email;
            userVM.Reputation = user.Reputation;
            userVM.Points = user.Points;
            userVM.Rank = user.Rank;
            userVM.Posts = _postService.GetAllPosts().Where(p => p.Author.Id == id).ToList();
            userVM.PostReplies = _replyService.GetUserPostReplies(user.Id).ToList();
            userVM.Analyses = _analysisService.GetUserAnalyses(id).ToList();
            return View(userVM);
        }
        public IActionResult OpenAnalysis(int id)
        {
            var analysis = _analysisService.GetAnalysisById(id);
            if (analysis == null) return NotFound();
            return View(analysis);
        }
    }
}
