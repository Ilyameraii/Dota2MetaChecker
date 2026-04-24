using Entities.Classes;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Context;

public class DatabaseContext : DbContext
{
    
    public DbSet<MetaUpdate> MetaUpdates { get; set; }
    
    public DbSet<HeroStat> HeroesStats { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<HeroStat>()
            .HasOne(hs => hs.MetaUpdate)
            .WithMany(mu => mu.HeroStats)
            .HasForeignKey(hs => hs.MetaUpdateId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=localhost;Database=Dota2MetaChecker;TrustServerCertificate=True;Trusted_Connection=True;");
    }
}
