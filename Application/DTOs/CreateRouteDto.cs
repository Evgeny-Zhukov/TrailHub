using System.ComponentModel.DataAnnotations;
using TrailHub.API.Domain.Enums;

namespace TrailHub.API.Application.DTOs;

public class CreateRouteDto
{
    [Required(ErrorMessage = "Название обязательно")]
    [StringLength(200, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Описание обязательно")]
    [StringLength(5000, MinimumLength = 10)]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Сложность обязательна")]
    public DifficultyLevel Difficulty { get; set; }

    public string[]? Tags { get; set; } = Array.Empty<string>();

    public string[]? Season { get; set; } = Array.Empty<string>();

    [Required(ErrorMessage = "Регион обязателен")]
    [StringLength(100)]
    public string Region { get; set; } = string.Empty;

    [Range(0.1, 1000, ErrorMessage = "Дистанция должна быть от 0.1 до 1000 км")]
    public decimal DistanceKm { get; set; }

    public string? CoverPhotoUrl { get; set; }

    // Координаты трека
    public List<CoordinateDto>? Coordinates { get; set; }

    [Required]
    public bool IsPublished { get; set; } = true;
}