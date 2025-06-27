using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using EventManagementApi.Data;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Routing;

namespace AiGeneratedApi.Tests.Integration
{
    // Using a custom Startup class for integration tests
    public class CustomWebApplicationFactory : WebApplicationFactory<CustomWebApplicationFactory.Startup>
    {
        public class Startup
        {
            public IConfiguration Configuration { get; }

            public Startup(IConfiguration configuration)
            {
                Configuration = configuration;
            }

            public void ConfigureServices(IServiceCollection services)
            {
                services.AddControllers().AddApplicationPart(typeof(EventManagementApi.Controllers.EventsController).Assembly);
                services.AddEndpointsApiExplorer();

                // Add in-memory database
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("IntegrationTestDb");
                });

                // Register repositories
                services.AddScoped<EventManagementApi.Repositories.RepositoryEvents.IRepositoryEvents,
                    EventManagementApi.Repositories.RepositoryEvents.RepositoryEvents>();
                services.AddScoped<EventManagementApi.Repositories.RepositoryUsers.IRepositoryUsers,
                    EventManagementApi.Repositories.RepositoryUsers.RepositoryUsers>();
                services.AddScoped<EventManagementApi.Repositories.RepositoryEventsRegistrations.IRepositoryEventsRegistrations,
                    EventManagementApi.Repositories.RepositoryEventsRegistrations.RepositoryEventsRegistrations>();

                // Add our custom AutoMapper
                services.AddSingleton<IMapper>(_ => MockMapper.CreateMockMapper());

                // Configure authentication
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = false,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes("testSecretKeyWithMinimum32Characters1234567890"))
                    };
                });
            }

            public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            {
                app.UseRouting();
                app.UseAuthentication();
                app.UseAuthorization();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
            }
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                // Add test configuration
                var configItems = new List<KeyValuePair<string, string?>>
                {
                    new KeyValuePair<string, string?>("JwtSettings:Secret", "testSecretKeyWithMinimum32Characters1234567890"),
                    new KeyValuePair<string, string?>("JwtSettings:Issuer", "test-issuer"),
                    new KeyValuePair<string, string?>("JwtSettings:Audience", "test-audience")
                };

                config.AddInMemoryCollection(configItems);
            });

            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContext registration if already registered
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add in-memory database
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

                db.Database.EnsureCreated();
            });
        }
    }
}
