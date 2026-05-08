namespace UIhub.Models
{
    public class Analysis
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual User User { get; set; }
        public virtual List<AnalysisFile> Files { get; set; } = new();
    }
}
