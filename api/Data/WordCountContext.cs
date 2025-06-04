using Microsoft.EntityFrameworkCore;
using api.Models;

namespace api.Data
{
    public class WordCountContext : DbContext
    {
        public WordCountContext(DbContextOptions<WordCountContext> options) : base(options)
        {
        }

        public DbSet<WordCount> WordCounts { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<WordCount>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.FileName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Word).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Count).IsRequired();
                entity.Property(e => e.FileUploadTime).IsRequired();

                entity.HasIndex(e => e.FileName);
                entity.HasIndex(e => e.FileUploadTime);
            });
        }
    }
}