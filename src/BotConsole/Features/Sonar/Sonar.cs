using System.Device.Gpio;
using Iot.Device.Hcsr04;
using UnitsNet;

namespace BotConsole.Features.Sonar;

public class Sonar
{
    public async Task<Length?> GetDistaince()
    {
        using var gpioController = new GpioController(PinNumberingScheme.Logical);
        var sonar = new Hcsr04(gpioController, 27, 22);
        if (sonar.TryGetDistance(out var distance))
        {
            return await Task.FromResult(distance);
        }

        Console.WriteLine("failed to get distance");
        return await Task.FromResult<Length?>(null);
    }
}