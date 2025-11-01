
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient("BookingApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:44370/Booking/");
});
builder.Services.AddRazorPages();
builder.Services.AddAntiforgery(options => options.HeaderName = "RequestVerificationToken");
var app = builder.Build();


app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");

app.Run();
