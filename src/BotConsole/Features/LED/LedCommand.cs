using System.CommandLine;
using System.CommandLine.Invocation;

namespace BotConsole.Features.LED;

public class LedCommand : Command
{
    public LedCommand() : base("led", "Manipulate Led Ring")
    {
    }
    
    // ReSharper disable once UnusedType.Global
    // ReSharper disable once ClassNeverInstantiated.Global
    public new class Handler : ICommandHandler
    {
        private readonly LedRing _ledRing = new LedRing();
        
        public async Task<int> InvokeAsync(InvocationContext context)
        {
            await _ledRing.Animate(LedMode.Wipe, null, context.GetCancellationToken());
            await _ledRing.Animate(LedMode.TheaterChase, null, context.GetCancellationToken());
            await _ledRing.Animate(LedMode.Rainbow, null, context.GetCancellationToken());
            await _ledRing.Animate(LedMode.RainbowCycle, null, context.GetCancellationToken());
            await _ledRing.Animate(LedMode.TheaterChaseRainbow, null, context.GetCancellationToken());
            return 0;
        }
    }
}