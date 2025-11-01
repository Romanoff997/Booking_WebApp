using Booking_WebApp.Data.Entities;
using Booking_WebApp.Data.Services;

using Microsoft.AspNetCore.Mvc;


namespace Booking_WebApp.Controllers;

[ApiController]
[Route("[controller]")]
public class BookingController : Controller
{
    private readonly BookingService _roomService;
    private readonly RegexBookingService _regexService;

    public BookingController(BookingService roomService, RegexBookingService regexService)
    {
        _roomService = roomService;
        _regexService = regexService;
    }

    [HttpGet("RunBooking")]
    public IActionResult RunBooking()
    {
        _roomService.RunBooking();
        return Ok("RunBooking выполнен");
    }

    [HttpPost("CancelBooking")]
    public IActionResult CancelBooking([FromBody] MessageDto messageDto)
    {
        var numbers = _regexService.NumbersFromString(messageDto.Message);
        _roomService.CancelBooking(numbers);

        return Ok("Отмена потоков: " + messageDto.Message);
    }

    [HttpGet("StatisticBooking")]
    public async Task<IActionResult> StatisticBooking()
    {
        var response = await _roomService.StatisticBooking();
        return Ok(response);
    }
}

