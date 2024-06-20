using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SolarWatch.Model;

namespace SolarWatch.Data;

public class SolarWatchContext : IdentityDbContext<IdentityUser, IdentityRole, string>
{
    private readonly IConfiguration _configuration;
    
    public DbSet<Sunrise> SunriseList { get; set; }
    public DbSet<Sunset> SunsetList { get; set; }
    public DbSet<City> Cities { get; set; }

    public SolarWatchContext(IConfiguration configuration, DbContextOptions<SolarWatchContext> options) : base(options)
    {
        _configuration = configuration;
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(_configuration["SqlConnectionString"]);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Sunrise>()
            .HasIndex(s => new { s.CityId, s.SunriseTime })
            .IsUnique();
        
        modelBuilder.Entity<Sunrise>()
            .Property(sR => sR.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Sunset>()
            .HasIndex(s => new { s.CityId, s.SunsetTime })
            .IsUnique();
        
        modelBuilder.Entity<Sunset>()
            .Property(sS => sS.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<City>()
            .HasIndex(c => new { c.Country, c.Name })
            .IsUnique();
        
        modelBuilder.Entity<City>()
            .Property(c => c.Id)
            .ValueGeneratedOnAdd();
    }
}