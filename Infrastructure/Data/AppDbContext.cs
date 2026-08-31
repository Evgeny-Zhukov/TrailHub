using Microsoft.EntityFrameworkCore;
using TrailHub.API.Domain.Entities;

namespace TrailHub.API.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Domain.Entities.Route> Routes => Set<Domain.Entities.Route>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка PostGIS для LineString
            modelBuilder.Entity<Domain.Entities.Route>()
                .Property(r => r.TrackGeometry)
                .HasColumnType("geometry(LineString, 4326)"); // SRID 4326 = WGS84

            // Настройка массивов (опционально, EF Core обычно понимает это сам для Npgsql)
            modelBuilder.Entity<Domain.Entities.Route>()
                .Property(r => r.Tags)
                .HasColumnType("text[]");

            modelBuilder.Entity<Domain.Entities.Route>()
                .Property(r => r.ElevationProfile)
                .HasColumnType("numeric[]");
        }
    }
}
