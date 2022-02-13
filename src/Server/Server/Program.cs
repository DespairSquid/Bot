using Microsoft.AspNetCore.ResponseCompression;
using Serilog;
using Serilog.Enrichers;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Server.Server.Hubs;

const string loggerTemplate = @"{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u4}]<{ThreadId}> [{SourceContext:l}] {Message:lj}{NewLine}{Exception}";
var baseDir = AppDomain.CurrentDomain.BaseDirectory;
var logfile = Path.Combine(baseDir, "logs", "log.txt");

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .MinimumLevel.Verbose()
    .Enrich.With(new ThreadIdEnricher())
    .Enrich.FromLogContext()
    .WriteTo.Console(LogEventLevel.Verbose, loggerTemplate, theme: AnsiConsoleTheme.Literate)
    .WriteTo.File(logfile, LogEventLevel.Verbose, loggerTemplate,
        rollingInterval: RollingInterval.Day, retainedFileCountLimit: 90)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSignalR();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Logging.AddSerilog(Log.Logger);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();
app.MapHub<HexapodHub>("/hexapod-comms");
app.MapHub<LoggingHub>("/logging");
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();