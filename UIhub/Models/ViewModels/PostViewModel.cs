using Microsoft.AspNetCore.Identity.UI.Services;

namespace UIhub.Models.ViewModels
{
    public class PostViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Views {  get; set; }
        public int EstimateCount { get; set; }
        public string Created { get; set; }
        public User Author { get; set; }
        public AutoAssessmentResult AutoAssessment { get; set; }
        public IEnumerable<PostReply> Replies { get; set; }
        public IEnumerable<Estimate> Estimates { get; set; }
        public IEnumerable<InterfaceLayout> InterfaceLayouts { get; set; }
    }
}
