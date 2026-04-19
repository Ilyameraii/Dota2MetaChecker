using System;
using System.Collections.Generic;
using Context.Models;
using Microsoft.EntityFrameworkCore;

namespace Context.Data;

public partial class DatabaseContext : DbContext
{
    public DatabaseContext()
    {
    }

    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<HeroStat> HeroStats { get; set; }

    public virtual DbSet<MetaUpdate> MetaUpdates { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=Dota2MetaChecker;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HeroStat>(entity =>
        {
            entity.ToTable("HeroStat");

            entity.Property(e => e.HeroRank).HasMaxLength(25);
            entity.Property(e => e.HeroRole).HasMaxLength(25);

            entity.HasOne(d => d.MetaUpdate).WithMany(p => p.HeroStats)
                .HasForeignKey(d => d.MetaUpdateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HeroStat_MetaUpdate");
        });

        modelBuilder.Entity<MetaUpdate>(entity =>
        {
            entity.ToTable("MetaUpdate");

            entity.Property(e => e.DateTime).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
