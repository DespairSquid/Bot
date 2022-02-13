//using HexapodBot;
using Microsoft.AspNetCore.SignalR;

namespace Server.Server.Hubs;

public class HexapodHub : Hub
{
    /*
    public async Task SendMessage(HexapodCommand command, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", command, message);
    }
    */
}