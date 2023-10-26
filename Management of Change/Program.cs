using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Management_of_Change.Data;
using Management_of_Change.Models;
using Management_of_Change.Utilities;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Diagnostics;
using Management_of_Change.Provider;

try
{
    var builder = WebApplication.CreateBuilder(args);
    Initialization.Initialize(builder);

    // Test Sending Email
    //Initialization.EmailProviderSmtp.SendMessage("TEST EMAIL subject", "<h1>Hello</h1></br>This is <h2>Michael James Wilson II</h2></br></br>", "michael.wilson@sksiltron.com;", null, null/*, "", null*/);
    // Test Sending Teams Message
    //Initialization.TeamsErrorProvider.SendMessage("This is a test <br/><br/> EOM");

    // Add services to the container.
    // USE SQL SERVER //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //builder.Services.AddDbContext<Management_of_ChangeContext>(options =>
    //    options.UseSqlServer(builder.Configuration.GetConnectionString("Management_of_ChangeContext") ?? throw new InvalidOperationException("Connection string 'Management_of_ChangeContext' not found.")));
    // USE POSTGRESQL //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    builder.Services.AddDbContext<Management_of_ChangeContext>(options => options.UseNpgsql(Initialization.ConnectionString));
    //builder.Services.AddDbContext<Management_of_ChangeContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQLprd")));
    builder.Services.AddControllersWithViews();
    builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

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
        //pattern: "{controller=ChangeRequests}/{action=Index}/{id?}");
        pattern: "{controller=Home}/{action=Index}/{id?}/{rec?}");

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
            "ERROR: Management of Change Application (MoC)" + Environment.NewLine +
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
    "INNER EXCEPTION: " + ex.InnerException);

    // SEND EMAIL ERROR MESSAGE
    Initialization.EmailProviderSmtp.SendMessage(
        "ERROR: Management of Change Application (MoC)",
        "ERROR: Management of Change Application (MoC) </br>" +
            "MESSAGE: " + ex.Message + "</br> " +
            "INNER EXCEPTION: " + ex.InnerException,
        "michael.wilson@sksiltron.com;",
        null,
        null,
        "High");
        //"",
        //null);
}