using Microsoft.EntityFrameworkCore;
using TrailHub.API.Application.DTOs;
using TrailHub.API.Application.Interfaces;
using TrailHub.API.Domain.Enums;
using TrailHub.API.Infrastructure.Data;

namespace TrailHub.API.Application.Services
{
    public class RouteService : IRouteService
    {
        private readonly AppDbContext _context;

        public RouteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RouteListDto>> GetPublishedRoutesAsync(string? region, DifficultyLevel? difficulty)
        {
            var query = _context.Routes
                .Include(r => r.Author) // Подгружаем автора для имени и аватара
                .Where(r => r.IsPublished);

            if (!string.IsNullOrEmpty(region))
                query = query.Where(r => r.Region == region);

            if (difficulty.HasValue)
                query = query.Where(r => r.Difficulty == difficulty.Value);

            // Проецируем Entity в легкий DTO прямо в БД (эффективный SQL)
            return await query.Select(r => new RouteListDto
            {
                Id = r.Id,
                Title = r.Title,
                AuthorName = r.Author.Name,
                AuthorAvatarUrl = r.Author.AvatarUrl,
                Difficulty = r.Difficulty,
                Region = r.Region,
                DistanceKm = r.DistanceKm,
                CoverPhotoUrl = r.CoverPhotoUrl,
                ShortDescription = r.Description.Length > 150 ? r.Description.Substring(0, 150) + "..." : r.Description
            }).ToListAsync();
        }

        public async Task<RouteDetailDto?> GetRouteByIdAsync(int id)
        {
            var route = await _context.Routes
                .Include(r => r.Author)
                .FirstOrDefaultAsync(r => r.Id == id && r.IsPublished);

            if (route == null) return null;

            return new RouteDetailDto
            {
                Id = route.Id,
                Title = route.Title,
                Description = route.Description,
                Difficulty = route.Difficulty,
                Tags = route.Tags,
                Season = route.Season,
                Region = route.Region,
                DistanceKm = route.DistanceKm,
                CoverPhotoUrl = route.CoverPhotoUrl,
                TrackGeometry = route.TrackGeometry, // EF Core + NetTopologySuite автоматически сериализуют это в GeoJSON для API!
                ElevationProfile = route.ElevationProfile,
                AuthorName = route.Author.Name,
                CreatedAt = route.CreatedAt
            };
        }
    }
}
