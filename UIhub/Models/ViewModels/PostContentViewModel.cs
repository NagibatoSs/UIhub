namespace UIhub.Models.ViewModels
{
    public class PostContentViewModel
    {
        public int Id { get; set; }
        public PostViewModel PostViewModel { get; set; }
        public NewReplyViewModel NewReplyModel { get; set; }
        //public List<InputEstimateScaleViewModel> InputsEstimatesScale { get; set; }
        public List<NewScaleEstimateViewModel> NewScaleEstimateViewModel { get; set; }
        public List<NewVotingEstimateViewModel> NewVotingEstimateViewModel { get; set; }
        public List<NewRangingEstimateViewModel> NewRangingEstimateViewModel { get; set; }
        public string CurrentUserId { get; set; }
        public bool IsPostEstimatedByUser { get; set; }
    }
}
