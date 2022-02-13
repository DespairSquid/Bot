using System.CommandLine;
using System.CommandLine.Invocation;

namespace BotConsole.Features.ADC;

public class AdcCommand : Command
{
    public AdcCommand() : base("adc", "Get voltages.")
    {
    }
    
    // ReSharper disable once ClassNeverInstantiated.Global
    public new class Handler : ICommandHandler
    {
        private readonly Adc _adc = new Adc();
        
        public async Task<int> InvokeAsync(InvocationContext context)
        {
            if (!await _adc.Detect()) return 0;
            while (!context.GetCancellationToken().IsCancellationRequested)
            {
                var reading = await _adc.GetVoltage();
                Console.WriteLine($"{reading.Time}: voltage 1: {reading.Voltage1} voltage 2: {reading.Voltage2}");

                await Task.Delay(1000);
            }
            return 0;
        }
    }
}