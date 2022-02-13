namespace BotConsole.Features.I2C;

public class DeviceRow
{
    public string RowNumber { get; init; } = null!;

    public List<string> Devices { get; set; } = new();
}