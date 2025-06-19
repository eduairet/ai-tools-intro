using Microsoft.EntityFrameworkCore;
using EventManagementApi.Models.User;
using EventManagementApi.Models.Event;
using EventManagementApi.Models.EventRegistration;

namespace EventManagementApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<EventRegistration> EventRegistrations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure your entities here
        modelBuilder.Entity<EventRegistration>()
            .HasIndex(er => new { er.EventId, er.UserId })
            .IsUnique();
    }
} 