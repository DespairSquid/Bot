using Microsoft.AspNetCore.SignalR;

namespace Server.Server.Hubs;

public class LoggingHub : Hub
{
    private readonly ILogger<LoggingHub> _logger;

    public LoggingHub(ILogger<LoggingHub> logger)
    {
        _logger = logger;
    }
    public async Task SendMessage(string robot, string message)
    {
        _logger.LogInformation("hexapod - {message}", message);
        await Clients.All.SendAsync("ReceiveMessage", robot, message);
    }
}