using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Parsing;
using BotConsole;
using BotConsole.Features.ADC;
using BotConsole.Features.Buzzer;
using BotConsole.Features.Camera;
using BotConsole.Features.I2C;
using BotConsole.Features.LED;
using BotConsole.Features.Sonar;
using Microsoft.Extensions.Hosting;
using Serilog;

var runner = BuildCommandLine()
    .UseHost(_ => CreateHostBuilder(args), (builder) => builder
        .UseSerilog()
        .ConfigureServices((_, services) =>
        {
            services.AddSerilog();
        })
        .UseCommandHandler<AdcCommand, AdcCommand.Handler>()
        .UseCommandHandler<CameraCommand, CameraCommand.Handler>()
        .UseCommandHandler<I2CDetectCommand, I2CDetectCommand.Handler>()
        .UseCommandHandler<LedCommand, LedCommand.Handler>()
        .UseCommandHandler<SonarCommand, SonarCommand.Handler>()
        .UseCommandHandler<BuzzerCommand, BuzzerCommand.Handler>()).UseDefaults().Build();

return await runner.InvokeAsync(args);

static CommandLineBuilder BuildCommandLine()
{
    var root = new RootCommand();
    root.AddCommand(BuildBotCommands());
    root.Handler = CommandHandler.Create(() => root.Invoke("-h"));
    return new CommandLineBuilder(root);

    static Command BuildBotCommands()
    {
        var command = new Command("bot", "Bot management")
        {
            new AdcCommand(),
            new CameraCommand(),
            new I2CDetectCommand(),
            new LedCommand(),
            new SonarCommand(),
            new BuzzerCommand()
        };
        return command;
    }
}

static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args);