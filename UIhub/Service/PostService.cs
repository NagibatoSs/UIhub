using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Collections;
using UIhub.Data;
using UIhub.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace UIXtimate.Service
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

        public Post GetPostById(int id)
        {
            IQueryable<Post> posts = _context.Posts
                .Where(post => post.Id == id)
                .Include(post => post.Replies)
                   .ThenInclude(r => r.Author)
                .Include(post => post.Author)
                .Include(post => post.InterfaceLayouts)
                .Include(post => post.AutoAssessment)
                .Include(post => post.Estimates);
            return posts.FirstOrDefault();
        }
        public async Task Create(Post post)
        {
            _context.Add(post);
            await _context.SaveChangesAsync();
        }
    }
}
