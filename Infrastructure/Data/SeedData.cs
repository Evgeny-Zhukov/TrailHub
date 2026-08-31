using NetTopologySuite.Geometries;
using TrailHub.API.Domain.Entities;
using TrailHub.API.Domain.Enums;
using TrailHub.API.Infrastructure.Data;

namespace Trail.API.Infrastructure.Data;

public static class SeedData
{
    public static void Initialize(AppDbContext context)
    {
        // Проверяем, есть ли уже данные
        if (context.Users.Any() || context.Routes.Any())
        {
            return; // DB already seeded
        }

        // Создаем тестового пользователя
        var user = new User
        {
            Login = "testuser",
            Email = "test@example.com",
            PasswordHash = "hashed_password_placeholder", // В реальности здесь будет хэш
            Name = "Иван Тестов",
            AvatarUrl = null,
            CreatedAt = DateTime.UtcNow
        };

        context.Users.Add(user);
        context.SaveChanges();

        // Создаем тестовые маршруты
        var geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);

        // Маршрут 1: Простой маршрут вокруг Москвы
        var route1Points = new[]
        {
            new Coordinate(37.6173, 55.7558), // Красная площадь
            new Coordinate(37.6200, 55.7600), // Парк Горького
            new Coordinate(37.6250, 55.7650), // Воробьевы горы
        };
        var route1Geometry = geometryFactory.CreateLineString(route1Points);

        var route1 = new TrailHub.API.Domain.Entities.Route
        {
            AuthorId = user.Id,
            Title = "Прогулка по центру Москвы",
            Difficulty = DifficultyLevel.Level1,
            Tags = new[] { "город", "прогулка", "история" },
            Season = new[] { "лето", "весна", "осень" },
            Description = "Легкая прогулка по историческому центру Москвы. Подходит для всей семьи.",
            Region = "Москва",
            DistanceKm = 5.2m,
            CoverPhotoUrl = null,
            IsPublished = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            TrackGeometry = route1Geometry,
            ElevationProfile = new decimal[] { 120, 125, 130 }
        };

        // Маршрут 2: Горный маршрут (Кавказ)
        var route2Points = new[]
        {
            new Coordinate(42.4375, 43.3500), // Домбай
            new Coordinate(42.4400, 43.3550), // Перевал
            new Coordinate(42.4450, 43.3600), // Озеро
            new Coordinate(42.4500, 43.3650), // Вершина
        };
        var route2Geometry = geometryFactory.CreateLineString(route2Points);

        var route2 = new TrailHub.API.Domain.Entities.Route
        {
            AuthorId = user.Id,
            Title = "Домбай - озеро Азек",
            Difficulty = DifficultyLevel.Level4,
            Tags = new[] { "горы", "озеро", "сложный" },
            Season = new[] { "лето" },
            Description = "Сложный горный маршрут с набором высоты 1200 метров. Требуется хорошая физическая подготовка.",
            Region = "Кавказ",
            DistanceKm = 18.5m,
            CoverPhotoUrl = null,
            IsPublished = true,
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            UpdatedAt = DateTime.UtcNow.AddDays(-2),
            TrackGeometry = route2Geometry,
            ElevationProfile = new decimal[] { 1600, 1800, 2100, 2400, 2800 }
        };

        // Маршрут 3: Неопубликованный (для проверки фильтрации)
        var route3Points = new[]
        {
            new Coordinate(30.3150, 59.9390), // Санкт-Петербург
            new Coordinate(30.3200, 59.9450),
        };
        var route3Geometry = geometryFactory.CreateLineString(route3Points);

        var route3 = new TrailHub.API.Domain.Entities.Route
        {
            AuthorId = user.Id,
            Title = "Черновик: Питер",
            Difficulty = DifficultyLevel.Level1,
            Tags = new[] { "город" },
            Season = new[] { "лето" },
            Description = "Этот маршрут еще не опубликован.",
            Region = "Санкт-Петербург",
            DistanceKm = 3.0m,
            IsPublished = false, // Не опубликован!
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            TrackGeometry = route3Geometry,
            ElevationProfile = new decimal[] { 10, 12 }
        };

        context.Routes.AddRange(route1, route2, route3);
        context.SaveChanges();
    }
}