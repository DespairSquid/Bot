using System.CommandLine;
using System.CommandLine.Invocation;

namespace BotConsole.Features.Sonar;

public class SonarCommand : Command
{
    public SonarCommand() : base("sonar", "Get distance to nearest object.")
    {
    }
    
    // ReSharper disable once UnusedType.Global
    // ReSharper disable once ClassNeverInstantiated.Global
    public new class Handler : ICommandHandler
    {
        private readonly Sonar _sonar = new();
        public async Task<int> InvokeAsync(InvocationContext context)
        {
            while (!context.GetCancellationToken().IsCancellationRequested)
            {
                var distance = await _sonar.GetDistaince();
                Console.WriteLine($"distance: {(distance.HasValue ? $"{distance.Value.Centimeters} cm" : string.Empty)}");

                await Task.Delay(1000);
            }
            
            return 0;
        }
    }
}