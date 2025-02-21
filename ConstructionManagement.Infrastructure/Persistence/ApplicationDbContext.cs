using ConstructionManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConstructionManagement.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectStage> ProjectStages { get; set; }
    public DbSet<ProjectCategory> ProjectCategories { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>()
            .HasKey(x => x.Id);
        
        modelBuilder.Entity<Project>()
            .HasOne(p => p.ProjectCategory)
            .WithMany(c => c.Projects)
            .HasForeignKey(p => p.ProjectCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
            
        modelBuilder.Entity<Project>()
            .HasOne(p => p.ProjectStage)
            .WithMany(s => s.Projects)
            .HasForeignKey(p => p.ProjectStageId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();

        modelBuilder.Entity<ProjectStage>().HasData(
            new ProjectStage { Id = 1, Name = "Concept" },
            new ProjectStage { Id = 2, Name = "Design & Documentation" },
            new ProjectStage { Id = 3, Name = "Pre-Construction" },
            new ProjectStage { Id = 4, Name = "Construction" }
        );
        
        modelBuilder.Entity<ProjectCategory>().HasData(
            new ProjectStage { Id = 1, Name = "Education" },
            new ProjectStage { Id = 2, Name = "Health" },
            new ProjectStage { Id = 3, Name = "Office" }
        );
        
        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Username = "Mili", Email = "mili@gmail.com", Password = "halomili"}
        );

    }
}
