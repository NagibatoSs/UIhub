namespace UIhub.Models.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public int Reputation { get; set; }
        public int Points { get; set; }
        public UserRank Rank { get; set; }
        public List<Post> Posts { get; set; }
        public List<PostReply> PostReplies { get; set; }
    }
}
