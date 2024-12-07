using Microsoft.EntityFrameworkCore;
using RENHelpers.ExampleProject.Data;

namespace RENHelpers.ExampleProject.Database;

public class RENDbContext : DbContext
{
    public RENDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Side> Sides { get; set; }
    public DbSet<Test> Tests { get; set; }
    public DbSet<TestDescription> TestDescriptions { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ConfigureUserEntities(modelBuilder);
    }


    private void ConfigureUserEntities(ModelBuilder builder)
    {
        builder.Entity<User>()
            .HasOne(_ => _.Side)
            .WithMany(_ => _.Users)
            .HasForeignKey(_ => _.SideId);

        builder.Entity<Test>()
            .HasOne(_ => _.Side)
            .WithOne(_ => _.Test)
            .HasForeignKey<Test>(_ => _.SideId);

        builder.Entity<Test>()
            .HasMany(_ => _.TestDescriptions)
            .WithOne(_ => _.Test)
            .HasForeignKey(_ => _.TestId);
    }
}