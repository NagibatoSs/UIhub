namespace UIhub.Models
{
    public class AnalysisCriteria
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Recommendation { get; set; }
        public double? ThresholdValue { get; set; }
        public string StandardReference { get; set; }

        public virtual List<CriteriaResult> Results { get; set; } = new();
    }
}
