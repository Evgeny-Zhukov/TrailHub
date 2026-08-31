using TrailHub.API.Application.DTOs;
using TrailHub.API.Domain.Enums;

namespace TrailHub.API.Application.Interfaces
{
    public interface IRouteService
    {
        Task<IEnumerable<RouteListDto>> GetPublishedRoutesAsync(string? region = null, DifficultyLevel? difficulty = null);
        Task<RouteDetailDto?> GetRouteByIdAsync(int id);
    }
}
