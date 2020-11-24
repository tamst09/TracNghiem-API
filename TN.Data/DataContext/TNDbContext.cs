using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TN.Data.Config;
using TN.Data.DataContext.DbSeeding;
using TN.Data.Entities;

namespace TN.Data.DataContext
{
    public class TNDbContext : IdentityDbContext<AppUser,AppRole,int>
    {
        public TNDbContext(DbContextOptions options) : base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new CategoryConfig());
            modelBuilder.ApplyConfiguration(new ExamConfig());
            modelBuilder.ApplyConfiguration(new QuestionConfig());
            //modelBuilder.ApplyConfiguration(new AppRoleConfig());
            //modelBuilder.ApplyConfiguration(new AppUserConfig());
            modelBuilder.ApplyConfiguration(new HistoryConfig());         
            modelBuilder.ApplyConfiguration(new ResultConfig());
            modelBuilder.ApplyConfiguration(new FavoriteExamConfig());
            modelBuilder.ApplyConfiguration(new MailBoxConfig());

            modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("AppNetUserClaims");
            modelBuilder.Entity<IdentityUserRole<int>>().ToTable("AppNetUserRoles").HasKey(a => new { a.UserId, a.RoleId});
            modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("AppNetUserLogins").HasKey(a => new { a.LoginProvider, a.ProviderKey });
            modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("AppNetRoleClaims");
            modelBuilder.Entity<IdentityUserToken<int>>().ToTable("AppNetUserTokens").HasKey(a => new { a.UserId,a.LoginProvider,a.Name });
            
            modelBuilder.Entity<RefreshToken>(e => 
                {
                    e.ToTable("RefreshTokens").HasKey(t => t.TokenId);
                    e.HasOne(d => d.User).WithMany(p => p.RefreshTokens).HasForeignKey(d => d.UserId);
                });
            modelBuilder.Seed();
            
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<HistoryExam> HistoryExams { get; set; }
        public DbSet<FavoriteExam> FavoriteExams { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<MailBox> MailBoxes { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
