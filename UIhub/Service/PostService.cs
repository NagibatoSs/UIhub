using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Collections;
using UIhub.Data;
using UIhub.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace UIhub.Service
{
    public class PostService : IPost
    {
        private readonly ApplicationDbContext _context;
        public PostService(ApplicationDbContext context)
        {
            _context = context;
        }  

        public IEnumerable<Post> GetAllPosts()
        {
            return _context.Posts
                .Include(post => post.Replies)
                .Include(post => post.Author);
        }
        public async Task Create(Post post)
        {
            _context.Add(post);
            await _context.SaveChangesAsync();
        }
        public async Task Update(Post post)
        {
            _context.Update(post);
            await _context.SaveChangesAsync();
        }
        public void Delete(Post post)
        {
            _context.Posts.Remove(post);
            _context.SaveChanges();
        }
        public Post GetPostById(int id)
        {
            IQueryable<Post> posts = _context.Posts
                .Where(post => post.Id == id)
                .Include(post => post.Replies)
                   .ThenInclude(r => r.Author)
                .Include(post => post.Author)
                    .ThenInclude(a => a.Rank)
                .Include(post => post.InterfaceLayouts)
                .Include(post => post.AutoAssessment)
                .Include(post => post.Estimates)
                    .ThenInclude(est => (est as EstimateVoting).VotingObjects)
                .Include(post => post.Estimates)
                    .ThenInclude(est => (est as EstimateRanging).RangingObjects)
                .Include(post => post.Estimates)
                    .ThenInclude(est => (est as EstimateRanging).Sequences);
            return posts.FirstOrDefault();
        }
    }
}
