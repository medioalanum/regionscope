using Microsoft.EntityFrameworkCore;
using RegionScope.Models;

namespace RegionScope.Data;

public sealed class RegionScopeDbContext(DbContextOptions<RegionScopeDbContext> options)
    : DbContext(options)
{
    public DbSet<Country> Countries => Set<Country>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(country => country.Code);
            entity.Property(country => country.Code).HasMaxLength(2);
            entity.Property(country => country.Name).HasMaxLength(100).IsRequired();
            entity.Property(country => country.IsEuropeanUnionMember).IsRequired();
        });
    }
}
