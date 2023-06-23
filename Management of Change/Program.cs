using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Management_of_Change.Data;
using Management_of_Change.Models;

var builder = WebApplication.CreateBuilder(args);

// USE SQL SERVER.....
builder.Services.AddDbContext<Management_of_ChangeContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Management_of_ChangeContext") ?? throw new InvalidOperationException("Connection string 'Management_of_ChangeContext' not found.")));
// USE POSTGRESQL.....
//AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
////builder.Services.AddDbContext<Management_of_ChangeContext>(options => options.UseNpgsql(Initialization.connectionString));
//builder.Services.AddDbContext<Management_of_ChangeContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQLdev")));

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
    pattern: "{controller=ChangeRequests}/{action=Index}/{id?}");

app.Run();
