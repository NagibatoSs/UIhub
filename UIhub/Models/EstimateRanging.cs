namespace UIhub.Models
{
    public class EstimateRanging : Estimate
    {
        public virtual List<RangingSequence> Sequences { get; set; }
    }
}
