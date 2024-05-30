using Microsoft.EntityFrameworkCore;
using UIhub.Data;
using UIhub.Models;

namespace UIhub.Service
{
    public class PostReplyService: IPostReply
    {
        private readonly ApplicationDbContext _context;
        public PostReplyService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task Create(PostReply postReply)
        {
            _context.Add(postReply);
            await _context.SaveChangesAsync();
        }
        public async Task Update(PostReply postReply)
        {
            _context.Update(postReply);
            await _context.SaveChangesAsync();
        }
        public PostReplyLike GetPostReplyLikeById(int replyId, string userId)
        {
            return _context.PostReplyLikes
                 .Where(like => like.PostReply.Id == replyId && like.User.Id == userId)
                 .FirstOrDefault();
        }
        public IEnumerable<PostReply> GetAllRepliesByPostId(int postId)
        {
            return _context.PostTextReplies
                .Where(reply => reply.Post.Id == postId)
                .Include(reply => reply.Post)
                .Include(u => u.PostReplyLikes)
                .Include(reply => reply.Author);
        }

        public IEnumerable<PostReply> GetAllPostsReplies()
        {
            return _context.PostTextReplies
                .Include(post => post.Author)
                .Include(post => post.Post);
        }

        public PostReply GetPostReplyById(int id)
        {
            IQueryable<PostReply> postReplies = _context.PostTextReplies
                .Where(reply => reply.Id == id)
                .Include(post => post.Author)
                    .ThenInclude(a => a.Rank)
                .Include(post => post.Post)
                .Include(u => u.PostReplyLikes);
            //.Include(post => post.Replies)
            //   .ThenInclude(r => r.Author)
            //.Include(post => post.Author);
            return postReplies.FirstOrDefault();
        }

        public IEnumerable<PostReply> GetUserPostReplies(string userId)
        {
            return _context.PostTextReplies
                .Where(rep => rep.Author.Id == userId)
                .Include(reply => reply.Author)
                .Include(reply => reply.Post);
        }
    }
}
