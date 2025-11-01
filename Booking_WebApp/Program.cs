using Booking_WebApp.Data.Repository;
using Booking_WebApp.Data.Repository.EF;
using Booking_WebApp.Data.Seeds;
using Booking_WebApp.Data.Services;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseInMemoryDatabase("BookingDbContext"));
builder.Services.AddSingleton<IHostedService, SeedDataService>();
builder.Services.AddTransient<IRoomModelRepository, EFRoomModelRepository>();
builder.Services.AddTransient<BookingService>();
builder.Services.AddSingleton<RegexBookingService>();


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


