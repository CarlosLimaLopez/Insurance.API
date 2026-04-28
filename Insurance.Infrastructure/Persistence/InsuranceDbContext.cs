using Insurance.Application.Interfaces;
using Insurance.Domain.Insuring.Entities;
using Insurance.Domain.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Insurance.Infrastructure.Persistence;

public class InsuranceDbContext : DbContext, IUnitOfWork, IReadDbContext
{
    public DbSet<Booking> Bookings { get; set; } = null!;
    public DbSet<InsuredBooking> InsuredBookings { get; set; } = null!;

    IQueryable<Booking> IReadDbContext.Bookings => Bookings;
    IQueryable<InsuredBooking> IReadDbContext.InsuredBookings => InsuredBookings;

    public InsuranceDbContext(DbContextOptions<InsuranceDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(x => x.Reference);
            entity.Property(e => e.CheckIn).HasColumnType("date");
            entity.Property(e => e.CheckOut).HasColumnType("date");
        });

        modelBuilder.Entity<InsuredBooking>().HasKey(x => x.Id);

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}