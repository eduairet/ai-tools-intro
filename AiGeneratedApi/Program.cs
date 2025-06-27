using Microsoft.EntityFrameworkCore;
using EventManagementApi.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using EventManagementApi.Repositories.RepositoryUsers;
using EventManagementApi.Repositories.RepositoryEvents;
using EventManagementApi.Repositories.RepositoryEventsRegistrations;
using EventManagementApi.Config;
using EventManagementApi.Models;
using EventManagementApi.Shared.Helpers;
using Microsoft.AspNetCore.Identity;
using Scalar.AspNetCore;

public partial class Program
{
    // Added for integration testing
    private static bool _isTestEnvironment;

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Check if this is running in a test environment
        _isTestEnvironment = builder.Environment.IsEnvironment("Testing");

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        // Add Swagger/OpenAPI
        builder.Services.AddEndpointsApiExplorer();

        // Add Entity Framework with SQLite, but skip database registration if running in testing environment
        // since the test factory will configure its own database
        if (!_isTestEnvironment)
        {
            if (!builder.Environment.IsEnvironment("Test"))
            {
                builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
            }
            else
            {
                builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase("TestDb"));
            }
        }

        // Register Repositories
        builder.Services.AddScoped<IRepositoryUsers, RepositoryUsers>();
        builder.Services.AddScoped<IRepositoryEvents, RepositoryEvents>();
        builder.Services.AddScoped<IRepositoryEventsRegistrations, RepositoryEventsRegistrations>();

        // Add JWT Authentication and Authorization
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = Helpers.Jwt.GetTokenValidationParameters(builder.Configuration);
            });

        // Add AutoMapper
        builder.Services.AddAutoMapper(typeof(AutoMapperConfig));

        // Add CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        // Add ASP.NET Core Identity
        builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        var app = builder.Build();

        // Ensure database is created and migrated
        if (!_isTestEnvironment && !app.Environment.IsEnvironment("Test"))
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Database.EnsureCreated();
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.UseHttpsRedirection();

        app.UseCors("AllowAll");

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseStaticFiles();

        app.MapControllers();

        app.Run();
    }
}