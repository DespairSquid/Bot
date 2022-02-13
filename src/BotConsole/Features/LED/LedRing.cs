using System.Drawing;
using rpi_ws281x;

namespace BotConsole.Features.LED;

public class LedRing
{
    private const int PinCount = 8;

    private readonly List<Color> _defaultColors = new()
    {
        Color.Red,
        Color.Orange,
        Color.Yellow,
        Color.Green,
        Color.Blue,
        Color.Purple
    };
    
    public async Task Animate(LedMode mode, Color? color, CancellationToken cancellationToken)
    {
        var settings = Settings.CreateDefaultSettings();
        settings.AddController(PinCount, Pin.Gpio18, StripType.WS2811_STRIP_GRB);
        
        using var device = new WS281x(settings);
        switch (mode)
        {
            case LedMode.None:
                break;
            case LedMode.Rainbow:
                await Rainbow(device, color, cancellationToken);
                break;
            case LedMode.RainbowCycle:
                await RainbowCycle(device, color, cancellationToken);
                break;
            case LedMode.TheaterChase:
                await TheatreChase(device, color, cancellationToken);
                break;
            case LedMode.TheaterChaseRainbow:
                await TheaterChaseRainbow(device, color, cancellationToken);
                break;
            case LedMode.Wipe:
                await Wipe(device, color, cancellationToken);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }
        
        device.Reset();
    }

    private async Task Wipe(WS281x device, Color? color, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            device.Reset();
            return;
        }
        if (color.HasValue)
        {
            await Wipe(device, color.Value, cancellationToken);
        }
        else
        {
            foreach (var defaultColor in _defaultColors)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    device.Reset();
                    return;
                }
                await Wipe(device, defaultColor, cancellationToken);
                device.Reset();
                await Task.Delay(1000, cancellationToken);
            }
        }
        
        device.Reset();
    }

    private static async Task Wipe(WS281x device, Color color, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested) return;
        var controller = device.GetController();
        foreach (var led in controller.LEDs)
        {
            if (cancellationToken.IsCancellationRequested) return;
            led.Color = color;
            //must force the render or the LEDs don't light
            device.Render(true);

            // wait for a minimum of 5 milliseconds
            var waitPeriod = (int)Math.Max(500.0 / controller.LEDCount, 5.0); 

            await Task.Delay(waitPeriod, cancellationToken);
        }
        
        foreach (var led in controller.LEDs)
        {
            if (cancellationToken.IsCancellationRequested) return;
            led.Color = Color.Black;
            //must force the render or the LEDs don't light
            device.Render(true);

            // wait for a minimum of 5 milliseconds
            var waitPeriod = (int)Math.Max(500.0 / controller.LEDCount, 5.0); 

            await Task.Delay(waitPeriod, cancellationToken);
        }
    }
    
    private async Task TheatreChase(WS281x device, Color? color, CancellationToken cancellationToken, int iterations = 10)
    {
        if (cancellationToken.IsCancellationRequested) return;
        var controller = device.GetController();
        if (color.HasValue)
        {
            for (var iteration = 0; iteration < iterations; iteration++)
            {
                for (var j = 0; j < 3; j++)
                {
                    for (var k = 0; k < PinCount; k += 3)
                    {
                        if (k + j < PinCount) controller.LEDs.ElementAt(j + k).Color = color.Value;
                    }

                    device.Render(true);
                    await Task.Delay(100, cancellationToken);
                    foreach (var led in controller.LEDs)
                    {
                        led.Color = Color.Black;
                    }
                    device.Render(true);
                }
            }
        }
        else
        {
            foreach (var defaultColor in _defaultColors)
            {
                for (var iteration = 0; iteration < iterations; iteration++)
                {
                    for (var j = 0; j < 3; j++)
                    {
                        for (var k = 0; k < PinCount; k += 3)
                        {
                            if (k + j < PinCount) controller.LEDs.ElementAt(j + k).Color = defaultColor;
                        }

                        device.Render(true);
                        await Task.Delay(100, cancellationToken);
                        foreach (var led in controller.LEDs)
                        {
                            led.Color = Color.Black;
                        }
                        device.Render(true);
                    }
                }
            }
        }
    }
    
    private static async Task Rainbow(WS281x device, Color? color, CancellationToken cancellationToken, int iterations = 1)
    {
        if (cancellationToken.IsCancellationRequested) return;
        var controller = device.GetController();
        for (var step = 0; step < 255 * iterations; step++)
        {
            for (var pinNumber = 0; pinNumber < PinCount; pinNumber++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    device.Reset();
                    return;
                }
                controller.LEDs.ElementAt(pinNumber).Color = Wheel((step + pinNumber) & 255);
            }

            if (cancellationToken.IsCancellationRequested)
            {
                device.Reset();
                return;
            }
            device.Render(true);
            await Task.Delay(10, cancellationToken);
        }
        
        device.Reset();
    }
    
    private static async Task RainbowCycle(WS281x device, Color? color, CancellationToken cancellationToken, int iterations = 1)
    {
        if (cancellationToken.IsCancellationRequested) return;
        var controller = device.GetController();
        for (var step = 0; step < 255 * iterations; step++)
        {
            for (var pinNumber = 0; pinNumber < PinCount; pinNumber++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    device.Reset();
                    return;
                }
                controller.LEDs.ElementAt(pinNumber).Color = Wheel((pinNumber * 255 / PinCount + step) & 255);
                device.Render(true);
                await Task.Delay(10, cancellationToken);
            }
        }
        
        device.Reset();
    }
    
    private static async Task TheaterChaseRainbow(WS281x device, Color? color, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested) return;
        var controller = device.GetController();
        for (var step = 0; step < 255; step++)
        {
            for (var jump = 0; jump < 3; jump++)
            {
                for (var pinNumber = 0; pinNumber < PinCount; pinNumber += 3)
                {
                    if (jump + pinNumber < PinCount)
                        controller.LEDs.ElementAt(jump + pinNumber).Color = Wheel((pinNumber + step) % 255);
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    device.Reset();
                    return;
                }
                device.Render(true);
                await Task.Delay(100, cancellationToken);

                foreach (var led in controller.LEDs)
                {
                    led.Color = Color.Black;
                }
                if (cancellationToken.IsCancellationRequested)
                {
                    device.Reset();
                    return;
                }
                device.Render(true);
            }
        }
        
        device.Reset();
    }

    private static Color Wheel(int position)
    {
        switch (position)
        {
            case < 85:
                return Color.FromArgb(position * 3, 255 - position * 3, 0);
            case < 170:
                position -= 85;
                return Color.FromArgb(255 - position * 3, 0, position * 3);
            default:
                position -= 170;
                return Color.FromArgb(0, position * 3, 255 - position * 3);
        }
    }

}