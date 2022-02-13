namespace BotConsole.Features.ADC;

public class Adc
{
    private readonly ADS7830 _ads7380 = new();

    public async Task<bool> Detect()
    {
        return await _ads7380.Detect();
    }
    public async Task<VoltageReading> GetVoltage()
    {
        var voltage = new VoltageReading
        {
            Voltage1 = Math.Round(await _ads7380.ReadRaw(0)/255*5*3, 2),
            Voltage2 = Math.Round(await _ads7380.ReadRaw(4)/255*5*3, 2)
        };

        return voltage;
    }
}