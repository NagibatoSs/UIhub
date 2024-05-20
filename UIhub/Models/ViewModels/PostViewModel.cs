using Microsoft.AspNetCore.Identity.UI.Services;

namespace UIhub.Models.ViewModels
{
    public class PostViewModel
    {
        public PostViewModel()
        {
            
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Views {  get; set; }
        public int EstimateCount { get; set; }
        public string Created { get; set; }
        public User Author { get; set; }
        public AutoAssessmentResult AutoAssessment { get; set; }
        public IEnumerable<PostReply> Replies { get; set; }
        public List<Estimate> Estimates { get; set; }
        public List<EstimateScale> EstimatesScale { get; set; }
        public List<EstimateVoting> EstimatesVoting { get; set; }
        public List<EstimateRanging> EstimatesRanging { get; set; }
        public IEnumerable<InterfaceLayout> InterfaceLayouts { get; set; }
        public EstimatesResultViewModel EstimatesResult { get; set; }
        public List<RangingEstimatePresenterViewModel> RangingEstimatesPresenter { get; set; }
    }
}
