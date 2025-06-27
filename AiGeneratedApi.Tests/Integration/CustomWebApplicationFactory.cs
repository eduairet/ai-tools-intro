using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EventManagementApi.Data;
using AutoMapper;

namespace AiGeneratedApi.Tests.Integration
{
    // Using WebApplicationFactory pointing to the real Program class
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                // Add test configuration
                var configItems = new List<KeyValuePair<string, string?>>
                {
                    new("JwtSettings:Secret", "testSecretKeyWithMinimum32Characters1234567890"),
                    new("JwtSettings:Issuer", "test-issuer"),
                    new("JwtSettings:Audience", "test-audience"),
                    new("ConnectionStrings:DefaultConnection", "DataSource=:memory:")
                };

                config.AddInMemoryCollection(configItems);
            });

            // Set the environment to "Testing" - this matches our special flag in Program.cs
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContext registration if already registered
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

                if (descriptor is not null) services.Remove(descriptor);

                // Add in-memory database for testing
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("IntegrationTestDb");
                });

                // Add our custom AutoMapper
                services.AddSingleton<IMapper>(_ => MockMapper.CreateMockMapper());

                // Build service provider
                var serviceProvider = services.BuildServiceProvider();

                // Create and initialize the database
                using var scope = serviceProvider.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<AppDbContext>();

                // Ensure database is created
                db.Database.EnsureCreated();
            });
        }
    }
}
