using CarMaintenance.Data;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddViewLocalization() // keeps view-localization available if you want it later
    .AddDataAnnotationsLocalization();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Adding DBContext class for the Database operations.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("dbcs")));

// Configure session with cookie settings
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // ensure HTTPS in prod
});

// make IHttpContextAccessor available (if you need it)
builder.Services.AddHttpContextAccessor();

// Configure supported cultures
var supportedCultures = new[]
{
    new CultureInfo("en"),
    new CultureInfo("ar")
};

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("en");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    // providers: query-string (highest), cookie, accept-language header
    options.RequestCultureProviders = new List<IRequestCultureProvider>
    {
        new QueryStringRequestCultureProvider(),
        new CookieRequestCultureProvider(),
        new AcceptLanguageHeaderRequestCultureProvider()
    };
});

// Add logging with console output (keeps your previous settings)
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders(); // Remove default providers
    logging.AddConsole();    // Add console provider
    logging.SetMinimumLevel(LogLevel.Debug); // Log all levels including Debug
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// IMPORTANT: RequestLocalization early so resources and view rendering get correct culture
var locOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
app.UseRequestLocalization(locOptions);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// session should be before routing if you're using session inside controllers during routing
app.UseSession();

// debug logging for localization on each request (optional)
app.Use(async (context, next) =>
{
    var cultureFeature = context.Features.Get<IRequestCultureFeature>();
    var culture = cultureFeature?.RequestCulture.Culture;
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogDebug("Request Culture at {Time}: {Culture}", DateTime.Now, culture?.Name ?? "Not set");
    await next();
});

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
