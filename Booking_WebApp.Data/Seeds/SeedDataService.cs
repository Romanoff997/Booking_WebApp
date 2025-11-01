using Booking_WebApp.Data.Entities;
using Booking_WebApp.Data.Repository;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Booking_WebApp.Data.Seeds;

public class SeedDataService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public SeedDataService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<BookingDbContext>();
            if (!dbContext.Rooms.Any())
            {
                await SeedDataAsync(dbContext);
            }
        }
    }
    private async Task SeedDataAsync(BookingDbContext context)
    {
        var rooms = new List<Room>
            {
                new Room {  IsBooked = false },
                new Room {  IsBooked = false },
                new Room {  IsBooked = false },
                new Room {  IsBooked = false },
                new Room {  IsBooked = false },
                new Room {  IsBooked = false },
                new Room {  IsBooked = false },
                new Room {  IsBooked = false },
                new Room {  IsBooked = false },
                new Room {  IsBooked = false },
                new Room {  IsBooked = false },
                new Room {  IsBooked = false }//,
                //new Room {  IsBooked = false },
                //new Room {  IsBooked = false },
                //new Room {  IsBooked = false },
                //new Room {  IsBooked = false },
                //new Room {  IsBooked = false },
                //new Room {  IsBooked = false },
                //new Room {  IsBooked = false },
                //new Room {  IsBooked = false },
                //new Room {  IsBooked = false }
            };
        await context.Rooms.AddRangeAsync(rooms);
        await context.SaveChangesAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
