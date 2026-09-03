using TrailHub.API.Application.DTOs;
using TrailHub.API.Domain.Enums;

namespace TrailHub.API.Application.Services;

public interface IRouteService
{
    Task<IEnumerable<RouteListDto>> GetPublishedRoutesAsync(string? region = null, DifficultyLevel? difficulty = null);
    Task<RouteDetailDto?> GetRouteByIdAsync(int id);
    Task<int> CreateRouteAsync(CreateRouteDto dto, int userId);
    Task<bool> UpdateRouteAsync(int id, CreateRouteDto dto, int userId);
    Task<bool> DeleteRouteAsync(int id, int userId);
}