namespace UIhub.Models
{
    public class EstimateRanging : Estimate
    {
        public virtual List<RangingSequence>? Sequences { get; set; }
        public virtual List<RangingObject> RangingObjects { get; set; }
        //public string NumbersOrder { get; set; }
    }
}
