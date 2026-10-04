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

        modelBuilder.Entity<Country>().HasData(
            new Country { Code = "AT", Name = "Austria", IsEuropeanUnionMember = true },
            new Country { Code = "BE", Name = "Belgium", IsEuropeanUnionMember = true },
            new Country { Code = "BG", Name = "Bulgaria", IsEuropeanUnionMember = true },
            new Country { Code = "HR", Name = "Croatia", IsEuropeanUnionMember = true },
            new Country { Code = "CY", Name = "Cyprus", IsEuropeanUnionMember = true },
            new Country { Code = "CZ", Name = "Czechia", IsEuropeanUnionMember = true },
            new Country { Code = "DK", Name = "Denmark", IsEuropeanUnionMember = true },
            new Country { Code = "EE", Name = "Estonia", IsEuropeanUnionMember = true },
            new Country { Code = "FI", Name = "Finland", IsEuropeanUnionMember = true },
            new Country { Code = "FR", Name = "France", IsEuropeanUnionMember = true },
            new Country { Code = "DE", Name = "Germany", IsEuropeanUnionMember = true },
            new Country { Code = "GR", Name = "Greece", IsEuropeanUnionMember = true },
            new Country { Code = "HU", Name = "Hungary", IsEuropeanUnionMember = true },
            new Country { Code = "IE", Name = "Ireland", IsEuropeanUnionMember = true },
            new Country { Code = "IT", Name = "Italy", IsEuropeanUnionMember = true },
            new Country { Code = "LV", Name = "Latvia", IsEuropeanUnionMember = true },
            new Country { Code = "LT", Name = "Lithuania", IsEuropeanUnionMember = true },
            new Country { Code = "LU", Name = "Luxembourg", IsEuropeanUnionMember = true },
            new Country { Code = "MT", Name = "Malta", IsEuropeanUnionMember = true },
            new Country { Code = "NL", Name = "Netherlands", IsEuropeanUnionMember = true },
            new Country { Code = "PL", Name = "Poland", IsEuropeanUnionMember = true },
            new Country { Code = "PT", Name = "Portugal", IsEuropeanUnionMember = true },
            new Country { Code = "RO", Name = "Romania", IsEuropeanUnionMember = true },
            new Country { Code = "SK", Name = "Slovakia", IsEuropeanUnionMember = true },
            new Country { Code = "SI", Name = "Slovenia", IsEuropeanUnionMember = true },
            new Country { Code = "ES", Name = "Spain", IsEuropeanUnionMember = true },
            new Country { Code = "SE", Name = "Sweden", IsEuropeanUnionMember = true });
    }
}
