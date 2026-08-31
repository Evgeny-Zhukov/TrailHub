using Microsoft.AspNetCore.Mvc;
using TrailHub.API.Application.Interfaces;
using TrailHub.API.Domain.Enums;

namespace Trail.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoutesController : ControllerBase
{
    private readonly IRouteService _routeService;

    public RoutesController(IRouteService routeService)
    {
        _routeService = routeService;
    }

    // GET: api/routes?region=Кавказ&difficulty=3
    [HttpGet]
    public async Task<IActionResult> GetRoutes([FromQuery] string? region, [FromQuery] DifficultyLevel? difficulty)
    {
        var routes = await _routeService.GetPublishedRoutesAsync(region, difficulty);
        return Ok(routes);
    }

    // GET: api/routes/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRoute(int id)
    {
        var route = await _routeService.GetRouteByIdAsync(id);
        if (route == null)
            return NotFound(new { message = "Маршрут не найден или не опубликован" });

        return Ok(route);
    }
}