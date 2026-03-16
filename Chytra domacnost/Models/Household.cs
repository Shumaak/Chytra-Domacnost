namespace Chytra_domacnost.Models;

/// <summary>
/// Domácnost
/// </summary>
public class Household
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int UserId { get; set; }
}
