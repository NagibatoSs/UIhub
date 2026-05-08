namespace UIhub.Models
{
    public class CriteriaResult
    {
        public int Id { get; set; }
        public int AnalysisFileId { get; set; }
        public int AnalysisCriteriaId { get; set; }
        public string MetricValue { get; set; }
        public bool IsOk { get; set; }
        public string? ResultImagePath { get; set; } 

        public virtual AnalysisFile AnalysisFile { get; set; }
        public virtual AnalysisCriteria AnalysisCriteria { get; set; }
        public virtual List<CriteriaIssue> Issues { get; set; } = new();
    }
}
