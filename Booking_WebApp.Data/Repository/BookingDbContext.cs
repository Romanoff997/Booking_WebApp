using Booking_WebApp.Data.Entities;
using Booking_WebApp.Data.Repository.EF;

using Microsoft.EntityFrameworkCore;

namespace Booking_WebApp.Data.Repository;

public class DataManager
{
    public IRoomModelRepository _roomRepository { get; set; }

    public DataManager(IRoomModelRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }
}
public class BookingDbContext : DbContext
{
    public DbSet<Room> Rooms { get; set; }

    public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Room>().HasKey(r => r.Id);

        base.OnModelCreating(modelBuilder);
    }
}
