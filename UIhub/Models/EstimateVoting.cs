namespace UIhub.Models
{
    public class EstimateVoting: Estimate
    {
        public virtual List<VotingObject> VotingObjects { get; set; }
    }
}
