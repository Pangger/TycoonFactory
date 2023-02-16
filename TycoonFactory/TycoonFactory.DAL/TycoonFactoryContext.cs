using Microsoft.EntityFrameworkCore;
using TycoonFactory.DAL.Entities;

namespace TycoonFactory.DAL;

public class TycoonFactoryContext : DbContext
{
    public DbSet<Android> Androids { get; set; }
    public DbSet<Activity> Activities { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase(databaseName: "TycoonFactoryDB");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActivityAndroid>()
             .HasKey(aa => new {aa.ActivityId, aa.AndroidId});
        
        modelBuilder.Entity<ActivityAndroid>()
            .HasOne(aa => aa.Activity)
            .WithMany(a => a.Androids)
            .HasForeignKey(aa => aa.ActivityId);
        
        modelBuilder.Entity<ActivityAndroid>()
            .HasOne(aa => aa.Android)
            .WithMany(a => a.Activities)
            .HasForeignKey(aa => aa.AndroidId);
    }
}