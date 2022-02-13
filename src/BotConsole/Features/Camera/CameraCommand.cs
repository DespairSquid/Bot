using System.CommandLine;
using System.CommandLine.Invocation;

namespace BotConsole.Features.Camera;

public class CameraCommand : Command
{
    public CameraCommand() : base("camera", "Command the camera.")
    {
        AddOption(new Option<bool>(new[] { "--picture", "-p" }, "Take a picture."));
    }
    
    public new class Handler : ICommandHandler
    {
        private readonly PiCamera _camera = new();
        
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public bool Picture { get; set; }
        
        public async Task<int> InvokeAsync(InvocationContext context)
        {
            if (!Picture) return 0;
            var path= await _camera.TakePicture();
            context.Console.WriteLine($"picture path: {path}");
            return 0;
        }
    }
}