namespace UIhub.Models
{
    public class CriteriaIssue
    {
        public int Id { get; set; }
        public int CriteriaResultId { get; set; }
        public string Message { get; set; }

        public virtual CriteriaResult CriteriaResult { get; set; }
    }
}
