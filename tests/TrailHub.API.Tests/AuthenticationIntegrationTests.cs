using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TrailHub.API.Infrastructure.Data;
using FluentAssertions;
using TrailHub.API.Application.DTOs.Auth;

namespace TrailHub.API.Tests;

public class AuthenticationIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AuthenticationIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add In-Memory database for testing
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });
            });
        });
    }

    [Fact]
    public async Task Register_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClient();
        var registerDto = new RegisterDto
        {
            Login = "testuser",
            Email = "test@example.com",
            Password = "Password123!",
            Name = "Test User"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/register", registerDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.Login.Should().Be("testuser");
    }

    [Fact]
    public async Task Login_WithCorrectCredentials_ShouldReturnToken()
    {
        // Arrange
        var client = _factory.CreateClient();
        var loginDto = new LoginDto
        {
            Login = "testuser",
            Password = "Password123!"
        };

        // Pre-register user
        var registerDto = new RegisterDto
        {
            Login = "loginuser",
            Email = "login@example.com",
            Password = "Password123!",
            Name = "Login User"
        };
        await client.PostAsJsonAsync("/api/auth/register", registerDto);

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", loginDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.Login.Should().Be("loginuser");
    }

    [Fact]
    public async Task Login_WithWrongCredentials_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();
        var loginDto = new LoginDto
        {
            Login = "nonexistent",
            Password = "wrongpassword"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", loginDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
