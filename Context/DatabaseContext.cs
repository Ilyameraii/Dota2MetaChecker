using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Context;

/// <summary>
/// Контекст базы данных Entity Framework Core для работы с SQL Server
/// </summary>
public class DatabaseContext : DbContext
{
    /// <summary>
    /// Набор данных для обновлений метаданных
    /// </summary>
    public DbSet<MetaUpdate> MetaUpdates { get; set; }
    
    /// <summary>
    /// Набор данных для статистики персонажей
    /// </summary>
    public DbSet<HeroStat> HeroesStats { get; set; }

    /// <summary>
    /// Настройка модели данных
    /// </summary>
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
