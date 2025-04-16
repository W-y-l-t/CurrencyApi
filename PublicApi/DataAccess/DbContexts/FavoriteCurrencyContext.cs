using Fuse8.BackendInternship.PublicApi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fuse8.BackendInternship.PublicApi.DataAccess.DbContexts;

public class FavoriteCurrencyContext : DbContext
{
    public FavoriteCurrencyContext(DbContextOptions<FavoriteCurrencyContext> options)
        : base(options)
    { }

    public DbSet<FavoriteCurrency> FavoriteCurrencies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("user");

        modelBuilder.Entity<FavoriteCurrency>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.HasIndex(e => new { e.Currency, e.BaseCurrency }).IsUnique();
        });
    }
}