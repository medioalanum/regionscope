using Microsoft.EntityFrameworkCore;
using RegionScope.Models;

namespace RegionScope.Data;

public sealed class RegionScopeDbContext(DbContextOptions<RegionScopeDbContext> options)
    : DbContext(options)
{
    public DbSet<Country> Countries => Set<Country>();

    public DbSet<Indicator> Indicators => Set<Indicator>();

    public DbSet<Observation> Observations => Set<Observation>();

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

        modelBuilder.Entity<Indicator>(entity =>
        {
            entity.HasKey(indicator => indicator.Code);
            entity.Property(indicator => indicator.Code).HasMaxLength(50);
            entity.Property(indicator => indicator.Name).HasMaxLength(150).IsRequired();
            entity.Property(indicator => indicator.Unit).HasMaxLength(50).IsRequired();
        });

        modelBuilder.Entity<Indicator>().HasData(
            new Indicator { Code = "population", Name = "Population", Unit = "persons" },
            new Indicator { Code = "gdp_per_capita", Name = "GDP per capita", Unit = "EUR per person" },
            new Indicator { Code = "unemployment_rate", Name = "Unemployment rate", Unit = "percentage" });

        modelBuilder.Entity<Observation>(entity =>
        {
            entity.HasKey(observation => new { observation.CountryCode, observation.IndicatorCode, observation.Year });
            entity.Property(observation => observation.CountryCode).HasMaxLength(2);
            entity.Property(observation => observation.IndicatorCode).HasMaxLength(50);
            entity.Property(observation => observation.Value).HasPrecision(20, 6);
            entity.Property(observation => observation.Unit).HasMaxLength(50).IsRequired();
            entity.Property(observation => observation.Source).HasMaxLength(100).IsRequired();
            entity.HasIndex(observation => new { observation.IndicatorCode, observation.Year });
            entity.HasOne(observation => observation.Country).WithMany()
                .HasForeignKey(observation => observation.CountryCode).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(observation => observation.Indicator).WithMany(indicator => indicator.Observations)
                .HasForeignKey(observation => observation.IndicatorCode).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
