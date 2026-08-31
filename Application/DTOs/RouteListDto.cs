using TrailHub.API.Domain.Enums;

namespace TrailHub.API.Application.DTOs
{
    public class RouteListDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public string? AuthorAvatarUrl { get; set; }
        public DifficultyLevel Difficulty { get; set; }
        public string Region { get; set; } = string.Empty;
        public decimal DistanceKm { get; set; }
        public string? CoverPhotoUrl { get; set; }
        public string ShortDescription { get; set; } = string.Empty; // Первые 150 символов
    }
}
