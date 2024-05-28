namespace UIhub.Models.ViewModels
{
    public class NewPostViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public int EstimateCount { get; set; }
        public bool IsTop { get; set; }
        public AutoAssessmentResult AutoAssessment { get; set; }
        public List<string> InterfaceLayoutsSrc { get; set; }
        public List<IFormFile> ImgFormFiles { get; set; }
        public List<EstimateScale> EstimatesScale { get; set; }
        public List<EstimateVoting> EstimatesVoting { get; set; }
        public List<EstimateRanging> EstimatesRanging { get; set; }
        public string EstimateFormat { get; set; }
        public User Author { get; set; }
        public List<IFormFile> FormFiles { get; set; }
    }
}
