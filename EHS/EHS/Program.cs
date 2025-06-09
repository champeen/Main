using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using EHS.Data;
using EHS.Utilities;
using EHS.Models;

var builder = WebApplication.CreateBuilder(args);
//Initialization.Initialize(builder);

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
builder.Services.AddDbContext<EHSContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("EHSContext") ?? throw new InvalidOperationException("Connection string 'EHSContext' not found.")));
builder.Services.AddDbContext<MOCContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("MOCContext") ?? throw new InvalidOperationException("Connection string 'MOCContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    SeedData.Initialize(services);
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
