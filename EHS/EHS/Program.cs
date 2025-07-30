using EHS.Data;
using EHS.Models;
using EHS.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using EHS;
using Microsoft.AspNetCore.Authentication.Negotiate;

try
{
    var builder = WebApplication.CreateBuilder(args);
    Initialization.Initialize(builder);

    // setup database connections
    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    builder.Services.AddDbContext<EHSContext>(options => options.UseNpgsql(Initialization.ConnectionStringEhs, npgsqlOptions =>
        npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "ehs")));
    builder.Services.AddDbContext<MOCContext>(options => options.UseNpgsql(Initialization.ConnectionStringMoc));

    // Add services to the container.
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