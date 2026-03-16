namespace Chytra_domacnost.Models;

/// <summary>
/// Místnost v domácnosti
/// </summary>
public class Room
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int HouseholdId { get; set; }
}
