using Chytra_domacnost.Enums;

namespace Chytra_domacnost.Models;

/// <summary>
/// Zařízení v místnosti
/// </summary>
public class Device
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DeviceType Type { get; set; }
    public DeviceState State { get; set; }
    public int PowerConsumption { get; set; } // Spotřeba ve W
    public int? CurrentValue { get; set; } // Pro senzory - teplota, jas, atd.
    public int RoomId { get; set; }
}
