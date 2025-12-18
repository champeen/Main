using EHS;
using EHS.Data;
using EHS.Models;
using EHS.Services.Chemicals;
using EHS.Utilities;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.EntityFrameworkCore;
using System.Net;

try
{
    var builder = WebApplication.CreateBuilder(args);
    Initialization.Initialize(builder);

    // setup database connections
    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    builder.Services.AddDbContext<EHSContext>(options => options.UseNpgsql(Initialization.ConnectionStringEhs, npgsqlOptions =>
        npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "ehs")));
    builder.Services.AddDbContext<MOCContext>(options => options.UseNpgsql(Initialization.ConnectionStringMoc));
    builder.Services.AddDbContext<EHSContext>(opts =>
        opts
            .UseNpgsql(
                builder.Configuration.GetConnectionString("EHSContext"),
                npg => npg.CommandTimeout(60) // client-side command timeout
            )
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking) // optional but good for read-mostly
    );

    // Add services to the container.
    builder.Services.AddHttpClient<PubChemClient>();
    builder.Services.AddHttpClient<EHS.Services.Chemicals.OshaNioshClient>(c =>
    {
        c.Timeout = TimeSpan.FromSeconds(15);
        c.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124 Safari/537.36");
        c.DefaultRequestHeaders.TryAddWithoutValidation("Accept",
            "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
        c.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "en-US,en;q=0.9");
    })
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        var handler = new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            UseCookies = true,
            CookieContainer = new CookieContainer(),   // <-- add this
            AllowAutoRedirect = true
        };
        return handler;
    });


    builder.Services.AddScoped<PugViewParser>();
    builder.Services.AddScoped<ChemicalIngestService>();
    builder.Services.AddControllersWithViews();

    if (Initialization.Environment != "Development")
        builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate();

    var app = builder.Build();
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        SeedData.Initialize(services);
    }

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error/Generic");
        app.UseHsts();
    }

    app.UseAuthentication();
    app.UseAuthorization();
    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseStatusCodePagesWithReExecute("/Home/StatusCode", "?code={0}");

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception ex)
{
    ErrorHandling.HandleException(ex);
}