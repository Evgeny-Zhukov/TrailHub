using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrailHub.API.Application.DTOs;
using TrailHub.API.Application.Services;
using TrailHub.API.Domain.Enums;

namespace TrailHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RouteController : ControllerBase
{
    private readonly IRouteService _routeService;
    private readonly IMapper _mapper;

    public RouteController(IRouteService routeService)
    {
        _routeService = routeService;
    }

    /// <summary>
    /// Получить список опубликованных маршрутов с фильтрацией
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetRoutes([FromQuery] string? region, [FromQuery] DifficultyLevel? difficulty)
    {
        var routes = await _routeService.GetPublishedRoutesAsync(region, difficulty);
        return Ok(routes);
    }

    /// <summary>
    /// Получить детали маршрута по ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRoute(int id)
    {
        var route = await _routeService.GetRouteByIdAsync(id);
        if (route == null)
            return NotFound(new { message = "Маршрут не найден или не опубликован" });

        return Ok(route);
    }

    /// <summary>
    /// Создать новый маршрут (требует аутентификацию)
    /// </summary>
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateRoute([FromBody] CreateRouteDto dto)
    {
        // Получаем ID пользователя из JWT токена
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized(new { message = "Не удалось определить пользователя" });
        }

        var routeId = await _routeService.CreateRouteAsync(dto, userId);

        return CreatedAtAction(nameof(GetRoute), new { id = routeId }, new
        {
            message = "Маршрут успешно создан",
            routeId = routeId
        });
    }

    /// <summary>
    /// Обновить маршрут (только автор)
    /// </summary>
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRoute(int id, [FromBody] CreateRouteDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized(new { message = "Не удалось определить пользователя" });
        }

        var success = await _routeService.UpdateRouteAsync(id, dto, userId);

        if (!success)
            return NotFound(new { message = "Маршрут не найден или у вас нет прав на редактирование" });

        return Ok(new { message = "Маршрут обновлен" });
    }

    /// <summary>
    /// Удалить маршрут (только автор)
    /// </summary>
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRoute(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized(new { message = "Не удалось определить пользователя" });
        }

        var success = await _routeService.DeleteRouteAsync(id, userId);

        if (!success)
            return NotFound(new { message = "Маршрут не найден или у вас нет прав на удаление" });

        return Ok(new { message = "Маршрут удален" });
    }

    [HttpPost("upload-gpx")]
    [Authorize]
    [RequestSizeLimit(10 * 1024 * 1024)] // 10 MB
    public async Task<IActionResult> UploadGpx(IFormFile file, [FromForm] string? name, [FromForm] string? description)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Файл не найден.");

        if (!Path.GetExtension(file.FileName).Equals(".gpx", StringComparison.OrdinalIgnoreCase))
            return BadRequest("Неверный формат файла. Требуется .gpx");

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized(new { message = "Не удалось определить пользователя" });
        }

        try
        {
            using var stream = file.OpenReadStream();
            var route = await _routeService.CreateRouteFromGpxAsync(userId, stream, name, description);

            return Ok(new
            {
                message = "Маршрут успешно создан из GPX",
                routeId = route.Id
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ошибка обработки файла: {ex.Message}");
        }
    }
}