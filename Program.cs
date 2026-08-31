using Microsoft.EntityFrameworkCore;
using Trail.API.Infrastructure.Data;
using TrailHub.API.Application.Interfaces;
using TrailHub.API.Application.Services;
using TrailHub.API.Infrastructure.Data;

namespace TrailHub.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Добавляем контроллеры и Swagger
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // 2. Настраиваем БД с поддержкой PostGIS (NetTopologySuite)
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.UseNetTopologySuite(); // Включает магию PostGIS
                }));

            // 3. Регистрируем наши сервисы
            builder.Services.AddScoped<IRouteService, RouteService>();

            // 4. Разрешаем CORS (чтобы Angular мог обращаться к API)
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular", policy =>
                {
                    policy.WithOrigins("http://localhost:4200") // Адрес вашего Angular приложения
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            var app = builder.Build();

            // 5. Middleware pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowAngular");
            app.UseAuthorization();
            app.MapControllers();
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.Migrate(); // Применяет миграции, если они не применены

                // Заполняем тестовыми данными
                SeedData.Initialize(dbContext);
            }
            app.Run();
        }
    }
}
