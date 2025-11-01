using Booking_WebApp.Client.Models;

using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;
using System.Text;

using Newtonsoft.Json;

namespace Booking_WebApp.Client.Controllers;



public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly HttpClient _httpClient;
    public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient("BookingApi");
    }

    public IActionResult Index()
    {
        return View(new FrontViewModel() {Count = 20, Numbers = "1, 4, 10"});
    }


    public IActionResult Privacy()
    {
        return View();
    }

    [HttpGet]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RunBooking()
    {
        try
        {
            var httpResponseMessage = await _httpClient.GetAsync("RunBooking");
            var response = await httpResponseMessage.Content.ReadAsStringAsync();
            return Json(response);
        }
        catch (Exception ex)
        {
        }

        return BadRequest("No succes");
    }

    [ValidateAntiForgeryToken]
    [HttpPost("CancelBooking")]
    public async Task<IActionResult> CancelBooking(FrontViewModel model)
    {
        try
        {
            var json = JsonConvert.SerializeObject(new { Message = model.Numbers});
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var httpResponseMessage = await _httpClient.PostAsync("CancelBooking", data);
            var response = await httpResponseMessage.Content.ReadAsStringAsync();
            return Json(response);

        }
        catch (Exception ex)
        {
        }

        return BadRequest("No succes");
    }

    [ValidateAntiForgeryToken]
    [HttpGet("StatisticBooking")]
    public async Task<IActionResult> StatisticBooking()
    {
        try
        {
            var httpResponseMessage = await _httpClient.GetAsync("StatisticBooking");
            var response = await httpResponseMessage.Content.ReadAsStringAsync();
            return Json(response);
        }
        catch (Exception ex)
        {
        }

        return BadRequest("No succes");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}
