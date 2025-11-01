using Booking_WebApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Booking_WebApp.Data.Repository.EF;

public interface IRoomModelRepository
{
    public void UpdateRoom(Room room);
    public Room? GetRoom();
    public Task ClearRooms();
}

public class EFRoomModelRepository : IRoomModelRepository
{
    private readonly BookingDbContext _context;
    public EFRoomModelRepository(BookingDbContext context)
    {
        _context = context;
    }
    public Room? GetRoom()
    {
        return _context.Rooms.FirstOrDefault(x => !x.IsBooked);

    }
    public void UpdateRoom(Room room)
    {
        _context.Rooms.Update(room);
        _context.SaveChanges();
    }
    public async Task ClearRooms()
    {
        var rooms = _context.Rooms;
        await rooms.ForEachAsync(x => x.IsBooked = false);
        _context.Rooms.UpdateRange(rooms);
        await _context.SaveChangesAsync();
    }
}

