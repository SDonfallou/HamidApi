using bookShareBEnd.Database.Model;
using Microsoft.EntityFrameworkCore;

namespace bookShareBEnd.Database
{
  
    public class AppDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }
        public DbSet<Users> users { get; set; }
        public DbSet<Roles> roles { get; set; }
        public DbSet<Books> books { get; set; }
        public DbSet<BookLoan> bookLoan { get; set; }
        public DbSet<Likes> likes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>().HasKey(u => u.UserId);
            // Define other entity configurations if needed
            base.OnModelCreating(modelBuilder);
        }
    }
}
