using NetTopologySuite.Geometries;
using TrailHub.API.Domain.Enums;

namespace TrailHub.API.Domain.Entities
{
    public class Route
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public User Author { get; set; } = null!; // Навигационное свойство

        public string Title { get; set; } = string.Empty;
        public DifficultyLevel Difficulty { get; set; }

        // В PostgreSQL это будет тип text[]
        public string[] Tags { get; set; } = Array.Empty<string>();
        public string[] Season { get; set; } = Array.Empty<string>();

        public string Description { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public decimal DistanceKm { get; set; }

        public string? CoverPhotoUrl { get; set; } // Для каталога
        public bool IsPublished { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        // PostGIS Geometry
        public LineString TrackGeometry { get; set; } = null!;

        // Массив высот (в PostgreSQL numeric[])
        public decimal[] ElevationProfile { get; set; } = Array.Empty<decimal>();
    }
}
