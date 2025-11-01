using System.ComponentModel.DataAnnotations;

namespace Booking_WebApp.Client.Models;

public class FrontViewModel
{
    [Range(0, 20, ErrorMessage = "Count должен быть от 0 до 20")]
    public int Count { get; set; }

    public string Numbers { get; set; }
    public string SystemMessage { get; set; }
}
