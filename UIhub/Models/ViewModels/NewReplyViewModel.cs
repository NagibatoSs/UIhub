namespace UIhub.Models.ViewModels
{
    public class NewReplyViewModel
    {
        public Post Post { get; set; }
        public User Author { get; set; }
        public string Content { get; set; }
        public DateTime Created { get; set; }
        public int LikesCount { get; set; }
    }
}
