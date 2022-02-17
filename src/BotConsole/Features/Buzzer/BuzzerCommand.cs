using System.CommandLine;
using System.CommandLine.Invocation;
using BotConsole.Features.Buzzer.Songs;

namespace BotConsole.Features.Buzzer;

public class BuzzerCommand : Command
{
    public BuzzerCommand() : base("buzzer", "Play the piezo buzzer.")
    {
    }
    
    // ReSharper disable once ClassNeverInstantiated.Global
    public new class Handler : ICommandHandler
    {
        public async Task<int> InvokeAsync(InvocationContext context)
        {
            using (var player = new MelodyPlayer(17))
            {
                player.Play(AlphabetSong.Song, 100);
            }
            
            await Task.Delay(10);
            return 0;
        }
    }
}