using Booking_Data.Entities;

namespace Booking_Data.Repository.EF;

public interface IRoomModelRepository
{
    public Task UpdateRoom(Room room);
    public Room? GetRoom();
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
        var efe = _context.Rooms.ToList();
        return _context.Rooms.FirstOrDefault(x => !x.IsBooked);

    }
    public async Task UpdateRoom(Room room)
    {
        _context.Rooms.Update(room);
        await _context.SaveChangesAsync();
    }
}

