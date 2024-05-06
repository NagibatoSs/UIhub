namespace UIhub.Models
{
    public class PostReply
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime Created { get; set; }
        public int LikesCount { get; set; }
        public virtual User Author { get; set; }
        public virtual Post Post { get; set; }
    }
}
