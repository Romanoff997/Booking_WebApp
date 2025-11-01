using Booking_Data.Entities;
using Booking_Data.Repository.EF;
using Microsoft.EntityFrameworkCore;

namespace Booking_Data.Repository;

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

        //modelBuilder.Entity<Room>().HasData(
        //    new Room {  IsBooked = false },
        //    new Room {  IsBooked = false },
        //    new Room {  IsBooked = false },
        //    new Room {  IsBooked = false },
        //    new Room {  IsBooked = false },
        //    new Room {  IsBooked = false },
        //    new Room {  IsBooked = false },
        //    new Room {  IsBooked = false },
        //    new Room {  IsBooked = false },
        //    new Room {  IsBooked = false },
        //    new Room {  IsBooked = false },
        //    new Room {  IsBooked = false },
        //    new Room {  IsBooked = false },
        //    new Room {  IsBooked = false },
        //    new Room {  IsBooked = false },
        //    new Room {  IsBooked = false },
        //    new Room {  IsBooked = false },
        //    new Room {  IsBooked = false },
        //    new Room {  IsBooked = false },
        //    new Room {  IsBooked = false },
        //    new Room {  IsBooked = false }
        //);

        base.OnModelCreating(modelBuilder);
    }
}
