using System.CommandLine;
using System.CommandLine.Invocation;

namespace BotConsole.Features.I2C;

public class I2CDetectCommand : Command
{
    public I2CDetectCommand()
        : base(name: "i2cdetect", "equivalent of i2cdetect -y 1")
    {
        
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    public new class Handler : ICommandHandler
    {
        public async Task<int> InvokeAsync(InvocationContext context)
        {
            var detector = new I2CDetector();

            await detector.Execute();
            
            return 0;
        }
    }
}