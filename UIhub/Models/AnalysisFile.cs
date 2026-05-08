namespace UIhub.Models
{
    public class AnalysisFile
    {
        public int Id { get; set; }
        public int AnalysisId { get; set; }
        public string FileName { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        public string FilePath { get; set; }

        public virtual Analysis Analysis { get; set; }
        public virtual List<CriteriaResult> Results { get; set; } = new();
    }
}
