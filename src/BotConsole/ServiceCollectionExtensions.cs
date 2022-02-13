using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace BotConsole;

public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Registers Serilog from configuration.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddSerilog(this IServiceCollection services)
    {
        Log.Logger = CreateLogger(services);
        return services;
    }

    private static Logger CreateLogger(IServiceCollection services)
    {
        var scope = services.BuildServiceProvider();
        var loggerConfiguration = new LoggerConfiguration().ReadFrom.Configuration(scope.GetRequiredService<IConfiguration>());

        loggerConfiguration.MinimumLevel.Override("Microsoft", LogEventLevel.Warning);
        
        return loggerConfiguration.CreateLogger();
    }

}