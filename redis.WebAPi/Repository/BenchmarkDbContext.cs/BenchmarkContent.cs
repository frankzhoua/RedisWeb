using Microsoft.EntityFrameworkCore;
using redis.WebAPi.Model;
using redis.WebAPi.Model.BenchmarkModel;

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
                .ToTable("BenchmarkRequest", schema: "dbo")
                .HasKey(p => p.Id);


            // BenchmarkResultData ӳ�䵽 BenchmarkResultData
            modelBuilder.Entity<BenchmarkResultData>()
                .ToTable("BenchmarkResultData", schema: "dbo");

            modelBuilder.Entity<BenchmarkFinalDataModel>()
                .ToTable("BenchmarkFinalData", schema: "dbo");

            modelBuilder.Entity<BenchmarkQueueDataModel>()
               .ToTable("BenchmarkQueue", schema: "dbo");

        }
        public DbSet<BenchmarkResultData> BenchmarkResultData { get; set; }
        public DbSet<BenchmarkFinalDataModel> BenchmarkFinalData { get; set; }
        public DbSet<BenchmarkRequestModel> BenchmarkRequest { get; set; }
        public DbSet<BenchmarkQueueDataModel> BenchmarkQueue { get; set; }
    }
}