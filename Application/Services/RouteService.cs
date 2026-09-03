using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using TrailHub.API.Application.DTOs;
using TrailHub.API.Infrastructure.Data;

namespace TrailHub.API.Application.Services;

public class RouteService : IRouteService
{
    private readonly AppDbContext _context;
    private readonly GeometryFactory _geometryFactory;

    public RouteService(AppDbContext context)
    {
        _context = context;
        // Создаем фабрику геометрии с SRID 4326 (WGS84)
        _geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);
    }

    public async Task<IEnumerable<RouteListDto>> GetPublishedRoutesAsync(string? region, Domain.Enums.DifficultyLevel? difficulty)
    {
        var query = _context.Routes
            .Include(r => r.Author)
            .Where(r => r.IsPublished);

        if (!string.IsNullOrEmpty(region))
            query = query.Where(r => r.Region == region);

        if (difficulty.HasValue)
            query = query.Where(r => r.Difficulty == difficulty.Value);

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
            ShortDescription = r.Description.Length > 150
                ? r.Description.Substring(0, 150) + "..."
                : r.Description
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
            TrackGeometry = route.TrackGeometry,
            ElevationProfile = route.ElevationProfile,
            AuthorName = route.Author.Name,
            CreatedAt = route.CreatedAt
        };
    }

    public async Task<int> CreateRouteAsync(CreateRouteDto dto, int userId)
    {
        // Создаем LineString из координат
        LineString? trackGeometry = null;
        decimal[] elevationProfile = Array.Empty<decimal>();

        if (dto.Coordinates != null && dto.Coordinates.Count >= 2)
        {
            var coordinates = dto.Coordinates
                .Select(c => new Coordinate(c.Longitude, c.Latitude))
                .ToArray();

            trackGeometry = _geometryFactory.CreateLineString(coordinates);
            trackGeometry.SRID = 4326;

            // Извлекаем высоты, если они есть
            elevationProfile = dto.Coordinates
                .Select(c => (decimal)(c.Elevation ?? 0))
                .ToArray();
        }
        else
        {
            // Если координаты не переданы, создаем пустой LineString
            trackGeometry = _geometryFactory.CreateLineString(Array.Empty<Coordinate>());
            trackGeometry.SRID = 4326;
        }

        var route = new Domain.Entities.Route
        {
            AuthorId = userId,
            Title = dto.Title,
            Description = dto.Description,
            Difficulty = dto.Difficulty,
            Tags = dto.Tags ?? Array.Empty<string>(),
            Season = dto.Season ?? Array.Empty<string>(),
            Region = dto.Region,
            DistanceKm = dto.DistanceKm,
            CoverPhotoUrl = dto.CoverPhotoUrl,
            IsPublished = dto.IsPublished,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            TrackGeometry = trackGeometry,
            ElevationProfile = elevationProfile
        };

        _context.Routes.Add(route);
        await _context.SaveChangesAsync();

        return route.Id;
    }

    public async Task<bool> UpdateRouteAsync(int id, CreateRouteDto dto, int userId)
    {
        var route = await _context.Routes.FindAsync(id);

        if (route == null || route.AuthorId != userId)
            return false;

        route.Title = dto.Title;
        route.Description = dto.Description;
        route.Difficulty = dto.Difficulty;
        route.Tags = dto.Tags ?? Array.Empty<string>();
        route.Season = dto.Season ?? Array.Empty<string>();
        route.Region = dto.Region;
        route.DistanceKm = dto.DistanceKm;
        route.CoverPhotoUrl = dto.CoverPhotoUrl;
        route.IsPublished = dto.IsPublished;
        route.UpdatedAt = DateTime.UtcNow;

        // Обновляем геометрию, если переданы новые координаты
        if (dto.Coordinates != null && dto.Coordinates.Count >= 2)
        {
            var coordinates = dto.Coordinates
                .Select(c => new Coordinate(c.Longitude, c.Latitude))
                .ToArray();

            route.TrackGeometry = _geometryFactory.CreateLineString(coordinates);
            route.TrackGeometry.SRID = 4326;

            route.ElevationProfile = dto.Coordinates
                .Select(c => (decimal)(c.Elevation ?? 0))
                .ToArray();
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteRouteAsync(int id, int userId)
    {
        var route = await _context.Routes.FindAsync(id);

        if (route == null || route.AuthorId != userId)
            return false;

        _context.Routes.Remove(route);
        await _context.SaveChangesAsync();
        return true;
    }
}