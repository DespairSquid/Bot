namespace BotConsole.Features.ADC;

public class VoltageReading
{
    public DateTime Time { get; } = DateTime.Now;

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public double Voltage1 { get; set; }

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public double Voltage2 { get; set; }
}