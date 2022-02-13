using System.Device.I2c;

namespace BotConsole.Features.ADC;

public class ADS7830
{
    private readonly I2cDevice _i2CDevice;

    private const int BusId = 1;
    
    private const int DeviceId = 0x48;

    public ADS7830()
    {
        var connectionSettings = new I2cConnectionSettings(BusId, DeviceId);
        
        _i2CDevice = I2cDevice.Create(connectionSettings);
    }

    public async Task<bool> Detect()
    {
        try
        {
            _i2CDevice.WriteByte(0);

            return await Task.FromResult(true);
        }
        catch (Exception e)
        {
            Console.WriteLine($"detection error: {e.Message}");
            return await Task.FromResult(false);
        }
    }

    public async Task<double> ReadRaw(int channel)
    {
        var cmd = Convert.ToByte(0x84 | ((channel << 2 | channel >> 1) & 0x07) << 4);
        _i2CDevice.WriteByte(cmd);
        var readBuffer = _i2CDevice.ReadByte();

        return await Task.FromResult(Convert.ToDouble(readBuffer));
    }
}