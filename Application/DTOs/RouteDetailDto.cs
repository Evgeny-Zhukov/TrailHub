using TrailHub.API.Domain.Enums;

namespace TrailHub.API.Application.DTOs
{
    public class RouteDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DifficultyLevel Difficulty { get; set; }
        public string[] Tags { get; set; } = Array.Empty<string>();
        public string[] Season { get; set; } = Array.Empty<string>();
        public string Region { get; set; } = string.Empty;
        public decimal DistanceKm { get; set; }
        public string? CoverPhotoUrl { get; set; }

        // Для карт на фронтенде (GeoJSON или упрощенный формат)
        public object TrackGeometry { get; set; } = null!;
        public decimal[] ElevationProfile { get; set; } = Array.Empty<decimal>();

        public string AuthorName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
