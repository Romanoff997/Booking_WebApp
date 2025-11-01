using System.Text.RegularExpressions;

namespace Booking_WebApp.Data.Services;

public class RegexBookingService
{
    public int[] NumbersFromString(string text)
    {
        Regex regex = new Regex(@"\d+\s*(\D|\Z)");
        List<int> numbers = new();
        foreach (var reg in regex.Matches(text))
        {
            var nubmberText = Regex.Match(reg.ToString() ?? "", @"\d+").Value;
            int number;
            if (int.TryParse(nubmberText, out number))
            {
                numbers.Add(number);
            }
        }
        return numbers.ToArray();
    }
}
