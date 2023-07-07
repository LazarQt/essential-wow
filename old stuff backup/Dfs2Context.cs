using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace essential_wow;

public partial class Dfs2Context : DbContext
{
    public Dfs2Context() { }

    public Dfs2Context(DbContextOptions<Dfs2Context> options)
        : base(options) { }

    public virtual DbSet<Block> Blocks { get; set; }

    public virtual DbSet<Dungeon> Dungeons { get; set; }

    public virtual DbSet<Entry> Entries { get; set; }

    public virtual DbSet<Map> Maps { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        =>
        optionsBuilder.UseSqlite("Data Source=dfs2.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Block>(entity =>
        {
            entity.HasOne(d => d.Map).WithMany(p => p.Blocks).HasForeignKey(d => d.MapId);
        });

        modelBuilder.Entity<Dungeon>(entity =>
        {
            entity.HasKey(e => e.Name);
        });

        modelBuilder.Entity<Entry>(entity =>
        {
            entity.ToTable("Entry");

            // entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Block).WithMany(p => p.Entries).HasForeignKey(d => d.BlockId);
        });

        modelBuilder.Entity<Map>(entity =>
        {
            entity
                .HasOne(d => d.DungeonNavigation)
                .WithMany(p => p.Maps)
                .HasForeignKey(d => d.Dungeon)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Dungeon>().Navigation(e => e.Maps).AutoInclude();
        modelBuilder.Entity<Map>().Navigation(e => e.Blocks).AutoInclude();
        modelBuilder.Entity<Block>().Navigation(e => e.Entries).AutoInclude();

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
