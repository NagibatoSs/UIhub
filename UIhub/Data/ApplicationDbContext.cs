using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UIhub.Models;

namespace UIhub.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostReply> PostTextReplies { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRank> UserRanks { get; set; }
        public DbSet<AutoAssessmentResult> AutoAssesmentResults { get; set; }
        public DbSet<InterfaceLayout> InterfaceLayouts { get; set; }
        public DbSet<Estimate> Estimates { get; set; }
        public DbSet<EstimateScale> EstimateScales { get; set; }
        public DbSet<EstimateVoting> EstimateVotings { get; set; }
        public DbSet<VotingObject> VotingObjects { get; set; }
        public DbSet<EstimateRanging> EstimateRangings { get; set; }
        public DbSet<RangingSequence> RangingSequences { get; set; }
        public DbSet<RangingObject> RangingObjects { get; set; }
        public DbSet<UserPostEstimate> UserPostEstimates {  get; set; }
        public DbSet<PostReplyLike> PostReplyLikes { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //builder.Entity<IdentityRole>().HasData(new IdentityRole
            //{
            //    Id = "1ba64fb1-faf0-424f-8a66-af45363d4c58",
            //    Name = "admin",
            //    NormalizedName = "ADMIN"
            //});

            //builder.Entity<User>().HasData(new User
            //{
            //    Id = "8ff7a4c5-33b3-4332-a4d4-979ba86ec589",
            //    UserName = "NagibatoSs",
            //    NormalizedUserName = "NAGIBATOSS",
            //    Email = "zhuravleva_02@mail.ru",
            //    EmailConfirmed = true,
            //    PasswordHash = new PasswordHasher<User>().HashPassword(null, "kjifhfdjljgkfdf.ott"),
            //    SecurityStamp = string.Empty,
            //    Reputation = 0,
            //    Points = 0
            //});

            //builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            //{
            //    RoleId = "1ba64fb1-faf0-424f-8a66-af45363d4c58",
            //    UserId = "8ff7a4c5-33b3-4332-a4d4-979ba86ec589"
            //});
        }
    }
}
