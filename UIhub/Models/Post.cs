namespace UIhub.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public int Views { get; set; }
        public int EstimateCount { get; set; }
        public bool IsTop { get; set; }
        public AutoAssessmentResult AutoAssessment { get; set; }
        public virtual List<InterfaceLayout> InterfaceLayouts { get; set; }
        public virtual List<PostReply> Replies { get; set; }
        public virtual List<Estimate> Estimates { get; set; }
        public virtual User Author { get; set; }
    }
}
