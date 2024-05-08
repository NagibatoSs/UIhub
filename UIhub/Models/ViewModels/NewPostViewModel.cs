namespace UIhub.Models.ViewModels
{
    public class NewPostViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public int Views { get; set; }
        public int EstimateCount { get; set; }
        public bool IsTop { get; set; }
        public AutoAssessmentResult AutoAssessment { get; set; }
        public List<string> InterfaceLayoutsSrc { get; set; }
        public List<Estimate> Estimates { get; set; }
        public User Author { get; set; }
    }
}
