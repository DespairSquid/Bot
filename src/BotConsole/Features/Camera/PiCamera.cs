using MMALSharp;
using MMALSharp.Common;
using MMALSharp.Handlers;

namespace BotConsole.Features.Camera;

public class PiCamera
{
    public async Task<string> TakePicture()
    {
        var cam = MMALCamera.Instance;

        using var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/", "jpg");
        await cam.TakePicture(imgCaptureHandler, MMALEncoding.JPEG, MMALEncoding.I420);

        return $"{imgCaptureHandler.GetFilepath()}{imgCaptureHandler.GetFilename()}";
    }
}