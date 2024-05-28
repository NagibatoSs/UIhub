namespace UIhub.Models
{
    public class PostReplyLike
    {
        public int Id { get; set; }
        public virtual PostReply PostReply { get; set; }
        public virtual User User { get; set; }
    }
}
