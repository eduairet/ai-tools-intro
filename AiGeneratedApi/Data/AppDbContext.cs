using Microsoft.EntityFrameworkCore;
using EventManagementApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace EventManagementApi.Data;

public partial class AppDbContext : IdentityDbContext<User>
{
    // Default constructor
    public AppDbContext()
    {
    }

    // Constructor that accepts DbContextOptions
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // DbSets for your entities
    public virtual DbSet<Event> Events { get; set; }
    public virtual DbSet<EventRegistration> EventRegistrations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasOne(d => d.Owner).WithMany(p => p.Events).HasForeignKey(d => d.OwnerId);
        });

        modelBuilder.Entity<EventRegistration>(entity =>
        {
            entity.HasIndex(e => new { e.EventId, e.UserId }, "IX_EventRegistrations_EventId_UserId").IsUnique();

            entity.HasOne(d => d.Event).WithMany(p => p.EventRegistrations).HasForeignKey(d => d.EventId);

            entity.HasOne(d => d.User).WithMany(p => p.EventRegistrations).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email, "IX_Users_Email").IsUnique();

            entity.HasIndex(e => e.UserName, "IX_Users_UserName").IsUnique();

            entity.Property(e => e.LockoutEnabled).HasColumnType("BOOLEAN");
            entity.Property(e => e.PhoneNumberConfirmed).HasColumnType("BOOLEAN");
            entity.Property(e => e.TwoFactorEnabled).HasColumnType("BOOLEAN");
        });
    }
}