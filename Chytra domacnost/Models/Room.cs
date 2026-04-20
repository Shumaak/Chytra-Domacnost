namespace Chytra_domacnost.Models;
public class Room
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int HouseholdId { get; set; }
}
