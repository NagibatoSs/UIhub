namespace UIhub.Models
{
    public class UserPostEstimate
    {
        public int Id { get; set; }
        public virtual User User { get; set; }
        public virtual Post Post { get; set; }
        //public bool isUserEstimated { get; set; }
    }
}
