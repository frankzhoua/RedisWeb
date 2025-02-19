using Microsoft.EntityFrameworkCore;
using redis.WebAPi.Model;

namespace redis.WebAPi.Repository.AppDbContext{
    public class BenchmarkContent : DbContext
    {
        public BenchmarkContent(DbContextOptions<BenchmarkContent> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<BenchmarkRequestModel>()
                 .HasKey(p => p.Id); 
        }
        public DbSet<BenchmarkResultData> BenchmarkResultData { get; set; }
        public DbSet<BenchmarkResultData> BenchmarkFinalData { get; set; }
        public DbSet<BenchmarkRequestModel> BenchmarkRequest { get; set; }
        public DbSet<BenchmarkRequestModel> BenchmarkQueue { get; set; }
    }
}