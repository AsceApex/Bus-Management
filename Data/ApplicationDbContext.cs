// Data/ApplicationDbContext.cs
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BusReservation.API.Models;

namespace BusReservation.API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Booking> Bookings { get; set; } = default!;
        public DbSet<Bus> Buses { get; set; } = default!;
        public DbSet<BusRoute> BusRoutes { get; set; } = default!;
        public DbSet<Schedule> Schedules { get; set; } = default!;
        public DbSet<Driver> Drivers { get; set; } = default!;
        public DbSet<Reservation> Reservations { get; set; } = default!;
        public DbSet<BusSeat> BusSeats { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // UNIQUE indexes  ✅ use modelBuilder (NOT builder)
            modelBuilder.Entity<Reservation>()
                .HasIndex(r => new { r.ScheduleId, r.SeatNumber })
                .IsUnique();

            modelBuilder.Entity<BusSeat>()
                .HasIndex(s => new { s.BusId, s.SeatNumber })
                .IsUnique();

            // BusRoute -> Routes table
            modelBuilder.Entity<BusRoute>(entity =>
            {
                entity.ToTable("Routes");
                entity.HasKey(r => r.Id);
                entity.Property(r => r.StartLocation).IsRequired().HasMaxLength(100);
                entity.Property(r => r.EndLocation).IsRequired().HasMaxLength(100);
            });

            // Schedule relationships
            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.HasKey(s => s.Id);

                entity.HasOne(s => s.BusRoute)
                      .WithMany()
                      .HasForeignKey(s => s.BusRouteId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.Bus)
                      .WithMany()
                      .HasForeignKey(s => s.BusId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.Driver)
                      .WithMany()
                      .HasForeignKey(s => s.DriverId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // Reservation -> Schedule
            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.HasKey(r => r.Id);

                entity.HasOne(r => r.Schedule)
                      .WithMany()
                      .HasForeignKey(r => r.ScheduleId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
