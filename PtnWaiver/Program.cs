using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PtnWaiver.Data;
using PtnWaiver.Utilities;
using System.Diagnostics;

try
{
    var builder = WebApplication.CreateBuilder(args);
    Initialization.Initialize(builder);

    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    builder.Services.AddDbContext<MocContext>(options => options
    .UseNpgsql(Initialization.ConnectionStringMoc)
    //.UseLazyLoadingProxies()
    );
    builder.Services.AddDbContext<PtnWaiverContext>(options => options
    .UseNpgsql(Initialization.ConnectionStringPtnWaiver)
    //.UseLazyLoadingProxies()
    );
    //builder.Services.AddDbContext<PtnWaiverContext>(options =>
    //    options.UseSqlServer(builder.Configuration.GetConnectionString("PtnWaiverContext") ?? throw new InvalidOperationException("Connection string 'PtnWaiverContext' not found.")));

    // Add services to the container.
    builder.Services.AddControllersWithViews();
    builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        //SeedData.Initialize(services);
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
    //app.MapControllerRoute(
    //    name: "PTN",
    //    pattern: "PTN/{*MesLookup}", defaults: new { controller = "PTN", action = "MesLookup" });

    app.MapControllerRoute(
        name: "ptn",
        pattern: "{controller=PTN}/{action=MesLookup}/{docId?}");
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception ex)
{
    // LOG IN WINDOWS EVENT LOG
    using (EventLog eventLog = new EventLog("Application"))
    {
        StackTrace st = new StackTrace(ex, true);
        StackFrame frame = st.GetFrame(0);
        string fileName = frame.GetFileName();
        string methodName = frame.GetMethod().Name;
        int line = frame.GetFileLineNumber();

        eventLog.Source = "Application";
        eventLog.WriteEntry(
            "ERROR: Process Test Notification Application (PTN)" + Environment.NewLine +
            "CLASS: " + fileName + Environment.NewLine +
            "METHOD: " + methodName + Environment.NewLine +
            "LINE NUMBER: " + line.ToString() + Environment.NewLine +
            "MESSAGE: " + ex.Message,
            EventLogEntryType.Error);
    }

    // SEND TEAMS ERROR MESSAGE
    Initialization.TeamsErrorProvider.SendMessage(
    "ERROR: Management of Change Application (MoC) </br>" +
    "MESSAGE: " + ex.Message + "</br> " +
    "INNER EXCEPTION: " + ex.InnerException?.ToString());


    // SEND EMAIL ERROR MESSAGE
    Initialization.EmailProviderSmtp.SendMessage(
        "ERROR: Process Test Notification Application (PTN)",
        "ERROR: Process Test Notification (PTN) </br>" +
            "MESSAGE: " + ex.Message + "</br> " +
            "INNER EXCEPTION: " + ex.InnerException?.ToString() + "</br></br>",
        Initialization.EmailError,
        null,
        null,
        "High");
    //"",
    //null);
}
