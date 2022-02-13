using System.Device.I2c;
using System.Text;

namespace BotConsole.Features.I2C;

public class I2CDetector
{
    private const int FirstAddress = 0x08;

    private const int LastAddress = 0x77;

    private const int BusId = 1;
    
    public async Task Execute()
    {
        Console.WriteLine($"BusId={BusId}, FirstAddress={FirstAddress} (0x{FirstAddress:X2}), LastAddress={LastAddress} (0x{LastAddress:X2})");

        await ScanDeviceAddressesOnI2cBus();
    }
    
    // ReSharper disable once UnusedMethodReturnValue.Local
    private static Task ScanDeviceAddressesOnI2cBus()
    {
        // ReSharper disable once CollectionNeverQueried.Local
        var devices = new List<DeviceRow>();
        var stringBuilder = new StringBuilder();

        stringBuilder.Append("     0  1  2  3  4  5  6  7  8  9  a  b  c  d  e  f");
        stringBuilder.Append(Environment.NewLine);

        var deviceRow = 0;
        for (var startingRowAddress = 0; startingRowAddress < 128; startingRowAddress += 16)
        {
            stringBuilder.Append($"{startingRowAddress:x2}: ");  // Beginning of row.

            if (startingRowAddress > 0)
            {
                deviceRow = startingRowAddress / 16;
            }

            var deviceRowItem = new DeviceRow() {RowNumber = $"{deviceRow:x2}"};

            for (var rowAddress = 0; rowAddress < 16; rowAddress++)
            {
                var deviceAddress = startingRowAddress + rowAddress;

                // Skip the unwanted addresses.
                if (deviceAddress is < FirstAddress or > LastAddress)
                {
                    deviceRowItem.Devices.Add(string.Empty);
                    stringBuilder.Append("   ");
                    continue;
                }

                var connectionSettings = new I2cConnectionSettings(BusId, deviceAddress);
                using var i2CDevice = I2cDevice.Create(connectionSettings);
                try
                {
                    var deviceByte = i2CDevice.ReadByte();  // Only checking if device is present.
                    Console.WriteLine($"device byte({deviceByte}) for address({deviceAddress:x2})");
                    stringBuilder.Append($"{deviceAddress:x2} ");
                    deviceRowItem.Devices.Add($"{deviceAddress:x2}");
                }
                catch
                {
                    stringBuilder.Append("-- ");
                    deviceRowItem.Devices.Add("--");
                }
            }

            stringBuilder.Append(Environment.NewLine);
            
            Console.WriteLine($"number of devices for row({deviceRowItem.RowNumber}): {deviceRowItem.Devices.Count}");
            
            devices.Add(deviceRowItem);
        }

        Console.WriteLine(stringBuilder.ToString());

        return Task.CompletedTask;
    }
}