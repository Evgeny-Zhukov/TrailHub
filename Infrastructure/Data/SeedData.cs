using Bogus;
using NetTopologySuite.Geometries;
using TrailHub.API.Domain.Entities;
using TrailHub.API.Domain.Enums;
using TrailHub.API.Infrastructure.Data;

namespace Trail.API.Infrastructure.Data;

public static class SeedData
{
    // Добавляем параметры для управления объемом
    public static void Initialize(AppDbContext context, int userCount = 500, int routeCount = 10000)
    {
        if (context.Users.Any() || context.Routes.Any())
        {
            return; // DB already seeded
        }

        var geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);

        var regions = new[] { "Москва", "Санкт-Петербург", "Кавказ", "Алтай", "Урал", "Карелия", "Камчатка" };
        var tagsPool = new[] { "горы", "лес", "река", "озеро", "город", "прогулка", "сложный", "история", "зимний", "семейный" };
        var seasons = new[] { "лето", "весна", "осень", "зима" };

        // Фиксированный Seed для воспроизводимости тестов
        Randomizer.Seed = new Random(8675309);

        // 1. Генерация пользователей
        var userFaker = new Faker<User>()
            .RuleFor(u => u.Login, f => f.Internet.UserName())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.PasswordHash, f => f.Internet.Password())
            .RuleFor(u => u.Name, f => f.Name.FullName())
            .RuleFor(u => u.CreatedAt, f => f.Date.Past(2));

        var users = userFaker.Generate(userCount);
        context.Users.AddRange(users);
        context.SaveChanges(); // Сохраняем, чтобы получить Id

        // 2. Генерация маршрутов
        var routeFaker = new Faker<TrailHub.API.Domain.Entities.Route>()
            .RuleFor(r => r.AuthorId, f => f.PickRandom(users).Id)
            .RuleFor(r => r.Title, f => f.Lorem.Sentence(3).TrimEnd('.'))
            .RuleFor(r => r.Difficulty, f => f.PickRandom<DifficultyLevel>())
            .RuleFor(r => r.Tags, f => f.Make(f.Random.Int(1, 5), () => f.PickRandom(tagsPool)).ToArray())
            .RuleFor(r => r.Season, f => f.Make(f.Random.Int(1, 4), () => f.PickRandom(seasons)).ToArray())
            .RuleFor(r => r.Description, f => f.Lorem.Paragraphs(2))
            .RuleFor(r => r.Region, f => f.PickRandom(regions))
            .RuleFor(r => r.DistanceKm, f => (decimal)Math.Round(f.Random.Double(1.0, 80.0), 1))
            .RuleFor(r => r.IsPublished, f => f.Random.Bool(0.95f)) // 95% опубликовано
            .RuleFor(r => r.CreatedAt, f => f.Date.Past(1))
            .RuleFor(r => r.UpdatedAt, (f, r) => r.CreatedAt.AddDays(f.Random.Int(0, 30)))
            .RuleFor(r => r.TrackGeometry, f =>
            {
                // Генерируем реалистичный LineString (трек) в пределах РФ/СНГ
                var pointsCount = f.Random.Int(10, 100);
                var baseLon = f.Random.Double(30.0, 150.0); // Долгота
                var baseLat = f.Random.Double(45.0, 65.0);  // Широта

                var coords = new Coordinate[pointsCount];
                for (int i = 0; i < pointsCount; i++)
                {
                    // Небольшой дрифт координат для создания линии
                    coords[i] = new Coordinate(
                        baseLon + f.Random.Double(-0.5, 0.5),
                        baseLat + f.Random.Double(-0.5, 0.5)
                    );
                }
                return geometryFactory.CreateLineString(coords);
            })
            .RuleFor(r => r.ElevationProfile, f =>
            {
                var count = f.Random.Int(20, 100);
                var baseHeight = f.Random.Double(100, 2500);
                var profile = new decimal[count];
                for (int i = 0; i < count; i++)
                {
                    profile[i] = (decimal)Math.Round(baseHeight + f.Random.Double(-50, 50), 1);
                }
                return profile;
            });

        var routes = routeFaker.Generate(routeCount);

        // Совет: Для вставки >10k записей используйте ExecuteInsert (EF Core 7+) 
        // или Npgsql COPY, чтобы не грузить ChangeTracker.
        context.Routes.AddRange(routes);
        context.SaveChanges();

        Console.WriteLine($"Seeded {users.Count} users and {routes.Count} routes.");
    }
}