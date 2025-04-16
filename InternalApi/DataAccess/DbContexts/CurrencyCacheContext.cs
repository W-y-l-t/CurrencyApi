using Fuse8.BackendInternship.InternalApi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fuse8.BackendInternship.InternalApi.DataAccess.DbContexts;

public class CurrencyCacheContext : DbContext
{
    public CurrencyCacheContext(DbContextOptions<CurrencyCacheContext> options)
        : base(options)
    { }

    public DbSet<CurrencyCacheEntry> CurrencyCacheEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("cur");

        modelBuilder.Entity<CurrencyCacheEntry>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.BaseCurrency, e.Currency, e.CachedAt, e.TargetDate }).IsUnique();
            entity.Property(e => e.CachedAt).HasColumnType("timestamp with time zone");
        });
    }
}