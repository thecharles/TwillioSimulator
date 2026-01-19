using Microsoft.EntityFrameworkCore;
using TwilioSimulator.Data;
using TwilioSimulator.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure in-memory database
builder.Services.AddDbContext<SimulatorDbContext>(options =>
    options.UseInMemoryDatabase("TwilioSimulator"));

// Register HttpClient
builder.Services.AddHttpClient();

// Register services
builder.Services.AddScoped<ICallbackService, CallbackService>();
builder.Services.AddScoped<ISmsService, SmsService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
