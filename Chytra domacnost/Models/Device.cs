using Chytra_domacnost.Enums;

namespace Chytra_domacnost.Models;

public class Device
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DeviceType Type { get; set; }
    public DeviceState State { get; set; }
    public int PowerConsumption { get; set; }
    public int? CurrentValue { get; set; }
    public int RoomId { get; set; }
}
