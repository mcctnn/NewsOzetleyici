using Microsoft.EntityFrameworkCore;
using NewsOzetleyici.Core.Entities;

namespace NewsOzetleyici.Data.Context;
public class NewsDbContext : DbContext
{
    public NewsDbContext(DbContextOptions<NewsDbContext> options) : base(options)
    {
    }

    public DbSet<News> News { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Summary> Summaries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // News Configuration
        modelBuilder.Entity<News>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.Url)
                .IsRequired()
                .HasMaxLength(2000);

            entity.Property(e => e.Content)
                .IsRequired();

            entity.HasOne(e => e.Category)
                .WithMany(c => c.News)
                .HasForeignKey(e => e.CategoryId);

            entity.HasIndex(e => e.Url).IsUnique();
        });

        // Category Configuration
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(e => e.Name).IsUnique();
        });

        // Summary Configuration
        modelBuilder.Entity<Summary>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.SummaryText)
                .IsRequired();

            entity.HasOne(e => e.News)
                .WithMany(n => n.Summaries)
                .HasForeignKey(e => e.NewsId);
        });

        // Seed Data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Default Categories
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Genel", Description = "Genel haberler", Color = "#3f51b5" },
            new Category { Id = 2, Name = "Teknoloji", Description = "Teknoloji haberleri", Color = "#4caf50" },
            new Category { Id = 3, Name = "Spor", Description = "Spor haberleri", Color = "#ff9800" },
            new Category { Id = 4, Name = "Ekonomi", Description = "Ekonomi haberleri", Color = "#f44336" },
            new Category { Id = 5, Name = "Sağlık", Description = "Sağlık haberleri", Color = "#9c27b0" },
            new Category { Id = 6, Name = "Bilim", Description = "Bilim haberleri", Color = "#00bcd4" },
            new Category { Id = 7, Name = "Eğitim", Description = "Eğitim haberleri", Color = "#795548" }
        );
    }
}
